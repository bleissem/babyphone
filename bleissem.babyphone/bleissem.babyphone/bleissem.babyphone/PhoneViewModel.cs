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

        public PhoneViewModel(IAudioRecorder recorder, Settings settings, ICallNumber callNumber, IReactOnCall reactOnCall, ICreateTimer createTimer, IMutePhone mutePhone, IUnMutePhone unmutePhone, ISpeaker speaker)
        {
            this.IsStarted = false;
            this.m_Speaker = speaker;
            this.m_UnmutePhone = unmutePhone;
            this.m_MutePhone = mutePhone;
            this.m_Settings = settings;
            this.m_CallNumber = callNumber;
            this.m_AudioRecorderViewModel = recorder;

            this.m_PhoneTimer = createTimer.Create(new TimeSpan(0, 0, 0, 1, 0));
            this.m_PhoneTimer.AutoReset = false                ;
            this.m_PhoneTimer.MyElapsed += m_PhoneTimer_Elapsed;
            
            this.m_ReactOnCall = reactOnCall;
            this.m_ReactOnCall.Register(this.OnHangUp);
        }


        #endregion

        ~PhoneViewModel()
        {
            this.Dispose(false);
        }

        public bool IsStarted { get; private set; }

        private ISpeaker m_Speaker;

        private ITimer m_PhoneTimer;

        private Settings m_Settings;

        private ICallNumber m_CallNumber;

        private IAudioRecorder m_AudioRecorderViewModel;

        private IReactOnCall m_ReactOnCall;

        private TimeSpan TimeToWait { get; set; }

        private IMutePhone m_MutePhone;
        private IUnMutePhone m_UnmutePhone;

        private void OnHangUp()
        {
            if (null == m_PhoneTimer) return;
            m_PhoneTimer.Start();
        }

    
        private void m_PhoneTimer_Elapsed(object sender, MyTimerElapsedEventArgs e)
        {
            if (!this.CanStart) { this.Stop(); return; }


            if ( (null != m_AudioRecorderViewModel) && (m_AudioRecorderViewModel.IsStarted) && (m_AudioRecorderViewModel.GetAmplitude() >= m_Settings.NoiseLevel) && (m_CallNumber.CanDial()) )
            {
                m_AudioRecorderViewModel.Stop();
                this.m_Speaker.Turn(this.m_Settings.UseSpeakerEnabled, this.m_Settings.CallType);
                this.m_CallNumber.Dial();
            }
            else
            {
                m_AudioRecorderViewModel.Start();
                this.m_PhoneTimer.Start();
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
            m_MutePhone.Mute();            
            m_PhoneTimer.Start();
            return true;
        }

        public void Stop()
        {
            if (null == m_PhoneTimer) return;
            this.m_UnmutePhone.UnMute();
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

            if(null != m_MutePhone)
            {
                this.m_MutePhone.Dispose();
                this.m_MutePhone = null;
            }

            if (null != m_UnmutePhone)
            {
                this.m_UnmutePhone.Dispose();
                this.m_UnmutePhone = null;
            }

            if (null != m_ReactOnCall)
            {
                m_ReactOnCall.Dispose();
                m_ReactOnCall = null;
            }

            if (null != m_CallNumber)
            {
                m_CallNumber.Dispose();
                m_CallNumber = null;
            }

            if (null != m_AudioRecorderViewModel)
            {
                m_AudioRecorderViewModel.Dispose();
                m_AudioRecorderViewModel = null;
            }
        }


        private void Dispose(bool disposing)
        {
            CleanUp();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
