using EBP.Api.Models;
using EBP.Business;
using EBP.Business.Entity;
using EBP.Business.Entity.Practice;
using EBP.Business.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace EBP.Api.Controllers
{
    [RoutePrefix("Practice")]
    [ApiAuthorize]
    public class ApiPracticeController : ApiBaseController
    {

        [Route("All")]
        [HttpPost]
        public IHttpActionResult GetByFilter(EntityPractice entity)
        {
            var repository = new RepositoryPractice();
            var response = repository.GetAllPractices(entity.SpecialityTypeId, entity.KeyWord, CurrentBusinessId, CurrentUserId, CurrentUser.Roles.Contains("BusinessAdmin"));
            return Ok<DataResponse<EntityList<EntitySelectItem>>>(response);
        }

        [HttpPost]
        [Route("FlagIsActivestatuses")]
        public IHttpActionResult SetAccountStatuses(StatusManagementModel model)
        {
            var repository = new RepositoryPractice();
            var response = repository.SetApiActiveFlagStatus(model.Id, model.IsLead, model.IsActive, model.Flag);

            response.Status = DataResponseStatus.OK;
            return Ok<DataResponse>(response);
        }
    }
}