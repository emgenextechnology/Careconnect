using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace EmgenexBusinessPortal.Models
{
    public class RoleViewModel
    {
        public int Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "RoleName")]
        public string Name { get; set; }
    }

    public class EditUserViewModel
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "The Phone Number field is required.")]
        [Phone]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "The First Name field is required.")]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "The Last Name field is required.")]
        public string LastName { get; set; }

        [EmailAddress]
        [Display(Name = "Work Email")]
        public string WorkEmail { get; set; }

        [EmailAddress]
        [Display(Name = "ALternate Email")]
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

        public int CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public IEnumerable<SelectListItem> RolesList { get; set; }

        public IEnumerable<SelectListItem> Privileges { get; set; }

        public List<EBP.Business.Entity.EntitySelectItem> SelectedPrivileges { get; set; }

        public IEnumerable<SelectListItem> Departments { get; set; }
    }
}