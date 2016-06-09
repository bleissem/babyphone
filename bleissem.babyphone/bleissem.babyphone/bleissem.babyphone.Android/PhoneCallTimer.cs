using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Ioc;
using Android.Telephony;

namespace bleissem.babyphone.Droid
{
    /// <summary>
    /// implements a timer, that always runs to be able to "force" a hangup after a certain timespan
    /// </summary>
    public class PhoneCallTimer : IReactOnCall, INotifiedOnCalling
    {

        #region constructor 

        public PhoneCallTimer(Context context)
        {
            this.m_WasRinging = false;
            this.m_OnHangUp = new List<Action>();
            this.m_PhoneCallListener = new PhoneCallListener(context);
            this.m_PhoneCallListener.Register(OnPhoneCallListenerChanged);

            this.m_StopCallTimer = SimpleIoc.Default.GetInstance<ICreateTimer>().Create(new TimeSpan(0, 0, 1, 0, 0));
            this.m_StopCallTimer.AutoReset = false;
            this.m_StopCallTimer.MyElapsed += m_Timer_MyElapsed;

        }

        #endregion

        ~PhoneCallTimer()
        {
            this.Dispose(false);
        }

        private List<Action> m_OnHangUp;
        private Action<CallStateEnum> m_OnPhoneStateChange;
        private PhoneCallListener m_PhoneCallListener;
        private ITimer m_StopCallTimer;
        private volatile bool m_WasRinging;

        
        void m_Timer_MyElapsed(object sender, MyTimerElapsedEventArgs args)
        {           
            this.DoHangUp(true);
        }

        public void Register(Action onHangup)
        {
            m_OnHangUp.Add(onHangup);
        }

        public void Register(Action<CallStateEnum> onPhoneStateChange)
        {
            m_OnPhoneStateChange = onPhoneStateChange;
        }


        private void DoHangUp(bool byTimer)
        {
            if (!byTimer) this.m_StopCallTimer.Stop();
            m_PhoneCallListener.State = PhoneState.HangUp;

            if (byTimer)
            {
                this.m_WasRinging = false; //prevents hanging up again
                m_PhoneCallListener.ForceHangUp();
            }
           
            if (null != m_OnHangUp)
            {
                foreach(Action onHangup in m_OnHangUp)
                {
                    onHangup();
                }
                
            }

            

        }
      
        public PhoneState State
        {
            get 
            {
                if (null != m_PhoneCallListener) return m_PhoneCallListener.State;

                //you should not reach here..
                return PhoneState.HangUp;
            }
        }

        private void OnPhoneCallListenerChanged(CallStateEnum callState)
        {
            switch (callState)
            {
                case CallStateEnum.Ringing:
                    {

                        this.m_WasRinging = true;
                        break;
                    }
                case CallStateEnum.Offhook:
                    {
                        this.m_WasRinging = true;
                        break;
                    }
                case CallStateEnum.Idle:
                    {
                        if (m_WasRinging)
                        {
                            this.m_WasRinging = false;
                            this.DoHangUp(false);
                        }
                        break;
                    }
            }

            if (null != m_OnPhoneStateChange) m_OnPhoneStateChange(callState);

        }      

        private void Dispose(bool disposing)
        {            

            if (null != m_OnPhoneStateChange)
            {
                m_OnPhoneStateChange = null;
            }

            if (null != m_OnHangUp)
            {
                m_OnHangUp.Clear();
                m_OnHangUp = null;
            }

            if (null != this.m_StopCallTimer)
            {
                this.m_StopCallTimer.Stop();
                this.m_StopCallTimer.MyElapsed -= m_Timer_MyElapsed;
                this.m_StopCallTimer = null;
            }

            if (null != m_PhoneCallListener)
            {
                m_PhoneCallListener.Dispose();
                m_PhoneCallListener = null;
            }
            
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        public void CallStarts()
        {           
            m_PhoneCallListener.State = PhoneState.Calling;
            if (null != m_StopCallTimer)
            {
                m_StopCallTimer.Start();
            }
        }       
    }
}