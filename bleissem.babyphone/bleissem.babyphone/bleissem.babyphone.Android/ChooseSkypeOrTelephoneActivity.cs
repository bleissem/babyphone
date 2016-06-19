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
    public class ChooseSkypeOrTelephoneActivity : Activity
    {

        public ChooseSkypeOrTelephoneActivity()
        {

        }


        ~ChooseSkypeOrTelephoneActivity()
        {
            this.Dispose(false);
        }

        private Button m_ChooseSkypeButton;
        private Button m_ChoosePhoneButton;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            this.CleanUp();          
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.ChooseSkypeOrTelephone);

            m_ChooseSkypeButton = FindViewById<Button>(Resource.Id.ChooseSkypeButton);
            m_ChooseSkypeButton.Click += m_ChooseSkypeButton_Click;

            m_ChoosePhoneButton = FindViewById<Button>(Resource.Id.ChoosePhoneButton);
            m_ChoosePhoneButton.Click += m_ChoosePhoneButton_Click;

        }

        
        void m_ChooseSkypeButton_Click(object sender, EventArgs e)
        {
            IntentFactory.StartActivityWithNoHistory<SkypeChooseUserOrPhoneActivity>(this);         
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            this.CleanUp();
        }

        private void CleanUp()
        {
            if (null != m_ChoosePhoneButton)
            {
                m_ChoosePhoneButton.Click -= m_ChoosePhoneButton_Click;
                m_ChoosePhoneButton = null;
            }

            if (null != m_ChooseSkypeButton)
            {
                m_ChooseSkypeButton.Click -= m_ChooseSkypeButton_Click;
                m_ChooseSkypeButton = null;
            }
           
        }

        void m_ChoosePhoneButton_Click(object sender, EventArgs e)
        {
            IntentFactory.StartActivityWithNoHistory<ContactsMasterActivitiy>(this, (intent) =>
            {
                intent.PutExtra(IntentFactory.SetCallType, Convert.ToInt32(SettingsTable.CallTypeEnum.Phone));
            });
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();

            IntentFactory.StartActivityThatAlreadyExist<MainActivity>(this);
        }

    }
}