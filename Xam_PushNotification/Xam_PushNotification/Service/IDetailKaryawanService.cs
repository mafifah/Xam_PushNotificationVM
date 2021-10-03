using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xam_PushNotification.Model;

namespace Xam_PushNotification.Service
{
    public interface IDetailKaryawanService
    {
        DataKaryawan KaryawanSelected { get; }
        void SetKaryawanSelected(DataKaryawan item);
        Task<bool> UpdateKaryawan(DataKaryawan dataKaryawan);
        Task<bool> DeleteKaryawan(DataKaryawan dataKaryawan);
    }
}
