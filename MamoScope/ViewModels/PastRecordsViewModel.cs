using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MamoScope.Data;
using MamoScope.Models;
using MamoScope.Services;
using MamoScope.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows;

namespace MamoScope.ViewModels
{
    public partial class PastRecordsViewModel : ViewModelBase
    {
        [ObservableProperty]
        private bool isLoading;

        private readonly MotorDriversStore _store;   // ← IMotorDriversService alanı kaldırıldı, artık gerek yok

        public ObservableCollection<MotorDrivers> Kayitlar => _store.Kayitlar;

        public PastRecordsViewModel(MotorDriversStore store)
        {
            _store = store;   // ← parametreyi alana doğru şekilde ata
        }

        [RelayCommand]
        private async Task YukleAsync()
        {
            IsLoading = true;
            await _store.YukleAsync();
            IsLoading = false;
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