using EBP.Business.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBP.Business.Enums
{
    public enum TaskStatuses
    {
        New = 1,
        InProgress = 2,
        Completed = 3
    }

    public enum TaskPriorities
    {
        High = 1,
        //Medium = 2,
        Low = 3,
    }

    public enum LeadStatus
    {
        New = 1,
        Transacted = 2,
    }

    public enum FilterStatus
    {
        New = 1,
        Active = 2,
        Dormant = 3,
        Inactive = 4
    }

    public enum AddressType
    {
        Primary = 1,
        Secondary = 2,
    }

    public enum PhoneType
    {
        Mobile = 1,
        Work = 2,
        Home = 3,
    }

    public enum Periods
    {
        Today = 0,
        Yesterday,
        ThisWeek,
        LastWeek,
        ThisMonth,
        LastMonth,
        ThisYear,
        LastYear
    }

    public enum DashboardPeriods
    {
        [Display(Name = "This Week")]
        ThisWeek = 1,
        [Display(Name = "Last Week")]
        LastWeek,
        [Display(Name = "This Month")]
        ThisMonth,
        [Display(Name = "Last Month")]
        LastMonth,
        [Display(Name = "Last 3 Months")]
        Last3Months,
        [Display(Name = "Last 6 Months")]
        Last6Months,
        [Display(Name = "Last 12 Months")]
        Last12Months,
        [Display(Name = "This Year")]
        ThisYear,
        [Display(Name = "Last Year")]
        LastYear
    }

    public enum DashboardSalesViewBy
    {
        Year = 1,
        Month,
        Week,
        Day
    }

    public enum DashboardSalesDateType
    {
        Collected = 1,
        Received,
        Reported
    }

    public enum DashboardSalesTotal
    {
        Sales = 1,
        //Reimbursements,
        //Billed
    }

    public enum Errors
    {
        NoRole = 0,
        NoPrivilege = 1,
        NoAccess = 2,
        NoBusiness = 3,
    }

    public enum NotificationTargetType
    {
        Lead = 1,
        Account = 2,
        Task = 3,
        Sales = 4
    }

    public enum NotificationType
    {
        Normal = 1
    }

    public enum ContactPreference
    {
        Email = 1,
        Phone = 2
    }

    public enum NoteType
    {
        Lead = 1,
        Task = 3
    }

    public enum UserColumnModules
    {
        Lead = 1,
        Account = 2,
        Task = 3,
        Sales = 4
    }

    public enum AssignedOrRequest
    {
        AssignedToMe = 1,
        RequestedByMe = 2,
    }

    public enum ParsingStatus
    {
        Success = 1,
        Failed = 2,
        Skipped = 3
    }

    public enum SalesImportStatus
    {
        Inserted = 1,
        Updated = 2,
        Skiped = 3,
        Resolved = 4,
        NotResolved = 5,
        Duplicated = 6
    }

    public enum SalesDataMode
    {
        ServiceImport = 1,
        WebUpload,
        ManualSingleEntry,
        ManualBulkEntry
    }

    public enum SalesDataType
    {
        MasterData = 1,
        FinanceData
    }

    public enum IncomingType
    {
        Practice = 1,
        Provider,
        Rep
    }

    public enum SalesColumnType
    {
        ReportMaster = 1,
        ReportFinance,
    }

    public enum ServiceReportImportModes
    {
        FileSystem = 1,
        BoxAPI,
        Ftp,
        Web
    }

    public enum FTPProtocol
    {
        Sftp = 0,
        Scp = 1,
        Ftp = 2,
        Webdav = 3
    }

    public enum SalesInputType
    {
        Text = 1,
        Dropdown
    }
    public enum RepgroupUserType
    {
        Manager = 1,
        Director = 2
    }

    public enum SalesGroupBy
    {
        None = 0,
        Practice,
        Rep,
        Sales
    }

    public static class EnumHelper
    {
        public static EntityList<EntitySelectItem> GetEnumList(Type t)
        {
            EntityList<EntitySelectItem> entitiList = new EntityList<EntitySelectItem>();
            foreach (var name in Enum.GetNames(t))
            {
                entitiList.List.Add(
                    new EntitySelectItem
                    {
                        Id = (int)Enum.Parse(t, name),
                        Value = name
                    });
            }
            return entitiList;
        }

        public static string GetEnumName<T>(int id) where T : struct
        {
            Type t = typeof(T);
            return Enum.GetName(t, id);
        }
    }
}
