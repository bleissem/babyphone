using System;
using System.Collections.Generic;
using System.Reflection;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using bleissem.babyphone;
using bleissem.babyphone.Droid;
using GalaSoft.MvvmLight.Ioc;

namespace babyphone
{
    [Activity(Label = "@string/ApplicationName", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true, AlwaysRetainTaskState = true, LaunchMode = LaunchMode.SingleInstance)]
    //    [Activity(Icon = "@drawable/icon", MainLauncher = true, AlwaysRetainTaskState = true, LaunchMode = LaunchMode.SingleInstance)]
    public class MainActivity : AppCompatActivity
    {
        private bool DoFinish = false;
        private ExecuteGenericAction<bool> m_SetStatusUI;
        private ExecuteGenericAction<double> m_UpdateAmplitude;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;
            
            this.Title = "babyphone - " + Assembly.GetExecutingAssembly().GetName().Version.ToString();

            List<string> permissions = new List<string>()
            {
                Manifest.Permission.CallPhone,
                Manifest.Permission.ReadPhoneState,
                Manifest.Permission.RecordAudio,
                Manifest.Permission.ReorderTasks,
                Manifest.Permission.ReadContacts,
                Manifest.Permission.ModifyAudioSettings
            };

            foreach (string permission in permissions)
            {
                if (!RequestPermissions(permission))
                {
                    return;
                }
            }

            this.SetStatusUI(false);
            this.InitializeIoC();
            this.InitializeUI();

            this.SetStartStopUI();
        }

        private bool RequestPermissions(string permission)
        {
            bool result = ContextCompat.CheckSelfPermission(this, permission) == (int)Permission.Granted;
            if (!result)
            {
                Toast.MakeText(this, $"Permission: {permission} needed.", ToastLength.Long);
            }

            return result;
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

            string setPhoneNumber = intent.GetStringExtra(IntentFactory.SetIdToCall);
            SettingsTable.CallTypeEnum callType;
            int callTypeInt = intent.GetIntExtra(IntentFactory.SetCallType, -1);


            if ((Enum.TryParse<SettingsTable.CallTypeEnum>(callTypeInt.ToString(), out callType)) && (!string.IsNullOrWhiteSpace(setPhoneNumber)))
            {
                AssignCallSettings(setPhoneNumber, callType);
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
            switch (callType)
            {
                case SettingsTable.CallTypeEnum.Phone:
                    {
                        return this.ApplicationContext.Resources.GetText(Resource.String.Phone);
                    }
                case SettingsTable.CallTypeEnum.SkypeOut:
                    {
                        return this.ApplicationContext.Resources.GetText(Resource.String.SkypeOut);
                    }
                case SettingsTable.CallTypeEnum.SkypeUser:
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

            string setNumber = this.Intent.GetStringExtra(IntentFactory.SetIdToCall);
            if (!string.IsNullOrWhiteSpace(setNumber))
            {
                settings.NumberToDial = setNumber;
            }

            int callType = this.Intent.GetIntExtra(IntentFactory.SetCallType, -1);
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

            var dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "Babyphone.Settings.db3");
            Settings settings = new Settings(dbPath);
            SimpleIoc.Default.Register<bleissem.babyphone.Settings>(() => settings, true);

            AudioManager audioManager = (AudioManager)this.GetSystemService(Context.AudioService);

            Speaker speaker = new Speaker(audioManager);
            SimpleIoc.Default.Register<ISpeaker>(() => speaker, true);

            MuteUnmutePhone mum = new MuteUnmutePhone(audioManager);

            SimpleIoc.Default.Register<IUnMutePhone>(() => mum, true);
            SimpleIoc.Default.Register<IMutePhone>(() => mum, true);

            WindowManagerFlags screenFlags = WindowManagerFlags.ShowWhenLocked | WindowManagerFlags.TurnScreenOn | WindowManagerFlags.KeepScreenOn | WindowManagerFlags.DismissKeyguard;

            TurnOnOffScreenViewModel turnOnOffScreen = new TurnOnOffScreenViewModel(
                new ExecuteAction(() =>
                {
                    try
                    {
                        this.Window.AddFlags(screenFlags);
                        var attributes = new WindowManagerLayoutParams();
                        attributes.CopyFrom(this.Window.Attributes);
                        attributes.ScreenBrightness = 0f;
                        this.Window.Attributes = attributes;
                    }
                    catch
                    {

                    }
                }),
                new ExecuteAction(() =>
                {

                    try
                    {
                        this.Window.ClearFlags(screenFlags);
                        var attributes = new WindowManagerLayoutParams();
                        attributes.CopyFrom(this.Window.Attributes);
                        attributes.ScreenBrightness = -1f;
                        this.Window.Attributes = attributes;
                    }
                    catch
                    {

                    }
                }));


            SimpleIoc.Default.Register<ITurnOnOffScreen>(() => turnOnOffScreen, true);

            PhoneCallTimer pct = new PhoneCallTimer(this.ApplicationContext);
            SimpleIoc.Default.Register<IReactOnCall>(() => pct, true);
            SimpleIoc.Default.Register<INotifiedOnCalling>(() => pct, true);

            ForceHangup fh = new ForceHangup();
            fh.Register(this.ForceHangup);
            SimpleIoc.Default.Register<IForceHangup>(() => fh, true);

            ReadContacts rc = new ReadContacts();
            rc.OnFinished += ReadContactsFinished;
            rc.Execute(this);
            SimpleIoc.Default.Register<ReadContacts>(() => rc, true);

            CallNumber callNumber = new CallNumber();
            callNumber.Register(this.Dial, this.CanDial);
            SimpleIoc.Default.Register<ICallNumber>(() => callNumber, true);

            SimpleIoc.Default.Register<IAudioRecorder>(() => new AudioRecorderViewModel(), true);

            SimpleIoc.Default.Register<MainViewModel>(true);
            pct.Register(OnHangUp);
        }

        private void OnHangUp()
        {
            this.SetStatusUI(false);
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
            Settings settings = SimpleIoc.Default.GetInstance<Settings>();

            if (((settings.CallType == SettingsTable.CallTypeEnum.SkypeUser) || (settings.CallType == SettingsTable.CallTypeEnum.SkypeOut)) && (!IsSkypeInstalled()))
            {
                Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
                alert.SetTitle(GetText(Resource.String.SkypeIsNotInstalledTitle));
                alert.SetMessage(GetText(Resource.String.SkypeIsNotInstalledText));
                alert.SetPositiveButton(GetText(Resource.String.SkypeIsNotInstalledOK), (senderAlert, args) => { });
                Dialog dialog = alert.Create();
                dialog.Show();
                return;
            }

            MainViewModel babyPhoneViewModel = SimpleIoc.Default.GetInstance<MainViewModel>();
            if (babyPhoneViewModel.Phone.IsStarted)
            {
                SimpleIoc.Default.GetInstance<ITurnOnOffScreen>().TurnOn();
                babyPhoneViewModel.Phone.Stop();
            }
            else if (this.CanStarted())
            {
                SimpleIoc.Default.GetInstance<ITurnOnOffScreen>().TurnOff();
                babyPhoneViewModel.Phone.Start();

            }

            this.SetStartStopUI();
        }

        private void SetStartStopUI()
        {
            if (!SimpleIoc.Default.IsRegistered<MainViewModel>()) return;

            Button contactButton = FindViewById<Button>(Resource.Id.ChooseContactButton);
            Button noiseLevelButton = FindViewById<Button>(Resource.Id.NoiseLevelButton);
            Button startServiceButton = FindViewById<Button>(Resource.Id.ServiceButton);
            TextView noiseLevel = FindViewById<TextView>(Resource.Id.NoiseLevelTextView);

            MainViewModel babyPhoneViewModel = SimpleIoc.Default.GetInstance<MainViewModel>();
            bool isStarted = babyPhoneViewModel.Phone.IsStarted;

            if (isStarted)
            {
                noiseLevel.Focusable = false;
                noiseLevel.Enabled = false;
                contactButton.Enabled = false;
                noiseLevelButton.Enabled = false;
                startServiceButton.Text = this.ApplicationContext.Resources.GetText(Resource.String.StopService);
            }
            else
            {
                noiseLevel.Focusable = true;
                noiseLevel.Enabled = true;
                contactButton.Enabled = true;
                noiseLevelButton.Enabled = true;
                startServiceButton.Text = this.ApplicationContext.Resources.GetText(Resource.String.StartService);
            }

            Color color = isStarted ? Color.Argb(255, 0, 0, 0) : Color.Argb(255, 255, 255, 255);
            LinearLayout linearLayout = FindViewById<LinearLayout>(Resource.Id.MainView);
            linearLayout.SetBackgroundColor(color);


            noiseLevel.SetBackgroundColor(color);

            TextView contactTextView = FindViewById<TextView>(Resource.Id.ContactTextView);
            contactTextView.SetBackgroundColor(color);
        }

        void chooseContactButton_Click(object sender, EventArgs e)
        {
            IntentFactory.StartActivityWithNoHistory<ChooseSkypeOrTelephoneActivity>(this);
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

                SimpleIoc.Default.GetInstance<ITurnOnOffScreen>().TurnOn();
                SimpleIoc.Default.GetInstance<ITurnOnOffScreen>().Dispose();

                SimpleIoc.Default.GetInstance<ICallNumber>().Dispose();
                SimpleIoc.Default.GetInstance<IForceHangup>().Dispose();

                SimpleIoc.Default.GetInstance<ReadContacts>().OnFinished -= ReadContactsFinished;

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

        private bool IsSkypeInstalled()
        {
            PackageManager pm = this.PackageManager;
            try
            {
                pm.GetPackageInfo("com.skype.raider", PackageInfoFlags.Activities);
            }
            catch
            {
                return false;
            }
            return true;
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
                bool enableSkypeVideo = setting.IsSkypeVideoEnabled;

                SimpleIoc.Default.GetInstance<INotifiedOnCalling>().CallStarts();

                switch (setting.CallType)
                {
                    case SettingsTable.CallTypeEnum.SkypeUser:
                        {
                            Intent skypeintent = IntentFactory.GetSkypeUserIntent(numberToDial, enableSkypeVideo);
                            base.StartActivity(skypeintent);
                            break;
                        }
                    case SettingsTable.CallTypeEnum.SkypeOut:
                        {
                            Intent skypeintent = IntentFactory.GetSkypeOutIntent(numberToDial);
                            base.StartActivity(skypeintent);
                            break;
                        }
                    case SettingsTable.CallTypeEnum.Phone:
                    default:
                        {
                            try
                            {
                                Intent phoneIntent = IntentFactory.GetCallPhoneIntent(numberToDial, true);
                                base.StartActivity(phoneIntent);
                            }
                            catch (ActivityNotFoundException)
                            {
                                // if setting the package doesn't work, call without it
                                Intent phoneIntent = IntentFactory.GetCallPhoneIntent(numberToDial, false);
                                base.StartActivity(phoneIntent);
                            }

                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long);
            }
        }

        public void ForceHangup()
        {
            Settings setting = SimpleIoc.Default.GetInstance<Settings>();
            var callType = setting.CallType;
            switch (callType)
            {
                case SettingsTable.CallTypeEnum.SkypeUser:
                    {
                        //TODO:
                        break;
                    }
                case SettingsTable.CallTypeEnum.SkypeOut:
                    {
                        //TODO:
                        break;
                    }
                case SettingsTable.CallTypeEnum.Phone:
                default:
                    {
                        using (PhoneManager phoneManager = new PhoneManager(this))
                        {
                            phoneManager.EndCall();
                        }
                        break;
                    }

            }
        }

        public void Close()
        {
            this.DoFinish = true;
            base.Finish();
        }
        //public override bool OnCreateOptionsMenu(IMenu menu)
        //{
        //    MenuInflater.Inflate(Resource.Menu.menu_main, menu);
        //    return true;
        //}

        //public override bool OnOptionsItemSelected(IMenuItem item)
        //{
        //    int id = item.ItemId;
        //    if (id == Resource.Id.action_settings)
        //    {
        //        return true;
        //    }

        //    return base.OnOptionsItemSelected(item);
        //}

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View) sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
	}
}

