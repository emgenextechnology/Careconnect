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
using EBP.Business.Resource;

namespace EmgenexBusinessPortal.Areas.Admin.Controllers
{
    [Authorize]
    public class DegreesController : BaseController
    {
       

        // GET: Admin/Degrees
        public ActionResult Index()
        {
            var lookupDegrees = db.LookupDegrees.Include(l => l.User).Include(l => l.User1);
            return View(lookupDegrees.ToList());
        }

        // GET: Admin/Degrees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupDegree lookupDegree = db.LookupDegrees.Find(id);
            if (lookupDegree == null)
            {
                return HttpNotFound();
            }
            return View(lookupDegree);
        }

        // GET: Admin/Degrees/Create
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName");
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        // POST: Admin/Degrees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,DegreeName,IsActive,CreatedOn,ShortCode,CreatedBy,UpdatedOn,UpdatedBy")] LookupDegree lookupDegree)
        {
            if (ModelState.IsValid)
            {
                lookupDegree.CreatedBy = CurrentUser.Id;
                lookupDegree.CreatedOn = DateTime.UtcNow;
                db.LookupDegrees.Add(lookupDegree);
                db.SaveChanges();
                lookupDegree.ClearCache(CacheKeys.Degrees);
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupDegree.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupDegree.UpdatedBy);
            return View(lookupDegree);
        }

        // GET: Admin/Degrees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupDegree lookupDegree = db.LookupDegrees.Find(id);
            if (lookupDegree == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupDegree.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupDegree.UpdatedBy);
            return View(lookupDegree);
        }

        // POST: Admin/Degrees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,DegreeName,IsActive,CreatedOn,ShortCode,CreatedBy,UpdatedOn,UpdatedBy")] LookupDegree lookupDegree)
        {
            if (ModelState.IsValid)
            {
                lookupDegree.UpdatedBy = CurrentUser.Id;
                lookupDegree.UpdatedOn = DateTime.UtcNow;
                db.Entry(lookupDegree).State = EntityState.Modified;
                db.SaveChanges();
                lookupDegree.ClearCache(CacheKeys.Degrees);
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupDegree.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupDegree.UpdatedBy);
            return View(lookupDegree);
        }

        // GET: Admin/Degrees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupDegree lookupDegree = db.LookupDegrees.Find(id);
            if (lookupDegree == null)
            {
                return HttpNotFound();
            }
            return View(lookupDegree);
        }

        // POST: Admin/Degrees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LookupDegree lookupDegree = db.LookupDegrees.Find(id);
            try
            {
                db.LookupDegrees.Remove(lookupDegree);
                db.SaveChanges();
                lookupDegree.ClearCache(CacheKeys.Degrees);
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("Error", "There are some releted item in database, please delete those first");
                return View("Delete", lookupDegree);
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
