using EBP.Business.Database;
using EBP.Business.Repository;
using EmgenexBusinessPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmgenexBusinessPortal.Areas.Business.Controllers
{
    [BusinessAuthorize]
    public class NotificationSettingsController : BaseController
    {
        // GET: Business/NotificationSettings
        public ActionResult Index()
        {
            var model = new NotificationsettingModel();
            var user = new RepositoryUserProfile().GetUserbyId(CurrentUserId);
            model.Name = user.FirstName + " " + user.LastName;
            model.Notifications = from p in db.LookupNotificationTypes
                                  join ap in db.UserNotificationMappers.Where(e => e.UserId == CurrentUserId) on p.Id equals ap.NotificationTypeId
                                  into joined
                                  from j in joined.DefaultIfEmpty()
                                  select new NotificationModel { NotificationTypeId = p.Id, NotificationType = p.Title, Status = j.Status == null ? true : j.Status };
            return View(model);
        }
        [HttpPost]
        public ActionResult Index(NotificationModel entity)
        {
            var model = db.UserNotificationMappers.FirstOrDefault(a => a.NotificationTypeId == entity.NotificationTypeId & a.UserId == CurrentUserId);
            if (model != null)
            {
                model.Status = entity.Status;
                model.UpdatedBy = CurrentUserId;
                model.UpdatedOn = DateTime.UtcNow;
                db.SaveChanges();
            }
            else
            {
                db.UserNotificationMappers.Add(new UserNotificationMapper
                {
                    UserId = CurrentUserId,
                    NotificationTypeId = entity.NotificationTypeId,
                    Status = entity.Status,
                    CreatedBy = CurrentUserId,
                    CreatedOn = DateTime.UtcNow
                });
                db.SaveChanges();

            }
            return Json(true);
        }
    }
}