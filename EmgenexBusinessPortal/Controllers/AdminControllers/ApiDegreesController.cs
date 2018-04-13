using EBP.Business;
using EBP.Business.Entity;
using EBP.Business.Entity.Degree;
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
    [RoutePrefix("api/degrees")]
    public class ApiDegreesController : ApiBaseController
    {
        RepositoryDegreescs repository = new RepositoryDegreescs();

        [HttpPost]
        [Route("")]
        public IHttpActionResult GetAllCountries(FilterDegree filter)
        {

            if (filter == null)
            {
                filter = new FilterDegree();
                filter.PageSize = 25;
                filter.CurrentPage = 1;
            }

            var response = repository.GetAllDegrees(filter);

            return Ok<DataResponse<EntityList<EntityDegrees>>>(response);
        }

        [HttpPost]
        [Route("save")]
        public IHttpActionResult CreateDegree(DegreeModel model)
        {
            var response = new DataResponse<EntityDegrees>();

            if (ModelState.IsValid)
            {
                var entityDegree = new EntityDegrees();
                entityDegree.DegreeName = model.DegreeName;
                entityDegree.ShortCode = model.ShortCode;
                entityDegree.IsActive = model.IsActive;
                entityDegree.CreatedBy = CurrentUserId;
                response = repository.Insert(entityDegree);
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
        public IHttpActionResult GetDegreeById(int id)
        {

            var response = new DataResponse<EntityDegrees>();
            if (id != 0)
            {
                response = repository.GetDegreeById(id);

                return Ok<DataResponse>(response);
            }
            else
            {
                return null;
            }
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult UpdateDegree(DegreeModel model)
        {
            var response = new DataResponse<EntityDegrees>();

            if (ModelState.IsValid)
            {
                var entityDegree = new EntityDegrees();
                entityDegree.Id = model.Id;
                entityDegree.IsActive = model.IsActive;
                entityDegree.DegreeName = model.DegreeName;
                entityDegree.ShortCode = model.ShortCode;
                entityDegree.UpdatedBy = CurrentUserId;
                entityDegree.UpdatedOn = System.DateTime.UtcNow;
                response = repository.UpdateDegree(entityDegree);
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
        public IHttpActionResult DeleteDegreeById(int id)
        {
            var response = new DataResponse<bool>();
            response = repository.DeleteDegree(id);
            return Ok<DataResponse>(response);
        }
    }
}
