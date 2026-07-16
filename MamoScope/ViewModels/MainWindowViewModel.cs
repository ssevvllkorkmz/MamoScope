using CommunityToolkit.Mvvm.ComponentModel;
using MamoScope.Navigations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace MamoScope.ViewModels
{
    [ObservableObject]

    public partial class MainWindowViewModel
    {
        private readonly INavigationService _navigationService;
        [ObservableProperty]
        private object _currentView;

        public MainWindowViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            _navigationService.CurrentViewChanged += OnCurrentViewChanged;
        }
        private void OnCurrentViewChanged()
        {
            CurrentView = _navigationService.CurrentView;
        }
    }
}
