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
using EBP.Business.Entity.Business;
using EBP.Business.Repository;
using EBP.Business.Entity.Practice;

namespace EmgenexBusinessPortal.Areas.BusinessSettings.Controllers
{
    [RoutePrefix("Address")]

    public class ApiAddressController : ApiBaseController
    {
        protected int CurrentUserId { get { return 2; } }
        protected int? CurrentBusinessId { get { return 1; } }

        [HttpPost]
        [Route("getbyfilter")]
        [Route("")]
        public IHttpActionResult GetByFilter(FilterAddress filter)
        {
            //string[] superRoles = { "EDTADSLOC" };
            //bool hasSuperRight = HasRight(superRoles);
            //if (!hasSuperRight)
            //{ 
            //    return Ok<DataResponse>(null); 
            //}
            //else
            //{
                var repository = new RepositoryAddress();
                if (filter == null)
                    filter = new FilterAddress { PageSize = 25, CurrentPage = 1 };
                var response = repository.GetAddress(filter, CurrentBusinessId.Value);
                return Ok<DataResponse<EntityList<EntityPracticeAddress>>>(response);
            //}
        }
        [Route("getAddressById/{Id}")]
        [Route("{Id}")]
        public IHttpActionResult GetAddress(int? Id)
        {
            var response = new DataResponse<EntityPracticeAddress>();
            var repository = new RepositoryAddress();
            if (CurrentBusinessId.HasValue)
            {
                response = repository.GetAddressById(Id.Value);
            }
            else
            {
                response.Model = new EntityPracticeAddress();
            }
            return Ok<DataResponse>(response);
        }
        [HttpPost]
        [Route("Save")]
        public IHttpActionResult InsertAddressData(EntityPracticeAddress model)
        {
            var response = new DataResponse<EntityPracticeAddress>();

            if (ModelState.IsValid)
            {
                if (model.Id > 0)
                {
                    response = new RepositoryAddress().Update(model);
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
    }
}