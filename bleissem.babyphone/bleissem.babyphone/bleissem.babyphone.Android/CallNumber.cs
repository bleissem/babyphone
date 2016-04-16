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

        public void Dial()
        {
            m_CallAction();
        }


        public void Register(Action dialAction)
        {
            m_CallAction = dialAction;
        }


        private void Dispose(bool disposing)
        {
            if (null != m_CallAction)
            {
                m_CallAction = null;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }
    }
}