using System;
using System.Collections.Generic;
using System.Text;
using MamoScope.Data;
using MamoScope.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MamoScope.Core.Interfaces;


namespace MamoScope.Services
{
    public class MotorDriversStore
    {
        private readonly IMotorDriversService _service;

        public ObservableCollection<MotorDrivers> Kayitlar { get; } = new();

        public MotorDriversStore(IMotorDriversService service)
        {
            _service = service;
        }

        public async Task YukleAsync()
        {
            var liste = await _service.GetPastRecordsAsync();
            Kayitlar.Clear();
            foreach (var item in liste)
                Kayitlar.Add(item);
        }

        public async Task<(bool BasariliMi, string TestSonucu)> TestEtVeKaydetAsync(string serialNumber, double voltaj)
        {
            var (basariliMi, testSonucu, kayit) = await _service.VoltajTestVeKaydetAsync(serialNumber, voltaj);
            Kayitlar.Insert(0, kayit);
            return (basariliMi, testSonucu);
        }
    }
}
