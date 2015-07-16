using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace bleissem.babyphone
{
    public class App : Application
    {

        public App()
        {
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
                                    Text = "Hello from babyphone"
                                }
					        }
				        }
                    }
                }
            };
        }
      
    }
}
