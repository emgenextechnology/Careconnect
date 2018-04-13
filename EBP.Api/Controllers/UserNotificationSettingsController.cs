using EBP.Business.Database;
using EBP.Business.Repository;
using EmgenexBusinessPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace EmgenexBusinessPortal.Controllers
{
    public class UserNotificationSettingsController : Controller
    {
        private CareConnectCrmEntities db = new CareConnectCrmEntities();
        // GET: NotificationSettings
        public ActionResult Index()
        {
            var currentUserId =User.Identity.GetUserId<int>();
            var model = new NotificationsettingModel();
            var user = new RepositoryUserProfile().GetUserbyId(currentUserId);
            ViewBag.businessName = user.BusinessName;
            ViewBag.RelativeUrl = user.RelativeUrl;
            ViewBag.Logo = "/Assets/" + user.BusinessId + "/Logo_" + user.BusinessId + ".jpg";
            model.Name = user.FirstName + " " + user.LastName;
            model.Notifications = from p in db.LookupNotificationTypes
                                  join ap in db.UserNotificationMappers.Where(e => e.UserId == currentUserId) on p.Id equals ap.NotificationTypeId
                                  into joined
                                  from j in joined.DefaultIfEmpty()
                                  select new NotificationModel { NotificationTypeId = p.Id, NotificationType = p.Title, Status = j.Status == null ? true : j.Status };
            return View(model);
        }
        [HttpPost]
        public ActionResult Index(NotificationModel entity)
        {
            var currentUserId = User.Identity.GetUserId<int>();
            var model = db.UserNotificationMappers.FirstOrDefault(a => a.NotificationTypeId == entity.NotificationTypeId & a.UserId == currentUserId);
            if (model != null)
            {
                if (entity.Status != null)
                {
                    model.Status = entity.Status;
                    model.UpdatedBy = currentUserId;
                    model.UpdatedOn = DateTime.UtcNow;
                }
                db.SaveChanges();
            }
            else
            {
                db.UserNotificationMappers.Add(new UserNotificationMapper
                {
                    UserId = currentUserId,
                    NotificationTypeId = entity.NotificationTypeId,
                    Status = entity.Status,
                    CreatedBy = currentUserId,
                    CreatedOn = DateTime.UtcNow
                });
                db.SaveChanges();

            }
            return Json(true);
        }
    }
}