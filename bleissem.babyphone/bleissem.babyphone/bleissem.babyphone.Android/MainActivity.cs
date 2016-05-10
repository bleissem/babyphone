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
    [Activity(Label = "bleissem.babyphone", Icon = "@drawable/icon", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private bool DestroyIoC = false;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            this.InitializeIoC();
            this.InitializeUI();

            SimpleIoc.Default.GetInstance<ICallNumber>().Register(this.Dial, this.CanDial);

            this.SetStartStopUI();
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            Settings settings = SimpleIoc.Default.GetInstance<bleissem.babyphone.Settings>();

            string setNumber = intent.GetStringExtra(Consts.SetPhoneNumber);
            if (!string.IsNullOrWhiteSpace(setNumber))
            {
                settings.NumberToDial = setNumber;
                TextView numberToDial = FindViewById<TextView>(Resource.Id.ContactTextView);
                numberToDial.Text = settings.NumberToDial;
            }

            SetStartStopUI();
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            this.Close();
        }

        private void InitializeUI()
        {            
            Settings settings = SimpleIoc.Default.GetInstance<bleissem.babyphone.Settings>();

            string setNumber = this.Intent.GetStringExtra(Consts.SetPhoneNumber);
            if (!string.IsNullOrWhiteSpace(setNumber))
            {
                settings.NumberToDial = setNumber;
            }

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

            TextView noiseLevel = FindViewById<TextView>(Resource.Id.NoiseLevelTextView);
            noiseLevel.Text = settings.NoiseLevel.ToString();
            noiseLevel.TextChanged -= noiseLevel_TextChanged;
            noiseLevel.TextChanged += noiseLevel_TextChanged;

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

            var phoneListener = new PhoneCallListener(this.ApplicationContext);
            TelephonyManager tm = this.GetSystemService(Context.TelephonyService) as TelephonyManager;
            tm.Listen(phoneListener, PhoneStateListenerFlags.CallState);

            SimpleIoc.Default.Register<IReactOnHangUp>(() => phoneListener, true);

            ReadContacts rc = new ReadContacts();
            rc.OnFinished += ReadContactsFinished;
            rc.Execute(this);
            SimpleIoc.Default.Register<ReadContacts>(() => rc, true);
                        
            SimpleIoc.Default.Register<ICallNumber>(() => new CallNumber(), true);
            SimpleIoc.Default.Register<IAudioRecorder>(() => new AudioRecorderViewModel(), true);


            SimpleIoc.Default.Register<MainViewModel>(true);

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
            Consts.StartActivity<ContactsMasterActivitiy>(this);
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

            MainViewModel babyPhoneViewModel = SimpleIoc.Default.GetInstance<MainViewModel>();
            babyPhoneViewModel.PeriodicNotifications -= MainActivity_PeriodicNotifications;

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


            if (this.DestroyIoC)
            {
                SimpleIoc.Default.GetInstance<ReadContacts>().OnFinished -= ReadContactsFinished;
                SimpleIoc.Default.Reset();
            }
          
        }

        private void MainActivity_PeriodicNotifications(object sender, int e)
        {
            this.UpdateAmplitude(e);
        }

        private void UpdateAmplitude(double amp)
        {
            this.RunOnUiThread(() =>
            {
                TextView textView = FindViewById<TextView>(Resource.Id.AmpTextView);
                textView.Text = amp.ToString();
            });
        }

        public bool CanDial()
        {
            return (SimpleIoc.Default.GetInstance<IReactOnHangUp>().State == PhoneState.HangUp);
        }

        public void Dial()
        {            
            Settings setting = SimpleIoc.Default.GetInstance<Settings>();

            if (string.IsNullOrWhiteSpace(setting.NumberToDial)) return;
            string numberToDial = setting.NumberToDial;

            Intent phoneIntent = new Intent(Intent.ActionCall);
            phoneIntent.SetData(Android.Net.Uri.Parse("tel:" + numberToDial));
            phoneIntent.AddFlags(ActivityFlags.NoUserAction);
            phoneIntent.AddFlags(ActivityFlags.NoHistory);
            phoneIntent.AddFlags(ActivityFlags.ClearWhenTaskReset);
            base.StartActivity(phoneIntent);
        }

        public void Close()
        {
            MainViewModel babyPhoneViewModel = SimpleIoc.Default.GetInstance<MainViewModel>();
            babyPhoneViewModel.Dispose();
            this.DestroyIoC = true;
            base.Finish();
        }
    }
}

