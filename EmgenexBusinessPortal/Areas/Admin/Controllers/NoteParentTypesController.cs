using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EmgenexBusinessPortal.Areas.Admin.Database;

namespace EmgenexBusinessPortal.Areas.Admin.Controllers
{
    public class NoteParentTypesController : Controller
    {
        private EmgenBiz2016Entities db = new EmgenBiz2016Entities();

        // GET: Admin/NoteParentTypes
        public ActionResult Index()
        {
            var lookupNoteParentTypes = db.LookupNoteParentTypes.Include(l => l.User).Include(l => l.User1);
            return View(lookupNoteParentTypes.ToList());
        }

        // GET: Admin/NoteParentTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupNoteParentType lookupNoteParentType = db.LookupNoteParentTypes.Find(id);
            if (lookupNoteParentType == null)
            {
                return HttpNotFound();
            }
            return View(lookupNoteParentType);
        }

        // GET: Admin/NoteParentTypes/Create
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName");
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        // POST: Admin/NoteParentTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,NoteParentType,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupEnrolledService lookupNoteParentType)
        {
            if (ModelState.IsValid)
            {
                lookupNoteParentType.CreatedBy = 1;
                lookupNoteParentType.CreatedOn = DateTime.Now;
                db.LookupNoteParentTypes.Add(lookupNoteParentType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupNoteParentType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupNoteParentType.UpdatedBy);
            return View(lookupNoteParentType);
        }

        // GET: Admin/NoteParentTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupNoteParentType lookupNoteParentType = db.LookupNoteParentTypes.Find(id);
            if (lookupNoteParentType == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupNoteParentType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupNoteParentType.UpdatedBy);
            return View(lookupNoteParentType);
        }

        // POST: Admin/NoteParentTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,NoteParentType,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupEnrolledService lookupNoteParentType)
        {
            if (ModelState.IsValid)
            {
                lookupNoteParentType.UpdatedBy = 1;
                lookupNoteParentType.UpdatedOn = DateTime.Now;
                db.Entry(lookupNoteParentType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupNoteParentType.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupNoteParentType.UpdatedBy);
            return View(lookupNoteParentType);
        }

        // GET: Admin/NoteParentTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupNoteParentType lookupNoteParentType = db.LookupNoteParentTypes.Find(id);
            if (lookupNoteParentType == null)
            {
                return HttpNotFound();
            }
            return View(lookupNoteParentType);
        }

        // POST: Admin/NoteParentTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LookupNoteParentType lookupNoteParentType = db.LookupNoteParentTypes.Find(id);
            db.LookupNoteParentTypes.Remove(lookupNoteParentType);
            db.SaveChanges();
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
