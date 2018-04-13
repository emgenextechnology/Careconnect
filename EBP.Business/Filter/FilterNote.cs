using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBP.Business.Filter
{
    public class FilterNote : SelectFilterBase
    {
        public int Id { get; set; }
        public int ParentTypeId { get; set; }
        public int ParentId { get; set; }

    }
}
