using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MamoScope.Core.Interfaces;
using MamoScope.Data;
using MamoScope.Models;
using MamoScope.Navigations;
using MamoScope.Services;
using MamoScope.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Navigation;



namespace MamoScope.ViewModels
{
    
    public partial class TestRecordsViewModel : ViewModelBase
    {

        [ObservableProperty]
        private string serialNumber;

        [ObservableProperty]
        private string voltajDegeri;

        [ObservableProperty]
        private string tarih;

        [ObservableProperty]
        private string testSonucu;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private bool isResultDialogOpen;

        [ObservableProperty]
        private bool resultIsSucces;

        [ObservableProperty]
        private string resultMessage;


        private readonly IMotorDriversService motorDriversService;
        private readonly MotorDriversStore motorDriversStore;
        private readonly INavigationService navigationService;



        public TestRecordsViewModel(IMotorDriversService motorDriversService,MotorDriversStore motorDriversStore,INavigationService navigationService)

        {
            this.motorDriversService = motorDriversService;
            this.motorDriversStore = motorDriversStore;
            this.navigationService = navigationService;
            tarih = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
        }


        [RelayCommand]
        private void CloseResultDialog()
        {
            IsResultDialogOpen = false;
        }

        [RelayCommand]
        private async Task VoltajTestveKaydetAsync()
        {
            if (string.IsNullOrEmpty(VoltajDegeri) || string.IsNullOrEmpty(SerialNumber))
            {
                MessageBox.Show("Lütfen önce voltaj ve seri numarası girin veya simüle edin!");
                return;
            }

            string temizVoltajMetni = VoltajDegeri?.Trim().Replace(',', '.') ?? "0";

            if (!double.TryParse(temizVoltajMetni, NumberStyles.Any, CultureInfo.InvariantCulture, out double gercekVoltaj))
            {
                MessageBox.Show("Geçerli bir voltaj değeri girin (örnek: 24.5)");
                return;
            }

            try
            {
                
                var (basariliMi, sonucMetni) = await motorDriversStore.TestEtVeKaydetAsync(SerialNumber, gercekVoltaj);

                TestSonucu = sonucMetni;
                ResultIsSucces = basariliMi;
                ResultMessage = $"Seri No: {SerialNumber}\nVoltaj: {gercekVoltaj}V\nSonuç: {TestSonucu}";
                IsResultDialogOpen = true;

                SerialNumber = string.Empty;
                VoltajDegeri = string.Empty;
            }

            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Tekrar Kayıt Uyarısı", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Veri tabanı hatası: {ex.Message}");
            }
        }


        [RelayCommand] 
        private void TestVerisiSimuleEt()
        {
            var (serialNumber, voltaj) = motorDriversService.TestVerisiSimuleEt();
            SerialNumber = serialNumber;
            VoltajDegeri = voltaj.ToString();
            TestSonucu = "";
        }


        [RelayCommand] 
        private async Task GecmisKayitlariAc()
        {
            IsLoading = true;
            await Task.Delay(50);

            navigationService.NavigateTo<PastRecordsViewModel>();

            IsLoading = false;

        } 


    }
}
