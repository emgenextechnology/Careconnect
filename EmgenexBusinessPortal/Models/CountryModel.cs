using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmgenexBusinessPortal.Models
{
    public class CountryModel
    {
        public int Id { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public int? OldId { get; set; }
    }
}