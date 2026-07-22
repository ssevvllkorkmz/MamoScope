using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MamoScope.Core.Interfaces;
using MamoScope.Data;
using MamoScope.Models;
using MamoScope.Navigations;
using MamoScope.Services;
using MamoScope.ViewModels.Factories;
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

        [ObservableProperty]
        private MotorDrivers _selectedRecord;

        [ObservableProperty]
        private bool _isDeleteConfirmOpen;

        [ObservableProperty]
        private MotorDrivers? _recordToDelete;

        private readonly MotorDriversStore _store;
        private readonly INavigationService _navigationService;
        private readonly IViewModelFactory _viewModelFactory;

        public ObservableCollection<MotorDrivers> Kayitlar => _store.Kayitlar;

        public PastRecordsViewModel(MotorDriversStore store,INavigationService navigationService,IViewModelFactory viewModelFactory)
        {
            _store = store;
            _navigationService = navigationService;
            _ = YukleAsync();
            _viewModelFactory = viewModelFactory;
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

        [RelayCommand]
        private void SelectRecord(MotorDrivers record)
        {
            if (record == null) return;

            var detailVm = _viewModelFactory.CreateDetailViewModel(record);
            _navigationService.NavigateTo(detailVm);
        }

        [RelayCommand]
        private void RequestDelete(MotorDrivers record)
        {
            RecordToDelete = record;
            IsDeleteConfirmOpen = true;
        }

        [RelayCommand]
        private async Task ConfirmDeleteAsync()
        {
            if (RecordToDelete == null) return;

            await _store.DeleteAsync(RecordToDelete.Id);

            IsDeleteConfirmOpen = false;
            RecordToDelete = null;
        }

        [RelayCommand]
        private void CancelDelete()
        {
            IsDeleteConfirmOpen = false;
            RecordToDelete = null;
        }

    }
}