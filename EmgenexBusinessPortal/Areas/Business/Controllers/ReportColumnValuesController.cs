using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EBP.Business.Database;

namespace EmgenexBusinessPortal.Areas.Business.Controllers
{
    [BusinessAuthorize]
    public class ReportColumnValuesController : BaseController
    {
        private EmegenexBiz2016Entities db = new EmegenexBiz2016Entities();

        // GET: Business/ReportColumnValues
        public ActionResult Index()
        {
            var reportColumnValues = db.ReportColumnValues.Where(a=>a.ReportColumn.BusinessId==CurrentBusinessId).Include(r => r.User).Include(r => r.User1).Include(r => r.ReportMaster).Include(r => r.ReportColumn);
            return View(reportColumnValues.ToList());
        }

        // GET: Business/ReportColumnValues/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportColumnValue reportColumnValue = db.ReportColumnValues.Find(id);
            if (reportColumnValue == null)
            {
                return HttpNotFound();
            }
            return View(reportColumnValue);
        }

        // GET: Business/ReportColumnValues/Create
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.ReportId = new SelectList(db.ReportMasters, "Id", "Id");
            ViewBag.ColumnId = new SelectList(db.ReportColumns, "Id", "ColumnName");
            return View();
        }

        // POST: Business/ReportColumnValues/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ColumnId,ReportId,Value,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] ReportColumnValue reportColumnValue)
        {
            if (ModelState.IsValid)
            {
                reportColumnValue.CreatedBy = CurrentUserId;
                reportColumnValue.CreatedOn = System.DateTime.UtcNow;
                db.ReportColumnValues.Add(reportColumnValue);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName", reportColumnValue.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "FirstName", reportColumnValue.UpdatedBy);
            ViewBag.ReportId = new SelectList(db.ReportMasters.Where(a=>a.BusinessId==CurrentBusinessId), "Id", "Id", reportColumnValue.ReportId);
            ViewBag.ColumnId = new SelectList(db.ReportColumns.Where(a => a.BusinessId == CurrentBusinessId), "Id", "ColumnName", reportColumnValue.ColumnId);
            return View(reportColumnValue);
        }

        // GET: Business/ReportColumnValues/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportColumnValue reportColumnValue = db.ReportColumnValues.Find(id);
            if (reportColumnValue == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName", reportColumnValue.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "FirstName", reportColumnValue.UpdatedBy);
            ViewBag.ReportId = new SelectList(db.ReportMasters, "Id", "Id", reportColumnValue.ReportId);
            ViewBag.ColumnId = new SelectList(db.ReportColumns, "Id", "ColumnName", reportColumnValue.ColumnId);
            return View(reportColumnValue);
        }

        // POST: Business/ReportColumnValues/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ColumnId,ReportId,Value,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] ReportColumnValue reportColumnValue)
        {
            if (ModelState.IsValid)
            {
                reportColumnValue.UpdatedBy = CurrentUserId;
                reportColumnValue.UpdatedOn = System.DateTime.UtcNow;
                db.Entry(reportColumnValue).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName", reportColumnValue.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "FirstName", reportColumnValue.UpdatedBy);
            ViewBag.ReportId = new SelectList(db.ReportMasters, "Id", "Id", reportColumnValue.ReportId);
            ViewBag.ColumnId = new SelectList(db.ReportColumns, "Id", "ColumnName", reportColumnValue.ColumnId);
            return View(reportColumnValue);
        }

        // GET: Business/ReportColumnValues/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportColumnValue reportColumnValue = db.ReportColumnValues.Find(id);
            if (reportColumnValue == null)
            {
                return HttpNotFound();
            }
            return View(reportColumnValue);
        }

        // POST: Business/ReportColumnValues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ReportColumnValue reportColumnValue = db.ReportColumnValues.Find(id);
            db.ReportColumnValues.Remove(reportColumnValue);
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
