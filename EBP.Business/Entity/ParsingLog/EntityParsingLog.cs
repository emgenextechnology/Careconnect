using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EBP.Business.Enums;
using EBP.Business.Repository;

namespace EBP.Business.Entity.ParsingLog
{
    public class EntityParsingLogSummary : EntityBase
    {
        public int Id { get; set; }

        public int ServiceId { get; set; }

        public System.DateTime? ImportedDate { get; set; }

        public string ImportedFilePath { get; set; }

        public string LogFilePath { get; set; }

        public string ImportMode { get; set; }

        public string MessageSummery { get; set; }

        public IEnumerable<EntityParsingLog> LoggedData { get; set; }

        public string SourceFileName
        {
            get
            {
                if (string.IsNullOrEmpty(SourceFileUrl))
                    return "";
                return SourceFileUrl.Split('/').Last();
            }
        }

        public string SourceFileUrl { get; set; }

        public string ImportedFrom { get; internal set; }

        public int? RowsImported { get; internal set; }

        public int? RowsFailed { get; internal set; }

        public int? RowsAffected { get; set; }

        public int? RowsInserted { get; set; }

        public int? RowsUpdated { get; set; }

        public int? RowsSkipped { get; set; }

        public int RowsResolved { get; internal set; }

        public int NotReolved { get; internal set; }

        public string FileType { get; internal set; }
    }

    public class EntityParsingLog : EntityBase
    {
        public int Id { get; set; }

        public int ColumnId { get; set; }

        public int LogSummaryId { get; set; }

        public string RecordJson { get; set; }

        public ParsingStatus Status { get; set; }

        public int LastUpdatedBy { get; set; }
    }

    public class EnityParsingDetails : EntityBase
    {
        public int ImportDetailsId { get; set; }

        public int ImportSummaryId { get; set; }

        public string ImportedData { get; set; }

        public int? ImportStatus { get; internal set; }

        public IEnumerable<ImportMessage> ImportMessages { get; set; }
    }

    public class ResolvedSalesData
    {
        public int DataType { get; set; }

        public int RepId { get; set; }

        public string RepName { get; set; }

        public int? RepGroupId { get; set; }

        public string RepGroupName { get; set; }

        public IEnumerable<SelectlistItem> SelectList { get; set; }
    }

    public class SelectlistItem
    {
        public int Id { get; set; }

        public string Value { get; set; }
    }

    public class ImportMessage
    {
        public int Id { get; set; }

        public string ColumnName { get; set; }

        public string Message { get; set; }
    }
}
