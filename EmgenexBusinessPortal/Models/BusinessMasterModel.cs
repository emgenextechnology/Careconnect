using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmgenexBusinessPortal.Models
{
    public class BusinessMasterModel
    {
        public int? Id { get; set; }

        public string BusinessName { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public bool Status { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string MiddleName { get; set; }

        public int CreatedBy { get; set; }
        
        public int? UpdatedBy { get; set; }

        public string RootPath { get; set; }
    }
}