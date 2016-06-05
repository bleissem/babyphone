using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using GalaSoft.MvvmLight.Ioc;
using SQLite.Net.Platform.XamarinAndroid;
using Android.Util;
using Android.Content;
using Android.Telephony;

namespace bleissem.babyphone.Droid
{
	[Activity(Label = "bleissem.babyphone", Icon = "@drawable/icon", MainLauncher = true, AlwaysRetainTaskState = true)]
	public class MainActivity : Activity
	{
		private bool DoFinish = false;
        private ExecuteGenericAction<bool> m_SetStatusUI;
        private ExecuteGenericAction<double> m_UpdateAmplitude;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			SetContentView(Resource.Layout.Main);

            this.SetStatusUI(false);
			this.InitializeIoC();
			this.InitializeUI();            

			this.SetStartStopUI();
		}

		protected override void OnStart()
		{
			base.OnStart();
		}


        private void AssignCallSettings(string number, SettingsTable.CallTypeEnum callType)
        {
            Settings settings = SimpleIoc.Default.GetInstance<bleissem.babyphone.Settings>();
            settings.NumberToDial = number;
            settings.CallType = callType;
            TextView numberToDial = FindViewById<TextView>(Resource.Id.ContactTextView);
            numberToDial.Text = settings.NumberToDial;

            TextView youreUsing = FindViewById<TextView>(Resource.Id.YouAreUsingTextView);
            youreUsing.Text = this.ConvertCallType(settings.CallType);

        }

		protected override void OnNewIntent(Intent intent)
		{
			base.OnNewIntent(intent);

			string setPhoneNumber = intent.GetStringExtra(Consts.SetIdToCall);
            SettingsTable.CallTypeEnum callType;
            int callTypeInt = intent.GetIntExtra(Consts.SetCallType, -1);
            

			if ((Enum.TryParse<SettingsTable.CallTypeEnum>(callTypeInt.ToString(), out callType)) && (!string.IsNullOrWhiteSpace(setPhoneNumber)))
			{                
                AssignCallSettings(setPhoneNumber,callType);
			}
            
			
			SetStartStopUI();
		}

		public override void OnBackPressed()
		{
			base.OnBackPressed();
			this.Close();
		}

        private string ConvertCallType(SettingsTable.CallTypeEnum callType)
        {
            switch(callType)
            {
                case SettingsTable.CallTypeEnum.Phone:
                    {
                        return this.ApplicationContext.Resources.GetText(Resource.String.Phone); 
                    }
                case SettingsTable.CallTypeEnum.SkypeOut:
                    {
                        return this.ApplicationContext.Resources.GetText(Resource.String.SkypeOut); 
                    }
                case   SettingsTable.CallTypeEnum.SkypeUser:
                    {
                        return this.ApplicationContext.Resources.GetText(Resource.String.Skype); 
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }

		private void InitializeUI()
        {
            Settings settings = SimpleIoc.Default.GetInstance<bleissem.babyphone.Settings>();

            string setNumber = this.Intent.GetStringExtra(Consts.SetIdToCall);
            if (!string.IsNullOrWhiteSpace(setNumber))
            {
                settings.NumberToDial = setNumber;
            }

            int callType = this.Intent.GetIntExtra(Consts.SetCallType, -1);
            if (0 < callType)
            {
                settings.CallType = (SettingsTable.CallTypeEnum)callType;
            }

            {
                m_UpdateAmplitude = new ExecuteGenericAction<double>((amp) =>
                {
                    TextView textView = FindViewById<TextView>(Resource.Id.AmpTextView);
                    textView.Text = amp.ToString();
                });
            }

            {
                m_SetStatusUI = new ExecuteGenericAction<bool>(
                (babyphoneSleeps) =>
                {
                    Button startServiceButton = FindViewById<Button>(Resource.Id.ServiceButton);
                    Button chooseContactButton = FindViewById<Button>(Resource.Id.ChooseContactButton);
                    TextView numberToDial = FindViewById<TextView>(Resource.Id.ContactTextView);
                    TextView ampTextView = FindViewById<TextView>(Resource.Id.AmpTextView);
                    Button noiseLevelButton = FindViewById<Button>(Resource.Id.NoiseLevelButton);


                    startServiceButton.Enabled = !babyphoneSleeps;
                    numberToDial.Enabled = !babyphoneSleeps;
                    ampTextView.Enabled = !babyphoneSleeps;
                    chooseContactButton.Enabled = !babyphoneSleeps;
                    noiseLevelButton.Enabled = !babyphoneSleeps;

                    TextView babyPhoneSleepsTextView = FindViewById<TextView>(Resource.Id.StatusTextView);
                    string status = string.Empty;
                    if (babyphoneSleeps)
                    {
                        status = this.ApplicationContext.Resources.GetText(Resource.String.StatusTextSleeping);
                    }
                    else
                    {
                        status = this.ApplicationContext.Resources.GetText(Resource.String.StatusTextNothing);
                    }


                    babyPhoneSleepsTextView.Text = status;
                });
            }

            {
                MainViewModel babyViewModel = SimpleIoc.Default.GetInstance<MainViewModel>();
                babyViewModel.PeriodicNotifications -= MainActivity_PeriodicNotifications;
                babyViewModel.PeriodicNotifications += MainActivity_PeriodicNotifications;

                Button chooseContactButton = FindViewById<Button>(Resource.Id.ChooseContactButton);
                chooseContactButton.Enabled = false | chooseContactButton.Enabled;
                chooseContactButton.Click -= chooseContactButton_Click;
                chooseContactButton.Click += chooseContactButton_Click;

                Button noiseLevelButton = FindViewById<Button>(Resource.Id.NoiseLevelButton);
                noiseLevelButton.Click -= noiseLevelButton_Click;
                noiseLevelButton.Click += noiseLevelButton_Click;

                Button startServiceButton = FindViewById<Button>(Resource.Id.ServiceButton);
                startServiceButton.Text = startServiceButton.Text = this.ApplicationContext.Resources.GetText(Resource.String.StartService);
                startServiceButton.Click -= startServiceButton_Click;
                startServiceButton.Click += startServiceButton_Click;

                TextView numberToDial = FindViewById<TextView>(Resource.Id.ContactTextView);
                numberToDial.Text = settings.NumberToDial;
                numberToDial.TextChanged -= numberToDial_TextChanged;
                numberToDial.TextChanged += numberToDial_TextChanged;

                TextView youreUsing = FindViewById<TextView>(Resource.Id.YouAreUsingTextView);
                youreUsing.Text = this.ConvertCallType(settings.CallType);

                TextView noiseLevel = FindViewById<TextView>(Resource.Id.NoiseLevelTextView);
                noiseLevel.Text = settings.NoiseLevel.ToString();
                noiseLevel.TextChanged -= noiseLevel_TextChanged;
                noiseLevel.TextChanged += noiseLevel_TextChanged;
            }

        }

        private void SetStatusUI(bool babyphoneSleeps)
        {
            if (null == m_SetStatusUI) return;
            m_SetStatusUI.SetValue(babyphoneSleeps);
            this.RunOnUiThread(m_SetStatusUI.Execute);
               
        }

		void noiseLevelButton_Click(object sender, EventArgs e)
		{
			TextView amptextView = FindViewById<TextView>(Resource.Id.AmpTextView);
			TextView noiseLevel = FindViewById<TextView>(Resource.Id.NoiseLevelTextView);
			noiseLevel.Text = amptextView.Text;

			int result = 0;
			if (Int32.TryParse(noiseLevel.Text.ToString(), out result))
			{
				this.SaveNoiseLevel(result);
			}
		}       

		private void InitializeIoC()
		{
			if (SimpleIoc.Default.IsRegistered<bleissem.babyphone.Settings>()) return;

			SimpleIoc.Default.Register<ICreateTimer>(() => new MyTimerCreator(), true);

			var platform = new SQLitePlatformAndroid();
			var dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "Babyphone.Settings.db3");
			Settings settings = new Settings(dbPath, platform);
			SimpleIoc.Default.Register<bleissem.babyphone.Settings>(() => settings, true);

			PhoneCallTimer pct = new PhoneCallTimer(this.ApplicationContext);            
			SimpleIoc.Default.Register<IReactOnCall>(() => pct, true);
			SimpleIoc.Default.Register<INotifiedOnCalling>(() => pct, true);

			ReadContacts rc = new ReadContacts();
			rc.OnFinished += ReadContactsFinished;
			rc.Execute(this);
			SimpleIoc.Default.Register<ReadContacts>(() => rc, true);
						
			CallNumber callNumber = new CallNumber();
			callNumber.Register(this.Dial, this.CanDial);
			SimpleIoc.Default.Register<ICallNumber>(()=>callNumber, true);
		   
			SimpleIoc.Default.Register<IAudioRecorder>(() => new AudioRecorderViewModel(), true);


			SimpleIoc.Default.Register<MainViewModel>(true);
            pct.Register(() => this.SetStatusUI(false));
            pct.Register(OnHangUp);
            

		}

        private void OnHangUp()
        {
            Consts.StartActivityThatAlreadyExist<MainActivity>(this);
        }

		private void ReadContactsFinished()
		{
			Button chooseContactButton = FindViewById<Button>(Resource.Id.ChooseContactButton);
			chooseContactButton.Enabled = true;
		}

		protected override void OnRestart()
		{
			base.OnRestart();
		}


		void startServiceButton_Click(object sender, EventArgs e)
		{
			MainViewModel babyPhoneViewModel = SimpleIoc.Default.GetInstance<MainViewModel>();
			if (babyPhoneViewModel.Phone.IsStarted)
			{
				babyPhoneViewModel.Phone.Stop();
			}
			else if (this.CanStarted())
			{                
				babyPhoneViewModel.Phone.Start();

			}

			this.SetStartStopUI();
		}

		private void SetStartStopUI()
		{
			Button contactButton = FindViewById<Button>(Resource.Id.ChooseContactButton);
			Button startServiceButton = FindViewById<Button>(Resource.Id.ServiceButton);

			if (!SimpleIoc.Default.IsRegistered<MainViewModel>()) return;

			MainViewModel babyPhoneViewModel = SimpleIoc.Default.GetInstance<MainViewModel>();
			if (babyPhoneViewModel.Phone.IsStarted)
			{
				contactButton.Enabled = false;
				startServiceButton.Text = this.ApplicationContext.Resources.GetText(Resource.String.StopService);                 
			}
			else
			{             
				contactButton.Enabled = true;
				startServiceButton.Text = this.ApplicationContext.Resources.GetText(Resource.String.StartService);   
			}
		}

		void chooseContactButton_Click(object sender, EventArgs e)
		{
            Consts.StartActivityWithNoHistory<ChooseSkypeOrTelephoneActivity>(this);
		}

		private void SaveNoiseLevel(int noiselevel)
		{
			SimpleIoc.Default.GetInstance<Settings>().NoiseLevel = noiselevel;
		}

		void noiseLevel_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
		{
			int result = 0;
			if (Int32.TryParse(e.Text.ToString(), out result))
			{
				this.SaveNoiseLevel(result);
			}
		}

		void numberToDial_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
		{
			SimpleIoc.Default.GetInstance<Settings>().NumberToDial = Convert.ToString(e.Text);
		}

		private bool CanStarted()
		{
			Settings setting = SimpleIoc.Default.GetInstance<Settings>();
			return (!string.IsNullOrWhiteSpace(setting.NumberToDial));
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();


            if (this.DoFinish)
            {
                m_SetStatusUI.Dispose();
                m_SetStatusUI = null;

                m_UpdateAmplitude.Dispose();
                m_UpdateAmplitude = null;

			    MainViewModel babyPhoneViewModel = SimpleIoc.Default.GetInstance<MainViewModel>();
			    babyPhoneViewModel.PeriodicNotifications -= MainActivity_PeriodicNotifications;
			    babyPhoneViewModel.Dispose();

			    Button chooseContactButton = FindViewById<Button>(Resource.Id.ChooseContactButton);
			    chooseContactButton.Click -= chooseContactButton_Click;

			    Button startServiceButton = FindViewById<Button>(Resource.Id.ServiceButton);
			    startServiceButton.Click -= startServiceButton_Click;

			    Button noiseLevelButton = FindViewById<Button>(Resource.Id.NoiseLevelButton);
			    noiseLevelButton.Click -= noiseLevelButton_Click;

			    TextView numberToDial = FindViewById<TextView>(Resource.Id.ContactTextView);
			    numberToDial.TextChanged -= numberToDial_TextChanged;

			    TextView noiseLevel = FindViewById<TextView>(Resource.Id.NoiseLevelTextView);
			    noiseLevel.TextChanged -= noiseLevel_TextChanged;

               
				SimpleIoc.Default.GetInstance<ReadContacts>().OnFinished -= ReadContactsFinished;
				SimpleIoc.Default.GetInstance<ICallNumber>().Dispose();
                SimpleIoc.Default.GetInstance<IReactOnCall>().Dispose();
				SimpleIoc.Default.Reset();
			}
		  
		}

		private void MainActivity_PeriodicNotifications(object sender, int e)
		{
			this.UpdateAmplitude(e);
		}

		private void UpdateAmplitude(double amp)
		{
            if (null == m_UpdateAmplitude) return;

            m_UpdateAmplitude.SetValue(amp);
            this.RunOnUiThread(m_UpdateAmplitude.Execute);
		}

		public bool CanDial()
		{
			return (SimpleIoc.Default.GetInstance<IReactOnCall>().State == PhoneState.HangUp);
		}

		public void Dial()
		{            		

			try
			{
                this.SetStatusUI(true);

                Settings setting = SimpleIoc.Default.GetInstance<Settings>();

                if (string.IsNullOrWhiteSpace(setting.NumberToDial)) return;
                string numberToDial = setting.NumberToDial;

                
                SimpleIoc.Default.GetInstance<INotifiedOnCalling>().CallStarts();

                switch (setting.CallType)
                {
                    case SettingsTable.CallTypeEnum.SkypeUser:
                        {
                            /*
                            Intent skype = new Intent("android.intent.action.VIEW");

                            Intent skypeintent = new Intent(Intent.ActionCall);
                            skypeintent.SetClassName("com.skype.raider", "com.skype.raider.Main");
                            skypeintent.SetData(Android.Net.Uri.Parse("skype:" + numberToDial + "?call"));
                            
                            base.StartActivity(skypeintent);
                             */
                            Intent skypeintent = new Intent("android.intent.action.VIEW");
                            skypeintent.SetData(Android.Net.Uri.Parse("skype:" + numberToDial));
                            base.StartActivity(skypeintent);
                            break;
                        }
                    case SettingsTable.CallTypeEnum.SkypeOut:
                        {
                            Intent skypeintent = new Intent(Intent.ActionCall);
                            skypeintent.SetClassName("com.skype.raider", "com.skype.raider.Main");
                            skypeintent.SetData(Android.Net.Uri.Parse("tel:" + numberToDial ));
                            
                            base.StartActivity(skypeintent);
                            break;
                        }
                    case SettingsTable.CallTypeEnum.Phone:
                    default:
                        {

                            Intent phoneIntent = new Intent(Intent.ActionCall);
                            phoneIntent.SetData(Android.Net.Uri.Parse("tel:" + numberToDial));
                            phoneIntent.AddFlags(ActivityFlags.NoUserAction);
                            phoneIntent.AddFlags(ActivityFlags.NoHistory);
                            phoneIntent.AddFlags(ActivityFlags.FromBackground);

                            base.StartActivity(phoneIntent);
                            break;
                        }
                }
			}
			catch
			{

			}
		}


		public void Close()
		{
			this.DoFinish = true;
			base.Finish();
		}
	}
}

