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
            await hubConnection.StartAsync();
            _status = hubConnection.State.ToString();
            await hubConnection.InvokeAsync("OnConnect", Preferences.Get("divisi", null));
        }

        public async Task Disconnect()
        {
            await hubConnection.InvokeAsync("OnDisconnect", Preferences.Get("divisi", null));
            await hubConnection.StopAsync();
            _status = hubConnection.State.ToString();

        }

        public async Task SendMessage(string title, string method, bool isBroadcast, long id = 0)
        {
            var msg = $"xam_{title} {method}";
            var message = new ClientMessage
            {
                Message = msg,
                Method = method,
                Divisi = Preferences.Get("divisi", null),
                IdKaryawan = id
            };
            if (isBroadcast)
            {
                await hubConnection.InvokeAsync("BroadcastMessage", message);
            }
            else
            {
                await hubConnection.InvokeAsync("SendMessage", message);
            }
        }

        public void ReceiveMessage(Action<ClientMessage> GetMessage, bool isBroadcast = false)
        {
            if (isBroadcast)
            {
                hubConnection.On("BroadcastMessage", GetMessage);
            }
            else
            {
                hubConnection.On("SendMessage", GetMessage);
            }
        }
    }
}