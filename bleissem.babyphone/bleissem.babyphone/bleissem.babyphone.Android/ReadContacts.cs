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
    public class ReadContacts
    {

        public ReadContacts()
        {
            this.List = new SortedDictionary<string, Tuple<string, List<Phone>>>();
            this.Finished = false;
        }

        public async void Execute(Context context)
        {
            this.Finished = false;
            AddressBook addressBook = new AddressBook(context);
            if (await addressBook.RequestPermission())
            {                
                this.List = new SortedDictionary<string,Tuple<string, List<Phone>>>();

                string key = null;

                foreach (Xamarin.Contacts.Contact contact in addressBook)
                {
                    key = contact.Id;
                    this.List.Add(key, new Tuple<string,List<Phone>>(contact.DisplayName, new List<Phone>()));
                    foreach (Phone phone in contact.Phones)
                    {
                        this.List[key].Item2.Add(phone);
                    }

                }

                this.Finished = true;
                if (null != OnFinished)
                {
                    OnFinished();
                }


            }
            else
            {
                // TODO: no rights to read contacts
            }

        }

        public bool Finished { get; set; }

        public delegate void FinishedDelegate();
        public event FinishedDelegate OnFinished;

        public SortedDictionary<string, Tuple<string, List<Phone>>> List { get; set; }
    }
}