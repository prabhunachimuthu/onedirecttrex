using System;
using System.Collections.Generic;

namespace OneDirect.Models
{
    public partial class ActivityLog
    {
        public string TransactionId { get; set; }
        public string TransactionType { get; set; }
        public string UserId { get; set; }
        public int? PatientId { get; set; }
        public string LinkToActivity { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int? Duration { get; set; }
        public string Comment { get; set; }

        public virtual Patient Patient { get; set; }
        public virtual User User { get; set; }
    }
}
