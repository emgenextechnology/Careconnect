using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBP.Business.Entity.States
{
   public class EntityStates
    {
        public int Id { get; set; }
        public int CountryId { get; set; }
        public string CountryCode { get; set; }
        public string StateCode { get; set; }
        public string StateName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public int? OldId { get; set; }
        public string CreatedUser { get; set; }
        public string Updateduser { get; set; }
    }
}
