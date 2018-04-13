
using EBP.Business;
using EBP.Business.Entity;
using EBP.Business.Entity.Departments;
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
    [RoutePrefix("api/departments")]

    public class ApiDepartmentController : ApiBaseController
    {
        RepositoryDepartments repository = new RepositoryDepartments();

        [HttpPost]
        [Route("")]
        public IHttpActionResult GetAllDepartments(FilterDepartments filter)
        {
            if (filter == null)
            {
                filter = new FilterDepartments();
                filter.PageSize = 25;
                filter.CurrentPage = 1;
            }
            var response = repository.GetDepartments(filter, CurrentBusinessId);
            return Ok<DataResponse<EntityList<EntityDepartments>>>(response);
        }

        [HttpPost]
        [Route("save")]
        public IHttpActionResult CreateDepartments(DepartmentModels model)
        {
            var response = new DataResponse<EntityDepartments>();

            var entitydepartments = new EntityDepartments();
            entitydepartments.DepartmentName = model.DepartmentName;
            entitydepartments.Description = model.Description;
            entitydepartments.BusinessId = CurrentBusinessId;
            entitydepartments.CreatedBy = CurrentUserId;
            entitydepartments.CreatedOn = DateTime.UtcNow;
            entitydepartments.DepartmentPrivilege = model.DepartmentPrivilege;
            response = repository.Insert(entitydepartments);
            return Ok<DataResponse>(response);
        }

        [Route("{id}")]
        public IHttpActionResult GetDepartmentById(int id)
        {

            var response = new DataResponse<EntityDepartments>();
            if (id != 0)
            {
                response = repository.GetDepartmentById(id);

                return Ok<DataResponse>(response);
            }
            else
            {
                return null;
            }
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult UpdateDepartment(DepartmentModels model)
        {
            var response = new DataResponse<EntityDepartments>();

            if (ModelState.IsValid)
            {
                var entityDepartments = new EntityDepartments();
                entityDepartments.Id = model.Id;
                entityDepartments.DepartmentName = model.DepartmentName;
                entityDepartments.Description = model.Description;
                entityDepartments.IsActive = model.IsActive;
                entityDepartments.UpdatedBy = CurrentUserId;
                entityDepartments.UpdatedOn = System.DateTime.UtcNow;
                entityDepartments.DepartmentPrivilege = model.DepartmentPrivilege;
                response = repository.Update(entityDepartments);

            }
            return Ok<DataResponse>(response);
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeleteDepartmentById(int id)
        {
            var response = new DataResponse<bool>();
            response = repository.Delete(id);
            return Ok<DataResponse>(response);
        }

    }
}
