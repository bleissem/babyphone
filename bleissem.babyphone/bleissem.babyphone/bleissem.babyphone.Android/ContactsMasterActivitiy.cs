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
    [Activity(Label = "bleissem.babyphone", Icon = "@drawable/icon", LaunchMode = LaunchMode.SingleTask)]
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
            var name = m_ContactsAdapter[e.Position];

            Intent i = new Intent(this, typeof(PhoneNumbersActivity));
            i.PutExtra(Consts.SetNameForPhoneNumbers, name.Name);
            StartActivity(i);           


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