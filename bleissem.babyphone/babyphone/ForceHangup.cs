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
    public class ForceHangup: IForceHangup
    {

        public ForceHangup()
        {

        }

        ~ForceHangup()
        {
            this.Dispose(false);
        }


        private Action m_ForceHangupAction;

        public void Hangup()
        {
            if (null != m_ForceHangupAction)
            {
                m_ForceHangupAction();
            }
        }

        public void Register(Action forceHangupAction)
        {
            m_ForceHangupAction = forceHangupAction;
        }

        private void Dispose(bool disposing)
        {
            if (null != m_ForceHangupAction)
            {
                m_ForceHangupAction = null;
            }

        }
       
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}