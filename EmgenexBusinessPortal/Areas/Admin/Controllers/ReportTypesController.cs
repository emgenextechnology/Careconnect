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
    public class ReportTypesController : BaseController
    {
        private EmegenexBiz2016Entities db = new EmegenexBiz2016Entities();

        // GET: Admin/ReportTypes
        public ActionResult Index()
        {
            var lookupReportTypes = db.LookupReportTypes.Include(l => l.User).Include(l => l.User1);
            return View(lookupReportTypes.ToList());
        }

        // GET: Admin/ReportTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupReportType lookupReportType = db.LookupReportTypes.Find(id);
            if (lookupReportType == null)
            {
                return HttpNotFound();
            }
            return View(lookupReportType);
        }

        // GET: Admin/ReportTypes/Create
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName");
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        // POST: Admin/ReportTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ReportType,ShortName,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupReportType lookupReportType)
        {
            if (ModelState.IsValid)
            {
                lookupReportType.CreatedBy = CurrentUser.Id;
                lookupReportType.CreatedOn = DateTime.UtcNow;
                db.LookupReportTypes.Add(lookupReportType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupReportType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupReportType.UpdatedBy);
            return View(lookupReportType);
        }

        // GET: Admin/ReportTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupReportType lookupReportType = db.LookupReportTypes.Find(id);
            if (lookupReportType == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupReportType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupReportType.UpdatedBy);
            return View(lookupReportType);
        }

        // POST: Admin/ReportTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ReportType,ShortName,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupReportType lookupReportType)
        {
            if (ModelState.IsValid)
            {
                lookupReportType.UpdatedBy = CurrentUser.Id;
                lookupReportType.UpdatedOn = DateTime.UtcNow;
                db.Entry(lookupReportType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupReportType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupReportType.UpdatedBy);
            return View(lookupReportType);
        }

        // GET: Admin/ReportTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupReportType lookupReportType = db.LookupReportTypes.Find(id);
            if (lookupReportType == null)
            {
                return HttpNotFound();
            }
            return View(lookupReportType);
        }

        // POST: Admin/ReportTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LookupReportType lookupReportType = db.LookupReportTypes.Find(id);
            try
            {
                db.LookupReportTypes.Remove(lookupReportType);
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("Error", "There are some releted item in database, please delete those first");
                return View("Delete", lookupReportType);
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
