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
using babyphone;
using GalaSoft.MvvmLight.Ioc;

namespace bleissem.babyphone.Droid
{
    [Activity(Label = "bleissem.babyphone", Icon = "@drawable/icon")]
    public class SkypeChooseUserOrPhoneActivity : Activity
    {

        public SkypeChooseUserOrPhoneActivity()
        {

        }

        ~SkypeChooseUserOrPhoneActivity()
        {
            this.Dispose(false);
        }


        private Button m_ChooseSkypeUserButton;
        private Button m_ChooseSkypeOutButton;

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.SkypeChooseUserOrSkypePhone);

            m_ChooseSkypeUserButton = this.FindViewById<Button>(Resource.Id.ChooseSkypeUserButton);
            m_ChooseSkypeUserButton.Click += ChooseSkypeUserButton_Click;

            m_ChooseSkypeOutButton = this.FindViewById<Button>(Resource.Id.ChooseSkypeOutButton);
            m_ChooseSkypeOutButton.Click += ChooseSkypeOutButton_Click;


        }

        private void ChooseSkypeUserButton_Click(object sender, EventArgs e)
        {
            IntentFactory.StartActivityWithNoHistory<SkypeUserActivity>(this);
        }

        void ChooseSkypeOutButton_Click(object sender, EventArgs e)
        {
            // start Contact List (for Skype phone type)
            IntentFactory.StartActivityWithNoHistory<ContactsMasterActivitiy>(this, (intent) =>
                {
                    intent.PutExtra(IntentFactory.SetCallType, Convert.ToInt32(SettingsTable.CallTypeEnum.SkypeOut));
                });
        }



        private void CleanUp()
        {
            if (null != m_ChooseSkypeUserButton)
            {
                m_ChooseSkypeUserButton.Click -= ChooseSkypeUserButton_Click;
                m_ChooseSkypeUserButton = null;
            }

            if (null != m_ChooseSkypeOutButton)
            {
                m_ChooseSkypeOutButton.Click -= ChooseSkypeOutButton_Click;
                m_ChooseSkypeOutButton = null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            this.CleanUp();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}