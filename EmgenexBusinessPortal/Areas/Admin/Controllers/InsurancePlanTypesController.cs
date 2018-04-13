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
    public class InsurancePlanTypesController : BaseController
    {
        private EmegenexBiz2016Entities db = new EmegenexBiz2016Entities();

        // GET: Admin/InsurancePlanTypes
        public ActionResult Index()
        {
            var lookupInsurancePlanTypes = db.LookupInsurancePlanTypes.Include(l => l.User).Include(l => l.User1);
            return View(lookupInsurancePlanTypes.ToList());
        }

        // GET: Admin/InsurancePlanTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupInsurancePlanType lookupInsurancePlanType = db.LookupInsurancePlanTypes.Find(id);
            if (lookupInsurancePlanType == null)
            {
                return HttpNotFound();
            }
            return View(lookupInsurancePlanType);
        }

        // GET: Admin/InsurancePlanTypes/Create
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName");
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        // POST: Admin/InsurancePlanTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,InsurancePlanTypeName,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupInsurancePlanType lookupInsurancePlanType)
        {
            if (ModelState.IsValid)
            {
                lookupInsurancePlanType.CreatedBy = CurrentUser.Id;
                lookupInsurancePlanType.CreatedOn = DateTime.UtcNow;
                db.LookupInsurancePlanTypes.Add(lookupInsurancePlanType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupInsurancePlanType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupInsurancePlanType.UpdatedBy);
            return View(lookupInsurancePlanType);
        }

        // GET: Admin/InsurancePlanTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupInsurancePlanType lookupInsurancePlanType = db.LookupInsurancePlanTypes.Find(id);
            if (lookupInsurancePlanType == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupInsurancePlanType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupInsurancePlanType.UpdatedBy);
            return View(lookupInsurancePlanType);
        }

        // POST: Admin/InsurancePlanTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,InsurancePlanTypeName,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupInsurancePlanType lookupInsurancePlanType)
        {
            if (ModelState.IsValid)
            {
                lookupInsurancePlanType.UpdatedBy = CurrentUser.Id;
                lookupInsurancePlanType.UpdatedOn = DateTime.UtcNow;
                db.Entry(lookupInsurancePlanType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupInsurancePlanType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupInsurancePlanType.UpdatedBy);
            return View(lookupInsurancePlanType);
        }

        // GET: Admin/InsurancePlanTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupInsurancePlanType lookupInsurancePlanType = db.LookupInsurancePlanTypes.Find(id);
            if (lookupInsurancePlanType == null)
            {
                return HttpNotFound();
            }
            return View(lookupInsurancePlanType);
        }

        // POST: Admin/InsurancePlanTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LookupInsurancePlanType lookupInsurancePlanType = db.LookupInsurancePlanTypes.Find(id);
            try
            {
                db.LookupInsurancePlanTypes.Remove(lookupInsurancePlanType);
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("Error", "There are some releted item in database, please delete those first");
                return View("Delete", lookupInsurancePlanType);
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
