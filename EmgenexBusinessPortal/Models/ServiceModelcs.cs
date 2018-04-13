using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmgenexBusinessPortal.Models
{
    public class ServiceModelcs
    {
        public class ServiceModel
        {
            // GET: Business/ServiceModel
            public int ServiceId { get; set; }

            public bool Status { get; set; }
        }
       
        public class EnrolledServiceModel
        {
            public DateTime? CreatedOn = DateTime.UtcNow;
            public DateTime? UpdatedOn = DateTime.UtcNow;

            public int Id { get; set; }

            public int BusinessId { get; set; }

            public int ImportMode { get; set; }

            public string BoxUrl { get; set; }

            public string ServiceName { get; set; }

            public string ServiceDecription { get; set; }

            public bool? IsActive { get; set; }

            public bool? Status { get; set; }

            public string ServiceColor { get; set; }

            public int CreatedBy { get; set; }

            public int? UpdatedBy { get; set; }

            public int? OldId { get; set; }

            public string CreatedByName { get; set; }

            public string UpdatedByName { get; set; }

            public string OldServiceName { get; set; }

            public EnrolledServiceFtpModel FtpInfo { get; set; }

        }

        public class EnrolledServiceFtpModel
        {
            public string Host { get; set; }

            public int? Protocol { get; set; }

            public string Username { get; set; }

            public string Passsword { get; set; }

            public string RemotePath { get; set; }

            public int? PortNumber { get; set; }
        }


        public class ReportColumnModel
        {
            public DateTime CreatedOn = DateTime.UtcNow;

            public DateTime? UpdatedOn = DateTime.UtcNow;

            public int Id { get; set; }

            public int BusinessId { get; set; }

            public int ServiceId { get; set; }

            public string ColumnName { get; set; }

            public bool? IsMandatory { get; set; }

            public bool? DisplayInFilter { get; set; }

            public bool? ShowInGrid { get; set; }

            public int CreatedBy { get; set; }

            public int? UpdatedBy { get; set; }

            public string DisplayName { get; set; }

            public List<int> RolePrivilegeIds { get; set; }

            public List<int> DepartmentPrivilegeIds { get; set; }

            public List<int> UserPrivilegeIds { get; set; }

            public int? ColumnType { get; set; }
            public int? InputType { get; set; }

            public string ServiceName { get; set; }

            public string CreatedByName { get; set; }

            public string UpdatedByName { get; set; }
        }
        public class ReportColumnLookupModel
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
}