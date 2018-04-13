using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmgenexBusinessPortal.Areas.Business.Models
{
    public class BusinessProfileModel 
    {
        // GET: Business/BusinessProfileModel
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
        public Nullable<int> Status { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }
}