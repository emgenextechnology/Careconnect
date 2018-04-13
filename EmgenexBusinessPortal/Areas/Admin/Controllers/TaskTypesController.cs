using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EBP.Business.Database;
using System.Data.Entity.Infrastructure;

namespace EmgenexBusinessPortal.Areas.Admin.Controllers
{
    [Authorize]
    public class TaskTypesController : BaseController
    {
        //private EmegenexBiz2016Entities db = new EmegenexBiz2016Entities();

        // GET: Admin/TaskTypes
        public ActionResult Index()
        {
            var lookupTaskTypes = db.LookupTaskTypes.Include(l => l.User).Include(l => l.User1);
            return View(lookupTaskTypes.ToList());
        }

        // GET: Admin/TaskTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupTaskType lookupTaskType = db.LookupTaskTypes.Find(id);
            if (lookupTaskType == null)
            {
                return HttpNotFound();
            }
            return View(lookupTaskType);
        }

        // GET: Admin/TaskTypes/Create
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName");
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        // POST: Admin/TaskTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TaskType,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupTaskType lookupTaskType)
        {
            if (ModelState.IsValid)
            {
                lookupTaskType.CreatedBy = CurrentUser.Id;
                lookupTaskType.CreatedOn = DateTime.UtcNow;
                db.LookupTaskTypes.Add(lookupTaskType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupTaskType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupTaskType.UpdatedBy);
            return View(lookupTaskType);
        }

        // GET: Admin/TaskTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupTaskType lookupTaskType = db.LookupTaskTypes.Find(id);
            if (lookupTaskType == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupTaskType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupTaskType.UpdatedBy);
            return View(lookupTaskType);
        }

        // POST: Admin/TaskTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TaskType,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupTaskType lookupTaskType)
        {
            if (ModelState.IsValid)
            {
                lookupTaskType.UpdatedBy = CurrentUser.Id;
                lookupTaskType.UpdatedOn = DateTime.UtcNow;
                db.Entry(lookupTaskType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupTaskType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupTaskType.UpdatedBy);
            return View(lookupTaskType);
        }

        // GET: Admin/TaskTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupTaskType lookupTaskType = db.LookupTaskTypes.Find(id);
            if (lookupTaskType == null)
            {
                return HttpNotFound();
            }
            return View(lookupTaskType);
        }

        // POST: Admin/TaskTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LookupTaskType lookupTaskType = db.LookupTaskTypes.Find(id);
            try
            {
                db.LookupTaskTypes.Remove(lookupTaskType);
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("Error", "There are some releted item in database, please delete those first");
                return View("Delete", lookupTaskType);
            }
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
