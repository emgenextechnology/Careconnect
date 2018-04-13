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
    public class LeadSourcesController : BaseController
    {
        //private EmegenexBiz2016Entities db = new EmegenexBiz2016Entities();

        // GET: Admin/LeadSources
        public ActionResult Index()
        {
            var lookupLeadSources = db.LookupLeadSources.Include(l => l.User).Include(l => l.User1);
            return View(lookupLeadSources.OrderByDescending(a=>a.Id).ToList());
        }

        // GET: Admin/LeadSources/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupLeadSource lookupLeadSource = db.LookupLeadSources.Find(id);
            if (lookupLeadSource == null)
            {
                return HttpNotFound();
            }
            return View(lookupLeadSource);
        }

        // GET: Admin/LeadSources/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/LeadSources/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,LeadSource,IsActive,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupLeadSource lookupLeadSource)
        {
            if (ModelState.IsValid)
            {
                lookupLeadSource.CreatedBy = CurrentUser.Id;
                lookupLeadSource.CreatedOn = DateTime.UtcNow;
                db.LookupLeadSources.Add(lookupLeadSource);
                db.SaveChanges();

                lookupLeadSource.ClearCache(CacheKeys.LeadSources);
                return RedirectToAction("Index");
            }
            return View(lookupLeadSource);
        }

        // GET: Admin/LeadSources/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupLeadSource lookupLeadSource = db.LookupLeadSources.Find(id);
            if (lookupLeadSource == null)
            {
                return HttpNotFound();
            }
            return View(lookupLeadSource);
        }

        // POST: Admin/LeadSources/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,LeadSource,IsActive,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupLeadSource lookupLeadSource)
        {
            if (ModelState.IsValid)
            {
                lookupLeadSource.UpdatedBy = CurrentUser.Id;
                lookupLeadSource.UpdatedOn = DateTime.UtcNow;
                db.Entry(lookupLeadSource).State = EntityState.Modified;
                db.SaveChanges();
                lookupLeadSource.ClearCache(CacheKeys.LeadSources);
                return RedirectToAction("Index");
            }
            return View(lookupLeadSource);
        }

        // GET: Admin/LeadSources/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupLeadSource lookupLeadSource = db.LookupLeadSources.Find(id);
            if (lookupLeadSource == null)
            {
                return HttpNotFound();
            }
            return View(lookupLeadSource);
        }

        // POST: Admin/LeadSources/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
                LookupLeadSource lookupLeadSource = db.LookupLeadSources.Find(id);
            try
            {
                db.LookupLeadSources.Remove(lookupLeadSource);
                db.SaveChanges();
                lookupLeadSource.ClearCache(CacheKeys.LeadSources);
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("Error", "There are some releted item in database, please delete those first");
                return View("Delete", lookupLeadSource);
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
