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
using EmgenexBusinessPortal.Models;
using EmgenexBusinessPortal.Helpers;

namespace EmgenexBusinessPortal.Areas.Business.Controllers
{
    [BusinessAuthorize]
    public class DepartmentsController : BaseController
    {
        //private EmegenexBiz2016Entities db = new EmegenexBiz2016Entities();

        // GET: Business/Departments
        public ActionResult Index(string SearchKey, int page = 1, bool IsPartial = false)
        {
            var departments = db.Departments.Where(a => a.BusinessId == CurrentUser.BusinessId).Include(d => d.User).Include(d => d.User1);
            if (!string.IsNullOrEmpty(SearchKey))
            {
                departments = departments.Where(ua => ua.DepartmentName.ToLower().Contains(SearchKey.ToLower()));
                ViewBag.SearchKey = SearchKey;
            }
            var count=departments.Count();
            ViewBag.Count = count;
            ViewBag.page = page;
            var pager = new Pager(count, page, 10);
            ViewBag.StartPage = pager.StartPage;
            ViewBag.EndPage = pager.EndPage;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPage = pager.TotalPages;
            var query = departments.OrderByDescending(a => a.CreatedOn).Skip(((page - 1) * pager.PageSize)).Take(pager.PageSize).ToList();
            if (IsPartial)
            {
                return PartialView("_PartialDepartmentList", query);
            }
            return View(query);
        }

        // GET: Business/Departments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // GET: Business/Departments/Create
        public ActionResult Create()
        {
            ViewBag.DepartmentPrivileges = db.Privileges.GroupBy(a => a.ModuleId).Select(b => new Modulesmodel { ModuleName = b.FirstOrDefault(a => a.ModulesMaster.Id == a.ModuleId).ModulesMaster.Title, Privileges = b.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Title }) }).OrderByDescending(a => a.ModuleName);
            return View();
        }

        // POST: Business/Departments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,DepartmentName,Description,StatusId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy,IsActive")] Department department, params int[] selectedPrivileges)
        {
            if (ModelState.IsValid)
            {
                department.CreatedBy = CurrentUser.Id;
                department.CreatedOn = DateTime.UtcNow;
                department.StatusId = 1;
                department.BusinessId = CurrentBusinessId;
                department.IsActive = true;
                var DepartmentModel = db.Departments.Add(department);
                if (db.SaveChanges() > 0)
                {
                    if (selectedPrivileges != null && selectedPrivileges.Count() > 0)
                    {
                        foreach (var item in selectedPrivileges)
                        {
                            db.DepartmentPrivileges.Add(new DepartmentPrivilege { DepartmentId = DepartmentModel.Id, PrivilegeId = item, CreatedBy = department.CreatedBy, CreatedOn = department.CreatedOn });
                            db.SaveChanges();
                        }
                    }
                }
                ViewBag.DepartmentPrivileges = db.Privileges.GroupBy(a => a.ModuleId).Select(b => new Modulesmodel { ModuleName = b.FirstOrDefault(a => a.ModulesMaster.Id == a.ModuleId).ModulesMaster.Title, Privileges = b.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Title }) }).OrderByDescending(a => a.ModuleName);
                return RedirectToAction("Index");
            }

            return View(department);
        }

        // GET: Business/Departments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            var DepartmentPrivilegeList = department.DepartmentPrivileges.Select(a => a.PrivilegeId);
            ViewBag.DepartmentPrivileges = db.Privileges.GroupBy(a => a.ModuleId).Select(b => new Modulesmodel { ModuleName = b.FirstOrDefault(a => a.ModulesMaster.Id == a.ModuleId).ModulesMaster.Title, Privileges = b.Select(c => new SelectListItem { Value = c.Id.ToString(), Selected = DepartmentPrivilegeList.Contains(c.Id), Text = c.Title }) }).OrderByDescending(a => a.ModuleName);
            return View(department);
        }

        // POST: Business/Departments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,DepartmentName,Description,StatusId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy,IsActive,BusinessId")] Department department, params int[] selectedPrivileges)
        {
            if (ModelState.IsValid)
            {
                department.UpdatedBy = CurrentUser.Id;
                department.UpdatedOn = DateTime.UtcNow;
                db.Entry(department).State = EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    IEnumerable<DepartmentPrivilege> departmentPrivilege = db.DepartmentPrivileges.Where(a => a.DepartmentId == department.Id).ToList();
                    if (departmentPrivilege.Count() > 0)
                    {
                        db.DepartmentPrivileges.RemoveRange(departmentPrivilege);
                        db.SaveChanges();
                    }
                    if (selectedPrivileges != null && selectedPrivileges.Count() > 0)
                    {
                        foreach (var item in selectedPrivileges)
                        {
                            db.DepartmentPrivileges.Add(new DepartmentPrivilege { DepartmentId = department.Id, PrivilegeId = item, CreatedBy = department.CreatedBy, CreatedOn = department.CreatedOn });
                            db.SaveChanges();
                        }
                    }
                }
                return RedirectToAction("Index");
            }
            var DepartmentPrivilegeList = department.DepartmentPrivileges.Select(a => a.PrivilegeId);
            ViewBag.DepartmentPrivileges = db.Privileges.GroupBy(a => a.ModuleId).Select(b => new Modulesmodel { ModuleName = b.FirstOrDefault(a => a.ModulesMaster.Id == a.ModuleId).ModulesMaster.Title, Privileges = b.Select(c => new SelectListItem { Value = c.Id.ToString(), Selected = DepartmentPrivilegeList.Contains(c.Id), Text = c.Title }) }).OrderByDescending(a => a.ModuleName);
            return View(department);
        }

        // GET: Business/Departments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: Business/Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Department department = db.Departments.Find(id);
            try
            {
                db.Departments.Remove(department);
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("Error", "There are some releted item in database, please delete those first");
                return View("Delete", department);
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
