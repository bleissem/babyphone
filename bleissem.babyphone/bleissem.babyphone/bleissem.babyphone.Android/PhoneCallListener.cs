using Android.Telephony;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bleissem.babyphone.Droid
{
    public class PhoneCallListener : PhoneStateListener
    {

        public PhoneCallListener(MainActivity activity, Action action)
        {
            this.IsPhoneCalling = false;
            m_Activity = activity;
            m_Action = action;
        }

        private bool IsPhoneCalling;

        private MainActivity m_Activity;
        private Action m_Action;

        public override void OnCallStateChanged(CallState state, string incomingNumber)
        {
            base.OnCallStateChanged(state, incomingNumber);

            if (CallState.Ringing == state)
            {
                Toast.MakeText(m_Activity.ApplicationContext, "ringing", ToastLength.Long).Show();
            }
            if (CallState.Offhook == state)
            {
                this.IsPhoneCalling = true;
                Toast.MakeText(m_Activity.ApplicationContext, "calling", ToastLength.Long).Show();
            }
            if (CallState.Idle == state)
            {
                if (this.IsPhoneCalling)
                {
                    //go back
                    Toast.MakeText(m_Activity.ApplicationContext, "finish", ToastLength.Long).Show();

                    // Intent intent = new Intent(_Activity, typeof(MainActivity));
                    // _Activ
                    //_Activity.Recreate();
                    //startActivity(intent);
                    /*
                    Intent intent = Application.Context.PackageManager.GetLaunchIntentForPackage("Babyphone.Android.MainActivity");
                    _Activity.Finish();
                    _Activity.StartActivity(intent);
                    */

                    m_Action();
                }
            }
        }
    }
}
