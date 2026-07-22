using MamoScope.Core.Interfaces;
using MamoScope.Models;
using MamoScope.Navigations;
using MamoScope.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Navigation;

namespace MamoScope.ViewModels.Factories
{
    public class ViewModelFactory : IViewModelFactory
    {
        private readonly IServiceProvider _serviceProvider;
        
        public ViewModelFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            
        }
        public RecordDetailsViewModel CreateDetailViewModel(MotorDrivers record)
        {
            var navigationService = _serviceProvider.GetRequiredService<INavigationService>();
            var store = _serviceProvider.GetRequiredService<MotorDriversStore>();
            return new RecordDetailsViewModel(record,navigationService,store);
        }
    }
}
