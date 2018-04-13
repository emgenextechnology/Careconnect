using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBP.Business.Filter
{
    public class FilterLead : SelectFilterBase
    {
        public FilterLead()
        {
            Periods = new int[] { };
            RepGroupIds = new int[] { };
            RepIds = new int[] { };
            LeadStatuses = new int[] { };
        }

        private int[] _Periods = null;
        private int[] _RepGroupIds = null;
        private int[] _RepIds = null;
        private int[] _LeadStatuses = null;

        public int[] Periods
        {
            get
            {
                if ((_Periods == null || _Periods.Count() == 0) && Period.HasValue)
                    _Periods = new int[] { Period.Value };

                return _Periods;
            }
            set { _Periods = value; }
        }

        public int[] RepGroupIds
        {
            get
            {
                if ((_RepGroupIds == null || _RepGroupIds.Count() == 0) && RepGroupId.HasValue)
                    _RepGroupIds = new int[] { RepGroupId.Value };

                return _RepGroupIds;
            }
            set { _RepGroupIds = value; }
        }

        public int[] RepIds
        {
            get
            {
                if ((_RepIds == null || _RepIds.Count() == 0) && RepId.HasValue)
                    _RepIds = new int[] { RepId.Value };

                return _RepIds;
            }
            set { _RepIds = value; }
        }

        public int[] LeadStatuses
        {
            get
            {
                if ((_LeadStatuses == null || _LeadStatuses.Count() == 0) && LeadStatus.HasValue)
                    _LeadStatuses = new int[] { LeadStatus.Value };

                return _LeadStatuses;
            }
            set { _LeadStatuses = value; }
        }

        public int? Period { get; set; }

        public int? RepGroupId { get; set; }

        public int? RepId { get; set; }

        public int? LeadStatus { get; set; }

        public bool? IsActive { get; set; }

        public bool? HasFlag { get; set; }

        public bool? HasTask { get; set; }

        public string KeyWords { get; set; }

        public string OrderBy { get; set; }
    }
}
