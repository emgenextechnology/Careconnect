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
    public class RepGroupStatusController : BaseController
    {
        private EmegenexBiz2016Entities db = new EmegenexBiz2016Entities();

        // GET: Admin/RepGroupStatus
        public ActionResult Index()
        {
            var lookupRepGroupStatus = db.LookupRepGroupStatus.Include(l => l.User).Include(l => l.User1);
            return View(lookupRepGroupStatus.ToList());
        }

        // GET: Admin/RepGroupStatus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupRepGroupStatu lookupRepGroupStatu = db.LookupRepGroupStatus.Find(id);
            if (lookupRepGroupStatu == null)
            {
                return HttpNotFound();
            }
            return View(lookupRepGroupStatu);
        }

        // GET: Admin/RepGroupStatus/Create
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName");
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        // POST: Admin/RepGroupStatus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,StatusType,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupRepGroupStatu lookupRepGroupStatu)
        {
            if (ModelState.IsValid)
            {
                lookupRepGroupStatu.CreatedBy = CurrentUser.Id;
                lookupRepGroupStatu.CreatedOn = DateTime.UtcNow;
                db.LookupRepGroupStatus.Add(lookupRepGroupStatu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupRepGroupStatu.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupRepGroupStatu.UpdatedBy);
            return View(lookupRepGroupStatu);
        }

        // GET: Admin/RepGroupStatus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupRepGroupStatu lookupRepGroupStatu = db.LookupRepGroupStatus.Find(id);
            if (lookupRepGroupStatu == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupRepGroupStatu.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupRepGroupStatu.UpdatedBy);
            return View(lookupRepGroupStatu);
        }

        // POST: Admin/RepGroupStatus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,StatusType,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupRepGroupStatu lookupRepGroupStatu)
        {
            if (ModelState.IsValid)
            {
                lookupRepGroupStatu.UpdatedBy = CurrentUser.Id;
                lookupRepGroupStatu.UpdatedOn = DateTime.UtcNow;
                db.Entry(lookupRepGroupStatu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupRepGroupStatu.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupRepGroupStatu.UpdatedBy);
            return View(lookupRepGroupStatu);
        }

        // GET: Admin/RepGroupStatus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupRepGroupStatu lookupRepGroupStatu = db.LookupRepGroupStatus.Find(id);
            if (lookupRepGroupStatu == null)
            {
                return HttpNotFound();
            }
            return View(lookupRepGroupStatu);
        }

        // POST: Admin/RepGroupStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
                LookupRepGroupStatu lookupRepGroupStatu = db.LookupRepGroupStatus.Find(id);
            try
            {
                db.LookupRepGroupStatus.Remove(lookupRepGroupStatu);
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("Error", "There are some releted item in database, please delete those first");
                return View("Delete", lookupRepGroupStatu);
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
