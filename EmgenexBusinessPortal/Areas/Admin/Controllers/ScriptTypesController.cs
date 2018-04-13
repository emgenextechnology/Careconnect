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
    public class ScriptTypesController : BaseController
    {
        private EmegenexBiz2016Entities db = new EmegenexBiz2016Entities();

        // GET: Admin/ScriptTypes
        public ActionResult Index()
        {
            var lookupScriptTypes = db.LookupScriptTypes.Include(l => l.User).Include(l => l.User1);
            return View(lookupScriptTypes.ToList());
        }

        // GET: Admin/ScriptTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupScriptType lookupScriptType = db.LookupScriptTypes.Find(id);
            if (lookupScriptType == null)
            {
                return HttpNotFound();
            }
            return View(lookupScriptType);
        }

        // GET: Admin/ScriptTypes/Create
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName");
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        // POST: Admin/ScriptTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ScriptType,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupScriptType lookupScriptType)
        {
            if (ModelState.IsValid)
            {
                lookupScriptType.CreatedBy = CurrentUser.Id;
                lookupScriptType.CreatedOn = DateTime.UtcNow;
                db.LookupScriptTypes.Add(lookupScriptType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupScriptType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupScriptType.UpdatedBy);
            return View(lookupScriptType);
        }

        // GET: Admin/ScriptTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupScriptType lookupScriptType = db.LookupScriptTypes.Find(id);
            if (lookupScriptType == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupScriptType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupScriptType.UpdatedBy);
            return View(lookupScriptType);
        }

        // POST: Admin/ScriptTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ScriptType,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupScriptType lookupScriptType)
        {
            if (ModelState.IsValid)
            {
                lookupScriptType.UpdatedBy = CurrentUser.Id;
                lookupScriptType.UpdatedOn = DateTime.UtcNow;
                db.Entry(lookupScriptType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupScriptType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupScriptType.UpdatedBy);
            return View(lookupScriptType);
        }

        // GET: Admin/ScriptTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupScriptType lookupScriptType = db.LookupScriptTypes.Find(id);
            if (lookupScriptType == null)
            {
                return HttpNotFound();
            }
            return View(lookupScriptType);
        }

        // POST: Admin/ScriptTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LookupScriptType lookupScriptType = db.LookupScriptTypes.Find(id);
            try
            {
                db.LookupScriptTypes.Remove(lookupScriptType);
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("Error", "There are some releted item in database, please delete those first");
                return View("Delete", lookupScriptType);
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
