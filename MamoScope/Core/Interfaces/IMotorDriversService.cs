using MamoScope.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MamoScope.Core.Interfaces
{
    public interface IMotorDriversService
    {
        (string SerialNumber, double VoltajDegeri) TestVerisiSimuleEt();
        Task<(bool BasariliMi, string TestSonucu, MotorDrivers Kayit)> VoltajTestVeKaydetAsync(string serialNumber, double voltaj);
        Task<List<MotorDrivers>> GetPastRecordsAsync();
        Task UpdateAsync(MotorDrivers kayit);
        Task DeleteAsync(int id);
    }
}