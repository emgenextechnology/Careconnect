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
using EBP.Business.Entity.Roles;

namespace EmgenexBusinessPortal.Areas.BusinessSettings.Controllers
{
    [RoutePrefix("Roles")]
    public class ApiRolesController : ApiBaseController
    {
        
        protected int CurrentUserId { get { return 2; } }
        protected int? CurrentBusinessId { get { return 1; } }
        [HttpPost]
        [Route("getbyfilter")]
        [Route("")]
        public IHttpActionResult GetByFilter(FilterRoles filter)
        {
            var repository = new RepositoryRoles();
            if (filter == null)
                filter = new FilterRoles { PageSize = 25, CurrentPage = 1 };
            var response = repository.GetRoles(filter, CurrentBusinessId.Value);
            return Ok<DataResponse<EntityList<EntityRoles>>>(response);
        }
        [Route("getRolebyid/{Id}")]
        [Route("{Id}")]
        public IHttpActionResult GetRoleById(int? Id)
        {
            var response = new DataResponse<EntityRoles>();
            var repository = new RepositoryRoles();
            if (Id.HasValue)
            {
                response = repository.GetRolesById(Id.Value);
            }
            else
            {
                response.Model = new EntityRoles();
            }
            return Ok<DataResponse>(response);
        }
        [HttpPost]
        [Route("Save")]
        public IHttpActionResult InsertRoleData(EntityRoles model)
        {
            var response = new DataResponse<EntityRoles>();

            if (ModelState.IsValid)
            {
                model.CreatedBy = CurrentUserId;
                model.BusinessId = CurrentBusinessId;
                if (model.Id > 0)
                {
                    response = new RepositoryRoles().Update(model);
                }
                else
                {
                    response = new RepositoryRoles().Insert(model);
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
        [Route("delete/{roleid}")]
        [Route("{roleid}/delete")]
        public IHttpActionResult Delete(int roleid)
        {
            var repository = new RepositoryRoles();
            var response = repository.Delete(roleid);
            return Ok<DataResponse>(response);
        }
    }
}