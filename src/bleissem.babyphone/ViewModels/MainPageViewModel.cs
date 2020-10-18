using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace bleissem.babyphone.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
           
        }

        private string _selectedContact;
        public string SelectedContact 
        { 
            get 
            { 
                return _selectedContact; 
            } 
            set 
            {
                SetProperty(ref _selectedContact, value);
            } 
        }

        private string _selectedYouAreUsing;
        public string SelectedYouAreUsing
        {
            get
            {
                return _selectedYouAreUsing;
            }
            set
            {
                SetProperty(ref _selectedYouAreUsing, value);
            }
        }
    }
}
