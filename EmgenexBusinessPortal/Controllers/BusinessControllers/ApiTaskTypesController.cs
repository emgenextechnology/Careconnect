using EBP.Business;
using EBP.Business.Entity;
using EBP.Business.Entity.DocumentType;
using EBP.Business.Entity.TaskType;
using EBP.Business.Filter;
using EBP.Business.Repository;
using EmgenexBusinessPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Linq.Dynamic;

namespace EmgenexBusinessPortal.Controllers
{
    [ApiAuthorize]
    [RoutePrefix("api/tasktype")]
    public class ApiTaskTypesController : ApiBaseController
    {
        RepositoryTaskTypes repository = new RepositoryTaskTypes();

        [HttpPost]
        [Route("")]
        public IHttpActionResult GetAllTaskTypes(FilterTaskTypes filter)
        {
            if (filter == null)
            {
                filter = new FilterTaskTypes();
                filter.PageSize = 25;
                filter.CurrentPage = 1;
            }
            var response = repository.GetTaskTypes(filter, CurrentBusinessId.Value);
            return Ok<DataResponse<EntityList<EntityTaskTypes>>>(response);
        }

        [HttpPost]
        [Route("save")]
        public IHttpActionResult CreateTaskType(TaskTypeModel model)
        {
            var response = new DataResponse<EntityTaskTypes>();

            if (ModelState.IsValid)
            {
                var entityDocumentTypes = new EntityTaskTypes();
                entityDocumentTypes.TaskType = model.TaskType;
                entityDocumentTypes.BusinessId = CurrentBusinessId.Value;
                entityDocumentTypes.CreatedBy = CurrentUserId;
                entityDocumentTypes.CreatedOn = DateTime.UtcNow;
                response = repository.Insert(entityDocumentTypes);
            }
            return Ok<DataResponse>(response);
        }

        [Route("{id}")]
        public IHttpActionResult GetTaskTypeById(int id)
        {
            var response = new DataResponse<EntityTaskTypes>();
            if (id != 0)
            {
                response = repository.GetTaskTypeById(id);

                return Ok<DataResponse>(response);
            }
            else
            {
                return null;
            }
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult UpdateTaskType(TaskTypeModel model)
        {
            var response = new DataResponse<EntityTaskTypes>();

            if (ModelState.IsValid)
            {
                var entityDocumentTypes = new EntityTaskTypes();
                entityDocumentTypes.Id = model.Id;
                entityDocumentTypes.TaskType = model.TaskType;
                entityDocumentTypes.UpdatedBy = CurrentUserId;
                entityDocumentTypes.UpdatedOn = System.DateTime.UtcNow;

                response = repository.Update(entityDocumentTypes);
            }

            return Ok<DataResponse>(response);
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeleteTaskType(int id)
        {
            var response = new DataResponse<bool>();
            response = repository.Delete(id);
            return Ok<DataResponse>(response);
        }
    }
}
