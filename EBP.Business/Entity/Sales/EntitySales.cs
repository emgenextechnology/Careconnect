using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EBP.Business.Filter;
using EBP.Business.Entity.ParsingLog;
using EBP.Business.Repository;
using System.ComponentModel.DataAnnotations;

namespace EBP.Business.Entity.Sales
{
    public class EntitySales : FinanceData
    {
        public EntitySales()
        {
            ShowFinanceDataList = false;
        }

        public string NPI { get; set; }

        public string PatientFirstName { get; set; }

        public string PatientLastName { get; set; }

        public string Patient { get { return (string.Format("{0} {1}", this.PatientFirstName, this.PatientLastName)).Trim(); } }

        public DateTime ProcessedOn { get; set; }

        public string ProviderFirstName { get; set; }

        public string ProviderMiddleName { get; set; }

        public string ProviderLastName { get; set; }

        string _provider;
        public string Provider
        {
            get
            {
                if (string.IsNullOrEmpty(_provider))
                    return string.Format("{0} {1} {2}", this.ProviderFirstName, this.ProviderMiddleName, this.ProviderLastName);
                return null;
            }
            set
            {
                _provider = value;
            }
        }

        public DateTime? ReportedDate { get; set; }

        public string RepGroup { get; set; }

        public string RepFirstName { get; set; }

        public string RepLastName { get; set; }

        public string RepName { get { return string.Format("{0} {1}", this.RepFirstName, this.RepLastName); } }

        private int _reportId = 0;
        public int ReportId
        {
            get
            {
                return _reportId;
            }
            set { _reportId = value; }
        }

        public string ServiceName { get; set; }

        public string Status { get; set; }

        public string SpecimenId { get; set; }

        public bool HasFinanceData { get; set; }

        public string Value { get; set; }

        public DateTime? WrittenOn { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public List<FilesUploaded> Files { get; set; }

        public IEnumerable<ReportColumnValue> ReportColumnValues { get; set; }

        [JsonIgnore]
        public List<DynamicColumns> DynamicColumns { get; set; }

        public DateTime? CollectionDate { get; set; }

        public string CollectionDateString
        {
            get
            {
                if (CollectionDate.HasValue)
                    return CollectionDate.Value.ToString("MM-dd-yyyy");
                return string.Empty;
            }
        }

        public DateTime? ReceivedDate { get; set; }

        public string Practice { get; set; }

        public bool IsRep { get; set; }

        public int? PatientId { get; set; }

        public DateTime CreatedOn { get; set; }

        public string DegreeName { get; set; }

        public string ShortCode { get; set; }

        public bool IsFinanceFile { get; set; }

        #region Created For Sales Add/EditForm

        public int? PracticeId { get; set; }

        public int? RepId { get; set; }

        public int? ProviderId { get; set; }

        public bool IsSpecimenExists { get; set; }

        public FinanceData FinanceData
        {
            set
            {
                if (value != null)
                {
                    this.ReportFinanceId = value.ReportFinanceId;
                    this.BilledDate = value.BilledDate;
                    this.PaidDate = value.PaidDate;
                    this.Charges = value.Charges;
                    this.PaidAmount = value.PaidAmount;
                    this.AdjustAmount = value.AdjustAmount;
                    this.AdjustReason = value.AdjustReason;
                }
            }
            get
            {
                return null;
            }
        }

        public EnityParsingDetails LastParsedData { get; set; }

        public bool ShowFinanceDataList { get; set; }

        IEnumerable<FinanceData> _financeDataList;
        public IEnumerable<FinanceData> FinanceDataList
        {
            get
            {
                if (_financeDataList != null && _financeDataList.Count() > 0)
                {
                    this.ReportFinanceId = _financeDataList.Last().ReportFinanceId;
                    this.BilledDate = _financeDataList.Last().BilledDate;
                    this.PaidDate = _financeDataList.Last().PaidDate;
                    this.Charges = _financeDataList.Sum(fd => fd.Charges);
                    this.PaidAmount = _financeDataList.Sum(fd => fd.PaidAmount);
                    this.AdjustAmount = _financeDataList.Sum(fd => fd.AdjustAmount);
                    this.AdjustReason = _financeDataList.Last().AdjustReason;
                }
                return _financeDataList;
            }
            set { _financeDataList = value; }
        }

        public int FinanceDataRecordCount { get; internal set; }

        #endregion
    }

    public class ReportColumnValue
    {
        public int ColumnId { get; set; }

        public string ColumnName { get; set; }

        public int? ColumnType { get; internal set; }

        public int? IntuptType { get; internal set; }

        public string DisplayName { get; set; }

        public string Value { get; set; }
    }

    public class FilesUploaded
    {
        public string Base64 { get; set; }

        public string FileName { get; set; }

        public int FileSize { get; set; }

        public string FileType { get; set; }
    }

    public class FinanceData
    {
        public int ReportFinanceId { get; set; }

        public DateTime? BilledDate { get; set; }

        public DateTime? PaidDate { get; set; }

        public double? Charges { get; set; }

        public double? PaidAmount { get; set; }

        public double? AdjustAmount { get; set; }

        public string AdjustReason { get; set; }

        public int ServiceId { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<ReportColumnValue> FinanceColumnValues { get; set; }
    }

    public class ReportFinance
    {
        [Required]
        public DateTime? BilledDate { get; set; }

        [Required]
        public DateTime? PaidDate { get; set; }

        [Required]
        public double? Charges { get; set; }

        [Required]
        public double? PaidAmount { get; set; }

        [Required]
        public string ReportKey { get; set; }
    }

    public class SalesEntity
    {
        public List<EntitySales> SalesList { get; set; }

        public int ServiceId { get; set; }
    }

    public class SalesColumnLookup
    {
        public int ColumnId { get; internal set; }

        public IEnumerable<EntitySelectItem> ColumnLookup { get; internal set; }
    }

    public class FinanceRecord : EntitySales { }
        
    public class GroupedSales
    {
        public int ReportId { get; internal set; }

        public int? PracticeId { get; set; }

        public int? RepId { get; internal set; }

        public int? RepGroupId { get; internal set; }

        public string KeyName { get; internal set; }

        public int Count { get; internal set; }

        public DateTime LastActivityOn { get; internal set; }
    }
}
