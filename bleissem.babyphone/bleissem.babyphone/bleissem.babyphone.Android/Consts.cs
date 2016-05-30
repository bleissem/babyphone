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
        public const string SetSkypeUser = "SetSkypeUser";
        public const string SetSkypePhoneNumber = "SetSkypePhoneNumber";
        public const string SetCallType = "SetCallType";

        public static void StartActivityThatAlreadyExist<TActivity>(Context context) where TActivity : Activity
        {
            StartActivityThatAlreadyExist<TActivity>(context, null);
        }
        public static void StartActivityThatAlreadyExist<TActivity>(Context context, Action<Intent> newIntent) where TActivity : Activity
        {
            Intent intent = new Intent(context, typeof(TActivity));
            intent.AddFlags(ActivityFlags.SingleTop);
            intent.AddFlags(ActivityFlags.ReorderToFront);
            intent.AddFlags(ActivityFlags.ClearTop);
            intent.AddFlags(ActivityFlags.NoUserAction);
            intent.AddFlags(ActivityFlags.FromBackground);
            
            if (null != newIntent)
            {
                newIntent(intent);
            }
            context.StartActivity(intent);
        }

        public static void StartActivityWithNoHistory<TActivity>(Context context) where TActivity : Activity
        {
            StartActivityWithNoHistory<TActivity>(context, null);
        }

        public static void StartActivityWithNoHistory<TActivity>(Context context, Action<Intent> newIntent) where TActivity : Activity
        {
            Intent intent = new Intent(context, typeof(TActivity));
            intent.AddFlags(ActivityFlags.NoHistory);
            intent.AddFlags(ActivityFlags.NoUserAction);
            intent.AddFlags(ActivityFlags.FromBackground);
            
            if (null != newIntent)
            {
                newIntent(intent);
            }
            context.StartActivity(intent);
        }
    }
}