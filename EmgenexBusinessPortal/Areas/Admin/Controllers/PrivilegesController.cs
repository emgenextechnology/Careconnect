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
using EmgenexBusinessPortal.Helpers;

namespace EmgenexBusinessPortal.Areas.Admin.Controllers
{
    [Authorize]
    public class PrivilegesController : BaseController
    {
        //private EmegenexBiz2016Entities db = new EmegenexBiz2016Entities();

        // GET: Admin/Privileges
        public ActionResult Index(string SearchKey, int page = 1, bool IsPartial = false)
        {
            var privileges = db.Privileges.Include(p => p.ModulesMaster).Include(p => p.User).Include(p => p.User1);
            ViewBag.page = page;
            if (!string.IsNullOrEmpty(SearchKey))
            {
                privileges = privileges.Where(ua => ua.Title.ToLower().Contains(SearchKey.ToLower()) || ua.PrivilegeKey.ToLower().Contains(SearchKey.ToLower()));
                ViewBag.SearchKey = SearchKey;
            }
            var pager = new Pager(privileges.Count(), page, 10);
            ViewBag.StartPage = pager.StartPage;
            ViewBag.EndPage = pager.EndPage;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPage = pager.TotalPages;
            var query = privileges.OrderByDescending(a => a.CreatedOn).Skip(((page - 1) * pager.PageSize)).Take(pager.PageSize).ToList();
            if (IsPartial)
            {
                return PartialView("_PartialPrivilegeList", query);
            }
            return View(query);
        }

        // GET: Admin/Privileges/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Privilege privilege = db.Privileges.Find(id);
            if (privilege == null)
            {
                return HttpNotFound();
            }
            return View(privilege);
        }

        // GET: Admin/Privileges/Create
        public ActionResult Create()
        {
            ViewBag.ModuleId = new SelectList(db.ModulesMasters, "Id", "Title");
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "FirstName");
            return View();
        }

        // POST: Admin/Privileges/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,ModuleId,Description,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy,PrivilegeKey")] Privilege privilege)
        {
            if (db.Privileges.FirstOrDefault(a => a.PrivilegeKey.ToLower() == privilege.PrivilegeKey.ToLower()) != null)
            {
                ViewBag.ModuleId = new SelectList(db.ModulesMasters, "Id", "Title");
                ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName");
                ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "FirstName");
                ModelState.AddModelError("Error", "Already Used");
                return View(privilege);
            }
            if (ModelState.IsValid)
            {
                privilege.CreatedBy = CurrentUser.Id;
                privilege.CreatedOn = DateTime.UtcNow;
                db.Privileges.Add(privilege);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ModuleId = new SelectList(db.ModulesMasters, "Id", "Title");
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName", privilege.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "FirstName", privilege.UpdatedBy);
            return View(privilege);
        }

        // GET: Admin/Privileges/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Privilege privilege = db.Privileges.Find(id);
            if (privilege == null)
            {
                return HttpNotFound();
            }
            ViewBag.ModuleId = new SelectList(db.ModulesMasters, "Id", "Title", privilege.ModuleId);
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName", privilege.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "FirstName", privilege.UpdatedBy);
            return View(privilege);
        }

        // POST: Admin/Privileges/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,ModuleId,Description,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy,PrivilegeKey")] Privilege privilege)
        {
            if (db.Privileges.FirstOrDefault(a => a.Id != privilege.Id && a.PrivilegeKey.ToLower() == privilege.PrivilegeKey.ToLower()) != null)
            {
                ViewBag.ModuleId = new SelectList(db.ModulesMasters, "Id", "Title", privilege.ModuleId);
                ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName", privilege.CreatedBy);
                ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "FirstName", privilege.UpdatedBy);
                ModelState.AddModelError("Error", "Already Used");
                return View(privilege);
            }
            if (ModelState.IsValid)
            {
                privilege.UpdatedBy = CurrentUser.Id;
                privilege.UpdatedOn = DateTime.UtcNow;
                db.Entry(privilege).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ModuleId = new SelectList(db.ModulesMasters, "Id", "Title", privilege.ModuleId);
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName", privilege.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "FirstName", privilege.UpdatedBy);
            return View(privilege);
        }

        // GET: Admin/Privileges/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Privilege privilege = db.Privileges.Find(id);
            if (privilege == null)
            {
                return HttpNotFound();
            }
            return View(privilege);
        }

        // POST: Admin/Privileges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
                Privilege privilege = db.Privileges.Find(id);
            try
            {
                db.Privileges.Remove(privilege);
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("Error", "There are some releted item in database, please delete those first");
                return View("Delete", privilege);
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
