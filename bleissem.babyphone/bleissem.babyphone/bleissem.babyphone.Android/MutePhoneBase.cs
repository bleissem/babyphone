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
using Android.Media;

namespace bleissem.babyphone.Droid
{
    public class MutePhoneBase : IDisposable
    {
        public MutePhoneBase()
        {

        }

        ~MutePhoneBase()
        {
            this.Dispose(false);
        }

        private void Dispose(bool disposing)
        {

        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected void SetMute(bool enable)
        {

        }
    }
}