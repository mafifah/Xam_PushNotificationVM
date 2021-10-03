using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using gRPCServer;
using MvvmHelpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xam_PushNotification.Model;
using Xam_PushNotification.Service;
using Xamarin.Forms;
[assembly: Dependency(typeof(KaryawanService))]
namespace Xam_PushNotification.Service
{
    public class KaryawanService : ObservableObject, IKaryawanService
    {
        private ObservableCollection<DataKaryawan> _listKaryawan = new ObservableCollection<DataKaryawan>();
        public ObservableCollection<DataKaryawan> ListKaryawan { get => _listKaryawan; set => SetProperty(ref _listKaryawan, value); }
        public DataKaryawan DataKaryawan { get; set; }
        public KaryawanService()
        {
            GetDataKaryawan();
        }

        public async Task<ObservableCollection<DataKaryawan>> GetDataKaryawan()
        {
            try
            {
                var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
                var channel = GrpcChannel.ForAddress("https://fcgrpcserver.azurewebsites.net/", new GrpcChannelOptions { HttpClient = httpClient });
                var client = new Karyawan.KaryawanClient(channel);
                var request = new Empty();
                _listKaryawan = new ObservableCollection<DataKaryawan>();
                using (var reply = client.GetKaryawan(request))
                    while (await reply.ResponseStream.MoveNext(System.Threading.CancellationToken.None))
                    {
                        T1Karyawan karyawan = reply.ResponseStream.Current;
                        ListKaryawan.Add(new DataKaryawan
                        {
                            IdKaryawan = karyawan.IdKaryawan,
                            Nama = karyawan.NamaLengkap,

                        });
                    }
                return _listKaryawan;
            }
            catch (Exception ex)
            {

                await Application.Current.MainPage.DisplayAlert("Pesan", ex.Message, "OK");
            }
            return null;
        }

        public async Task<DataKaryawan> GetKaryawanById(long Id)
        {
            DataKaryawan = new DataKaryawan();
            var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
            var channel = GrpcChannel.ForAddress("https://fcgrpcserver.azurewebsites.net/", new GrpcChannelOptions { HttpClient = httpClient });
            var client = new Karyawan.KaryawanClient(channel);
            var request = new T1KaryawanRequest { IdKaryawan = Id };
            using (var reply = client.GetKaryawanById(request))
                while (await reply.ResponseStream.MoveNext(System.Threading.CancellationToken.None))
                {
                    T1Karyawan karyawan = reply.ResponseStream.Current;
                    DataKaryawan.Nama = karyawan.NamaLengkap;
                    DataKaryawan.IdKaryawan = karyawan.IdKaryawan;
                }
            return DataKaryawan;
        }

        public async Task<bool> InsertKaryawan(long id, string nama)
        {
            var output = false;
            try
            {
                var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
                var channel = GrpcChannel.ForAddress("https://fcgrpcserver.azurewebsites.net/", new GrpcChannelOptions { HttpClient = httpClient });
                var client = new Karyawan.KaryawanClient(channel);
                var request = client.InsertKaryawan(new T1Karyawan { IdKaryawan = id, NamaLengkap = nama });
                if (request.Result == "Data Berhasil Disimpan")
                {
                    output = true;
                    _listKaryawan.Add(new DataKaryawan
                    {
                        IdKaryawan = id,
                        Nama = nama,
                    });
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }

            return output;
        }
    }
}