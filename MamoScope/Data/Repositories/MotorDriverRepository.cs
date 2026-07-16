using System;
using System.Collections.Generic;
using System.Text;
using MamoScope.Data;
using MamoScope.Models;
using Microsoft.EntityFrameworkCore;

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
    }
}
