using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmgenexBusinessPortal.Areas.BusinessSettings.Models
{
    public class BusinessUserModels
    {
        // GET: BusinessSettings/BusinessUserModels

        public int Id { get; set; }

        public bool LockoutEnabled { get; set; }

        [Required(ErrorMessage = "The Name field is required.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "The LastName field is required.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "The Phone Number field is required.")]
        [Phone]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        [Display(Name = "Phone")]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [EmailAddress]
        [Display(Name = "Work Email")]
        public string WorkEmail { get; set; }

        [EmailAddress]
        public string ALternateEmail { get; set; }

        [Display(Name = "Home Phone")]
        public string HomePhone { get; set; }

        [Display(Name = "Additional Phone")]
        public string AdditionalPhone { get; set; }

        public string Address { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public string Zip { get; set; }

        public int? StateId { get; set; }

        public string StartDate { get; set; }

        public string UserName { get; set; }
        public List<int> DepartmentIds { get; set; }
        public List<string> RoleIds { get; set; }
    }
}