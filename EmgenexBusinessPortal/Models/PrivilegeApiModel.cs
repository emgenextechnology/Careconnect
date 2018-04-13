using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmgenexBusinessPortal.Models
{
    public class PrivilegeApiModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int? ModuleId { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public string PrivilegeKey { get; set; }
        public string Module { get; set; }
    }
}