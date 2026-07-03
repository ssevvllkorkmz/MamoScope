using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace MamoScope.ViewModels
{
    [ObservableObject]
    
    public  partial class MainWindowViewModel
    {
        [ObservableProperty]
        private object _currentView;
    }
}
