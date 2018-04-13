using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EBP.Api.Models
{
    public class VMTask
    {
        public int TaskId { get; set; }

        public string[] AssignedTo { get; set; }

        public string Subject { get; set; }

        public string TaskDescription { get; set; }

        public int TaskRequestTypeId { get; set; }

        public int PriorityTypeId { get; set; }

        public DateTime? TargetDate { get; set; }

        public DateTime? ClosingDate { get; set; }

        public string ReferenceNumber { get; set; }

        public bool IsActive { get; set; }

        public int? PracticeId { get; set; }
    }
}