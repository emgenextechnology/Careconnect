using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EBP.Api.Models
{
    public class VMAccount : VMContact
    {
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

        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Please enter valid phone number.")]
        [Required(ErrorMessage = "Phone Number is required.")]
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

        public string EnrolledDateLocal
        {
            get
            {
                if (EnrolledDate.HasValue)
                    return EnrolledDate.Value.ToString("MM-dd-yyyy");

                return "";
            }
        }
        public DateTime? EnrolledDate { get; set; }

        public bool? IsActive { get; set; }

        public string PracticeOther { get; set; }

        public string Line1 { get; set; }

        public string Extension { get; set; }

        public string Line2 { get; set; }

        public int AddressTypeId { get; set; }

        public int LeadId { get; set; }

        [Required(ErrorMessage = "Enrolled Services is required.")]
        public string[] EnrolledServices { get; set; }

        public string ReportDeliveryPreference { get; set; }

        public int? MethodOfContact { get; set; }

        public int PracticeId { get; set; }

        [Required(ErrorMessage = "Report delivery email is required.")]
        public string ReportDeliveryEmail { get; set; }

        [Required(ErrorMessage = "Report delivery fax is required.")]
        public string ReportDeliveryFax { get; set; }

        public string CreatedByName { get; set; }

        public string ReturnUrl { get; set; }

        public List<VMProvider> Providers { get; set; }

        public List<VMLocation> Locations { get; set; }

    }


    public class VMContact
    {
        public string BillingContact { get; set; }

        public string BillingContactEmail { get; set; }

        public string ManagerPhone { get; set; }

        public string ManagerEmail { get; set; }

        public string WorkingHours { get; set; }

        public string ManagerName { get; set; }

        public string BillingContactPhone { get; set; }
    }

    public class StatusManagementModel
    {
        public int Id { get; set; }
        public bool IsLead { get; set; }
        public bool IsActive { get; set; }
        public bool Flag { get; set; }
    }
}