using Android.Content;
using Android.Net;
using Android.Runtime;
using Android.Telephony;
using Android.Widget;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bleissem.babyphone.Droid
{
    public class PhoneCallListener : PhoneStateListener, IReactOnCall, INotifiedOnCalling, IDisposable
    {

        public PhoneCallListener(Context context)
        {
            this.m_PhoneState = PhoneState.HangUp;
            this.m_TM = context.GetSystemService(Context.TelephonyService) as TelephonyManager;
            this.m_TM.Listen(this, PhoneStateListenerFlags.CallState);
            this.m_Context = context;
        }

       

        ~PhoneCallListener()
        {
            this.Dispose(false);
        }

        TelephonyManager m_TM;
        private Action<CallStateEnum> m_OnPhoneStateChanged;
        private volatile PhoneState m_PhoneState;
        private Context m_Context;
        
      
        public override void OnCallStateChanged(CallState state, string incomingNumber)
        {
            base.OnCallStateChanged(state, incomingNumber);

            switch(state)
            {
                case CallState.Ringing:
                    {
                        this.m_PhoneState = PhoneState.Calling;
                        if (null != m_OnPhoneStateChanged) m_OnPhoneStateChanged(CallStateEnum.Ringing);
                        break;
                    }
                case CallState.Offhook:
                    {

                        this.m_PhoneState = PhoneState.Calling;
                        if (null != m_OnPhoneStateChanged) m_OnPhoneStateChanged(CallStateEnum.Offhook);
                        break;
                    }
                case CallState.Idle:
                    {
                        this.m_PhoneState = PhoneState.HangUp;
                        if (null != m_OnPhoneStateChanged) m_OnPhoneStateChanged(CallStateEnum.Idle);                      
                        break;
                    }
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            this.m_PhoneState = PhoneState.HangUp;

            this.m_Context = null;

            if (null != m_TM)
            {
                m_TM = null;
            }

            if (null != m_OnPhoneStateChanged)
            {
                m_OnPhoneStateChanged = null;
            }
        }
          

        public void Register(Action<CallStateEnum> onPhoneStateChange)
        {
            m_OnPhoneStateChanged = onPhoneStateChange;
        }

        public void Register(Action onHangUp)
        {
            throw new NotImplementedException();
        }

        public PhoneState State
        {
            get
            {
                return m_PhoneState;
            }
            internal set
            {
                m_PhoneState = value;
            }
        }

        public void CallStarts()
        {
            this.m_PhoneState = PhoneState.Calling;
        }
       
    }
}
