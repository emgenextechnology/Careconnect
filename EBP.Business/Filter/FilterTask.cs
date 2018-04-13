using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBP.Business.Filter
{
    public class FilterTask : SelectFilterBase
    {

        public int? RequestType { get; set; }

        public int? Status { get; set; }

        public int? DueOn { get; set; }

        public int? RequestedBy { get; set; }

        public int? AssignedTo { get; set; }

        public int? Priority { get; set; }

        private string _keyword;
        public string KeyWords
        {
            get
            {
                return _keyword == null ? _keyword : _keyword.ToLower().ToLower();
            }
            set
            {
                _keyword = value;
            }
        }

        public string ReferenceNumber { get; set; }

        public bool? IsActive { get; set; }
        public int? AssignedOrRequest { get; set; }

        public int? PracticeID { get; set; }

        public string  BusinessName{ get; set; }
    }
}
