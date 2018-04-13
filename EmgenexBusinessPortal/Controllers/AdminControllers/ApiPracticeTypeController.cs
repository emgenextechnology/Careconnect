using EBP.Business;
using EBP.Business.Entity;
using EBP.Business.Entity.PracticeType;
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
    [RoutePrefix("api/practicetype")]
    public class ApiPracticeTypeController : ApiBaseController
    {
        RepositoryPracticeType repository = new RepositoryPracticeType();

        [HttpPost]
        [Route("")]
        public IHttpActionResult GetAllPracticeTypes(FilterPracticeType filter)
        {
            if (filter == null)
            {
                filter = new FilterPracticeType();
                filter.PageSize = 25;
                filter.CurrentPage = 1;
            }
            var response = repository.GetAllPracticeType(filter);
            return Ok<DataResponse<EntityList<EntityPracticeType>>>(response);
        }

        [HttpPost]
        [Route("save")]
        public IHttpActionResult CreatePracticeType(PracticeTypeModel model)
        {
            var response = new DataResponse<EntityPracticeType>();

            if (ModelState.IsValid)
            {
                var entityPracticeType = new EntityPracticeType();
                entityPracticeType.PracticeType = model.PracticeType;
                entityPracticeType.IsActive = model.IsActive;
                entityPracticeType.CreatedBy = CurrentUserId;
                response = repository.Insert(entityPracticeType);
            }
            return Ok<DataResponse>(response);
        }

        [Route("{id}")]
        public IHttpActionResult GetPracticeTypeById(int id)
        {
            var response = new DataResponse<EntityPracticeType>();
            if (id != 0)
            {
                response = repository.GetPracticeTypeById(id);

                return Ok<DataResponse>(response);
            }
            else
            {
                return null;
            }
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult UpdatePracticeType(PracticeTypeModel model)
        {
            var response = new DataResponse<EntityPracticeType>();

            if (ModelState.IsValid)
            {
                var entityPracticeType = new EntityPracticeType();
                entityPracticeType.Id = model.Id;
                entityPracticeType.PracticeType = model.PracticeType;
                entityPracticeType.IsActive = model.IsActive;
                entityPracticeType.UpdatedBy = CurrentUserId;
                entityPracticeType.UpdatedOn = System.DateTime.UtcNow;
                response = repository.UpdatePracticeSpeciality(entityPracticeType);

            }
            return Ok<DataResponse>(response);
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeletePracticeType(int id)
        {
            var response = new DataResponse<bool>();
            response = repository.DeletePracticeType(id);
            return Ok<DataResponse>(response);
        }
    }
}
