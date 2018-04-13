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
    public class RolesController : BaseController
    {
        //private EmegenexBiz2016Entities db = new EmegenexBiz2016Entities();

        // GET: Business/Roles
        public ActionResult Index(string SearchKey, int page = 1, bool IsPartial = false)
        {
            int? CurrentBusinessId = CurrentUser.BusinessId == null ? 0 : CurrentUser.BusinessId;
            var model = db.Roles.Where(a => a.BusinessId == CurrentBusinessId);

            if (!string.IsNullOrEmpty(SearchKey))
            {
                model = model.Where(ua => ua.Name.ToLower().Contains(SearchKey.ToLower()));
                ViewBag.SearchKey = SearchKey;
            }
            var count = model.Count();
            ViewBag.Count = count;
            ViewBag.page = page;
            var pager = new Pager(count, page, 10);
            ViewBag.StartPage = pager.StartPage;
            ViewBag.EndPage = pager.EndPage;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPage = pager.TotalPages;
            var query = model.OrderByDescending(a => a.CreatedOn).Skip(((page - 1) * pager.PageSize)).Take(pager.PageSize).ToList();
            if (IsPartial)
            {
                return PartialView("_PartialRolesList", query);
            }
            return View(query);
        }

        // GET: Business/Roles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Role role = db.Roles.Find(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        // GET: Business/Roles/Create
        public ActionResult Create()
        {
            ViewBag.RolePrivileges = db.Privileges.GroupBy(a => a.ModuleId).Select(b => new Modulesmodel
            {
                ModuleName = b.FirstOrDefault(a => a.ModulesMaster.Id == a.ModuleId).ModulesMaster.Title,
                Privileges = b.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Title })
            }).OrderByDescending(a => a.ModuleName);
            return View();
        }

        // POST: Business/Roles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Description,Name")] Role role, params int[] selectedPrivileges)
        {
            if (ModelState.IsValid)
            {
                role.BusinessId = CurrentBusinessId;
                role.IsActive = true;
                role.CreatedBy = CurrentUser.Id;
                role.CreatedOn = System.DateTime.UtcNow;
                var RolesModel = db.Roles.Add(role);
                if (db.SaveChanges() > 0)
                {
                    if (selectedPrivileges != null && selectedPrivileges.Count() > 0)
                    {
                        foreach (var item in selectedPrivileges)
                        {
                            db.RolePrivileges.Add(new RolePrivilege { RoleId = RolesModel.Id, PrivilegeId = item, CreatedBy = CurrentUser.Id, CreatedOn = DateTime.UtcNow });
                            db.SaveChanges();
                        }
                    }
                }
                ViewBag.RolePrivileges = db.Privileges.GroupBy(a => a.ModuleId).Select(b => new Modulesmodel { ModuleName = b.FirstOrDefault(a => a.ModulesMaster.Id == a.ModuleId).ModulesMaster.Title, Privileges = b.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Title }) }).OrderByDescending(a => a.ModuleName);
                return RedirectToAction("Index");
            }

            return View(role);
        }

        // GET: Business/Roles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Role role = db.Roles.Find(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            var RolePrivilegeList = role.RolePrivileges.Select(a => a.PrivilegeId);
            ViewBag.RolePrivileges = db.Privileges.GroupBy(a => a.ModuleId).Select(b => new Modulesmodel
            {
                ModuleName = b.FirstOrDefault(a => a.ModulesMaster.Id == a.ModuleId).ModulesMaster.Title,
                Privileges = b.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Selected = RolePrivilegeList.Contains(c.Id),
                    Text = c.Title
                })
            }).OrderByDescending(a => a.ModuleName);
            return View(role);
        }

        // POST: Business/Roles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Description,Name,IsActive,BusinessId,CreatedBy,CreatedOn")] Role role, params int[] selectedPrivileges)
        {
            if (ModelState.IsValid)
            {
                db.Entry(role).State = EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    IEnumerable<RolePrivilege> RolePrivilege = db.RolePrivileges.Where(a => a.RoleId == role.Id).ToList();
                    if (RolePrivilege.Count() > 0)
                    {
                        db.RolePrivileges.RemoveRange(RolePrivilege);
                        db.SaveChanges();
                    }
                    if (selectedPrivileges != null && selectedPrivileges.Count() > 0)
                    {
                        foreach (var item in selectedPrivileges)
                        {
                            db.RolePrivileges.Add(new RolePrivilege { RoleId = role.Id, PrivilegeId = item, CreatedBy = CurrentUser.Id, CreatedOn = DateTime.UtcNow });
                            db.SaveChanges();
                        }
                    }
                }
                return RedirectToAction("Index");
            }
            var RolePrivilegeList = role.RolePrivileges.Select(a => a.PrivilegeId);
            ViewBag.RolePrivileges = db.Privileges.GroupBy(a => a.ModuleId).Select(b => new Modulesmodel
            {
                ModuleName = b.FirstOrDefault(a => a.ModulesMaster.Id == a.ModuleId).ModulesMaster.Title,
                Privileges = b.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Selected = RolePrivilegeList.Contains(c.Id),
                    Text = c.Title
                })
            }).OrderByDescending(a => a.ModuleName);
            return View(role);
        }

        // GET: Business/Roles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Role role = db.Roles.Find(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        // POST: Business/Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Role role = db.Roles.Find(id);
            try
            {
                db.Roles.Remove(role);
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("Error", "There are some releted item in database, please delete those first");
                return View("Delete", role);
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
