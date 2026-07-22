using System;
using System.Collections.Generic;
using System.Text;
using MamoScope.Core.Interfaces;
using MamoScope.Models;
using MamoScope.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace MamoScope.Navigations
{
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private object? _currentView;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            
        }

        public object? CurrentView
        {
            get => _currentView;
            private set
            {
                _currentView = value;
                CurrentViewChanged?.Invoke();
            }
        }

        public event Action? CurrentViewChanged;

        public void NavigateTo<TViewModel>() where TViewModel : notnull
        {
            CurrentView = _serviceProvider.GetRequiredService<TViewModel>();
        }

        public void NavigateTo(ViewModelBase viewModel)
        {
            CurrentView = viewModel;
        }
    }
}
