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
using bleissem.babyphone.Core;

namespace bleissem.babyphone.Droid
{
    public class Call : ICall
    {
        public Call(ICallContext callContext, INumberContext numberContext)
        {
            _callContext = callContext;
            _numberContext = numberContext;
        }

        ~Call()
        {
            Dispose(false);
        }

        private ICallContext _callContext;
        private INumberContext _numberContext;

        private const string PhonePackage = "com.android.server.telecom";

        public void Number()
        {
            string number = _numberContext.Number;
            var mainActivity = global::Android.App.Application.Context;// _callContext.MainActivity;
            Intent phoneIntent = new Intent(Intent.ActionCall);

            phoneIntent.SetPackage(PhonePackage);
            phoneIntent.SetData(Android.Net.Uri.Parse($"tel:{number}"));
            phoneIntent.AddFlags(ActivityFlags.NewTask);
            phoneIntent.AddFlags(ActivityFlags.ClearWhenTaskReset);
            phoneIntent.AddFlags(ActivityFlags.NoUserAction);
            phoneIntent.AddFlags(ActivityFlags.NoHistory);
            phoneIntent.AddFlags(ActivityFlags.FromBackground);
            mainActivity.StartActivity(phoneIntent);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _callContext?.Dispose();
            _callContext = null;
            _numberContext?.Dispose();
            _numberContext = null;
        }
    }
}