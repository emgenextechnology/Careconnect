using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EBP.Business.Entity.Rep;

namespace EmgenexBusinessPortal.Models
{
    public class SalesRepModel
    {
        public DateTime? CreatedOn = DateTime.UtcNow;

        public DateTime? UpdatedOn = DateTime.UtcNow;

        public int Id { get; set; }

        public int UserId { get; set; }

        public string RepName { get; set; }

        public string RepGroupName { get; set; }

        public int? RepGroupId { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? SignonDate { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public int? OldId { get; set; }

        public List<int> ServiceIds { get; set; }

        public string CreatedByName { get; set; }

        public string UpdatedByName { get; set; }
        public List<SelectedItem> SelectedServiceNames { get; set; }
    }
}