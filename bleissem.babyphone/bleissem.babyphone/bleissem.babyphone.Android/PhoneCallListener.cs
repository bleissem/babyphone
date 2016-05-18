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
            this.m_StopCallTimer = SimpleIoc.Default.GetInstance<ICreateTimer>().Create(new TimeSpan(0,0,1,0,0));
            this.m_StopCallTimer.AutoReset = false;
            this.m_StopCallTimer.MyElapsed += m_Timer_MyElapsed;
        }

        void m_Timer_MyElapsed(object sender, MyTimerElapsedEventArgs args)
        {
            this.DoHangUp();
        }

        ~PhoneCallListener()
        {
            this.Dispose(false);
        }

        private Context m_Context;
        private volatile PhoneState m_PhoneState;
        private ITimer m_StopCallTimer;

        private void DoHangUp()
        {
            if (this.m_PhoneState == PhoneState.Calling)
            {
                this.m_PhoneState = PhoneState.HangUp;
                Consts.StartActivity<MainActivity>(this.m_Context);
            }
        }

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
                        this.m_StopCallTimer.Stop();
                        this.DoHangUp();                        
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

      

        public PhoneState State
        {
            get
            {
                return m_PhoneState;
            }
        }
    }
}
