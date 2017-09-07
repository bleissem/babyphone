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
using Android.Content.PM;

namespace bleissem.babyphone.Droid
{
    [Activity(Label = "bleissem.babyphone", Icon = "@drawable/icon")]
    public class PhoneNumbersActivity : Activity
    {

        public PhoneNumbersActivity()
        {
        }

        private ListView m_ListView;
        private PhoneNumbersAdapter m_PhoneNumbersAdapter;
        private SettingsTable.CallTypeEnum m_CallType;

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ContactsMaster);

            m_ListView = FindViewById<ListView>(Resource.Id.ContactsList);
            
            string id = this.Intent.GetStringExtra(IntentFactory.SetIdToCall);
            m_CallType = (SettingsTable.CallTypeEnum)this.Intent.GetIntExtra(IntentFactory.SetCallType, -1);

            ReadContacts rc = SimpleIoc.Default.GetInstance<ReadContacts>();

            m_PhoneNumbersAdapter = new PhoneNumbersAdapter(this, rc.List[id].Phones);
            m_ListView.Adapter = m_PhoneNumbersAdapter;
            m_ListView.ItemClick += m_ListView_ItemClick;
        }

        void m_ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var number = m_PhoneNumbersAdapter[e.Position];

            // IntentFactory.StartActivityWithNoHistory<PhoneNumberAdditionalSettingsActivity>(this, (intent) =>    //TODO: use this in future versions 
            IntentFactory.StartActivityThatAlreadyExist<MainActivity>(this, (intent) =>
            {
                intent.PutExtra(IntentFactory.SetIdToCall, number.Number);
                intent.PutExtra(IntentFactory.SetCallType, Convert.ToInt32(m_CallType));
            });
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();

            IntentFactory.StartActivityWithNoHistory<ContactsMasterActivitiy>(this, (intent) =>
            {
                intent.PutExtra(IntentFactory.SetCallType, Convert.ToInt32(m_CallType));
            });
        }

    }
}