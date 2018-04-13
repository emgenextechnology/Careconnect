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
    public class PaymentTypesController : BaseController
    {
        private EmegenexBiz2016Entities db = new EmegenexBiz2016Entities();

        // GET: Admin/PaymentTypes
        public ActionResult Index()
        {
            var lookupPaymentTypes = db.LookupPaymentTypes.Include(l => l.User).Include(l => l.User1);
            return View(lookupPaymentTypes.ToList());
        }

        // GET: Admin/PaymentTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupPaymentType lookupPaymentType = db.LookupPaymentTypes.Find(id);
            if (lookupPaymentType == null)
            {
                return HttpNotFound();
            }
            return View(lookupPaymentType);
        }

        // GET: Admin/PaymentTypes/Create
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName");
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        // POST: Admin/PaymentTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PaymentType,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupPaymentType lookupPaymentType)
        {
            if (ModelState.IsValid)
            {
                lookupPaymentType.CreatedBy = CurrentUser.Id;
                lookupPaymentType.CreatedOn = DateTime.UtcNow;
                db.LookupPaymentTypes.Add(lookupPaymentType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupPaymentType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupPaymentType.UpdatedBy);
            return View(lookupPaymentType);
        }

        // GET: Admin/PaymentTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupPaymentType lookupPaymentType = db.LookupPaymentTypes.Find(id);
            if (lookupPaymentType == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupPaymentType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupPaymentType.UpdatedBy);
            return View(lookupPaymentType);
        }

        // POST: Admin/PaymentTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PaymentType,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupPaymentType lookupPaymentType)
        {
            if (ModelState.IsValid)
            {
                lookupPaymentType.UpdatedBy = CurrentUser.Id;
                lookupPaymentType.UpdatedOn = DateTime.UtcNow;
                db.Entry(lookupPaymentType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupPaymentType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupPaymentType.UpdatedBy);
            return View(lookupPaymentType);
        }

        // GET: Admin/PaymentTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupPaymentType lookupPaymentType = db.LookupPaymentTypes.Find(id);
            if (lookupPaymentType == null)
            {
                return HttpNotFound();
            }
            return View(lookupPaymentType);
        }

        // POST: Admin/PaymentTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LookupPaymentType lookupPaymentType = db.LookupPaymentTypes.Find(id);
            try
            {
                db.LookupPaymentTypes.Remove(lookupPaymentType);
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("Error", "There are some releted item in database, please delete those first");
                return View("Delete", lookupPaymentType);
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
