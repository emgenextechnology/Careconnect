using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmgenexBusinessPortal.Models
{
    public class DegreeModel
    {
        public int Id { get; set; }
        public string DegreeName { get; set; }
        public string ShortCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime OldId { get; set; }
        public string CreatedUser { get; set; }
        public string Updateduser { get; set; }
    }
}