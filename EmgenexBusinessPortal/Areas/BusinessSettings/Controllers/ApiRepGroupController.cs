using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using EBP.Business.Database;
using EBP.Business.Filter;
using EBP.Business;
using EBP.Business.Entity;
using EmgenexBusinessPortal.Controllers;
using EBP.Business.Repository;
using EBP.Business.Entity.RepGroups;

namespace EmgenexBusinessPortal.Areas.BusinessSettings.Controllers
{
     [RoutePrefix("RepGroups")]
    public class ApiRepGroupController : ApiBaseController
     {
        protected int CurrentUserId { get { return 2; } }
        protected int? CurrentBusinessId { get { return 1; } }
         [HttpPost]
         [Route("getbyfilter")]
         [Route("")]
         public IHttpActionResult GetByFilter(FilterRepGroups filter)
         {
             var repository = new RepositoryRepGroups();
                 if (filter == null)
                     filter = new FilterRepGroups { PageSize = 25, CurrentPage = 1 };
                 var response = repository.GetRepGroups(filter, CurrentBusinessId.Value);
                 return Ok<DataResponse<EntityList<EntityRepGroups>>>(response);
         }
         [Route("getRepGroupbyid/{Id}")]
         [Route("{Id}")]
         public IHttpActionResult GetRepGroupById(int? Id)
         {
             var response = new DataResponse<EntityRepGroups>();
             var repository = new RepositoryRepGroups();
             if (Id.HasValue)
             {
                 response = repository.GetRepGroupById(Id.Value);
             }
             else
             {
                 response.Model = new EntityRepGroups();
             }
             return Ok<DataResponse>(response);
         }
         [HttpPost]
         [Route("Save")]
         public IHttpActionResult InsertRepgroupData(EntityRepGroups model)
         {
             var response = new DataResponse<EntityRepGroups>();

             if (ModelState.IsValid)
             {
                 model.UpdatedBy = model.CreatedBy  = CurrentUserId;
                 model.BusinessId = CurrentBusinessId;
                 if (model.Id > 0)
                 {
                     response = new RepositoryRepGroups().Update(model);
                 }
                 else
                 {
                     response = new RepositoryRepGroups().Insert(model);
                 }
                 return Ok<DataResponse>(response);
             }
             else
             {
                 var errorList = ModelState.Where(a => a.Value.Errors.Any()).Select(s => new
                 {
                     Key = s.Key.Split('.').Last(),
                     Message = s.Value.Errors[0].ErrorMessage
                 });
                 return Ok<dynamic>(new { Status = HttpStatusCode.BadRequest, Model = errorList });
             }
         }
         [HttpPost]
         [Route("delete/{repgroupid}")]
         [Route("{repgroupid}/delete")]
         public IHttpActionResult Delete(int repgroupid)
         {
             var repository = new RepositoryRepGroups();
             var response = repository.Delete(repgroupid);
             return Ok<DataResponse>(response);
         }
    }
}