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
    public class DocumentTypesController : BaseController
    {
        private EmegenexBiz2016Entities db = new EmegenexBiz2016Entities();

        // GET: Admin/DocumentTypes
        public ActionResult Index()
        {
            var lookupDocumentTypes = db.LookupDocumentTypes.Include(l => l.User).Include(l => l.User1);
            return View(lookupDocumentTypes.ToList());
        }

        // GET: Admin/DocumentTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupDocumentType lookupDocumentType = db.LookupDocumentTypes.Find(id);
            if (lookupDocumentType == null)
            {
                return HttpNotFound();
            }
            return View(lookupDocumentType);
        }

        // GET: Admin/DocumentTypes/Create
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName");
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        // POST: Admin/DocumentTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,DocumentType,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupDocumentType lookupDocumentType)
        {
            if (ModelState.IsValid)
            {
                lookupDocumentType.CreatedBy = CurrentUser.Id;
                lookupDocumentType.CreatedOn = DateTime.UtcNow;
                db.LookupDocumentTypes.Add(lookupDocumentType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupDocumentType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupDocumentType.UpdatedBy);
            return View(lookupDocumentType);
        }

        // GET: Admin/DocumentTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupDocumentType lookupDocumentType = db.LookupDocumentTypes.Find(id);
            if (lookupDocumentType == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupDocumentType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupDocumentType.UpdatedBy);
            return View(lookupDocumentType);
        }

        // POST: Admin/DocumentTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,DocumentType,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupDocumentType lookupDocumentType)
        {
            if (ModelState.IsValid)
            {
                lookupDocumentType.UpdatedBy = CurrentUser.Id;
                lookupDocumentType.UpdatedOn = DateTime.UtcNow;
                db.Entry(lookupDocumentType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupDocumentType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupDocumentType.UpdatedBy);
            return View(lookupDocumentType);
        }

        // GET: Admin/DocumentTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupDocumentType lookupDocumentType = db.LookupDocumentTypes.Find(id);
            if (lookupDocumentType == null)
            {
                return HttpNotFound();
            }
            return View(lookupDocumentType);
        }

        // POST: Admin/DocumentTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LookupDocumentType lookupDocumentType = db.LookupDocumentTypes.Find(id);
            try
            {
                db.LookupDocumentTypes.Remove(lookupDocumentType);
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("Error", "There are some releted item in database, please delete those first");
                return View("Delete", lookupDocumentType);
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
