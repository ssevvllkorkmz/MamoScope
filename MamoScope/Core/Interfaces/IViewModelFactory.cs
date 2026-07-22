using MamoScope.Models;
using MamoScope.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;


namespace MamoScope.Core.Interfaces
{
    public interface IViewModelFactory
    {
     RecordDetailsViewModel CreateDetailViewModel(MotorDrivers record);
    }
}
