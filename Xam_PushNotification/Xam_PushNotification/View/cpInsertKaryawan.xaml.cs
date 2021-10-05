using Xam_PushNotification.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xam_PushNotification.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class cpInsertKaryawan : ContentPage
    {
        vmNewKaryawan vmNewKaryawan = new vmNewKaryawan();
        public cpInsertKaryawan()
        {
            InitializeComponent();
        }

        //protected async override void OnAppearing()
        //{
        //    base.OnAppearing();
        //    await vmNewKaryawan.ConnectSignalR();
        //}
    }
}