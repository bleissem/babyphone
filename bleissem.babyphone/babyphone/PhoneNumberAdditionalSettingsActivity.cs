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
    public class PhoneNumberAdditionalSettingsActivity : Activity
    {

        private Button m_OkButton;
        private CheckBox m_UseSpeaker;
        private SettingsTable.CallTypeEnum m_CallType;
        private string m_CallNumber;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.PhoneNumberAdditionalSettings);

            m_CallNumber = Intent.GetStringExtra(IntentFactory.SetIdToCall);
            m_CallType = (SettingsTable.CallTypeEnum)this.Intent.GetIntExtra(IntentFactory.SetCallType, -1);

            Settings settings = SimpleIoc.Default.GetInstance<bleissem.babyphone.Settings>();

            m_UseSpeaker = FindViewById<CheckBox>(Resource.Id.PhoneAdditionalSettingsSpeakerCheckBox);
            m_UseSpeaker.Checked = settings.UseSpeakerEnabled;
            m_UseSpeaker.Click += M_UseSpeaker_Click;

            m_OkButton = FindViewById<Button>(Resource.Id.PhoneAdditionalSettingsSpeakerOKButton);
            m_OkButton.Click += M_OkButton_Click;
        }

        private void M_UseSpeaker_Click(object sender, EventArgs e)
        {
            Settings settings = SimpleIoc.Default.GetInstance<bleissem.babyphone.Settings>();
            settings.UseSpeakerEnabled = m_UseSpeaker.Checked;
        }

        private void M_OkButton_Click(object sender, EventArgs e)
        {

            IntentFactory.StartActivityThatAlreadyExist<MainActivity>(this, (intent) =>
            {
                intent.PutExtra(IntentFactory.SetIdToCall, m_CallNumber);
                intent.PutExtra(IntentFactory.SetCallType, Convert.ToInt32(m_CallType));
            });
        }
    }
}