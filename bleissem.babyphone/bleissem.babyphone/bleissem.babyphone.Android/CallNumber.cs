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

namespace bleissem.babyphone.Droid
{
    public class CallNumber: ICallNumber
    {

        #region constructor

        public CallNumber()
        {

        }

        #endregion

        ~CallNumber()
        {
            this.Dispose(false);
        }

        private Action m_CallAction;
        private Func<bool> m_CanDial;

        public bool CanDial()
        {
            return m_CanDial();
        }

        public void Dial()
        {
            if (m_CanDial())
            {
                m_CallAction();
            }
        }


        public void Register(Action dialAction, Func<bool> canDial)
        {
            m_CallAction = dialAction;
            m_CanDial = canDial;
        }


        private void Dispose(bool disposing)
        {
            if (null != m_CallAction)
            {
                m_CallAction = null;
            }

            if(null != m_CanDial)
            {
                m_CanDial = null;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}