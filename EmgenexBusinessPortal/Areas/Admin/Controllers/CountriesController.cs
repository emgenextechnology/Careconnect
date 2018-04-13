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
    public class CountriesController : BaseController
    {
        //private EmegenexBiz2016Entities db = new EmegenexBiz2016Entities();

        // GET: Admin/Countries
        public ActionResult Index()
        {
            var lookupCountries = db.LookupCountries.Include(l => l.User).Include(l => l.User1);
            return View(lookupCountries.ToList());
        }

        // GET: Admin/Countries/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupCountry lookupCountry = db.LookupCountries.Find(id);
            if (lookupCountry == null)
            {
                return HttpNotFound();
            }
            return View(lookupCountry);
        }

        // GET: Admin/Countries/Create
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName");
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        // POST: Admin/Countries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CountryCode,CountryName,IsActive,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupCountry lookupCountry)
        {
            if (ModelState.IsValid)
            {
                lookupCountry.CreatedBy = CurrentUser.Id;
                lookupCountry.CreatedOn = DateTime.UtcNow;
                db.LookupCountries.Add(lookupCountry);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupCountry.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupCountry.UpdatedBy);
            return View(lookupCountry);
        }

        // GET: Admin/Countries/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupCountry lookupCountry = db.LookupCountries.Find(id);
            if (lookupCountry == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupCountry.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupCountry.UpdatedBy);
            return View(lookupCountry);
        }

        // POST: Admin/Countries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CountryCode,CountryName,IsActive,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupCountry lookupCountry)
        {
            if (ModelState.IsValid)
            {
                lookupCountry.UpdatedBy = CurrentUser.Id;
                lookupCountry.UpdatedOn = DateTime.UtcNow;
                db.Entry(lookupCountry).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupCountry.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupCountry.UpdatedBy);
            return View(lookupCountry);
        }

        // GET: Admin/Countries/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupCountry lookupCountry = db.LookupCountries.Find(id);
            if (lookupCountry == null)
            {
                return HttpNotFound();
            }
            return View(lookupCountry);
        }

        // POST: Admin/Countries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LookupCountry lookupCountry = db.LookupCountries.Find(id);
            try
            {
                db.LookupCountries.Remove(lookupCountry);
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("Error", "There are some releted item in database, please delete those first");
                return View("Delete", lookupCountry);
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
