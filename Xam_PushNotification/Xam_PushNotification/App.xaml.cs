using System;
using Xam_PushNotification.View;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xam_PushNotification
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            if (Preferences.Get("divisi", null) != null)
            {
                MainPage = new NavigationPage(new cpKaryawan());
            }
            else
            {
                MainPage = new NavigationPage(new cpLogin());
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
