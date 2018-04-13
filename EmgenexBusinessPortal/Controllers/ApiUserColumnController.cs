using EBP.Business;
using EBP.Business.Entity;
using EBP.Business.Entity.UserColumn;
using EBP.Business.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;

namespace EmgenexBusinessPortal.Controllers
{
    [RoutePrefix("ColumnVisibility")]
    [ApiAuthorize]
    public class ApiUserColumnController : ApiBaseController
    {

        [Route("all/{moduleId}/{ServiceId}")]
        public IHttpActionResult GetAllColumns(int moduleId, int? ServiceId)
        {
            var repository = new RepositoryColumnVisibility();
            var response = repository.GetAllColumns(moduleId, CurrentBusinessId, CurrentUserId, ServiceId);
            return Ok<DataResponse<EntityList<EntityUserColumn>>>(response);
        }

        //[Route("save")]
        //[HttpPost]
        //public IHttpActionResult GetAllColumns(EntityUserColumn EntityColumn)
        //{
        //    var repository = new RepositoryColumnVisibility();

        //    EntityColumn.CreatedBy = EntityColumn.UserId = CurrentUserId;
        //    EntityColumn.BusinessId = CurrentBusinessId;

        //    bool isSuccess = repository.SaveColumns(EntityColumn);

        //    if (isSuccess)
        //        return Ok<dynamic>(new { Status = HttpStatusCode.Created });

        //    return Ok<dynamic>(new { Status = HttpStatusCode.BadRequest });
        //}
    }
}