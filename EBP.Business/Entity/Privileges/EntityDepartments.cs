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

namespace EBP.Business.Entity.Privileges
{
    //public class EntityPrivileges
    //{
    //    public DateTime CreatedOn = DateTime.UtcNow;
    //    public DateTime? UpdatedOn = DateTime.UtcNow;
    //}
    public class EntityPrivileges
    {
        public EntityPrivileges()
        {
            UserPrivileges = new List<Privileges>();
        }

        public int UserId { get; set; }



        public List<DepartmentPrivileges> UserDepartments { get; set; }

        public List<DepartmentPrivileges> UserRoles { get; set; }

        public List<Privileges> UserPrivileges { get; set; }

        public EBP.Business.Entity.UserProfile.EntityUser User { get; set; }

        public List<Modulesmodel> Modules { get; set; }
    }
    public class DepartmentPrivileges
    {
        public string DepartmentName { get; set; }
        public IEnumerable<SelectListItem> Privileges { get; set; }
    }

    public class Privileges
    {
        public int? businessId;

        public int Id { get; set; }

        public string Name { get; set; }

        public bool? Deny { get; set; }

        public int UserId { get; set; }
        public bool? Allow { get; internal set; }
    }
    public class Modulesmodel
    {
        public string ModuleName { get; set; }
        public IEnumerable<SelectListItem> Privileges { get; set; }

        public List<Privileges> UserPrivileges { get; set; }
    }
    public class PrivilegeDetails
    {
        public string Title { get; set; }

        public string Description { get; set; }
    }
    public class VMPrivilege
    {
        public List<PrivilegeDetails> UserPrivilegesList { get; internal set; }

        public IEnumerable<string> UserPrivileges { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }
    }
}
