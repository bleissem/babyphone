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
using Android.Telephony;

namespace bleissem.babyphone.Droid
{
    public class PhoneManager: IDisposable
    {
        public PhoneManager(Context context)
        {
            this.m_TelephonyManager = context.GetSystemService(Context.TelephonyService) as TelephonyManager;
        }

        ~PhoneManager()
        {
            this.Dispose(false);
        }


        private TelephonyManager m_TelephonyManager;
        

        public bool EndCall()
        {
            try
            {

                IntPtr getITelephony = JNIEnv.GetMethodID(this.m_TelephonyManager.Class.Handle, "getITelephony", "()Lcom/android/internal/telephony/ITelephony;");
                IntPtr telephony = JNIEnv.CallObjectMethod(this.m_TelephonyManager.Handle, getITelephony);
                IntPtr ITelephony_class = JNIEnv.GetObjectClass(telephony);
                IntPtr ITelephony_endCall = JNIEnv.GetMethodID(
                        ITelephony_class,
                        "endCall",
                        "()Z");
                JNIEnv.CallBooleanMethod(telephony, ITelephony_endCall);
                JNIEnv.DeleteLocalRef(telephony);
                JNIEnv.DeleteLocalRef(ITelephony_class);
                JNIEnv.DeleteLocalRef(getITelephony);

                return true;

            }
            catch
            {
                return false;
            }
        }
      

        private void Dispose(bool disposing)
        {
            if (null != m_TelephonyManager)
            {
                m_TelephonyManager = null;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}