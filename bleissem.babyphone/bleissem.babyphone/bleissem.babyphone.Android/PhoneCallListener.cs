using Android.Content;
using Android.Net;
using Android.Telephony;
using Android.Widget;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bleissem.babyphone.Droid
{
    public class PhoneCallListener : PhoneStateListener, IReactOnHangUp
    {

        public PhoneCallListener(Context context)
        {
            this.m_PhoneState = PhoneState.HangUp;
            this.m_Context = context;
            this.m_StopCallTimer = SimpleIoc.Default.GetInstance<ICreateTimer>().Create(new TimeSpan(0,1,0,0));
            this.m_StopCallTimer.AutoReset = false;
            this.m_StopCallTimer.MyElapsed += m_Timer_MyElapsed;
        }

        void m_Timer_MyElapsed(object sender, MyTimerElapsedEventArgs args)
        {
            this.m_PhoneState = PhoneState.HangUp;
        }

        ~PhoneCallListener()
        {
            this.Dispose(false);
        }

        private Context m_Context;
        private volatile PhoneState m_PhoneState;
        private ITimer m_StopCallTimer;

        public override void OnCallStateChanged(CallState state, string incomingNumber)
        {
            base.OnCallStateChanged(state, incomingNumber);

            switch(state)
            {
                case CallState.Ringing:
                    {
                        //Toast.MakeText(m_Context, "Ringing", ToastLength.Long).Show();
                        this.m_PhoneState = PhoneState.Calling;
                        this.m_StopCallTimer.Start();
                        break;
                    }
                case CallState.Offhook:
                    {

                        //Toast.MakeText(m_Context, "Offhook", ToastLength.Long).Show();
                        this.m_PhoneState = PhoneState.Calling;
                        this.m_StopCallTimer.Start();
                        break;
                    }
                case CallState.Idle:
                    {
                        //Toast.MakeText(m_Context, "Idle", ToastLength.Long).Show();
                        this.m_PhoneState = PhoneState.HangUp;
                        this.m_StopCallTimer.Stop();
                        break;
                    }
            }
        }

       
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            this.m_PhoneState = PhoneState.HangUp;

            if (null != this.m_StopCallTimer)
            {
                this.m_StopCallTimer.MyElapsed -= m_Timer_MyElapsed;
                this.m_StopCallTimer = null;
            }
        }

        public void Reset()
        {
            this.m_PhoneState = PhoneState.HangUp;
        }


        public PhoneState State
        {
            get
            {
                return m_PhoneState;
            }
        }
    }
}
