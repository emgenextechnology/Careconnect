using EBP.Api.Models;
using EBP.Business;
using EBP.Business.Entity.EntityNotificationSettings;
using EBP.Business.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EBP.Api.Controllers
{
    [ApiAuthorize]
    [RoutePrefix("{Type:regex(notificationSetting|notificationSettings)}")]
    //[RoutePrefix("notificationSettings")]
    public class ApiUserNotificationSettingsController : ApiBaseController
    {
        [Route("")]
        public IHttpActionResult GetAllNotifications()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Ok<dynamic>(new { IsSuccess = 0, Status = HttpStatusCode.BadRequest, Model = new { }, Message = "Authorization failed!" });
            }
              var repository = new RepositoryNotification();
         
            var user = new RepositoryUserProfile().GetUserbyId(CurrentUserId);         
            var response = repository.GetAllNotificationSettings(CurrentUserId);
            return Ok<DataResponse>(response);
        }
        [HttpPost]
        [Route("save")]
        public IHttpActionResult UpdateNotifications(NotificationModel entity)
        {

            var repository = new RepositoryNotification();
            var response = new DataResponse();
            if (ModelState.IsValid)
            {
                var model = new EntityNotificationSettings
                {
                    UserId=CurrentUserId,
                    Status=entity.Status,
                    NotificationTypeId=entity.NotificationTypeId
                };

                response = repository.SaveNotification(model);
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
            response.Status = DataResponseStatus.OK;
            response.Message = "Success";
            return Ok<DataResponse>(response);
        }

    }
}
