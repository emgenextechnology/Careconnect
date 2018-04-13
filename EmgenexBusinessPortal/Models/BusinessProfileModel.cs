using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmgenexBusinessPortal.Models
{
    public class BusinessProfileModel
    {
        public int Id { get; set; }
        public string BusinessName { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Logo { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string About { get; set; }
        public string RelativeUrl { get; set; }
        public string DomainUrl { get; set; }
        public bool IsActive { get; set; }
        public int? Status { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public string CreatedUser { get; set; }
        public string Updateduser { get; set; }
        public int? DateRange { get; set; }
        public int? SalesGroup { get; set; }
        public string OtherEmails { get; set; }
    }
}