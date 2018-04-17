using System;
using System.Collections.Generic;

namespace OneDirect.Models
{
    public partial class User
    {
        public User()
        {
            ActivityLog = new HashSet<ActivityLog>();
            AppointmentSchedule = new HashSet<AppointmentSchedule>();
            Availability = new HashSet<Availability>();
            EquipmentAssignment = new HashSet<EquipmentAssignment>();
            MessagesReceiver = new HashSet<Messages>();
            MessagesSender = new HashSet<Messages>();
            PatientConfiguration = new HashSet<PatientConfiguration>();
            PatientRx = new HashSet<PatientRx>();
        }

        public string UserId { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }
        public string Npi { get; set; }
        public string Vseeid { get; set; }
        public string LoginSessionId { get; set; }

        public virtual ICollection<ActivityLog> ActivityLog { get; set; }
        public virtual ICollection<AppointmentSchedule> AppointmentSchedule { get; set; }
        public virtual ICollection<Availability> Availability { get; set; }
        public virtual ICollection<EquipmentAssignment> EquipmentAssignment { get; set; }
        public virtual ICollection<Messages> MessagesReceiver { get; set; }
        public virtual ICollection<Messages> MessagesSender { get; set; }
        public virtual ICollection<PatientConfiguration> PatientConfiguration { get; set; }
        public virtual ICollection<PatientRx> PatientRx { get; set; }
    }
}
