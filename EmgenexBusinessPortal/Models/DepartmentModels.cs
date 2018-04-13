using EBP.Business.Entity.Departments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmgenexBusinessPortal.Models
{
    public class DepartmentModels
    {
        public DateTime CreatedOn = DateTime.UtcNow;
        public DateTime? UpdatedOn = DateTime.UtcNow;
        public int Id { get; set; }
        public string DepartmentName { get; set; }
        public string Description { get; set; }
        public int? StatusId { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public int? BusinessId { get; set; }
        public bool? IsActive { get; set; }
        public int? OldId { get; set; }
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }
        public List<int> DepartmentPrivilegeIds { get; set; }
        public List<string> Users { get; set; }
        public List<EntityModules> DepartmentPrivilege { get; set; }
    }
}