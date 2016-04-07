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
            this.m_DidHangUp = false;
            this.m_Context = context;
            this.m_StopCallTimer = SimpleIoc.Default.GetInstance<ICreateTimer>().Create(new TimeSpan(0,1,0,0));
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
        private volatile bool m_DidHangUp;
        private ITimer m_StopCallTimer;
        private Action m_ActionOnHangUp;

        public override void OnCallStateChanged(CallState state, string incomingNumber)
        {
            base.OnCallStateChanged(state, incomingNumber);

            switch(state)
            {
                case CallState.Ringing:
                    {
                        Toast.MakeText(m_Context, "Ringing", ToastLength.Long).Show();
                        break;
                    }
                case CallState.Offhook:
                    {

                        Toast.MakeText(m_Context, "Offhook", ToastLength.Long).Show();
                        this.m_StopCallTimer.Start();
                        break;
                    }
                case CallState.Idle:
                    {
                        Toast.MakeText(m_Context, "Idle", ToastLength.Long).Show();
                        this.DoHangUp();
                        break;
                    }
            }
        }

        private void DoHangUp()
        {
            if (!this.m_DidHangUp)
            {
                this.m_DidHangUp = true;
                if (null != m_ActionOnHangUp)
                {
                    m_ActionOnHangUp();
                }
            }

        }
       
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
          
            if (null != m_ActionOnHangUp)
            {
                m_ActionOnHangUp = null;
            }

            this.m_StopCallTimer.MyElapsed -= m_Timer_MyElapsed;
        }

        public void Accept(Action actionOnHangUp)
        {
            m_ActionOnHangUp = actionOnHangUp;
        }
    }
}
