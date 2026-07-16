using MamoScope.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MamoScope.Data.Repositories
{
    public interface IMotorDriversRepository
    {
        Task<List<MotorDrivers>> GetAllAsync();
        Task<MotorDrivers?> GetBySerialNumberAsync(string serialNumber);
        Task AddAsync(MotorDrivers kayit);
    }
}
