using MvvmHelpers;
using System.Windows.Input;
using Xam_PushNotification.Model;
using Xam_PushNotification.Service;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Xam_PushNotification.ViewModel
{
    public class vmKaryawanDetil : ObservableObject
    {
        private string title = "Data Karyawan";
        private string _app = "xam";
        private string Status = "";

        private DataKaryawan _karyawanSelected;
        public DataKaryawan KaryawanSelected { get => _karyawanSelected; set => SetProperty(ref _karyawanSelected, value); }
        
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }

        IDetailKaryawanService detailKaryawanService;
        IKaryawanService karyawanService;
        ISignalRService signalRService;
        public readonly ILocalNotificationsService localNotificationsService;
        public vmKaryawanDetil()
        {
            detailKaryawanService = DependencyService.Get<IDetailKaryawanService>();
            karyawanService = DependencyService.Get<IKaryawanService>();
            signalRService = DependencyService.Get<ISignalRService>();
            localNotificationsService = DependencyService.Get<ILocalNotificationsService>();
            KaryawanSelected = detailKaryawanService.KaryawanSelected;
            UpdateCommand = new Command(UpdateKaryawan);
            DeleteCommand = new Command(DeleteKaryawan);
        }

        private void DeleteKaryawan()
        {
            detailKaryawanService.DeleteKaryawan(_karyawanSelected);
            signalRService.SendMessage(title, "delete", false, KaryawanSelected.IdKaryawan);
            karyawanService.ListKaryawan.Remove(_karyawanSelected);
            Application.Current.MainPage.Navigation.PopAsync();
        }

        private void UpdateKaryawan()
        {
            detailKaryawanService.UpdateKaryawan(_karyawanSelected);
            signalRService.SendMessage(title, "update", false, KaryawanSelected.IdKaryawan);
            Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}