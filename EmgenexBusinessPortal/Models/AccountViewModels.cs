using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EmgenexBusinessPortal.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public string Provider { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string Action { get; set; }

        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }

        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }

        public string ReturnUrl { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
    public class BusinessLoginViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string RelativeUrl { get; set; }

        public string ReturnRedirictUrl { get; set; }
    }
    public class RegisterViewModel
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

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
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

        public string RoleName { get; set; }

        public string DepartmentName { get; set; }
    }


    public class UserProfileModel
    {
        public int? userId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Required(ErrorMessage = "The Phone Number field is required.")]
        [Phone]
        [MinLength(10)]
        [MaxLength(12)]
        [Display(Name = "Phone")]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public string UserCountry { get; set; }

        public string UserState { get; set; }

        public string UserCity { get; set; }

        public string UserAddress { get; set; }

        public string UserArea { get; set; }

        [Required(ErrorMessage = "The Location field is required.")]
        public string Location { get; set; }

        public string Zip { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }
    }

    public class ResetPasswordViewModel
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ChangePasswordViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }

        public bool IsBusinessLogin { get; set; }

        public string BusinessName { get; set; }

        public string BusinessLogo { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Display(Name = "Email")]
        public string Email { get; set; }

        public bool IsBusinessLogin { get; set; }

        public string BusinessName { get; set; }

        public string BusinessLogo { get; set; }

        public int bizId { get; set; }
    }
    public class XMLRegisterModel
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

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
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

        public string RoleName { get; set; }

        public string DepartmentName { get; set; }

        public string SalesTeam { get; set; }

        public string State { get; set; }

        public string Services { get; set; }
        public string ErrorMessage { get; set; }
    }

}