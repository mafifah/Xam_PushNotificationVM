using Microsoft.AspNetCore.SignalR.Client;
using MvvmHelpers;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xam_PushNotification.Model;
using Xam_PushNotification.Service;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(SignalRService))]
namespace Xam_PushNotification.Service
{
    public class SignalRService : ObservableObject, ISignalRService
    {
        private string _status;

        public string Status
        {
            get=>_status;
        }

        private HubConnection hubConnection;
        public SignalRService()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            HttpClient client = new HttpClient(clientHandler);
            client.BaseAddress = new Uri("https://newfcsignalr.azurewebsites.net/chatHub");
            hubConnection = new HubConnectionBuilder().WithUrl(client.BaseAddress)
            .Build();
        }
        public async Task Connect()
        {
            if (hubConnection.ConnectionId != null)
            {
                await Disconnect();
            }
            await hubConnection.StartAsync();
            _status = hubConnection.State.ToString();
            await hubConnection.InvokeAsync("MulaiKoneksi", Preferences.Get("divisi", null)); 
        }

        public async Task Disconnect()
        {
            try
            {
                await hubConnection.InvokeAsync("StopKoneksi", Preferences.Get("divisi", null));
                await hubConnection.StopAsync();
                _status = hubConnection.State.ToString();
            }
            catch (Exception e)
            {
                var msg = e.Message;
            }
        }

        public async Task SendMessage(string title, string method, string namaHalaman, bool isBroadcast, object id = null)
        {
            var msg = $"xam_{title} {method}";
            var message = new ClientMessage
            {
                IsiPesan = msg,
                JenisPesan = method,
                Divisi = Preferences.Get("divisi", null),
                Id_PrimaryKey = id,
                NamaHalaman = namaHalaman,
            };
            if (isBroadcast)
            {
                await hubConnection.InvokeAsync("KirimPesanBroadcast", message);
            }
            else
            {
                await hubConnection.InvokeAsync("KirimPesan", message);
            }
        }

        public void ReceiveMessage(Action<ClientMessage> GetMessage = null, bool isBroadcast = false)
        {
            if (isBroadcast)
            {
                hubConnection.On("KirimPesanBroadcast", GetMessage);
            }
            else
            {
                hubConnection.On("KirimPesan", GetMessage);
            }
        }
    }
}