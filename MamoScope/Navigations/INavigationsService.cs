using MamoScope.Models;
using MamoScope.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace MamoScope.Navigations
{
    public interface INavigationService
    {
        object? CurrentView { get; }
        event Action? CurrentViewChanged;

        void NavigateTo<TViewModel>() where TViewModel : notnull;

        void NavigateTo(ViewModelBase viewModel);
    }
}
