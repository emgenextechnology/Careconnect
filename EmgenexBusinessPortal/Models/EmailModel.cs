using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EBP.Business.Database;

namespace EmgenexBusinessPortal.Models
{
    public class EmailModel
    {
        // GET: EmailModel
        public class BusinessEmailModel
        {
            public string BusinessName { get; set; }

            public string UserName { get; set; }

            public string Password { get; set; }

            public string CurentUserName { get; set; }

            public string FirstName { get; set; }

            public string ReturnUrl { get; set; }
        }

        public class LeadEmailModel
        {
            public int? RepId { get; set; }

            public string ManagerName { get; set; }

            public string RepName { get; set; }
        }

        public class UserPrivilegeModel
        {
            public string CreatedUserName
            {
                get
                {
                    string _return = string.Format("{0} {1} {2}", this.CreatedUserFirstName, this.CreatedUserMiddleName, this.CreatedUserLastName);
                    return string.IsNullOrEmpty(_return) ? _return : _return.Replace("  ", " ");
                }
            }

            public string CreatedUserFirstName { get; set; }

            public string CreatedUserMiddleName { get; set; }

            public string CreatedUserLastName { get; set; }
            
            public string AssignedUserName
            {
                get
                {
                    string _return = string.Format("{0} {1} {2}", this.AssignedUserFirstName, this.AssignedUserMiddleName, this.AssignedUserLastName);
                    return string.IsNullOrEmpty(_return) ? _return : _return.Replace("  ", " ");
                }
            }

            public string AssignedUserFirstName { get; set; }

            public string AssignedUserMiddleName { get; set; }

            public string AssignedUserLastName { get; set; }

            public string BusinessName { get; set; }

            public List<UserPrivilege> UserPrivileges { get; internal set; }
        }

        public class UserPrivilege
        {
            public string Title { get; set; }

            public string Description { get; set; }
        }

        public class UserEmailModel
        {

            public string CurentUserName { get; set; }

            public string FirstName { get; set; }

            public string BusinessName { get; set; }

            public string UserName { get; set; }

            public string Password { get; set; }

            public string ReturnUrl { get; set; }

            public IQueryable<string> UserDepartments { get; set; }

        }
    }
}