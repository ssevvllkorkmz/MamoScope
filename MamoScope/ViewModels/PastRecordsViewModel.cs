using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MamoScope.Data;
using MamoScope.Models;
using MamoScope.Navigations;
using MamoScope.Services;
using MamoScope.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Navigation;

namespace MamoScope.ViewModels
{
    public partial class PastRecordsViewModel : ViewModelBase
    {
        [ObservableProperty]
        private bool isLoading;

        private readonly MotorDriversStore _store;
        private readonly INavigationService _navigationService;

        public ObservableCollection<MotorDrivers> Kayitlar => _store.Kayitlar;

        public PastRecordsViewModel(MotorDriversStore store,INavigationService navigationService)
        {
            _store = store;
            _navigationService = navigationService;
            _ = YukleAsync();
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
            _navigationService.NavigateTo<TestRecordsViewModel>();
        }
    }
}