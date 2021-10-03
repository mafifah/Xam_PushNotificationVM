using MvvmHelpers;

namespace Xam_PushNotification.Model
{
    public class DataKaryawan : ObservableObject
    {
        public long IdKaryawan { get; set; }
        private string _nama;
        public string Nama { get => _nama; set => SetProperty(ref _nama, value); }
    
    }
}
