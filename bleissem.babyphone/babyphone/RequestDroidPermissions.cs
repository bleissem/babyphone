using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;

namespace babyphone
{
    public class RequestDroidPermissions
    {
        private Activity _Activity;
        public RequestDroidPermissions(Activity activity)
        {
            _Activity = activity;
        }
        public void CheckPermission(IEnumerable<String> permissions)
        {
            foreach (String permission in permissions)
            {

                if (!HasPermission(permission))
                {
                    ActivityCompat.RequestPermissions(_Activity, new String[] { permission }, 1);
                }
            }
        }

        private bool HasPermission(string permission)
        {
            PackageInfo info = _Activity.PackageManager.GetPackageInfo(_Activity.PackageName, 0);
            BuildVersionCodes targetSdkVersion = info.ApplicationInfo.TargetSdkVersion;

            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M)
            {
                if (targetSdkVersion >= Android.OS.BuildVersionCodes.M)
                {
                    return _Activity.CheckSelfPermission(permission) == PermissionChecker.PermissionGranted;
                }
                else
                {
                    return PermissionChecker.CheckSelfPermission(_Activity, permission) == PermissionChecker.PermissionGranted;
                }
            }

            return true;
        }
    }
}