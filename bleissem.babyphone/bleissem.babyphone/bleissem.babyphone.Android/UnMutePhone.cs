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
    public class UnMutePhone : MutePhoneBase, IUnMutePhone
    {
        public UnMutePhone():base()
        {
            
        }

        public void Execute()
        {
            base.SetMute(false);
        }
    }
}