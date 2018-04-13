using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmgenexBusinessPortal.Models
{
    public class DepartmentPrivilegesModel
    {
        public DepartmentPrivilegesModel()
        {
            UserPrivileges = new List<PrivilegesModel>();
        }

        public int UserId { get; set; }

        public IQueryable<DepartmentPrivileges> UserDepartments { get; set; }

        public IQueryable<DepartmentPrivileges> UserRoles { get; set; }

        public List<PrivilegesModel> UserPrivileges { get; set; }

        public EBP.Business.Entity.UserProfile.EntityUser User { get; set; }

        public List<Modulesmodel> Modules { get; set; }
    }
    public class DepartmentPrivileges
    {
        public string DepartmentName { get; set; }

        //public IEnumerable<Privileges> Privileges { get; set; }
        public IEnumerable<SelectListItem> Privileges { get; set; }
    }

    public class PrivilegesModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool? Deny { get; set; }

        public int UserId { get; set; }
    }
    public class Modulesmodel
    {
        public string ModuleName { get; set; }

        public IEnumerable<SelectListItem> Privileges { get; set; }

        public List<PrivilegesModel> UserPrivileges { get; set; }
    }

    public class Modulesmodels
    {
        public string ModuleName { get; set; }

        public IEnumerable<SelectListItem> Privileges { get; set; }

        public List<PrivilegesModel> UserPrivileges { get; set; }
    }
}