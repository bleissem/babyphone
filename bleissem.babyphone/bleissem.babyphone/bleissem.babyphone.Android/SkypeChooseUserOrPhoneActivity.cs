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


        private Button m_ChooseSkypeUserOKButton;
        private Button m_ChooseSkypePhoneButton;

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.SkypeChooseUserOrPhone);
            
            Button chooseSkypeUserOKButton = this.FindViewById<Button>(Resource.Id.ChooseSkypeUserOKButton);
            chooseSkypeUserOKButton.Click += chooseSkypeUserOKButton_Click;


            Button m_ChooseSkypePhoneButton = this.FindViewById<Button>(Resource.Id.ChooseSkypePhoneButton);
            m_ChooseSkypePhoneButton.Click += m_ChooseSkypePhoneButton_Click;
        }

        void m_ChooseSkypePhoneButton_Click(object sender, EventArgs e)
        {
            // start Contact List (for Skype phone type)
            Consts.StartActivityWithNoHistory<ContactsMasterActivitiy>(this, (intent) =>
                {
                    intent.PutExtra(Consts.SetCallType, Convert.ToInt32(SettingsTable.CallTypeEnum.SkypePhone));
                });
        }

        void chooseSkypeUserOKButton_Click(object sender, EventArgs e)
        {
            TextView skypeUserTextView = this.FindViewById<TextView>(Resource.Id.ChooseSkypeUserText);
            string skypeUser = skypeUserTextView.Text;
            if (string.IsNullOrWhiteSpace(skypeUser)) return;

            Consts.StartActivityThatAlreadyExist<MainActivity>(this, (intent) =>
            {
                intent.PutExtra(Consts.SetPhoneNumber, skypeUser);
            });

        }

        private void CleanUp()
        {
            if (null != m_ChooseSkypeUserOKButton)
            {
                m_ChooseSkypeUserOKButton.Click -= chooseSkypeUserOKButton_Click;
                m_ChooseSkypeUserOKButton = null;
            }

            if (null != m_ChooseSkypePhoneButton)
            {
                m_ChooseSkypePhoneButton.Click -= m_ChooseSkypePhoneButton_Click;
                m_ChooseSkypePhoneButton = null;
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