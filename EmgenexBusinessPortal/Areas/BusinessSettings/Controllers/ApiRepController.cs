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
using EBP.Business.Entity.Rep;

namespace EmgenexBusinessPortal.Areas.BusinessSettings.Controllers
{
     [RoutePrefix("Reps")]
    public class ApiRepController : ApiBaseController
     {
        protected int CurrentUserId { get { return 2; } }
        protected int? CurrentBusinessId { get { return 1; } }
         [HttpPost]
         [Route("getbyfilter")]
         [Route("")]
         public IHttpActionResult GetByFilter(FilterReps filter)
         {
             var repository = new RepositoryReps();
                 if (filter == null)
                     filter = new FilterReps { PageSize = 25, CurrentPage = 1 };
                 var response = repository.GetReps(filter, CurrentBusinessId.Value);
                 return Ok<DataResponse<EntityList<EntityReps>>>(response);
         }
         [Route("getRepbyid/{Id}")]
         [Route("{Id}")]
         public IHttpActionResult GetRepById(int? Id)
         {
             var response = new DataResponse<EntityReps>();
             var repository = new RepositoryReps();
             if (Id.HasValue)
             {
                 response = repository.GetRepById(Id.Value);
             }
             else
             {
                 response.Model = new EntityReps();
             }
             return Ok<DataResponse>(response);
         }
         [HttpPost]
         [Route("Save")]
         public IHttpActionResult InsertRepData(EntityReps model)
         {
             var response = new DataResponse<EntityReps>();

             if (ModelState.IsValid)
             {
                 model.UpdatedBy = model.CreatedBy  = CurrentUserId;
                 if (model.Id > 0)
                 {
                     response = new RepositoryReps().Update(model);
                 }
                 else
                 {
                     response = new RepositoryReps().Insert(model);
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
         [Route("delete/{repid}")]
         [Route("{repid}/delete")]
         public IHttpActionResult Delete(int repid)
         {
             var repository = new RepositoryReps();
             var response = repository.Delete(repid);
             return Ok<DataResponse>(response);
         }
    }
}