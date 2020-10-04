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
using Xamarin.Forms.Platform.Android;

namespace bleissem.babyphone.Droid
{
    public class CallContext : ICallContext
    {
        public CallContext()
        {
        }

        ~CallContext()
        {
            Dispose(false);
        }

        public FormsAppCompatActivity MainActivity { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            MainActivity = null;
        }
    }
}