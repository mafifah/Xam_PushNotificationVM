using MvvmHelpers;
using System.Windows.Input;
using Xam_PushNotification.View;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Xam_PushNotification.ViewModel
{
    public class vmLogin : ObservableObject
    {
        private string _divisi;
        public string Divisi { get => _divisi; set => SetProperty(ref _divisi, value); }

        public ICommand LoginCommand { get; }

        public vmLogin()
        {
            LoginCommand = new Command(OnLogin);
        }

        public void OnLogin()
        {
            Preferences.Set("divisi", _divisi);
            Application.Current.MainPage.Navigation.PushAsync(new cpKaryawan());

        }
    }
}
