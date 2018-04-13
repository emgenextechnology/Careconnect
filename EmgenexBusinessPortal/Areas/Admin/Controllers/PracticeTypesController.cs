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
using EBP.Business.Resource;

namespace EmgenexBusinessPortal.Areas.Admin.Controllers
{
    [Authorize]
    public class PracticeTypesController : BaseController
    {
        //private EmegenexBiz2016Entities db = new EmegenexBiz2016Entities();

        // GET: Admin/PracticeTypes
        public ActionResult Index()
        {
            var lookupPracticeTypes = db.LookupPracticeTypes.Include(l => l.User).Include(l => l.User1);
            return View(lookupPracticeTypes.ToList());
        }

        // GET: Admin/PracticeTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupPracticeType lookupPracticeType = db.LookupPracticeTypes.Find(id);
            if (lookupPracticeType == null)
            {
                return HttpNotFound();
            }
            return View(lookupPracticeType);
        }

        // GET: Admin/PracticeTypes/Create
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName");
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        // POST: Admin/PracticeTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PracticeType,IsActive,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupPracticeType lookupPracticeType)
        {
            if (ModelState.IsValid)
            {
                lookupPracticeType.CreatedBy = CurrentUser.Id;
                lookupPracticeType.CreatedOn = DateTime.UtcNow;
                db.LookupPracticeTypes.Add(lookupPracticeType);
                db.SaveChanges();
                lookupPracticeType.ClearCache(CacheKeys.PracticeTypes);
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupPracticeType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupPracticeType.UpdatedBy);
            return View(lookupPracticeType);
        }

        // GET: Admin/PracticeTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupPracticeType lookupPracticeType = db.LookupPracticeTypes.Find(id);
            if (lookupPracticeType == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupPracticeType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupPracticeType.UpdatedBy);
            return View(lookupPracticeType);
        }

        // POST: Admin/PracticeTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PracticeType,IsActive,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupPracticeType lookupPracticeType)
        {
            if (ModelState.IsValid)
            {
                lookupPracticeType.UpdatedBy = CurrentUser.Id;
                lookupPracticeType.UpdatedOn = DateTime.UtcNow;
                db.Entry(lookupPracticeType).State = EntityState.Modified;
                db.SaveChanges();
                lookupPracticeType.ClearCache(CacheKeys.PracticeTypes);
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupPracticeType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupPracticeType.UpdatedBy);
            return View(lookupPracticeType);
        }

        // GET: Admin/PracticeTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupPracticeType lookupPracticeType = db.LookupPracticeTypes.Find(id);
            if (lookupPracticeType == null)
            {
                return HttpNotFound();
            }
            return View(lookupPracticeType);
        }

        // POST: Admin/PracticeTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LookupPracticeType lookupPracticeType = db.LookupPracticeTypes.Find(id);
            try
            {
                db.LookupPracticeTypes.Remove(lookupPracticeType);
                db.SaveChanges();
                lookupPracticeType.ClearCache(CacheKeys.PracticeTypes);
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("Error", "There are some releted item in database, please delete those first");
                return View("Delete", lookupPracticeType);
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
