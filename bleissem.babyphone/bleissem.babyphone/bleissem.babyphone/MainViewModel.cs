using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bleissem.babyphone
{
    public class MainViewModel : IDisposable
    {
        #region constructor

        public MainViewModel(IAudioRecorder audioRecorder, Settings settings, ICallNumber callNumber, ICloseApp closeApp)
        {
            this.CloseApp = closeApp;
            this.Settings = settings;
            this.m_RecorderViewModel = audioRecorder;
            this.m_RecorderViewModel.Start();
            this.m_PhoneViewModel = new PhoneViewModel(m_RecorderViewModel, settings, callNumber);


            this.m_InfoTimer = new MyTimer(new TimeSpan(0, 0, 1));
            this.m_InfoTimer.AutoReset = true;
            this.m_InfoTimer.Elapsed += m_Timer_Elapsed;
            //this.m_InfoTimer.Start();  //TODO: fix a problem with timer
        }

        #endregion

        ~MainViewModel()
        {
            this.Dispose(false);
        }

        #region InfoTimer

        private MyTimer m_InfoTimer;

        void m_Timer_Elapsed(object sender, MyTimerElapsedEventArgs e)
        {
            if ((null != PeriodicNotifications) && (null != m_RecorderViewModel))
            {

                PeriodicNotifications(this, m_RecorderViewModel.GetAmplitude());
            }
        }

        public event EventHandler<int> PeriodicNotifications;

        #endregion

        private IAudioRecorder m_RecorderViewModel;

        private PhoneViewModel m_PhoneViewModel;

        public PhoneViewModel Phone
        {
            get
            {
                return m_PhoneViewModel;
            }
        }

        public Settings Settings { get; private set; }

        public ICloseApp CloseApp { get; private set; }

        private void Dispose(bool disposing)
        {
            if (null != m_InfoTimer)
            {
                m_InfoTimer.Stop();
                m_InfoTimer.Elapsed -= m_Timer_Elapsed;
                m_InfoTimer.Dispose();
                m_InfoTimer = null;
            }

            if (null != m_RecorderViewModel)
            {
                m_RecorderViewModel.Dispose();
                m_RecorderViewModel = null;

            }

            if (null != m_PhoneViewModel)
            {
                m_PhoneViewModel.Dispose();
                m_PhoneViewModel = null;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
