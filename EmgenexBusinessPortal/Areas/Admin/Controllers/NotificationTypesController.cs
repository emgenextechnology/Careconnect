using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EBP.Business.Database;

namespace EmgenexBusinessPortal.Areas.Admin.Controllers
{
     [Authorize]
    public class NotificationTypesController : BaseController
    {
        private CareConnectCrmEntities db = new CareConnectCrmEntities();

        // GET: Admin/LookupNotificationTypes
        public ActionResult Index()
        {
            var lookupNotificationTypes = db.LookupNotificationTypes.Include(l => l.User).Include(l => l.User1);
            return View(lookupNotificationTypes.ToList());
        }

        // GET: Admin/LookupNotificationTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupNotificationType lookupNotificationType = db.LookupNotificationTypes.Find(id);
            if (lookupNotificationType == null)
            {
                return HttpNotFound();
            }
            return View(lookupNotificationType);
        }

        // GET: Admin/LookupNotificationTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/LookupNotificationTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy,NotificationKey")] LookupNotificationType lookupNotificationType)
        {
            if (ModelState.IsValid)
            {
                lookupNotificationType.CreatedBy = CurrentUser.Id;
                lookupNotificationType.CreatedOn = DateTime.UtcNow;
                db.LookupNotificationTypes.Add(lookupNotificationType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(lookupNotificationType);
        }

        // GET: Admin/LookupNotificationTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupNotificationType lookupNotificationType = db.LookupNotificationTypes.Find(id);
            if (lookupNotificationType == null)
            {
                return HttpNotFound();
            }
            return View(lookupNotificationType);
        }

        // POST: Admin/LookupNotificationTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy,NotificationKey")] LookupNotificationType lookupNotificationType)
        {
            if (ModelState.IsValid)
            {
                lookupNotificationType.UpdatedBy = CurrentUser.Id;
                lookupNotificationType.UpdatedOn = DateTime.UtcNow;
                db.Entry(lookupNotificationType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(lookupNotificationType);
        }

        // GET: Admin/LookupNotificationTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupNotificationType lookupNotificationType = db.LookupNotificationTypes.Find(id);
            if (lookupNotificationType == null)
            {
                return HttpNotFound();
            }
            return View(lookupNotificationType);
        }

        // POST: Admin/LookupNotificationTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LookupNotificationType lookupNotificationType = db.LookupNotificationTypes.Find(id);
            db.LookupNotificationTypes.Remove(lookupNotificationType);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
