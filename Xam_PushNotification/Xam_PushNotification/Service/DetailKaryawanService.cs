using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using gRPCServer;
using MvvmHelpers;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xam_PushNotification.Model;
using Xam_PushNotification.Service;
using Xamarin.Forms;

[assembly: Dependency(typeof(DetailKaryawanService))]
namespace Xam_PushNotification.Service
{
    public class DetailKaryawanService : ObservableObject, IDetailKaryawanService
    {
        private DataKaryawan _karyawanSelected;
        public DataKaryawan KaryawanSelected { get => _karyawanSelected; }

        public void SetKaryawanSelected(DataKaryawan item) => _karyawanSelected = item;
        public async Task<bool> UpdateKaryawan(DataKaryawan dataKaryawan)
        {
            var output = false;
            try
            {
                var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
                var channel = GrpcChannel.ForAddress("https://fcgrpcserver.azurewebsites.net/", new GrpcChannelOptions { HttpClient = httpClient });
                var client = new Karyawan.KaryawanClient(channel);
                var request = client.UpdateKaryawan(new T1Karyawan { IdKaryawan = dataKaryawan.IdKaryawan, NamaLengkap = dataKaryawan.Nama });
                if (request.Result == "Data Berhasil Disimpan")
                {
                    output = true;
                }
            }
            catch (Exception e)
            {

                var msg = e.Message;
            }

            return output;
        }
        public async Task<bool> DeleteKaryawan(DataKaryawan dataKaryawan)
        {
            var output = false;
            try
            {
                var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
                var channel = GrpcChannel.ForAddress("https://fcgrpcserver.azurewebsites.net/", new GrpcChannelOptions { HttpClient = httpClient });
                var client = new Karyawan.KaryawanClient(channel);
                var request = client.DeleteKaryawan(new T1KaryawanRequest { IdKaryawan = dataKaryawan.IdKaryawan });
                if (request.Result == "Data Berhasil dihapus")
                {
                    output = true;
                }
            }
            catch (Exception e)
            {

                var msg = e.Message;
            }

            return output;
        }
    }
}
