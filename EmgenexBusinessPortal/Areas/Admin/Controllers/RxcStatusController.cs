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
    public class RxcStatusController : BaseController
    {
        private EmegenexBiz2016Entities db = new EmegenexBiz2016Entities();

        // GET: Admin/RxcStatus
        public ActionResult Index()
        {
            var lookupRxcStatus = db.LookupRxcStatus.Include(l => l.User).Include(l => l.User1);
            return View(lookupRxcStatus.ToList());
        }

        // GET: Admin/RxcStatus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupRxcStatu lookupRxcStatu = db.LookupRxcStatus.Find(id);
            if (lookupRxcStatu == null)
            {
                return HttpNotFound();
            }
            return View(lookupRxcStatu);
        }

        // GET: Admin/RxcStatus/Create
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName");
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        // POST: Admin/RxcStatus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,StatusType,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupRxcStatu lookupRxcStatu)
        {
            if (ModelState.IsValid)
            {
                lookupRxcStatu.CreatedBy = CurrentUser.Id;
                lookupRxcStatu.CreatedOn = DateTime.UtcNow;
                db.LookupRxcStatus.Add(lookupRxcStatu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupRxcStatu.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupRxcStatu.UpdatedBy);
            return View(lookupRxcStatu);
        }

        // GET: Admin/RxcStatus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupRxcStatu lookupRxcStatu = db.LookupRxcStatus.Find(id);
            if (lookupRxcStatu == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupRxcStatu.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupRxcStatu.UpdatedBy);
            return View(lookupRxcStatu);
        }

        // POST: Admin/RxcStatus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,StatusType,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupRxcStatu lookupRxcStatu)
        {
            if (ModelState.IsValid)
            {
                lookupRxcStatu.UpdatedBy = CurrentUser.Id;
                lookupRxcStatu.UpdatedOn = DateTime.UtcNow;
                db.Entry(lookupRxcStatu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupRxcStatu.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupRxcStatu.UpdatedBy);
            return View(lookupRxcStatu);
        }

        // GET: Admin/RxcStatus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupRxcStatu lookupRxcStatu = db.LookupRxcStatus.Find(id);
            if (lookupRxcStatu == null)
            {
                return HttpNotFound();
            }
            return View(lookupRxcStatu);
        }

        // POST: Admin/RxcStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LookupRxcStatu lookupRxcStatu = db.LookupRxcStatus.Find(id);
            try
            {
                db.LookupRxcStatus.Remove(lookupRxcStatu);
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("Error", "There are some releted item in database, please delete those first");
                return View("Delete", lookupRxcStatu);
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
