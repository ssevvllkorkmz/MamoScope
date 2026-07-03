using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MamoScope.Data;
using MamoScope.Models;
using MamoScope.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace MamoScope.ViewModels
{
   
    public partial class PastRecordsViewModel : ViewModelBase
    {
        [ObservableProperty]
        private bool _isLoading;

        private readonly IDbContextFactory<AppDbContext> _dbFactory;

        [ObservableProperty]
        private ObservableCollection<MamoScope.Models.MotorDrivers> _gecmisTestler;


        public PastRecordsViewModel(IDbContextFactory<AppDbContext> dbFactory)

        {
            _dbFactory = dbFactory;
            VerileriVeriTabanindanYukle();
           
        }

      

        [RelayCommand]
        private async Task VerileriVeriTabanindanYukle()
        {
            IsLoading = true;
            try
            {
                using var db = _dbFactory.CreateDbContext();
                var sqlVerileri = await Task.Run(() => db.MotorDrivers.ToList());
              
                GecmisTestler = new ObservableCollection<MotorDrivers>(sqlVerileri);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veri yükleme hatası: {ex.Message}");
            }
            finally { IsLoading = false; }
        }

        [RelayCommand]
        private void KayıtSayfasiniAc()
        {
            
            var yeniSayfa = App.ServiceProvider.GetRequiredService<TestRecordsView>();
            var yeniSayfaVM = App.ServiceProvider.GetRequiredService<TestRecordsViewModel>();

           
            yeniSayfa.DataContext = yeniSayfaVM;

            
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow != null && mainWindow.DataContext is MainWindowViewModel mainVM)
            {
               
                mainVM.CurrentView = yeniSayfa;
            }
        }

    }
}
