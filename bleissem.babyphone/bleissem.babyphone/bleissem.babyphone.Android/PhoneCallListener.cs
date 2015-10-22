using Android.Content;
using Android.Telephony;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bleissem.babyphone.Droid
{
    public class PhoneCallListener : PhoneStateListener, IReactOnHangUp
    {

        public PhoneCallListener()
        {
            this.IsPhoneCalling = false;
        }

        ~PhoneCallListener()
        {
            this.Dispose(false);
        }

        private bool IsPhoneCalling;

        private Action m_Action;

        public override void OnCallStateChanged(CallState state, string incomingNumber)
        {
            base.OnCallStateChanged(state, incomingNumber);

            if (CallState.Ringing == state)
            {
                //string dialing = m_Activity.GetString(Resource.String.Dialing);
                //Toast.MakeText(m_Activity.ApplicationContext, dialing, ToastLength.Long).Show();
            }
            if (CallState.Offhook == state)
            {
                this.IsPhoneCalling = true;
                //string activeCall = m_Activity.GetString(Resource.String.ActiveCall);
                //Toast.MakeText(m_Activity.ApplicationContext, activeCall, ToastLength.Long).Show();
            }
            if (CallState.Idle == state)
            {
                if (this.IsPhoneCalling)
                {
                    //go back
                    //string finishCall = m_Activity.GetString(Resource.String.FinishCall);
                    //Toast.MakeText(m_Activity.ApplicationContext, finishCall, ToastLength.Long).Show();
                    if (null != m_Action)
                    {
                        this.IsPhoneCalling = false;
                        m_Action();                  
                    }
                }
            }
        }
       
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
          
            if (null != m_Action)
            {
                m_Action = null;
            }
        }

        public void Accept(Action actionOnHangUp)
        {
            m_Action = actionOnHangUp;
        }
    }
}
