using System;
using System.Collections.Generic;
using System.Text;

namespace Xam_PushNotification.Model
{
    public class ClientMessage
    {
        public string Message { get; set; }
        public string Divisi { get; set; }
        public string Method { get; set; }
        public long IdKaryawan { get; set; }
    }
}
