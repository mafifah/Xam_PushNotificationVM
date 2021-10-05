﻿using MvvmHelpers;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xam_PushNotification.Model;
using Xam_PushNotification.Service;
using Xam_PushNotification.View;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Xam_PushNotification.ViewModel
{
    public class vmKaryawanDetil : ObservableObject
    {
        private string title = "Data Karyawan";

        private DataKaryawan _karyawanSelected;
        public DataKaryawan KaryawanSelected { get => _karyawanSelected; set => SetProperty(ref _karyawanSelected, value); }
        
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }

        IDetailKaryawanService detailKaryawanService;
        IKaryawanService karyawanService;
        ISignalRService signalRService;
        public readonly ILocalNotificationsService localNotificationsService;
        public vmKaryawanDetil()
        {
            detailKaryawanService = DependencyService.Get<IDetailKaryawanService>();
            karyawanService = DependencyService.Get<IKaryawanService>();
            signalRService = DependencyService.Get<ISignalRService>();
            localNotificationsService = DependencyService.Get<ILocalNotificationsService>();
            KaryawanSelected = detailKaryawanService.KaryawanSelected;
            UpdateCommand = new Command(UpdateKaryawan);
            DeleteCommand = new Command(DeleteKaryawan);
            ConnectSignalR();
        }

        protected async Task ConnectSignalR()
        {
            try
            {
                signalRService.ReceiveMessage(null);
                await signalRService.Connect();
            }
            catch (Exception e)
            {
                var msg = e.Message;
            }
        }

        private async void DeleteKaryawan()
        {
            await detailKaryawanService.DeleteKaryawan(_karyawanSelected);
            await signalRService.SendMessage(title, "delete", false, KaryawanSelected.IdKaryawan);
            karyawanService.ListKaryawan.Remove(_karyawanSelected);
            await signalRService.Disconnect();
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        private async void UpdateKaryawan()
        {
            await detailKaryawanService.UpdateKaryawan(_karyawanSelected);
            await signalRService.SendMessage(title, "update", false, KaryawanSelected.IdKaryawan);
            await signalRService.Disconnect();
            await Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}