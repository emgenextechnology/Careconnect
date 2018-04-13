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
    public class AddressTypesController : BaseController
    {
        private EmegenexBiz2016Entities db = new EmegenexBiz2016Entities();

        // GET: Admin/AddressTypes
        public ActionResult Index()
        {
            var lookupAddressTypes = db.LookupAddressTypes.Include(l => l.User).Include(l => l.User1);
            return View(lookupAddressTypes.ToList());
        }

        // GET: Admin/AddressTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupAddressType lookupAddressType = db.LookupAddressTypes.Find(id);
            if (lookupAddressType == null)
            {
                return HttpNotFound();
            }
            return View(lookupAddressType);
        }

        // GET: Admin/AddressTypes/Create
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "FirstName");
            return View();
        }

        // POST: Admin/AddressTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AddressType,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupAddressType lookupAddressType)
        {
            if (ModelState.IsValid)
            {
                lookupAddressType.CreatedBy = CurrentUser.Id;
                lookupAddressType.CreatedOn = DateTime.UtcNow;
                db.LookupAddressTypes.Add(lookupAddressType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName", lookupAddressType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "FirstName", lookupAddressType.UpdatedBy);
            return View(lookupAddressType);
        }

        // GET: Admin/AddressTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupAddressType lookupAddressType = db.LookupAddressTypes.Find(id);
            if (lookupAddressType == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName", lookupAddressType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "FirstName", lookupAddressType.UpdatedBy);
            return View(lookupAddressType);
        }

        // POST: Admin/AddressTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AddressType,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupAddressType lookupAddressType)
        {
            if (ModelState.IsValid)
            {
                lookupAddressType.UpdatedBy = CurrentUser.Id;
                lookupAddressType.UpdatedOn = DateTime.UtcNow;
                db.Entry(lookupAddressType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName", lookupAddressType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "FirstName", lookupAddressType.UpdatedBy);
            return View(lookupAddressType);
        }

        // GET: Admin/AddressTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupAddressType lookupAddressType = db.LookupAddressTypes.Find(id);
            if (lookupAddressType == null)
            {
                return HttpNotFound();
            }
            return View(lookupAddressType);
        }

        // POST: Admin/AddressTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LookupAddressType lookupAddressType = db.LookupAddressTypes.Find(id);
            try
            {
                db.LookupAddressTypes.Remove(lookupAddressType);
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("Error", "There are some releted item in database, please delete those first");
                return View("Delete", lookupAddressType);
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
