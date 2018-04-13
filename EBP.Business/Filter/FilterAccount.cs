using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBP.Business.Filter
{
    public class FilterAccount : SelectFilterBase
    {
        public FilterAccount()
        {
            Periods = new int[] { };
            RepGroupIds = new int[] { };
            RepIds = new int[] { };
            AccountStatuses = new int[] { };
            ProviderIds = new int[] { };
            ServiceIds = new int[] { };
           // ShowIsActive = 0;
        }

        public int[] _Periods;
        public int[] _RepGroupIds;
        public int[] _RepIds;
        public int[] _AccountStatuses;
        public int[] _ProviderIds;
        public int[] _ServiceIds;

       /// public int ShowInactive { get; set { if(value==0) } }

        //public bool? IsActive { get; set; }

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

        public int[] AccountStatuses
        {
            get
            {
                if ((_AccountStatuses == null || _AccountStatuses.Count() == 0) && AccountStatus.HasValue)
                    _AccountStatuses = new int[] { AccountStatus.Value };

                return _AccountStatuses;
            }


            set { _AccountStatuses = value; }
        }

        public int[] ProviderIds
        {
            get
            {
                if ((_ProviderIds == null || _ProviderIds.Count() == 0) && ProviderId.HasValue)
                    _ProviderIds = new int[] { ProviderId.Value };

                return _ProviderIds;
            }


            set { _ProviderIds = value; }
        }

        public int[] ServiceIds
        {
            get
            {
                if ((_ServiceIds == null || _ServiceIds.Count() == 0) && ServiceId.HasValue)
                    _ServiceIds = new int[] { ServiceId.Value };

                return _ServiceIds;
            }


            set { _ServiceIds = value; }
        }

        public string KeyWords { get; set; }

        public bool? HasFlag { get; set; }

        public bool? HasTask { get; set; }

        public int? Period { get; set; }

        public int? RepGroupId { get; set; }

        public int? RepId { get; set; }

        /// <summary>
        ///   New = 1,
        ///   Active = 2,
        ///   Dormant = 3
        /// </summary>
        public int? AccountStatus { get; set; }

        public DateTime LastActivityDate { get; set; }

        public int? ProviderId { get; set; }

        public int? ServiceId { get; set; }

        public bool WithTask { get; set; }

        public bool? HasReports { get; set; }

        public string OrderBy { get; set; }

    }
}
