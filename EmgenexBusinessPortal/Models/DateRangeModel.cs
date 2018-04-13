using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmgenexBusinessPortal.Models
{
    public class DateRangeModel 
    {
        // GET: DateRangeModel
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string ViewBy { get; set; }
        public string DateType { get; set; }
        public string Total { get; set; }
    }
}