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
    public class PgxStatusController : BaseController
    {
        private EmegenexBiz2016Entities db = new EmegenexBiz2016Entities();

        // GET: Admin/PgxStatus
        public ActionResult Index()
        {
            var lookupPgxStatus = db.LookupPgxStatus.Include(l => l.User).Include(l => l.User1);
            return View(lookupPgxStatus.ToList());
        }

        // GET: Admin/PgxStatus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupPgxStatu lookupPgxStatu = db.LookupPgxStatus.Find(id);
            if (lookupPgxStatu == null)
            {
                return HttpNotFound();
            }
            return View(lookupPgxStatu);
        }

        // GET: Admin/PgxStatus/Create
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName");
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        // POST: Admin/PgxStatus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,StatusType,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupPgxStatu lookupPgxStatu)
        {
            if (ModelState.IsValid)
            {
                lookupPgxStatu.CreatedBy = CurrentUser.Id;
                lookupPgxStatu.CreatedOn = DateTime.UtcNow;
                db.LookupPgxStatus.Add(lookupPgxStatu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupPgxStatu.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupPgxStatu.UpdatedBy);
            return View(lookupPgxStatu);
        }

        // GET: Admin/PgxStatus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupPgxStatu lookupPgxStatu = db.LookupPgxStatus.Find(id);
            if (lookupPgxStatu == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupPgxStatu.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupPgxStatu.UpdatedBy);
            return View(lookupPgxStatu);
        }

        // POST: Admin/PgxStatus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,StatusType,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupPgxStatu lookupPgxStatu)
        {
            if (ModelState.IsValid)
            {
                lookupPgxStatu.UpdatedBy = CurrentUser.Id;
                lookupPgxStatu.UpdatedOn = DateTime.UtcNow;
                db.Entry(lookupPgxStatu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupPgxStatu.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupPgxStatu.UpdatedBy);
            return View(lookupPgxStatu);
        }

        // GET: Admin/PgxStatus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupPgxStatu lookupPgxStatu = db.LookupPgxStatus.Find(id);
            if (lookupPgxStatu == null)
            {
                return HttpNotFound();
            }
            return View(lookupPgxStatu);
        }

        // POST: Admin/PgxStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LookupPgxStatu lookupPgxStatu = db.LookupPgxStatus.Find(id);
            try
            {
                db.LookupPgxStatus.Remove(lookupPgxStatu);
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("Error", "There are some releted item in database, please delete those first");
                return View("Delete", lookupPgxStatu);
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
