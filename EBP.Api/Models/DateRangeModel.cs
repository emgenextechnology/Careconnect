using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EBP.Api.Models
{
    public class DateRangeModel
    {

        // GET: DateRangeModel
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}