using MamoScope.Core.Interfaces;
using MamoScope.Data;
using MamoScope.Models;
using MamoScope.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MamoScope.Core.Interfaces;
using MamoScope.Data.Repositories;


namespace MamoScope.Services
{
    public class MotorDriversService : IMotorDriversService
    {
        private readonly IMotorDriversRepository _repository;

        public MotorDriversService(IMotorDriversRepository repository)
        {
            _repository = repository;
        }

        public (string SerialNumber, double VoltajDegeri) TestVerisiSimuleEt()
        {
            var rnd = new Random();
            int rastgeleSayi = rnd.Next(1000, 9999);
            string serialNumber = $"OPT-DRV-{rastgeleSayi}";

            double uretilenVoltaj = 20.0 + (rnd.NextDouble() * 6.0);
            uretilenVoltaj = Math.Round(uretilenVoltaj, 1);

            return (serialNumber, uretilenVoltaj);
        }

        public async Task<(bool BasariliMi, string TestSonucu, MotorDrivers Kayit)> VoltajTestVeKaydetAsync(string serialNumber, double voltaj)
        {

            var mevcutKayit = await _repository.GetBySerialNumberAsync(serialNumber);
            if (mevcutKayit != null)
            {
                throw new InvalidOperationException($"'{serialNumber}' seri numaralı kayıt zaten mevcut. Lütfen yeni bir test simüle edin.");
            }

            bool basariliMi = voltaj >= 23.5 && voltaj <= 24.5;
            string testSonucu = basariliMi ? "Başarılı" : "Başarısız";

            var yeniKayit = new MotorDrivers
            {
                SerialNumber = serialNumber,
                Voltage = voltaj,
                IsPassed = basariliMi,
                TestDate = DateTime.Now
            };

            await _repository.AddAsync(yeniKayit);

            return (basariliMi, testSonucu, yeniKayit);
        }

        public Task<List<MotorDrivers>> GetPastRecordsAsync()
        {
            return _repository.GetAllAsync();
        }
    }
}
