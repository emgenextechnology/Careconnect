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
    public class RxcPlanTypesController : BaseController
    {
        private EmegenexBiz2016Entities db = new EmegenexBiz2016Entities();

        // GET: Admin/RxcPlanTypes
        public ActionResult Index()
        {
            var lookupRxcPlanTypes = db.LookupRxcPlanTypes.Include(l => l.User).Include(l => l.User1);
            return View(lookupRxcPlanTypes.ToList());
        }

        // GET: Admin/RxcPlanTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupRxcPlanType lookupRxcPlanType = db.LookupRxcPlanTypes.Find(id);
            if (lookupRxcPlanType == null)
            {
                return HttpNotFound();
            }
            return View(lookupRxcPlanType);
        }

        // GET: Admin/RxcPlanTypes/Create
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName");
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        // POST: Admin/RxcPlanTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,RxPlanType,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupRxcPlanType lookupRxcPlanType)
        {
            if (ModelState.IsValid)
            {
                lookupRxcPlanType.CreatedBy = CurrentUser.Id;
                lookupRxcPlanType.CreatedOn = DateTime.UtcNow;
                db.LookupRxcPlanTypes.Add(lookupRxcPlanType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupRxcPlanType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupRxcPlanType.UpdatedBy);
            return View(lookupRxcPlanType);
        }

        // GET: Admin/RxcPlanTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupRxcPlanType lookupRxcPlanType = db.LookupRxcPlanTypes.Find(id);
            if (lookupRxcPlanType == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupRxcPlanType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupRxcPlanType.UpdatedBy);
            return View(lookupRxcPlanType);
        }

        // POST: Admin/RxcPlanTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,RxPlanType,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupRxcPlanType lookupRxcPlanType)
        {
            if (ModelState.IsValid)
            {
                lookupRxcPlanType.UpdatedBy = CurrentUser.Id;
                lookupRxcPlanType.UpdatedOn = DateTime.UtcNow;
                db.Entry(lookupRxcPlanType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupRxcPlanType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupRxcPlanType.UpdatedBy);
            return View(lookupRxcPlanType);
        }

        // GET: Admin/RxcPlanTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupRxcPlanType lookupRxcPlanType = db.LookupRxcPlanTypes.Find(id);
            if (lookupRxcPlanType == null)
            {
                return HttpNotFound();
            }
            return View(lookupRxcPlanType);
        }

        // POST: Admin/RxcPlanTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LookupRxcPlanType lookupRxcPlanType = db.LookupRxcPlanTypes.Find(id);
            try
            {
                db.LookupRxcPlanTypes.Remove(lookupRxcPlanType);
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("Error", "There are some releted item in database, please delete those first");
                return View("Delete", lookupRxcPlanType);
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
