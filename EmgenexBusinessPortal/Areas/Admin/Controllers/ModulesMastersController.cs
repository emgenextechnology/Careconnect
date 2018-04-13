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
    public class ModulesMastersController : BaseController
    {
        //private EmegenexBiz2016Entities db = new EmegenexBiz2016Entities();

        // GET: Admin/ModulesMasters
        public ActionResult Index()
        {
            return View(db.ModulesMasters.ToList());
        }

        // GET: Admin/ModulesMasters/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModulesMaster modulesMaster = db.ModulesMasters.Find(id);
            if (modulesMaster == null)
            {
                return HttpNotFound();
            }
            return View(modulesMaster);
        }

        // GET: Admin/ModulesMasters/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/ModulesMasters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description,CreatedBy,CreatedOn,UpdatedBy,UpdatedOn,IsActive")] ModulesMaster modulesMaster)
        {
            if (ModelState.IsValid)
            {
                modulesMaster.CreatedBy = CurrentUser.Id;
                modulesMaster.CreatedOn = System.DateTime.UtcNow;
                modulesMaster.IsActive = true;
                db.ModulesMasters.Add(modulesMaster);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(modulesMaster);
        }

        // GET: Admin/ModulesMasters/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModulesMaster modulesMaster = db.ModulesMasters.Find(id);
            if (modulesMaster == null)
            {
                return HttpNotFound();
            }
            return View(modulesMaster);
        }

        // POST: Admin/ModulesMasters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,CreatedBy,CreatedOn,UpdatedBy,UpdatedOn,IsActive")] ModulesMaster modulesMaster)
        {
            if (ModelState.IsValid)
            {
                modulesMaster.UpdatedBy = CurrentUser.Id;
                modulesMaster.UpdatedOn = System.DateTime.UtcNow;
                db.Entry(modulesMaster).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(modulesMaster);
        }

        // GET: Admin/ModulesMasters/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModulesMaster modulesMaster = db.ModulesMasters.Find(id);
            if (modulesMaster == null)
            {
                return HttpNotFound();
            }
            return View(modulesMaster);
        }

        // POST: Admin/ModulesMasters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ModulesMaster modulesMaster = db.ModulesMasters.Find(id);
            try
            {
                db.ModulesMasters.Remove(modulesMaster);
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("Error", "There are some releted item in database, please delete those first");
                return View("Delete", modulesMaster);
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
