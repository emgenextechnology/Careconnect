using EBP.Business.Entity.Practice;
using EBP.Business.Entity.Rep;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EBP.Business.Entity.DashBoard
{
    public class EntitySalesPerformance
    {

        public string[] ServiceNames { get; set; }

        public string[] Counts { get; set; }

        public string[] ServiceName { get; set; }

        public int ThisWeekCount { get; set; }

        public int TotalCount { get; set; }

        public int MonthToDate { get; set; }

        public string[] ServiceColor { get; set; }
    }
    public class EntitySalesPeriodicTrends
    {
        public List<EntitySalesPeriodicModel> PeriodicTrends { get; set; }

        public string month { get; set; }
    }
    public class EntitySalesPeriodicModel
    {
        public DateTime month { get; set; }
        public IEnumerable<ServiceModel> Services { get; set; }
    }
    public class ServiceModel
    {
        public string ServiceName { get; set; }
        public int Count { get; set; }

        public int ServiceId { get; set; }

        public int month { get; set; }
    }
    public class VMServiceModel
    {
        public string ServiceName { get; set; }
        public int ServiceCount { get; set; }

        public int ServiceId { get; set; }

        //public int Month { get; set; }
        public string Month { get; set; }

        public int Year { get; set; }

        //public int? Week { get; set; }

        public DateTime? Day { get; set; }

        public double? Billed { get; set; }

        public double? Reimbursements { get; set; }

        public string Week { get; set; }

        public double? Paid { get; set; }
    }
}
