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
    public class TxcStatusController : BaseController
    {
        private EmegenexBiz2016Entities db = new EmegenexBiz2016Entities();

        // GET: Admin/TxcStatus
        public ActionResult Index()
        {
            var lookupTxcStatus = db.LookupTxcStatus.Include(l => l.User).Include(l => l.User1);
            return View(lookupTxcStatus.ToList());
        }

        // GET: Admin/TxcStatus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupTxcStatu lookupTxcStatu = db.LookupTxcStatus.Find(id);
            if (lookupTxcStatu == null)
            {
                return HttpNotFound();
            }
            return View(lookupTxcStatu);
        }

        // GET: Admin/TxcStatus/Create
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName");
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        // POST: Admin/TxcStatus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,StatusType,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupTxcStatu lookupTxcStatu)
        {
            if (ModelState.IsValid)
            {
                lookupTxcStatu.CreatedBy = CurrentUser.Id;
                lookupTxcStatu.CreatedOn = DateTime.UtcNow;
                db.LookupTxcStatus.Add(lookupTxcStatu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupTxcStatu.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupTxcStatu.UpdatedBy);
            return View(lookupTxcStatu);
        }

        // GET: Admin/TxcStatus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupTxcStatu lookupTxcStatu = db.LookupTxcStatus.Find(id);
            if (lookupTxcStatu == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupTxcStatu.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupTxcStatu.UpdatedBy);
            return View(lookupTxcStatu);
        }

        // POST: Admin/TxcStatus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,StatusType,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupTxcStatu lookupTxcStatu)
        {
            if (ModelState.IsValid)
            {
                lookupTxcStatu.UpdatedBy = CurrentUser.Id;
                lookupTxcStatu.UpdatedOn = DateTime.UtcNow;
                db.Entry(lookupTxcStatu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupTxcStatu.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupTxcStatu.UpdatedBy);
            return View(lookupTxcStatu);
        }

        // GET: Admin/TxcStatus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupTxcStatu lookupTxcStatu = db.LookupTxcStatus.Find(id);
            if (lookupTxcStatu == null)
            {
                return HttpNotFound();
            }
            return View(lookupTxcStatu);
        }

        // POST: Admin/TxcStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LookupTxcStatu lookupTxcStatu = db.LookupTxcStatus.Find(id);
            try
            {
                db.LookupTxcStatus.Remove(lookupTxcStatu);
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("Error", "There are some releted item in database, please delete those first");
                return View("Delete", lookupTxcStatu);
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
