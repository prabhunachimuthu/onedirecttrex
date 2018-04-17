using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.ViewModels
{
    public class NewPatient
    {

        public int PatientId { get; set; }
        public string ProviderId { get; set; }
        public string TherapistId { get; set; }
        [Required(ErrorMessage = "Patient Name is required")]
        public string PatientName { get; set; }
        [Required(ErrorMessage = "Date of Birth is required")]
        public DateTime? Dob { get; set; }
        [Required(ErrorMessage = "Mobile Number is required")]
        //[RegularExpression(@"(?:\+\s*\d{2}[\s-]*)?(?:\d[-\s]*){10}", ErrorMessage = "Invalid Mobile Number.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "The phone number must be numeric.")]
        [StringLength(10, ErrorMessage = "The phone number must contain 10 digits.", MinimumLength = 10)]
        public string PhoneNumber { get; set; }

        public string AddressLine { get; set; }

        public string City { get; set; }
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "State must be alphabets.")]
        [StringLength(2, ErrorMessage = "State only 2 digits", MinimumLength = 2)]
        public string State { get; set; }
        [RegularExpression("^[0-9]*$", ErrorMessage = "Zip must be numeric.")]
        [StringLength(5, ErrorMessage = "Zip must contain 5 digits.", MinimumLength = 5)]
        public string Zip { get; set; }
        [Required(ErrorMessage = "Equipment Type is required")]
        public string EquipmentType { get; set; }
        [Required(ErrorMessage = "Surgery Date is required")]
        public DateTime? SurgeryDate { get; set; }
        [Required(ErrorMessage = "Side is required")]
        public string Side { get; set; }
        [Required(ErrorMessage = "SSN is required")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Ssn must be numeric.")]
        [StringLength(4, ErrorMessage = "Ssn must contain 4 digits.", MinimumLength = 4)]
        public string Ssn { get; set; }
        public int? Pin { get; set; }
        public Guid? LoginSessionId { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string Action { get; set; }

        public string PatientLoginId { get; set; }
    }
}
