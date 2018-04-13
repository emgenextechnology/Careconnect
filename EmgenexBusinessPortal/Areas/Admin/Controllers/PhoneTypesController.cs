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
    public class PhoneTypesController : BaseController
    {
        private EmegenexBiz2016Entities db = new EmegenexBiz2016Entities();

        // GET: Admin/PhoneTypes
        public ActionResult Index()
        {
            var lookupPhoneTypes = db.LookupPhoneTypes.Include(l => l.User).Include(l => l.User1);
            return View(lookupPhoneTypes.ToList());
        }

        // GET: Admin/PhoneTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupPhoneType lookupPhoneType = db.LookupPhoneTypes.Find(id);
            if (lookupPhoneType == null)
            {
                return HttpNotFound();
            }
            return View(lookupPhoneType);
        }

        // GET: Admin/PhoneTypes/Create
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName");
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        // POST: Admin/PhoneTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PhoneType,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupPhoneType lookupPhoneType)
        {
            if (ModelState.IsValid)
            {
                lookupPhoneType.CreatedBy = CurrentUser.Id;
                lookupPhoneType.CreatedOn = DateTime.UtcNow;
                db.LookupPhoneTypes.Add(lookupPhoneType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupPhoneType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupPhoneType.UpdatedBy);
            return View(lookupPhoneType);
        }

        // GET: Admin/PhoneTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupPhoneType lookupPhoneType = db.LookupPhoneTypes.Find(id);
            if (lookupPhoneType == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupPhoneType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupPhoneType.UpdatedBy);
            return View(lookupPhoneType);
        }

        // POST: Admin/PhoneTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PhoneType,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupPhoneType lookupPhoneType)
        {
            if (ModelState.IsValid)
            {
                lookupPhoneType.UpdatedBy = CurrentUser.Id;
                lookupPhoneType.UpdatedOn = DateTime.UtcNow;
                db.Entry(lookupPhoneType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupPhoneType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupPhoneType.UpdatedBy);
            return View(lookupPhoneType);
        }

        // GET: Admin/PhoneTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupPhoneType lookupPhoneType = db.LookupPhoneTypes.Find(id);
            if (lookupPhoneType == null)
            {
                return HttpNotFound();
            }
            return View(lookupPhoneType);
        }

        // POST: Admin/PhoneTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LookupPhoneType lookupPhoneType = db.LookupPhoneTypes.Find(id);
            try
            {
                db.LookupPhoneTypes.Remove(lookupPhoneType);
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("Error", "There are some releted item in database, please delete those first");
                return View("Delete", lookupPhoneType);
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
