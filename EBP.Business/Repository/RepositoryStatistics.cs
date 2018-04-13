using EBP.Business.Entity;
using EBP.Business.Entity.DashBoard;
using EBP.Business.Entity.Note;
using EBP.Business.Enums;
using EBP.Business.Filter;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.SqlServer;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Xml.Linq;
using EBP.Business.Database;

namespace EBP.Business.Repository
{
    public class RepositoryStatistics : _Repository
    {
        public RepositoryStatistics()
        {

            //private static MemoryCache _cache = new MemoryCache("ExampleCache");
        }
        public XDocument XmlMapper { get; set; }

        public List<DynamicColumns> ReportStaticColumnsByAttribute(XName attributeName)
        {
            var model = new List<DynamicColumns>();
            var mapper = XmlMapper.Descendants()
                             .Attributes(attributeName).Where(a => a.Value == "true")
                             .Select(x => x)
                             .ToList();
            foreach (var item in mapper)
            {
                model.Add(new DynamicColumns { ColumnName = item.Parent.Name.LocalName, DisplayName = item.Parent.Attribute("DisplayName").Value });
            }
            return model;
        }

        public DataResponse<EntityStatistics> GetAllStatistics(int businessId, int currentUserId, bool isBuzAdmin, bool isRepOrManager, bool isSalesManager, bool isSalesDirector)
        {
            var response = new DataResponse<EntityStatistics>();
            try
            {
                using (var dbEntity = new Database.CareConnectCrmEntities())
                {


                    var objLeads = dbEntity.Leads.Where(a => a.BusinessId == businessId && a.IsConverted == false && a.IsActive == true);
                    var objAccounts = dbEntity.Accounts.Where(a => a.BusinessId == businessId);//&& a.IsActive == true
                    var objSales = dbEntity.ReportMasters.Where(a => a.BusinessId == businessId && a.LookupEnrolledService.IsActive == true);
                    var objTasks = dbEntity.Tasks.Where(a => a.BusinessId == businessId && a.IsActive == true);

                    if (!isBuzAdmin)
                    {
                        // Note : implemented property isSalesManager
                        //bool isManager = dbEntity.RepGroups.Any(a => a.RepgroupManagerMappers.Any(b => b.ManagerId == currentUserId));//---ManagerChange       bool isManager = dbEntity.RepGroups.Any(a => a.ManagerId == currentUserId);
                        if (isSalesDirector || isSalesManager)
                        {
                            objLeads = objLeads.Where(a => a.Rep.RepGroup.RepgroupManagerMappers.Any(b => b.ManagerId == currentUserId));// || a.Rep.RepGroup.SalesDirectorId == currentUserId)//---ManagerChange       objLeads = objLeads.Where(a => a.Rep.RepGroup.ManagerId == currentUserId);
                            objAccounts = objAccounts.Where(a => a.Lead.Rep.RepGroup.RepgroupManagerMappers.Any(b => b.ManagerId == currentUserId));//|| a.Lead.Rep.RepGroup.SalesDirectorId == currentUserId//---ManagerChange       
                        }
                        else
                        {
                            var objRep = dbEntity.Reps.FirstOrDefault(a => a.UserId == currentUserId);
                            bool isRep = (objRep != null);
                            if (isRep)
                            {
                                objLeads = objLeads.Where(a => a.Practice.RepId == objRep.Id);
                                objAccounts = objAccounts.Where(a => a.Lead.Practice.RepId == objRep.Id);
                            }
                        }
                        if (isRepOrManager || isSalesDirector)
                            objSales = objSales.Where(a => a.Rep.User2.Reps2.Any(b => b.UserId == currentUserId) ||
                                a.Rep.RepGroup.RepgroupManagerMappers.Any(b => b.ManagerId == currentUserId) ||
                                a.Rep.User2.Reps2.Any(b => b.RepGroup.RepgroupManagerMappers.Any(c => c.ManagerId == currentUserId && c.UserRole == (int)RepgroupUserType.Director)) ||
                                a.CreatedBy == currentUserId);
                        //---ManagerChange      objSales = objSales.Where(a => a.Rep.User2.Reps2.Select(b => b.UserId).Contains(currentUserId) || a.Rep.User2.Reps2.Select(b => b.RepGroup.ManagerId).Contains(currentUserId) || a.CreatedBy == currentUserId);

                        var reps = new List<int>();
                        reps = dbEntity.Reps.Where(a => a.RepGroup.RepgroupManagerMappers.Any(b => b.ManagerId == currentUserId)).Select(a => a.UserId).ToList();// || a.RepGroup.SalesDirectorId == currentUserId
                        //---ManagerChange      reps = dbEntity.Reps.Where(a => a.RepGroup.ManagerId == currentUserId).Select(a => a.UserId).ToList();
                        reps.Add(currentUserId);

                        objTasks = objTasks.Where(a => a.TaskUsers.Any(tu => reps.Contains(tu.UserId)) || a.RequestedBy == currentUserId);
                    }

                    int leadsTotalCount = objLeads.Count();
                    int accountsTotalCount = objAccounts.Count();
                    int salesTotalCount = objSales.Count();
                    int tasksTotalCount = objTasks.Count();

                    var inputDate = DateTime.UtcNow.AddDays(30);

                    int leadsLastMonthCount = leadsTotalCount - objLeads.Where(a => a.CreatedOn < inputDate).Count();
                    int accountsLastMonthCount = accountsTotalCount - objAccounts.Where(a => a.CreatedOn < inputDate).Count();
                    int salesLastMonthCount = salesTotalCount - objSales.Where(a => a.CreatedOn < inputDate).Count();
                    int tasksLastMonthCount = tasksTotalCount - objTasks.Where(a => a.CreatedOn < inputDate).Count();

                    var objUserProfile = dbEntity.UserProfiles.FirstOrDefault(a => a.UserId == currentUserId);
                    var notificationCount = objUserProfile != null ? objUserProfile.NotificationCount : 0;

                    response.CreateResponse<EntityStatistics>(new EntityStatistics
                    {
                        LeadsCount = leadsTotalCount,
                        LeadsCountLastMonth = leadsLastMonthCount,
                        AccountsCount = accountsTotalCount,
                        AccountsCountLastMonth = accountsLastMonthCount,
                        SalesCount = salesTotalCount,
                        SalesCountLastMonth = salesLastMonthCount,
                        TasksCount = tasksTotalCount,
                        TasksCountLastMonth = tasksLastMonthCount,
                        NotificationCount = notificationCount
                    }, DataResponseStatus.OK);

                }
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            return response;
        }

        public DataResponse<EntitySalesPerformance> GetAllSalesPerformance(int businessId, int currentUserId, bool isBuzAdmin, bool isSalesDirector, bool isSalesManager, bool isRep, DateTime? DateFrom = null, DateTime? DateTo = null)
        {
            var response = new DataResponse<EntitySalesPerformance>();
            try
            {
                base.DBInit();
                int ThisWeekCount = 0;
                int TotalCount = 0;
                int MonthToDate = 0;

                var salesModel = DBEntity.ReportMasters.Where(a => a.BusinessId == businessId && a.LookupEnrolledService.IsActive == true);

                //if (IsRepOrManager)
                if (!isBuzAdmin)
                {
                    if (isSalesManager || isSalesDirector || isRep)
                        salesModel = salesModel.Where(a => a.Rep.User2.Reps2.Any(b => b.UserId == currentUserId) ||
                            a.Rep.RepGroup.RepgroupManagerMappers.Any(b => b.ManagerId == currentUserId) || //---ManagerChange   a.Rep.User2.Reps2.Select(b => b.RepGroup.ManagerId).Contains(currentUserId) ||
                            a.Rep.User2.Reps2.Any(b => b.RepGroup.RepgroupManagerMappers.Any(c=>c.ManagerId== currentUserId && c.UserRole == (int)RepgroupUserType.Director)) ||
                             a.CreatedBy == currentUserId);
                }

                //if (isSalesDirector)
                //    salesModel = salesModel.Where(a => a.Rep.User2.Reps2.Any(b => b.RepGroup.SalesDirectorId == currentUserId));

                var ThisWeekStartOn = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);// DateTime.UtcNow.AddDays(-7);
                if (salesModel != null)
                {
                    ThisWeekCount = salesModel.Count(a => a.SpecimenReceivedDate >= ThisWeekStartOn);
                    var ThisMonthStartOn = DateTime.UtcNow.AddDays(-(DateTime.UtcNow.Day - 1));
                    MonthToDate = salesModel.Count(a => a.SpecimenReceivedDate >= ThisMonthStartOn);
                }
                //DateFrom = DateTime.UtcNow.AddDays(-60);
                //DateTo = DateTime.UtcNow.AddDays(-21);
                if (DateFrom.HasValue)
                {
                    salesModel = salesModel.Where(a => a.SpecimenReceivedDate >= DateFrom);
                }
                if (DateTo.HasValue)
                {
                    salesModel = salesModel.Where(a => a.SpecimenReceivedDate <= DateTo);
                }
                TotalCount = salesModel.Count();
                var model = salesModel
               .GroupBy(n => n.ServiceId)
               .Select(n => new
               {
                   ServiceId = n.Key,
                   Count = n.Count(),
                   ServiceName = n.FirstOrDefault().LookupEnrolledService.ServiceName,
                   ServiceColor = n.FirstOrDefault().LookupEnrolledService.ServiceColor
               }
               )
               .OrderByDescending(n => n.ServiceId);
                response.CreateResponse<EntitySalesPerformance>(new EntitySalesPerformance
                {
                    MonthToDate = MonthToDate,
                    TotalCount = TotalCount,
                    ThisWeekCount = ThisWeekCount,
                    ServiceNames = model.Select(a => a.ServiceName).ToArray(),
                    ServiceColor = model.Select(a => a.ServiceColor).ToArray(),
                    Counts = model.Select(a => a.Count.ToString()).ToArray(),
                    ServiceName = model.Select(a => a.ServiceName + "(" + a.Count + ")").ToArray(),
                }, DataResponseStatus.OK);
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

        public DataResponse<EntityAccountHighlight> GetAllAccountHighlight(int businessId, int currentUserId, bool isBuzAdmin, bool isSalesManager, bool isSalesDirector)
        {
            var response = new DataResponse<EntityAccountHighlight>();
            try
            {
                base.DBInit();
                int ThisWeekCount = 0;
                int TotalCount = 0;
                var accountModel = DBEntity.Accounts.Where(a => a.BusinessId == businessId); //&& a.IsActive == true
                if (!isBuzAdmin)
                {
                    //var reps = DBEntity.Reps.Any(a => a.RepGroup.RepgroupManagerMappers.Any(b => b.ManagerId == currentUserId));//---ManagerChange     var reps = DBEntity.Reps.Any(a => a.RepGroup.ManagerId == currentUserId);
                    if (isSalesDirector || isSalesManager)
                    {
                        //Manager
                        accountModel = accountModel.Where(a => a.Lead.Rep.UserId == currentUserId || a.Lead.Rep.RepGroup.RepgroupManagerMappers.Any(b => b.ManagerId == currentUserId));//|| a.Lead.Rep.RepGroup.SalesDirectorId == currentUserId
                        //---ManagerChange     accountModel = accountModel.Where(a => a.Lead.Rep.UserId == currentUserId || a.Lead.Rep.RepGroup.ManagerId == currentUserId);
                    }
                    else
                    {
                        //Rep
                        accountModel = accountModel.Where(a => a.Lead.Rep.UserId == currentUserId);
                    }
                }
                TotalCount = accountModel.Count();
                #region ThisWeek

                var ThisWeekModel = accountModel;
                var ThisWeekStartOn = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                var Day = new List<string>();
                var Count = new List<string>();
                ThisWeekModel = ThisWeekModel.Where(a => a.CreatedOn >= ThisWeekStartOn);
                var model = ThisWeekModel.AsEnumerable().Select(s => new WeekModel
                {
                    CreatedOn = s.CreatedOn.ToShortDateString()
                }).ToList();
                ThisWeekCount = model.Count();

                var datepart1 = DateTime.UtcNow.ToShortDateString();
                string dayCount1 = model.Count(a => a.CreatedOn == datepart1).ToString();

                for (var startdt = ThisWeekStartOn; ThisWeekStartOn <= DateTime.UtcNow; ThisWeekStartOn = ThisWeekStartOn.AddDays(1))
                {
                    var datePart = ThisWeekStartOn.ToShortDateString();

                    string dayCount = model.Count(a => a.CreatedOn == datePart).ToString();
                    Count.Add(dayCount);
                    Day.Add(ThisWeekStartOn.DayOfWeek.ToString());
                    //Day.Add(ThisWeekStartOn.ToShortDateString());
                }

                #endregion

                #region LastWeek

                var LastWeekModel = accountModel;
                var LastWeekStartOn = DateTime.UtcNow.AddDays(-7);
                var LastWeekEndOn = LastWeekStartOn.AddDays(-6);
                var LastWeekDay = new List<string>();
                var LastWeekCount = new List<string>();
                LastWeekModel = LastWeekModel.Where(a => a.CreatedOn <= LastWeekStartOn && a.CreatedOn >= LastWeekEndOn);
                var modelLastWeek = LastWeekModel.AsEnumerable().Select(s => new WeekModel
                {
                    CreatedOn = s.CreatedOn.ToShortDateString()
                }).ToList();
                for (var startdt = LastWeekEndOn; LastWeekEndOn <= LastWeekStartOn; LastWeekEndOn = LastWeekEndOn.AddDays(1))
                {
                    string dayCount = model.Count(a => a.CreatedOn == LastWeekEndOn.ToShortDateString()).ToString();
                    LastWeekCount.Add(dayCount);
                    LastWeekDay.Add(LastWeekEndOn.DayOfWeek.ToString());
                    //LastWeekDay.Add(LastWeekEndOn.ToShortDateString());
                }
                #endregion

                response.CreateResponse<EntityAccountHighlight>(new EntityAccountHighlight
                {
                    TotalCount = TotalCount,
                    ThisWeekCount = ThisWeekCount,
                    ThisWeekCounts = Count.ToArray(),
                    ThisWeekdays = Day.ToArray(),
                    LastWeekCounts = LastWeekCount.ToArray(),
                    LastWeekdays = LastWeekDay.ToArray(),
                }, DataResponseStatus.OK);
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

        public DataResponse<EntityAccountHighlight> GetAllLeadSummary(int businessId, int currentUserId, bool isBuzAdmin, bool isSalesManager, bool isSalesDirector)
        {
            var response = new DataResponse<EntityAccountHighlight>();
            try
            {
                base.DBInit();
                int ThisWeekCount = 0;
                int TotalCount = 0;
                int MonthToDate = 0;
                var leadModel = DBEntity.Leads.Where(a => a.BusinessId == businessId && a.IsConverted == false && a.IsActive == true);
                if (!isBuzAdmin)
                {
                    // Note : implemented property isSalesManager
                    //var reps = DBEntity.Reps.Any(a => a.RepGroup.RepgroupManagerMappers.Any(b => b.ManagerId == currentUserId));//---ManagerChange     var reps = DBEntity.Reps.Any(a => a.RepGroup.ManagerId == currentUserId);
                    if (isSalesDirector || isSalesManager)
                    {
                        //Manager
                        leadModel = leadModel.Where(a => a.Rep.UserId == currentUserId || a.Rep.RepGroup.RepgroupManagerMappers.Any(b => b.ManagerId == currentUserId));// || a.Rep.RepGroup.SalesDirectorId == currentUserId
                        //---ManagerChange      leadModel = leadModel.Where(a => a.Rep.UserId == currentUserId || a.Rep.RepGroup.ManagerId == currentUserId);
                    }
                    else
                    {
                        //Rep
                        leadModel = leadModel.Where(a => a.Rep.UserId == currentUserId);
                    }

                }
                TotalCount = leadModel.Count();
                if (leadModel != null)
                {
                    var ThisMonthStartOn = DateTime.UtcNow.AddDays(-(DateTime.UtcNow.Day - 1));
                    MonthToDate = leadModel.Count(a => a.CreatedOn >= ThisMonthStartOn);
                }

                #region ThisWeek

                var ThisWeekModel = leadModel;
                var ThisWeekStartOn = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                var Day = new List<string>();
                var Count = new List<string>();
                ThisWeekModel = ThisWeekModel.Where(a => a.CreatedOn >= ThisWeekStartOn);
                var model = ThisWeekModel.AsEnumerable().Select(s => new WeekModel
                {
                    CreatedOn = s.CreatedOn.ToShortDateString()
                }).ToList();
                ThisWeekCount = model.Count();
                for (var startdt = ThisWeekStartOn; ThisWeekStartOn <= DateTime.UtcNow; ThisWeekStartOn = ThisWeekStartOn.AddDays(1))
                {
                    string dayCount = model.Count(a => a.CreatedOn == ThisWeekStartOn.ToShortDateString()).ToString();
                    Count.Add(dayCount);
                    Day.Add(ThisWeekStartOn.DayOfWeek.ToString());
                }

                #endregion

                #region LastWeek

                var LastWeekModel = leadModel;
                var LastWeekStartOn = DateTime.UtcNow.AddDays(-7);
                var LastWeekEndOn = LastWeekStartOn.AddDays(-6);
                var LastWeekDay = new List<string>();
                var LastWeekCount = new List<string>();
                LastWeekModel = LastWeekModel.Where(a => a.CreatedOn <= LastWeekStartOn && a.CreatedOn >= LastWeekEndOn);
                var modelLastWeek = LastWeekModel.AsEnumerable().Select(s => new WeekModel
                {
                    CreatedOn = s.CreatedOn.ToShortDateString()
                }).ToList();
                for (var startdt = LastWeekEndOn; LastWeekEndOn <= LastWeekStartOn; LastWeekEndOn = LastWeekEndOn.AddDays(1))
                {
                    string dayCount = model.Count(a => a.CreatedOn == LastWeekEndOn.ToShortDateString()).ToString();
                    LastWeekCount.Add(dayCount);
                    LastWeekDay.Add(LastWeekEndOn.DayOfWeek.ToString());
                }
                #endregion

                response.CreateResponse<EntityAccountHighlight>(new EntityAccountHighlight
                {
                    MonthToDate = MonthToDate,
                    TotalCount = TotalCount,
                    ThisWeekCount = ThisWeekCount,
                    ThisWeekCounts = Count.ToArray(),
                    ThisWeekdays = Day.ToArray(),
                    LastWeekCounts = LastWeekCount.ToArray(),
                    LastWeekdays = LastWeekDay.ToArray(),
                }, DataResponseStatus.OK);
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

        public DataResponse<EntityList<EntityLatestLead>> GetLeads(int? businessId, int currentUserId, bool isBuzAdmin, bool isSalesManager, bool isSalesDirector, bool isRep, DateTime? DateFrom = null, DateTime? DateTo = null)
        {
            var response = new DataResponse<EntityList<EntityLatestLead>>();
            try
            {
                SelectFilterBase filter = new SelectFilterBase();
                filter.CurrentPage = 1;
                filter.PageSize = 5;

                base.DBInit();

                var query = DBEntity.Leads.Where(a => a.IsConverted == false && a.BusinessId == businessId);

                if (!isBuzAdmin)
                {
                    //// Note : implemented property isSalesManager
                    ////var reps = DBEntity.Reps.Any(a => a.RepGroup.RepgroupManagerMappers.Any(b => b.ManagerId == currentUserId));//---ManagerChange     var reps = DBEntity.Reps.Any(a => a.RepGroup.ManagerId == currentUserId);
                    //if (isSalesDirector || isSalesManager)
                    //{
                    //    //Manager
                    //    query = query.Where(a => a.Rep.UserId == currentUserId || a.Rep.RepGroup.RepgroupManagerMappers.Any(b => b.ManagerId == currentUserId) || a.Rep.RepGroup.SalesDirectorId == currentUserId);
                    //    //---ManagerChange      query = query.Where(a => a.Rep.UserId == currentUserId || a.Rep.RepGroup.ManagerId == currentUserId);
                    //}
                    //else
                    //{
                    //    //Rep
                    //    query = query.Where(a => a.Rep.UserId == currentUserId);
                    //}

                    if (isSalesManager || isSalesDirector || isRep)
                        query = query.Where(a => a.Rep.UserId == currentUserId ||
                            a.Rep.RepGroup.RepgroupManagerMappers.Any(b => b.ManagerId == currentUserId) ||
                            //a.Rep.RepGroup.SalesDirectorId == currentUserId ||
                            a.Rep.UserId == currentUserId);
                }
                //DateFrom = DateTime.UtcNow.AddDays(-22);
                //DateTo = DateTime.UtcNow.AddDays(-21);
                if (DateFrom.HasValue)
                {
                    query = query.Where(a => a.CreatedOn >= DateFrom);
                }
                if (DateTo.HasValue)
                {
                    query = query.Where(a => a.CreatedOn <= DateTo);
                }

                var selectQuery = query.Select(a => new EntityLatestLead
                {
                    PracticeName = a.Practice.PracticeName,
                    RepFirstName = a.Rep.User2.FirstName,
                    RepLastName = a.Rep.User2.LastName,
                    ProvidersCount = a.Practice.PracticeProviderMappers.Count,
                    CreatedOn = a.CreatedOn
                }).OrderByDescending(o => o.CreatedOn);

                response = GetList<EntityLatestLead>(selectQuery, filter.Skip, filter.Take);
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

        public DataResponse<EntityList<EntityLatestAccount>> GetAccount(int? businessId, int currentUserId, bool isBuzAdmin, bool isSalesManager, bool isSalesDirector, DateTime? DateFrom = null, DateTime? DateTo = null)
        {
            var response = new DataResponse<EntityList<EntityLatestAccount>>();
            SelectFilterBase filter = new SelectFilterBase();
            filter.CurrentPage = 1;
            filter.PageSize = 5;

            base.DBInit();

            var query = DBEntity.Accounts.Join(
                DBEntity.Leads,
                accounts => accounts.LeadId,
                lead => lead.Id,
                (accounts, lead) => new { Accounts = accounts, Lead = lead }
                ).Where(a => a.Lead.IsConverted == true && a.Accounts.BusinessId == businessId);

            if (!isBuzAdmin)
            {
                // Note : implemented property isSalesManager
                //var reps = DBEntity.Reps.Any(a => a.RepGroup.RepgroupManagerMappers.Any(b => b.ManagerId == currentUserId));//---ManagerChange     var reps = DBEntity.Reps.Any(a => a.RepGroup.ManagerId == currentUserId);
                if (isSalesDirector || isSalesManager)
                {
                    //Manager
                    query = query.Where(a => a.Lead.Rep.UserId == currentUserId || a.Lead.Rep.RepGroup.RepgroupManagerMappers.Any(b => b.ManagerId == currentUserId));// || a.Lead.Rep.RepGroup.SalesDirectorId == currentUserId
                    //---ManagerChange     query = query.Where(a => a.Lead.Rep.UserId == currentUserId || a.Lead.Rep.RepGroup.ManagerId == currentUserId);
                }
                else
                {
                    //Rep
                    query = query.Where(a => a.Lead.Rep.UserId == currentUserId);
                }
            }
            //DateFrom = DateTime.UtcNow.AddDays(-22);
            //DateTo = DateTime.UtcNow.AddDays(-21);
            if (DateFrom.HasValue)
            {
                query = query.Where(a => a.Lead.CreatedOn >= DateFrom);
            }
            if (DateTo.HasValue)
            {
                query = query.Where(a => a.Lead.CreatedOn <= DateTo);
            }

            var selectQuery = query.Select(a => new EntityLatestAccount
            {
                PracticeName = a.Lead.Practice.PracticeName,
                CreatedOn = a.Accounts.CreatedOn,
                RepFirstName = a.Lead.Rep.User2.FirstName,
                RepLastName = a.Lead.Rep.User2.LastName,
                ProvidersCount = a.Lead.Practice.PracticeProviderMappers.Count,
            }).OrderByDescending(o => o.CreatedOn);

            response = GetList<EntityLatestAccount>(selectQuery, filter.Skip, filter.Take);

            base.DBClose();
            return response;
        }

        public DataResponse<EntityList<AccountSummary>> TopAccount(int serviceId, int? businessId, int currentUserId, bool isBuzAdmin, bool isSalesManager, bool isSalesDirector, bool isRep, DateTime? DateFrom = null, DateTime? DateTo = null)
        {
            var response = new DataResponse<EntityList<AccountSummary>>();
            SelectFilterBase filter = new SelectFilterBase();
            filter.CurrentPage = 1;
            filter.PageSize = 5;

            base.DBInit();

            var query = DBEntity.ReportMasters.Where(a => a.BusinessId == businessId);

            if (serviceId > 0)
            {
                query = query.Where(a => a.Practice.PracticeServiceMappers.Any(b => b.EnrolledServiceId == serviceId));
            }

            if (!isBuzAdmin)
            {
                if (isSalesManager || isSalesDirector || isRep)
                    query = query.Where(a => a.Rep.UserId == currentUserId || a.Rep.RepGroup.RepgroupManagerMappers.Any(b => b.ManagerId == currentUserId) ||
                        //a.Rep.RepGroup.SalesDirectorId == currentUserId ||
                        a.Rep.UserId == currentUserId);
            }

            if (DateFrom.HasValue)
            {
                query = query.Where(a => a.SpecimenReceivedDate >= DateFrom);
            }
            if (DateTo.HasValue)
            {
                query = query.Where(a => a.SpecimenReceivedDate <= DateTo);
            }

            var selectQuery = query.GroupBy(a => a.PracticeId).Select(a => new AccountSummary
            {
                Account = a.Select(g => new EntityLatestAccount
                {
                    PracticeName = g.PracticeName,
                    RepFirstName = g.Rep.User2.FirstName,
                    RepLastName = g.Rep.User2.LastName,
                }).FirstOrDefault(),
                SalesCount = a.Count(),
            }).OrderByDescending(o => o.SalesCount);

            response = GetList<AccountSummary>(selectQuery, filter.Skip, filter.Take);

            base.DBClose();
            return response;
        }

        public DataResponse<EntityList<TopRepSummary>> TopReps(int serviceId, int? businessId, int currentUserId, bool isBuzAdmin, bool isSalesManager, bool isSalesDirector, bool isRep, DateTime? DateFrom = null, DateTime? DateTo = null)
        {
            var response = new DataResponse<EntityList<TopRepSummary>>();
            SelectFilterBase filter = new SelectFilterBase();
            filter.CurrentPage = 1;
            filter.PageSize = 5;
            base.DBInit();
            var query = DBEntity.ReportMasters.Where(a => a.BusinessId == businessId && a.RepId != null && a.ServiceId == serviceId);

            if (!isBuzAdmin)
            {
                //    if (isSalesManager || isSalesDirector)
                //        query = query.Where(a => a.RepId == currentUserId ||
                //            a.Rep.RepGroup.RepgroupManagerMappers.Any(g => g.ManagerId == currentUserId) ||
                //            a.Rep.RepGroup.SalesDirectorId == currentUserId);

                if (isRep || isSalesManager || isSalesDirector)
                    query = query.Where(a => a.Rep.User2.Reps2.Any(b => b.UserId == currentUserId) ||
                        a.Rep.RepGroup.RepgroupManagerMappers.Any(b => b.ManagerId == currentUserId) || //---ManagerChange    a.Rep.User2.Reps2.Select(b => b.RepGroup.ManagerId).Contains(currentUserId) ||
                        a.Rep.User2.Reps2.Any(b => b.RepGroup.RepgroupManagerMappers.Any(c=>c.ManagerId== currentUserId && c.UserRole == (int)RepgroupUserType.Director)) ||
                        a.CreatedBy == currentUserId);
            }
            if (DateFrom.HasValue)
            {
                query = query.Where(a => a.SpecimenReceivedDate >= DateFrom);
            }
            if (DateTo.HasValue)
            {
                query = query.Where(a => a.SpecimenReceivedDate <= DateTo);
            }
            var queryGrouped = query.GroupBy(a => a.RepId).Select(s =>
                 new TopRepSummary
                 {
                     SalesCount = s.Count(),
                     Rep = s.Select(g => new EntityTopReps
                     {
                         RepFirstName = g.Rep.User2.FirstName,
                         RepLastName = g.Rep.User2.LastName,
                         RepGroup = g.Rep.RepGroup.RepGroupName
                     }).FirstOrDefault()
                 }).OrderByDescending(o => o.SalesCount);

            response = GetList<TopRepSummary>(queryGrouped, filter.Skip, filter.Take);

            base.DBClose();
            return response;
        }

        public DataResponse<EntityLeadFunnelModel> GetLeadFunnel(int businessId, int currentUserId, bool isBuzAdmin, bool isSalesManager, bool isSalesDirector, DateTime? DateFrom = null, DateTime? DateTo = null)
        {
            var response = new DataResponse<EntityLeadFunnelModel>();
            try
            {
                base.DBInit();
                int monthToDate = 0;
                int totalCount = 0;
                int weekToDate = 0;

                var oneMonth = DateTime.Now.AddMonths(-1);
                var twoMonth = DateTime.Now.AddMonths(-2);
                var threeMonth = DateTime.Now.AddMonths(-3);
                var twoDays = DateTime.Now.AddDays(-2);
                var thisWeekStartOn = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);

                var thisMonthStartOn = DateTime.UtcNow.AddDays(-(DateTime.UtcNow.Day - 1));

                var query = DBEntity.Accounts.Where(a => a.BusinessId == businessId);

                //var query = DBEntity.Accounts.Join(
                //   DBEntity.Leads,
                //   accounts => accounts.LeadId,
                //   lead => lead.Id,
                //   (accounts, lead) => new { Accounts = accounts, Lead = lead }
                //   ).Where(a => a.Lead.IsConverted == true && a.Accounts.BusinessId == businessId);//&& a.Accounts.IsActive == true

                if (!isBuzAdmin)
                {
                    // Note : implemented property isSalesManager
                    //var reps = DBEntity.Reps.Any(a => a.RepGroup.RepgroupManagerMappers.Any(b => b.ManagerId == currentUserId));//---ManagerChange     var reps = DBEntity.Reps.Any(a => a.RepGroup.ManagerId == currentUserId);
                    if (isSalesDirector || isSalesManager)
                    {
                        //Manager
                        query = query.Where(a => a.Lead.Rep.UserId == currentUserId
                        || a.Lead.Rep.RepGroup.RepgroupManagerMappers.Any(b => b.ManagerId == currentUserId));
                        //|| a.Lead.Rep.RepGroup.SalesDirectorId == currentUserId
                        //---ManagerChange     query = query.Where(a => a.Lead.Rep.UserId == currentUserId || a.Lead.Rep.RepGroup.ManagerId == currentUserId)
                    }
                    else
                    {
                        //Rep
                        query = query.Where(a => a.Lead.Rep.UserId == currentUserId);
                    }
                }

                totalCount = query.Count();
                monthToDate = query.Count(a => a.CreatedOn >= thisMonthStartOn);
                weekToDate = query.Count(a => a.CreatedOn >= thisWeekStartOn);

                if (DateFrom.HasValue)
                {
                    query = query.Where(a => a.CreatedOn >= DateFrom);
                }
                if (DateTo.HasValue)
                {
                    query = query.Where(a => a.CreatedOn <= DateTo);
                }

                float
                    NewCount = query.Count(a => (a.Lead.Practice.ReportMasters.Count() == 0 && a.CreatedOn > threeMonth) || (a.Lead.Practice.ReportMasters.Any(r => r.CreatedOn > threeMonth && a.CreatedOn < twoDays))),
                    ActiveCount = query.Count(a => a.Lead.Practice.ReportMasters.Any(r => r.CreatedOn >= twoDays)),
                    DormantCount = query.Count(a => !a.Lead.Practice.ReportMasters.Any(r => r.CreatedOn > threeMonth)),
                    TotalCount = NewCount + ActiveCount + DormantCount,
                    New = TotalCount > 0 ? (NewCount * 100) / TotalCount : 0,
                    Active = TotalCount > 0 ? (ActiveCount * 100) / TotalCount : 0,
                    Dormant = TotalCount > 0 ? (DormantCount * 100) / TotalCount : 0;

                var leadFunnel = new EntityLeadFunnelModel
                {
                    New = (float)Math.Round(New, 2),
                    Active = (float)Math.Round(Active, 2),
                    Dormant = (float)Math.Round(Dormant, 2),
                    TotalCount = totalCount,
                    ThisWeekCount = weekToDate,
                    MonthToDate = monthToDate
                };

                response.CreateResponse<EntityLeadFunnelModel>(leadFunnel, DataResponseStatus.OK);
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

        public DataResponse<dynamic> GetAllSalesPeriodicTrends(int businessId, int currentUserId, bool isBuzAdmin, bool IsRep, bool isSalesManager, bool isSalesDirector, int serviceId = 0, DateTime? DateFrom = null, DateTime? DateTo = null, string ViewBy = null, string DateType = null, string Total = null, string summaryColumns = "")
        {
            var response = new DataResponse<dynamic>();
            try
            {
                base.DBInit();
                var services = new RepositoryLookups().GetAllServices(businessId, currentUserId, isSalesManager, isSalesDirector, IsRep, isBuzAdmin).Model.List;

                if (services == null)
                {
                    response.Model = null;
                    response.Status = DataResponseStatus.NotFound;
                    response.Message = "Service not allowed!";
                    return response;
                }

                if (serviceId != 0)
                    services = services.Where(a => a.Id == serviceId).ToList();

                var serviceIds = services.Select(a => a.Id).ToArray();

                //var query = DBEntity.ReportMasters
                //    .Join(DBEntity.ReportFinances, r => r.Id, f => f.ReportId, (r, f) => new { r, f })
                //    .Where(a=>a.BusinessId==businessId && serviceIds.Contains(a.ServiceId)); 

                var query = DBEntity.ReportMasters
                    //.GroupJoin(DBEntity.ReportFinances, r => r.Id, f => f.ReportId, (r, f) => new { r, f })
                    //.Select(a =>
                    //new
                    //{
                    //    r = a.r,
                    //    f = new
                    //    {
                    //        BilledDate = a.f.FirstOrDefault().BilledDate,
                    //        Charges = a.f.Sum(C => C.Charges),
                    //        PaidDate = a.f.FirstOrDefault().PaidDate,
                    //        PaidAmount = a.f.Sum(S => S.PaidAmount)
                    //    }
                    //}
                    //)
                    .Where(a => a.BusinessId == businessId && serviceIds.Contains(a.ServiceId));

                //.Where(a => a.BusinessId == businessId && serviceIds.Contains(a.ServiceId));

                if (!isBuzAdmin)
                {

                    if (IsRep || isSalesManager || isSalesDirector)
                        query = query.Where(a => a.Rep.User2.Reps2.Any(b => b.UserId == currentUserId) ||
                            a.Rep.RepGroup.RepgroupManagerMappers.Any(b => b.ManagerId == currentUserId) || //---ManagerChange    a.Rep.User2.Reps2.Select(b => b.RepGroup.ManagerId).Contains(currentUserId) ||
                            //a.Rep.User2.Reps2.Any(b => b.RepGroup.SalesDirectorId == currentUserId) ||
                            a.CreatedBy == currentUserId);

                }

                var numberOfPreviousMonths = -6;// if from date and to date are not null, then take month difference

                var endDate = DateTime.UtcNow;
                var StartDate = endDate.AddMonths(numberOfPreviousMonths);

                if (DateFrom.HasValue)//if from date and to date are not null
                {
                    StartDate = DateFrom.Value;
                }

                if (DateTo.HasValue)
                {
                    endDate = DateTo.Value;
                }


                var sqlMinDate = (DateTime)SqlDateTime.MinValue;
                var objMonths = StartDate;
                var firstobjMonths = StartDate;

                StringBuilder dataColumns = new StringBuilder("[");
                var columnColors = new StringBuilder("{");
                StringBuilder sb = new StringBuilder("[");
                var isFirst = true;

                var yearResult = new List<VMServiceModel>();
                var monthResult = new List<VMServiceModel>();
                var weekResult = new List<VMServiceModel>();
                var dayResult = new List<VMServiceModel>();

                double totalBilled = 0;
                //double totalReimbursements = 0;
                double totalPaid = 0;

                int totalCount = 0;

                #region DateType
                switch (DateType)
                {
                    case "CreatedOn":
                        #region case1
                        if (query != null)
                        {
                            query = query.Where(a => a.CreatedOn != null && a.CreatedOn >= StartDate && a.CreatedOn <= endDate);

                            if (ViewBy == "1")
                            {
                                yearResult = query.GroupBy(x => new
                                {
                                    x.CreatedOn.Year,
                                    x.ServiceId
                                }).Select(g => new VMServiceModel
                                {
                                    Year = g.Key.Year,
                                    ServiceId = g.Key.ServiceId,
                                    ServiceCount = g.Count(),
                                    Billed = g.Sum(p => p.ReportFinances.Sum(s => s.Charges)),
                                    //Reimbursements = g.FirstOrDefault().ReportFinances.Sum(a => a.PaidAmount),
                                    Paid = g.Sum(p => p.ReportFinances.Sum(s => s.PaidAmount))
                                }).OrderBy(a => a.Year).ToList();
                                totalBilled = yearResult.Sum(a => a.Billed) ?? 0;
                                totalPaid = yearResult.Sum(a => a.Paid) ?? 0;
                            }

                            if (ViewBy == "2")
                            {
                                monthResult = query.GroupBy(x => new
                                {
                                    x.CreatedOn.Month,
                                    x.CreatedOn.Year,
                                    x.ServiceId
                                }).Select(g => new VMServiceModel
                                {
                                    //Month = g.Key.Month,
                                    Month = g.Key.Month + "-" + g.Key.Year,
                                    ServiceId = g.Key.ServiceId,
                                    ServiceCount = g.Count(),
                                    Billed = g.Sum(p => p.ReportFinances.Sum(s => s.Charges)),
                                    //Reimbursements = g.FirstOrDefault().ReportFinances.Sum(a => a.PaidAmount),
                                    Paid = g.Sum(p => p.ReportFinances.Sum(s => s.PaidAmount))
                                }).ToList();
                                totalBilled = monthResult.Sum(a => a.Billed) ?? 0;
                                totalPaid = monthResult.Sum(a => a.Paid) ?? 0;
                            }
                            if (ViewBy == "3")
                            {
                                weekResult = query.GroupBy(x => new
                                {
                                    Week = SqlFunctions.DatePart("week", x.CreatedOn),
                                    x.CreatedOn.Year,
                                    x.ServiceId
                                }).Select(g => new VMServiceModel
                                {
                                    Week = g.Key.Week + "-" + g.Key.Year,
                                    ServiceId = g.Key.ServiceId,
                                    ServiceCount = g.Count(),
                                    Billed = g.Sum(p => p.ReportFinances.Sum(s => s.Charges)),
                                    //Reimbursements = g.FirstOrDefault().ReportFinances.Sum(a => a.PaidAmount),
                                    Paid = g.Sum(p => p.ReportFinances.Sum(s => s.PaidAmount))
                                }).ToList();
                                totalBilled = weekResult.Sum(a => a.Billed) ?? 0;
                                totalPaid = weekResult.Sum(a => a.Paid) ?? 0;
                            }
                            if (ViewBy == "4")
                            {
                                dayResult = query.GroupBy(x => new
                                {
                                    day = EntityFunctions.TruncateTime(x.CreatedOn),
                                    x.ServiceId
                                }).Select(g => new VMServiceModel
                                {
                                    Day = EntityFunctions.TruncateTime(g.Key.day),
                                    ServiceId = g.Key.ServiceId,
                                    ServiceCount = g.Count(),
                                    Billed = g.Sum(p => p.ReportFinances.Sum(s => s.Charges)),
                                    //Reimbursements = g.FirstOrDefault().ReportFinances.Sum(a => a.PaidAmount),
                                    Paid = g.Sum(p => p.ReportFinances.Sum(s => s.PaidAmount))
                                }).OrderBy(a => a.Day).ToList();
                                totalBilled = dayResult.Sum(a => a.Billed) ?? 0;
                                totalPaid = dayResult.Sum(a => a.Paid) ?? 0;
                            }
                        }
                        #endregion
                        break;
                    case "CollectedDate":
                        #region case2
                        if (query != null)
                        {
                            query = query.Where(a => a.SpecimenCollectionDate != null && a.SpecimenCollectionDate.Value >= StartDate && a.SpecimenCollectionDate <= endDate);

                            if (ViewBy == "1")
                            {
                                yearResult = query.GroupBy(x => new
                                {
                                    x.SpecimenCollectionDate.Value.Year,
                                    x.ServiceId
                                }).Select(g => new VMServiceModel
                                {
                                    Year = g.Key.Year,
                                    ServiceId = g.Key.ServiceId,
                                    ServiceCount = g.Count(),
                                    Billed = g.Sum(p => p.ReportFinances.Sum(s => s.Charges)),
                                    //Reimbursements = g.FirstOrDefault().ReportFinances.Sum(a => a.PaidAmount),
                                    Paid = g.Sum(p => p.ReportFinances.Sum(s => s.PaidAmount))
                                }).ToList();
                                totalBilled = yearResult.Sum(a => a.Billed) ?? 0;
                                totalPaid = yearResult.Sum(a => a.Paid) ?? 0;
                                totalCount = yearResult.Sum(a => a.ServiceCount);
                            }
                            if (ViewBy == "2")
                            {
                                monthResult = query.GroupBy(x => new
                                {
                                    x.SpecimenCollectionDate.Value.Month,
                                    x.SpecimenCollectionDate.Value.Year,
                                    x.ServiceId
                                }).Select(g => new VMServiceModel
                                {
                                    //Month = g.Key.Month,
                                    Month = g.Key.Month + "-" + g.Key.Year,
                                    ServiceId = g.Key.ServiceId,
                                    ServiceCount = g.Count(),
                                    Billed = g.Sum(p => p.ReportFinances.Sum(s => s.Charges)),
                                    // Reimbursements = g.FirstOrDefault().ReportFinances.Sum(a => a.PaidAmount),
                                    Paid = g.Sum(p => p.ReportFinances.Sum(s => s.PaidAmount))
                                }).ToList();
                                totalBilled = monthResult.Sum(a => a.Billed) ?? 0;
                                totalPaid = monthResult.Sum(a => a.Paid) ?? 0;
                                totalCount = monthResult.Sum(a => a.ServiceCount);
                            }
                            if (ViewBy == "3")
                            {
                                weekResult = query.GroupBy(x => new
                                {
                                    Week = SqlFunctions.DatePart("week", x.SpecimenCollectionDate),
                                    x.SpecimenCollectionDate.Value.Year,
                                    x.ServiceId
                                }).Select(g => new VMServiceModel
                                {
                                    //Week = g.Key.Week,
                                    Week = g.Key.Week + "-" + g.Key.Year,
                                    ServiceId = g.Key.ServiceId,
                                    ServiceCount = g.Count(),
                                    Billed = g.Sum(p => p.ReportFinances.Sum(s => s.Charges)),
                                    // Reimbursements = g.FirstOrDefault().ReportFinances.Sum(a => a.PaidAmount),
                                    Paid = g.Sum(p => p.ReportFinances.Sum(s => s.PaidAmount))
                                }).ToList();
                                totalBilled = weekResult.Sum(a => a.Billed) ?? 0;
                                totalPaid = weekResult.Sum(a => a.Paid) ?? 0;
                                totalCount = weekResult.Sum(a => a.ServiceCount);
                            }
                            if (ViewBy == "4")
                            {
                                dayResult = query.GroupBy(x => new
                                {
                                    day = EntityFunctions.TruncateTime(x.SpecimenCollectionDate.Value),
                                    x.ServiceId
                                }).Select(g => new VMServiceModel
                                {
                                    Day = EntityFunctions.TruncateTime(g.Key.day),
                                    ServiceId = g.Key.ServiceId,
                                    ServiceCount = g.Count(),
                                    Billed = g.Sum(p => p.ReportFinances.Sum(s => s.Charges)),
                                    // Reimbursements = g.FirstOrDefault().ReportFinances.Sum(a => a.PaidAmount),
                                    Paid = g.Sum(p => p.ReportFinances.Sum(s => s.PaidAmount))
                                }).ToList();

                                totalBilled = dayResult.Sum(a => a.Billed) ?? 0;
                                totalPaid = dayResult.Sum(a => a.Paid) ?? 0;
                                totalCount = dayResult.Sum(a => a.ServiceCount);
                            }
                        }
                        #endregion
                        break;
                    case "ReceivedDate":
                        #region case3
                        if (query != null)
                        {
                            query = query.Where(a => a.SpecimenReceivedDate != null && a.SpecimenReceivedDate.Value >= StartDate && a.SpecimenReceivedDate <= endDate);
                            if (ViewBy == "1")
                            {
                                yearResult = query.GroupBy(x => new
                                {
                                    Year = x.SpecimenReceivedDate == null ? -1 : x.SpecimenReceivedDate.Value.Year,
                                    x.ServiceId
                                }).Select(g => new VMServiceModel
                                {
                                    Year = g.Key.Year,
                                    ServiceId = g.Key.ServiceId,
                                    ServiceCount = g.Count(),
                                    Billed = g.Sum(p => p.ReportFinances.Sum(s => s.Charges)),
                                    //Reimbursements = g.FirstOrDefault().ReportFinances.Sum(a => a.PaidAmount),
                                    Paid = g.Sum(p => p.ReportFinances.Sum(s => s.PaidAmount))
                                }).ToList();

                                totalBilled = yearResult.Sum(a => a.Billed) ?? 0;
                                totalPaid = yearResult.Sum(a => a.Paid) ?? 0;
                                totalCount = yearResult.Sum(a => a.ServiceCount);
                            }
                            else if (ViewBy == "2")
                            {
                                monthResult = query.GroupBy(x => new
                                {
                                    Month = x.SpecimenReceivedDate == null ? -1 : x.SpecimenReceivedDate.Value.Month,
                                    Year = x.SpecimenReceivedDate == null ? -1 : x.SpecimenReceivedDate.Value.Year,
                                    ServiceId = x.ServiceId
                                    // Month = x.r.SpecimenReceivedDate.Value.Month,
                                    // Year = x.r.SpecimenReceivedDate.Value.Year,
                                    // ServiceId =  x.r.ServiceId
                                }).Select(g => new VMServiceModel
                                {
                                    //Month = g.Key.Month,
                                    Month = g.Key.Month + "-" + g.Key.Year,
                                    ServiceId = g.Key.ServiceId,
                                    ServiceCount = g.Count(),
                                    Billed = g.Sum(p => p.ReportFinances.Sum(s => s.Charges)),
                                    // Reimbursements = g.FirstOrDefault().ReportFinances.Sum(a => a.PaidAmount),
                                    Paid = g.Sum(p => p.ReportFinances.Sum(s => s.PaidAmount))
                                }).ToList();

                                totalBilled = monthResult.Sum(a => a.Billed) ?? 0;
                                totalPaid = monthResult.Sum(a => a.Paid) ?? 0;
                                totalCount = monthResult.Sum(a => a.ServiceCount);
                            }

                            else if (ViewBy == "3")
                            {
                                weekResult = query.GroupBy(x => new
                                {
                                    Week = SqlFunctions.DatePart("week", x.SpecimenReceivedDate),
                                    x.SpecimenReceivedDate.Value.Year,
                                    x.ServiceId
                                }).Select(g => new VMServiceModel
                                {
                                    //Week = g.Key.Week,
                                    Week = g.Key.Week + "-" + g.Key.Year,
                                    ServiceId = g.Key.ServiceId,
                                    ServiceCount = g.Count(),
                                    Billed = g.Sum(p => p.ReportFinances.Sum(s => s.Charges)),
                                    // Reimbursements = g.FirstOrDefault().ReportFinances.Sum(a => a.PaidAmount),
                                    Paid = g.Sum(p => p.ReportFinances.Sum(s => s.PaidAmount))
                                }).ToList();
                                totalBilled = weekResult.Sum(a => a.Billed) ?? 0;
                                totalPaid = weekResult.Sum(a => a.Paid) ?? 0;
                                totalCount = weekResult.Sum(a => a.ServiceCount);
                            }
                            else if (ViewBy == "4")
                            {
                                dayResult = query.GroupBy(x => new

                                {
                                    day = EntityFunctions.TruncateTime(x.SpecimenReceivedDate.Value),
                                    x.ServiceId
                                }).Select(g => new VMServiceModel
                                {
                                    Day = EntityFunctions.TruncateTime(g.Key.day),
                                    ServiceId = g.Key.ServiceId,
                                    ServiceCount = g.Count(),
                                    Billed = g.Sum(p => p.ReportFinances.Sum(s => s.Charges)),
                                    // Reimbursements = g.FirstOrDefault().ReportFinances.Sum(a => a.PaidAmount),
                                    Paid = g.Sum(p => p.ReportFinances.Sum(s => s.PaidAmount))
                                }).ToList();
                                totalBilled = dayResult.Sum(a => a.Billed) ?? 0;
                                totalPaid = dayResult.Sum(a => a.Paid) ?? 0;
                                totalCount = dayResult.Sum(a => a.ServiceCount);
                            }
                        }
                        #endregion
                        break;
                    case "ReportedDate":
                        #region case4
                        if (query != null)
                        {
                            query = query.Where(a => a.ReportedDate != null && a.ReportedDate.Value >= StartDate && a.ReportedDate <= endDate);
                            if (ViewBy == "1")
                            {
                                yearResult = query.GroupBy(x => new
                                {
                                    Year = x.ReportedDate == null ? -1 : x.ReportedDate.Value.Year,
                                    x.ServiceId
                                }).Select(g => new VMServiceModel
                                {
                                    Year = g.Key.Year,
                                    ServiceId = g.Key.ServiceId,
                                    ServiceCount = g.Count(),
                                    Billed = g.Sum(p => p.ReportFinances.Sum(s => s.Charges)),
                                    // Reimbursements = g.FirstOrDefault().ReportFinances.Sum(a => a.PaidAmount),
                                    Paid = g.Sum(p => p.ReportFinances.Sum(s => s.PaidAmount))
                                }).ToList();
                                totalBilled = yearResult.Sum(a => a.Billed) ?? 0;
                                totalPaid = yearResult.Sum(a => a.Paid) ?? 0;
                                totalCount = yearResult.Sum(a => a.ServiceCount);
                                //totalCount = query.Count();
                            }
                            if (ViewBy == "2")
                            {
                                monthResult = query.GroupBy(x => new
                                {
                                    Month = x.ReportedDate == null ? -1 : x.ReportedDate.Value.Month,
                                    Year = x.ReportedDate == null ? -1 : x.ReportedDate.Value.Year,
                                    x.ServiceId
                                }).Select(g => new VMServiceModel
                                {
                                    //Month = g.Key.Month,
                                    Month = g.Key.Month + "-" + g.Key.Year,
                                    ServiceId = g.Key.ServiceId,
                                    ServiceCount = g.Count(),
                                    Billed = g.Sum(p => p.ReportFinances.Sum(s => s.Charges)),
                                    //Reimbursements = g.FirstOrDefault().ReportFinances.Sum(a => a.PaidAmount),
                                    Paid = g.Sum(p => p.ReportFinances.Sum(s => s.PaidAmount))
                                }).ToList();
                                totalBilled = monthResult.Sum(a => a.Billed) ?? 0;
                                totalPaid = monthResult.Sum(a => a.Paid) ?? 0;
                                totalCount = query.Count();
                            }
                            if (ViewBy == "3")
                            {
                                weekResult = query.GroupBy(x => new
                                {
                                    Week = x.ReportedDate == null ? -1 : SqlFunctions.DatePart("week", x.ReportedDate.Value),
                                    Year = x.ReportedDate == null ? -1 : x.ReportedDate.Value.Year,
                                    x.ServiceId
                                }).Select(g => new VMServiceModel
                                {
                                    //Week = g.Key.Week,
                                    Week = g.Key.Week + "-" + g.Key.Year,
                                    ServiceId = g.Key.ServiceId,
                                    ServiceCount = g.Count(),
                                    Billed = g.Sum(p => p.ReportFinances.Sum(s => s.Charges)),
                                    // Reimbursements = g.FirstOrDefault().ReportFinances.Sum(a => a.PaidAmount),
                                    Paid = g.Sum(p => p.ReportFinances.Sum(s => s.PaidAmount))
                                }).ToList();
                                totalBilled = weekResult.Sum(a => a.Billed) ?? 0;
                                totalPaid = weekResult.Sum(a => a.Paid) ?? 0;
                                totalCount = query.Count();
                            }
                            if (ViewBy == "4")
                            {
                                dayResult = query.GroupBy(x => new
                                {
                                    day = x.ReportedDate.HasValue ? EntityFunctions.TruncateTime(x.ReportedDate.Value) : null,
                                    x.ServiceId
                                }).Select(g => new VMServiceModel
                                {
                                    Day = EntityFunctions.TruncateTime(g.Key.day),
                                    ServiceId = g.Key.ServiceId,
                                    ServiceCount = g.Count(),
                                    Billed = g.Sum(p => p.ReportFinances.Sum(s => s.Charges)),
                                    //Reimbursements = g.FirstOrDefault().ReportFinances.Sum(a => a.PaidAmount),
                                    Paid = g.Sum(p => p.ReportFinances.Sum(s => s.PaidAmount))
                                }).ToList();

                                totalBilled = dayResult.Sum(a => a.Billed) ?? 0;
                                totalPaid = dayResult.Sum(a => a.Paid) ?? 0;
                                totalCount = query.Count();
                            }
                        }
                        #endregion
                        break;
                    case "BilledDate":
                        #region case5

                        var query1 = DBEntity.ReportFinances
                            .Where(a => serviceIds.Contains(a.ServiceId));

                        if (query1 != null)
                        {
                            query1 = query1.Where(a => a.BilledDate != null && a.BilledDate.Value >= StartDate && a.BilledDate <= endDate);

                            if (ViewBy == "1")
                            {
                                yearResult = query1.GroupBy(x => new
                                {
                                    Year = x.BilledDate.Value.Year,
                                    x.ServiceId
                                }).Select(g => new VMServiceModel
                                {
                                    Year = g.Key.Year,
                                    ServiceId = g.Key.ServiceId,
                                    ServiceCount = g.Select(c => c.ReportKey).Distinct().Count(),
                                    Billed = g.Sum(p => p.Charges),
                                    //Reimbursements = g.FirstOrDefault().ReportFinances.Sum(a => a.PaidAmount),
                                    Paid = g.Sum(p => p.PaidAmount)
                                }).ToList();
                                totalBilled = yearResult.Sum(a => a.Billed) ?? 0;
                                totalPaid = yearResult.Sum(a => a.Paid) ?? 0;
                                totalCount = yearResult.Sum(a => a.ServiceCount);
                            }
                            if (ViewBy == "2")
                            {
                                monthResult = query1.GroupBy(x => new
                                {
                                    Month = x.BilledDate.Value.Month,
                                    Year = x.BilledDate.Value.Year,
                                    x.ServiceId
                                }).Select(g => new VMServiceModel
                                {
                                    //Month = g.Key.Month,
                                    Month = g.Key.Month + "-" + g.Key.Year,
                                    ServiceId = g.Key.ServiceId,
                                    ServiceCount = g.Select(c => c.ReportKey).Distinct().Count(),
                                    Billed = g.Sum(p => p.Charges),
                                    //Reimbursements = g.FirstOrDefault().ReportFinances.Sum(a => a.PaidAmount),
                                    Paid = g.Sum(p => p.PaidAmount)
                                }).ToList();
                                totalBilled = monthResult.Sum(a => a.Billed) ?? 0;
                                totalPaid = monthResult.Sum(a => a.Paid) ?? 0;
                                totalCount = monthResult.Sum(a => a.ServiceCount);
                            }
                            if (ViewBy == "3")
                            {
                                weekResult = query1.GroupBy(x => new
                                {
                                    Week = SqlFunctions.DatePart("week", x.BilledDate.Value),
                                    Year = x.BilledDate.Value.Year,
                                    x.ServiceId
                                }).Select(g => new VMServiceModel
                                {
                                    //Week = g.Key.Week,
                                    Week = g.Key.Week + "-" + g.Key.Year,
                                    ServiceId = g.Key.ServiceId,
                                    ServiceCount = g.Select(c => c.ReportKey).Distinct().Count(),
                                    Billed = g.Sum(p => p.Charges),
                                    // Reimbursements = g.FirstOrDefault().ReportFinances.Sum(a => a.PaidAmount),
                                    Paid = g.Sum(p => p.PaidAmount)
                                }).ToList();
                                totalBilled = weekResult.Sum(a => a.Billed) ?? 0;
                                totalPaid = weekResult.Sum(a => a.Paid) ?? 0;
                                totalCount = weekResult.Sum(a => a.ServiceCount);
                                //totalCount = query1.Count();
                            }
                            if (ViewBy == "4")
                            {
                                dayResult = query1.GroupBy(x => new
                                {
                                    day = EntityFunctions.TruncateTime(x.BilledDate.Value),
                                    x.ServiceId
                                }).Select(g => new VMServiceModel
                                {
                                    Day = EntityFunctions.TruncateTime(g.Key.day),
                                    ServiceId = g.Key.ServiceId,
                                    ServiceCount = g.Select(c => c.ReportKey).Distinct().Count(),
                                    Billed = g.Sum(p => p.Charges),
                                    // Reimbursements = g.FirstOrDefault().ReportFinances.Sum(a => a.PaidAmount),
                                    Paid = g.Sum(p => p.PaidAmount)
                                }).OrderBy(a => a.Day).ToList();
                                totalBilled = dayResult.Sum(a => a.Billed) ?? 0;
                                totalPaid = dayResult.Sum(a => a.Paid) ?? 0;
                                totalCount = dayResult.Sum(a => a.ServiceCount);
                                // totalCount = query1.Count();
                            }
                        }
                        #endregion
                        break;
                    case "PaidDate":
                        #region case6

                        query1 = DBEntity.ReportFinances
                           .Where(a => serviceIds.Contains(a.ServiceId));


                        if (query1 != null)
                        {

                            query1 = query1.Where(a => a.PaidDate != null && a.PaidDate.Value >= StartDate && a.PaidDate <= endDate);

                            if (ViewBy == "1")
                            {
                                yearResult = query1.GroupBy(x => new
                                {
                                    Year = x.PaidDate.Value.Year,//x.ReportFinances.FirstOrDefault(c => c.PaidDate != null).PaidDate.Value.Year,
                                    ServiceId = x.ServiceId
                                }).Select(g => new VMServiceModel
                                {
                                    Year = g.Key.Year,
                                    ServiceId = g.Key.ServiceId,
                                    ServiceCount = g.Select(c => c.ReportKey).Distinct().Count(),
                                    Billed = g.Sum(p => p.Charges), //g.Sum(p => p.ReportFinances.Sum(a => a.Charges)),
                                    //Reimbursements = g.Sum(p=>p.ReportFinances.PaidDate),// .FirstOrDefault().ReportFinances.Sum(a => a.PaidAmount),
                                    Paid = g.Sum(p => p.PaidAmount)
                                }).ToList();



                                totalBilled = yearResult.Sum(a => a.Billed) ?? 0;
                                totalPaid = yearResult.Sum(a => a.Paid) ?? 0;
                                totalCount = yearResult.Sum(a => a.ServiceCount);
                                //totalCount = query1.Count();
                            }
                            if (ViewBy == "2")
                            {
                                monthResult = query1
                                    .GroupBy(x => new
                                    {
                                        Month = x.PaidDate.Value.Month, //x.ReportFinances.FirstOrDefault(c => c.PaidDate != null).PaidDate.Value.Month,
                                        Year = x.PaidDate.Value.Year, //x.ReportFinances.FirstOrDefault(c => c.PaidDate != null).PaidDate.Value.Year,
                                        x.ServiceId
                                    }).Select(g => new VMServiceModel
                                    {
                                        //Month = g.Key.Month,
                                        Month = g.Key.Month + "-" + g.Key.Year,
                                        ServiceId = g.Key.ServiceId,
                                        ServiceCount = g.Select(c => c.ReportKey).Distinct().Count(),
                                        Billed = g.Sum(p => p.Charges),
                                        //Reimbursements = g.FirstOrDefault().ReportFinances.Sum(a => a.PaidAmount),
                                        Paid = g.Sum(p => p.PaidAmount)
                                    }).ToList();
                                totalBilled = monthResult.Sum(a => a.Billed) ?? 0;
                                totalPaid = monthResult.Sum(a => a.Paid) ?? 0;
                                totalCount = monthResult.Sum(a => a.ServiceCount);
                                //totalCount = query1.Count();
                            }
                            if (ViewBy == "3")
                            {
                                weekResult = query1.GroupBy(x => new
                                {
                                    Week = SqlFunctions.DatePart("week", x.PaidDate.Value),
                                    Year = x.PaidDate.Value.Year,
                                    x.ServiceId
                                }).Select(g => new VMServiceModel
                                {
                                    //Week = g.Key.Week,
                                    Week = g.Key.Week + "-" + g.Key.Year,
                                    ServiceId = g.Key.ServiceId,
                                    ServiceCount = g.Count(),
                                    Billed = g.Sum(p => p.Charges),
                                    // Reimbursements = g.FirstOrDefault().ReportFinances.Sum(a => a.PaidAmount),
                                    Paid = g.Sum(p => p.PaidAmount)
                                }).ToList();

                                totalBilled = weekResult.Sum(a => a.Billed) ?? 0;
                                totalPaid = weekResult.Sum(a => a.Paid) ?? 0;
                                totalCount = weekResult.Sum(a => a.ServiceCount);
                                // totalCount = query1.Count();
                            }
                            if (ViewBy == "4")
                            {
                                dayResult = query1.GroupBy(x => new
                                {
                                    day = EntityFunctions.TruncateTime(x.PaidDate.Value),
                                    x.ServiceId
                                }).Select(g => new VMServiceModel
                                {
                                    Day = EntityFunctions.TruncateTime(g.Key.day),
                                    ServiceId = g.Key.ServiceId,
                                    ServiceCount = g.Count(),
                                    Billed = g.Sum(p => p.Charges),
                                    //  Reimbursements = g.FirstOrDefault().ReportFinances.Sum(a => a.PaidAmount),
                                    Paid = g.Sum(p => p.PaidAmount)
                                }).ToList();
                                totalBilled = dayResult.Sum(a => a.Billed) ?? 0;
                                totalPaid = dayResult.Sum(a => a.Paid) ?? 0;
                                totalCount = dayResult.Sum(a => a.ServiceCount);
                                //totalCount = query1.Count();
                            }
                        }
                        #endregion
                        break;
                }
                #endregion

                while (endDate >= objMonths)
                {
                    if (!isFirst)
                        sb.Append(",");
                    isFirst = false;
                    var monthString = objMonths.ToString("yyyy-MM-dd");
                    sb.Append(string.Format("{{ \"Month\":\"{0}\"", monthString));
                    objMonths = new DateTime(objMonths.Year, objMonths.Month, objMonths.Day, 0, 0, 0);
                    firstobjMonths = new DateTime(firstobjMonths.Year, objMonths.Month, objMonths.Day, 0, 0, 0);
                    foreach (var service in services)
                    {
                        if (objMonths == firstobjMonths)
                        {
                            dataColumns.Append(string.Format("{{\"id\":\"{0}\", \"type\":\"bar\"}},", service.Value.ToString()));
                            columnColors.Append(string.Format("\"{0}\":\"{1}\",", service.Value.ToString(), service.Text));
                        }
                        switch (ViewBy)
                        {
                            case "1":
                                switch (Total)
                                {
                                    case "Sales":
                                        var YearCount = yearResult.Where(a => a.ServiceId == service.Id && a.Year == objMonths.Year).FirstOrDefault();
                                        sb.Append(string.Format(",\"{0}\" : \"{1}\"", service.Value.ToString(), YearCount == null ? "0" : YearCount.ServiceCount.ToString()));
                                        break;
                                    case "Reimbursements":
                                        YearCount = yearResult.Where(a => a.ServiceId == service.Id && a.Year == objMonths.Year).FirstOrDefault();
                                        sb.Append(string.Format(",\"{0}\" : \"{1}\"", service.Value.ToString(), YearCount == null ? "0" : YearCount.Reimbursements.ToString()));
                                        break;
                                    case "PaidAmount":
                                        YearCount = yearResult.Where(a => a.ServiceId == service.Id && a.Year == objMonths.Year).FirstOrDefault();
                                        sb.Append(string.Format(",\"{0}\" : \"{1}\"", service.Value.ToString(), YearCount == null ? "0" : YearCount.Paid.HasValue ? YearCount.Paid.Value.ToString("F2") : "0"));
                                        break;
                                    case "Charges":
                                        YearCount = yearResult.Where(a => a.ServiceId == service.Id && a.Year == objMonths.Year).FirstOrDefault();
                                        sb.Append(string.Format(",\"{0}\" : \"{1}\"", service.Value.ToString(), YearCount == null ? "0" : YearCount.Billed.HasValue ? YearCount.Billed.Value.ToString("F2") : "0"));
                                        break;
                                    default:
                                        YearCount = yearResult.Where(a => a.ServiceId == service.Id && a.Year == objMonths.Year).FirstOrDefault();
                                        sb.Append(string.Format(",\"{0}\" : \"{1}\"", service.Value.ToString(), YearCount == null ? "0" : YearCount.ServiceCount.ToString()));
                                        break;
                                }

                                break;

                            case "2":
                                switch (Total)
                                {
                                    case "Sales":
                                        var monthCount = monthResult.Where(a => a.ServiceId == service.Id && a.Month == objMonths.Month + "-" + objMonths.Year).FirstOrDefault();
                                        sb.Append(string.Format(",\"{0}\" : \"{1}\"", service.Value.ToString(), monthCount == null ? "0" : monthCount.ServiceCount.ToString()));
                                        break;
                                    case "Reimbursements":
                                        monthCount = monthResult.Where(a => a.ServiceId == service.Id && a.Month == objMonths.Month + "-" + objMonths.Year).FirstOrDefault();
                                        sb.Append(string.Format(",\"{0}\" : \"{1}\"", service.Value.ToString(), monthCount == null ? "0" : monthCount.Reimbursements.ToString()));
                                        break;
                                    case "PaidAmount":
                                        monthCount = monthResult.Where(a => a.ServiceId == service.Id && a.Month == objMonths.Month + "-" + objMonths.Year).FirstOrDefault();
                                        sb.Append(string.Format(",\"{0}\" : \"{1}\"", service.Value.ToString(), monthCount == null ? "0" : monthCount.Paid.HasValue ? monthCount.Paid.Value.ToString("F2") : "0"));
                                        break;
                                    case "Charges":
                                    case "BilledAmount":
                                        monthCount = monthResult.Where(a => a.ServiceId == service.Id && a.Month == objMonths.Month + "-" + objMonths.Year).FirstOrDefault();
                                        sb.Append(string.Format(",\"{0}\" : \"{1}\"", service.Value.ToString(), monthCount == null ? "0" : monthCount.Billed.HasValue ? monthCount.Billed.Value.ToString("F2") : "0"));
                                        break;
                                    default:
                                        monthCount = monthResult.Where(a => a.ServiceId == service.Id && a.Month == objMonths.Month + "-" + objMonths.Year).FirstOrDefault();
                                        sb.Append(string.Format(",\"{0}\" : \"{1}\"", service.Value.ToString(), monthCount == null ? "0" : monthCount.ServiceCount.ToString()));
                                        break;
                                }

                                break;

                            case "3":
                                switch (Total)
                                {
                                    case "Sales":
                                        var currentCulture = CultureInfo.CurrentCulture;
                                        var weekNo = currentCulture.Calendar.GetWeekOfYear(
                                                        objMonths,
                                                        currentCulture.DateTimeFormat.CalendarWeekRule,
                                                        currentCulture.DateTimeFormat.FirstDayOfWeek);
                                        var WeekCount = weekResult.Where(a => a.ServiceId == service.Id && a.Week == weekNo + "-" + objMonths.Year).FirstOrDefault();
                                        sb.Append(string.Format(",\"{0}\" : \"{1}\"", service.Value.ToString(), WeekCount == null ? "0" : WeekCount.ServiceCount.ToString()));
                                        break;
                                    case "Reimbursements":
                                        currentCulture = CultureInfo.CurrentCulture;
                                        weekNo = currentCulture.Calendar.GetWeekOfYear(
                                                        objMonths,
                                                        currentCulture.DateTimeFormat.CalendarWeekRule,
                                                        currentCulture.DateTimeFormat.FirstDayOfWeek);
                                        WeekCount = weekResult.Where(a => a.ServiceId == service.Id && a.Week == weekNo + "-" + objMonths.Year).FirstOrDefault();
                                        sb.Append(string.Format(",\"{0}\" : \"{1}\"", service.Value.ToString(), WeekCount == null ? "0" : WeekCount.Reimbursements.ToString()));
                                        break;
                                    case "PaidAmount":
                                        currentCulture = CultureInfo.CurrentCulture;
                                        weekNo = currentCulture.Calendar.GetWeekOfYear(
                                                        objMonths,
                                                        currentCulture.DateTimeFormat.CalendarWeekRule,
                                                        currentCulture.DateTimeFormat.FirstDayOfWeek);
                                        WeekCount = weekResult.Where(a => a.ServiceId == service.Id && a.Week == weekNo + "-" + objMonths.Year).FirstOrDefault();
                                        sb.Append(string.Format(",\"{0}\" : \"{1}\"", service.Value.ToString(), WeekCount == null ? "0" : WeekCount.Paid.HasValue ? WeekCount.Paid.Value.ToString("F2") : "0"));
                                        break;
                                    case "Charges":
                                        currentCulture = CultureInfo.CurrentCulture;
                                        weekNo = currentCulture.Calendar.GetWeekOfYear(
                                                        objMonths,
                                                        currentCulture.DateTimeFormat.CalendarWeekRule,
                                                        currentCulture.DateTimeFormat.FirstDayOfWeek);
                                        WeekCount = weekResult.Where(a => a.ServiceId == service.Id && a.Week == weekNo + "-" + objMonths.Year).FirstOrDefault();
                                        sb.Append(string.Format(",\"{0}\" : \"{1}\"", service.Value.ToString(), WeekCount == null ? "0" : WeekCount.Billed.HasValue ? WeekCount.Billed.Value.ToString("F2") : "0"));
                                        break;
                                    default:
                                        currentCulture = CultureInfo.CurrentCulture;
                                        weekNo = currentCulture.Calendar.GetWeekOfYear(
                                                        objMonths,
                                                        currentCulture.DateTimeFormat.CalendarWeekRule,
                                                        currentCulture.DateTimeFormat.FirstDayOfWeek);
                                        WeekCount = weekResult.Where(a => a.ServiceId == service.Id && a.Week == weekNo + "-" + objMonths.Year).FirstOrDefault();
                                        sb.Append(string.Format(",\"{0}\" : \"{1}\"", service.Value.ToString(), WeekCount == null ? "0" : WeekCount.ServiceCount.ToString()));
                                        break;
                                }
                                break;

                            case "4":
                                switch (Total)
                                {
                                    case "Sales":
                                        var DayCount = dayResult.Where(a => a.ServiceId == service.Id && a.Day == objMonths).FirstOrDefault();
                                        sb.Append(string.Format(",\"{0}\" : \"{1}\"", service.Value.ToString(), DayCount == null ? "0" : DayCount.ServiceCount.ToString()));
                                        break;
                                    case "Reimbursements":
                                        DayCount = dayResult.Where(a => a.ServiceId == service.Id && a.Day == objMonths).FirstOrDefault();
                                        sb.Append(string.Format(",\"{0}\" : \"{1}\"", service.Value.ToString(), DayCount == null ? "0" : DayCount.Reimbursements.ToString()));
                                        break;
                                    case "PaidAmount":
                                        DayCount = dayResult.Where(a => a.ServiceId == service.Id && a.Day == objMonths).FirstOrDefault();
                                        sb.Append(string.Format(",\"{0}\" : \"{1}\"", service.Value.ToString(), DayCount == null ? "0" : DayCount.Paid.HasValue ? DayCount.Paid.Value.ToString("F2") : "0"));
                                        break;
                                    case "Charges":
                                        DayCount = dayResult.Where(a => a.ServiceId == service.Id && a.Day == objMonths).FirstOrDefault();
                                        sb.Append(string.Format(",\"{0}\" : \"{1}\"", service.Value.ToString(), DayCount == null ? "0" : DayCount.Billed.HasValue ? DayCount.Billed.Value.ToString("F2") : "0"));
                                        break;
                                    default:
                                        DayCount = dayResult.Where(a => a.ServiceId == service.Id && a.Day == objMonths).FirstOrDefault();
                                        sb.Append(string.Format(",\"{0}\" : \"{1}\"", service.Value.ToString(), DayCount == null ? "0" : DayCount.ServiceCount.ToString()));
                                        break;
                                }
                                break;
                            default:
                                switch (Total)
                                {
                                    case "Sales":
                                        var monthCount = monthResult.Where(a => a.ServiceId == service.Id && a.Month == objMonths.Month + "-" + objMonths.Year).FirstOrDefault();
                                        sb.Append(string.Format(",\"{0}\" : \"{1}\"", service.Value.ToString(), monthCount == null ? "0" : monthCount.ServiceCount.ToString()));
                                        break;
                                    case "Reimbursements":
                                        monthCount = monthResult.Where(a => a.ServiceId == service.Id && a.Month == objMonths.Month + "-" + objMonths.Year).FirstOrDefault();
                                        sb.Append(string.Format(",\"{0}\" : \"{1}\"", service.Value.ToString(), monthCount == null ? "0" : monthCount.Reimbursements.ToString()));
                                        break;
                                    case "PaidAmount":
                                        monthCount = monthResult.Where(a => a.ServiceId == service.Id && a.Month == objMonths.Month + "-" + objMonths.Year).FirstOrDefault();
                                        sb.Append(string.Format(",\"{0}\" : \"{1}\"", service.Value.ToString(), monthCount == null ? "0" : monthCount.Paid.HasValue ? monthCount.Paid.Value.ToString("F2") : "0"));
                                        break;
                                    case "Charges":
                                        monthCount = monthResult.Where(a => a.ServiceId == service.Id && a.Month == objMonths.Month + "-" + objMonths.Year).FirstOrDefault();
                                        sb.Append(string.Format(",\"{0}\" : \"{1}\"", service.Value.ToString(), monthCount == null ? "0" : monthCount.Billed.HasValue ? monthCount.Billed.Value.ToString("F2") : "0"));
                                        break;
                                    default:
                                        monthCount = monthResult.Where(a => a.ServiceId == service.Id && a.Month == objMonths.Month + "-" + objMonths.Year).FirstOrDefault();
                                        sb.Append(string.Format(",\"{0}\" : \"{1}\"", service.Value.ToString(), monthCount == null ? "0" : monthCount.ServiceCount.ToString()));
                                        break;
                                }
                                break;
                        }
                    }
                    switch (ViewBy)
                    {
                        case "1":
                            objMonths = objMonths.AddYears(1);
                            objMonths = new DateTime(objMonths.Year, 1, 1);
                            break;

                        case "2":
                            objMonths = objMonths.AddMonths(1);
                            break;

                        case "3":
                            objMonths = objMonths.AddDays(7);
                            break;

                        case "4":
                            objMonths = objMonths.AddDays(1);
                            break;

                        default:
                            objMonths = objMonths.AddMonths(1);
                            break;
                    }
                    sb.Append("}");
                }
                sb.Append("]");
                dataColumns.Append("]");
                columnColors.Append("}");

                if (!string.IsNullOrEmpty(summaryColumns))
                {
                    // query.Sum(a=>a.)
                }

                var graphData = new GraphData { DataPoints = sb.ToString(), DataColumns = dataColumns.ToString().Replace(",]", "]"), ColumnColors = columnColors.ToString().Replace(",}", "}"), TotalSales = totalCount, SummaryTotals = new SummaryTotals { TotalBilled = totalBilled, TotalPaid = totalPaid } };
                response.Status = DataResponseStatus.OK;
                response.Model = graphData;
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


        public DataResponse<EntityDefaultDaterangeModel> GetDefaultDateRange(int? currentBusinessId)
        {
            var response = new DataResponse<EntityDefaultDaterangeModel>();
            try
            {
                base.DBInit();

                var query = DBEntity.BusinessMasters.Where(a => a.Id == currentBusinessId).Select(a => new EntityDefaultDaterangeModel
                {
                    DateRange = a.DateRange,
                });

                response = GetFirst<EntityDefaultDaterangeModel>(query);

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

        public DataResponse<EntityList<EntitySalesPeriodicDataTrends>> GetSalesCountByService(int businessId, int currentUserId, bool isBuzAdmin, bool IsRep, bool isSalesManager, bool isSalesDirector, int serviceId = 0, DateTime? DateFrom = null, DateTime? DateTo = null)
        {
            var response = new DataResponse<EntityList<EntitySalesPeriodicDataTrends>>();
            DateTime now = DateTime.Now;
            DateTime today = new DateTime(now.Year, now.Month, now.Day);

            DateTime yesterday = today.AddDays(-1);
            DateTime thisWeek = DateTime.UtcNow.AddDays(-(int)DateTime.Today.DayOfWeek);

            try
            {
                base.DBInit();
                int TodayCount = 0;
                int YesterdayCount = 0;
                int YesterdayBeforeCount = 0;
                int ThisWeekCount = 0;
                int LastWeekCount = 0;
                int LastWeekCountCompare = 0;
                int LastWeekBeforeCount = 0;
                int ThisMonthCount = 0;
                int LastMonthCount = 0;
                int LastMonthCountCompare = 0;
                int LastMonthBeforeCount = 0;
                double PercentageProgress = 0;
                int differenceCounts = 0;
                bool todayStatus, yesterDayStatus, thisMonthStatus, lastMonthStatus, thisWeekkStatus, lastWeekStatus;

                var targetDate = DateTime.UtcNow.AddMonths(-4);

                EntityList<EntitySalesPeriodicDataTrends> model = new EntityList<EntitySalesPeriodicDataTrends>();

                //giving this name because old query was not well named
                var queryEntity = DBEntity.ReportMasters
                    .Where(a => a.ServiceId == serviceId && a.SpecimenReceivedDate.HasValue && a.SpecimenReceivedDate >= targetDate);


                if (isSalesManager || isSalesDirector || IsRep)
                    queryEntity = queryEntity.Where(a => a.RepId == currentUserId || a.Rep.RepGroup.RepgroupManagerMappers.Any(g => g.ManagerId == currentUserId) ||
                        //a.Rep.RepGroup.SalesDirectorId == currentUserId ||
                        a.Rep.UserId == currentUserId);


                var query = queryEntity.Select(a => new { Date = EntityFunctions.TruncateTime(a.SpecimenReceivedDate.Value) })
                     .ToList();

                if (query != null)
                {
                    //Today Count
                    TodayCount = query.Count(a => a.Date == today); //query.Count(a => a.SpecimenReceivedDate.HasValue && EntityFunctions.TruncateTime(a.SpecimenReceivedDate.Value) == today.Date);

                    //Yesterday Count
                    YesterdayCount = query.Count(a => a.Date == yesterday.Date); //query.Count(a => a.SpecimenReceivedDate.HasValue && EntityFunctions.TruncateTime(a.SpecimenReceivedDate.Value) == yesterday);

                    //compare
                    todayStatus = TodayCount > YesterdayCount ? true : false;
                    //new-original/original*100

                    //percentage
                    PercentageProgress = GetPercentageValue(TodayCount, YesterdayCount);

                    model.List.Add(new EntitySalesPeriodicDataTrends
                    {
                        Period = "Today",
                        Count = TodayCount,
                        IsInProgress = todayStatus,
                        ProgressPercentage = PercentageProgress
                    });

                    //Yesterday Before Count
                    DateTime yesterdayBefore = today.AddDays(-2);
                    YesterdayBeforeCount = query.Count(a => a.Date == yesterdayBefore.Date); //query.Count(a => a.SpecimenReceivedDate.HasValue && EntityFunctions.TruncateTime(a.SpecimenReceivedDate.Value) == yesterdayBefore);

                    //percentage
                    PercentageProgress = GetPercentageValue(YesterdayCount, YesterdayBeforeCount);
                    //compare
                    yesterDayStatus = YesterdayCount > YesterdayBeforeCount ? true : false;

                    model.List.Add(new EntitySalesPeriodicDataTrends
                    {
                        Period = "Yesterday",
                        Count = YesterdayCount,
                        IsInProgress = yesterDayStatus,
                        ProgressPercentage = PercentageProgress
                    });
                    //This Week Count
                    DateTime startDateWeek = today.Date.AddDays(-(int)today.DayOfWeek);// prev sunday 00:00
                    ThisWeekCount = query.Count(a => a.Date >= startDateWeek && a.Date <= now);//query.Where(a => a.SpecimenReceivedDate.HasValue && EntityFunctions.TruncateTime(a.SpecimenReceivedDate.Value) >= startDateWeek && a.SpecimenReceivedDate.HasValue && EntityFunctions.TruncateTime(a.SpecimenReceivedDate.Value) <= today).Count();


                    //today minus 7 to get last week day
                    DateTime endLastDateWeekCompare = today.AddDays(-7);
                    DateTime lastWeekStartDate = endLastDateWeekCompare.Date.AddDays(-(int)endLastDateWeekCompare.DayOfWeek);

                    LastWeekCountCompare = query.Count(a => a.Date >= lastWeekStartDate && a.Date < endLastDateWeekCompare);


                    //percentage
                    PercentageProgress = GetPercentageValue(ThisWeekCount, LastWeekCountCompare);


                    //compare
                    thisWeekkStatus = ThisWeekCount > LastWeekCountCompare;

                    model.List.Add(new EntitySalesPeriodicDataTrends
                    {
                        Period = "Week-to-Date",
                        Count = ThisWeekCount,
                        IsInProgress = thisWeekkStatus,
                        ProgressPercentage = PercentageProgress
                    });

                    DateTime lastWeekEndDate = lastWeekStartDate.AddDays(7);
                    LastWeekCount = query.Count(a => a.Date >= lastWeekStartDate && a.Date <= lastWeekEndDate);

                    var twoWeekBackStartDate = lastWeekStartDate.AddDays(-7);
                    LastWeekBeforeCount = query.Count(a => a.Date >= twoWeekBackStartDate && a.Date < lastWeekStartDate);

                    PercentageProgress = GetPercentageValue(LastWeekCount, LastWeekBeforeCount);

                    //compare
                    lastWeekStatus = LastWeekCount > LastWeekBeforeCount ? true : false;


                    model.List.Add(new EntitySalesPeriodicDataTrends
                    {
                        Period = "Last Week",
                        Count = LastWeekCount,
                        IsInProgress = lastWeekStatus,
                        ProgressPercentage = PercentageProgress
                    });
                    int CurrentYear = today.Year;
                    int CurrentMonth = today.Month;

                    //This Month Count
                    DateTime startDateMonth = new DateTime(CurrentYear, CurrentMonth, 1);
                    ThisMonthCount = query.Count(a => a.Date >= startDateMonth && a.Date <= today);  // query.Where(a => a.SpecimenReceivedDate.HasValue && EntityFunctions.TruncateTime(a.SpecimenReceivedDate.Value) >= startDateMonth && a.SpecimenReceivedDate.HasValue && EntityFunctions.TruncateTime(a.SpecimenReceivedDate.Value) <= today).Count();


                    //thismonthtoday minus 1 to get last month day
                    DateTime endDateMonthLastCompare = today.AddMonths(-1);
                    int lastDateYear = endDateMonthLastCompare.Year;
                    int lastDateMonth = endDateMonthLastCompare.Month;
                    DateTime startDateMonthLast = new DateTime(lastDateYear, lastDateMonth, 1);
                    LastMonthCountCompare = query.Count(a => a.Date >= startDateMonthLast && a.Date < endDateMonthLastCompare); //query.Where(a => a.SpecimenReceivedDate.HasValue && EntityFunctions.TruncateTime(a.SpecimenReceivedDate.Value) >= startDateMonthLast && a.SpecimenReceivedDate.HasValue && EntityFunctions.TruncateTime(a.SpecimenReceivedDate.Value) < startDateMonth).Count();
                    var DaysInMonth = DateTime.DaysInMonth(lastDateYear, lastDateMonth);
                    var endDateMonthLast = new DateTime(lastDateYear, lastDateMonth, DaysInMonth);

                    //  DateTime endDateMonthLast = startDateMonthLast.AddMonths(1);
                    LastMonthCount = query.Count(a => a.Date >= startDateMonthLast && a.Date <= endDateMonthLast);
                    //percentage
                    PercentageProgress = GetPercentageValue(ThisMonthCount, LastMonthCountCompare);


                    //compare
                    thisMonthStatus = ThisMonthCount > LastMonthCount ? true : false;

                    model.List.Add(new EntitySalesPeriodicDataTrends
                    {
                        Period = "Month-to-Date",
                        Count = ThisMonthCount,
                        IsInProgress = thisMonthStatus,
                        ProgressPercentage = PercentageProgress
                    });


                    var twoMonthsBack = DateTime.UtcNow.AddMonths(-2);
                    var twoMonthBackStartDay = new DateTime(twoMonthsBack.Year, twoMonthsBack.Month, 1);
                    var twoMonthBackEndDay = new DateTime(twoMonthsBack.Year, twoMonthsBack.Month, DateTime.DaysInMonth(twoMonthBackStartDay.Year, twoMonthsBack.Month));

                    LastMonthBeforeCount = query.Count(a => a.Date >= twoMonthBackStartDay && a.Date <= twoMonthBackEndDay);
                    //percentage
                    PercentageProgress = GetPercentageValue(LastMonthCount, LastMonthBeforeCount);

                    //compare
                    lastMonthStatus = ThisMonthCount > LastMonthCount ? true : false;

                    model.List.Add(new EntitySalesPeriodicDataTrends
                    {
                        Period = "Last Month",
                        Count = LastMonthCount,
                        IsInProgress = lastMonthStatus,
                        ProgressPercentage = PercentageProgress
                    });
                }
                response.Model = model;

            }
            // }
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

        public double GetPercentageValue(int newCount, int oldCount)
        {
            try
            {
                double countPercentage = 0;
                int countDifference;
                if (newCount == 0 && oldCount == 0)
                {
                    countPercentage = 0;
                }
                else if (oldCount == 0)
                {
                    countPercentage = newCount * 100;
                }
                else if (newCount == 0)
                {
                    countPercentage = -100;
                }
                else
                {
                    countDifference = newCount - oldCount;
                    countPercentage = countDifference * 100 / oldCount;
                }
                return countPercentage;
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }
            return 0;
        }

        private static int GetMonthsDifference(DateTime from, DateTime to)
        {
            if (from > to) return GetMonthsDifference(to, from);

            var monthDiff = Math.Abs((to.Year * 12 + (to.Month - 1)) - (from.Year * 12 + (from.Month - 1)));

            if (from.AddMonths(monthDiff) > to || to.Day < from.Day)
            {
                return monthDiff - 0;
            }
            else
            {
                return monthDiff;
            }
        }

        private static int DaysBetween(DateTime d1, DateTime d2)
        {
            TimeSpan span = d2.Subtract(d1);
            return (int)span.TotalDays;
        }
    }

    public class GraphData
    {
        public string DataPoints { get; set; }

        public string DataColumns { get; set; }

        public string ColumnColors { get; set; }

        public int TotalSales { get; set; }

        public SummaryTotals SummaryTotals { get; set; }
    }

    public class SummaryTotals
    {
        public double TotalBilled { get; set; }

        public double TotalPaid { get; set; }
    }
}
