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
using EBP.Api.Models;
using EBP.Business.Entity.DashBoard;

namespace EBP.Api.Controllers
{
    [RoutePrefix("Statistics")]
    [ApiAuthorize]
    public class ApiStatisticsController : ApiBaseController
    {
        [Route("all")]
        [HttpGet]
        public IHttpActionResult Index()
        {
            var repository = new RepositoryStatistics();
            var response = repository.GetAllStatistics(CurrentBusinessId.Value, CurrentUserId, IsBuzAdmin, IsRep || IsSalesManager, IsSalesManager, IsSalesDirector);
            return Ok<DataResponse<EntityStatistics>>(response);
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

    }

}