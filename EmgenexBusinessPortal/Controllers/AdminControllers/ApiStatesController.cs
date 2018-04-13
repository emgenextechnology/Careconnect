using EBP.Business;
using EBP.Business.Entity;
using EBP.Business.Entity.States;
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
    [ApiAuthorize(Roles = "admin")]
    [RoutePrefix("api/states")]
    public class ApiStatesController : ApiBaseController
    {
        RepositoryStates repository = new RepositoryStates();

        [HttpPost]
        [Route("")]
        public IHttpActionResult GetAllStates(FilterStates filter)
        {
            if (filter == null)
            {
                filter = new FilterStates();
                filter.PageSize = 25;
                filter.CurrentPage = 1;
            }
            var response = repository.GetAllStates(filter);
            return Ok<DataResponse<EntityList<EntityStates>>>(response);
        }

        [HttpPost]
        [Route("save")]
        public IHttpActionResult CreateStates(StateModel model)
        {
            var response = new DataResponse<EntityStates>();

            if (ModelState.IsValid)
            {
                var entityState = new EntityStates();
                entityState.StateName = model.StateName;
                entityState.StateCode = model.StateCode;
                entityState.CountryId = model.CountryId;
                entityState.CreatedBy = CurrentUserId;
                entityState.IsActive = model.IsActive;
                response = repository.Insert(entityState);
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
        public IHttpActionResult GetStateById(int id)
        {

            var response = new DataResponse<EntityStates>();
            if (id != 0)
            {
                response = repository.GetStateById(id);

                return Ok<DataResponse>(response);
            }
            else
            {
                return null;
            }
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult UpdateState(StateModel model)
        {
            var response = new DataResponse<EntityStates>();

            if (ModelState.IsValid)
            {
                var entityState = new EntityStates();
                entityState.Id = model.Id;
                entityState.StateName = model.StateName;
                entityState.StateCode = model.StateCode;
                entityState.CountryId = model.CountryId;
                entityState.IsActive = model.IsActive;
                entityState.UpdatedBy = CurrentUserId;
                entityState.UpdatedOn = System.DateTime.UtcNow;
                response = repository.UpdateState(entityState);
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
        public IHttpActionResult DeleteStateById(int id)
        {
            var response = new DataResponse<bool>();
            response = repository.DeleteState(id);
            return Ok<DataResponse>(response);
        }
    }
}
