using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EBP.Api.Models
{
    public class UserDetailsModel
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public Nullable<int> BusinessId { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string UserName { get; set; }

        public string[] UserPrivileges { get; set; }

        public string[] Roles
        {
            get
            {
                return this.IRoles != null ? this.IRoles.Select(a => a.Name).ToArray() : null;
            }
        }

        public string[] Departments
        {
            get
            {
                return this.IDepartments != null ? this.IDepartments.Select(a => a.Department.DepartmentName).ToArray() : null;
            }
        }
        
        public string BusinessName { get; set; }

        public bool IsRep { get; set; }

        public bool IsSalesManager { get; set; }

        public string OtherEmails { get; set; }

        public string DomainUrl { get; internal set; }

        public string RelativeUrl { get; internal set; }

        public ICollection<Business.Database.Role> IRoles { get; set; }

        public ICollection<Business.Database.UserDepartment> IDepartments { get; set; }
    }

    public class DeviceTokenModel
    {
        public string DeviceId { get; set; }
    }

    public static class UserExt
    {
        public static bool IsBuzAdmin(this UserDetailsModel user)
        {
            return user.Roles.Contains("BusinessAdmin");
        }

        public static bool IsSalesManager(this UserDetailsModel user)
        {
            return user.IsSalesManager && !user.IsBuzAdmin();
        }

        public static bool IsSalesDirector(this UserDetailsModel user)
        {
            var privileges = new string[] { "MNGALLSLSTMS" };
            return user.UserPrivileges.Any(a => privileges != null && privileges.Any(b => b == a))
                && !user.Roles.Contains("BusinessAdmin");
        }

        public static bool IsSalesRep(this UserDetailsModel user)
        {
            return user.IsRep;
        }

        public static bool HasAllLeadReadPrivilage(this UserDetailsModel user)
        {
            return user.UserPrivileges.Contains("RDLDALL");
        }

        public static bool HasAllAccountReadPrivilage(this UserDetailsModel user)
        {
            return user.UserPrivileges.Contains("RDACNTALL");
        }
    }
}