using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmgenexBusinessPortal.Areas.Business.Models
{
    public class CurrentUserDetails
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Nullable<int> BusinessId { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public string[] UserPrivileges { get; set; }

        public string[] Roles { get; set; }

        public string[] Departments { get; set; }

        public string BusinessName { get; set; }
    }
}