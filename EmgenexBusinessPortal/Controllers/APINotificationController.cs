using EBP.Business;
using EBP.Business.Entity;
using EBP.Business.Entity.EntityNotificationSettings;
using EBP.Business.Entity.Practice;
using EBP.Business.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace EmgenexBusinessPortal.Controllers
{
    [ApiAuthorize]
    [RoutePrefix("notification")]
    public class APINotificationController : ApiBaseController
    {
        [Route("all/{skip}/{take}")]
        public IHttpActionResult GetAllNotifications(int skip, int take)
        {
            var repository = new RepositoryNotification();

            var response = repository.GetAllNotifications(CurrentUserId,CurrentBusinessId, skip, take);

            return Ok<DataResponse<Notification>>(response);
        }

        [HttpPost]
        [Route("updateNotification/{id}")]
        public IHttpActionResult updateNotification(string id)
        {
            var repository = new RepositoryNotification();
            if (!User.Identity.IsAuthenticated)
            {
                return Ok<dynamic>(new { IsSuccess = 0, Status = HttpStatusCode.BadRequest, Model = new { }, Message = "Authorization failed!" });
            }
            try
            {
                repository.UpdateNotification(Int32.Parse(id), CurrentUserId);
                return Ok<dynamic>(new { IsSuccess = 1, Status = 200, Model = new { }, Message = "Success!" });
            }
            catch (Exception e)
            {
                return Ok<dynamic>(new { IsSuccess = 0, Status = DataResponseStatus.InternalServerError, Message = e.Message });
            }
        }
        [Route("UnReadNotificationCount")]
        public IHttpActionResult GetUnReadCount()
        {
            var repository = new RepositoryNotification();
            var response = repository.GetUnReadNotificationCount(CurrentUserId);
            return Ok<DataResponse<UnReadNotification>>(response);
        }
    }
}
