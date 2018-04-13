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

namespace EmgenexBusinessPortal.Areas.Business.Controllers
{
    [BusinessAuthorize]
    public class MarketingCategoriesController : BaseController
    {
        //private EmegenexBiz2016Entities db = new EmegenexBiz2016Entities();

        // GET: Admin/MarketingCategories
        public ActionResult Index()
        {
            var lookupMarketingCategories = db.LookupMarketingCategories.Where(a => a.BusinessId == CurrentBusinessId).Include(l => l.User).Include(l => l.User1);
            return View(lookupMarketingCategories.OrderByDescending(a=>a.CreatedOn).ToList());
        }

        // GET: Admin/MarketingCategories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupMarketingCategory lookupMarketingCategory = db.LookupMarketingCategories.Find(id);
            if (lookupMarketingCategory == null)
            {
                return HttpNotFound();
            }
            return View(lookupMarketingCategory);
        }

        // GET: Admin/MarketingCategories/Create
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName");
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        // POST: Admin/MarketingCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Category,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupMarketingCategory lookupMarketingCategory)
        {
            if (ModelState.IsValid)
            {
                lookupMarketingCategory.BusinessId = CurrentBusinessId;
                lookupMarketingCategory.CreatedBy = CurrentUser.Id;
                lookupMarketingCategory.CreatedOn = DateTime.UtcNow;
                db.LookupMarketingCategories.Add(lookupMarketingCategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupMarketingCategory.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupMarketingCategory.UpdatedBy);
            return View(lookupMarketingCategory);
        }

        // GET: Admin/MarketingCategories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupMarketingCategory lookupMarketingCategory = db.LookupMarketingCategories.Find(id);
            if (lookupMarketingCategory == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupMarketingCategory.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupMarketingCategory.UpdatedBy);
            return View(lookupMarketingCategory);
        }

        // POST: Admin/MarketingCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Category,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy,BusinessId")] LookupMarketingCategory lookupMarketingCategory)
        {
            if (ModelState.IsValid)
            {
                lookupMarketingCategory.UpdatedBy = CurrentUser.Id;
                lookupMarketingCategory.UpdatedOn = DateTime.UtcNow;
                db.Entry(lookupMarketingCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupMarketingCategory.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupMarketingCategory.UpdatedBy);
            return View(lookupMarketingCategory);
        }

        // GET: Admin/MarketingCategories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupMarketingCategory lookupMarketingCategory = db.LookupMarketingCategories.Find(id);
            if (lookupMarketingCategory == null)
            {
                return HttpNotFound();
            }
            return View(lookupMarketingCategory);
        }

        // POST: Admin/MarketingCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LookupMarketingCategory lookupMarketingCategory = db.LookupMarketingCategories.Find(id);
            try
            {
                db.LookupMarketingCategories.Remove(lookupMarketingCategory);
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("Error", "There are some releted item in database, please delete those first");
                return View("Delete", lookupMarketingCategory);
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
