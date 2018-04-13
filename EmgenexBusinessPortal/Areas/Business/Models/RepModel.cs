using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmgenexBusinessPortal.Areas.Business.Models
{
    public class RepModel
    {
        // GET: Business/RepModel

        public int Id { get; set; }
        public int UserId { get; set; }
        public int? RepGroupId { get; set; }
        public bool IsActive { get; set; }
        //public Nullable<System.DateTime> SignonDate { get; set; }
        public string SignonDate { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<int> OldId { get; set; }
        public Nullable<int> MangerId { get; set; }
    }
}