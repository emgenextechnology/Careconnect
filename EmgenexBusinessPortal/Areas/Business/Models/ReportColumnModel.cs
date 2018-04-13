using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmgenexBusinessPortal.Areas.Business.Models
{
    public class ReportColumnModel 
    {
        // GET: Admin/ReportColumnModel
        public int Id { get; set; }
        public int BusinessId { get; set; }
        public int ServiceId { get; set; }
        public string ColumnName { get; set; }
        public int? ColumnType { get; set; }
        public bool? IsMandatory { get; set; }
        public bool? DisplayInFilter { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public int[] DepartmentPrivileges { get; set; }
        public int[] RolePrivileges { get; set; }
        public int[] UserPrivileges { get; set; }
        public bool? IsDeny { get; set; }
        public bool? ShowInGrid { get; set; }
        public string DisplayName { get; set; }
        public int? InputType { get; set; }
    }

    public class LookupServiceColumnsModel
    {
        public int Id { get; set; }
        public int ColumnId { get; set; }
        public string Text { get; set; }
        public int? Value { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
    }
}