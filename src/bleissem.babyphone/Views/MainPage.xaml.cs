using Xamarin.Essentials;
using Xamarin.Forms;
using static Xamarin.Essentials.Permissions;

namespace bleissem.babyphone.Views
{
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            CheckPermissions();
        }

        private async void CheckPermissions()
        {
            var requestResult = await Permissions.RequestAsync<Phone>();
            if (requestResult != PermissionStatus.Granted)
            {
                await base.DisplayAlert("permissions", "need permissions", "ok");
                Application.Current.Quit();
            }
        }
    }
}