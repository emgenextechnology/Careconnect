using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace EmgenexBusinessPortal.Models
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

        public string DomainUrl { get; set; }

        public string RelativeUrl { get; set; }

        public bool IsRep { get; set; }

        public bool IsSalesManager { get; set; }

        public bool IsSalesDirector { get; set; }

        public string FilePath
        {
            get
            {
                return
                    string.Format("{0}/{1}/{2}/{3}/{4}.jpg", ConfigurationManager.AppSettings["BaseUrl"] ?? "", "Assets", BusinessId.HasValue ? BusinessId.ToString() : "0", "Users", Id.ToString());

            }
        }

        public object FullName { get { return string.Format("{0} {1}", FirstName, LastName); } }

        public int? DefaultDateRange { get; set; }

        public ICollection<EBP.Business.Database.Role> IRoles { get; set; }

        public ICollection<EBP.Business.Database.UserDepartment> IDepartments { get; set; }

        public string OtherEmails { get; set; }

        public int? SalesGroupBy { get; internal set; }

        string _logoUrl;
        public string LogoUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_logoUrl))
                    return string.Format("{0}{1}", ConfigurationManager.AppSettings["BaseUrl"], "/Controlpanel/Content/images/careconnect-logo.png");
                else
                    return string.Format("{0}/{1}/{2}/{3}", ConfigurationManager.AppSettings["BaseUrl"], "Assets", this.BusinessId, _logoUrl);
            }
            set
            {
                _logoUrl = value;
            }
        }
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