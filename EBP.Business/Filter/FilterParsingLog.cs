using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBP.Business.Filter
{
    public class FilterParsingLog : SelectFilterBase
    {
        public DateTime? ImportedDateFrom { get; set; }

        public DateTime? ImportedDateTo { get; set; }

        public int ServiceId { get; set; }
    }
}
