using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EBP.Business.Database;
using EBP.Business.Repository;

namespace EmgenexBusinessPortal.Areas.Business.Controllers
{
    public class ReportStaticColumnConfigsController : Controller
    {
        private CareConnectCrmEntities db = new CareConnectCrmEntities();

        // GET: Business/ReportStaticColumnConfigs
        public ActionResult Index()
        {
            //var reportStaticColumnConfigs = db.ReportStaticColumnConfigs.Include(r => r.LookupEnrolledService);
            var reportStaticColumnConfigs = new RepositorySales().GetReportStaticColumns(2);
            return View(reportStaticColumnConfigs.ToList());
        }

        // GET: Business/ReportStaticColumnConfigs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportStaticColumnConfig reportStaticColumnConfig = db.ReportStaticColumnConfigs.Find(id);
            if (reportStaticColumnConfig == null)
            {
                return HttpNotFound();
            }
            return View(reportStaticColumnConfig);
        }

        // GET: Business/ReportStaticColumnConfigs/Create
        public ActionResult Create()
        {
            ViewBag.ServiceId = new SelectList(db.LookupEnrolledServices, "Id", "ServiceName");
            return View();
        }

        // POST: Business/ReportStaticColumnConfigs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ColumnName,IsVisible,ServiceId,ColumnOrder")] ReportStaticColumnConfig reportStaticColumnConfig)
        {
            if (ModelState.IsValid)
            {
                db.ReportStaticColumnConfigs.Add(reportStaticColumnConfig);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ServiceId = new SelectList(db.LookupEnrolledServices, "Id", "ServiceName", reportStaticColumnConfig.ServiceId);
            return View(reportStaticColumnConfig);
        }

        // GET: Business/ReportStaticColumnConfigs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportStaticColumnConfig reportStaticColumnConfig = db.ReportStaticColumnConfigs.Find(id);
            if (reportStaticColumnConfig == null)
            {
                return HttpNotFound();
            }
            ViewBag.ServiceId = new SelectList(db.LookupEnrolledServices, "Id", "ServiceName", reportStaticColumnConfig.ServiceId);
            return View(reportStaticColumnConfig);
        }

        // POST: Business/ReportStaticColumnConfigs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ColumnName,IsVisible,ServiceId,ColumnOrder")] ReportStaticColumnConfig reportStaticColumnConfig)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reportStaticColumnConfig).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ServiceId = new SelectList(db.LookupEnrolledServices, "Id", "ServiceName", reportStaticColumnConfig.ServiceId);
            return View(reportStaticColumnConfig);
        }

        // GET: Business/ReportStaticColumnConfigs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportStaticColumnConfig reportStaticColumnConfig = db.ReportStaticColumnConfigs.Find(id);
            if (reportStaticColumnConfig == null)
            {
                return HttpNotFound();
            }
            return View(reportStaticColumnConfig);
        }

        // POST: Business/ReportStaticColumnConfigs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ReportStaticColumnConfig reportStaticColumnConfig = db.ReportStaticColumnConfigs.Find(id);
            db.ReportStaticColumnConfigs.Remove(reportStaticColumnConfig);
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
