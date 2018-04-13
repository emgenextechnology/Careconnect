using EBP.Business.Entity;
using EBP.Business.Entity.Sales;
using EBP.Business.Enums;
using EBP.Business.Filter;
using EBP.Business.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Data.Entity.Migrations;
using System.Linq.Dynamic;
using System.IO;
using EBP.Business.Entity.ParsingLog;
using Newtonsoft.Json;

namespace EBP.Business.Repository
{

    public class RepositorySales : _Repository
    {
        private List<Database.SalesImportMessage> salesImportMessages = null;

        private List<DynamicColumns> ReportStaticColumns
        {
            get
            {
                try
                {
                    List<DynamicColumns> statics = XmlMapper.Descendants("StaticColumns").Elements().Select((a, i) => new DynamicColumns
                    {
                        OrderIndex = i,
                        ColumnName = a.Name.LocalName,
                        DisplayName = a.Attribute("DisplayName").Value,
                        IsVisible = ParseToBool(a.Attribute("IsVisible").Value)
                    }).ToList();

                    return statics;
                }
                catch
                {
                    return new List<DynamicColumns>();
                }
            }
        }

        public XDocument XmlMapper { get; set; }

        #region For Admin Panel
        public IEnumerable<DynamicColumns> GetReportStaticColumns(int serviceID)
        {
            base.DBInit();
            var serviceStaticColumns = DBEntity.ReportStaticColumnConfigs
                .Where(a => a.ServiceId == serviceID)
                .Select(a => new DynamicColumns
                {
                    ColumnName = a.ColumnName,
                    Id = a.Id,
                    IsVisible = a.IsVisible,
                    OrderIndex = a.ColumnOrder
                }).ToList();
            base.DBClose();

            var reportStaticColumns = ReportStaticColumns;

            foreach (var item in serviceStaticColumns)
            {
                var column = reportStaticColumns.Where(a => a.ColumnName == item.ColumnName).FirstOrDefault();
                if (column != null)
                {
                    reportStaticColumns.Remove(column);
                }
            }

            return serviceStaticColumns.Concat(reportStaticColumns);
        }
        #endregion

        public DataResponse<EntityList<EntitySales>> GetAllList(FilterSales filter, int? currentBusinessId, int currentUserId, bool isBuzAdmin, string[] currentRoles, string[] currentDepartments, string[] currentPrivileges, bool isRepOrManager, bool isSalesDirector, bool displayPatientName, int take = 10, int skip = 0, bool includeLog = true, bool isDataForExport = false, string mapperFilePath = "")
        {
            var response = new DataResponse<EntityList<EntitySales>>();
            try
            {
                bool hasFinanceData = false;
                if (!string.IsNullOrEmpty(mapperFilePath))
                {
                    XDocument XmlMapper = XDocument.Load(mapperFilePath);
                    var objFinanceElement = XmlMapper.Descendants("ReportFinance");
                    hasFinanceData = objFinanceElement.Count() > 0;
                }

                base.DBInit();

                var query = DBEntity.ReportMasters.Where(a => a.BusinessId == currentBusinessId);

                if (filter.ServiceId.HasValue)
                {
                    query = query.Where(a => a.ServiceId == filter.ServiceId);
                }

                if (filter.LogId.HasValue && filter.LogId != 0)
                {
                    if (filter.LogStatuses != null && filter.LogStatuses.Count() > 0)
                    {
                        query = DBEntity.SalesImportDetails.Where(a => a.ImportSummeryId == filter.LogId && filter.LogStatuses.Any(b=>b==a.ImportStatus)).Select(a => a.ReportMaster);
                    }
                    else
                    {
                        query = DBEntity.SalesImportDetails.Where(a => a.ImportSummeryId == filter.LogId).Select(a => a.ReportMaster);
                    }
                }

                if (!isBuzAdmin)
                {
                    if (isRepOrManager || isSalesDirector)
                        query = query.Where(a => a.Rep.User2.Reps2.Any(b => b.UserId == currentUserId) ||
                            a.Rep.RepGroup.RepgroupManagerMappers.Any(b => b.ManagerId == currentUserId) || //---ManagerChange    a.Rep.User2.Reps2.Select(b => b.RepGroup.ManagerId).Contains(currentUserId) ||
                            a.Rep.User2.Reps2.Any(b => b.RepGroup.RepgroupManagerMappers.Any(c=>c.ManagerId== currentUserId && c.UserRole == (int)RepgroupUserType.Director)) ||
                            a.CreatedBy == currentUserId);
                }


                if (filter != null)
                {
                    #region FIlter

                    take = filter.Take;
                    skip = filter.Skip;

                    if (!String.IsNullOrEmpty(filter.Keyword))
                    {
                        filter.Keyword = filter.Keyword.ToLower();
                        query = query.Where(a => (a.ProviderFirstName + a.ProviderLastName + a.PatientFirstName + a.PatientLastName + a.Practice.PracticeName).ToLower().Contains(filter.Keyword)
                            || a.SpecimenId.Contains(filter.Keyword)
                            //|| a.Provider.MiddleName.ToLower().Contains(filter.Keyword)
                            //|| a.Provider.LastName.ToLower().Contains(filter.Keyword)
                            //|| a.Practice.Rep.RepGroup.RepGroupName.Contains(filter.Keyword)
                            //|| a.Practice.PracticeName.ToLower().Contains(filter.Keyword)
                            //|| a.Rep.User2.Reps2.Any(b => b.User2.FirstName.Contains(filter.Keyword))
                            || a.ReportFinances.Any(b => b.Charges.ToString() == filter.Keyword)
                            || a.ReportColumnValues.Any(b => b.Value.ToLower().Contains(filter.Keyword)));
                    }
                    if (filter.GroupBy > 0)
                    {
                        switch (filter.GroupBy)
                        {
                            case 1:
                                query = query.Where(a => a.PracticeId == filter.GroupByPracticeId);
                                break;
                            case 2:
                                query = query.Where(a => a.RepId == filter.GroupByRepId);
                                break;
                            case 3:
                                query = query.Where(a => a.Rep.RepGroupId == filter.GroupByRepGroupId);
                                break;
                        }
                    }

                    if (filter.ProviderId > 0)
                        query = query.Where(a => a.ProviderId == filter.ProviderId);

                    if (filter.PracticeId > 0)
                        query = query.Where(a => a.PracticeId == filter.PracticeId);

                    if (filter.ReportId > 0)
                        query = query.Where(a => a.Id == filter.ReportId);

                    #region Date Filter Backup

                    //if (filter.WrittenDateFrom.HasValue && filter.WrittenDateFrom != DateTime.MinValue)
                    //{
                    //    var writtenDateFrom = filter.WrittenDateFrom.Value.Date;
                    //    query = query.Where(a => DbFunctions.TruncateTime(a.CreatedOn) >= writtenDateFrom);
                    //}

                    //if (filter.WrittenDateTo.HasValue && filter.WrittenDateTo != DateTime.MinValue)
                    //{
                    //    var writtenDateTo = filter.WrittenDateTo.Value.Date;
                    //    query = query.Where(a => DbFunctions.TruncateTime(a.CreatedOn) <= writtenDateTo);
                    //}

                    //if (filter.ReportedDateFrom.HasValue && filter.ReportedDateFrom != DateTime.MinValue)
                    //{
                    //    var reportedDateFrom = filter.ReportedDateFrom.Value.Date;
                    //    query = query.Where(a => DbFunctions.TruncateTime(a.ReportedDate) >= reportedDateFrom);
                    //}

                    //if (filter.ReportedDateTo.HasValue && filter.ReportedDateTo != DateTime.MinValue)
                    //{
                    //    var reportedDateTo = filter.ReportedDateTo.Value.Date;
                    //    query = query.Where(a => DbFunctions.TruncateTime(a.ReportedDate) <= reportedDateTo);
                    //}
                    //

                    #endregion

                    if (filter.CollectedDateFrom.HasValue && filter.CollectedDateFrom != DateTime.MinValue)
                    {
                        var collectedDateFrom = filter.CollectedDateFrom.Value.Date;
                        query = query.Where(a => DbFunctions.TruncateTime(a.SpecimenCollectionDate) >= collectedDateFrom);
                    }

                    if (filter.CollectedDateTo.HasValue && filter.CollectedDateTo != DateTime.MinValue)
                    {
                        var collectedDateTo = filter.CollectedDateTo.Value.Date;
                        query = query.Where(a => DbFunctions.TruncateTime(a.SpecimenCollectionDate) <= collectedDateTo);
                    }

                    if (filter.ReceivedDateFrom.HasValue && filter.ReceivedDateFrom != DateTime.MinValue)
                    {
                        var receivedDateFrom = filter.ReceivedDateFrom.Value.Date;
                        query = query.Where(a => DbFunctions.TruncateTime(a.SpecimenReceivedDate) >= receivedDateFrom);
                    }

                    if (filter.ReceivedDateTo.HasValue && filter.ReceivedDateTo != DateTime.MinValue)
                    {
                        var receivedDateTo = filter.ReceivedDateTo.Value.Date;
                        query = query.Where(a => DbFunctions.TruncateTime(a.SpecimenReceivedDate) <= receivedDateTo);
                    }

                    if (filter.RepGroupIds != null && filter.RepGroupIds.Length > 0)
                        query = query.Where(a => a.Practice.Rep.RepGroupId == filter.RepGroupId);

                    if (filter.RepIds != null && filter.RepIds.Length > 0)
                        query = query.Where(a => a.Practice.RepId == filter.RepId);

                    if (filter.DynamicFilters != null)
                        foreach (var dFilter in filter.DynamicFilters)
                        {
                            if (!string.IsNullOrEmpty(dFilter.ColumnValue))
                                query = query.Where(a => a.ReportColumnValues.Any(k => k.ColumnId == dFilter.Id && k.Value.Contains(dFilter.ColumnValue)));
                        }

                    if (filter.DynamicFilters != null)
                    {
                        var objSave = filter.DynamicFilters.Select(a => new Database.UserColumnVisibility
                        {
                            BusinessId = currentBusinessId.Value,
                            UserId = currentUserId,
                            ServiceId = filter.ServiceId.Value,
                            ModuleId = (int)UserColumnModules.Sales,
                            IsVisible = a.IsVisible ?? true,
                            ColumnName = a.ColumnName,
                            DisplayName = a.DisplayName,
                            CreatedOn = DateTime.UtcNow,
                            CreatedBy = currentUserId
                        }).ToArray();

                        if (!displayPatientName)
                            filter.DynamicFilters.RemoveAll(a => a.ColumnName == "Patient");

                        try
                        {
                            DBEntity.UserColumnVisibilities.AddOrUpdate(a => new { a.ColumnName, a.ServiceId, a.ModuleId, a.UserId }, objSave);

                            DBEntity.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            ex.Log();
                        }
                    }

                    #endregion
                }

                IQueryable<EntitySales> selectQuery = null;

                selectQuery = GenerateQuery(
                        query,
                        currentBusinessId,
                        currentUserId,
                        isBuzAdmin,
                        currentRoles,
                        currentDepartments,
                        currentPrivileges,
                        isRepOrManager,
                        isSalesDirector,
                        displayPatientName, includeLog, hasFinanceData, isDataForExport);

                #region Sort

                if (string.IsNullOrEmpty(filter.SortKey))
                {
                    selectQuery = selectQuery.OrderByDescending(o => o.ReportId);
                }
                else
                {
                    if (filter.SortKey.ToLower() == "salesteam")
                        filter.SortKey = "RepGroup";

                    if (filter.SortKey.ToLower() == "collecteddate")
                        filter.SortKey = "CollectionDate";

                    bool isSpecimenId = false;
                    if (filter.SortKey.ToLower() == "specimenid")
                    {
                        filter.SortKey = "SpecimenId";
                        isSpecimenId = true;
                    }

                    var propertyInfo = typeof(EntitySales).GetProperty(filter.SortKey);

                    if (propertyInfo != null && propertyInfo.DeclaringType.Name == "FinanceData")
                    {
                        string orderBy = string.Format("{0}.{1} {2}", "FinanceData", filter.SortKey, filter.OrderBy);
                        selectQuery = selectQuery.OrderBy(orderBy);
                    }
                    else if (propertyInfo != null || isSpecimenId)
                    {
                        switch (filter.SortKey)
                        {
                            case "Provider":
                                if (filter.OrderBy == "desc")
                                    selectQuery = selectQuery.OrderByDescending(c => c.ProviderFirstName).ThenBy(n => n.ProviderMiddleName).ThenBy(n => n.ProviderLastName);
                                else
                                    selectQuery = selectQuery.OrderBy(c => c.ProviderFirstName).ThenBy(n => n.ProviderMiddleName).ThenBy(n => n.ProviderLastName);
                                break;
                            case "RepName":
                                if (filter.OrderBy == "desc")
                                    selectQuery = selectQuery.OrderByDescending(c => c.RepFirstName).ThenBy(n => n.RepLastName);
                                else
                                    selectQuery = selectQuery.OrderBy(c => c.RepFirstName).ThenBy(n => n.RepLastName);
                                break;
                            case "Patient":
                                if (filter.OrderBy == "desc")
                                    selectQuery = selectQuery.OrderByDescending(c => c.PatientFirstName).ThenBy(n => n.PatientLastName);
                                else
                                    selectQuery = selectQuery.OrderBy(c => c.PatientFirstName).ThenBy(n => n.PatientLastName);
                                break;
                            default:
                                string orderBy = string.Format("{0} {1}", filter.SortKey, filter.OrderBy);
                                selectQuery = selectQuery.OrderBy(orderBy);
                                break;
                        }
                    }
                    else
                    {
                        if (filter.OrderBy == "desc")
                            selectQuery = selectQuery.OrderByDescending(x => x.ReportColumnValues.Where(y => y.ColumnName == filter.SortKey).Select(z => z.Value).FirstOrDefault());
                        else
                            selectQuery = selectQuery.OrderBy(x => x.ReportColumnValues.Where(y => y.ColumnName == filter.SortKey).Select(z => z.Value).FirstOrDefault());
                    }
                }

                #endregion

                response = GetList<EntitySales>(selectQuery, skip, take);

                var reportColumns = DBEntity.ReportColumns.Where(b => (isBuzAdmin || b.SalesDepartmentPrivileges.Any(dp => currentDepartments.Contains(dp.Department.DepartmentName))
                    || b.SalesRolePrivileges.Any(rp => currentRoles.Contains(rp.Role.Name))
                    || b.SalesUserPrivileges.Any(up => up.UserId == currentUserId))
                    && b.ServiceId == filter.ServiceId && b.BusinessId == currentBusinessId)
                    .Select(b => new ReportColumnValue
                    {
                        ColumnType = b.ColumnType,
                        ColumnId = b.Id,
                        ColumnName = b.ColumnName,
                        DisplayName = b.DisplayName,
                        Value = null
                    }).ToList();

                foreach (var item in response.Model.List)
                {
                    item.ReportColumnValues = item.ReportColumnValues
                        .Concat(reportColumns.Where(a => !item.ReportColumnValues.Select(b => b.ColumnId).Contains(a.ColumnId)))
                        .OrderByDescending(o => o.ColumnType).ToList();
                }
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }
            return response;
        }

        public DataResponse<EntityList<GroupedSales>> GetAllGroupedSales(FilterSales filter, int? currentBusinessId, int currentUserId, bool isBuzAdmin, string[] currentRoles, string[] currentDepartments, string[] currentPrivileges, bool isRepOrManager, bool isSalesDirector, bool displayPatientName, int take = 10, int skip = 0, bool includeLog = true, bool isDataForExport = false, string mapperFilePath = "")
        {
            var response = new DataResponse<EntityList<GroupedSales>>();
            try
            {
                //bool hasMultipleFinanceData = false;
                //if (!string.IsNullOrEmpty(mapperFilePath))
                //{
                //    XDocument XmlMapper = XDocument.Load(mapperFilePath);
                //    var objFinanceElement = XmlMapper.Descendants("ReportFinance");
                //    var objHasMultiFinData = objFinanceElement.Attributes("HasMultipleFinanceData").FirstOrDefault();
                //    if (objHasMultiFinData != null)
                //        bool.TryParse(objHasMultiFinData.Value, out hasMultipleFinanceData);
                //}

                bool hasFinanceData = false;
                if (!string.IsNullOrEmpty(mapperFilePath))
                {
                    XDocument XmlMapper = XDocument.Load(mapperFilePath);
                    var objFinanceElement = XmlMapper.Descendants("ReportFinance");
                    hasFinanceData = objFinanceElement.Count() > 0;
                }

                base.DBInit();

                var query = DBEntity.ReportMasters.Where(a => a.BusinessId == currentBusinessId && a.ServiceId == filter.ServiceId);

                if (filter.LogId.HasValue && filter.LogId != 0)
                {
                    if (filter.LogStatuses != null && filter.LogStatuses.Count() > 0)
                    {
                        query = DBEntity.SalesImportDetails.Where(a => a.ImportSummeryId == filter.LogId && filter.LogStatuses.Any(b => b == a.ImportStatus)).Select(a => a.ReportMaster);
                    }
                    else
                    {
                        query = DBEntity.SalesImportDetails.Where(a => a.ImportSummeryId == filter.LogId).Select(a => a.ReportMaster);
                    }
                }

                if (!isBuzAdmin)
                {
                    if (isRepOrManager || isSalesDirector)
                        query = query.Where(a => a.Rep.User2.Reps2.Any(b => b.UserId == currentUserId) ||
                            a.Rep.RepGroup.RepgroupManagerMappers.Any(b => b.ManagerId == currentUserId) || //---ManagerChange    a.Rep.User2.Reps2.Select(b => b.RepGroup.ManagerId).Contains(currentUserId) ||
                            a.Rep.User2.Reps2.Any(b => b.RepGroup.RepgroupManagerMappers.Any(c=>c.ManagerId== currentUserId && c.UserRole == (int)RepgroupUserType.Director)) ||
                            a.CreatedBy == currentUserId);
                }

                if (filter != null)
                {
                    take = filter.Take;
                    skip = filter.Skip;


                    if (!String.IsNullOrEmpty(filter.Keyword))
                    {
                        filter.Keyword = filter.Keyword.ToLower();

                        query = query.Where(a => (a.ProviderFirstName + a.ProviderLastName + a.PatientFirstName + a.PatientLastName + a.Practice.PracticeName).ToLower().Contains(filter.Keyword)
                            || a.SpecimenId.Contains(filter.Keyword)
                            //|| a.Provider.MiddleName.ToLower().Contains(filter.Keyword)
                            //|| a.Provider.LastName.ToLower().Contains(filter.Keyword)
                            //|| a.Practice.Rep.RepGroup.RepGroupName.Contains(filter.Keyword)
                            //|| a.Practice.PracticeName.ToLower().Contains(filter.Keyword)
                            //|| a.Rep.User2.Reps2.Any(b => b.User2.FirstName.Contains(filter.Keyword))
                            || a.ReportFinances.Any(b => b.Charges.ToString() == filter.Keyword)
                            || a.ReportColumnValues.Any(b => b.Value.ToLower().Contains(filter.Keyword)));
                    }
                    if (filter.ProviderId > 0)
                        query = query.Where(a => a.ProviderId == filter.ProviderId);

                    if (filter.PracticeId > 0)
                        query = query.Where(a => a.PracticeId == filter.PracticeId);

                    if (filter.ReportId > 0)
                        query = query.Where(a => a.Id == filter.ReportId);

                    #region Date Filter Backup

                    //if (filter.WrittenDateFrom.HasValue && filter.WrittenDateFrom != DateTime.MinValue)
                    //{
                    //    var writtenDateFrom = filter.WrittenDateFrom.Value.Date;
                    //    query = query.Where(a => DbFunctions.TruncateTime(a.CreatedOn) >= writtenDateFrom);
                    //}

                    //if (filter.WrittenDateTo.HasValue && filter.WrittenDateTo != DateTime.MinValue)
                    //{
                    //    var writtenDateTo = filter.WrittenDateTo.Value.Date;
                    //    query = query.Where(a => DbFunctions.TruncateTime(a.CreatedOn) <= writtenDateTo);
                    //}

                    //if (filter.ReportedDateFrom.HasValue && filter.ReportedDateFrom != DateTime.MinValue)
                    //{
                    //    var reportedDateFrom = filter.ReportedDateFrom.Value.Date;
                    //    query = query.Where(a => DbFunctions.TruncateTime(a.ReportedDate) >= reportedDateFrom);
                    //}

                    //if (filter.ReportedDateTo.HasValue && filter.ReportedDateTo != DateTime.MinValue)
                    //{
                    //    var reportedDateTo = filter.ReportedDateTo.Value.Date;
                    //    query = query.Where(a => DbFunctions.TruncateTime(a.ReportedDate) <= reportedDateTo);
                    //}
                    //

                    #endregion

                    if (filter.CollectedDateFrom.HasValue && filter.CollectedDateFrom != DateTime.MinValue)
                    {
                        var collectedDateFrom = filter.CollectedDateFrom.Value.Date;
                        query = query.Where(a => DbFunctions.TruncateTime(a.SpecimenCollectionDate) >= collectedDateFrom);
                    }

                    if (filter.CollectedDateTo.HasValue && filter.CollectedDateTo != DateTime.MinValue)
                    {
                        var collectedDateTo = filter.CollectedDateTo.Value.Date;
                        query = query.Where(a => DbFunctions.TruncateTime(a.SpecimenCollectionDate) <= collectedDateTo);
                    }

                    if (filter.ReceivedDateFrom.HasValue && filter.ReceivedDateFrom != DateTime.MinValue)
                    {
                        var receivedDateFrom = filter.ReceivedDateFrom.Value.Date;
                        query = query.Where(a => DbFunctions.TruncateTime(a.SpecimenReceivedDate) >= receivedDateFrom);
                    }

                    if (filter.ReceivedDateTo.HasValue && filter.ReceivedDateTo != DateTime.MinValue)
                    {
                        var receivedDateTo = filter.ReceivedDateTo.Value.Date;
                        query = query.Where(a => DbFunctions.TruncateTime(a.SpecimenReceivedDate) <= receivedDateTo);
                    }

                    if (filter.RepGroupIds != null && filter.RepGroupIds.Length > 0)
                        query = query.Where(a => a.Practice.Rep.RepGroupId == filter.RepGroupId);

                    if (filter.RepIds != null && filter.RepIds.Length > 0)
                        query = query.Where(a => a.Practice.RepId == filter.RepId);

                    if (filter.DynamicFilters != null)
                        foreach (var dFilter in filter.DynamicFilters)
                        {
                            if (!string.IsNullOrEmpty(dFilter.ColumnValue))
                                query = query.Where(a => a.ReportColumnValues.Any(k => k.ColumnId == dFilter.Id && k.Value.Contains(dFilter.ColumnValue)));
                        }

                    if (filter.DynamicFilters != null)
                    {
                        var objSave = filter.DynamicFilters.Select(a => new Database.UserColumnVisibility
                        {
                            BusinessId = currentBusinessId.Value,
                            UserId = currentUserId,
                            ServiceId = filter.ServiceId.Value,
                            ModuleId = (int)UserColumnModules.Sales,
                            IsVisible = a.IsVisible ?? true,
                            ColumnName = a.ColumnName,
                            DisplayName = a.DisplayName,
                            CreatedOn = DateTime.UtcNow,
                            CreatedBy = currentUserId
                        }).ToArray();

                        if (!displayPatientName)
                            filter.DynamicFilters.RemoveAll(a => a.ColumnName == "Patient");

                        try
                        {
                            DBEntity.UserColumnVisibilities.AddOrUpdate(a => new { a.ColumnName, a.ServiceId, a.ModuleId, a.UserId }, objSave);

                            DBEntity.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            ex.Log();
                        }
                    }
                }

                #region Grouping

                switch (filter.GroupBy)
                {
                    case 1:
                        var objGroupedDataByPractice = query.GroupBy(a => a.PracticeId)
                            .Select(a => new { First = a.FirstOrDefault(), Count = a.Count(), LastActivityOn = a.Max(m => m.CreatedOn) })
                            .Select(a => new GroupedSales
                            {
                                ReportId = a.First.Id,
                                PracticeId = a.First.PracticeId,
                                KeyName = a.First.Practice.PracticeName,
                                LastActivityOn = a.LastActivityOn,
                                Count = a.Count
                            });

                        if (string.IsNullOrEmpty(filter.SortKey))
                            objGroupedDataByPractice = objGroupedDataByPractice.OrderByDescending(o => o.PracticeId == null).ThenBy(o => o.KeyName);
                        else
                            objGroupedDataByPractice = SortGroupedData(objGroupedDataByPractice, filter.SortKey, filter.OrderBy);

                        response = isDataForExport ? GetList<GroupedSales>(objGroupedDataByPractice) : GetList<GroupedSales>(objGroupedDataByPractice, skip, take);
                        break;
                    case 2:
                        var objGroupedDataByRep = query.GroupBy(a => a.RepId)
                            .Select(a => new { First = a.FirstOrDefault(), Count = a.Count(), LastActivityOn = a.Max(m => m.CreatedOn) })
                            .Select(a => new GroupedSales
                            {
                                ReportId = a.First.Id,
                                RepId = a.First.RepId,
                                KeyName = a.First.Rep.User2.FirstName + " " + a.First.Rep.User2.LastName,
                                LastActivityOn = a.LastActivityOn,
                                Count = a.Count
                            });

                        if (string.IsNullOrEmpty(filter.SortKey))
                            objGroupedDataByRep = objGroupedDataByRep.OrderByDescending(o => o.RepId == null).ThenBy(o => o.KeyName);
                        else
                            objGroupedDataByRep = SortGroupedData(objGroupedDataByRep, filter.SortKey, filter.OrderBy);

                        response = isDataForExport ? GetList<GroupedSales>(objGroupedDataByRep) : GetList<GroupedSales>(objGroupedDataByRep, skip, take);
                        break;
                    case 3:
                        var objGroupedDataByRepGroup = query.GroupBy(a => a.Rep.RepGroupId)
                            .Select(a => new { First = a.FirstOrDefault(), Count = a.Count(), LastActivityOn = a.Max(m => m.CreatedOn) })
                            .Select(a => new GroupedSales
                            {
                                ReportId = a.First.Id,
                                RepGroupId = a.First.Rep.RepGroupId,
                                KeyName = a.First.Rep.RepGroup.RepGroupName,
                                LastActivityOn = a.LastActivityOn,
                                Count = a.Count
                            });
                        if (string.IsNullOrEmpty(filter.SortKey))
                            objGroupedDataByRepGroup = objGroupedDataByRepGroup.OrderByDescending(o => o.RepGroupId == null).ThenBy(o => o.KeyName);
                        else
                            objGroupedDataByRepGroup = SortGroupedData(objGroupedDataByRepGroup, filter.SortKey, filter.OrderBy);

                        response = isDataForExport ? GetList<GroupedSales>(objGroupedDataByRepGroup) : GetList<GroupedSales>(objGroupedDataByRepGroup, skip, take);
                        break;
                }
                #endregion

                #region Removed Code

                //IQueryable<EntitySales> selectQuery = null;

                //selectQuery = GenerateQuery(
                //        query,
                //        currentBusinessId,
                //        currentUserId,
                //        isBuzAdmin,
                //        currentRoles,
                //        currentDepartments,
                //        currentPrivileges,
                //        isRepOrManager,
                //        isSalesDirector,
                //        displayPatientName, includeLog, hasFinanceData, isDataForExport);

                //#region Sort
                //if (string.IsNullOrEmpty(filter.SortKey))
                //{
                //    selectQuery = selectQuery.OrderByDescending(o => o.ReportId);
                //}
                //else
                //{
                //    if (filter.SortKey.ToLower() == "salesteam")
                //        filter.SortKey = "RepGroup";

                //    if (filter.SortKey.ToLower() == "collecteddate")
                //        filter.SortKey = "CollectionDate";

                //    bool isSpecimenId = false;
                //    if (filter.SortKey.ToLower() == "specimenid")
                //    {
                //        filter.SortKey = "SpecimenId";
                //        isSpecimenId = true;
                //    }

                //    var propertyInfo = typeof(EntitySales).GetProperty(filter.SortKey);

                //    if (propertyInfo != null && propertyInfo.DeclaringType.Name == "FinanceData")
                //    {
                //        string orderBy = string.Format("{0}.{1} {2}", "FinanceData", filter.SortKey, filter.OrderBy);
                //        selectQuery = selectQuery.OrderBy(orderBy);
                //    }
                //    else if (propertyInfo != null || isSpecimenId)
                //    {
                //        switch (filter.SortKey)
                //        {
                //            case "Provider":
                //                if (filter.OrderBy == "desc")
                //                    selectQuery = selectQuery.OrderByDescending(c => c.ProviderFirstName).ThenBy(n => n.ProviderMiddleName).ThenBy(n => n.ProviderLastName);
                //                else
                //                    selectQuery = selectQuery.OrderBy(c => c.ProviderFirstName).ThenBy(n => n.ProviderMiddleName).ThenBy(n => n.ProviderLastName);
                //                break;
                //            case "RepName":
                //                if (filter.OrderBy == "desc")
                //                    selectQuery = selectQuery.OrderByDescending(c => c.RepFirstName).ThenBy(n => n.RepLastName);
                //                else
                //                    selectQuery = selectQuery.OrderBy(c => c.RepFirstName).ThenBy(n => n.RepLastName);
                //                break;
                //            case "Patient":
                //                if (filter.OrderBy == "desc")
                //                    selectQuery = selectQuery.OrderByDescending(c => c.PatientFirstName).ThenBy(n => n.PatientLastName);
                //                else
                //                    selectQuery = selectQuery.OrderBy(c => c.PatientFirstName).ThenBy(n => n.PatientLastName);
                //                break;
                //            default:
                //                string orderBy = string.Format("{0} {1}", filter.SortKey, filter.OrderBy);
                //                selectQuery = selectQuery.OrderBy(orderBy);
                //                break;
                //        }
                //    }
                //    else
                //    {
                //        if (filter.OrderBy == "desc")
                //            selectQuery = selectQuery.OrderByDescending(x => x.ReportColumnValues.Where(y => y.ColumnName == filter.SortKey).Select(z => z.Value).FirstOrDefault());
                //        else
                //            selectQuery = selectQuery.OrderBy(x => x.ReportColumnValues.Where(y => y.ColumnName == filter.SortKey).Select(z => z.Value).FirstOrDefault());
                //    }
                //}
                //#endregion

                //response = GetList<EntitySales>(selectQuery, skip, take);

                //var reportColumns = DBEntity.ReportColumns.Where(b => (isBuzAdmin || b.SalesDepartmentPrivileges.Any(dp => currentDepartments.Contains(dp.Department.DepartmentName))
                //    || b.SalesRolePrivileges.Any(rp => currentRoles.Contains(rp.Role.Name))
                //    || b.SalesUserPrivileges.Any(up => up.UserId == currentUserId))
                //    && b.ServiceId == filter.ServiceId && b.BusinessId == currentBusinessId)
                //    .Select(b => new ReportColumnValue
                //    {
                //        ColumnType = b.ColumnType,
                //        ColumnId = b.Id,
                //        ColumnName = b.ColumnName,
                //        DisplayName = b.DisplayName,
                //        Value = null
                //    }).ToList();

                //foreach (var item in response.Model.List)
                //{
                //    item.ReportColumnValues = item.ReportColumnValues
                //        .Concat(reportColumns.Where(a => !item.ReportColumnValues.Select(b => b.ColumnId).Contains(a.ColumnId)))
                //        .OrderByDescending(o => o.ColumnType).ToList();
                //} 

                #endregion
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }
            return response;
        }

        public IQueryable<GroupedSales> SortGroupedData(IQueryable<GroupedSales> query, string sortKey, string orderBy)
        {
            switch (sortKey)
            {
                case "1":
                    if (orderBy == "asc")
                        query = query.OrderBy(a => a.KeyName);
                    else
                        query = query.OrderByDescending(a => a.KeyName);
                    break;
                case "2":
                    if (orderBy == "asc")
                        query = query.OrderBy(a => a.Count);
                    else
                        query = query.OrderByDescending(a => a.Count);
                    break;
                case "3":
                    if (orderBy == "asc")
                        query = query.OrderBy(a => a.LastActivityOn);
                    else
                        query = query.OrderByDescending(a => a.LastActivityOn);
                    break;
            }

            return query;
        }

        public DataResponse DeleteFinance(int financeId, int currentUserId, bool hasRight)
        {
            var response = new DataResponse<bool>();
            try
            {
                base.DBInit();

                if (!hasRight)
                    return response;

                var model = DBEntity.ReportFinances.FirstOrDefault(a => a.Id == financeId);
                if (model != null)
                {
                    model.DeleteStatus = true;

                    if (base.DBSaveUpdate(model) == 1)
                    {
                        response.Model = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }
            return response;
        }

        IQueryable<EntitySales> GenerateQuery(
            IQueryable<Database.ReportMaster> query,
            int? currentBusinessId,
            int currentUserId,
            bool isBuzAdmin,
            string[] currentRoles,
            string[] currentDepartments,
            string[] currentPrivileges,
            bool isRepOrManager,
            bool isSalesDirector,
            bool displayPatientName, bool includeLog, bool hasFinanceData, bool isDataForExport)
        {
            if (isDataForExport)
                return query.Select(a => new EntitySales
                {
                    ReportedDate = a.ReportedDate,
                    ProviderId = a.ProviderId,
                    ProviderFirstName = a.Provider.FirstName,
                    ProviderMiddleName = a.Provider.MiddleName,
                    ProviderLastName = a.Provider.LastName,
                    NPI = a.Provider.NPI,
                    RepGroup = a.Rep.RepGroup.RepGroupName,
                    RepId = a.RepId,
                    RepFirstName = a.Rep.User2.FirstName,
                    RepLastName = a.Rep.User2.LastName,
                    ReportId = a.Id,
                    CollectionDate = a.SpecimenCollectionDate,
                    ReceivedDate = a.SpecimenReceivedDate,
                    WrittenOn = a.LookupEnrolledService.ServiceName.Contains("Pharmacogenetics") ? null : (DateTime?)a.CreatedOn,
                    CreatedOn = a.CreatedOn,
                    Practice = a.Practice.PracticeName,
                    PracticeId = a.PracticeId,
                    PatientFirstName = !displayPatientName ? string.Empty : a.PatientFirstName,
                    PatientLastName = !displayPatientName ? string.Empty : a.PatientLastName,
                    PatientId = !displayPatientName ? 0 : a.PatientId,
                    SpecimenId = a.SpecimenId,
                    HasFinanceData = hasFinanceData,
                    ReportColumnValues = a.ReportColumnValues.Where(b =>
                            (isBuzAdmin || b.ReportColumn.SalesDepartmentPrivileges.Any(dp => currentDepartments.Contains(dp.Department.DepartmentName))
                            || b.ReportColumn.SalesRolePrivileges.Any(rp => currentRoles.Contains(rp.Role.Name))
                            || b.ReportColumn.SalesUserPrivileges.Any(up => up.UserId == currentUserId))
                        )
                        .GroupBy(g => g.ReportColumn.Id)
                        .Select(s => new { Last = s.OrderByDescending(o => o.Id).FirstOrDefault() })
                        .Select(b => new ReportColumnValue
                        {
                            ColumnType = b.Last.ReportColumn.ColumnType,
                            ColumnId = b.Last.ReportColumn.Id,
                            ColumnName = b.Last.ReportColumn.ColumnName,
                            DisplayName = b.Last.ReportColumn.DisplayName,
                            Value = b.Last.Value
                        }).OrderByDescending(o => o.ColumnType),
                    FinanceDataRecordCount = a.ReportFinances.Count(b => b.DeleteStatus != true),
                    FinanceDataList = a.ReportFinances.Where(b => b.DeleteStatus != true).Select(b => new FinanceRecord
                    {
                        BilledDate = b.BilledDate,
                        PaidDate = b.PaidDate,
                        Charges = b.Charges,
                        PaidAmount = b.PaidAmount,
                        AdjustAmount = b.AdjustAmount,
                        AdjustReason = b.AdjustReason,
                        FinanceColumnValues = b.ReportColumnValues.Where(c =>
                                (isBuzAdmin || c.ReportColumn.SalesDepartmentPrivileges.Any(dp => currentDepartments.Contains(dp.Department.DepartmentName))
                                || c.ReportColumn.SalesRolePrivileges.Any(rp => currentRoles.Contains(rp.Role.Name))
                                || c.ReportColumn.SalesUserPrivileges.Any(up => up.UserId == currentUserId))
                          )
                          .Select(c => new ReportColumnValue
                          {
                              ColumnType = c.ReportColumn.ColumnType,
                              ColumnId = c.ReportColumn.Id,
                              ColumnName = c.ReportColumn.ColumnName,
                              DisplayName = c.ReportColumn.DisplayName,
                              Value = c.Value
                          }),
                    })
                });
            else
                return query.Select(a => new EntitySales
                {
                    ReportedDate = a.ReportedDate,
                    ProviderId = a.ProviderId,
                    ProviderFirstName = a.Provider.FirstName,
                    ProviderMiddleName = a.Provider.MiddleName,
                    ProviderLastName = a.Provider.LastName,
                    NPI = a.Provider.NPI,
                    RepGroup = a.Rep.RepGroup.RepGroupName,
                    RepId = a.RepId,
                    RepFirstName = a.Rep.User2.FirstName,
                    RepLastName = a.Rep.User2.LastName,
                    ReportId = a.Id,
                    CollectionDate = a.SpecimenCollectionDate,
                    ReceivedDate = a.SpecimenReceivedDate,
                    WrittenOn = a.LookupEnrolledService.ServiceName.Contains("Pharmacogenetics") ? null : (DateTime?)a.CreatedOn,
                    CreatedOn = a.CreatedOn,
                    Practice = a.Practice.PracticeName,
                    PracticeId = a.PracticeId,
                    PatientFirstName = !displayPatientName ? string.Empty : a.PatientFirstName,
                    PatientLastName = !displayPatientName ? string.Empty : a.PatientLastName,
                    PatientId = !displayPatientName ? 0 : a.PatientId,
                    SpecimenId = a.SpecimenId,
                    HasFinanceData = hasFinanceData,
                    ReportColumnValues = a.ReportColumnValues.Where(b =>
                            (isBuzAdmin || b.ReportColumn.SalesDepartmentPrivileges.Any(dp => currentDepartments.Contains(dp.Department.DepartmentName))
                            || b.ReportColumn.SalesRolePrivileges.Any(rp => currentRoles.Contains(rp.Role.Name))
                            || b.ReportColumn.SalesUserPrivileges.Any(up => up.UserId == currentUserId))
                        )
                        .GroupBy(g => g.ReportColumn.Id)
                        .Select(s => new { Last = s.OrderByDescending(o => o.Id).FirstOrDefault() })
                        .Select(b => new ReportColumnValue
                        {
                            ColumnType = b.Last.ReportColumn.ColumnType,
                            IntuptType = b.Last.ReportColumn.InputType,
                            ColumnId = b.Last.ReportColumn.Id,
                            ColumnName = b.Last.ReportColumn.ColumnName,
                            DisplayName = b.Last.ReportColumn.DisplayName,
                            Value = b.Last.Value
                        }).OrderByDescending(o => o.ColumnType),
                    FinanceDataRecordCount = a.ReportFinances.Count(b => b.DeleteStatus != true),
                    FinanceData = a.ReportFinances.Where(b => b.DeleteStatus != true).Select(b => new FinanceData
                    {
                        ReportFinanceId = b.Id,
                        BilledDate = b.BilledDate,
                        PaidDate = b.PaidDate,
                        AdjustReason = b.AdjustReason,
                        Charges = a.ReportFinances.Where(c => c.DeleteStatus != true).Sum(c => c.Charges),
                        PaidAmount = a.ReportFinances.Where(c => c.DeleteStatus != true).Sum(c => c.PaidAmount),
                        AdjustAmount = a.ReportFinances.Where(c => c.DeleteStatus != true).Sum(c => c.AdjustAmount),
                    }).OrderByDescending(o => o.ReportFinanceId).FirstOrDefault(),
                    LastParsedData = includeLog ? a.SalesImportDetails.Select(si => new EnityParsingDetails
                    {
                        ImportDetailsId = si.Id,
                        ImportedData = si.ImportedData,
                        ImportSummaryId = si.ImportSummeryId,
                        ImportStatus = si.ImportStatus,
                        ImportMessages = si.SalesImportMessages.Select(im => new ImportMessage
                        {
                            ColumnName = im.ColumnName,
                            Id = im.Id,
                            Message = im.Message
                        })
                    }).OrderByDescending(o => o.ImportDetailsId).FirstOrDefault() : null
                });
        }

        public DataResponse<EntityList<FinanceData>> GetFinanceDataList(int reportId, int? currentBusinessId, int currentUserId, bool isBuzAdmin, string[] currentRoles, string[] currentDepartments)
        {
            var response = new DataResponse<EntityList<FinanceData>>();

            try
            {
                base.DBInit();

                var query = DBEntity.ReportFinances.Where(a => a.ReportId == reportId && a.BusinessId == currentBusinessId && a.DeleteStatus != true).Select(a => new FinanceData
                {
                    ReportFinanceId = a.Id,
                    BilledDate = a.BilledDate,
                    PaidDate = a.PaidDate,
                    Charges = a.Charges,
                    PaidAmount = a.PaidAmount,
                    AdjustAmount = a.AdjustAmount,
                    AdjustReason = a.AdjustReason,
                    ServiceId = a.ServiceId,
                    FinanceColumnValues = a.ReportColumnValues.Where(b =>
                            (isBuzAdmin || b.ReportColumn.SalesDepartmentPrivileges.Any(dp => currentDepartments.Contains(dp.Department.DepartmentName))
                            || b.ReportColumn.SalesRolePrivileges.Any(rp => currentRoles.Contains(rp.Role.Name))
                            || b.ReportColumn.SalesUserPrivileges.Any(up => up.UserId == currentUserId))
                    )
                    .Select(b => new ReportColumnValue
                    {
                        ColumnType = b.ReportColumn.ColumnType,
                        ColumnId = b.ReportColumn.Id,
                        ColumnName = b.ReportColumn.ColumnName,
                        DisplayName = b.ReportColumn.DisplayName,
                        Value = b.Value
                    }),
                });
                response = GetList<FinanceData>(query);

                if (response.Model.List == null || response.Model.List.Count <= 0)
                {
                    response.Model.List = new List<FinanceData> { new FinanceData { FinanceColumnValues = new List<ReportColumnValue>() } };
                }

                int serviceId = 0;
                var objReportMaster = DBEntity.ReportMasters.FirstOrDefault(a => a.Id == reportId);
                if (objReportMaster != null)
                    serviceId = objReportMaster.ServiceId;

                if (serviceId > 0)
                {
                    var reportColumns = DBEntity.ReportColumns.Where(b => b.ColumnType == ((int)SalesColumnType.ReportFinance) && ((isBuzAdmin || b.SalesDepartmentPrivileges.Any(dp => currentDepartments.Contains(dp.Department.DepartmentName))
                        || b.SalesRolePrivileges.Any(rp => currentRoles.Contains(rp.Role.Name))
                        || b.SalesUserPrivileges.Any(up => up.UserId == currentUserId))
                        && b.ServiceId == serviceId && b.BusinessId == currentBusinessId))
                        .Select(b => new ReportColumnValue
                        {
                            ColumnType = (int)SalesColumnType.ReportFinance,
                            ColumnId = b.Id,
                            ColumnName = b.ColumnName,
                            DisplayName = b.DisplayName,
                            Value = null
                        }).ToList();

                    foreach (var item in response.Model.List)
                    {
                        item.FinanceColumnValues = item.FinanceColumnValues
                            .Concat(reportColumns.Where(a => !item.FinanceColumnValues.Select(b => b.ColumnId).Contains(a.ColumnId)))
                            .OrderByDescending(o => o.ColumnType).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }

            return response;
        }

        public DataResponse<EntityList<SalesColumnLookup>> GetColumnLookups(int serviceId, int? currentBusinessId, int currentUserId, bool isBuzAdmin, string[] currentRoles, string[] currentDepartments)
        {
            var response = new DataResponse<EntityList<SalesColumnLookup>>();
            try
            {
                base.DBInit();
                var query = DBEntity.ReportColumns
                    .Where(a => a.BusinessId == currentBusinessId && a.ServiceId == serviceId && a.InputType == (int)SalesInputType.Dropdown)
                    .Select(s => new SalesColumnLookup
                    {
                        ColumnId = s.Id,
                        ColumnLookup = s.LookupServiceColumns.Select(t => new EntitySelectItem
                        {
                            Id = t.Id,
                            Value = t.Text
                        })
                    }).OrderBy(o => o.ColumnId);

                response = GetList<SalesColumnLookup>(query);
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }

            return response;
        }

        public bool IsSalesFileExists(string incommingFileName, int businessId, int serviceId)
        {
            try
            {
                DBInit();
                return
                    DBEntity.SalesImportSummeries.Any(a => a.BusinessId == businessId && a.IncomingFileName == incommingFileName && a.ServiceId == serviceId);
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                DBClose();
            }

            return false;
        }

        public DataResponse Insert(XmlHelper xmlHelper, int recordCount, int? currentBusinessId, int currentUserId, int serviceId, bool isFinanceData, out int importSummeryId, System.IO.StreamWriter logWriter = null, string requestSource = "Service Import", string importedFilePath = "", string incomingFileName = "")
        {
            importSummeryId = 0;
            var response = new DataResponse();
            StringBuilder logString = new StringBuilder();
            int rowsAffected = 0, failedCount = 0, skippedCount = 0, insertCount = 0, updateCount = 0;

            try
            {
                XDocument XmlMapper = xmlHelper.XmlMapper;
                string tempPracticeName = string.Empty;
                var reportMasterList = new List<Database.ReportMaster>();
                var xmlReportMasterData = XmlMapper.Descendants("ReportMaster");
                var xmlReportFinanceData = XmlMapper.Descendants("ReportFinance");
                int reportMasterId = 0;
                bool hasMultipleFinanceData = false, deleteFinanceData = false;
                List<string> logMessages = new List<string>();

                var objHasMultiFinData = xmlReportFinanceData.Attributes("HasMultipleFinanceData").FirstOrDefault();
                if (objHasMultiFinData != null)
                    bool.TryParse(objHasMultiFinData.Value, out hasMultipleFinanceData);

                var objDeleteFinanceData = xmlReportFinanceData.Attributes("DeleteFinanceData").FirstOrDefault();
                if (objDeleteFinanceData != null)
                    bool.TryParse(objDeleteFinanceData.Value, out deleteFinanceData);

                Database.SalesImportSummery salesSummery = new Database.SalesImportSummery
                {
                    BusinessId = currentBusinessId.Value,
                    ServiceId = serviceId,
                    UserId = currentUserId,
                    ImportedDate = DateTime.Now,
                    LogFilePath = ((FileStream)(logWriter.BaseStream)).Name,
                    ImportedFilePath = importedFilePath,
                    DataMode = requestSource == "Service Import" ? (int)SalesDataMode.ServiceImport : (int)SalesDataMode.WebUpload,
                    TotalRecords = recordCount,
                    IncomingFileName = incomingFileName
                };

                if (deleteFinanceData)
                {
                    try
                    {
                        using (var dbEntity = new EBP.Business.Database.CareConnectCrmEntities())
                        {
                            //int excecutionResponse = DBEntity.DeleteFinanceData(CurrentBusinessId, serviceId);
                            bool excecutionResponse = dbEntity.Database.SqlQuery<bool>("exec DeleteFinanceData {0},{1}", currentBusinessId, serviceId).FirstOrDefault();
                            if (!excecutionResponse)
                            {
                                salesSummery.MessageSummery = "Finance Data Deletion Failed";
                                salesSummery.IncomingFileName = incomingFileName.Substring(0, incomingFileName.LastIndexOf("."));
                                goto continueToLog;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.Log();
                        salesSummery.MessageSummery = ex.Message;
                        if (ex.InnerException != null)
                            salesSummery.MessageSummery += " | " + ex.InnerException.Message;
                        salesSummery.IncomingFileName = incomingFileName.Substring(0, incomingFileName.LastIndexOf("."));
                        goto continueToLog;
                    }
                }

                if (IsSalesFileExists(incomingFileName, currentBusinessId.Value, serviceId))
                {
                    salesSummery.MessageSummery = "File already exists | Import Skipped";
                    salesSummery.IncomingFileName = incomingFileName.Substring(0, incomingFileName.LastIndexOf("."));
                    goto continueToLog;
                }

                #region Process Each Record in file

                for (int i = 0; i < recordCount; i++)
                {
                    reportMasterId = 0;
                    salesImportMessages = new List<Database.SalesImportMessage>();
                    int? importStatus = null;
                    int salesDataType = (int)(isFinanceData ? SalesDataType.FinanceData : SalesDataType.MasterData);

                    logString.Clear();
                    try
                    {
                        using (var dbRootContext = new EBP.Business.Database.CareConnectCrmEntities())
                        {
                            var reportMaster = xmlReportMasterData.Select(u =>
                                                                   new Database.ReportMaster
                                                                   {
                                                                       ObjPatientFirstName = xmlHelper.GetNodeValue(ref u, "PatientFirstName"),
                                                                       ObjPatientLastName = xmlHelper.GetNodeValue(ref u, "PatientLastName"),
                                                                       ObjSpecimenCollectionDate = xmlHelper.GetNodeValue(ref u, "SpecimenCollectionDate"),
                                                                       ObjSpecimenReceivedDate = xmlHelper.GetNodeValue(ref u, "SpecimenReceivedDate"),
                                                                       ObjReportedDate = xmlHelper.GetNodeValue(ref u, "ReportedDate"),
                                                                       ObjPracticeName = xmlHelper.GetNodeValue(ref u, "PracticeName"),
                                                                       ObjProviderFirstName = xmlHelper.GetNodeValue(ref u, "ProviderFirstName"),
                                                                       ObjProviderLastName = xmlHelper.GetNodeValue(ref u, "ProviderLastName"),
                                                                       ObjProviderNpi = xmlHelper.GetNodeValue(ref u, "ProviderNpi"),
                                                                       ObjRepFirstName = xmlHelper.GetNodeValue(ref u, "RepFirstName"),
                                                                       ObjRepLastName = xmlHelper.GetNodeValue(ref u, "RepLastName"),
                                                                       ObjPatientId = xmlHelper.GetNodeValue(ref u, "PatientId"),
                                                                       ObjLocationId = xmlHelper.GetNodeValue(ref u, "EmgenexLocationId"),
                                                                       ObjSpecimenId = xmlHelper.GetNodeValue(ref u, "SpecimenId")
                                                                   }).FirstOrDefault();

                            var reportFinance = xmlReportFinanceData.Select(u =>
                                                                   new Database.ReportFinance
                                                                   {
                                                                       ObjBilledDate = xmlHelper.GetNodeValue(ref u, "BilledDate"),
                                                                       ObjPaidDate = xmlHelper.GetNodeValue(ref u, "PaidDate"),
                                                                       ObjCharges = xmlHelper.GetNodeValue(ref u, "Charges"),
                                                                       ObjPaidAmount = xmlHelper.GetNodeValue(ref u, "PaidAmount"),
                                                                       ReportKey = xmlHelper.GetNodeValue(ref u, "ReportKey").Value,
                                                                       ObjAdjustAmount = xmlHelper.GetNodeValue(ref u, "AdjustAmount"),
                                                                       ObjAdjustReason = xmlHelper.GetNodeValue(ref u, "AdjustReason")
                                                                   }).FirstOrDefault();

                            if (reportMaster != null && !string.IsNullOrEmpty(reportMaster.SpecimenId))
                            {
                                string _providerNpi = reportMaster.ProviderNpi;

                                #region ReportMaster and ReportColumnValues Entries

                                reportMaster.ServiceId = serviceId;
                                tempPracticeName = reportMaster.PracticeName;

                                logString.AppendLine(string.Format("Record Number \t:\t{0}\tPatient Id\t:\t{1}\tPatient Name\t:\t{2}\tPractice Name\t:\t{3}",
                                    (i + 1),
                                    reportMaster.PatientId,
                                    reportMaster.PatientFirstName + " " + reportMaster.PatientLastName,
                                    reportMaster.PracticeName));

                                #region NPI Scenario - Based on the input data

                                var provider = dbRootContext.Providers.Where(r => r.NPI == reportMaster.ProviderNpi && r.PracticeProviderMappers.Any(pm => pm.Practice.BusinessId == currentBusinessId));
                                reportMaster.ProviderNpi = provider.Count() > 0 ? reportMaster.ProviderNpi : null;

                                if (string.IsNullOrEmpty(reportMaster.ProviderNpi))
                                {
                                    BuildImportMessage("Provider not resolved by given NPI", "ProviderNpi", reportMaster.ObjProviderNpi.NodeName, _providerNpi, false);
                                }

                                if (reportMaster.ProviderNpi == null && !string.IsNullOrEmpty(reportMaster.ProviderFirstName))
                                {
                                    string nodeName = reportMaster.ObjProviderFirstName.NodeName;
                                    if (nodeName != reportMaster.ObjProviderLastName.NodeName)
                                        nodeName += "," + reportMaster.ObjProviderLastName.NodeName;

                                    //Clinician Key to get Provider Name
                                    var providerData = dbRootContext.Providers.Where(a => a.FirstName == reportMaster.ProviderFirstName && a.LastName == reportMaster.ProviderLastName && a.PracticeProviderMappers.Any(pm => pm.Practice.BusinessId == currentBusinessId));
                                    if (providerData.Count() == 1)
                                    {
                                        var _providerData = providerData.FirstOrDefault();
                                        reportMaster.ProviderId = _providerData.Id;
                                        reportMaster.ProviderNpi = _providerData.NPI;
                                        BuildImportMessage("Provider resolved by name", "ProviderFirstName,ProviderLastName", nodeName, reportMaster.ProviderFirstName + " " + reportMaster.ProviderLastName, true);
                                    }
                                    else
                                    {
                                        BuildImportMessage("Provider not resolved by given provider name", "ProviderFirstName,ProviderLastName", nodeName, reportMaster.ProviderFirstName + " " + reportMaster.ProviderLastName, false);
                                    }
                                }

                                if (reportMaster.ProviderNpi != null)
                                {
                                    //Use NPI Number to get Provider
                                    reportMaster.ProviderId = provider.FirstOrDefault().Id;
                                    reportMaster.ProviderFirstName = reportMaster.ProviderFirstName ?? provider.FirstOrDefault().FirstName;
                                    reportMaster.ProviderLastName = reportMaster.ProviderLastName ?? provider.FirstOrDefault().LastName;

                                    var practiceProviderMapper = dbRootContext.PracticeProviderMappers.Where(a => a.ProviderId == reportMaster.ProviderId && a.Practice.BusinessId == currentBusinessId &&
                                                                                                                a.Practice.Leads.Any(b => b.IsConverted == true && b.Accounts.Any(c => c.IsActive == true)));
                                    if (practiceProviderMapper.Count() == 1)
                                    {
                                        var _practiceProviderMapper = practiceProviderMapper.FirstOrDefault();

                                        reportMaster.PracticeId = _practiceProviderMapper.PracticeId;
                                        reportMaster.RepId = _practiceProviderMapper.Practice.Rep.Id;
                                        reportMaster.RepFirstName = reportMaster.RepFirstName ?? _practiceProviderMapper.Practice.Rep.User2.FirstName;
                                        reportMaster.RepLastName = reportMaster.RepLastName ?? _practiceProviderMapper.Practice.Rep.User2.LastName;
                                        BuildImportMessage("Practice and Rep resolved by provider", isTechnical: true);
                                    }
                                    else if (practiceProviderMapper.Count() > 1)
                                    {
                                        BuildImportMessage("Practice and Rep not resolved by provider", isTechnical: false);
                                        var practiceProviderMapperByPracticeName = practiceProviderMapper.Where(a => a.Practice.PracticeName == tempPracticeName);
                                        if (practiceProviderMapperByPracticeName.Count() == 1)
                                        {
                                            var _practiceProviderMapper = practiceProviderMapperByPracticeName.FirstOrDefault();

                                            reportMaster.PracticeId = _practiceProviderMapper.PracticeId;
                                            reportMaster.RepId = _practiceProviderMapper.Practice.Rep.Id;
                                            reportMaster.RepFirstName = reportMaster.RepFirstName ?? _practiceProviderMapper.Practice.Rep.User2.FirstName;
                                            reportMaster.RepLastName = reportMaster.RepLastName ?? _practiceProviderMapper.Practice.Rep.User2.LastName;
                                        }
                                        else //Use LocationId to find the Provider
                                        {
                                            BuildImportMessage("Practice and Rep not resolved by provider and practice name", isTechnical: false);
                                            if (!string.IsNullOrEmpty(reportMaster.LocationId))
                                            {
                                                var practiceAddressMappers = dbRootContext.PracticeAddressMappers.FirstOrDefault(a => a.Address.LocationId == reportMaster.LocationId && a.Practice.BusinessId == currentBusinessId &&
                                                                                                                                    a.Practice.Leads.Any(b => b.IsConverted == true && b.Accounts.Any(c => c.IsActive == true)));
                                                if (practiceAddressMappers != null)
                                                {
                                                    reportMaster.PracticeId = practiceAddressMappers.PracticeId;
                                                    reportMaster.RepId = practiceAddressMappers.Practice.RepId;
                                                    reportMaster.RepFirstName = reportMaster.RepFirstName ?? practiceAddressMappers.Practice.Rep.User2.FirstName;
                                                    reportMaster.RepLastName = reportMaster.RepLastName ?? practiceAddressMappers.Practice.Rep.User2.LastName;
                                                }
                                                else
                                                {
                                                    BuildImportMessage("Practice and Rep not resolved by Location Id", isTechnical: false);
                                                }
                                            }
                                            else
                                            {
                                                BuildImportMessage("Location Id is null, Practice and Rep not resolved by Location Id", isTechnical: false);
                                            }
                                        }
                                    }
                                }

                                if (reportMaster.ProviderNpi == null && reportMaster.PracticeName != null)
                                {
                                    var practicePortal = dbRootContext.Practices.Where(a => a.PracticeName == reportMaster.PracticeName && a.BusinessId == currentBusinessId &&
                                                                                            a.Leads.Any(b => b.IsConverted == true && b.Accounts.Any(c => c.IsActive == true)));
                                    if (practicePortal.Count() == 1)
                                    {
                                        var _practicePortal = practicePortal.FirstOrDefault();
                                        reportMaster.PracticeId = _practicePortal.Id;

                                        var practiceProviderMapper = _practicePortal.PracticeProviderMappers;

                                        if (practiceProviderMapper.Count() == 1)
                                        {
                                            var _practiceProviderMapper = practiceProviderMapper.FirstOrDefault();
                                            reportMaster.ProviderId = _practiceProviderMapper.ProviderId;

                                            reportMaster.ProviderFirstName = _practiceProviderMapper.Provider.FirstName;
                                            reportMaster.ProviderLastName = _practiceProviderMapper.Provider.LastName;

                                            reportMaster.PracticeName = _practiceProviderMapper.Practice.PracticeName;
                                            reportMaster.RepId = _practiceProviderMapper.Practice.RepId;
                                            reportMaster.RepFirstName = _practiceProviderMapper.Practice.Rep.User2.FirstName;
                                            reportMaster.RepLastName = _practiceProviderMapper.Practice.Rep.User2.LastName;

                                            string nodeName = reportMaster.ObjPracticeName.NodeName;
                                            BuildImportMessage("Practice and Rep resolved by Practice Name", "PracticeName", nodeName, reportMaster.ObjPracticeName.Value, isTechnical: false);
                                        }
                                        else
                                        {
                                            BuildImportMessage("Practice and Rep not resolved by Practice Name", isTechnical: false);
                                        }
                                    }
                                    else
                                    {
                                        BuildImportMessage("Practice and Rep not resolved by Practice Name", isTechnical: false);
                                    }
                                }
                                reportMaster.PracticeName = reportMaster.PracticeName ?? tempPracticeName;

                                #endregion

                                #region Get data from excel based on dynamic columns

                                var reportColumns = dbRootContext.ReportColumns.Where(a => a.BusinessId == currentBusinessId.Value && a.ServiceId == reportMaster.ServiceId && a.ColumnType == (int)SalesColumnType.ReportMaster).ToList();
                                List<Database.ReportColumnValue> reportColumnValues = new List<Database.ReportColumnValue>();
                                if (reportColumns != null && reportColumns.Count > 0)
                                {
                                    foreach (var column in reportColumns)
                                    {
                                        var emptyXelement = new XElement("EmptyElement");
                                        string columnValue = xmlHelper.GetNodeValue(ref emptyXelement, column.ColumnName).Value;
                                        columnValue = !string.IsNullOrEmpty(columnValue) ? columnValue.Replace("_x000D_", "") : null;

                                        reportColumnValues.Add(new Database.ReportColumnValue
                                        {
                                            ColumnId = column.Id,
                                            Value = columnValue,
                                            CreatedOn = DateTime.UtcNow,
                                            CreatedBy = currentUserId
                                        });
                                    }
                                }

                                #endregion

                                reportMaster.BusinessId = currentBusinessId.Value;
                                reportMaster.ReportColumnValues = reportColumnValues;
                                reportMaster.CreatedOn = DateTime.UtcNow;
                                reportMaster.CreatedBy = currentUserId;
                                string performedOperation = string.Empty;

                                #region Report Master Entries

                                using (var dbEntity = new EBP.Business.Database.CareConnectCrmEntities())
                                {
                                    Database.ReportMaster objReportMaster = null;
                                    objReportMaster = dbEntity.ReportMasters.FirstOrDefault(a => a.BusinessId == currentBusinessId.Value && a.ServiceId == reportMaster.ServiceId && a.PatientId == reportMaster.PatientId && a.SpecimenId == reportMaster.SpecimenId);

                                    if (objReportMaster != null)
                                    {
                                        reportMasterId = reportMaster.Id = objReportMaster.Id;

                                        if ((objReportMaster.ReportedDate == null && reportMaster.ReportedDate != null) || (reportMaster.ReportedDate != null && objReportMaster.ReportedDate < reportMaster.ReportedDate))
                                        {
                                            if (!reportMaster.HasPatientFirstNameColumn)
                                                reportMaster.PatientFirstName = objReportMaster.PatientFirstName;
                                            if (!reportMaster.HasPatientLastNameColumn)
                                                reportMaster.PatientLastName = objReportMaster.PatientLastName;
                                            if (!reportMaster.HasSpecimenCollectionDateColumn)
                                                reportMaster.SpecimenCollectionDate = objReportMaster.SpecimenCollectionDate;
                                            if (!reportMaster.HasSpecimenReceivedDateColumn)
                                                reportMaster.SpecimenReceivedDate = objReportMaster.SpecimenReceivedDate;
                                            if (!reportMaster.HasReportedDateColumn)
                                                reportMaster.ReportedDate = objReportMaster.ReportedDate;
                                            if (!reportMaster.HasPracticeNameColumn)
                                                reportMaster.PracticeName = objReportMaster.PracticeName;
                                            if (!reportMaster.HasProviderFirstNameColumn)
                                                reportMaster.ProviderFirstName = objReportMaster.ProviderFirstName;
                                            if (!reportMaster.HasProviderLastNameColumn)
                                                reportMaster.ProviderLastName = objReportMaster.ProviderLastName;
                                            if (!reportMaster.HasProviderNpiColumn)
                                                reportMaster.ProviderNpi = objReportMaster.ProviderNpi;
                                            if (!reportMaster.HasRepFirstNameColumn)
                                                reportMaster.RepFirstName = objReportMaster.RepFirstName;
                                            if (!reportMaster.HasRepLastNameColumn)
                                                reportMaster.RepLastName = objReportMaster.RepLastName;
                                            if (!reportMaster.HasPatientIdColumn)
                                                reportMaster.PatientId = objReportMaster.PatientId;
                                            if (!reportMaster.HasLocationIdColumn)
                                                reportMaster.LocationId = objReportMaster.LocationId;
                                            if (!reportMaster.HasSpecimenIdColumn)
                                                reportMaster.SpecimenId = objReportMaster.SpecimenId;

                                            reportMaster.CreatedOn = objReportMaster.CreatedOn;
                                            reportMaster.CreatedBy = objReportMaster.CreatedBy;
                                            reportMaster.UpdatedOn = DateTime.UtcNow;
                                            reportMaster.UpdatedBy = currentUserId;

                                            var objReportModel = dbRootContext.ReportMasters.Add(reportMaster);
                                            dbRootContext.Entry(objReportModel).State = EntityState.Modified;

                                            foreach (var item in objReportModel.ReportColumnValues)
                                            {
                                                var objReportColumnValue = objReportMaster.ReportColumnValues.FirstOrDefault(a => a.ColumnId == item.ColumnId);
                                                if (objReportColumnValue != null)
                                                {
                                                    item.Id = objReportColumnValue.Id;
                                                    dbRootContext.Entry(item).State = EntityState.Modified;
                                                }
                                            }

                                            updateCount++;
                                            performedOperation = "updated";
                                            if ((reportMaster.PracticeId.HasValue && reportMaster.PracticeId.Value > 0) && (reportMaster.ProviderId.HasValue && reportMaster.ProviderId.Value > 0))
                                            {
                                                importStatus = (int)SalesImportStatus.Updated;
                                            }
                                            else
                                            {
                                                importStatus = (int)SalesImportStatus.NotResolved;
                                            }
                                        }
                                        else
                                        {
                                            importStatus = (int)SalesImportStatus.Duplicated;
                                        }
                                    }
                                    else
                                    {
                                        insertCount++;
                                        performedOperation = "inserted";
                                        if ((reportMaster.PracticeId.HasValue && reportMaster.PracticeId.Value > 0) && (reportMaster.ProviderId.HasValue && reportMaster.ProviderId.Value > 0))
                                        {
                                            importStatus = (int)SalesImportStatus.Inserted;
                                        }
                                        else
                                        {
                                            importStatus = (int)SalesImportStatus.NotResolved;
                                        }
                                        dbRootContext.ReportMasters.Add(reportMaster);
                                    }
                                }

                                if (dbRootContext.SaveChanges() > 0)
                                {
                                    rowsAffected++;
                                    string successMessage = string.Format("Record successfully {0}", performedOperation);
                                    BuildImportMessage(successMessage, isTechnical: false);
                                    logString.AppendLine(successMessage);
                                    logMessages.Insert(0, successMessage);
                                    reportMasterId = reportMaster.Id;
                                }
                                else
                                {
                                    if (importStatus != (int)SalesImportStatus.Duplicated)
                                        importStatus = (int)SalesImportStatus.Skiped;
                                    skippedCount++;
                                    string message = "Report collection record skipped";
                                    logString.AppendLine(message);
                                    BuildImportMessage(message, isTechnical: false);
                                    logMessages.Insert(0, message);
                                }

                                #endregion

                                #endregion
                            }

                            if (reportFinance != null && (!string.IsNullOrEmpty(reportFinance.ReportKey) || (reportMaster != null && !string.IsNullOrEmpty(reportMaster.SpecimenId))))
                            {
                                if (reportMaster == null)
                                    reportMaster = new Database.ReportMaster();

                                if (hasMultipleFinanceData && reportMasterId <= 0)
                                {
                                    var objReportmaster = dbRootContext.ReportMasters.FirstOrDefault(a => a.BusinessId == currentBusinessId.Value && a.ServiceId == serviceId && a.SpecimenId == reportFinance.ReportKey);
                                    if (objReportmaster != null)
                                        reportMasterId = objReportmaster.Id;
                                }

                                if (reportMaster.ColumnsMissingMessages.Count > 0)
                                    reportMaster.ColumnsMissingMessages = reportMaster.ColumnsMissingMessages.Concat(reportFinance.ColumnsMissingMessages).ToList();

                                #region Get data from excel based on dynamic columns

                                var reportColumns = dbRootContext.ReportColumns.Where(a => a.BusinessId == currentBusinessId.Value && a.ServiceId == serviceId && a.ColumnType == (int)SalesColumnType.ReportFinance).ToList();
                                List<Database.ReportColumnValue> reportColumnValues = new List<Database.ReportColumnValue>();
                                if (reportColumns != null && reportColumns.Count > 0)
                                {
                                    foreach (var column in reportColumns)
                                    {
                                        var emptyXelement = new XElement("EmptyElement");
                                        string columnValue = xmlHelper.GetNodeValue(ref emptyXelement, column.ColumnName).Value;
                                        columnValue = !string.IsNullOrEmpty(columnValue) ? columnValue.Replace("_x000D_", "") : null;

                                        reportColumnValues.Add(new Database.ReportColumnValue
                                        {
                                            ReportId = reportMasterId,
                                            ColumnId = column.Id,
                                            Value = columnValue,
                                            CreatedOn = DateTime.UtcNow,
                                            CreatedBy = currentUserId
                                        });
                                    }
                                }

                                reportFinance.ReportColumnValues = reportColumnValues;

                                #endregion

                                #region ReportFinance Entries

                                using (var dbEntity = new EBP.Business.Database.CareConnectCrmEntities())
                                {
                                    reportFinance.BusinessId = currentBusinessId.Value;
                                    reportFinance.ServiceId = serviceId;
                                    string performedOperation = string.Empty;
                                    int savedRecordCount = 0;

                                    string reportKey = reportFinance.ReportKey ?? reportMaster.SpecimenId;
                                    var objReportFinance = dbEntity.ReportFinances.FirstOrDefault(a => !hasMultipleFinanceData && a.ReportKey == reportKey && a.ServiceId == serviceId && a.BusinessId == currentBusinessId);

                                    if (objReportFinance != null)
                                    {
                                        if (reportFinance.HasAdjustAmountColumn)
                                            objReportFinance.AdjustAmount = reportFinance.AdjustAmount;
                                        if (reportFinance.HasAdjustReasonColumn)
                                            objReportFinance.AdjustReason = reportFinance.AdjustReason;
                                        if (reportFinance.HasBilledDateColumn)
                                            objReportFinance.BilledDate = reportFinance.BilledDate;
                                        if (reportFinance.HasChargesColumn)
                                            objReportFinance.Charges = reportFinance.Charges;
                                        if (reportFinance.HasPaidAmountColumn)
                                            objReportFinance.PaidAmount = reportFinance.PaidAmount;
                                        if (reportFinance.HasPaidDateColumn)
                                            objReportFinance.PaidDate = reportFinance.PaidDate;

                                        objReportFinance.UpdatedOn = DateTime.UtcNow;
                                        objReportFinance.UpdatedBy = currentUserId;

                                        var objReportModel = dbRootContext.ReportMasters.Add(reportMaster);
                                        dbRootContext.Entry(objReportModel).State = EntityState.Modified;

                                        foreach (var item in objReportModel.ReportColumnValues)
                                        {
                                            var objReportColumnValue = objReportFinance.ReportColumnValues.FirstOrDefault(a => a.ColumnId == item.ColumnId);
                                            if (objReportColumnValue != null)
                                            {
                                                item.Id = objReportColumnValue.Id;
                                                dbRootContext.Entry(item).State = EntityState.Modified;
                                            }
                                        }

                                        savedRecordCount = dbEntity.SaveChanges();
                                        performedOperation = "updated";
                                        if (isFinanceData)
                                        {
                                            importStatus = (int)SalesImportStatus.Updated;
                                            updateCount++;
                                        }
                                    }
                                    else if (reportMasterId > 0)
                                    {
                                        reportFinance.ReportKey = reportFinance.ReportKey ?? reportKey;
                                        reportFinance.ReportId = reportMasterId;
                                        reportFinance.CreatedOn = DateTime.UtcNow;
                                        reportFinance.CreatedBy = currentUserId;
                                        dbRootContext.ReportFinances.Add(reportFinance);

                                        savedRecordCount = dbRootContext.SaveChanges();
                                        performedOperation = "inserted";
                                        if (isFinanceData)
                                        {
                                            importStatus = (int)SalesImportStatus.Inserted;
                                            insertCount++;
                                        }
                                    }

                                    if (savedRecordCount > 0)
                                    {
                                        if (isFinanceData)
                                            rowsAffected++;
                                        string successMessage = string.Format("Finance data successfully {0}", performedOperation);
                                        BuildImportMessage(successMessage, isTechnical: false);
                                        logString.AppendLine(successMessage);
                                        logMessages.Insert(logMessages.Count > 1 ? 1 : 0, successMessage);
                                    }
                                    else
                                    {
                                        if (isFinanceData)
                                        {
                                            importStatus = (int)SalesImportStatus.Skiped;
                                            skippedCount++;
                                        }
                                        string message = "Report finance record skipped";
                                        logString.AppendLine(message);
                                        BuildImportMessage(message, isTechnical: false);
                                        logMessages.Insert(0, message);
                                    }
                                }

                                #endregion

                            }

                            if (reportMaster == null)
                                reportMaster = new Database.ReportMaster();

                            reportMaster.LogMessages = logMessages;
                            salesSummery.MessageSummery = string.Join(" | ", reportMaster.ReportLogMessages);
                            salesSummery.SalesImportDetails.Add(new Database.SalesImportDetail
                            {
                                ImportedData = xmlHelper.GetFirstChildAsJson(),
                                ReportId = reportMasterId > 0 ? (int?)reportMasterId : null,
                                CreatedOn = DateTime.UtcNow,
                                SalesImportMessages = salesImportMessages.Concat(reportMaster.ColumnsMissingMessages).ToList(),
                                ImportStatus = importStatus,
                                DataType = salesDataType
                            });

                            xmlHelper.RemoveFirstChild(); /* Remove processed node from XmlDocument */
                        }
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                    {
                        failedCount++;
                        ex.Log();

                        var validationErrors = string.Empty;
                        foreach (var eve in ex.EntityValidationErrors)
                        {
                            foreach (var ve in eve.ValidationErrors)
                            {
                                string details = string.Format("Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                                validationErrors = validationErrors + "\n" + details;
                            }
                        }

                        if (!string.IsNullOrEmpty(validationErrors))
                        {
                            logString.AppendLine(string.Format("Validation Error\n{0}", validationErrors));
                        }
                    }
                    catch (Exception ex)
                    {
                        failedCount++;
                        ex.Log();
                        string message = string.Format("{0} {1}", ex.Message, ex.InnerException != null ? ex.InnerException.Message : "");
                        logString.AppendLine(message);
                    }
                    logWriter.WriteLine(logString);
                }

                #endregion

                continueToLog:

                logWriter.WriteLine(string.Format("Number of rows Inserted\t:\t{0}\t\nNumber of rows Updated \t:\t{1}\t\nNumber of rows skipped \t:\t{2}\t\nNumber of rows failed \t:\t{3}", insertCount, updateCount, skippedCount, failedCount));
                response.Status = DataResponseStatus.OK;

                try
                {
                    using (var dbContext = new EBP.Business.Database.CareConnectCrmEntities())
                    {
                        salesSummery.NumberOfInserts = insertCount;
                        salesSummery.NumberOfUpdates = updateCount;
                        salesSummery.NumberOfFails = failedCount;
                        salesSummery.NumberOfSkips = skippedCount;
                        salesSummery.RowsAffected = rowsAffected;

                        var objSalesSummery = dbContext.SalesImportSummeries.Add(salesSummery);
                        dbContext.SaveChanges();
                        importSummeryId = objSalesSummery.Id;
                    }
                }
                catch (Exception ex)
                {
                    ex.Log();
                }
            }
            catch (Exception ex)
            {
                ex.Log();
                response.Status = DataResponseStatus.InternalServerError;
            }

            return response;
        }

        public DataResponse Insert(List<EntitySales> model, int? currentBusinessId, int currentUserId, int serviceId, string mapperFilePath, int PracticeId, bool displayPatient, System.IO.StreamWriter logWriter = null)
        {
            logWriter.WriteLine("Insert - line 1703");
            var response = new DataResponse();
            StringBuilder logString = new StringBuilder();
            int rowsAffected = 0,
                failedCount = 0,
                skippedCount = 0,
                insertCount = 0,
                updateCount = 0;

            try
            {
                string tempPracticeName = string.Empty;
                var reportMasterList = new List<Database.ReportMaster>();
                int reportMasterId = 0;
                XDocument XmlMapper = XDocument.Load(mapperFilePath);
                var hasFinanceData = XmlMapper.Descendants("ReportFinance").Count();
                //int? importStatus = null;

                List<string> logMessages = new List<string>();

                Database.SalesImportSummery salesSummery = new Database.SalesImportSummery
                {
                    BusinessId = currentBusinessId.Value,
                    ServiceId = serviceId,
                    UserId = currentUserId,
                    ImportedDate = DateTime.Now,
                    LogFilePath = ((FileStream)(logWriter.BaseStream)).Name,
                    DataMode = model.Count > 1 ? (int)SalesDataMode.ManualBulkEntry : (int)SalesDataMode.ManualSingleEntry,
                    TotalRecords = model.Count,
                };

                foreach (var item in model)
                {
                    salesImportMessages = new List<Database.SalesImportMessage>();

                    logString.Clear();
                    try
                    {
                        using (var dbRootContext = new EBP.Business.Database.CareConnectCrmEntities())
                        {
                            var reportMaster = new Database.ReportMaster
                            {
                                Id = item.ReportId,
                                PatientFirstName = item.PatientFirstName,
                                PatientLastName = item.PatientLastName,
                                SpecimenCollectionDate = item.CollectionDate,
                                SpecimenReceivedDate = item.ReceivedDate,
                                ReportedDate = item.ReportedDate,
                                PracticeName = item.Practice,
                                ReportColumnValues = item.ReportColumnValues == null ? null : item.ReportColumnValues.Select(b => new Database.ReportColumnValue
                                {
                                    ColumnId = b.ColumnId,
                                    Value = b.Value,
                                    ReportId = item.ReportId,
                                    CreatedOn = DateTime.UtcNow,
                                    CreatedBy = currentUserId
                                }).ToList(),
                                PatientId = item.PatientId.HasValue ? item.PatientId.Value : 0,
                                SpecimenId = item.SpecimenId,
                                PracticeId = item.PracticeId,
                                ProviderId = item.ProviderId,
                                ServiceId = serviceId,
                                RepId = item.RepId
                            };

                            var reportFinance = new Database.ReportFinance
                            {
                                Id = item.ReportFinanceId,
                                BilledDate = item.BilledDate,
                                PaidDate = item.PaidDate,
                                Charges = item.Charges,
                                PaidAmount = item.PaidAmount,
                                ReportKey = item.SpecimenId,
                                AdjustAmount = item.AdjustAmount,
                                AdjustReason = item.AdjustReason,
                                BusinessId = currentBusinessId.Value,
                                ServiceId = serviceId
                            };

                            string _providerNpi = reportMaster.ProviderNpi;

                            if (reportMaster != null && !string.IsNullOrEmpty(reportMaster.SpecimenId))
                            {
                                #region ReportMaster and ReportColumnValues Entries

                                reportMaster.ServiceId = serviceId;
                                tempPracticeName = reportMaster.PracticeName;

                                logString.AppendLine(string.Format("Patient Id\t:\t{0}\tPatient Name\t:\t{1}\tPractice Name\t:\t{2}",
                                    reportMaster.PatientId,
                                    reportMaster.PatientFirstName + " " + reportMaster.PatientLastName,
                                    reportMaster.PracticeName));

                                reportMaster.BusinessId = currentBusinessId.Value;
                                reportMaster.CreatedOn = DateTime.UtcNow;
                                reportMaster.CreatedBy = currentUserId;
                                string performedOperation = string.Empty;

                                #region Report Master Entries

                                using (var dbEntity = new EBP.Business.Database.CareConnectCrmEntities())
                                {
                                    Database.ReportMaster objReportMaster = null;
                                    objReportMaster = dbEntity.ReportMasters.FirstOrDefault(a => a.Id == reportMaster.Id || (a.ServiceId == reportMaster.ServiceId && a.PatientId == reportMaster.PatientId && a.SpecimenId == reportMaster.SpecimenId));

                                    if (objReportMaster != null)
                                    {
                                        reportMaster.UpdatedOn = DateTime.UtcNow;
                                        reportMaster.UpdatedBy = currentUserId;

                                        #region Block : Note

                                        if (!displayPatient)
                                        {
                                            reportMaster.PatientId = objReportMaster.PatientId;
                                            reportMaster.PatientFirstName = objReportMaster.PatientFirstName;
                                            reportMaster.PatientLastName = objReportMaster.PatientLastName;
                                        }

                                        reportMaster.ProviderFirstName = objReportMaster.ProviderFirstName;
                                        reportMaster.ProviderLastName = objReportMaster.ProviderLastName;
                                        reportMaster.ProviderNpi = objReportMaster.ProviderNpi;
                                        reportMaster.RepFirstName = objReportMaster.RepFirstName;
                                        reportMaster.RepLastName = objReportMaster.RepLastName;
                                        reportMaster.IsColumnValuesImported = objReportMaster.IsColumnValuesImported;

                                        #endregion

                                        var objReportModel = dbRootContext.ReportMasters.Add(reportMaster);
                                        dbRootContext.Entry(objReportModel).State = EntityState.Modified;

                                        if (objReportModel.ReportColumnValues != null)
                                            foreach (var reportColumn in objReportModel.ReportColumnValues)
                                            {
                                                var objReportColumnValue = objReportMaster.ReportColumnValues.FirstOrDefault(a => a.ColumnId == reportColumn.ColumnId);
                                                if (objReportColumnValue != null)
                                                {
                                                    reportColumn.Id = objReportColumnValue.Id;
                                                    dbRootContext.Entry(reportColumn).State = EntityState.Modified;
                                                }
                                            }

                                        updateCount++;
                                        performedOperation = "updated";
                                    }
                                    else
                                    {
                                        insertCount++;
                                        performedOperation = "inserted";
                                        dbRootContext.ReportMasters.Add(reportMaster);
                                    }
                                }

                                if (dbRootContext.SaveChanges() > 0)
                                {
                                    rowsAffected++;
                                    string successMessage = string.Format("Record successfully {0}", performedOperation);
                                    BuildImportMessage(successMessage, isTechnical: false);
                                    logString.AppendLine(successMessage);
                                    logMessages.Insert(0, successMessage);
                                    reportMasterId = reportMaster.Id;
                                }
                                else
                                {
                                    skippedCount++;
                                    string message = "Report collection record skipped";
                                    logString.AppendLine(message);
                                    BuildImportMessage(message, isTechnical: false);
                                    logMessages.Insert(0, message);
                                }

                                #endregion

                                #endregion
                            }

                            reportMaster.ColumnsMissingMessages = reportMaster.ColumnsMissingMessages.Concat(reportFinance.ColumnsMissingMessages).ToList();

                            #region ReportFinance Entries

                            using (var dbRootEntity = new EBP.Business.Database.CareConnectCrmEntities())
                            {
                                string performedOperation = string.Empty;
                                int savedRecordCount = 0;

                                using (var dbEntity = new EBP.Business.Database.CareConnectCrmEntities())
                                {
                                    int i = 0;
                                    do
                                    {
                                        if (item.FinanceDataList != null && item.FinanceDataList.Count() > 0)
                                        {
                                            var objFinanceData = item.FinanceDataList.ElementAt(i);
                                            reportFinance = new Database.ReportFinance
                                            {
                                                Id = objFinanceData.ReportFinanceId,
                                                BilledDate = objFinanceData.BilledDate,
                                                PaidDate = objFinanceData.PaidDate,
                                                Charges = objFinanceData.Charges,
                                                PaidAmount = objFinanceData.PaidAmount,
                                                ReportKey = item.SpecimenId,
                                                AdjustAmount = objFinanceData.AdjustAmount,
                                                AdjustReason = objFinanceData.AdjustReason,
                                                BusinessId = currentBusinessId.Value,
                                                ServiceId = serviceId,
                                                ReportColumnValues = objFinanceData.FinanceColumnValues == null ? null : objFinanceData.FinanceColumnValues.Select(b => new Database.ReportColumnValue
                                                {
                                                    ColumnId = b.ColumnId,
                                                    Value = b.Value,
                                                    ReportId = reportMasterId,
                                                    CreatedOn = DateTime.UtcNow,
                                                    CreatedBy = currentUserId
                                                }).ToList(),
                                            };
                                        }

                                        var query = dbRootEntity.ReportFinances.Where(a => 1 == 1);
                                        if (reportFinance.Id > 0)
                                            query = query.Where(a => a.Id == reportFinance.Id);
                                        else
                                            query = query.Where(a => a.ReportId == reportMasterId && a.ReportKey == reportFinance.ReportKey && a.ServiceId == serviceId && a.BusinessId == currentBusinessId);
                                        var objReportFinance = query.FirstOrDefault();

                                        if (objReportFinance != null && reportFinance.Id > 0)
                                        {
                                            reportFinance.Id = reportFinance.Id;
                                            reportFinance.ReportId = objReportFinance.ReportId;
                                            reportFinance.CreatedOn = objReportFinance.CreatedOn;
                                            reportFinance.CreatedBy = objReportFinance.CreatedBy;
                                            reportFinance.UpdatedOn = DateTime.UtcNow;
                                            reportFinance.UpdatedBy = currentUserId;

                                            var objReportFinanceModel = dbEntity.ReportFinances.Add(reportFinance);
                                            dbEntity.Entry(objReportFinanceModel).State = EntityState.Modified;

                                            foreach (var reportColumn in objReportFinanceModel.ReportColumnValues)
                                            {
                                                var objReportColumnValue = objReportFinance.ReportColumnValues.FirstOrDefault(a => a.ColumnId == reportColumn.ColumnId);
                                                if (objReportColumnValue != null)
                                                {
                                                    reportColumn.Id = objReportColumnValue.Id;
                                                    dbEntity.Entry(reportColumn).State = EntityState.Modified;
                                                }
                                            }

                                            performedOperation = "updated";
                                        }
                                        else if (reportMasterId > 0 && hasFinanceData > 0)
                                        {
                                            reportFinance.ReportKey = reportFinance.ReportKey ?? reportFinance.ReportKey;
                                            reportFinance.ReportId = reportMasterId;
                                            reportFinance.CreatedOn = DateTime.UtcNow;
                                            reportFinance.CreatedBy = currentUserId;
                                            dbEntity.ReportFinances.Add(reportFinance);

                                            performedOperation = "inserted";
                                        }

                                        i++;
                                    } while (item.FinanceDataList != null && item.FinanceDataList.Count() > 0 && i < item.FinanceDataList.Count());

                                    savedRecordCount = dbEntity.SaveChanges();
                                }

                                if (savedRecordCount > 0)
                                {
                                    string successMessage = string.Format("Finance data successfully {0}", performedOperation);
                                    BuildImportMessage(successMessage, isTechnical: false);
                                    logString.AppendLine(successMessage);
                                    logMessages.Insert(1, successMessage);
                                }
                                else
                                {
                                    string message = "Report finance record skipped";
                                    logString.AppendLine(message);
                                    BuildImportMessage(message, isTechnical: false);
                                    logMessages.Insert(0, message);
                                }
                            }

                            #endregion

                            reportMaster.LogMessages = logMessages;
                            salesSummery.SalesImportDetails.Add(new Database.SalesImportDetail
                            {
                                ImportedData = JsonConvert.SerializeObject(item),
                                ReportId = reportMasterId > 0 ? (int?)reportMasterId : null,
                                CreatedOn = DateTime.UtcNow,
                                SalesImportMessages = salesImportMessages.Concat(reportMaster.ColumnsMissingMessages).ToList(),

                            });
                        }
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                    {
                        failedCount++;
                        ex.Log();

                        var validationErrors = string.Empty;
                        foreach (var eve in ex.EntityValidationErrors)
                        {
                            foreach (var ve in eve.ValidationErrors)
                            {
                                string details = string.Format("Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                                validationErrors = validationErrors + "\n" + details;
                            }
                        }
                        if (!string.IsNullOrEmpty(validationErrors))
                        {
                            logString.AppendLine(string.Format("Validation Error\n{0}", validationErrors));
                        }
                    }
                    catch (Exception ex)
                    {
                        failedCount++;
                        ex.Log();
                        string message = string.Format("{0} {1}", ex.Message, ex.InnerException != null ? ex.InnerException.Message : "");
                        logString.AppendLine(message);
                    }
                    logWriter.WriteLine(logString);
                }

                logWriter.WriteLine(string.Format("Number of rows Inserted \t:\t{0}\t\nNumber of rows Inserted \t:\t{1}\t\nNumber of rows skiped \t:\t{2}\t\nNumber of rows failed \t:\t{3}", insertCount, updateCount, skippedCount, failedCount));
                response.Status = DataResponseStatus.OK;

                try
                {
                    using (var dbContext = new EBP.Business.Database.CareConnectCrmEntities())
                    {
                        salesSummery.NumberOfInserts = insertCount;
                        salesSummery.NumberOfUpdates = updateCount;
                        salesSummery.NumberOfFails = failedCount;
                        salesSummery.NumberOfSkips = skippedCount;
                        salesSummery.RowsAffected = rowsAffected;

                        var objSalesSummery = dbContext.SalesImportSummeries.Add(salesSummery);
                        dbContext.SaveChanges();
                        //ImportSummeryId = objSalesSummery.Id;
                    }
                }
                catch (Exception ex)
                {
                    ex.Log();
                }
            }
            catch (Exception ex)
            {
                ex.Log();
                response.Status = DataResponseStatus.InternalServerError;
            }

            return response;
        }

        public FilterSales GetDynamicFilters(int? currentBusinessId, int currentUserId, int serviceId, bool isBuzAdmin, string[] currentRoles, string[] currentDepartments, string[] currentPrivileges, bool isRepOrManager, bool displayPatientName)
        {
            FilterSales filter = new FilterSales();
            filter.PageSize = 25;
            try
            {
                DBInit();

                List<DynamicColumns> statics = ReportStaticColumns;

                //user visible columns list
                var userVisibleColumns = DBEntity.UserColumnVisibilities.Where(a => a.UserId == currentUserId && a.ModuleId == (int)UserColumnModules.Sales && a.ServiceId == serviceId)
                  .Select(a => new DynamicColumns
                  {
                      ColumnName = a.ColumnName,
                      DisplayName = a.DisplayName ?? a.ColumnName,
                      Id = a.Id,
                      IsVisible = a.IsVisible,
                      ShowInFilter = a.BusinessMaster.ReportColumns.Any(b => b.ColumnName == a.ColumnName && b.DisplayInFilter == true && b.BusinessId == currentBusinessId)
                  }).ToList();

                var dynamicColumnsList = getReportColumns(isBuzAdmin, currentBusinessId, currentUserId, serviceId, currentRoles, currentDepartments, currentPrivileges);

                foreach (var item in userVisibleColumns)
                {
                    var _static = statics.FirstOrDefault(a => a.ColumnName == item.ColumnName);
                    if (_static != null)
                    {
                        item.OrderIndex = _static.OrderIndex;
                        item.Id = _static.Id;
                        statics.Remove(_static);
                        item.EntityState = EntityState.Modified;
                    }
                    else
                    {
                        var _dynamic = dynamicColumnsList.FirstOrDefault(a => a.ColumnName == item.ColumnName);
                        if (_dynamic != null)
                        {
                            item.OrderIndex = _dynamic.OrderIndex;
                            item.Id = _dynamic.Id;
                            item.EntityState = EntityState.Modified;
                            dynamicColumnsList.Remove(_dynamic);
                        }
                        else
                        {
                            item.EntityState = EntityState.Deleted;
                        }
                    }
                }

                if (dynamicColumnsList.Count() > 0)
                {
                    statics = statics.Concat(dynamicColumnsList).ToList();
                }

                foreach (var item in statics)
                {
                    if (!userVisibleColumns.Any(a => a.ColumnName == item.ColumnName))
                    {
                        item.EntityState = EntityState.Added;
                    }
                }

                filter.DynamicFilters = statics.Concat(userVisibleColumns).Where(a => a.EntityState != EntityState.Deleted).OrderBy(o => o.OrderIndex).ToList();

                if (!displayPatientName)
                    filter.DynamicFilters.RemoveAll(a => a.ColumnName == "Patient");
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                DBClose();
            }
            return filter;
        }

        public List<DynamicColumns> getReportColumns(bool isBuzAdmin, int? currentBusinessId, int currentUserId, int serviceId, string[] currentRoles, string[] currentDepartments, string[] currentPrivileges)
        {
            //dynamic reportcolumns
            var dynamicColumnsList = DBEntity.ReportColumns.Where(a => a.BusinessId == currentBusinessId.Value && a.ServiceId == serviceId //&& a.ColumnType == (int)SalesColumnType.ReportMaster 
                & (isBuzAdmin || a.SalesDepartmentPrivileges.Any(dp => currentDepartments.Contains(dp.Department.DepartmentName))
                || a.SalesRolePrivileges.Any(rp => currentRoles.Contains(rp.Role.Name))
                || a.SalesUserPrivileges.Any(up => up.UserId == currentUserId))
                ).Select(a => new DynamicColumns
                {
                    OrderIndex = a.Id + 1000,
                    ColumnName = a.ColumnName,
                    DisplayName = a.DisplayName ?? a.ColumnName,
                    Id = a.Id,
                    IsVisible = false,
                    ShowInFilter = a.DisplayInFilter,
                    ColumnType = a.ColumnType
                })
                .OrderByDescending(o => o.ColumnType)
                .ToList();

            return dynamicColumnsList;
        }

        public DataResponse<EntitySales> GetReport(int id, int currentUserId, bool isBuzAdmin, string[] currentRoles, string[] currentDepartments, string[] currentPrivileges, bool isRepOrManager, bool displayPatientData)
        {
            var response = new DataResponse<EntitySales>();
            try
            {
                base.DBInit();

                var query = DBEntity.ReportMasters.Where(a => a.Id == id);

                var selectQuery = query.Select(a => new EntitySales
                {
                    ReportId = a.Id,
                    ReportedDate = a.ReportedDate,
                    WrittenOn = a.CreatedOn,
                    CollectionDate = a.SpecimenCollectionDate,
                    ReceivedDate = a.SpecimenReceivedDate,
                    ServiceName = a.LookupEnrolledService.ServiceName,
                    PatientFirstName = !displayPatientData ? string.Empty : a.PatientFirstName,
                    PatientLastName = !displayPatientData ? string.Empty : a.PatientLastName,
                    PatientId = !displayPatientData ? null : (int?)a.PatientId,
                    RepGroup = DBEntity.Reps.Where(b => b.Id == a.RepId).FirstOrDefault().RepGroup.RepGroupName,
                    IsRep = isRepOrManager,
                    RepFirstName = a.Rep.User2.FirstName,
                    RepLastName = a.Rep.User2.LastName,
                    ProviderFirstName = a.Provider.FirstName,
                    ProviderMiddleName = a.Provider.MiddleName,
                    ProviderLastName = a.Provider.LastName,
                    Practice = a.Practice.PracticeName,
                    NPI = a.Provider.NPI,
                    SpecimenId = a.SpecimenId,
                    ReportColumnValues = a.ReportColumnValues.Where(b => b.ReportId == id && b.ReportColumn.ColumnType == (int)SalesColumnType.ReportMaster &&
                        (isBuzAdmin ||
                            b.ReportColumn.SalesUserPrivileges.Any(c => c.UserId == currentUserId) ||
                            b.ReportColumn.SalesRolePrivileges.Any(c => currentRoles.Any(d => d == c.Role.Name))) ||
                            b.ReportColumn.SalesDepartmentPrivileges.Any(c => currentDepartments.Any(d => d == c.Department.DepartmentName)
                        ))
                        .Select(b => new ReportColumnValue
                        {
                            ColumnId = b.ReportColumn.Id,
                            ColumnName = b.ReportColumn.ColumnName,
                            DisplayName = b.ReportColumn.DisplayName,
                            Value = b.Value,
                        }).OrderBy(o => o.ColumnName),
                    FinanceDataList = a.ReportFinances.Where(b => b.DeleteStatus != true).Select(b => new FinanceData
                    {
                        ReportFinanceId = b.Id,
                        BilledDate = b.BilledDate,
                        PaidDate = b.PaidDate,
                        Charges = b.Charges,
                        PaidAmount = b.PaidAmount,
                        AdjustAmount = b.AdjustAmount,
                        AdjustReason = b.AdjustReason,
                        FinanceColumnValues = DBEntity.ReportColumnValues.Where(d => d.FinanceId == b.Id
                            & (isBuzAdmin || d.ReportColumn.SalesDepartmentPrivileges.Any(dp => currentDepartments.Contains(dp.Department.DepartmentName))
                            || d.ReportColumn.SalesRolePrivileges.Any(rp => currentRoles.Contains(rp.Role.Name))
                            || d.ReportColumn.SalesUserPrivileges.Any(up => up.UserId == currentUserId))
                         ).Select(c => new ReportColumnValue
                         {
                             ColumnId = c.ReportColumn.Id,
                             ColumnName = c.ReportColumn.ColumnName,
                             DisplayName = c.ReportColumn.DisplayName,
                             Value = c.Value
                         })
                    })
                });

                response = GetFirst<EntitySales>(selectQuery);

                if (response.Model.FinanceDataList != null)
                    response.Model.FinanceData = response.Model.FinanceDataList.FirstOrDefault();
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }
            return response;
        }

        public DataResponse<EntityList<EntitySelectItem>> GetAllRxNumbersByServiceId(int serviceId, int? currentBusinessId)
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            try
            {
                base.DBInit();

                var query = DBEntity.ReportMasters.Where(a => a.ServiceId == serviceId && a.BusinessId == currentBusinessId)
                    .Select(s => new EntitySelectItem
                    {
                        Id = s.Id,
                        Value = s.Id.ToString()
                    }).OrderBy(o => o.Value);

                response = GetList<EntitySelectItem>(query, 0, 100);
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }
            return response;
        }

        public int GetServiceId(string serviceName, int currentBusinessId)
        {
            int serviceId = 0;
            try
            {
                base.DBInit();
                var service = DBEntity.LookupEnrolledServices.FirstOrDefault(a => a.BusinessId == currentBusinessId && a.ServiceName.Replace(" ", "").Trim().ToLower() == serviceName.Trim().ToLower());
                if (service != null)
                    serviceId = service.Id;
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }
            return serviceId;
        }

        public void UpdateImportSummery(int importSummeryId, string importFilePath)
        {
            try
            {
                using (var dbContext = new EBP.Business.Database.CareConnectCrmEntities())
                {
                    var objImportSummery = dbContext.SalesImportSummeries.FirstOrDefault(a => a.Id == importSummeryId);
                    if (objImportSummery != null)
                    {
                        objImportSummery.ImportedFilePath = importFilePath;
                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Log();
            }
        }

        private void BuildImportMessage(string message, string columnName = null, string nodeName = null, string incomingValue = null, bool isTechnical = true, bool isFinance = false)
        {
            salesImportMessages.Add(new Database.SalesImportMessage
            {
                ColumnName = columnName,
                NodeName = nodeName,
                IncomingValue = incomingValue,
                Message = message,
                IsFinance = isFinance,
                IsTechnical = isTechnical
            });
        }

        private bool? ParseToBool(string value)
        {
            bool _return;

            if (!bool.TryParse(value, out _return))
                return null;

            return _return;
        }

    }
}
