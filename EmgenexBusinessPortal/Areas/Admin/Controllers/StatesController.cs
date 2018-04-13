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
    public class StatesController : BaseController
    {
        //private EmegenexBiz2016Entities db = new EmegenexBiz2016Entities();

        // GET: Admin/States
        public ActionResult Index()
        {
            var lookupStates = db.LookupStates.Include(l => l.LookupCountry).Include(l => l.User).Include(l => l.User1);
            return View(lookupStates.ToList());
        }

        // GET: Admin/States/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupState lookupState = db.LookupStates.Find(id);
            if (lookupState == null)
            {
                return HttpNotFound();
            }
            return View(lookupState);
        }

        // GET: Admin/States/Create
        public ActionResult Create()
        {
            ViewBag.CountryId = new SelectList(db.LookupCountries, "Id", "CountryCode");
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName");
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        // POST: Admin/States/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CountryId,StateCode,StateName,IsActive,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupState lookupState)
        {
            if (ModelState.IsValid)
            {
                lookupState.CreatedBy = CurrentUser.Id;
                lookupState.CreatedOn = DateTime.UtcNow;
                db.LookupStates.Add(lookupState);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CountryId = new SelectList(db.LookupCountries, "Id", "CountryCode", lookupState.CountryId);
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupState.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupState.UpdatedBy);
            return View(lookupState);
        }

        // GET: Admin/States/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupState lookupState = db.LookupStates.Find(id);
            if (lookupState == null)
            {
                return HttpNotFound();
            }
            ViewBag.CountryId = new SelectList(db.LookupCountries, "Id", "CountryCode", lookupState.CountryId);
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupState.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupState.UpdatedBy);
            return View(lookupState);
        }

        // POST: Admin/States/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CountryId,StateCode,StateName,IsActive,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupState lookupState)
        {
            if (ModelState.IsValid)
            {
                lookupState.UpdatedBy = CurrentUser.Id;
                lookupState.UpdatedOn = DateTime.UtcNow;
                db.Entry(lookupState).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CountryId = new SelectList(db.LookupCountries, "Id", "CountryCode", lookupState.CountryId);
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupState.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupState.UpdatedBy);
            return View(lookupState);
        }

        // GET: Admin/States/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupState lookupState = db.LookupStates.Find(id);
            if (lookupState == null)
            {
                return HttpNotFound();
            }
            return View(lookupState);
        }

        // POST: Admin/States/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        { LookupState lookupState = db.LookupStates.Find(id);
            try
            {
                db.LookupStates.Remove(lookupState);
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("Error", "There are some releted item in database, please delete those first");
                return View("Delete", lookupState);
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
