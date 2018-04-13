using EBP.Business;
using EBP.Business.Entity;
using EBP.Business.Entity.Country;
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
    [RoutePrefix("api/countries")]
    public class ApiCountriesController : ApiBaseController
    {
        RepositoryCountries repository = new RepositoryCountries();

        [Route("")]
        [HttpPost]
        public IHttpActionResult GetAllCountries(FilterCountry filter)
        {
            if (filter == null)
            {
                filter = new FilterCountry();
                filter.PageSize = 25;
                filter.CurrentPage = 1;
            }
            var response = repository.GetAllCountries(filter);
            return Ok<DataResponse<EntityList<EntityCountry>>>(response);
        }

        [HttpPost]
        [Route("save")]
        public IHttpActionResult CreateCountry(CountryModel model)
        {
            var response = new DataResponse<EntityCountry>();

            if (ModelState.IsValid)
            {
                var entityCountry = new EntityCountry();
                entityCountry.CountryName = model.CountryName;
                entityCountry.CountryCode = model.CountryCode;
                entityCountry.IsActive = model.IsActive;
                entityCountry.CreatedBy = CurrentUserId;
                response = repository.Insert(entityCountry);
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
        public IHttpActionResult GetCountryById(int id)
        {

            var response = new DataResponse<EntityCountry>();
            if (id != 0)
            {
                response = repository.GetCountryById(id);

                return Ok<DataResponse>(response);
            }
            else
            {
                return null;
            }
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult UpdateCountry(CountryModel model)
        {
            var response = new DataResponse<EntityCountry>();

            if (ModelState.IsValid)
            {
                var entityCountry = new EntityCountry();
                entityCountry.Id = model.Id;
                entityCountry.CountryName = model.CountryName;
                entityCountry.CountryCode = model.CountryCode;
                entityCountry.IsActive = model.IsActive;
                entityCountry.UpdatedBy = CurrentUserId;
                entityCountry.UpdatedOn = System.DateTime.UtcNow;
                response = repository.UpdateCountry(entityCountry);
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
        public IHttpActionResult DeleteCountryById(int id)
        {
            var response = new DataResponse<bool>();
            response = repository.DeleteCountry(id);
            return Ok<DataResponse>(response);
        }
    }
}
