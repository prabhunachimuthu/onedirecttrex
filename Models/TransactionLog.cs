using System;
using System.Collections.Generic;

namespace OneDirect.Models
{
    public partial class TransactionLog
    {
        public string TransactionId { get; set; }
        public string PatientUserId { get; set; }
        public int? TransactionType { get; set; }
        public string TherapistUserId { get; set; }
        public string LinkToActivity { get; set; }
        public int? Duration { get; set; }
        public string Comment { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
