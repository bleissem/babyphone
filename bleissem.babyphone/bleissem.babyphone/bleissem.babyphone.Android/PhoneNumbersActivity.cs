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
    [Activity(Label = "ContactsPhoneNumbersActivity")]
    public class PhoneNumbersActivity : Activity
    {

        public PhoneNumbersActivity()
        {
        }

        private ListView m_ListView;
        private PhoneNumbersAdapter m_PhoneNumbersAdapter;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.ContactsMaster);

            m_ListView = FindViewById<ListView>(Resource.Id.ContactsList);


            string nameForPhoneNumbers = this.Intent.GetStringExtra(Consts.SetNameForPhoneNumbers);

            ReadContacts rc = SimpleIoc.Default.GetInstance<ReadContacts>();

            m_PhoneNumbersAdapter = new PhoneNumbersAdapter(this, rc.List[nameForPhoneNumbers]);
            m_ListView.Adapter = m_PhoneNumbersAdapter;
            m_ListView.ItemClick += m_ListView_ItemClick;
        }

        void m_ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var number = m_PhoneNumbersAdapter[e.Position];

            Intent i = new Intent(this, typeof(MainActivity));
            i.PutExtra(Consts.SetPhoneNumber, number.Number);
            StartActivity(i);
        }

    }
}