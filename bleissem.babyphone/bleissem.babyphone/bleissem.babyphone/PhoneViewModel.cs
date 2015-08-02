using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bleissem.babyphone
{
    public class PhoneViewModel : IDisposable
    {

        #region constructors

        public PhoneViewModel(IAudioRecorder recorder, Settings settings, ICallNumber callNumber, ICreateTimer createTimer)
        {
            this.IsStarted = false;
            this.m_Settings = settings;
            this.m_CallNumber = callNumber;
            this.m_RecorderViewModel = recorder;

            this.m_PhoneTimer = createTimer.Create(new TimeSpan(0, 0, 0, 0, 250));
            this.m_PhoneTimer.AutoReset = false;
            this.m_PhoneTimer.MyElapsed += m_PhoneTimer_Elapsed;
        }


        #endregion

        ~PhoneViewModel()
        {
            this.Dispose(false);
        }

        public bool IsStarted { get; private set; }

        private ITimer m_PhoneTimer;

        private Settings m_Settings;

        private ICallNumber m_CallNumber;

        private IAudioRecorder m_RecorderViewModel;


        private TimeSpan TimeToWait { get; set; }

        void m_WaitAfterCallTimer_Elapsed(object sender, MyTimerElapsedEventArgs e)
        {
            this.InitializePhoneTimer();
        }


        private void m_PhoneTimer_Elapsed(object sender, MyTimerElapsedEventArgs e)
        {
            if (!this.CanStart) { this.Stop(); return; }


            if (m_RecorderViewModel.GetAmplitude() >= m_Settings.NoiseLevel)
            {
                this.m_CallNumber.Dial();
            }
            else
            {

                this.InitializePhoneTimer();
            }

        }

        public bool CanStart
        {
            get
            {
                if (0 >= m_Settings.NoiseLevel)
                {
                    return false;
                }

                return true;
            }
        }

        public bool Start()
        {
            if (!this.CanStart) return false;
            this.IsStarted = true;
            this.InitializePhoneTimer();
            return true;
        }

        private void InitializePhoneTimer()
        {
            if (this.IsStarted)
            {
                m_PhoneTimer.Start();
            }

        }

        public void Stop()
        {
            this.m_PhoneTimer.Stop();
            this.IsStarted = false;
        }

        private void CleanUp()
        {
            if (null != this.m_PhoneTimer)
            {
                this.m_PhoneTimer.Stop();
                this.m_PhoneTimer.MyElapsed -= m_PhoneTimer_Elapsed;
                this.m_PhoneTimer.Dispose();
                this.m_PhoneTimer = null;
            }
        }


        private void Dispose(bool disposing)
        {
            CleanUp();
        }

        public void Dispose()
        {
            this.Dispose(true);
        }
    }
}
