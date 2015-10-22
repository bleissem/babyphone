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
    public class ContactsAdapter : BaseAdapter<Contact>
    {
        public ContactsAdapter(Activity context)
        {
            m_Context = context;
            ReadContacts rc = SimpleIoc.Default.GetInstance<ReadContacts>();
            m_List = new List<Contact>();
                        
            foreach(var r in rc.List.OrderByDescending(x => x.Value.Name).Reverse().ToList())
            {
                m_List.Add(r.Value);
            }
        }

        private Activity m_Context;
        private List<Contact> m_List;


        public override Contact this[int position]
        {
            get { return m_List[position]; }
        }

        public override int Count
        {
            get { return m_List.Count; }
        }

        public override long GetItemId(int position)
        {
            return m_List[position].GetHashCode();
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = m_List[position];
            View view = convertView;
            if (view == null) view = m_Context.LayoutInflater.Inflate(Resource.Layout.ContactsDetail, null);
            TextView contactDetail = view.FindViewById<TextView>(Resource.Id.ContactsDetailTextView);
            contactDetail.SetText(item.Name, TextView.BufferType.Normal);
           
            return view;

        }

      
    }
}