using EBP.Business.Entity.Practice;
using EBP.Business.Entity.Rep;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBP.Business.Entity.Account
{
    public class EntityAccount
    {
        public DateTime CreatedOn = DateTime.UtcNow;
        public DateTime? UpdatedOn = DateTime.UtcNow;

        public double UpdatedMinutes
        {
            get
            {
                if (LastActivityDate.HasValue)
                {
                    return TimeSpan.FromTicks(LastActivityDate.Value.Ticks).TotalMinutes;
                }

                return DateTime.MinValue.Ticks / TimeSpan.TicksPerMinute;
            }
        }

        public double CreatedMilliseconds
        {
            get
            {
                return CreatedOn.Ticks / TimeSpan.TicksPerMinute;
            }
        }

        public int ActivityStatus
        {
            get
            {
                if (LastActivity == null)
                    return EnrolledDate > DateTime.Now.AddDays(-60)?1:3;
                else 
                   return LastActivity.CreatedOn > DateTime.Now.AddDays(-60)?2:3;
                  
                //if ((LastActivity == null && 
                //    CreatedOn > DateTime.Now.AddMonths(-3)) || (LastActivity != null && 
                //    LastActivity.CreatedOn > DateTime.Now.AddMonths(-3) && 
                //    LastActivity.CreatedOn < DateTime.Now.AddDays(-2)))

                //    return 1;//new

                //if (LastActivity != null && LastActivity.CreatedOn >= DateTime.Now.AddDays(-2))
                //    return 2;
                //if (LastActivity != null && LastActivity.CreatedOn < DateTime.Now.AddMonths(-3))
                //    return 3;

                return 3; //created lest than 1 mnth and no activity
            }
        }

        public int Id { get; set; }

        public int LeadId { get; set; }

        public DateTime? EnrolledDate { get; set; }

        public bool? IsActive { get; set; }

        public int CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public string Status { get; set; }

        public bool? HasFlag { get; set; }

        public int? LeadSourceId { get; set; }

        public string LeadSourceName { get; set; }

        public string LeadServiceIntrest { get; set; }

        public string OtherLeadSource { get; set; }

        public int? RepId { get; set; }

        public int? RepGroupId { get; set; }

        public string CreatedByName { get; set; }

        public string UpdatedByName { get; set; }

        public bool IsConverted { get; set; }

        public int ManagerId { get; set; }

        public int? PracticeTypeId { get; set; }

        public int ServicesCount { get; set; }

        public int ProvidersCount { get; set; }

        public int NotesCount { get; set; }

        public int? BusinessId { get; set; }

        public int CurrentUserId { get; set; }

        public bool HasTask { get; set; }

        IEnumerable<string> _serviceIds;
        public object ServiceIds
        {
            set
            {
                _serviceIds = (IEnumerable<string>)value;
            }
            get
            {
                return _serviceIds != null ? _serviceIds.ToArray() : null;
            }
        }

        IEnumerable<string> _services;
        public object ServiceNames
        {
            set
            {
                _services = (IEnumerable<string>)value;
            }
            get
            {
                return _services != null ? _services.ToArray() : null;
            }
        }

        IEnumerable<string> _providers;
        public object ProviderNames
        {
            set
            {
                _providers = (IEnumerable<string>)value;
            }
            get
            {
                return _providers != null ? _providers.ToArray() : null;
            }
        }

        public EntityPractice Practice { get; set; }

        public EntityRep Rep { get; set; }
        
        public DateTime? LastActivityDate { get; set; }

        public DateTime? LastActivityStatus { get; set; }

        [JsonIgnore]
        public Database.ReportMaster LastActivity { get; set; }

        public int ReportCount { get; set; }
    }

    public class EntityGroupManagerDetails
    {
        public int Id { get; set; }

        public int LeadId { get; set; }

        public int? RepId { get; set; }

        public string RepEmail { get; set; }

        public int ManagerId { get; set; }

        public string ManagerEmail { get; set; }

        public string RepFirstName { get; set; }

        public string RepMiddleName { get; set; }

        public string RepLastName { get; set; }

        public string ManagerFirstName { get; set; }

        public string ManagerMiddleName { get; set; }

        public string ManagerLastName { get; set; }

        public string ManagerName
        {
            get
            {
                return string.Format("{0} {1} {2}", this.ManagerFirstName, this.ManagerMiddleName, this.ManagerLastName);
            }
        }

        public IEnumerable<Manager> Managers { get; set; }
    }

    public class EntityAccountAddress
    {
        public int Id { get; set; }

        public string LocationId { get; set; }

        public string PracticeName { get; set; }

        public IEnumerable<EntityPracticeAddress> Address { get; set; }
    }
}
