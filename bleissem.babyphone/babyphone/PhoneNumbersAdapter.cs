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
using Xamarin.Contacts;

namespace bleissem.babyphone.Droid
{
    public class PhoneNumbersAdapter: BaseAdapter<Phone>
    {

        public PhoneNumbersAdapter(Activity context, List<Phone> phoneList)
        {
            m_PhoneList = phoneList;
            m_Context = context;
        }

        private List<Phone> m_PhoneList;
        private Activity m_Context;

        public override Phone this[int position]
        {
            get { return m_PhoneList[position]; }
        }

        public override int Count
        {
            get { return m_PhoneList.Count; }
        }

        public override long GetItemId(int position)
        {
            return m_PhoneList[position].GetHashCode();
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = m_PhoneList[position];
            View view = convertView;
            if (view == null) view = m_Context.LayoutInflater.Inflate(Resource.Layout.ContactsDetail, null);
            TextView contactDetail = view.FindViewById<TextView>(Resource.Id.ContactsDetailTextView);
            contactDetail.SetText(item.Number, TextView.BufferType.Normal);

            return view;
        }
    }
}