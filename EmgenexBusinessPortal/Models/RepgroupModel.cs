using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmgenexBusinessPortal.Models
{
    public class RepgroupModel
    {
        public DateTime? CreatedOn = DateTime.UtcNow;
        public DateTime? UpdatedOn = DateTime.UtcNow;
        public int Id { get; set; }
        public string RepGroupName { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public int? BusinessId { get; set; }
        public int? OldId { get; set; }
        public int? SalesDirectorId { get; set; }

        public List<int> RepGroupDirectorIds { get; set; }

        public List<int> RepGroupManagerIds { get; set; }

        public List<int> SalesDirectorIds { get; set; }

        public string CreatedByName { get; set; }

        public string UpdatedByName { get; set; }

        public string SalesDirector { get; set; }
        
    }
}