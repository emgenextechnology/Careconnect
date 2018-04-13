using EBP.Business;
using EBP.Business.Database;
using EBP.Business.Entity;
using EBP.Business.Entity.Rep;
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
    [RoutePrefix("api/reps")]
    public class ApiSalesRepsController : ApiBaseController
    {
        RepositoryReps repository = new RepositoryReps();
        CareConnectCrmEntities DBEntity = new CareConnectCrmEntities();

        [HttpPost]
        [Route("")]
        public IHttpActionResult GetAll(FilterReps filter)
        {
            if (filter == null)
            {
                filter = new FilterReps();
                filter.PageSize = 25;
                filter.CurrentPage = 1;
            }
            var response = repository.GetReps(filter, CurrentBusinessId.Value);
            return Ok<DataResponse<EntityList<EntityReps>>>(response);
        }

        [HttpPost]
        [Route("save")]
        public IHttpActionResult CreateSalesRep(SalesRepModel model)
        {
            var response = new DataResponse<EntityReps>();
            var entityReps = new EntityReps();
            entityReps.UserId = model.UserId;
            entityReps.RepGroupId = model.RepGroupId;
            entityReps.CreatedBy = CurrentUserId;
            entityReps.CreatedOn = DateTime.UtcNow;
            entityReps.IsActive = true;
            entityReps.SelectedServiceNames = model.SelectedServiceNames;

            response = repository.Insert(entityReps);

            return Ok<DataResponse>(response);
        }

        [Route("{id}")]
        public IHttpActionResult GetSalesRepById(int id)
        {
            var response = new DataResponse<EntityReps>();
            if (id != 0)
            {
                response = repository.GetRepById(id);
                var ServiceList = DBEntity.RepServiceMappers.Where(a => a.RepId == id).Select(a => a.ServiceId);
                response.Model.SelectedServiceNames = DBEntity.LookupEnrolledServices.Where(a => a.BusinessId == CurrentBusinessId && a.IsActive == true).Select(x => new SelectedItem()
                {
                    Selected = ServiceList.Contains(x.Id),
                    Text = x.ServiceName,
                    Value = x.Id.ToString()
                });
                return Ok<DataResponse>(response);
            }
            else
            {
                return null;
            }
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult UpdateSalesRep(SalesRepModel model)
        {
            var response = new DataResponse<EntityReps>();

            if (ModelState.IsValid)
            {
                var entityReps = new EntityReps();
                entityReps.Id = model.Id;
                entityReps.UserId = model.UserId;
                entityReps.RepGroupId = model.RepGroupId;
                entityReps.IsActive = model.IsActive.Value;
                entityReps.UpdatedBy = CurrentUserId;
                entityReps.UpdatedOn = DateTime.UtcNow;
                entityReps.SelectedServiceNames = model.SelectedServiceNames;

                response = repository.Update(entityReps);

            }
            return Ok<DataResponse>(response);
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeleteSalesRep(int id)
        {
            var response = new DataResponse<bool>();
            response = repository.Delete(id);
            return Ok<DataResponse>(response);
        }
    }
}
