using Xam_PushNotification.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xam_PushNotification.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class cpDetilKaryawan : ContentPage
    {
        vmKaryawanDetil vmKaryawanDetil = new vmKaryawanDetil();
        public cpDetilKaryawan()
        {
            InitializeComponent();
        }

        //protected async override void OnAppearing()
        //{
        //    base.OnAppearing();
        //    await vmKaryawanDetil.ConnectSignalR();
        //}
    }
}