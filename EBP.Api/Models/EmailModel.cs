using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EBP.Api.Models
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
        }
        public class LeadEmailModel
        {
            public int? RepId { get; set; }

            public string ManagerName { get; set; }

            public string RepName { get; set; }
        }
    }
}