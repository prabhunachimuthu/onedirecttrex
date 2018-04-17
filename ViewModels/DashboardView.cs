using OneDirect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.ViewModels
{
    public class DashboardView
    {
        public PatientRx PatientRx { get; set; }
        public int MaxPain { get; set; }
        public DateTime? FirstUse { get; set; }

        public DateTime? LastUse { get; set; }
        public int UpRom1 { get; set; }
        public int DownRom1 { get; set; }
        public int UpRom2 { get; set; }
        public int DownRom2 { get; set; }
        public int UpRom3 { get; set; }
        public int DownRom3 { get; set; }
        public string PercentageofUsage { get; set; }
        public int TotalSession { get; set; }
        public Progress Progress { get; set; }
    }
}
