using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EBP.Api.Models
{
    public class VMLead
    {
        public VMLead()
        {
        }

        public int? Id { get; set; }

        [Required(ErrorMessage = "Practice name is required.")]
        public string PracticeName { get; set; }

        public int? SpecialityTypeId { get; set; }

        public int? SpecialityId { get; set; }

        public string[] SpecialityIds { get; set; }

        public string[] SpecialityIdsStr { get { return SpecialityIds != null ? SpecialityIds.Select(a => a.ToString()).ToArray() : null; } }

        public string NewSpectialityName { get; set; }

        public int? AddressId { get; set; }

        [Required(ErrorMessage = "Address line is required.")]
        public string PracticeAddressLine1 { get; set; }

        public string PracticeAddressLine2 { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required.")]
        public int? StateId { get; set; }

        [Required(ErrorMessage = "Zip code is required.")]
        [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Please enter 5 digit zip code.")]
        public string Zip { get; set; }

        public int? PhoneId { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Please enter valid phone number.")]
        public string PhoneNumber { get; set; }

        public string PhoneExtension { get; set; }

        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Please enter valid fax number.")]
        public string Fax { get; set; }

        [Required(ErrorMessage = "Sales Group is required.")]
        public int? RepGroupId { get; set; }

       [Required(ErrorMessage = "Sales Rep is required.")]
        public int? RepId { get; set; }

        public string ServiceInterest { get; set; }

        public int? LeadSourceId { get; set; }

        public string OtherLeadSource { get; set; }

        public int PracticeId { get; set; }

        public string ManagerName { get; set; }

        public List<VMProvider> Providers { get; set; }

        public List<VMLocation> Locations { get; set; }

        public string RepName { get; set; }

        public string ReturnUrl { get; set; }

        public string CreatedByName { get; set; }
    }

    public class VMLocation : VMContact
    {
        public int? AddressId { get; set; }

        public int? AddressIndex { get; set; }

        public int? PhoneId { get; set; }
        [Required(ErrorMessage = "Address line is required.")]

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required.")]
        public int? StateId { get; set; }

        [Required(ErrorMessage = "Zip code is required.")]
        [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Please enter 5 digit zip code.")]
        public string Zip { get; set; }

        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Please enter valid phone number.")]
        public string PhoneNumber { get; set; }

        public string Extension { get; set; }

        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Please enter valid phone number.")]
        public string Fax { get; set; }

        public int AddressTypeId { get; set; }

    }

    public class VMProvider
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Degree is required.")]
        public int? DegreeId { get; set; }

        [Required(ErrorMessage = "NPI is required.")]
        public string NPI { get; set; }

        private int? _IsPracticeLoc = null;
        public int IsPracticeLoc { get { return (Location != null && Location.AddressTypeId == 1) ? 1 : 0; } }// set { _IsPracticeLoc = value; } }

        public VMLocation Location { get; set; }

        public string DegreeName { get; set; }
    }
}