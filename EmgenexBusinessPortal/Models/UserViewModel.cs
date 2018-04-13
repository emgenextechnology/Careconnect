using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EBP.Business.Entity.Users;

namespace EmgenexBusinessPortal.Models
{
    public class UserViewModel
    {


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

        public int RepGroupId { get; set; }

        public string StartDate { get; set; }

        public string UserName { get; set; }
        //public List<int> DepartmentIds { get; set; }
        //public List<string> RoleIds { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }

        public IEnumerable<SelectedItem> UserDepartments {internal get; set; }

        public IEnumerable<SelectedItem> UserRoles {internal get; set; }

        List<int> _roleIds;
        public List<int> RoleIds
        {
            get
            {
                if (_roleIds != null)
                    return _roleIds;

                List<int> selectedIds = new List<int>();
                if (UserRoles != null && UserRoles.Count() > 0)
                {
                    foreach (var item in UserRoles)
                    {
                        if (item.Selected == true)
                        {
                            selectedIds.Add(Convert.ToInt32(item.Value));
                        }
                    }

                    return selectedIds;
                }
                return selectedIds;
            }
            set
            {
                _roleIds = value;
            }
        }
        List<int> _departmentIds;
        public List<int> DepartmentIds
        {
            get
            {
                if (_departmentIds != null)
                    return _departmentIds;

                List<int> selectedIds = new List<int>();
                if (UserDepartments != null && UserDepartments.Count() > 0)
                {
                    foreach (var item in UserDepartments)
                    {
                        if (item.Selected == true)
                        {
                            selectedIds.Add(Convert.ToInt32(item.Value));
                        }
                    }

                    return selectedIds;
                }
                return selectedIds;
            }
            set
            {
                _departmentIds = value;
            }
        }
    }
    public class BusinessUserProfileModel
    {
        public String FullName { get; set; }
        public string BusinessName { get; set; }
        public string FilePath { get; set; }
        public string Email { get; set; }
        public string LogoUrl { get; internal set; }
    }
}