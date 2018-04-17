using OneDirect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.ViewModels
{
    public class NewProtocol
    {
        public string ProtocolId { get; set; }
        public string RxId { get; set; }
        public int PatientId { get; set; }
        public string ProtocolName { get; set; }
        public int RestPosition { get; set; }
        public int? MaxUpLimit { get; set; }
        public int? StretchUpLimit { get; set; }
        public int? MaxDownLimit { get; set; }
        public int? StretchDownLimit { get; set; }
        public int? FlexUpLimit { get; set; }
        public int? FlexUpHoldtime { get; set; }
        public int? StretchUpHoldtime { get; set; }
        public int? FlexDownLimit { get; set; }
        public int? FlexDownHoldtime { get; set; }
        public int? StretchDownHoldtime { get; set; }
        public int? UpReps { get; set; }
        public int? DownReps { get; set; }
        public int? Reps { get; set; }
        public int? ProtocolEnum { get; set; }
        public string EquipmentType { get; set; }
        public string ExcerciseName { get; set; }
        public string ExcerciseEnum { get; set; }
        public int? Time { get; set; }
        public int? RestTime { get; set; }
        public PatientRx Rx { get; set; }
        public string PatientName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? SurgeryDate { get; set; }
        public int CurrentFlex { get; set; }
        public int GoalFlex { get; set; }
        public int CurrentExten { get; set; }
        public int GoalExten { get; set; }
        public DateTime? RxEndDate { get; set; }
        public int SessionCount { get; set; }
        public int Speed { get; set; }
        public int RepsAt { get; set; }
        public int RestAt { get; set; }
        //Prabhu
        public int RateOfChange { get; set; }
    }
}
