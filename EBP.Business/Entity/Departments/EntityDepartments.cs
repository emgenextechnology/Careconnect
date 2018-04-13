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
using System.Web.Mvc;

namespace EBP.Business.Entity.Departments
{
    public class EntityDepartments
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
        //public List<int> DepartmentPrivilegeIds { get; set; }
        public List<string> Users { get; set; }
        public string CreatedUser { get; set; }
        public string Updateduser { get; set; }
        List<int> _departmentPrivilegeIds;
        public List<int> DepartmentPrivilegeIds
        {
            get
            {
                if (_departmentPrivilegeIds != null)

                    return _departmentPrivilegeIds;

                List<int> selectedIds = new List<int>();
                if (DepartmentPrivilege != null && DepartmentPrivilege.Count() > 0)
                {
                    foreach (var item in DepartmentPrivilege)
                    {
                        var _selectedIds = item.Privileges.Where(a => a.Selected == true).Select(a => a.Value).ToList();
                        if (_selectedIds != null && _selectedIds.Count() > 0)
                            _selectedIds.ForEach(a => selectedIds.Add(Convert.ToInt32(a)));
                    }

                    return selectedIds;
                }
                return selectedIds;
            }
            set
            {
                _departmentPrivilegeIds = value;
            }
        }
        public List<EntityModules> DepartmentPrivilege { get; set; }
    }
    public class EntityModules
    {
        public string ModuleName { get; set; }
        public IEnumerable<SelectListItem> Privileges { get; set; }
    }
}
