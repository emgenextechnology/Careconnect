using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBP.Business.Filter
{
    public class FilterSales : SelectFilterBase
    {
        private int[] _RepGroupIds = null;
        private int[] _RepIds = null;

        private string _keyword;
        public string Keyword
        {
            get
            {
                return _keyword == null ? _keyword : _keyword.ToLower();
            }
            set
            {
                _keyword = value;
            }
        }

        public int? ProviderId { get; set; }

        public int? PracticeId { get; set; }

        public int? ServiceId { get; set; }

        public int? ReportId { get; set; }

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

        public int? RepGroupId { get; set; }

        public int? RepId { get; set; }

        //public DateTime? WrittenDateFrom { get; set; }

        //public DateTime? WrittenDateTo { get; set; }

        //public DateTime? ReportedDateFrom { get; set; }

        //public DateTime? ReportedDateTo { get; set; }

        public DateTime? CollectedDateFrom { get; set; }

        public DateTime? CollectedDateTo { get; set; }

        public DateTime? ReceivedDateFrom { get; set; }

        public DateTime? ReceivedDateTo { get; set; }

        public List<DynamicColumns> DynamicFilters { get; set; }

        public JObject Columns
        {
            get
            {
                JObject columns = new JObject();

                if (DynamicFilters != null)
                    foreach (var item in DynamicFilters)
                    {
                        try
                        {
                            columns.Add(item.ColumnName, "false");
                        }
                        catch { }
                    }

                return columns;
            }
        }

        public string OrderBy { get; set; }

        public int? LogId { get; set; }

        public int?[] LogStatuses { get; set; }

        public int GroupBy { get; set; }

        public int? GroupByPracticeId { get; set; }

        public int? GroupByRepId { get; set; }

        public int? GroupByRepGroupId { get; set; }

        public int? Period { get; set; }
    }

    public class DynamicColumns
    {
        public string ColumnName { get; set; }

        public string DisplayName { get; set; }

        public string ColumnValue { get; set; }

        public int Id { get; set; }

        public bool? IsVisible { get; set; }

        public bool? ShowInFilter { get; set; }

        public int? OrderIndex { get; set; }

        public int? ColumnType { get; internal set; }

        public EntityState EntityState { get; set; }
    }

}
