using MamoScope.Data;
using MamoScope.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace MamoScope.Data.Repositories
{
    public class MotorDriversRepository : IMotorDriversRepository
    {
        private readonly IDbContextFactory<AppDbContext> _dbFactory;

        public MotorDriversRepository(IDbContextFactory<AppDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<List<MotorDrivers>> GetAllAsync()
        {
            using var db = _dbFactory.CreateDbContext();
            return await db.MotorDrivers
                .OrderByDescending(x => x.TestDate)
                .ToListAsync();
        }

        public async Task AddAsync(MotorDrivers kayit)
        {
            using var db = _dbFactory.CreateDbContext();
            db.MotorDrivers.Add(kayit);
            await db.SaveChangesAsync();
        }

        public async Task<MotorDrivers?> GetBySerialNumberAsync(string serialNumber)
        {
            using var db = _dbFactory.CreateDbContext();
            return await db.MotorDrivers
                .FirstOrDefaultAsync(x => x.SerialNumber == serialNumber);
        }

        public async Task UpdateAsync(MotorDrivers kayit)
        {
            using var db = _dbFactory.CreateDbContext();
            db.MotorDrivers.Update(kayit);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var db = _dbFactory.CreateDbContext();

            var kayit = await db.MotorDrivers.FindAsync(id);
            if (kayit == null)
                throw new InvalidOperationException($"Id={id} olan kayıt bulunamadı.");

            db.MotorDrivers.Remove(kayit);
            await db.SaveChangesAsync();
        }

    }
}
