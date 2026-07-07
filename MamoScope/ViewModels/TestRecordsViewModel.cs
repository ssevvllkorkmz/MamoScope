using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MamoScope.Data;
using MamoScope.Models;
using MamoScope.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;


namespace MamoScope.ViewModels
{
    
    public partial class TestRecordsViewModel : ViewModelBase
    {

        [ObservableProperty]
        private string _serialNumber;

        [ObservableProperty]
        private string _voltajDegeri;

        [ObservableProperty]
        private string _tarih;

        [ObservableProperty]
        private string _testSonucu;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _isResultDialogOpen;

        [ObservableProperty]
        private bool _resultIsSucces;

        [ObservableProperty]
        private string _resultMessage;


        private readonly IDbContextFactory<AppDbContext> _dbFactory;


        public TestRecordsViewModel(IDbContextFactory<AppDbContext> dbFactory)

        {
            _dbFactory = dbFactory; 
            _tarih = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
        }

        [RelayCommand]
        private void CloseResultDialog()
        {
            IsResultDialogOpen = false;
        }

        [RelayCommand] 
        private void VoltajTest()
        {
            if (string.IsNullOrEmpty(_voltajDegeri) || string.IsNullOrEmpty(_serialNumber))
            {
                System.Windows.MessageBox.Show("Lütfen önce voltaj ve seri numarası girin veya simüle edin!");
                return;
            }


            string temizVoltajMetni = _voltajDegeri?.Replace('.', ',') ?? "0";
            double gercekVoltaj = 0;
            double.TryParse(temizVoltajMetni, out gercekVoltaj);

            bool BasariliMi = false;

            if (gercekVoltaj >= 23.5 && gercekVoltaj <= 24.5)
            {
                _testSonucu = "BAŞARILI";
                BasariliMi = true;
            }
            else
            {
                _testSonucu = "BAŞARISIZ";
                BasariliMi = false;
            }

            var yeniKayit = new MotorDrivers
            {
                SerialNumber = this.SerialNumber ?? "",
                Voltage = gercekVoltaj,
                TestDate = DateTime.Now,
                IsPassed = BasariliMi
            };


            try
            {
                using var db = _dbFactory.CreateDbContext();
                db.MotorDrivers.Add(yeniKayit);
                db.SaveChanges();
                var pastVM = App.ServiceProvider.GetRequiredService<PastRecordsViewModel>();
                pastVM.VerileriYenile();

                ResultIsSucces = BasariliMi;
                ResultMessage = $"Seri No: { SerialNumber}\nVoltaj: { gercekVoltaj}V\nSonuç: { TestSonucu}";
                IsResultDialogOpen = true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Veri tabanı hatası: {ex.Message}\n\nLütfen SQL Server bağlantınızı veya AppDbContext dosyasındaki ConnectionString'i kontrol edin.");
            }
        }


        [RelayCommand]
        private void TestVerisiSimuleEt()
        {
            Random rnd = new Random();

            int rastgeleSayi = rnd.Next(1000, 9999);
            SerialNumber = $"OPT-DRV-{rastgeleSayi}";

            double uretilenVoltaj = 20.0 + (rnd.NextDouble() * 6.0);
            uretilenVoltaj = Math.Round(uretilenVoltaj, 1);
            VoltajDegeri = uretilenVoltaj.ToString();

            TestSonucu = "";
        }

        [RelayCommand]
        private async Task GecmisKayıtlarıAc()
        {

            IsLoading = true;

            await Task.Delay(50);

            var gecmisSayfa = App.ServiceProvider.GetRequiredService<PastRecordsView>();

            var gecmisSayfVM = App.ServiceProvider.GetRequiredService<PastRecordsViewModel>();
            gecmisSayfa.DataContext = gecmisSayfVM;

            var mainWindow = Application.Current.MainWindow;
            if (mainWindow != null && mainWindow.DataContext is MainWindowViewModel mainVM)
            {
                mainVM.CurrentView = gecmisSayfa;
            }

            IsLoading = false;
        }


    }
}
