using EBP.Business;
using EBP.Business.Entity;
using EBP.Business.Entity.Practice;
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
    [RoutePrefix("api/location")]
    public class ApiLocationController : ApiBaseController
    {
        RepositoryAddress repository = new RepositoryAddress();

        [HttpPost]
        [Route("")]
        public IHttpActionResult GetAllAddressLocation(FilterAddress filter)
        {
            if (filter == null)
            {
                filter = new FilterAddress();
                filter.PageSize = 25;
                filter.CurrentPage = 1;
            }
            var response = repository.GetAddress(filter, CurrentBusinessId.Value);
            return Ok<DataResponse<EntityList<EntityPracticeAddress>>>(response);
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult UpdateAddressLocationId(LocationModel model)
        {
            var response = new DataResponse<EntityPracticeAddress>();

            if (ModelState.IsValid)
            {
                var entityPracticeAddress = new EntityPracticeAddress();
                entityPracticeAddress.Id = model.Id;
                entityPracticeAddress.LocationId = model.LocationId;

                response = repository.Update(entityPracticeAddress);
            }
            return Ok<DataResponse>(response);
        }

        [Route("{id}")]
        public IHttpActionResult GetAddressLocationById(int id)
        {
            var response = new DataResponse<EntityPracticeAddress>();
            if (id != 0)
            {
                response = repository.GetAddressById(id);

                return Ok<DataResponse>(response);
            }
            else
            {
                return null;
            }
        }
    }
}
