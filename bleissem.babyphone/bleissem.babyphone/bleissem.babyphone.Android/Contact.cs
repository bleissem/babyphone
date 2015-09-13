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
using Xamarin.Contacts;

namespace bleissem.babyphone.Droid
{
    public class Contact
    {
        #region Constructor 

        public Contact(string name)
        {
            this.Name = name;
            this.m_Phones = new List<Phone>();
        }

        #endregion

        public string Name { get; set; }

        private List<Phone> m_Phones;
        public List<Phone> Phones { get { return m_Phones; } }

        public void Add(IEnumerable<Phone> phone)
        {
            m_Phones.AddRange(phone);
        }
    }
}