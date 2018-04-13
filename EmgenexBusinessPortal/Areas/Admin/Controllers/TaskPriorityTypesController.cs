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
    public class TaskPriorityTypesController : BaseController
    {
        private EmegenexBiz2016Entities db = new EmegenexBiz2016Entities();

        // GET: Admin/TaskPriorityTypes
        public ActionResult Index()
        {
            var lookupTaskPriorityTypes = db.LookupTaskPriorityTypes.Include(l => l.User).Include(l => l.User1);
            return View(lookupTaskPriorityTypes.ToList());
        }

        // GET: Admin/TaskPriorityTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupTaskPriorityType lookupTaskPriorityType = db.LookupTaskPriorityTypes.Find(id);
            if (lookupTaskPriorityType == null)
            {
                return HttpNotFound();
            }
            return View(lookupTaskPriorityType);
        }

        // GET: Admin/TaskPriorityTypes/Create
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName");
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        // POST: Admin/TaskPriorityTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PriorityType,IsActive,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupTaskPriorityType lookupTaskPriorityType)
        {
            if (ModelState.IsValid)
            {
                lookupTaskPriorityType.CreatedBy = CurrentUser.Id;
                lookupTaskPriorityType.CreatedOn = DateTime.UtcNow;
                db.LookupTaskPriorityTypes.Add(lookupTaskPriorityType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupTaskPriorityType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupTaskPriorityType.UpdatedBy);
            return View(lookupTaskPriorityType);
        }

        // GET: Admin/TaskPriorityTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupTaskPriorityType lookupTaskPriorityType = db.LookupTaskPriorityTypes.Find(id);
            if (lookupTaskPriorityType == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupTaskPriorityType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupTaskPriorityType.UpdatedBy);
            return View(lookupTaskPriorityType);
        }

        // POST: Admin/TaskPriorityTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PriorityType,IsActive,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupTaskPriorityType lookupTaskPriorityType)
        {
            if (ModelState.IsValid)
            {
                lookupTaskPriorityType.UpdatedBy = CurrentUser.Id;
                lookupTaskPriorityType.UpdatedOn = DateTime.UtcNow;
                db.Entry(lookupTaskPriorityType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupTaskPriorityType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupTaskPriorityType.UpdatedBy);
            return View(lookupTaskPriorityType);
        }

        // GET: Admin/TaskPriorityTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupTaskPriorityType lookupTaskPriorityType = db.LookupTaskPriorityTypes.Find(id);
            if (lookupTaskPriorityType == null)
            {
                return HttpNotFound();
            }
            return View(lookupTaskPriorityType);
        }

        // POST: Admin/TaskPriorityTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LookupTaskPriorityType lookupTaskPriorityType = db.LookupTaskPriorityTypes.Find(id);
            try
            {
                db.LookupTaskPriorityTypes.Remove(lookupTaskPriorityType);
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("Error", "There are some releted item in database, please delete those first");
                return View("Delete", lookupTaskPriorityType);
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
