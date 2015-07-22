using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace bleissem.babyphone
{
    public class App : Application
    {

        #region constructor        

        public App()
        {
            SimpleIoc.Default.Register<MainViewModel>(true);
            this.MainPage = new MainPage();
        }
     

        #endregion
        
    }
}
