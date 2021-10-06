namespace Xam_PushNotification.Model
{
    public class ClientMessage
    {
        public string Divisi { get; set; }
        public string IsiPesan { get; set; }
        public string JenisPesan { get; set; }
        public object Id_PrimaryKey { get; set; }
        public string NamaHalaman { get; set; } //Digunakan di Xamarin, untuk redirect ke halaman tertentu ketika user klik notifikasi di status bar.

    }
}
