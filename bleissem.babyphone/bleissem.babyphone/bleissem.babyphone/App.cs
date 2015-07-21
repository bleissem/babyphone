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
            SimpleIoc.Default.Register<BabyPhoneViewModel>(true);

            m_ChooseContactButton = new Button();
            m_ChooseContactButton.Text = "Contact";
            m_ChooseContactButton.Clicked += m_ChooseContactButton_Clicked;


            this.MainPage = new ContentPage()
            {
                Content = new StackLayout()
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
                                m_ChooseContactButton

					        }
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


        #endregion
        private Button m_ChooseContactButton;
    }
}
