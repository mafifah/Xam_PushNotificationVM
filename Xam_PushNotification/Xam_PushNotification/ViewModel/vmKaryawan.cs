using MvvmHelpers;
using Plugin.LocalNotification;
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

        private ObservableCollection<DataKaryawan> _listKaryawan = new ObservableCollection<DataKaryawan>();
        public ObservableCollection<DataKaryawan> ListKaryawan { get => _listKaryawan; set => SetProperty(ref _listKaryawan, value); }

        
        private string _title;
        public string Title { get => _title; set => SetProperty(ref _title, value); }

        public ICommand MoveToInsertPageCommand { get; }
        public ICommand SelectedCommand { get; }
        
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
            SelectedCommand = new Command(SetKaryawanSelected);
            LogoutCommand = new Command(OnLogout);
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
                await ConnectSignalR();
            }

        }

        
        //Method untuk memulai koneksi ke SignalR Server
        public async Task ConnectSignalR()
        {
            try
            {
                signalRService.ReceiveMessage(OnDataBerubah);
                await signalRService.Connect();
            }
            catch (Exception e)
            {
                var msg = e.Message;
            }
        }

        //Method untuk menangani perubahan data
        private async void OnDataBerubah(ClientMessage clientMessage)
        {
            if (clientMessage.JenisPesan == "insert" || clientMessage.JenisPesan == "update")
            {
                var ConvertedId = Convert.ToInt64(clientMessage.Id_PrimaryKey);
                var data = await karyawanService.GetKaryawanById(ConvertedId);
                var data2 = karyawanService.ListKaryawan.Where(i => i.IdKaryawan == data.IdKaryawan).FirstOrDefault();
                if (clientMessage.JenisPesan == "update")
                {
                    data2.Nama = data.Nama;
                }
                else if (clientMessage.JenisPesan == "insert" && data2 == null)
                {
                    karyawanService.ListKaryawan.Add(data);
                }
            }
            else if (clientMessage.JenisPesan == "delete")
            {
                var item = _listKaryawan.Where(i => i.IdKaryawan == (long)clientMessage.Id_PrimaryKey).FirstOrDefault();
                karyawanService.ListKaryawan.Remove(item);
                await Application.Current.MainPage.Navigation.PopAsync();
            }
           // localNotificationsService.ShowNotification(title, clientMessage.Message, new Dictionary<string, string>());
            var rnd = new Random();
            var notification = new NotificationRequest
            {
                
                BadgeNumber = 1,
                Description = clientMessage.IsiPesan,
                Title = title,
                ReturningData = clientMessage.NamaHalaman,
                NotificationId = rnd.Next(1,1000)
            };

           await NotificationCenter.Current.Show(notification);
        }

        private async void MoveToInsertPage()
        {
            await signalRService.Disconnect();
            await Application.Current.MainPage.Navigation.PushAsync(new cpInsertKaryawan());
        }

        
        private async void SetKaryawanSelected(object obj)
        {
            await signalRService.Disconnect();
            var data = obj as DataKaryawan;
            detailKaryawanService.SetKaryawanSelected(data);
            await Application.Current.MainPage.Navigation.PushAsync(new cpDetilKaryawan());
        }

        private async void OnLogout(object obj)
        {
            await signalRService.Disconnect();
            Preferences.Remove("divisi");
            await Application.Current.MainPage.Navigation.PushAsync(new cpLogin());
        }
    }
}