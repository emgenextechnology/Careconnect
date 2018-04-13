using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EmgenexBusinessPortal.Areas.Admin.Database;

namespace EmgenexBusinessPortal.Areas.Admin.Controllers
{
    public class ServicesController : Controller
    {
        private EmgenBiz2016Entities db = new EmgenBiz2016Entities();

        // GET: Admin/Services
        public ActionResult Index()
        {
            var lookupServices = db.LookupEnrolledServices.Include(l => l.User).Include(l => l.User1);
            return View(lookupServices.ToList());
        }

        // GET: Admin/Services/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupEnrolledService lookupService = db.LookupEnrolledServices.Find(id);
            if (lookupService == null)
            {
                return HttpNotFound();
            }
            return View(lookupService);
        }

        // GET: Admin/Services/Create
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName");
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        // POST: Admin/Services/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ServiceName,ServiceDecription,StatusId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupEnrolledService lookupService)
        {
            if (ModelState.IsValid)
            {
                lookupService.CreatedBy = 1;
                lookupService.CreatedOn = DateTime.Now;
                db.LookupEnrolledServices.Add(lookupService);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupService.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupService.UpdatedBy);
            return View(lookupService);
        }

        // GET: Admin/Services/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupEnrolledService lookupService = db.LookupEnrolledServices.Find(id);
            if (lookupService == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupService.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupService.UpdatedBy);
            return View(lookupService);
        }

        // POST: Admin/Services/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ServiceName,ServiceDecription,StatusId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupEnrolledService lookupService)
        {
            if (ModelState.IsValid)
            {
                lookupService.UpdatedBy = 1;
                lookupService.UpdatedOn = DateTime.Now;
                db.Entry(lookupService).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupService.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupService.UpdatedBy);
            return View(lookupService);
        }

        // GET: Admin/Services/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupEnrolledService lookupService = db.LookupEnrolledServices.Find(id);
            if (lookupService == null)
            {
                return HttpNotFound();
            }
            return View(lookupService);
        }

        // POST: Admin/Services/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LookupEnrolledService lookupService = db.LookupEnrolledServices.Find(id);
            db.LookupEnrolledServices.Remove(lookupService);
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
