using System;
using System.Collections.Generic;

namespace OneDirect.Models
{
    public partial class Appointments
    {
        public string AppointmentId { get; set; }
        public string PatientUserId { get; set; }
        public string AppointmentType { get; set; }
        public string TherapistUserId { get; set; }
        public string SupportUserId { get; set; }
        public string AvailableSlots { get; set; }
        public string Urikey { get; set; }
        public int? Status { get; set; }
        public int? Duration { get; set; }
        public string PatientComment { get; set; }
        public string TherapistSupportComment { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? ConfirmedSlot { get; set; }
        public string SlotId { get; set; }
        public string SlotTime { get; set; }
        public string Timezone { get; set; }
    }
}
