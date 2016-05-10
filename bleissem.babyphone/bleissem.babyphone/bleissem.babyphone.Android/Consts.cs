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
    public class Consts
    {
        public const string SetPhoneID = "SetPhoneID";
        public const string SetPhoneNumber = "SetPhoneNumber";

        public static void StartActivity<TActivity>(Activity activity) where TActivity : Activity
        {
            StartActivity<TActivity>(activity, null);
        }
        public static void StartActivity<TActivity>(Activity activity, Action<Intent> newIntent) where TActivity : Activity
        {
            Intent intent = new Intent(activity, typeof(TActivity));
            intent.AddFlags(ActivityFlags.SingleTop);
            intent.AddFlags(ActivityFlags.ReorderToFront);
            //intent.AddFlags(ActivityFlags.ClearTop);
            
            if (null != newIntent)
            {
                newIntent(intent);
            }
            activity.StartActivity(intent);
        }
    }
}