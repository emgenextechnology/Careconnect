using EBP.Business.Entity.Practice;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EBP.Business.Entity.RepGroups
{
    public class EntityRepGroups
    {
        public DateTime CreatedOn = DateTime.UtcNow;
        public DateTime? UpdatedOn = DateTime.UtcNow;
        public int Id { get; set; }
        public string RepGroupName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public int? BusinessId { get; set; }
        public int? OldId { get; set; }
        public int? SalesDirectorId { get; set; }
        //public List<int> RepGroupDirectorIds { get; set; }
        public List<int> RepGroupManagerIds { get; set; }

        public string CreatedByName { get; set; }

        public string UpdatedByName { get; set; }

        public string SalesDirector
        {
            get
            {
                if (SalesDirectors == null || SalesDirectors.Count() <= 0)
                    return null;
                return string.Join(", ", this.SalesDirectors);
            }
        }

        public IEnumerable<string> SalesDirectors { get; set; }

        public IEnumerable<string> Managers { get; set; }

        public IEnumerable<string> SalesReps { get; set; }

        public List<int> SalesDirectorIds { get; set; }
    }
}
