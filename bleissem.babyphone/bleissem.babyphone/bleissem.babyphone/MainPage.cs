using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace bleissem.babyphone
{
    public class MainPage: ContentPage
    {
        public MainPage()
        {

        }

        private Button m_ChooseContactButton;
        private Entry m_ContactEntry;

        private void Initialize()
        {           
            Settings settings = SimpleIoc.Default.GetInstance<Settings>();
            this.BindingContext = settings;

            m_ChooseContactButton = new Button();
            m_ChooseContactButton.Text = "Contact";
            m_ChooseContactButton.Clicked += m_ChooseContactButton_Clicked;

            m_ContactEntry = new Entry();
            m_ContactEntry.SetBinding(Entry.TextProperty, "NumberToDial");

            this.Content = new StackLayout()
            {
                Spacing = 5,
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children = 
                {
                    new StackLayout()
				    {
					    Spacing= 0,
					    Orientation = StackOrientation.Vertical,
					    Children = 
					    {
									       
						    new Label()
                            {
                                Text = "Choose:"
                            },
                            m_ChooseContactButton,
                            m_ContactEntry

					    }
				    }
                }
            };
        }

        void m_ChooseContactButton_Clicked(object sender, EventArgs e)
        {
            /*
            Toast toast = Toast.MakeText(this, "this feature will be avaiable soon.", ToastLength.Short);
            toast.SetGravity(GravityFlags.CenterHorizontal, 0, 0);
            toast.Show();
             * */
        }


    }
}
