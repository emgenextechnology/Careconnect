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
    public class ReportMastersController : BaseController
    {
        private EmegenexBiz2016Entities db = new EmegenexBiz2016Entities();

        // GET: Business/ReportMasters
        public ActionResult Index()
        {
            var reportMasters = db.ReportMasters.Where(a=>a.BusinessId==CurrentBusinessId).Include(r => r.BusinessMaster).Include(r => r.LookupEnrolledService).Include(r => r.Practice).Include(r => r.Provider).Include(r => r.User).Include(r => r.User1).Include(r => r.User2);
            return View(reportMasters.ToList());
        }

        // GET: Business/ReportMasters/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportMaster reportMaster = db.ReportMasters.Find(id);
            if (reportMaster == null)
            {
                return HttpNotFound();
            }
            return View(reportMaster);
        }

        // GET: Business/ReportMasters/Create
        public ActionResult Create()
        {
            ViewBag.ServiceId = new SelectList(db.LookupEnrolledServices.Where(a=>a.BusinessId==CurrentBusinessId), "Id", "ServiceName");
            ViewBag.PracticeId = new SelectList(db.Practices, "Id", "PracticeName");
            ViewBag.ProviderId = new SelectList(db.Providers, "Id", "FirstName");
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.RepId = new SelectList(db.Users.Where(a => a.BusinessId == CurrentBusinessId && a.IsActive == true), "Id", "FirstName");
            return View();
        }

        // POST: Business/ReportMasters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,BusinessId,ServiceId,ProviderId,PracticeId,RepId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] ReportMaster reportMaster)
        {
            if (ModelState.IsValid)
            {
                reportMaster.BusinessId =(int) CurrentBusinessId;
                reportMaster.CreatedBy = CurrentUserId;
                reportMaster.CreatedOn = System.DateTime.UtcNow;
                db.ReportMasters.Add(reportMaster);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ServiceId = new SelectList(db.LookupEnrolledServices.Where(a => a.BusinessId == CurrentBusinessId), "Id", "ServiceName");
            ViewBag.PracticeId = new SelectList(db.Practices, "Id", "PracticeName");
            ViewBag.ProviderId = new SelectList(db.Providers, "Id", "FirstName");
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.RepId = new SelectList(db.Users.Where(a => a.BusinessId == CurrentBusinessId && a.IsActive == true), "Id", "FirstName");
            return View(reportMaster);
        }

        // GET: Business/ReportMasters/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportMaster reportMaster = db.ReportMasters.Find(id);
            if (reportMaster == null)
            {
                return HttpNotFound();
            }
            ViewBag.ServiceId = new SelectList(db.LookupEnrolledServices.Where(a => a.BusinessId == CurrentBusinessId), "Id", "ServiceName", reportMaster.ServiceId);
            ViewBag.PracticeId = new SelectList(db.Practices, "Id", "PracticeName", reportMaster.PracticeId);
            ViewBag.ProviderId = new SelectList(db.Providers, "Id", "FirstName", reportMaster.ProviderId);
            ViewBag.RepId = new SelectList(db.Users.Where(a => a.BusinessId == CurrentBusinessId && a.IsActive == true), "Id", "FirstName", reportMaster.RepId);
            return View(reportMaster);
        }

        // POST: Business/ReportMasters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BusinessId,ServiceId,ProviderId,PracticeId,RepId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] ReportMaster reportMaster)
        {
            if (ModelState.IsValid)
            {
                reportMaster.UpdatedBy = CurrentUserId;
                reportMaster.UpdatedOn = System.DateTime.UtcNow;
                db.Entry(reportMaster).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ServiceId = new SelectList(db.LookupEnrolledServices.Where(a => a.BusinessId == CurrentBusinessId), "Id", "ServiceName", reportMaster.ServiceId);
            ViewBag.PracticeId = new SelectList(db.Practices, "Id", "PracticeName", reportMaster.PracticeId);
            ViewBag.ProviderId = new SelectList(db.Providers, "Id", "FirstName", reportMaster.ProviderId);
            ViewBag.RepId = new SelectList(db.Users.Where(a => a.BusinessId == CurrentBusinessId && a.IsActive == true), "Id", "FirstName", reportMaster.RepId);
            return View(reportMaster);
        }

        // GET: Business/ReportMasters/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportMaster reportMaster = db.ReportMasters.Find(id);
            if (reportMaster == null)
            {
                return HttpNotFound();
            }
            return View(reportMaster);
        }

        // POST: Business/ReportMasters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ReportMaster reportMaster = db.ReportMasters.Find(id);
            db.ReportMasters.Remove(reportMaster);
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
