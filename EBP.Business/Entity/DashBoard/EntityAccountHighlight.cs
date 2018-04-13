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

namespace EBP.Business.Entity.DashBoard
{
    public class EntityAccountHighlight
    {
        public string[] LastWeekCounts { get; set; }

        public string[] LastWeekdays { get; set; }

        public string[] ThisWeekdays { get; set; }

        public string[] ThisWeekCounts { get; set; }

        public int ThisWeekCount { get; set; }

        public int TotalCount { get; set; }

        public int MonthToDate { get; set; }
    }
}
