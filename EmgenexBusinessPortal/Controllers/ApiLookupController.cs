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
using EBP.Business.Enums;
using System.Text.RegularExpressions;

namespace EmgenexBusinessPortal.Controllers
{
    [RoutePrefix("lookup")]
    [Authorize]
    [ApiAuthorize]
    public class ApiLookupController : ApiBaseController
    {
        [HttpGet]
        [Route("getallspecialities")]
        public IHttpActionResult GetPracticeSpecialities()
        {
            var repository = new RepositoryLookups();
            var response = repository.GetPractiesSpecialities();
            //if (response.Status == DataResponseStatus.OK)
            //{

            //}
            return Ok<DataResponse<EntityList<EntitySelectItem>>>(response);
        }

        [HttpGet]
        [Route("getallpracticetypes")]
        public IHttpActionResult GetPracticeTypes()
        {
            var repository = new RepositoryLookups();
            var response = repository.GetPracticeTypes();
            return Ok<DataResponse<EntityList<EntitySelectItem>>>(response);
        }

        [HttpGet]
        [Route("getallstates")]
        public IHttpActionResult GetAllStates()
        {
            var repository = new RepositoryLookups();
            var response = repository.GetAllStates();
            return Ok<DataResponse<EntityList<EntitySelectItem>>>(response);
        }       

        [HttpGet]
        [Route("getalldegrees")]
        public IHttpActionResult GetAllDegrees()
        {
            var repository = new RepositoryLookups();
            var response = repository.GetAllDegrees();
            return Ok<DataResponse<EntityList<EntitySelectItem>>>(response);
        }

        [HttpGet]
        [Route("getallrepgroups")]
        [ApiAuthorize]
        public IHttpActionResult GetAllRepGroups()
        {
            string[] privileges = { "RDREPGRPALL" };
            if ((!IsBuzAdmin && !IsSalesManager) && (IsRep || !HasRight(privileges)))
                return Ok<DataResponse>(new DataResponse { Message = "Access denied!", Status = DataResponseStatus.BadRequest });

            var repository = new RepositoryLookups();
            var response = repository.GetAllRepGroups(CurrentUser.BusinessId, CurrentUserId, IsBuzAdmin, IsSalesManager, IsSalesDirector);
            return Ok<DataResponse<EntityList<EntitySelectItem>>>(response);
        }

        [HttpPost]
        [Route("getallreps")]
        [ApiAuthorize]
        public IHttpActionResult GetAllReps(int?[] repGroupIds)
        {
            string[] privileges = { "RDREPALL" };
            if ((!IsBuzAdmin && !IsSalesManager) && (IsRep || !HasRight(privileges)))
                return Ok<DataResponse>(new DataResponse { Message = "Access denied!", Status = DataResponseStatus.BadRequest });

            var repository = new RepositoryLookups();
            DataResponse<EntityList<EntitySelectItem>> response = new DataResponse<EntityList<EntitySelectItem>>();
            if (repGroupIds == null)
            {
                response = repository.GetAllReps(CurrentBusinessId, CurrentUserId, IsBuzAdmin, IsSalesManager, IsSalesDirector);
            }
            else
            {
                response = repository.GetAllRepsByGroupId(repGroupIds, CurrentBusinessId, CurrentUserId, IsSalesManager, IsSalesDirector);
            }
            return Ok<DataResponse<EntityList<EntitySelectItem>>>(response);
        }

        [HttpPost]
        [Route("getallrepsbysamegroup")]
        [ApiAuthorize]
        public IHttpActionResult GetAllRepsBySameGroup(int?[] repGroupIds)
        {
            var repository = new RepositoryLookups();
            DataResponse<EntityList<EntitySelectItem>> response = new DataResponse<EntityList<EntitySelectItem>>();
            response = repository.GetAllRepsByGroupId(repGroupIds, CurrentUser.BusinessId, CurrentUser.Id, !CurrentUser.Roles.Contains("BusinessAdmin"), IsSalesDirector);
            return Ok<DataResponse<EntityList<EntitySelectItem>>>(response);
        }

        [HttpPost]
        [Route("getalltaskusers")]
        [ApiAuthorize]
        public IHttpActionResult GetAllTaskUsers(int?[] repGroupIds)
        {
            var repository = new RepositoryLookups();
            DataResponse<EntityList<EntitySelectItem>> response = new DataResponse<EntityList<EntitySelectItem>>();
            response = repository.GetAllTaskUsers(CurrentUser.BusinessId, CurrentUser.Id, IsRep, IsSalesManager, IsBuzAdmin, IsSalesDirector);
            return Ok<DataResponse<EntityList<EntitySelectItem>>>(response);
        }

        [HttpGet]
        [Route("getallleadsources")]
        public IHttpActionResult GetAllLeadSources()
        {
            var repository = new RepositoryLookups();
            var response = repository.GetAllLeadSources();
            return Ok<DataResponse<EntityList<EntitySelectItem>>>(response);
        }

        [HttpGet]
        [Route("getallservices")]
        public IHttpActionResult GetAllServicesByBusiness(int repId = 0)
        {
            var repository = new RepositoryLookups();
            var response = repository.GetAllServices(CurrentBusinessId.Value, CurrentUserId, IsSalesManager, IsSalesDirector, IsRep, IsBuzAdmin);
            return Ok<DataResponse<EntityList<EntitySelectItem>>>(response);
        }

        [HttpGet]
        [Route("getallrepservices")]
        public IHttpActionResult GetAllServicesByReps()
        {
            var repository = new RepositoryLookups();
            var response = repository.GetAllRepServices(CurrentBusinessId.Value);
            return Ok<DataResponse<EntityList<EntitySelectItem>>>(response);
        }

        [HttpGet]
        [Route("getallproviders")]
        [Route("getallproviders/{searchKey}")]
        public IHttpActionResult GetAllProviders(string searchKey = "")
        {
            var repository = new RepositoryLookups();
            var response = repository.GetAllProviders(CurrentBusinessId, CurrentUserId, IsSalesManager, IsSalesDirector, IsRep, searchKey);
            return Ok<DataResponse<EntityList<EntitySelectItem>>>(response);
        }

        [HttpGet]
        [Route("getallmarketingcategories")]
        public IHttpActionResult GetAllmarketingCategories()
        {
            var repository = new RepositoryLookups();
            var response = repository.GetAllmarketingCategories(CurrentBusinessId.Value);
            return Ok<DataResponse<EntityList<EntitySelectItem>>>(response);
        }

        [HttpGet]
        [Route("getalldocumenttypes")]
        public IHttpActionResult GetAllDocumentTypes()
        {
            var repository = new RepositoryLookups();
            var response = repository.GetAllDocumentTypes(CurrentBusinessId.Value);
            return Ok<DataResponse<EntityList<EntitySelectItem>>>(response);
        }
        [HttpGet]
        [Route("getallusersbybusinessId")]
        public IHttpActionResult GetAllUsersByBusinessId()
        {
            var repository = new RepositoryLookups();
            var response = repository.GetAllUsersByBusinessId(CurrentBusinessId.Value);
            return Ok<DataResponse<EntityList<EntitySelectItem>>>(response);
        }

        //[HttpGet]
        //[Route("getallnpi")]
        //public IHttpActionResult GetAllNPIs()
        //{
        //    var repository = new RepositoryLookups();
        //    var response = repository.GetAllNPIs();
        //    return Ok<DataResponse<EntityList<EntitySelectItem>>>(response);
        //}
        /*
        [HttpGet]
        [Route("getproviders/{serviceid}")]
        public IHttpActionResult GetProvidersByServiceId(int serviceid)
        {
            var repository = new RepositoryLookups();
            var response = repository.GetProvidersByServiceId(serviceid, CurrentBusinessId);
            return Ok<DataResponse<EntityList<EntitySelectItem>>>(response);
        }
        */
        [HttpGet]
        [Route("getallleadstatus")]
        public IHttpActionResult GetAllLeadStatus()
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            var model = EnumHelper.GetEnumList(typeof(LeadStatus));

            response.Model = model;

            return Ok(response);
        }

        [HttpGet]
        [Route("getallfilterstatus")]

        public IHttpActionResult GetAllFilterStatus()
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            var model = EnumHelper.GetEnumList(typeof(FilterStatus));

            response.Model = model;
            return Ok(response);
        }

        [HttpGet]
        [Route("getallperiods")]
        public IHttpActionResult GetAllPeriods()
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            var model = new EntityList<EntitySelectItem>();
            #region test
            //        new KeyValuePair().
            //        model = ((IEnumerable<EntitySelectItem>)Enum.GetValues(typeof(Periods))).Select(c => new EntitySelectItem() { Id = (int)c.Id, Value = c.ToString() }).ToList();
            //   var s=     Enum.GetValues(typeof(Periods))
            //.Cast<Periods>()
            //.Select(v => v.ToString())
            //.ToList();

            //        var qatype = typeof(Periods);
            //        var names = qatype.GetEnumNames();
            //        var values = qatype.GetEnumValues().Cast<int>().ToList();
            //        var nameValues = names.Select(n => qatype.GetMember(n)[0].CustomAttributes.First().NamedArguments[0].TypedValue.Value).ToList();
            //        var valuesList = names.Select((n, index) =>new { key = n, value = values[index] }).ToList();
            //        var nameValuesList = names.Select((n, index) =>new EntitySelectItem { Id = int.Parse(n), Value = nameValues[index] }).ToList();
            //        model.List = nameValuesList;
            #endregion

            model.List = new List<EntitySelectItem>();

            foreach (Periods period in Enum.GetValues(typeof(Periods)))
            {
                string _period = Regex.Replace(period.ToString(), "([a-z])([A-Z])", @"$1 $2"); ;
                model.List.Add(new EntitySelectItem { Id = (int)period, Value = _period });
            }

            //model.List = new List<EntitySelectItem> { 
            //    new EntitySelectItem { Id = (int)Periods.Today, Value = "Today" },
            //    new EntitySelectItem { Id = (int)Periods.Yesterday, Value = "Yesterday" },
            //    new EntitySelectItem { Id = (int)Periods.OneWeek, Value = "One Week" },
            //    new EntitySelectItem { Id = (int)Periods.OneMonth, Value = "1 Month" },
            //    new EntitySelectItem { Id = (int)Periods.TwoMonth, Value = "2 Month" },
            //    new EntitySelectItem { Id = (int)Periods.ThreeMonth, Value = "3 Month" },
            //    new EntitySelectItem { Id = (int)Periods.FourMonth, Value = "4 Month" }
            //};
            response.Model = model;

            return Ok(response);
        }

        [HttpGet]
        [Route("getallDashboardperiods")]
        public IHttpActionResult GetAllDashboardPeriods()
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            var model = new EntityList<EntitySelectItem>();

            model.List = new List<EntitySelectItem>();

            foreach (DashboardPeriods period in Enum.GetValues(typeof(DashboardPeriods)))
            {
                string _period = Regex.Replace(period.ToString(), "([A-Z]{1,2}|[0-9]+)", " $1").TrimStart();//Regex.Replace(period.ToString(), "([a-z])([0-9])", @"$1 $2");
                model.List.Add(new EntitySelectItem { Id = (int)period, Value = _period });
            }

            model.List[0].IsSelected = true;
            response.Model = model;
            return Ok(response);

            //var response = new DataResponse<EntityList<EntitySelectItem>>();
            //var model = EnumHelper.GetEnumList(typeof(DashboardPeriods));
            //response.Model = model;

            //return Ok(response);
        }

        [HttpGet]
        [Route("getdashboardsalesviewby")]
        public IHttpActionResult GetDashboardSalesViewBy()
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            var model = EnumHelper.GetEnumList(typeof(DashboardSalesViewBy));
            response.Model = model;
            return Ok(response);
        }

        [HttpGet]
        [Route("getdashboardsalesdatetype")]
        public IHttpActionResult GetDashboardSalesDateType()
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            var model = EnumHelper.GetEnumList(typeof(DashboardSalesDateType));
            response.Model = model;
            return Ok(response);
        }

        [HttpGet]
        [Route("getdashboardsalestotal")]
        public IHttpActionResult GetDashboardSalesTotal()
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            var model = EnumHelper.GetEnumList(typeof(DashboardSalesTotal));
            response.Model = model;
            return Ok(response);
        }

        [HttpGet]
        [Route("getalltasktypes")]
        public IHttpActionResult GetAllTaskTypes()
        {
            var repository = new RepositoryLookups();
            var response = repository.GetAllTaskTypes(CurrentBusinessId);
            return Ok<DataResponse<EntityList<EntitySelectItem>>>(response);
        }

        [HttpGet]
        [Route("getalltaskstatuses")]
        public IHttpActionResult GetAllTaskStatuses()
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            var model = EnumHelper.GetEnumList(typeof(TaskStatuses));
            response.Model = model;

            return Ok(response);
        }

        [HttpGet]
        [Route("getalltaskpriorities")]
        public IHttpActionResult GetAllTaskPriorities()
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            var model = EnumHelper.GetEnumList(typeof(TaskPriorities));
            response.Model = model;

            return Ok(response);
        }
        
        [HttpGet]
        [Route("getallSalesGroupbyValues")]
        public IHttpActionResult GetAllSalesGroupbyValues()
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            var model = EnumHelper.GetEnumList(typeof(SalesGroupBy));
            response.Model = model;

            return Ok(response);
        }

        [HttpGet]
        [Route("getalllookups/{serviceid}")]
        public IHttpActionResult GetAllTaskPriorities(int serviceid)
        {
            var repository = new RepositoryLookups();

            var lookupSales = new LookupModels();
            lookupSales.Providers = repository.GetAllProviders(CurrentBusinessId, CurrentUserId, IsSalesManager, IsSalesDirector, IsRep).Model;
            if (!IsRep)
            {
                lookupSales.Groups = repository.GetAllRepGroups(CurrentBusinessId, CurrentUserId, IsBuzAdmin, IsSalesManager, IsSalesDirector).Model;
                lookupSales.Reps = repository.GetAllReps(CurrentBusinessId, CurrentUserId, IsBuzAdmin, IsSalesManager, IsSalesDirector).Model;
            }

            var response = new DataResponse<LookupModels>();
            response.Model = lookupSales;

            return Ok<DataResponse<LookupModels>>(response);
        }

        [HttpGet]
        [Route("getallimportmodes")]

        public IHttpActionResult GetAllImportMode()
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            var model = EnumHelper.GetEnumList(typeof(ServiceReportImportModes));

            response.Model = model;
            return Ok(response);
        }

        [HttpGet]
        [Route("getallprotocols")]
        public IHttpActionResult GetAllProtocols()
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            var model = EnumHelper.GetEnumList(typeof(FTPProtocol));

            response.Model = model;
            return Ok(response);
        }

        [HttpGet]
        [Route("getalldirectors")]
        public IHttpActionResult GetAllDirectors()
        {

            DataResponse<EntityList<EntitySelectItem>> response = new DataResponse<EntityList<EntitySelectItem>>();
            RepositoryLookups repositoryLookup = new RepositoryLookups();
            response = repositoryLookup.GetAllDirectors(CurrentBusinessId);
            return Ok<DataResponse<EntityList<EntitySelectItem>>>(response);
        }

        [Route("getallmanagers")]
        public IHttpActionResult GetAllManagers()
        {

            DataResponse<EntityList<EntitySelectItem>> response = new DataResponse<EntityList<EntitySelectItem>>();
            RepositoryLookups repositoryLookup = new RepositoryLookups();
            response = repositoryLookup.GetAllMangers(CurrentBusinessId);
            return Ok<DataResponse<EntityList<EntitySelectItem>>>(response);
        }

        [Route("getallcountries")]
        public IHttpActionResult GetAllCountries()
        {
            var repository = new RepositoryLookups();
            var response = repository.GetAllCountriess();
            return Ok<DataResponse<EntityList<EntitySelectItem>>>(response);
        }

        [HttpGet]
        [Route("getallusersforrep/{repUserId}")]
        public IHttpActionResult GetAllUsersForRep(int repUserId = 0)
        {
            var repository = new RepositoryLookups();
            var response = repository.GetAllUsersForRep(CurrentBusinessId.Value,CurrentUserId, repUserId);
            return Ok<DataResponse<EntityList<EntitySelectItem>>>(response);
        }

        [HttpGet]
        [Route("getallusersforservicecolumn")]
        public IHttpActionResult GetAllUsersForServiceColumn()
        {
            var repository = new RepositoryLookups();
            var response = repository.GetAllUsersForRep(CurrentBusinessId.Value, CurrentUserId);
            return Ok<DataResponse<EntityList<EntitySelectItem>>>(response);
        }
        [Route("getallprivileges")]
        public IHttpActionResult GetAllPrivileges()
        {
            var repository = new RepositoryLookups();
            var response = repository.GetAllPrivileges();
            return Ok<DataResponse<EntityList<PrivilegeModulesmodel>>>(response);
        }
        [HttpGet]
        [Route("getallroles")]
        public IHttpActionResult GetAllRoles()
        {
            var repository = new RepositoryLookups();
            var response = repository.GetAllRoles(CurrentBusinessId);
            return Ok<DataResponse<EntityList<EntitySelectItem>>>(response);
        }

        [HttpGet]
        [Route("getalldepartments")]
        public IHttpActionResult GetAllDepartments()
        {
            var repository = new RepositoryLookups();
            var response = repository.GetAllDepartments(CurrentBusinessId);
            return Ok<DataResponse<EntityList<EntitySelectItem>>>(response);
        }

        [HttpGet]
        [Route("getallsalescolumntype")]
        public IHttpActionResult GetAllSalesColumnType()
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            var model = EnumHelper.GetEnumList(typeof(SalesColumnType));

            response.Model = model;
            return Ok(response);
        }

        [HttpGet]
        [Route("getallsalesinputtype")]
        public IHttpActionResult GetAllSalesInputType()
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            var model = EnumHelper.GetEnumList(typeof(SalesInputType));

            response.Model = model;
            return Ok(response);
        }
        [HttpGet]
        [Route("getallmanagersbyrepgroupid/{repgroupid}")]
        public IHttpActionResult GetAllManagersByRepgroupId(int repgroupid)
        {
            var repository = new RepositoryLookups();
            var response = repository.GetAllManagersByRepGroupId(repgroupid);
            return Ok (response);
        }
    }

    public class LookupModels
    {
        public EntityList<EntitySelectItem> Providers { get; set; }

        public EntityList<EntitySelectItem> Groups { get; set; }

        public EntityList<EntitySelectItem> Reps { get; set; }
    }
}