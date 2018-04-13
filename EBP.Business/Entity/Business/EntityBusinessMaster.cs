using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBP.Business.Entity.Business
{
   public class EntityBusinessMaster
    {
        public DateTime CreatedOn = DateTime.UtcNow;
        public DateTime? UpdatedOn = DateTime.UtcNow;
        public int Id { get; set; }
        public string BusinessName { get; set; }
        public string Description { get; set; }     
        public string RelativeUrl { get; set; }       
        public bool IsActive { get; set; }
        public int? Status { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public string CreatedUser { get; set; }
        public string UpdatedUser { get; set; }
     
    }
}
