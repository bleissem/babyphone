using Prism;
using Prism.Ioc;
using bleissem.babyphone.ViewModels;
using bleissem.babyphone.Views;
using Xamarin.Essentials.Interfaces;
using Xamarin.Essentials.Implementation;
using Xamarin.Forms;
using System.Threading;
using System.Globalization;

namespace bleissem.babyphone
{
    public partial class App
    {
        public App(IPlatformInitializer initializer)
            : base(initializer)
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InstalledUICulture;
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            await NavigationService.NavigateAsync("NavigationPage/MainPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
        }
    }
}