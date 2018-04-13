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
using EBP.Business.Entity.Departments;
using EmgenexBusinessPortal.Areas.BusinessSettings.Models;

namespace EmgenexBusinessPortal.Areas.BusinessSettings.Controllers
{
     [RoutePrefix("Departments")]

    public class ApiDepartmentsController : ApiBaseController
     {
        protected int CurrentUserId { get { return 2; } }
        protected int? CurrentBusinessId { get { return 1; } }
         [HttpPost]
         [Route("getbyfilter")]
         [Route("")]
         public IHttpActionResult GetByFilter(FilterDepartments filter)
         {
             var repository = new RepositoryDepartments();
                 if (filter == null)
                     filter = new FilterDepartments { PageSize = 25, CurrentPage = 1 };
                 var response = repository.GetDepartments(filter, CurrentBusinessId.Value);
                 return Ok<DataResponse<EntityList<EntityDepartments>>>(response);
         }
         [Route("getdepartmentbyid/{Id}")]
         [Route("{Id}")]
         public IHttpActionResult GetDepartmentById(int? Id)
         {
             var response = new DataResponse<EntityDepartments>();
             var repository = new RepositoryDepartments();
             if (Id.HasValue)
             {
                 response = repository.GetDepartmentById(Id.Value);
             }
             else
             {
                 response.Model = new EntityDepartments();
             }
             return Ok<DataResponse>(response);
         }

         //[Route("getvmDepartment/{id}")]
         //[Route("getvmDepartment/{id}/edit")]
         //public IHttpActionResult GetVmDepartmentObject(int? id)
         //{
         //    var response = new DataResponse<VMDepartment>();
         //    if (id.HasValue)
         //    {
         //        var repository = new RepositoryDepartments();
         //        var model = repository.GetDepartmentById(id.Value).Model;
         //        response.Model = new VMDepartment
         //        {
         //            Id=model.Id,
         //            DepartmentName=model.DepartmentName,
         //            Description=model.Description,
         //            IsActive=model.IsActive,
         //            DepartmentPrivilegeIds=model.DepartmentPrivilegeIds
         //        };
         //        return Ok<DataResponse>(response);
         //    }
         //    response.Model = new VMDepartment();
         //    return Ok<DataResponse>(response);
         //}
         [HttpPost]
         [Route("Save")]
         public IHttpActionResult InsertDepartmentData(VMDepartment model)
         {
             var response = new DataResponse<EntityDepartments>();

             if (ModelState.IsValid)
             {
                 var entityDepartment = new EntityDepartments
                 {
                     Id=model.Id,
                     DepartmentName=model.DepartmentName,
                     Description=model.Description,
                     DepartmentPrivilegeIds=model.DepartmentPrivilegeIds,
                     IsActive=model.IsActive,
                 };
                 entityDepartment.UpdatedBy = entityDepartment.CreatedBy = CurrentUserId;
                 entityDepartment.BusinessId = CurrentBusinessId;
                 if (model.Id > 0)
                 {
                     response = new RepositoryDepartments().Update(entityDepartment);
                 }
                 else
                 {
                     response = new RepositoryDepartments().Insert(entityDepartment);
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
         [Route("delete/{departmentid}")]
         [Route("{departmentid}/delete")]
         public IHttpActionResult Delete(int departmentid)
         {
             var repository = new RepositoryDepartments();
             var response = repository.Delete(departmentid);
             return Ok<DataResponse>(response);
         }
    }
}