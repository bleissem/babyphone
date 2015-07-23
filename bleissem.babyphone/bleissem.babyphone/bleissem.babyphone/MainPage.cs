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
            this.Initialize();
        }

        private Button m_ChooseContactButton;
        private Entry m_ContactEntry;

        private Button m_NoiseLevelButton;
        private Entry m_NoiseEntry;
        private Label m_CurrentNoiseLevelLabel;

        private Button m_StartButton;

        private Button m_CloseButton;

        private MainViewModel m_MainViewModel;

        private void Initialize()
        {
            m_MainViewModel = SimpleIoc.Default.GetInstance<MainViewModel>();
            m_MainViewModel.PeriodicNotifications += mvm_PeriodicNotifications;
            this.BindingContext = m_MainViewModel;


            m_ChooseContactButton = new Button();
            m_ChooseContactButton.Text = "Contact";
            m_ChooseContactButton.Clicked += m_ChooseContactButton_Clicked;

            m_ContactEntry = new Entry();
            m_ContactEntry.TextChanged += m_ContactEntry_TextChanged;

            m_NoiseLevelButton = new Button();
            m_NoiseLevelButton.Text = "Noise Level";
            m_NoiseLevelButton.Clicked += m_NoiseLevelButton_Clicked;

            m_NoiseEntry = new Entry();
            m_NoiseEntry.Text = this.m_MainViewModel.Settings.NoiseLevel.ToString();
            m_NoiseEntry.TextChanged += m_NoiseEntry_TextChanged;

            m_CurrentNoiseLevelLabel = new Label();

            m_StartButton = new Button();
            m_StartButton.Text = "Start";
            m_StartButton.Clicked += m_StartButton_Clicked;

            m_CloseButton = new Button();
            m_CloseButton.Text = "Close";
            m_CloseButton.Clicked += m_CloseButton_Clicked;
            try
            {
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
                            m_ContactEntry,
                            m_NoiseLevelButton,
                            m_CurrentNoiseLevelLabel,
                            m_NoiseEntry,
                            m_StartButton,
                            m_CloseButton
                           

					    }
				    }
                }
                };
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        void m_ContactEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.m_MainViewModel.Settings.NumberToDial = Convert.ToString(e.NewTextValue);
        }

        void m_NoiseEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            int result = 0;
            if (Int32.TryParse(e.NewTextValue.ToString(), out result))
            {
                this.m_MainViewModel.Settings.NoiseLevel = result;
            }
        }

        void m_CloseButton_Clicked(object sender, EventArgs e)
        {
            m_MainViewModel.CloseApp.Close();
        }

        void m_StartButton_Clicked(object sender, EventArgs e)
        {
            this.m_MainViewModel.Phone.Start();
        }

        void mvm_PeriodicNotifications(object sender, int e)
        {
            this.m_CurrentNoiseLevelLabel.Text = e.ToString();
        }

        void m_NoiseLevelButton_Clicked(object sender, EventArgs e)
        {
            this.m_NoiseEntry.Text = this.m_CurrentNoiseLevelLabel.Text;
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
