using EBP.Business;
using EBP.Business.Entity;
using EBP.Business.Entity.LeadSource;
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
    [RoutePrefix("api/leadSources")]
    public class ApiLeadSourceController : ApiBaseController
    {
        RepositoryLeadSource repository = new RepositoryLeadSource();

        [Route("")]
        [HttpPost]
        public IHttpActionResult GetAllLeadSources(FilterLeadSource filter)
        {
            if (filter == null)
            {
                filter = new FilterLeadSource();
                filter.PageSize = 25;
                filter.CurrentPage = 1;
            }
            var response = repository.GetAllLeadSources(filter);
            return Ok<DataResponse<EntityList<EntityLeadSource>>>(response);
        }

        [HttpPost]
        [Route("save")]
        public IHttpActionResult CreateLeadSource(LeadSourceModel model)
        {
            var response = new DataResponse<EntityLeadSource>();

            if (ModelState.IsValid)
            {
                var entityLeadsource = new EntityLeadSource();
                entityLeadsource.LeadSource = model.LeadSource;
                entityLeadsource.IsActive = model.IsActive;
                entityLeadsource.CreatedBy = CurrentUserId;
                response = repository.Insert(entityLeadsource);
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

        [Route("{id}")]
        public IHttpActionResult GetLeadSourceById(int id)
        {
            var response = new DataResponse<EntityLeadSource>();
            if (id != 0)
            {
                response = repository.GetLeadsourceById(id);

                return Ok<DataResponse>(response);
            }
            else
            {
                return null;
            }
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult UpdateLeadSource(LeadSourceModel model)
        {
            var response = new DataResponse<EntityLeadSource>();

            if (ModelState.IsValid)
            {
                var entityDegree = new EntityLeadSource();
                entityDegree.Id = model.Id;
                entityDegree.IsActive = model.IsActive;
                entityDegree.LeadSource = model.LeadSource;
                entityDegree.UpdatedBy = CurrentUserId;
                entityDegree.UpdatedOn = System.DateTime.UtcNow;
                response = repository.UpdateLeadSource(entityDegree);
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

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeleteLeadSourceById(int id)
        {
            var response = new DataResponse<bool>();
            response = repository.DeleteLeadSource(id);
            return Ok<DataResponse>(response);
        }
    }
}
