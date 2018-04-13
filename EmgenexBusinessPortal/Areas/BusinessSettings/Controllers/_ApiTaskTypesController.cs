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
using EBP.Business.Entity.TaskType;

namespace EmgenexBusinessPortal.Areas.BusinessSettings.Controllers
{
    [RoutePrefix("Settings/TaskTypes")]
    public class ApiTaskTypeController : ApiBaseController
    {
        protected int CurrentUserId { get { return 2; } }
        protected int? CurrentBusinessId { get { return 1; } }
        [HttpPost]
        [Route("getbyfilter")]
        [Route("")]
        public IHttpActionResult GetByFilter(FilterTaskTypes filter)
        {
            var repository = new RepositoryTaskTypes();
            if (filter == null)
                filter = new FilterTaskTypes { PageSize = 25, CurrentPage = 1 };
            var response = repository.GetTaskTypes(filter, CurrentBusinessId.Value);
            return Ok<DataResponse<EntityList<EntityTaskTypes>>>(response);
        }
        [Route("getTaskTypebyid/{Id}")]
        [Route("{Id}")]
        public IHttpActionResult GetTaskTypeById(int? Id)
        {
            var response = new DataResponse<EntityTaskTypes>();
            var repository = new RepositoryTaskTypes();
            if (Id.HasValue)
            {
                response = repository.GetTaskTypeById(Id.Value);
            }
            else
            {
                response.Model = new EntityTaskTypes();
            }
            return Ok<DataResponse>(response);
        }
        [HttpPost]
        [Route("Save")]
        public IHttpActionResult InsertTaskTypeData(EntityTaskTypes model)
        {
            var response = new DataResponse<EntityTaskTypes>();

            if (ModelState.IsValid)
            {
                model.UpdatedBy= model.CreatedBy = CurrentUserId;
                model.BusinessId = CurrentBusinessId.Value;
                if (model.Id > 0)
                {
                    response = new RepositoryTaskTypes().Update(model);
                }
                else
                {
                    response = new RepositoryTaskTypes().Insert(model);
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
        [Route("delete/{tasktypeid}")]
        [Route("{tasktypeid}/delete")]
        public IHttpActionResult Delete(int tasktypeid)
        {
            var repository = new RepositoryTaskTypes();
            var response = repository.Delete(tasktypeid);
            return Ok<DataResponse>(response);
        }
    }
}