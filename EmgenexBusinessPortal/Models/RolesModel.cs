using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmgenexBusinessPortal.Models
{
    public class RolesModel
    {
        public DateTime? CreatedOn = DateTime.UtcNow;
        public int Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public int? BusinessId { get; set; }
        public bool? IsActive { get; set; }
        public int? Status { get; set; }
        public int? CreatedBy { get; set; }
        public int? OldId { get; set; }
        public List<int> RolePrivilegeIds { get; set; }
        public string CreatedUser { get; set; }
        public string Updateduser { get; set; }
        public List<EBP.Business.Entity.Departments.EntityModules> RolePrivilege { get; set; }
    }
}