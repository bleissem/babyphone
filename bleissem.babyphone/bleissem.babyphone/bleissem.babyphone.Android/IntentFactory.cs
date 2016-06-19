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
    public class IntentFactory
    {
        public const string SetIdToCall = "SetIdToCall";
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


        public static Intent GetCallPhoneIntent(string tel, bool useExtraSetClass)
        {

            int androidSDKVersion = 0;

            Intent phoneIntent = new Intent(Intent.ActionCall);
            if ((useExtraSetClass) && (Int32.TryParse(Build.VERSION.Sdk, out androidSDKVersion)))
            {
                if (androidSDKVersion < 21)
                {
                    phoneIntent.SetPackage("com.android.phone");
                }
                else
                {
                    phoneIntent.SetPackage("com.android.server.telecom");
                }
            }

            phoneIntent.SetData(Android.Net.Uri.Parse("tel:" + tel));
            phoneIntent.AddFlags(ActivityFlags.NewTask);
            phoneIntent.AddFlags(ActivityFlags.ClearWhenTaskReset);
            phoneIntent.AddFlags(ActivityFlags.NoUserAction);
            phoneIntent.AddFlags(ActivityFlags.NoHistory);
            phoneIntent.AddFlags(ActivityFlags.FromBackground);

            return phoneIntent;

        }

        public  static Intent GetSkypeUserIntent(string user)
        {
            Intent skypeintent = new Intent("android.intent.action.VIEW");
            skypeintent.SetData(Android.Net.Uri.Parse("skype:" + user + "?call"));
            return skypeintent;
        }

        public static Intent GetSkypeOutIntent(string tel)
        {
            Intent skypeintent = new Intent(Intent.ActionCall);
            skypeintent.SetClassName("com.skype.raider", "com.skype.raider.Main");
            skypeintent.SetData(Android.Net.Uri.Parse("tel:" + tel));
            return skypeintent;
        }
    }
}