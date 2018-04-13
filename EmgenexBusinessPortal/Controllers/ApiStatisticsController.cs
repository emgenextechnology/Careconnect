using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EBP.Business.Repository;
using EBP.Business;
using EBP.Business.Entity;
using EBP.Business.Filter;
using EBP.Business.Entity.Note;
using EmgenexBusinessPortal.Models;
using EBP.Business.Entity.DashBoard;
using System.IO;
using System.Web;
using System.Xml.Linq;

namespace EmgenexBusinessPortal.Controllers
{

    [RoutePrefix("stats")]
    [ApiAuthorize]
    public class ApiStatisticsController : ApiBaseController
    {
        private int ServiceId { get; set; }

        private string MapperFilePath
        {
            get
            {
                if (this.ServiceId <= 0)
                    return null;
                string serviceName = new RepositoryLookups().GetServiceNameById(this.ServiceId).Replace(" ", ""),
                    mapperFilePath = HttpContext.Current.Server.MapPath(Path.Combine("~/Assets", CurrentBusinessId.Value.ToString(), "Sales", serviceName, "ReportColumnMapper.xml"));

                if (!File.Exists(mapperFilePath))
                    return null;

                return mapperFilePath;
            }
        }
        
        [Route("all")]
        [HttpGet]
        public IHttpActionResult Index()
        {
            var repository = new RepositoryStatistics();
            var response = repository.GetAllStatistics(CurrentBusinessId.Value, CurrentUserId, IsBuzAdmin, IsRep || IsSalesManager, IsSalesManager, IsSalesDirector);
            return Ok<DataResponse<EntityStatistics>>(response);
        }

        [Route("SalesPerformance")]
        [HttpPost]
        public IHttpActionResult SalesPerformance(DateRangeModel DateRangeModel)
        {
            string[] allowedRoles = { "RDSLS" };
            string[] superRoles = { "RDSLSALL" };
            bool hasSuperRight = HasRight(superRoles);

            if (HasRight(allowedRoles) || hasSuperRight)
            {
                var repository = new RepositoryStatistics();
                var response = repository.GetAllSalesPerformance(CurrentBusinessId.Value, CurrentUserId, IsBuzAdmin, IsSalesDirector, IsSalesManager, IsRep, DateRangeModel.DateFrom, DateRangeModel.DateTo);
                return Ok<DataResponse<EntitySalesPerformance>>(response);
            }
            else
            {
                return Ok<DataResponse>(null);
            }
        }

        [Route("SalesPeriodicTrends/{serviceId}")]
        [HttpPost]
        public IHttpActionResult SalesPeriodicTrends(int serviceId, DateRangeModel DateRangeModel)
        {
            string[] allowedRoles = { "RDSLS" };
            string[] superRoles = { "RDSLSALL" };
            bool hasSuperRight = HasRight(superRoles);
            if (HasRight(allowedRoles) || hasSuperRight)
            {
                var repository = new RepositoryStatistics();

                //var response = repository.GetAllSalesPeriodicTrends(CurrentBusinessId.Value, CurrentUserId, IsBuzAdmin, IsRep, IsSalesManager, IsSalesDirector, serviceId, DateRangeModel.DateFrom, DateRangeModel.DateTo);
                var response = repository.GetAllSalesPeriodicTrends(CurrentBusinessId.Value, CurrentUserId, IsBuzAdmin, IsRep, IsSalesManager, IsSalesDirector, serviceId, DateRangeModel.DateFrom, DateRangeModel.DateTo, DateRangeModel.ViewBy, DateRangeModel.DateType, DateRangeModel.Total);
                return Ok<DataResponse<dynamic>>(response);
            }
            else
            {
                return Ok<DataResponse>(null);
            }
        }

        [HttpGet]
        [Route("getdashboardsalesdatetype/{serviceId}")]
        public IHttpActionResult GetDashboardSalesDateType(int serviceId=0)
        {
            var repository = new RepositoryStatistics();
            var response = new EntityList<EntitySelectItem>();
            if (serviceId != 0)
            {
                
                this.ServiceId = serviceId;
                if (this.MapperFilePath == null)
                {
                    var model = response.List = null;
                    string serviceName = new RepositoryLookups().GetServiceNameById(this.ServiceId).Replace(" ", "");
                    var error = string.Format("XML mapper file is missing for the service \"{0}\"", serviceName);
                    return Ok<dynamic>(new { Status = HttpStatusCode.BadRequest, model = model, Message = error });
                }
                repository.XmlMapper = XDocument.Load(this.MapperFilePath);
                var reportStaticColumns = repository.ReportStaticColumnsByAttribute("SummaryFilter");
                response.List = new List<EntitySelectItem>();
                foreach (var item in reportStaticColumns)
                {
                    response.List.Add(new EntitySelectItem { Value = item.ColumnName, Text = item.DisplayName });
                }
                return Ok(response);
            }
            else if (CurrentBusinessId == 6)
            {
                response.List.Add(new EntitySelectItem { Value = "ReceivedDate", Text = "Received Date", IsSelected=true });
                response.List.Add(new EntitySelectItem { Value = "ReportedDate", Text = "Reported Date" });
                response.List.Add(new EntitySelectItem { Value = "BilledDate", Text = "Billed Date" });
                response.List.Add(new EntitySelectItem { Value = "PaidDate", Text = "Paid Date" });
                              
                return Ok(response);

            }
            return Ok<DataResponse>(null);
        }

        [HttpGet]
        [Route("getdashboardsalestotal/{serviceId}")]
        public IHttpActionResult GetDashboardSalesTotal(int serviceId = 0)
        {
            var repository = new RepositoryStatistics();
            var response = new EntityList<EntitySelectItem>();
            if (serviceId != 0)
            {

                this.ServiceId = serviceId;
                if (this.MapperFilePath == null)
                {
                    var model = response.List = null;
                    string serviceName = new RepositoryLookups().GetServiceNameById(this.ServiceId).Replace(" ", "");
                    var error = string.Format("XML mapper file is missing for the service \"{0}\"", serviceName);
                    return Ok<dynamic>(new { Status = HttpStatusCode.BadRequest, model = model, Message = error });
                }
                repository.XmlMapper = XDocument.Load(this.MapperFilePath);
                var reportStaticColumns = repository.ReportStaticColumnsByAttribute("SummaryGroupBy");
                response.List = new List<EntitySelectItem>();
                response.List.Add(new EntitySelectItem { Text = "Sales", Value = "Sales" });
                foreach (var item in reportStaticColumns)
                {
                    response.List.Add(new EntitySelectItem { Value = item.ColumnName, Text = item.DisplayName });
                }
                return Ok(response);
            }
            else if (CurrentBusinessId == 6)
            {
                response.List.Add(new EntitySelectItem { Value = "BilledAmount", Text = "Billed Amount" });

                response.List.Add(new EntitySelectItem { Value = "PaidAmount", Text = "Paid Amount" });
            }
            response.List.Add(new EntitySelectItem { Text = "Sales", Value = "Sales", IsSelected = true });
            return Ok(response);
        }

        [Route("AccountHighlight")]
        [HttpGet]
        public IHttpActionResult AccountHighlight()
        {
            string[] allowedRoles = { "RDACNT" };
            string[] superRoles = { "RDACNTALL" };
            bool hasSuperRight = HasRight(superRoles);

            if (HasRight(allowedRoles) || hasSuperRight)
            {
                var repository = new RepositoryStatistics();
                var response = repository.GetAllAccountHighlight(CurrentBusinessId.Value, CurrentUserId, hasSuperRight, IsSalesManager, IsSalesDirector);
                return Ok<DataResponse<EntityAccountHighlight>>(response);
            }
            else
            {
                return Ok<DataResponse>(null);
            }
        }

        [Route("LeadSummary")]
        [HttpGet]
        public IHttpActionResult LeadSummary()
        {
            string[] allowedRoles = { "RDLD" };
            string[] superRoles = { "RDLDALL" };
            bool hasSuperRight = HasRight(superRoles);

            if (HasRight(allowedRoles) || hasSuperRight)
            {
                var repository = new RepositoryStatistics();
                var response = repository.GetAllLeadSummary(CurrentBusinessId.Value, CurrentUserId, hasSuperRight, IsSalesManager, IsSalesDirector);
                return Ok<DataResponse<EntityAccountHighlight>>(response);
            }
            else
            {
                return Ok<DataResponse>(null);
            }
        }

        [Route("getLead")]
        [HttpPost]
        public IHttpActionResult GetLead(DateRangeModel DateRangeModel)
        {
            string[] allowedRoles = { "RDLD" };
            string[] superRoles = { "RDLDALL" };
            bool hasSuperRight = HasRight(superRoles);

            if (HasRight(allowedRoles) || hasSuperRight)
            {
                var repository = new RepositoryStatistics();
                var response = repository.GetLeads(CurrentUser.BusinessId, CurrentUserId, hasSuperRight, IsSalesManager, IsSalesDirector, IsRep, DateRangeModel.DateFrom, DateRangeModel.DateTo);
                return Ok<DataResponse<EntityList<EntityLatestLead>>>(response);
            }
            else
            {
                return Ok<DataResponse>(null);
            }
        }

        [Route("getAccount")]
        [HttpPost]
        public IHttpActionResult GetAccount(DateRangeModel DateRangeModel)
        {
            string[] allowedRoles = { "RDACNT" };
            string[] superRoles = { "RDACNTALL" };
            bool hasSuperRight = HasRight(superRoles);

            if (HasRight(allowedRoles) || hasSuperRight)
            {
                var repository = new RepositoryStatistics();
                var response = repository.GetAccount(CurrentUser.BusinessId, CurrentUserId, hasSuperRight, IsSalesManager, IsSalesDirector, DateRangeModel.DateFrom, DateRangeModel.DateTo);
                return Ok<DataResponse<EntityList<EntityLatestAccount>>>(response);
            }
            else
            {
                return Ok<DataResponse>(null);
            }
        }

        [Route("TopAccounts/{serviceId}")]
        public IHttpActionResult TopAccount(DateRangeModel DateRangeModel, int serviceId = 0)
        {
            string[] allowedRoles = { "RDACNT" };
            string[] superRoles = { "RDACNTALL" };
            bool hasSuperRight = HasRight(superRoles);

            if (HasRight(allowedRoles) || hasSuperRight)
            {
                var repository = new RepositoryStatistics();
                var response = repository.TopAccount(serviceId, CurrentUser.BusinessId, CurrentUserId, hasSuperRight, IsSalesManager, IsSalesDirector, IsRep, DateRangeModel.DateFrom, DateRangeModel.DateTo);
                return Ok<DataResponse<EntityList<AccountSummary>>>(response);
            }
            else
            {
                return Ok<DataResponse>(null);
            }
        }

        [Route("LeadFunnel")]
        [HttpPost]
        public IHttpActionResult LeadFunnel(DateRangeModel DateRangeModel)
        {
            string[] allowedRoles = { "RDACNT" };
            string[] superRoles = { "RDACNTALL" };
            bool hasSuperRight = HasRight(superRoles);

            if (HasRight(allowedRoles) || hasSuperRight)
            {
                var repository = new RepositoryStatistics();
                var response = repository.GetLeadFunnel(CurrentBusinessId.Value, CurrentUserId, hasSuperRight, IsSalesManager, IsSalesDirector, DateRangeModel.DateFrom, DateRangeModel.DateTo);
                return Ok<DataResponse<EntityLeadFunnelModel>>(response);
            }
            else
            {
                return Ok<DataResponse>(null);
            }
        }

        [Route("TopReps/{serviceId}")]
        [HttpPost]
        public IHttpActionResult TopReps(DateRangeModel DateRangeModel, int serviceId = 0)
        {
            string[] allowedRoles = { "RDACNT" };
            string[] superRoles = { "RDACNTALL" };
            bool hasSuperRight = HasRight(superRoles);

            var response = new DataResponse<EntityList<TopRepSummary>>();
            if (IsRep && !IsSalesDirector && !IsSalesManager)
            {

                return Ok<DataResponse<EntityList<TopRepSummary>>>(response);
            }
            
            var repository = new RepositoryStatistics();
            response = repository.TopReps(serviceId, CurrentUser.BusinessId, CurrentUserId, hasSuperRight, IsSalesManager, IsSalesDirector, IsRep, DateRangeModel.DateFrom, DateRangeModel.DateTo);
            return Ok<DataResponse<EntityList<TopRepSummary>>>(response);
        }

        [Route("DefaultDateRange")]
        [HttpGet]
        public IHttpActionResult DefaultDateRange()
        {
            var response = new DataResponse<EntityDefaultDaterangeModel>();
            var repository = new RepositoryStatistics();

            response = repository.GetDefaultDateRange(CurrentBusinessId);

            return Ok<DataResponse>(response);
        }
        
        [Route("PeriodicDataTrends/{serviceId}")]
        [HttpPost]
        public IHttpActionResult PeriodicDataTrends(int serviceId)
        {
            string[] allowedRoles = { "RDSLS" };
            string[] superRoles = { "RDSLSALL" };
            bool hasSuperRight = HasRight(superRoles);
            var response = new DataResponse<EntityList<EntitySalesPeriodicDataTrends>>();
            if (HasRight(allowedRoles) || hasSuperRight)
            {
                var repository = new RepositoryStatistics();
                response = repository.GetSalesCountByService(CurrentBusinessId.Value, CurrentUserId, IsBuzAdmin, IsRep, IsSalesManager, IsSalesDirector, serviceId);
                response.Status = DataResponseStatus.OK;
                response.Message = "Success";
                return Ok<DataResponse<EntityList<EntitySalesPeriodicDataTrends>>>(response);
            }
            else
            {
                return Ok<DataResponse>(null);
            }
        }
    }
}