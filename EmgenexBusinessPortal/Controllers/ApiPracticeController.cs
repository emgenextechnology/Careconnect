using EBP.Business;
using EBP.Business.Entity;
using EBP.Business.Entity.Practice;
using EBP.Business.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace EmgenexBusinessPortal.Controllers
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
            string[] privileges = { "RDPRCTALL" };
            var response = repository.GetAllPractices(entity.SpecialityTypeId, entity.KeyWord, CurrentBusinessId, CurrentUserId, IsBuzAdmin || HasRight(privileges));
            return Ok<DataResponse<EntityList<EntitySelectItem>>>(response);
        }
    }
}