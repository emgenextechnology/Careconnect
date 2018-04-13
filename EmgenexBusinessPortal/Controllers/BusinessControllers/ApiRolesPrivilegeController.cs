using EBP.Business;
using EBP.Business.Entity;
using EBP.Business.Entity.Roles;
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
    [RoutePrefix("api/roles")]
    public class ApiRolesPrivilegeController : ApiBaseController
    {
        RepositoryRoles repository = new RepositoryRoles();

        [HttpPost]
        [Route("")]
        public IHttpActionResult GetAllRoles(FilterRoles filter)
        {
            if (filter == null)
            {
                filter = new FilterRoles();
                filter.PageSize = 25;
                filter.CurrentPage = 1;
            }
            var response = repository.GetRoles(filter, CurrentBusinessId.Value);
            return Ok<DataResponse<EntityList<EntityRoles>>>(response);
        }

        [HttpPost]
        [Route("save")]
        public IHttpActionResult CreateRoles(RolesModel model)
        {
            var response = new DataResponse<EntityRoles>();

            var entityRoles = new EntityRoles();
            entityRoles.Name = model.Name;
            entityRoles.Description = model.Description;
            entityRoles.BusinessId = CurrentBusinessId;
            entityRoles.CreatedBy = CurrentUserId;
            entityRoles.CreatedOn = DateTime.UtcNow;
            entityRoles.RolePrivilege = model.RolePrivilege;
            response = repository.Insert(entityRoles);

            return Ok<DataResponse>(response);
        }

        [Route("{id}")]
        public IHttpActionResult GetRolesById(int id)
        {
            var response = new DataResponse<EntityRoles>();
            if (id != 0)
            {
                response = repository.GetRolesById(id);

                return Ok<DataResponse>(response);
            }
            else
            {
                return null;
            }
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult UpdateRoles(RolesModel model)
        {
            var response = new DataResponse<EntityRoles>();

            if (ModelState.IsValid)
            {
                var entityRoles = new EntityRoles();
                entityRoles.Id = model.Id;
                entityRoles.Name = model.Name;
                entityRoles.Description = model.Description;
                entityRoles.IsActive = model.IsActive;
                entityRoles.RolePrivilege = model.RolePrivilege;
                entityRoles.CreatedBy = CurrentUserId;

                response = repository.Update(entityRoles);
            }
            return Ok<DataResponse>(response);
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeleteRoleById(int id)
        {
            var response = new DataResponse<bool>();
            response = repository.Delete(id);
            return Ok<DataResponse>(response);
        }
    }
}
