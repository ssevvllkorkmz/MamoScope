using MamoScope.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MamoScope.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Models.MotorDrivers> MotorDrivers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
             @"Server=(localdb)\mssqllocaldb;Database=MamoScopeDb;Trusted_Connection=True;",
                sqlServerOptionsAction: sqlOptions =>
                {

                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                });
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<MotorDrivers>(entity =>
            {
                entity.HasKey(e => e.Id); 
                entity.Property(e => e.SerialNumber).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Voltage).IsRequired();
                entity.Property(e => e.TestDate).IsRequired();
                entity.Property(e => e.IsPassed).IsRequired();
            });
        }
    }
}
