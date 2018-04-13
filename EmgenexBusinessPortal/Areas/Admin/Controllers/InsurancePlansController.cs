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
    public class InsurancePlansController : BaseController
    {
        private EmegenexBiz2016Entities db = new EmegenexBiz2016Entities();

        // GET: Admin/InsurancePlans
        public ActionResult Index()
        {
            var lookupInsurancePlans = db.LookupInsurancePlans.Include(l => l.User).Include(l => l.User1);
            return View(lookupInsurancePlans.ToList());
        }

        // GET: Admin/InsurancePlans/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupInsurancePlan lookupInsurancePlan = db.LookupInsurancePlans.Find(id);
            if (lookupInsurancePlan == null)
            {
                return HttpNotFound();
            }
            return View(lookupInsurancePlan);
        }

        // GET: Admin/InsurancePlans/Create
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName");
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        // POST: Admin/InsurancePlans/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,InsurancePlanName,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupInsurancePlan lookupInsurancePlan)
        {
            if (ModelState.IsValid)
            {
                lookupInsurancePlan.CreatedBy = CurrentUser.Id;
                lookupInsurancePlan.CreatedOn = DateTime.UtcNow;
                db.LookupInsurancePlans.Add(lookupInsurancePlan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupInsurancePlan.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupInsurancePlan.UpdatedBy);
            return View(lookupInsurancePlan);
        }

        // GET: Admin/InsurancePlans/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupInsurancePlan lookupInsurancePlan = db.LookupInsurancePlans.Find(id);
            if (lookupInsurancePlan == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupInsurancePlan.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupInsurancePlan.UpdatedBy);
            return View(lookupInsurancePlan);
        }

        // POST: Admin/InsurancePlans/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,InsurancePlanName,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupInsurancePlan lookupInsurancePlan)
        {
            if (ModelState.IsValid)
            {
                lookupInsurancePlan.UpdatedBy = CurrentUser.Id;
                lookupInsurancePlan.UpdatedOn = DateTime.UtcNow;
                db.Entry(lookupInsurancePlan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupInsurancePlan.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupInsurancePlan.UpdatedBy);
            return View(lookupInsurancePlan);
        }

        // GET: Admin/InsurancePlans/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupInsurancePlan lookupInsurancePlan = db.LookupInsurancePlans.Find(id);
            if (lookupInsurancePlan == null)
            {
                return HttpNotFound();
            }
            return View(lookupInsurancePlan);
        }

        // POST: Admin/InsurancePlans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LookupInsurancePlan lookupInsurancePlan = db.LookupInsurancePlans.Find(id);
            try
            {
                db.LookupInsurancePlans.Remove(lookupInsurancePlan);
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("Error", "There are some releted item in database, please delete those first");
                return View("Delete", lookupInsurancePlan);
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
