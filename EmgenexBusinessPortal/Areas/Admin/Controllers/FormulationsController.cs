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
    public class FormulationsController : BaseController
    {
        private EmegenexBiz2016Entities db = new EmegenexBiz2016Entities();

        // GET: Admin/Formulations
        public ActionResult Index()
        {
            var lookupFormulations = db.LookupFormulations.Include(l => l.User).Include(l => l.User1);
            return View(lookupFormulations.ToList());
        }

        // GET: Admin/Formulations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupFormulation lookupFormulation = db.LookupFormulations.Find(id);
            if (lookupFormulation == null)
            {
                return HttpNotFound();
            }
            return View(lookupFormulation);
        }

        // GET: Admin/Formulations/Create
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName");
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        // POST: Admin/Formulations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Description,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupFormulation lookupFormulation)
        {
            if (ModelState.IsValid)
            {
                lookupFormulation.CreatedBy = CurrentUser.Id;
                lookupFormulation.CreatedOn = DateTime.UtcNow;
                db.LookupFormulations.Add(lookupFormulation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupFormulation.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupFormulation.UpdatedBy);
            return View(lookupFormulation);
        }

        // GET: Admin/Formulations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupFormulation lookupFormulation = db.LookupFormulations.Find(id);
            if (lookupFormulation == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupFormulation.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupFormulation.UpdatedBy);
            return View(lookupFormulation);
        }

        // POST: Admin/Formulations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Description,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupFormulation lookupFormulation)
        {
            if (ModelState.IsValid)
            {
                lookupFormulation.UpdatedBy = CurrentUser.Id;
                lookupFormulation.UpdatedOn = DateTime.UtcNow;
                db.Entry(lookupFormulation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupFormulation.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupFormulation.UpdatedBy);
            return View(lookupFormulation);
        }

        // GET: Admin/Formulations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupFormulation lookupFormulation = db.LookupFormulations.Find(id);
            if (lookupFormulation == null)
            {
                return HttpNotFound();
            }
            return View(lookupFormulation);
        }

        // POST: Admin/Formulations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LookupFormulation lookupFormulation = db.LookupFormulations.Find(id);
            try
            {
                db.LookupFormulations.Remove(lookupFormulation);
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("Error", "There are some releted item in database, please delete those first");
                return View("Delete", lookupFormulation);
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
