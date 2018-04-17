using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.ViewModels
{
    public class TransactionLogView
    {
        public string TransactionId { get; set; }
        [Required]
        public string PatientUserId { get; set; }
        [Required]
        public int? TransactionType { get; set; }
        public string TherapistUserId { get; set; }
        public string LinkToActivity { get; set; }
        [Required]
        public int? Duration { get; set; }
        [Required]
        public string Comment { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string TherapistName { get; set; }
        public string UserName { get; set; }
    }
}
