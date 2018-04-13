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
    public class RepStatusController : BaseController
    {
        private EmegenexBiz2016Entities db = new EmegenexBiz2016Entities();

        // GET: Admin/RepStatus
        public ActionResult Index()
        {
            var lookupRepStatus = db.LookupRepStatus.Include(l => l.User).Include(l => l.User1);
            return View(lookupRepStatus.ToList());
        }

        // GET: Admin/RepStatus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupRepStatu lookupRepStatu = db.LookupRepStatus.Find(id);
            if (lookupRepStatu == null)
            {
                return HttpNotFound();
            }
            return View(lookupRepStatu);
        }

        // GET: Admin/RepStatus/Create
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName");
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        // POST: Admin/RepStatus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,StatusType,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupRepStatu lookupRepStatu)
        {
            if (ModelState.IsValid)
            {
                lookupRepStatu.CreatedBy = CurrentUser.Id;
                lookupRepStatu.CreatedOn = DateTime.UtcNow;
                db.LookupRepStatus.Add(lookupRepStatu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupRepStatu.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupRepStatu.UpdatedBy);
            return View(lookupRepStatu);
        }

        // GET: Admin/RepStatus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupRepStatu lookupRepStatu = db.LookupRepStatus.Find(id);
            if (lookupRepStatu == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupRepStatu.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupRepStatu.UpdatedBy);
            return View(lookupRepStatu);
        }

        // POST: Admin/RepStatus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,StatusType,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupRepStatu lookupRepStatu)
        {
            if (ModelState.IsValid)
            {
                lookupRepStatu.UpdatedBy = CurrentUser.Id;
                lookupRepStatu.UpdatedOn = DateTime.UtcNow;
                db.Entry(lookupRepStatu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupRepStatu.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupRepStatu.UpdatedBy);
            return View(lookupRepStatu);
        }

        // GET: Admin/RepStatus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupRepStatu lookupRepStatu = db.LookupRepStatus.Find(id);
            if (lookupRepStatu == null)
            {
                return HttpNotFound();
            }
            return View(lookupRepStatu);
        }

        // POST: Admin/RepStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LookupRepStatu lookupRepStatu = db.LookupRepStatus.Find(id);
            try
            {
                db.LookupRepStatus.Remove(lookupRepStatu);
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("Error", "There are some releted item in database, please delete those first");
                return View("Delete", lookupRepStatu);
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
