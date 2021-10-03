using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xam_PushNotification.Model;

namespace Xam_PushNotification.Service
{
    public interface IKaryawanService
    {
        ObservableCollection<DataKaryawan> ListKaryawan { get; set; }
        Task<bool> InsertKaryawan(long id, string nama);
        Task<DataKaryawan> GetKaryawanById(long Id);
    }
}
