using EBP.Business;
using EBP.Business.Entity.Business;
using EBP.Business.Repository;
using EmgenexBusinessPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace EmgenexBusinessPortal.Controllers
{
    [ApiAuthorize]
    [RoutePrefix("api/businessprofile")]
    public class ApiBusinessProfileController : ApiBaseController
    {
        RepositoryBusinessProfiles repository = new RepositoryBusinessProfiles();

        [Route("")]
        public IHttpActionResult GetBusinessProfile()
        {
            var response = new DataResponse<EntityBusinessProfile>();

            if (CurrentBusinessId.HasValue)
            {
                response = repository.GetBusinessProfileById(CurrentBusinessId.Value);
            }
            else
            {
                response.Model = new EntityBusinessProfile();
            }
            return Ok<DataResponse>(response);
        }

        [HttpPost]
        [Route("save")]
        public IHttpActionResult InsertBusinessProfileData(BusinessProfileModel model)
        {
            var response = new DataResponse<EntityBusinessProfile>();
            EntityBusinessProfile entity = new EntityBusinessProfile();
            if (ModelState.IsValid)
            {
                entity.BusinessName = model.BusinessName;
                entity.Description = model.Description;
                entity.Address = model.Address;
                entity.About = model.About;
                entity.DomainUrl = model.DomainUrl;
                entity.City = model.City;
                entity.State = model.State;
                entity.Country = model.Country;
                entity.IsActive = model.IsActive;
                entity.OtherEmails = model.OtherEmails;
                entity.DateRange = model.DateRange;
                entity.SalesGroup = model.SalesGroup;
                entity.CreatedBy = CurrentUserId;
                entity.UpdatedBy = CurrentUserId;
                entity.RelativeUrl = model.RelativeUrl.Replace(" ", "-");
                entity.Id = model.Id;
                entity.OtherEmails = string.IsNullOrEmpty(model.OtherEmails) ? null : Regex.Replace(model.OtherEmails, @"[\[\]\""]+", "");
                if (!string.IsNullOrEmpty(model.DomainUrl))
                {
                    model.DomainUrl = model.DomainUrl.Replace(" ", "-");
                }
                if (model.Id > 0)
                {
                    response = new RepositoryBusinessProfiles().Update(entity);
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
