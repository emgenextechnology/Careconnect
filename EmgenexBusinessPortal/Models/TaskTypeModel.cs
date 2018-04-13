using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmgenexBusinessPortal.Models
{
    public class TaskTypeModel
    {
        public DateTime? CreatedOn = DateTime.UtcNow;
        public DateTime? UpdatedOn = DateTime.UtcNow;
        public int Id { get; set; }
        public string TaskType { get; set; }
        public int BusinessId { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }
    }
}