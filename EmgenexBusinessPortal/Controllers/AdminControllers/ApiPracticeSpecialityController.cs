using EBP.Business;
using EBP.Business.Entity;
using EBP.Business.Entity.PracticeSpeciality;
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
    [RoutePrefix("api/practiceSpeciality")]
    public class ApiPracticeSpecialityController : ApiBaseController
    {
        RepositoryPracticeSpeciality repository = new RepositoryPracticeSpeciality();

        [HttpPost]
        [Route("")]
        public IHttpActionResult GetAllPracticespecialities(PracticeSpecialityFilter filter)
        {
            if (filter == null)
            {
                filter = new PracticeSpecialityFilter();
                filter.PageSize = 25;
                filter.CurrentPage = 1;
            }
            var response = repository.GetAllSpecialityType(filter);
            return Ok<DataResponse<EntityList<EntityPracticeSpecialityType>>>(response);
        }

        [HttpPost]
        [Route("save")]
        public IHttpActionResult CreatePracticeSpeciality(PracticeSpecialityModel model)
        {
            var response = new DataResponse<EntityPracticeSpecialityType>();

            if (ModelState.IsValid)
            {
                var entityPracticeSpeciality = new EntityPracticeSpecialityType();
                entityPracticeSpeciality.PracticeSpecialityType = model.PracticeSpecialityType;
                entityPracticeSpeciality.IsActive = model.IsActive;
                entityPracticeSpeciality.CreatedBy = CurrentUserId;
                response = repository.Insert(entityPracticeSpeciality);
            }
            return Ok<DataResponse>(response);
        }

        [Route("{id}")]
        public IHttpActionResult GetPracticeSpecialityById(int id)
        {
            var response = new DataResponse<EntityPracticeSpecialityType>();
            if (id != 0)
            {
                response = repository.GetPracticeSpecialityTypeById(id);

                return Ok<DataResponse>(response);
            }
            else
            {
                return null;
            }
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult UpdatePracticeSpeciality(PracticeSpecialityModel model)
        {
            var response = new DataResponse<EntityPracticeSpecialityType>();

            if (ModelState.IsValid)
            {
                var entityPracticeSpeciality = new EntityPracticeSpecialityType();
                entityPracticeSpeciality.Id = model.Id;
                entityPracticeSpeciality.PracticeSpecialityType = model.PracticeSpecialityType;
                entityPracticeSpeciality.IsActive = model.IsActive;
                entityPracticeSpeciality.UpdatedBy = CurrentUserId;
                entityPracticeSpeciality.UpdatedOn = System.DateTime.UtcNow;
                response = repository.UpdatePracticeSpeciality(entityPracticeSpeciality);
            }
            return Ok<DataResponse>(response);
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeletePracticeSpeciality(int id)
        {
            var response = new DataResponse<bool>();
            response = repository.DeletePracticeSpeciality(id);
            return Ok<DataResponse>(response);
        }
    }
}
