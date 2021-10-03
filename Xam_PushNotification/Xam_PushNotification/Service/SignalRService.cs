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
            get => _status;
            set => SetProperty(ref _status, value);
        }

        private HubConnection hubConnection;
        public SignalRService()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            HttpClient client = new HttpClient(clientHandler);
            client.BaseAddress = new Uri("https://newfcsignalr.azurewebsites.net/chatHub");
            hubConnection = new HubConnectionBuilder().WithUrl(client.BaseAddress)
            .WithAutomaticReconnect().Build();
        }
        public async Task Connect()
        {
            await hubConnection.StartAsync();
            Status = hubConnection.State.ToString();
            await hubConnection.InvokeAsync("OnConnect", Preferences.Get("divisi", null));
        }

        public async Task Disconnect()
        {
            await hubConnection.InvokeAsync("OnDisconnect", Preferences.Get("divisi", null));
            await hubConnection.StopAsync();

        }

        public async Task SendMessage(ClientMessage clientMessage, bool isBroadcast)
        {
            if (isBroadcast)
            {
                await hubConnection.InvokeAsync("BroadcastMessage", clientMessage);
            }
            else
            {
                await hubConnection.InvokeAsync("SendMessage", clientMessage);
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