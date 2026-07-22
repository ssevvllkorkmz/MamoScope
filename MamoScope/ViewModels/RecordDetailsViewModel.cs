using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MamoScope.Core.Interfaces;
using MamoScope.Models;
using MamoScope.Navigations;
using MamoScope.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Navigation;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MamoScope.ViewModels
{
    public partial class RecordDetailsViewModel : ViewModelBase
    {

        private readonly INavigationService _navigationService;
        private readonly MotorDriversStore _store;

        [ObservableProperty]
        private MotorDrivers _editableRecord;

        [ObservableProperty]
        private bool _isResultDialogOpen;

        [ObservableProperty]
        private string _resultMessage;

        [ObservableProperty]
        private string _voltajDegeriText;

        public RecordDetailsViewModel(MotorDrivers selectedRecord,INavigationService navigationService,MotorDriversStore store)
        {
            _navigationService = navigationService;
            _store = store;


            EditableRecord = new MotorDrivers
            {
                Id = selectedRecord.Id,
                Voltage = selectedRecord.Voltage,
                SerialNumber = selectedRecord.SerialNumber,
                TestDate = selectedRecord.TestDate,
                IsPassed = selectedRecord.IsPassed
            };

            VoltajDegeriText = selectedRecord.Voltage.ToString(CultureInfo.InvariantCulture);
        }

        [RelayCommand]
        private void GoBack()
        {
            _navigationService.NavigateTo<PastRecordsViewModel>();
        }


        [RelayCommand] 
        private async Task SaveChangesAsync()
        {
            if (string.IsNullOrWhiteSpace(EditableRecord.SerialNumber))
            {
                ResultMessage = "Seri numarası boş olamaz.";
                IsResultDialogOpen = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(VoltajDegeriText))
            {
                ResultMessage = "Voltaj değeri boş olamaz.";
                IsResultDialogOpen = true;
                return;
            }

            string temizMetin = VoltajDegeriText.Trim().Replace(',', '.');
            if (!double.TryParse(temizMetin, NumberStyles.Any, CultureInfo.InvariantCulture, out double gercekVoltaj))
            {
                ResultMessage = "Geçerli bir voltaj değeri girin (örnek: 24.5)";
                IsResultDialogOpen = true;
                return;
            }

            EditableRecord.Voltage = gercekVoltaj;

            try
            {
                await _store.UpdateAsync(EditableRecord);
                _navigationService.NavigateTo<PastRecordsViewModel>();
            }
            catch (Exception ex)
            {
                ResultMessage=$"Güncelleme sırasında hata oluştu: {ex.Message}";
                IsResultDialogOpen = true;
            }
        }

        [RelayCommand] 
        private void CloseResultDialog()
        {
            IsResultDialogOpen = false;
        }

    }
}
