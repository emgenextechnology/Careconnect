using EBP.Business;
using EBP.Business.Entity;
using EBP.Business.Entity.PrivilegeModules;
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
    [RoutePrefix("api/privilegemodules")]
    public class ApiPrivilegeModulesController : ApiBaseController
    {
        RepositoryPrivilegeModules repository = new RepositoryPrivilegeModules();

        [HttpPost]
        [Route("")]
        public IHttpActionResult GetAll(FilterPrivilegeModule filter)
        {
            if (filter == null)
            {
                filter = new FilterPrivilegeModule();
                filter.PageSize = 25;
                filter.CurrentPage = 1;
            }
            var response = repository.GetAllPrivilegeModules(filter);
            return Ok<DataResponse<EntityList<EntityPrivilegeModules>>>(response);
        }

        [HttpPost]
        [Route("save")]
        public IHttpActionResult CreatePrivilegeModule(PrivilegeModulesModel model)
        {
            var response = new DataResponse<EntityPrivilegeModules>();

            if (ModelState.IsValid)
            {
                var entityPrivilegeModules = new EntityPrivilegeModules();
                entityPrivilegeModules.Title = model.Title;
                entityPrivilegeModules.Description = model.Description;
                entityPrivilegeModules.CreatedBy = CurrentUserId;
                entityPrivilegeModules.IsActive = true;
                response = repository.Insert(entityPrivilegeModules);
            }
            return Ok<DataResponse>(response);
        }

        [Route("{id}")]
        public IHttpActionResult GetPrivilegeModuleById(int id)
        {
            var response = new DataResponse<EntityPrivilegeModules>();
            if (id != 0)
            {
                response = repository.GetPrivilegeModulesById(id);

                return Ok<DataResponse>(response);
            }
            else
            {
                return null;
            }
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult UpdatePrivilegeModule(PrivilegeModulesModel model)
        {
            var response = new DataResponse<EntityPrivilegeModules>();

            if (ModelState.IsValid)
            {
                var entityPrivilegeModules = new EntityPrivilegeModules();
                entityPrivilegeModules.Id = model.Id;
                entityPrivilegeModules.Title = model.Title;
                entityPrivilegeModules.IsActive = model.IsActive;
                entityPrivilegeModules.Description = model.Description;
                entityPrivilegeModules.UpdatedBy = CurrentUserId;
                entityPrivilegeModules.UpdatedOn = System.DateTime.UtcNow;
                response = repository.UpdatePrivilegeModuleById(entityPrivilegeModules);

            }
            return Ok<DataResponse>(response);
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeletePrivilegeModule(int id)
        {
            var response = new DataResponse<bool>();
            response = repository.DeletePrivilegeModule(id);
            return Ok<DataResponse>(response);
        }
    }
}
