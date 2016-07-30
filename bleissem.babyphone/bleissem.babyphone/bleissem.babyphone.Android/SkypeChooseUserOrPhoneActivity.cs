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


        private Button m_ChooseSkypeUserOKButton;
        private Button m_ChooseSkypeOutButton;
        private CheckBox m_IsSkypeEnabledCheckBox;

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.SkypeChooseUserOrPhone);

            m_ChooseSkypeUserOKButton = this.FindViewById<Button>(Resource.Id.ChooseSkypeUserOKButton);
            m_ChooseSkypeUserOKButton.Click += chooseSkypeUserOKButton_Click;


            m_ChooseSkypeOutButton = this.FindViewById<Button>(Resource.Id.ChooseSkypeOutButton);
            m_ChooseSkypeOutButton.Click += m_ChooseSkypeOutButton_Click;

            Settings settings = SimpleIoc.Default.GetInstance<bleissem.babyphone.Settings>();

            m_IsSkypeEnabledCheckBox = this.FindViewById<CheckBox>(Resource.Id.CheckBoxEnableSkypeVideo);
            m_IsSkypeEnabledCheckBox.Click += m_IsSkypeEnabledCheckBox_Click;
            m_IsSkypeEnabledCheckBox.Checked = settings.IsSkypeVideoEnabled;
        }

        private void m_IsSkypeEnabledCheckBox_Click(object sender, EventArgs e)
        {
            Settings settings = SimpleIoc.Default.GetInstance<bleissem.babyphone.Settings>();
            settings.IsSkypeVideoEnabled = m_IsSkypeEnabledCheckBox.Checked;
        }            

        void m_ChooseSkypeOutButton_Click(object sender, EventArgs e)
        {
            // start Contact List (for Skype phone type)
            IntentFactory.StartActivityWithNoHistory<ContactsMasterActivitiy>(this, (intent) =>
                {
                    intent.PutExtra(IntentFactory.SetCallType, Convert.ToInt32(SettingsTable.CallTypeEnum.SkypeOut));
                });
        }

        void chooseSkypeUserOKButton_Click(object sender, EventArgs e)
        {
            TextView skypeUserTextView = this.FindViewById<TextView>(Resource.Id.ChooseSkypeUserText);
            string skypeUser = skypeUserTextView.Text;
            if (string.IsNullOrWhiteSpace(skypeUser)) return;

            IntentFactory.StartActivityThatAlreadyExist<MainActivity>(this, (intent) =>
            {
                intent.PutExtra(IntentFactory.SetCallType, Convert.ToInt32(SettingsTable.CallTypeEnum.SkypeUser));
                intent.PutExtra(IntentFactory.SetIdToCall, skypeUser);
            });

        }

        private void CleanUp()
        {
            if (null != m_ChooseSkypeUserOKButton)
            {
                m_ChooseSkypeUserOKButton.Click -= chooseSkypeUserOKButton_Click;
                m_ChooseSkypeUserOKButton = null;
            }

            if (null != m_ChooseSkypeOutButton)
            {
                m_ChooseSkypeOutButton.Click -= m_ChooseSkypeOutButton_Click;
                m_ChooseSkypeOutButton = null;
            }

            if (null != m_IsSkypeEnabledCheckBox)
            {
                m_IsSkypeEnabledCheckBox.Click -= m_IsSkypeEnabledCheckBox_Click;
                m_IsSkypeEnabledCheckBox = null;
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