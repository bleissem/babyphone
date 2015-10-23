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
    public class ContactsMasterActivitiy : Activity
    {

        public ContactsMasterActivitiy()
        {

        }

        private ListView m_ListView;
        private ContactsAdapter m_ContactsAdapter;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.ContactsMaster);

            m_ListView = FindViewById<ListView>(Resource.Id.ContactsList);

            m_ContactsAdapter = new ContactsAdapter(this);
            m_ListView.Adapter = m_ContactsAdapter;
            m_ListView.ItemClick += m_ListView_ItemClick;
        }

        void m_ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var contact = m_ContactsAdapter[e.Position];

            Consts.StartActivity<PhoneNumbersActivity>(this, (intent) =>
                {
                    intent.PutExtra(Consts.SetPhoneID, contact.ID);
                });

        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();

            Consts.StartActivity<MainActivity>(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (null != m_ListView)
            {
                m_ListView.ItemClick -= m_ListView_ItemClick;
            }
        }
    }
}