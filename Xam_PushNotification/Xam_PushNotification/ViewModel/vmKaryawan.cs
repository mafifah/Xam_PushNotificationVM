using MvvmHelpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xam_PushNotification.Model;
using Xam_PushNotification.Service;
using Xam_PushNotification.View;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Xam_PushNotification.ViewModel
{
    public class vmKaryawan : ObservableObject
    {
        private string title = "Data Karyawan";
        private string _app = "Xam";
        private string Status = "";

        private ObservableCollection<DataKaryawan> _listKaryawan = new ObservableCollection<DataKaryawan>();
        public ObservableCollection<DataKaryawan> ListKaryawan { get => _listKaryawan; set => SetProperty(ref _listKaryawan, value); }

        private string _namaLengkap;
        public string NamaLengkap { get => _namaLengkap; set => SetProperty(ref _namaLengkap, value); }

        private string _title;
        public string Title { get => _title; set => SetProperty(ref _title, value); }

        public ICommand MoveToInsertPageCommand { get; }
        public ICommand SelectedCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand LogoutCommand { get; }

        IKaryawanService karyawanService;
        IDetailKaryawanService detailKaryawanService;
        ISignalRService signalRService;
        public readonly ILocalNotificationsService localNotificationsService;

        public vmKaryawan()
        {
            karyawanService = DependencyService.Get<IKaryawanService>();
            signalRService = DependencyService.Get<ISignalRService>();
            detailKaryawanService = DependencyService.Get<IDetailKaryawanService>();
            localNotificationsService = DependencyService.Get<ILocalNotificationsService>();
            Title = $"Login Sebagai: {Preferences.Get("divisi", null)}";
            ListKaryawan = karyawanService.ListKaryawan;
            MoveToInsertPageCommand = new Command(MoveToInsertPage);
            SaveCommand = new Command(SaveKaryawan);
            SelectedCommand = new Command(SetKaryawanSelected);
            LogoutCommand = new Command(OnLogout);
            Status = signalRService.Status;
            ConnectSignalR();
            Connectivity.ConnectivityChanged += Connectivity_Changed;
        }
        async void Connectivity_Changed(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess != NetworkAccess.Internet)
            {
                await Application.Current.MainPage.DisplayAlert("Warning", "Koneksi anda tidak stabil, anda mungkin tidak dapat menerima notifikasi dan pembaruan data. Mohon periksa kembali koneksi internet anda.", "OK");
            }
            else if (e.NetworkAccess == NetworkAccess.Internet)
            {
                //signalRService = DependencyService.Get<ISignalRService>();
                //await signalRService.Disconnect();
                await signalRService.Connect();
            }

        }

        //Method untuk memulai koneksi ke SignalR Server
        protected async Task ConnectSignalR()
        {
            signalRService.ReceiveMessage(OnDataBerubah);
            if (Status != "Connected")
            {
                try
                {
                    await signalRService.Connect();
                }
                catch (Exception e)
                {
                    var msg = e.Message;
                }
            }
        }

        //Method untuk menangani perubahan data
        private async void OnDataBerubah(ClientMessage clientMessage)
        {
            if (clientMessage.Method == "insert" || clientMessage.Method == "update")
            {
                var data = await karyawanService.GetKaryawanById(clientMessage.IdKaryawan);
                var data2 = karyawanService.ListKaryawan.Where(i => i.IdKaryawan == data.IdKaryawan).FirstOrDefault();
                if (clientMessage.Method == "update")
                {
                    data2.Nama = data.Nama;
                }
                else if (clientMessage.Method == "insert" && data2 == null)
                {
                    karyawanService.ListKaryawan.Add(data);
                }
            }
            else if (clientMessage.Method == "delete")
            {
                var item = _listKaryawan.Where(i => i.IdKaryawan == clientMessage.IdKaryawan).FirstOrDefault();
                karyawanService.ListKaryawan.Remove(item);
                await Application.Current.MainPage.Navigation.PopAsync();
            }
            localNotificationsService.ShowNotification("Data Karyawan", clientMessage.Message);
        }

        //Send Message ke SignalR Server
        private void SendMessage(string method, long id)
        {
            var msg = $"{_app}_{title} {method}";
            var message = new ClientMessage
            {
                Message = msg,
                Method = method,
                Divisi = Preferences.Get("divisi", null),
                IdKaryawan = id
            };
            signalRService.SendMessage(message, false);
        }

        private void MoveToInsertPage()
        {
            Application.Current.MainPage.Navigation.PushAsync(new cpInsertKaryawan());
        }

        private void SaveKaryawan()
        {
            var rnd = new Random();
            long id = rnd.Next(1, 10000);
            var output = karyawanService.InsertKaryawan(id, _namaLengkap);
            SendMessage("insert", id);
            Application.Current.MainPage.Navigation.PopAsync();
        }

        private void SetKaryawanSelected(object obj)
        {
            var data = obj as DataKaryawan;
            detailKaryawanService.SetKaryawanSelected(data);
            Application.Current.MainPage.Navigation.PushAsync(new cpDetilKaryawan());
        }

        private async void OnLogout(object obj)
        {
            await signalRService.Disconnect();
            Preferences.Remove("divisi");
            await Application.Current.MainPage.Navigation.PushAsync(new cpLogin());
        }
    }
}