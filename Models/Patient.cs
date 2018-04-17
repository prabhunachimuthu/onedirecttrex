﻿using System;
using System.Collections.Generic;

namespace OneDirect.Models
{
    public partial class Patient
    {
        public Patient()
        {
            ActivityLog = new HashSet<ActivityLog>();
            AppointmentSchedule = new HashSet<AppointmentSchedule>();
            EquipmentAssignment = new HashSet<EquipmentAssignment>();
            PatientConfiguration = new HashSet<PatientConfiguration>();
            PatientRx = new HashSet<PatientRx>();
            Protocol = new HashSet<Protocol>();
            Session = new HashSet<Session>();
        }

        public int PatientId { get; set; }
        public string ProviderId { get; set; }
        public string PatientName { get; set; }
        public DateTime? Dob { get; set; }
        public string AddressLine { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Ssn { get; set; }
        public string EquipmentType { get; set; }
        public DateTime? SurgeryDate { get; set; }
        public string Side { get; set; }
        public int? Pin { get; set; }
        public Guid? LoginSessionId { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string Therapistid { get; set; }
        public string PatientLoginId { get; set; }

        public virtual ICollection<ActivityLog> ActivityLog { get; set; }
        public virtual ICollection<AppointmentSchedule> AppointmentSchedule { get; set; }
        public virtual ICollection<EquipmentAssignment> EquipmentAssignment { get; set; }
        public virtual ICollection<PatientConfiguration> PatientConfiguration { get; set; }
        public virtual ICollection<PatientRx> PatientRx { get; set; }
        public virtual ICollection<Protocol> Protocol { get; set; }
        public virtual ICollection<Session> Session { get; set; }
    }
}
