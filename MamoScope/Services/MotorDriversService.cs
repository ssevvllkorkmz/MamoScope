using MamoScope.Core.Interfaces;
using MamoScope.Data;
using MamoScope.Data.Repositories;
using MamoScope.Models;
using MamoScope.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace MamoScope.Services
{
    public class MotorDriversService : IMotorDriversService
    {
        private readonly IMotorDriversRepository _repository;
        
        private static readonly Regex SeriNumarasiFormati = new Regex(@"^OPT-DRV-\d{4}$", RegexOptions.Compiled);

        public MotorDriversService(IMotorDriversRepository repository)
        {
            _repository = repository;
        }

        private void SeriNumarasiFormatiniDogrula(string serialNumber)
        {
            if (string.IsNullOrWhiteSpace(serialNumber) || !SeriNumarasiFormati.IsMatch(serialNumber))
            {
                throw new InvalidOperationException("Seri numarası 'OPT-DRV-XXXX' formatında olmalıdır (örnek: OPT-DRV-4521).");
            }
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

            SeriNumarasiFormatiniDogrula(serialNumber);
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

        public async Task UpdateAsync(MotorDrivers kayit)
        {

            SeriNumarasiFormatiniDogrula(kayit.SerialNumber);

            var ayniSeriNumarali = await _repository.GetBySerialNumberAsync(kayit.SerialNumber);
            if (ayniSeriNumarali != null && ayniSeriNumarali.Id != kayit.Id)
            {
                throw new InvalidOperationException($"'{kayit.SerialNumber}' seri numarası başka bir kayıtta zaten kullanılıyor.");
            }

            bool basariliMi = kayit.Voltage >= 23.5 && kayit.Voltage <= 24.5;
            kayit.IsPassed = basariliMi;

            await _repository.UpdateAsync(kayit);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
