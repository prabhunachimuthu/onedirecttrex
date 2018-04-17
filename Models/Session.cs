using System;
using System.Collections.Generic;

namespace OneDirect.Models
{
    public partial class Session
    {
        public Session()
        {
            Pain = new HashSet<Pain>();
        }

        public int PatientId { get; set; }
        public string RxId { get; set; }
        public string ProtocolId { get; set; }
        public string SessionId { get; set; }
        public int? Reps { get; set; }
        public int? Duration { get; set; }
        public int? FlexionReps { get; set; }
        public int? ExtensionReps { get; set; }
        public DateTime? SessionDate { get; set; }
        public int? PainCount { get; set; }
        public int MaxFlexion { get; set; }
        public int MaxExtension { get; set; }
        public int MaxPain { get; set; }
        public string TimeZoneOffset { get; set; }
        public int? Boom1Position { get; set; }
        public int? Boom2Position { get; set; }
        public int? RangeDuration1 { get; set; }
        public int? RangeDuration2 { get; set; }
        public int? GuidedMode { get; set; }

        public virtual ICollection<Pain> Pain { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual Protocol Protocol { get; set; }
        public virtual PatientRx Rx { get; set; }
    }
}
