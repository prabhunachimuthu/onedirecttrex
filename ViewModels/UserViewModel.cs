using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.ViewModels
{
    public class UserViewModel
    {
        [Required(ErrorMessage = "User Id is required")]
        public string UserId { get; set; }
        public int Type { get; set; }
        [Required(ErrorMessage = "User Name is required")]
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        [Required(ErrorMessage = "Provider Name is required")]
        public string ProviderId { get; set; }
        public string Therapist { get; set; }
        public string Address { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        public string Npi { get; set; }
        public string Vseeid { get; set; }
    }
}
