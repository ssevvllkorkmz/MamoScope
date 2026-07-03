using System;
using System.Collections.Generic;
using System.Text;

namespace MamoScope.Models
{
   public class MotorDrivers
    {
        public int Id { get; set; }
        public string SerialNumber { get; set; }
        public double Voltage { get; set; }
        public DateTime TestDate { get; set; }
        public bool IsPassed { get; set; }

    }
}
