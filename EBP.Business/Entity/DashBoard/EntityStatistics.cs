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
    public class EntityStatistics
    {
        public int LeadsCount { get; set; }

        public int AccountsCount { get; set; }

        public int SalesCount { get; set; }

        public int TasksCount { get; set; }


        public int GrowsLeadsCount
        {
            get
            {
                if (LeadsCount == 0) return 0;

                return
                    LeadsCountLastMonth / LeadsCount * 100;
            }
        }

        public int GrowsAccountsCount
        {
            get
            {

                if (AccountsCount == 0) return 0;

                return
                    AccountsCountLastMonth / AccountsCount * 100;

            }
        }

        public int GrowsSalesCount
        {
            get
            {

                if (SalesCount == 0) return 0;

                return
                    SalesCountLastMonth / SalesCount * 100;

            }
        }

        public int GrowsTasksCount
        {
            get
            {

                if (TasksCount == 0) return 0;

                return
                    TasksCountLastMonth / TasksCount * 100;

            }
        }

        public int LeadsCountLastMonth { get; set; }

        public int AccountsCountLastMonth { get; set; }

        public int SalesCountLastMonth { get; set; }

        public int TasksCountLastMonth { get; set; }

        public int? NotificationCount { get; set; }
    }

    public class EntityLatestLead
    {
        public string PracticeName { get; set; }

        public int ProvidersCount { get; set; }

        public string RepFirstName { get; set; }

        public string RepLastName { get; set; }

        public string RepName
        {
            get
            {
                return string.Format("{0} {1}", this.RepFirstName, this.RepLastName);
            }
        }

        public DateTime CreatedOn { get; set; }

    }
    public class EntityLatestAccount
    {
        public string PracticeName { get; set; }

        public int ProvidersCount { get; set; }

        public int SalesCount { get; set; }

        public string RepFirstName { get; set; }

        public string RepLastName { get; set; }

        public string RepName
        {
            get
            {
                return string.Format("{0} {1}", this.RepFirstName, this.RepLastName);
            }
        }

        public DateTime CreatedOn { get; set; }

    }

    public class AccountSummary
    {
        public EntityLatestAccount Account { get; set; }
        public int SalesCount { get; set; }
    }
    public class EntityTopReps
    {
        public string PracticeName { get; set; }

        public string RepFirstName { get; set; }

        public string RepLastName { get; set; }

        public string RepName
        {
            get
            {
                return string.Format("{0} {1}", this.RepFirstName, this.RepLastName);
            }
        }

        public string RepGroup { get; set; }

        public int AccountsCount { get; set; }

        public DateTime CreatedOn { get; set; }
    }


    public class TopRepSummary{
        public EntityTopReps Rep { get; set; }
        public int SalesCount { get; set; }
    }

    public class WeekModel
    {
        public string CreatedOn { get; set; }
    }
    public class EntityLeadFunnelModel
    {

        public float New { get; set; }

        public float Active { get; set; }

        public float Dormant { get; set; }

        public int TotalCount { get; set; }

        public int ThisWeekCount { get; set; }

        public int MonthToDate { get; set; }
    }
    public class EntitySalesPerformanceApiModel
    {

        public int Today { get; set; }

        public int YesterDay { get; set; }

        public int ThisWeek { get; set; }

        public int LastWeek { get; set; }

        public int ThisMonth { get; set; }

        public int LastMonth { get; set; }
    }
    public class EntitySalesPeriodicDataTrends
    {

        public string Period { get; set; }

        public int Count { get; set; }

        public bool IsInProgress { get; set; }

        public double ProgressPercentage { get; set; }

      
    }

    public class EntityDefaultDaterangeModel
    {
        public int? DateRange { get; set; }
    }
}
