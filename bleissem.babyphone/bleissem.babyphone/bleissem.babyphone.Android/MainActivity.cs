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
    [Activity(Label = "bleissem.babyphone", Icon = "@drawable/icon", MainLauncher = true, LaunchMode = LaunchMode.SingleTask)]
    public class MainActivity : Activity, ICallNumber, ICloseApp
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            this.InitializeIoC();
            this.InitializeUI();            
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
            babyViewModel.PeriodicNotifications += MainActivity_PeriodicNotifications; ;

            Button chooseContactButton = FindViewById<Button>(Resource.Id.ChooseContactButton);
            chooseContactButton.Enabled = false | chooseContactButton.Enabled;
            chooseContactButton.Click += chooseContactButton_Click;

            Button noiseLevelButton = FindViewById<Button>(Resource.Id.NoiseLevelButton);
            noiseLevelButton.Click += noiseLevelButton_Click;

            Button startServiceButton = FindViewById<Button>(Resource.Id.ServiceButton);
            startServiceButton.Text = startServiceButton.Text = this.ApplicationContext.Resources.GetText(Resource.String.StartService);
            startServiceButton.Click += startServiceButton_Click;


            Button closeButton = FindViewById<Button>(Resource.Id.CloseButton);
            closeButton.Click += closeButton_Click;

            TextView numberToDial = FindViewById<TextView>(Resource.Id.ContactTextView);
            numberToDial.Text = settings.NumberToDial;
            numberToDial.TextChanged += numberToDial_TextChanged;

            TextView noiseLevel = FindViewById<TextView>(Resource.Id.NoiseLevelTextView);
            noiseLevel.Text = settings.NoiseLevel.ToString();
            noiseLevel.TextChanged += noiseLevel_TextChanged;

            PhoneCallListener listener = new PhoneCallListener(this, () =>
            {
                MainViewModel bpvm = SimpleIoc.Default.GetInstance<MainViewModel>();
                bpvm.Phone.Start();
            });

            TelephonyManager tm = this.GetSystemService(Context.TelephonyService) as TelephonyManager;
            tm.Listen(listener, PhoneStateListenerFlags.CallState);
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
            var platform = new SQLitePlatformAndroid();
            var dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "Babyphone.Settings.db3");
            Settings settings = new Settings(dbPath, platform);
            
            ReadContacts rc = new ReadContacts();
            rc.OnFinished+= delegate()
            {
                Button chooseContactButton = FindViewById<Button>(Resource.Id.ChooseContactButton);
                chooseContactButton.Enabled = true;
            };
            rc.Execute(this);
            SimpleIoc.Default.Register<ReadContacts>(() => rc, true);

            SimpleIoc.Default.Register<bleissem.babyphone.Settings>(() => settings, true);
            SimpleIoc.Default.Register<ICreateTimer>(() => new MyTimerCreator(), true);
            SimpleIoc.Default.Register<ICallNumber>(() => this, true);
            SimpleIoc.Default.Register<ICloseApp>(() => this, true);
            SimpleIoc.Default.Register<IAudioRecorder>(() => new AudioRecorderViewModel(), true);


            SimpleIoc.Default.Register<MainViewModel>(true);

        }

        protected override void OnRestart()
        {
            base.OnRestart();
        }

        void closeButton_Click(object sender, EventArgs e)
        {
            MainViewModel babyPhoneViewModel = SimpleIoc.Default.GetInstance<MainViewModel>();
            babyPhoneViewModel.Close();
        }

        void startServiceButton_Click(object sender, EventArgs e)
        {
            Button contactButton = FindViewById<Button>(Resource.Id.ChooseContactButton);            
            Button startServiceButton = FindViewById<Button>(Resource.Id.ServiceButton);

            MainViewModel babyPhoneViewModel = SimpleIoc.Default.GetInstance<MainViewModel>();
            if (babyPhoneViewModel.Phone.IsStarted)
            {
                babyPhoneViewModel.Phone.Stop();
                startServiceButton.Text = this.ApplicationContext.Resources.GetText(Resource.String.StartService);
                contactButton.Enabled = true;
            }
            else if (this.CanStarted())
            {
                if (babyPhoneViewModel.Phone.Start())
                {
                    contactButton.Enabled = false;
                    startServiceButton.Text = this.ApplicationContext.Resources.GetText(Resource.String.StopService);                    
                }
            }

        }

        void chooseContactButton_Click(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(ContactsMasterActivitiy));
            StartActivity(i);
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

            Button closeButton = FindViewById<Button>(Resource.Id.CloseButton);
            closeButton.Click -= closeButton_Click;


            TextView numberToDial = FindViewById<TextView>(Resource.Id.ContactTextView);
            numberToDial.TextChanged -= numberToDial_TextChanged;

            TextView noiseLevel = FindViewById<TextView>(Resource.Id.NoiseLevelTextView);
            noiseLevel.TextChanged -= noiseLevel_TextChanged;

            babyPhoneViewModel.Dispose();
            SimpleIoc.Default.Reset();
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


        public void Dial()
        {            
            Settings setting = SimpleIoc.Default.GetInstance<Settings>();

            if (string.IsNullOrWhiteSpace(setting.NumberToDial)) return;

            Intent phoneIntent = new Intent(Intent.ActionCall);

            string numberToDial = setting.NumberToDial;
            phoneIntent.SetData(Android.Net.Uri.Parse("tel:" + numberToDial));
            //phoneIntent.AddFlags(ActivityFlags.SingleTop);
            //phoneIntent.AddFlags(ActivityFlags.ReorderToFront);
            phoneIntent.AddFlags(ActivityFlags.NoUserAction);
            phoneIntent.AddFlags(ActivityFlags.PreviousIsTop);
            //phoneIntent.AddFlags(ActivityFlags.NewTask); 
            //phoneIntent.SetFlags(ActivityFlags.TaskOnHome);
            base.StartActivity(phoneIntent);

            //FLAG_ACTIVITY_SINGLE_TOP
        }

        public void Close()
        {
            this.Finish();
        }
    }
}

