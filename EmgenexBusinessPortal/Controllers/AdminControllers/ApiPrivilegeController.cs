using EBP.Business;
using EBP.Business.Entity;
using EBP.Business.Entity.PrivilegeModules;
using EBP.Business.Entity.Privileges;
using EBP.Business.Filter;
using EBP.Business.Repository;
using EmgenexBusinessPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EmgenexBusinessPortal.Controllers
{
    [ApiAuthorize]
    [RoutePrefix("api/privilege")]
    public class ApiPrivilegeController : ApiBaseController
    {
        RepositoryPrivilege repository = new RepositoryPrivilege();

        [HttpPost]
        [Route("")]
        public IHttpActionResult GetByFilter(FilterPrivilege filter)
        {
            if (filter == null)
            {
                filter = new FilterPrivilege();
                filter.PageSize = 25;
                filter.CurrentPage = 1;
            }
            var response = repository.GetAllPrivilege(filter);
            return Ok<DataResponse<EntityList<EntityPrivilege>>>(response);
        }

        [HttpPost]
        [Route("save")]
        public IHttpActionResult CreatePrivilegeModule(PrivilegeApiModel model)
        {
            var response = new DataResponse<EntityPrivilege>();

            if (ModelState.IsValid)
            {
                var entityPrivilege = new EntityPrivilege();
                entityPrivilege.Title = model.Title;
                entityPrivilege.Description = model.Description;
                entityPrivilege.PrivilegeKey = model.PrivilegeKey;
                entityPrivilege.ModuleId = model.ModuleId;
                entityPrivilege.CreatedBy = CurrentUserId;
                response = repository.Insert(entityPrivilege);
            }
            return Ok<DataResponse>(response);
        }

        [Route("{id}")]
        public IHttpActionResult GetPrivilegeById(int id)
        {
            var response = new DataResponse<EntityPrivilege>();
            if (id != 0)
            {
                response = repository.GetPrivilegeById(id);

                return Ok<DataResponse>(response);
            }
            else
            {
                return null;
            }
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult UpdatePrivilege(PrivilegeApiModel model)
        {
            var response = new DataResponse<EntityPrivilege>();

            if (ModelState.IsValid)
            {
                var entityPrivilege = new EntityPrivilege();
                entityPrivilege.Id = model.Id;
                entityPrivilege.Title = model.Title;
                entityPrivilege.PrivilegeKey = model.PrivilegeKey;
                entityPrivilege.ModuleId = model.ModuleId;
                entityPrivilege.Description = model.Description;
                entityPrivilege.UpdatedBy = CurrentUserId;
                entityPrivilege.UpdatedOn = System.DateTime.UtcNow;
                response = repository.UpdatePrivilegeById(entityPrivilege);

            }
            return Ok<DataResponse>(response);
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeletePrivilege(int id)
        {
            var response = new DataResponse<bool>();
            response = repository.DeletePrivilege(id);
            return Ok<DataResponse>(response);
        }

        [Route("getallprivilegemodules")]
        public IHttpActionResult GetAllPrivilegeModules()
        {
            var repository = new RepositoryLookups();
            var response = repository.GetAllPrivilegeModules();
            return Ok<DataResponse<EntityList<EntitySelectItem>>>(response);
        }
    }
}
