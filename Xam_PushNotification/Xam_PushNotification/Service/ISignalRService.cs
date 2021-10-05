using System;
using System.Threading.Tasks;
using Xam_PushNotification.Model;

namespace Xam_PushNotification.Service
{
    public interface ISignalRService
    {
        public string Status { get; }
        Task Connect();
        Task Disconnect();
        void ReceiveMessage(Action<ClientMessage> GetMessage = null, bool isBroadcast = false);
        Task SendMessage(string title, string method, bool isBroadcast, long id);
    }
}