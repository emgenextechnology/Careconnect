using EBP.Business.Entity.Departments;
using EBP.Business.Entity.Practice;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EBP.Business.Entity.Roles
{
    public class EntityRoles
    {
        public DateTime? CreatedOn = DateTime.UtcNow;
        public int Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public int? BusinessId { get; set; }
        public bool? IsActive { get; set; }
        public int? Status { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedUser { get; set; }
        public int? OldId { get; set; }
        List<int> _rolePrivilegeIds;
        public List<int> RolePrivilegeIds
        {
            get
            {
                if (_rolePrivilegeIds != null)
                    return _rolePrivilegeIds;

                List<int> selectedIds = new List<int>();
                if (RolePrivilege != null && RolePrivilege.Count() > 0)
                {
                    foreach (var item in RolePrivilege)
                    {
                        var _selectedIds = item.Privileges.Where(a => a.Selected == true).Select(a => a.Value).ToList();
                        if (_selectedIds != null && _selectedIds.Count() > 0)
                            _selectedIds.ForEach(a => selectedIds.Add(Convert.ToInt32(a)));
                    }

                    return selectedIds;
                }
                return selectedIds;
            }
            set {
                _rolePrivilegeIds = value;
            }
        }

        public List<EntityModules> RolePrivilege { get; set; }
    }
}
