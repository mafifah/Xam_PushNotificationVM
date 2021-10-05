using MvvmHelpers;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xam_PushNotification.Service;
using Xamarin.Forms;

namespace Xam_PushNotification.ViewModel
{
    public class vmNewKaryawan : ObservableObject
    {
        private string title = "Data Karyawan";
        private string _namaLengkap;
        public string NamaLengkap { get => _namaLengkap; set => SetProperty(ref _namaLengkap, value); }

        public ICommand SaveCommand { get; }

        IKaryawanService karyawanService;
        ISignalRService signalRService;

        public vmNewKaryawan()
        {
            karyawanService = DependencyService.Get<IKaryawanService>();
            signalRService = DependencyService.Get<ISignalRService>();
            SaveCommand = new Command(SaveKaryawan);
        }

        public async Task ConnectSignalR()
        {
            try
            {
                signalRService.ReceiveMessage();
                await signalRService.Connect();
            }
            catch (Exception e)
            {
                var msg = e.Message;
            }
        }

        private async void SaveKaryawan()
        {
            await ConnectSignalR();
            var rnd = new Random();
            long id = rnd.Next(1, 10000);
            var output = karyawanService.InsertKaryawan(id, _namaLengkap);
            await signalRService.SendMessage(title, "insert", false, id);
            await signalRService.Disconnect();
            await Application.Current.MainPage.Navigation.PopAsync();
        }

    }
}
