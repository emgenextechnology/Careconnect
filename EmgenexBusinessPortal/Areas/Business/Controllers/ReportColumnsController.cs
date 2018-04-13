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
using EmgenexBusinessPortal.Areas.Business.Models;

namespace EmgenexBusinessPortal.Areas.Business.Controllers
{
    [BusinessAuthorize]
    public class ReportColumnsController : BaseController
    {
        //private EmegenexBiz2016Entities db = new EmegenexBiz2016Entities();

        // GET: Business/ReportColumns
        public ActionResult Index(int ServiceId = 0)
        {
            ViewBag.Service = ServiceId;
            var reportColumns = db.ReportColumns.Where(a => a.BusinessId == CurrentBusinessId && a.LookupEnrolledService.BusinessId == CurrentBusinessId).Include(r => r.BusinessMaster).Include(r => r.User).Include(r => r.User1);
            if (ServiceId != 0)
            {
                reportColumns = reportColumns.Where(a => a.LookupEnrolledService.Id == ServiceId);
            }
            ViewBag.ServiceId = new SelectList(db.LookupEnrolledServices.Where(a => a.BusinessId == CurrentBusinessId), "Id", "ServiceName");
            return View(reportColumns.ToList());
        }

        // GET: Business/ReportColumns/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportColumn reportColumn = db.ReportColumns.Find(id);
            if (reportColumn == null)
            {
                return HttpNotFound();
            }
            return View(reportColumn);
        }
        // GET: Business/ReportColumns/Create
        public ActionResult Create()
        {
            ViewBag.RolePrivilegesList = new SelectList(db.Roles.Where(a => a.BusinessId == CurrentBusinessId), "Id", "Name");
            ViewBag.DepartmentPrivilegesList = new SelectList(db.Departments.Where(a => a.BusinessId == CurrentBusinessId), "Id", "DepartmentName");
            ViewBag.UserPrivilegesList = new SelectList(db.Users.Where(a => a.BusinessId == CurrentUser.BusinessId && a.IsActive == true && !a.Roles.Select(y => y.Name).Contains("BusinessAdmin") && !a.Roles.Select(y => y.Name).Contains("SuperAdmin") && !a.Roles.Select(y => y.Name).Contains("MasterAdmin")), "Id", "FirstName");
            ViewBag.ServiceId = new SelectList(db.LookupEnrolledServices.Where(a => a.BusinessId == CurrentBusinessId), "Id", "ServiceName");
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "FirstName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ReportColumnModel reportColumn)
        {
            if (db.ReportColumns.FirstOrDefault(a => a.BusinessId == CurrentBusinessId && a.ServiceId == reportColumn.ServiceId && a.ColumnName.ToLower().Replace(" ", "") == reportColumn.ColumnName.ToLower().Replace(" ", "")) != null)
            {
                ViewBag.RolePrivilegesList = new SelectList(db.Roles.Where(a => a.BusinessId == CurrentBusinessId), "Id", "Name");
                ViewBag.DepartmentPrivilegesList = new SelectList(db.Departments.Where(a => a.BusinessId == CurrentBusinessId), "Id", "DepartmentName");
                ViewBag.UserPrivilegesList = new SelectList(db.Users.Where(a => a.BusinessId == CurrentUser.BusinessId && a.IsActive == true && !a.Roles.Select(y => y.Name).Contains("BusinessAdmin") && !a.Roles.Select(y => y.Name).Contains("SuperAdmin") && !a.Roles.Select(y => y.Name).Contains("MasterAdmin")), "Id", "FirstName");
                ViewBag.ServiceId = new SelectList(db.LookupEnrolledServices.Where(a => a.BusinessId == CurrentBusinessId), "Id", "ServiceName");
                ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName");
                ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "FirstName");
                ModelState.AddModelError("Error", "Allready Used");
                return View(reportColumn);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var model = db.ReportColumns.Add(new ReportColumn
                    {
                        ColumnName = reportColumn.ColumnName.Replace(" ", ""),
                        BusinessId = (int)CurrentBusinessId,
                        CreatedBy = CurrentUserId,
                        CreatedOn = System.DateTime.UtcNow,
                        ServiceId = reportColumn.ServiceId,
                        DisplayInFilter = reportColumn.DisplayInFilter,
                        IsMandatory = reportColumn.IsMandatory,
                        ShowInGrid = reportColumn.ShowInGrid
                    });
                    if (db.SaveChanges() > 0)
                    {
                        if (reportColumn.RolePrivileges != null)
                        {
                            foreach (var item in reportColumn.RolePrivileges)
                            {
                                db.SalesRolePrivileges.Add(new SalesRolePrivilege { ColumnId = model.Id, RoleId = item, CreatedBy = CurrentUser.Id, CreatedOn = System.DateTime.UtcNow });
                                db.SaveChanges();
                            }
                        }
                        if (reportColumn.DepartmentPrivileges != null)
                        {
                            foreach (var item in reportColumn.DepartmentPrivileges)
                            {
                                db.SalesDepartmentPrivileges.Add(new SalesDepartmentPrivilege { ColumnId = model.Id, DepartmentId = item, CreatedBy = CurrentUser.Id, CreatedOn = System.DateTime.UtcNow });
                                db.SaveChanges();
                            }
                        }
                        if (reportColumn.UserPrivileges != null)
                        {
                            foreach (var item in reportColumn.UserPrivileges)
                            {
                                db.SalesUserPrivileges.Add(new SalesUserPrivilege { ColumnId = model.Id, UserId = item, CreatedBy = CurrentUser.Id, CreatedOn = System.DateTime.UtcNow });
                                db.SaveChanges();
                            }
                        }
                    }
                    return RedirectToAction("Index");
                }
                ViewBag.RolePrivilegesList = new SelectList(db.Roles.Where(a => a.BusinessId == CurrentBusinessId), "Id", "Name");
                ViewBag.DepartmentPrivilegesList = new SelectList(db.Departments.Where(a => a.BusinessId == CurrentBusinessId), "Id", "DepartmentName");
                ViewBag.UserPrivilegesList = new SelectList(db.Users.Where(a => a.BusinessId == CurrentUser.BusinessId && a.IsActive == true && !a.Roles.Select(y => y.Name).Contains("BusinessAdmin") && !a.Roles.Select(y => y.Name).Contains("SuperAdmin") && !a.Roles.Select(y => y.Name).Contains("MasterAdmin")), "Id", "FirstName");
                ViewBag.ServiceId = new SelectList(db.LookupEnrolledServices.Where(a => a.BusinessId == CurrentBusinessId), "Id", "ServiceName");
                ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName");
                ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "FirstName");
                return View(reportColumn);
            }
        }
        // GET: Business/ReportColumns/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportColumn reportColumn = db.ReportColumns.Find(id);
            if (reportColumn == null)
            {
                return HttpNotFound();
            }
            var model = new ReportColumnModel
            {
                Id = reportColumn.Id,
                ServiceId = reportColumn.ServiceId,
                ColumnName = reportColumn.ColumnName,
                IsMandatory = reportColumn.IsMandatory,
                DisplayInFilter = reportColumn.DisplayInFilter,
                RolePrivileges = db.SalesRolePrivileges.Where(t => t.ColumnId == id).Select(t => t.Role.Id).ToArray(),
                DepartmentPrivileges = db.SalesDepartmentPrivileges.Where(t => t.ColumnId == id).Select(t => t.Department.Id).ToArray(),
                UserPrivileges = db.SalesUserPrivileges.Where(t => t.ColumnId == id).Select(t => t.User.Id).ToArray(),
                ShowInGrid = reportColumn.ShowInGrid
            };
            ViewBag.RolePrivilegesList = new SelectList(db.Roles.Where(a => a.BusinessId == CurrentBusinessId), "Id", "Name");
            ViewBag.DepartmentPrivilegesList = new SelectList(db.Departments.Where(a => a.BusinessId == CurrentBusinessId), "Id", "DepartmentName");
            ViewBag.UserPrivilegesList = new SelectList(db.Users.Where(a => a.BusinessId == CurrentUser.BusinessId && a.IsActive == true && !a.Roles.Select(y => y.Name).Contains("BusinessAdmin") && !a.Roles.Select(y => y.Name).Contains("SuperAdmin") && !a.Roles.Select(y => y.Name).Contains("MasterAdmin")), "Id", "FirstName");
            ViewBag.ServiceId = new SelectList(db.LookupEnrolledServices.Where(a => a.BusinessId == CurrentBusinessId), "Id", "ServiceName", reportColumn.ServiceId);
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName", reportColumn.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "FirstName", reportColumn.UpdatedBy);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ReportColumnModel reportColumn)
        {
            if (db.ReportColumns.FirstOrDefault(a => a.Id != reportColumn.Id && a.BusinessId == CurrentBusinessId && a.ServiceId == reportColumn.ServiceId && a.ColumnName.ToLower().Replace(" ", "") == reportColumn.ColumnName.ToLower().Replace(" ", "")) != null)
            {
                ViewBag.RolePrivilegesList = new SelectList(db.Roles.Where(a => a.BusinessId == CurrentBusinessId), "Id", "Name");
                ViewBag.DepartmentPrivilegesList = new SelectList(db.Departments.Where(a => a.BusinessId == CurrentBusinessId), "Id", "DepartmentName");
                ViewBag.UserPrivilegesList = new SelectList(db.Users.Where(a => a.BusinessId == CurrentUser.BusinessId && a.IsActive == true && !a.Roles.Select(y => y.Name).Contains("BusinessAdmin") && !a.Roles.Select(y => y.Name).Contains("SuperAdmin") && !a.Roles.Select(y => y.Name).Contains("MasterAdmin")), "Id", "FirstName");
                ViewBag.ServiceId = new SelectList(db.LookupEnrolledServices.Where(a => a.BusinessId == CurrentBusinessId), "Id", "ServiceName", reportColumn.ServiceId);
                ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName", reportColumn.CreatedBy);
                ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "FirstName", reportColumn.UpdatedBy);
                ModelState.AddModelError("Error", "Allready Used");
                return View(reportColumn);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var model = db.ReportColumns.Find(reportColumn.Id);
                    if (model != null)
                    {
                        model.ServiceId = reportColumn.ServiceId;
                        model.ColumnName = reportColumn.ColumnName.Replace(" ", "");
                        model.IsMandatory = reportColumn.IsMandatory;
                        model.DisplayInFilter = reportColumn.DisplayInFilter;
                        model.UpdatedBy = CurrentUserId;
                        model.UpdatedOn = System.DateTime.UtcNow;
                        model.ShowInGrid = reportColumn.ShowInGrid;
                        #region RolePrivileges
                        var RolePrivileges = db.SalesRolePrivileges.Where(t => t.ColumnId == reportColumn.Id).Select(a => a.RoleId).ToList();
                        foreach (var item in RolePrivileges)
                        {
                            var delete = db.SalesRolePrivileges.Where(a => a.ColumnId == reportColumn.Id & a.RoleId == item).FirstOrDefault();
                            db.SalesRolePrivileges.Remove(delete);
                            db.SaveChanges();
                        }
                        if (reportColumn.RolePrivileges != null)
                        {
                            foreach (var item in reportColumn.RolePrivileges)
                            {
                                db.SalesRolePrivileges.Add(new SalesRolePrivilege
                                {
                                    ColumnId = reportColumn.Id,
                                    RoleId = item,
                                    CreatedBy = CurrentUserId,
                                    CreatedOn = System.DateTime.UtcNow,
                                    UpdatedBy = CurrentUserId,
                                    UpdatedOn = System.DateTime.UtcNow
                                });
                                db.SaveChanges();
                            }
                        }
                        #endregion

                        #region DepartmentPrivileges
                        var DepartmentPrivileges = db.SalesDepartmentPrivileges.Where(t => t.ColumnId == reportColumn.Id).Select(a => a.DepartmentId).ToList();
                        foreach (var item in DepartmentPrivileges)
                        {
                            var delete = db.SalesDepartmentPrivileges.Where(a => a.ColumnId == reportColumn.Id & a.DepartmentId == item).FirstOrDefault();
                            db.SalesDepartmentPrivileges.Remove(delete);
                            db.SaveChanges();
                        }
                        if (reportColumn.DepartmentPrivileges != null)
                        {
                            foreach (var item in reportColumn.DepartmentPrivileges)
                            {
                                db.SalesDepartmentPrivileges.Add(new SalesDepartmentPrivilege
                                {
                                    ColumnId = reportColumn.Id,
                                    DepartmentId = item,
                                    CreatedBy = CurrentUserId,
                                    CreatedOn = System.DateTime.UtcNow,
                                    UpdatedBy = CurrentUserId,
                                    UpdatedOn = System.DateTime.UtcNow
                                });
                                db.SaveChanges();
                            }
                        }
                        #endregion

                        #region UserPrivileges
                        var UserPrivileges = db.SalesUserPrivileges.Where(t => t.ColumnId == reportColumn.Id).Select(a => a.UserId).ToList();
                        foreach (var item in UserPrivileges)
                        {
                            var delete = db.SalesUserPrivileges.Where(a => a.ColumnId == reportColumn.Id & a.UserId == item).FirstOrDefault();
                            db.SalesUserPrivileges.Remove(delete);
                            db.SaveChanges();
                        }
                        if (reportColumn.UserPrivileges != null)
                        {
                            foreach (var item in reportColumn.UserPrivileges)
                            {
                                db.SalesUserPrivileges.Add(new SalesUserPrivilege {
                                    ColumnId = reportColumn.Id,
                                    UserId = item,
                                    CreatedBy = CurrentUserId,
                                    CreatedOn = System.DateTime.UtcNow,
                                    UpdatedBy = CurrentUserId,
                                    UpdatedOn = System.DateTime.UtcNow
                                });
                                db.SaveChanges();
                            }
                        }
                        #endregion
                        db.SaveChanges();
                    }
                    //reportColumn.ColumnName = reportColumn.ColumnName.Replace(" ", "");
                    //reportColumn.BusinessId = (int)CurrentBusinessId;
                    //reportColumn.UpdatedBy = CurrentUserId;
                    //reportColumn.UpdatedOn = System.DateTime.UtcNow;
                    //db.Entry(reportColumn).State = EntityState.Modified;

                    return RedirectToAction("Index");
                }
                ViewBag.RolePrivilegesList = new SelectList(db.Roles.Where(a => a.BusinessId == CurrentBusinessId), "Id", "Name");
                ViewBag.DepartmentPrivilegesList = new SelectList(db.Departments.Where(a => a.BusinessId == CurrentBusinessId), "Id", "DepartmentName");
                ViewBag.UserPrivilegesList = new SelectList(db.Users.Where(a => a.BusinessId == CurrentUser.BusinessId && a.IsActive == true && !a.Roles.Select(y => y.Name).Contains("BusinessAdmin") && !a.Roles.Select(y => y.Name).Contains("SuperAdmin") && !a.Roles.Select(y => y.Name).Contains("MasterAdmin")), "Id", "FirstName");
                ViewBag.ServiceId = new SelectList(db.LookupEnrolledServices.Where(a => a.BusinessId == CurrentBusinessId), "Id", "ServiceName", reportColumn.ServiceId);
                ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName", reportColumn.CreatedBy);
                ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "FirstName", reportColumn.UpdatedBy);
                return View(reportColumn);
            }
        }

        // GET: Business/ReportColumns/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportColumn reportColumn = db.ReportColumns.Find(id);
            if (reportColumn == null)
            {
                return HttpNotFound();
            }
            return View(reportColumn);
        }

        // POST: Business/ReportColumns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ReportColumn reportColumn = db.ReportColumns.Find(id);
            try
            {
                db.ReportColumns.Remove(reportColumn);
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("Error", "There are some releted item in database, please delete those first");
                return View("Delete", reportColumn);
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
