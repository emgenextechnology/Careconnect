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
    public class PracticeSpecialityTypesController : BaseController
    {
        //private EmegenexBiz2016Entities db = new EmegenexBiz2016Entities();

        // GET: Admin/PracticeSpecialityTypes
        public ActionResult Index()
        {
            var lookupPracticeSpecialityTypes = db.LookupPracticeSpecialityTypes.Include(l => l.User).Include(l => l.User1);
            return View(lookupPracticeSpecialityTypes.ToList());
        }

        // GET: Admin/PracticeSpecialityTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupPracticeSpecialityType lookupPracticeSpecialityType = db.LookupPracticeSpecialityTypes.Find(id);
            if (lookupPracticeSpecialityType == null)
            {
                return HttpNotFound();
            }
            return View(lookupPracticeSpecialityType);
        }

        // GET: Admin/PracticeSpecialityTypes/Create
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName");
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        // POST: Admin/PracticeSpecialityTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PracticeSpecialityType,IsActive,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupPracticeSpecialityType lookupPracticeSpecialityType)
        {
            if (ModelState.IsValid)
            {
                lookupPracticeSpecialityType.CreatedBy = CurrentUser.Id;
                lookupPracticeSpecialityType.CreatedOn = DateTime.UtcNow;
                db.LookupPracticeSpecialityTypes.Add(lookupPracticeSpecialityType);
                db.SaveChanges();
                lookupPracticeSpecialityType.ClearCache(CacheKeys.PracticeSpecialities);
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupPracticeSpecialityType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupPracticeSpecialityType.UpdatedBy);
            return View(lookupPracticeSpecialityType);
        }

        // GET: Admin/PracticeSpecialityTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupPracticeSpecialityType lookupPracticeSpecialityType = db.LookupPracticeSpecialityTypes.Find(id);
            if (lookupPracticeSpecialityType == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupPracticeSpecialityType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupPracticeSpecialityType.UpdatedBy);
            return View(lookupPracticeSpecialityType);
        }

        // POST: Admin/PracticeSpecialityTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PracticeSpecialityType,IsActive,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupPracticeSpecialityType lookupPracticeSpecialityType)
        {
            if (ModelState.IsValid)
            {
                lookupPracticeSpecialityType.UpdatedBy = CurrentUser.Id;
                lookupPracticeSpecialityType.UpdatedOn = DateTime.UtcNow;
                db.Entry(lookupPracticeSpecialityType).State = EntityState.Modified;
                db.SaveChanges();
                lookupPracticeSpecialityType.ClearCache(CacheKeys.PracticeSpecialities);
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupPracticeSpecialityType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupPracticeSpecialityType.UpdatedBy);
            return View(lookupPracticeSpecialityType);
        }

        // GET: Admin/PracticeSpecialityTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupPracticeSpecialityType lookupPracticeSpecialityType = db.LookupPracticeSpecialityTypes.Find(id);
            if (lookupPracticeSpecialityType == null)
            {
                return HttpNotFound();
            }
            
            return View(lookupPracticeSpecialityType);
        }

        // POST: Admin/PracticeSpecialityTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LookupPracticeSpecialityType lookupPracticeSpecialityType = db.LookupPracticeSpecialityTypes.Find(id);
            try
            {
                db.LookupPracticeSpecialityTypes.Remove(lookupPracticeSpecialityType);
                db.SaveChanges();
                lookupPracticeSpecialityType.ClearCache(CacheKeys.PracticeSpecialities);
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("Error", "There are some releted item in database, please delete those first");
                return View("Delete", lookupPracticeSpecialityType);
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
