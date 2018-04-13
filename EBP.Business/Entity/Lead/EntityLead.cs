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

namespace EBP.Business.Entity.Lead
{
    public class EntityLead
    {
        public DateTime CreatedOn = DateTime.UtcNow;
        public DateTime? UpdatedOn = DateTime.UtcNow;

        public int CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public int LeadId { get; set; }

        public int PracticeId { get; set; }

        public int? RepGroupId { get; set; }

        public string Status { get; set; }

        public bool IsActive { get; set; }

        public int? LeadSourceId { get; set; }

        public Nullable<int> RepId { get; set; }

        public int ContactPreferenceId { get; set; }

        public string LeadServiceIntrest { get; set; }

        public string OtherLeadSource { get; set; }

        public int? PracticeTypeId { get; set; }

        public bool? HasFlag { get; set; }

        public bool HasTask { get; set; }

        public string LeadSourceName { get; set; }

        public string CreatedByName { get; set; }

        public string UpdatedByName { get; set; }

        public bool? IsConverted { get; set; }

        public int? BusinessId { get; set; }

        public int CurrentUserId { get; set; }

        public int ManagerId { get; set; }

        public int TaskCount { get; set; }

        public EntityPractice Practice { get; set; }

        public EntityRep Rep { get; set; }
    }
}
