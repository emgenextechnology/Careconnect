using EBP.Business;
using EBP.Business.Entity;
using EBP.Business.Entity.Notification;
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
    [RoutePrefix("api/notificationtype")]
    public class ApiNotificationTypeController : ApiBaseController
    {
        RepositoryNotificationType repository = new RepositoryNotificationType();

        [Route("")]
        [HttpPost]
        public IHttpActionResult GetAll(FilterNotificationType filter)
        {
            if (filter == null)
            {
                filter = new FilterNotificationType();
                filter.PageSize = 25;
                filter.CurrentPage = 1;
            }
            var response = repository.GetAllNotificationType(filter);
            return Ok<DataResponse<EntityList<EntityNotificationType>>>(response);
        }

        [HttpPost]
        [Route("save")]
        public IHttpActionResult CreateNotificationType(NotificationTypeModel model)
        {
            var response = new DataResponse<EntityNotificationType>();

            if (ModelState.IsValid)
            {
                var entityNotificationType = new EntityNotificationType();
                entityNotificationType.Title = model.Title;
                entityNotificationType.NotificationKey = model.NotificationKey;
                entityNotificationType.CreatedBy = CurrentUserId;
                response = repository.Insert(entityNotificationType);
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
        public IHttpActionResult GetNotificationTypeById(int id)
        {

            var response = new DataResponse<EntityNotificationType>();
            if (id != 0)
            {
                response = repository.GetNoificationTypeById(id);

                return Ok<DataResponse>(response);
            }
            else
            {
                return null;
            }

        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult UpdateNotificationType(NotificationTypeModel model)
        {
            var response = new DataResponse<EntityNotificationType>();

            if (ModelState.IsValid)
            {
                var entityNotificationType = new EntityNotificationType();
                entityNotificationType.Id = model.Id;
                entityNotificationType.Title = model.Title;
                entityNotificationType.NotificationKey = model.NotificationKey;
                entityNotificationType.UpdatedBy = CurrentUserId;
                entityNotificationType.UpdatedOn = System.DateTime.UtcNow;
                response = repository.UpdateNoificationTypeById(entityNotificationType);
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
        public IHttpActionResult DeleteNotificationType(int id)
        {
            var response = new DataResponse<bool>();
            response = repository.DeleteNotificationType(id);
            return Ok<DataResponse>(response);
        }
    }
}
