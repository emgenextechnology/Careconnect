using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EBP.Business.Entity.Practice
{
    public class EntityPracticeContact
    {

        public int AddressId { get; set; }

        public string BillingContact { get; set; }

        public string BillingContactPhone { get; set; }

        public string ManagerName { get; set; }

        public string ManagerEmail { get; set; }

        public string ManagerPhone { get; set; }

        public string BillingContactEmail { get; set; }

        public string officedayshrs { get; set; }
    }
}
