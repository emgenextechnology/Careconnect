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
using System.Configuration;
using System.IO;
using EmgenexBusinessPortal.Areas.Business.Models;
using EntityFramework.Extensions;

namespace EmgenexBusinessPortal.Areas.Business.Controllers
{
    [Authorize]
    [BusinessAuthorize]
    public class EnrolledServicesController : BaseController
    {
        //private EmegenexBiz2016Entities db = new EmegenexBiz2016Entities();

        // GET: Admin/EnrolledServices
        public ActionResult Index()
        {
            var lookupEnrolledServices = db.LookupEnrolledServices.Where(a => a.BusinessId == CurrentBusinessId).Include(l => l.User).Include(l => l.User1);
            return View(lookupEnrolledServices.OrderByDescending(a => a.CreatedOn).ToList());
        }

        // GET: Admin/EnrolledServices/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupEnrolledService lookupEnrolledService = db.LookupEnrolledServices.Find(id);
            if (lookupEnrolledService == null)
            {
                return HttpNotFound();
            }
            return View(lookupEnrolledService);
        }

        // GET: Admin/EnrolledServices/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/EnrolledServices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EnrolledServiceModel lookupEnrolledService)
        {
            if ("#FFFFFF" == "#" + lookupEnrolledService.ServiceColor)
            {
                ModelState.AddModelError("SelectColorError", "please select any other color!");
                return View(lookupEnrolledService);
            }
            if (db.LookupEnrolledServices.Count(a => a.ServiceColor == "#" + lookupEnrolledService.ServiceColor) > 0)
            {
                ModelState.AddModelError("ServiceColorError", "Already exists!");
                return View(lookupEnrolledService);
            }
            if (ModelState.IsValid)
            {
                var EnrolledServicesModel = db.LookupEnrolledServices.Add(new LookupEnrolledService
                {
                    ServiceName = lookupEnrolledService.ServiceName,
                    ServiceDecription = lookupEnrolledService.ServiceDecription,
                    CreatedBy = CurrentUser.Id,
                    CreatedOn = DateTime.UtcNow,
                    BusinessId = (int)CurrentBusinessId,
                    ServiceColor = "#" + lookupEnrolledService.ServiceColor,
                    IsActive = true,
                    ImportMode = lookupEnrolledService.ImportMode,
                    BoxUrl = lookupEnrolledService.ImportMode == (int)EBP.Business.Enums.ServiceReportImportModes.BoxAPI ? lookupEnrolledService.BoxUrl : null,
                });
                if (db.SaveChanges() > 0)
                {
                    string rootPath = Server.MapPath("~/Assets");
                    string path = Path.Combine(rootPath, CurrentBusinessId.ToString(), "Sales", lookupEnrolledService.ServiceName.Replace(" ", "").ToString());
                    DirectoryInfo dir = new DirectoryInfo(path);
                    if (!dir.Exists)
                        dir.Create();
                    #region FTP
                    if (lookupEnrolledService.ImportMode == (int)EBP.Business.Enums.ServiceReportImportModes.Ftp)
                    {
                        db.ServiceFtpInfoes.Add(new ServiceFtpInfo
                        {
                            HostName = lookupEnrolledService.HostName,
                            Protocol = lookupEnrolledService.Protocol,
                            Username = lookupEnrolledService.Username,
                            Password = lookupEnrolledService.Password,
                            RemotePath = lookupEnrolledService.RemotePath,
                            PortNumber = lookupEnrolledService.PortNumber,
                            ServiceId = EnrolledServicesModel.Id
                        });
                        db.SaveChanges();
                    }
                    #endregion
                }
                return RedirectToAction("Index");
            }
            return View(lookupEnrolledService);
        }
        // GET: Admin/EnrolledServices/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupEnrolledService lookupEnrolledService = db.LookupEnrolledServices.Find(id);
            if (lookupEnrolledService == null)
            {
                return HttpNotFound();
            }
            ViewBag.OldServiceName = lookupEnrolledService.ServiceName;
            var EnrolledServicesModel = new EnrolledServiceModel();
            EnrolledServicesModel.ServiceName = lookupEnrolledService.ServiceName;
            EnrolledServicesModel.ServiceDecription = lookupEnrolledService.ServiceDecription;
            EnrolledServicesModel.ServiceColor = lookupEnrolledService.ServiceColor;
            EnrolledServicesModel.ImportMode = lookupEnrolledService.ImportMode;
            EnrolledServicesModel.BoxUrl = lookupEnrolledService.BoxUrl;
            EnrolledServicesModel.IsActive = lookupEnrolledService.IsActive;
            if (lookupEnrolledService.ImportMode == (int)EBP.Business.Enums.ServiceReportImportModes.Ftp)
            {
                var ftpModel = lookupEnrolledService.ServiceFtpInfoes.FirstOrDefault(a => a.ServiceId == id);
                if (ftpModel != null)
                {
                    EnrolledServicesModel.Protocol = ftpModel.Protocol;
                    EnrolledServicesModel.HostName = ftpModel.HostName;
                    EnrolledServicesModel.Username = ftpModel.Username;
                    EnrolledServicesModel.Password = ftpModel.Password;
                    EnrolledServicesModel.PortNumber = ftpModel.PortNumber;
                    EnrolledServicesModel.RemotePath = ftpModel.RemotePath;
                }
            }
            return View(EnrolledServicesModel);
        }
        // POST: Admin/EnrolledServices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EnrolledServiceModel lookupEnrolledService)
        {
            if ("#FFFFFF" == "#" + lookupEnrolledService.ServiceColor)
            {
                ModelState.AddModelError("SelectColorError", "please select any other color!");
                return View(lookupEnrolledService);
            }
            if (db.LookupEnrolledServices.Count(a => a.Id != lookupEnrolledService.Id && a.ServiceColor == "#" + lookupEnrolledService.ServiceColor) > 0)
            {
                ModelState.AddModelError("ServiceColorError", "Already exists!");
                return View(lookupEnrolledService);
            }
            if (ModelState.IsValid)
            {
                string rootPath = Server.MapPath("~/Assets");
                var model = db.LookupEnrolledServices.Find(lookupEnrolledService.Id);
                string OldPath = Path.Combine(rootPath, CurrentBusinessId.ToString(), "Sales", model.ServiceName.Replace(" ", "").ToString());
                model.ServiceName = lookupEnrolledService.ServiceName;
                model.ServiceDecription = lookupEnrolledService.ServiceDecription;
                model.IsActive = lookupEnrolledService.IsActive;
                model.UpdatedBy = CurrentUser.Id;
                model.UpdatedOn = DateTime.UtcNow;
                model.ServiceColor = "#" + lookupEnrolledService.ServiceColor;
                model.ImportMode = lookupEnrolledService.ImportMode;
                model.BoxUrl = lookupEnrolledService.ImportMode == (int)EBP.Business.Enums.ServiceReportImportModes.BoxAPI ? lookupEnrolledService.BoxUrl : null;
                if (db.SaveChanges() > 0)
                {
                    string NewPath = Path.Combine(rootPath, CurrentBusinessId.ToString(), "Sales", model.ServiceName.Replace(" ", "").ToString());
                    DirectoryInfo dir = new DirectoryInfo(NewPath);
                    DirectoryInfo oldDir = new DirectoryInfo(OldPath);
                    if (oldDir.Exists)
                    {
                        if (OldPath != NewPath)
                            System.IO.Directory.Move(OldPath, NewPath);
                    }
                    else
                    {
                        if (!dir.Exists)
                            dir.Create();
                    }
                    if (lookupEnrolledService.ImportMode == (int)EBP.Business.Enums.ServiceReportImportModes.Ftp)
                    {
                        var ftpModel = model.ServiceFtpInfoes.FirstOrDefault(a => a.ServiceId == lookupEnrolledService.Id);
                        if (ftpModel != null)
                        {
                            ftpModel.Protocol = lookupEnrolledService.Protocol;
                            ftpModel.HostName = lookupEnrolledService.HostName;
                            ftpModel.Username = lookupEnrolledService.Username;
                            ftpModel.Password = lookupEnrolledService.Password;
                            ftpModel.PortNumber = lookupEnrolledService.PortNumber;
                            ftpModel.RemotePath = lookupEnrolledService.RemotePath;
                            db.SaveChanges();
                        }
                        else
                        {
                            db.ServiceFtpInfoes.Add(new ServiceFtpInfo
                            {
                                HostName = lookupEnrolledService.HostName,
                                Protocol = lookupEnrolledService.Protocol,
                                Username = lookupEnrolledService.Username,
                                Password = lookupEnrolledService.Password,
                                RemotePath = lookupEnrolledService.RemotePath,
                                PortNumber = lookupEnrolledService.PortNumber,
                                ServiceId = lookupEnrolledService.Id
                            });
                            db.SaveChanges();
                        }
                    }
                }
                return RedirectToAction("Index");
            }
            return View(lookupEnrolledService);
        }
        // GET: Admin/EnrolledServices/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupEnrolledService lookupEnrolledService = db.LookupEnrolledServices.Find(id);
            if (lookupEnrolledService == null)
            {
                return HttpNotFound();
            }
            return View(lookupEnrolledService);
        }

        // POST: Admin/EnrolledServices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var ServiceFtpInfoes = db.ServiceFtpInfoes.Where(a => a.ServiceId == id).FirstOrDefault();
            if (ServiceFtpInfoes != null)
            {
                db.ServiceFtpInfoes.Remove(ServiceFtpInfoes);
            }
            LookupEnrolledService lookupEnrolledService = db.LookupEnrolledServices.Find(id);
            try
            {
                db.LookupEnrolledServices.Remove(lookupEnrolledService);
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("Error", "There are some releted item in database, please delete those first");
                return View("Delete", lookupEnrolledService);
            }
            return RedirectToAction("Index");
        }

        public ActionResult SetDefaultService(ServiceModel entity)
        {
            var lookupEnrolledServices = db.LookupEnrolledServices.Where(a => a.BusinessId == CurrentBusinessId).ToList();
            var model = lookupEnrolledServices.FirstOrDefault(a => a.Id == entity.ServiceId);
            if (entity.Status == true)
            {
                lookupEnrolledServices.ForEach(a => a.Status = false);
            }
            model.Status = entity.Status;
            db.SaveChanges();
            return Json(new { ServiceId = entity.ServiceId });
        }

        #region ReportColumns
        public ActionResult ReportColumns(int id)
        {
            var reportColumns = db.ReportColumns.Where(a => a.BusinessId == CurrentBusinessId && a.LookupEnrolledService.Id == id && a.LookupEnrolledService.BusinessId == CurrentBusinessId).Include(r => r.BusinessMaster).Include(r => r.User).Include(r => r.User1);
            ViewBag.ServiceId = id;
            ViewBag.ServiceName = db.LookupEnrolledServices.FirstOrDefault(a => a.Id == id).ServiceName;
            return View(reportColumns.ToList());
        }
        public ActionResult ReportColumnCreate(int id)
        {
            ViewBag.RolePrivilegesList = new SelectList(db.Roles.Where(a => a.BusinessId == CurrentBusinessId).OrderBy(a => a.Name), "Id", "Name");
            ViewBag.DepartmentPrivilegesList = new SelectList(db.Departments.Where(a => a.BusinessId == CurrentBusinessId).OrderBy(a => a.DepartmentName), "Id", "DepartmentName");
            //ViewBag.UserPrivilegesList = new SelectList(db.Users.Where(a => a.BusinessId == CurrentUser.BusinessId && a.IsActive == true && !a.Roles.Select(y => y.Name).Contains("BusinessAdmin") && !a.Roles.Select(y => y.Name).Contains("SuperAdmin") && !a.Roles.Select(y => y.Name).Contains("MasterAdmin")), "Id", "FirstName");
            ViewBag.UserPrivilegesList = new SelectList(db.Users.Where(a => a.BusinessId == CurrentUser.BusinessId && a.IsActive == true && !a.Roles.Select(y => y.Name).Contains("BusinessAdmin") && !a.Roles.Select(y => y.Name).Contains("SuperAdmin") && !a.Roles.Select(y => y.Name).Contains("MasterAdmin")).Select(s => new
            {
                Id = s.Id,
                UserName = s.FirstName + " " + s.LastName + " @" + s.UserName
            }), "Id", "UserName").OrderBy(a => a.Text);
            ViewBag.ServiceId = new SelectList(db.LookupEnrolledServices.Where(a => a.BusinessId == CurrentBusinessId).OrderBy(a => a.ServiceName), "Id", "ServiceName");
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "FirstName");
            return View(new ReportColumnModel { ServiceId = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReportColumnCreate(ReportColumnModel reportColumn)
        {
            string[] reservedFields = new string[] { "Id", "BusinessId", "ServiceId", "PatientId", "PatientFirstName", "PatientLastName", "SpecimenCollectionDate", "SpecimenReceivedDate", "ReportedDate","PracticeId",
            "PracticeName", "ProviderId", "ProviderFirstName", "ProviderLastName", "ProviderNpi", "RepId","RepFirstName", "RepLastName", "CreatedOn", "CreatedBy",
            "UpdatedOn", "UpdatedBy", "OldId", "IsColumnValuesImported", "OldReportId"};
            if (reservedFields.Contains(reportColumn.ColumnName))
            {
                ModelState.AddModelError("Error", string.Format("{0} is reserved, please choose another column name", reportColumn.ColumnName));
                return View(reportColumn);
            }
            if (db.ReportColumns.FirstOrDefault(a => a.BusinessId == CurrentBusinessId && a.ServiceId == reportColumn.ServiceId && a.ColumnName.ToLower().Replace(" ", "") == reportColumn.ColumnName.ToLower().Replace(" ", "")) != null)
            {
                ViewBag.RolePrivilegesList = new SelectList(db.Roles.Where(a => a.BusinessId == CurrentBusinessId).OrderBy(a => a.Name), "Id", "Name");
                ViewBag.DepartmentPrivilegesList = new SelectList(db.Departments.Where(a => a.BusinessId == CurrentBusinessId).OrderBy(a => a.DepartmentName), "Id", "DepartmentName");
                ViewBag.UserPrivilegesList = new SelectList(db.Users.Where(a => a.BusinessId == CurrentUser.BusinessId && a.IsActive == true && !a.Roles.Select(y => y.Name).Contains("BusinessAdmin") && !a.Roles.Select(y => y.Name).Contains("SuperAdmin") && !a.Roles.Select(y => y.Name).Contains("MasterAdmin")).Select(s => new
                {
                    Id = s.Id,
                    UserName = s.FirstName + " " + s.LastName + " @" + s.UserName
                }), "Id", "UserName").OrderBy(a => a.Text);
                ViewBag.ServiceId = new SelectList(db.LookupEnrolledServices.Where(a => a.BusinessId == CurrentBusinessId).OrderBy(a => a.ServiceName), "Id", "ServiceName");
                ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName");
                ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "FirstName");
                ModelState.AddModelError("Error", "Already Used");
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
                        ShowInGrid = reportColumn.ShowInGrid,
                        DisplayName = reportColumn.DisplayName,
                        ColumnType = reportColumn.ColumnType,
                        InputType = reportColumn.InputType
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
                    return RedirectToAction("ReportColumns", new { id = reportColumn.ServiceId });
                }
                ViewBag.RolePrivilegesList = new SelectList(db.Roles.Where(a => a.BusinessId == CurrentBusinessId).OrderBy(a => a.Name), "Id", "Name");
                ViewBag.DepartmentPrivilegesList = new SelectList(db.Departments.Where(a => a.BusinessId == CurrentBusinessId).OrderBy(a => a.DepartmentName), "Id", "DepartmentName");
                ViewBag.UserPrivilegesList = new SelectList(db.Users.Where(a => a.BusinessId == CurrentUser.BusinessId && a.IsActive == true && !a.Roles.Select(y => y.Name).Contains("BusinessAdmin") && !a.Roles.Select(y => y.Name).Contains("SuperAdmin") && !a.Roles.Select(y => y.Name).Contains("MasterAdmin")).Select(s => new
                {
                    Id = s.Id,
                    UserName = s.FirstName + " " + s.LastName + " @" + s.UserName
                }), "Id", "UserName").OrderBy(a => a.Text);
                ViewBag.ServiceId = new SelectList(db.LookupEnrolledServices.Where(a => a.BusinessId == CurrentBusinessId).OrderBy(a => a.ServiceName), "Id", "ServiceName");
                ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName");
                ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "FirstName");
                return View(reportColumn);
            }
        }
        public ActionResult ReportColumnEdit(int? id)
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
                ShowInGrid = reportColumn.ShowInGrid,
                DisplayName = reportColumn.DisplayName,
                ColumnType = reportColumn.ColumnType,
                InputType = reportColumn.InputType
            };
            ViewBag.RolePrivilegesList = new SelectList(db.Roles.Where(a => a.BusinessId == CurrentBusinessId).OrderBy(a => a.Name), "Id", "Name");
            ViewBag.DepartmentPrivilegesList = new SelectList(db.Departments.Where(a => a.BusinessId == CurrentBusinessId).OrderBy(a => a.DepartmentName), "Id", "DepartmentName");
            //ViewBag.UserPrivilegesList = new SelectList(db.Users.Where(a => a.BusinessId == CurrentUser.BusinessId && a.IsActive == true && !a.Roles.Select(y => y.Name).Contains("BusinessAdmin") && !a.Roles.Select(y => y.Name).Contains("SuperAdmin") && !a.Roles.Select(y => y.Name).Contains("MasterAdmin")), "Id", "FirstName");
            ViewBag.UserPrivilegesList = new SelectList(db.Users.Where(a => a.BusinessId == CurrentUser.BusinessId && a.IsActive == true && !a.Roles.Select(y => y.Name).Contains("BusinessAdmin") && !a.Roles.Select(y => y.Name).Contains("SuperAdmin") && !a.Roles.Select(y => y.Name).Contains("MasterAdmin")).Select(s => new
            {
                Id = s.Id,
                UserName = s.FirstName + " " + s.LastName + " @" + s.UserName
            }), "Id", "UserName").OrderBy(a => a.Text);
            ViewBag.ServiceId = new SelectList(db.LookupEnrolledServices.Where(a => a.BusinessId == CurrentBusinessId).OrderBy(a => a.ServiceName), "Id", "ServiceName", reportColumn.ServiceId);
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName", reportColumn.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "FirstName", reportColumn.UpdatedBy);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReportColumnEdit(ReportColumnModel reportColumn)
        {
            string[] reservedFields = new string[] { "Id", "BusinessId", "ServiceId", "PatientId", "PatientFirstName", "PatientLastName", "SpecimenCollectionDate", "SpecimenReceivedDate", "ReportedDate","PracticeId",
            "PracticeName", "ProviderId", "ProviderFirstName", "ProviderLastName", "ProviderNpi", "RepId","RepFirstName", "RepLastName", "CreatedOn", "CreatedBy",
            "UpdatedOn", "UpdatedBy", "OldId", "IsColumnValuesImported", "OldReportId"};
            if (reservedFields.Contains(reportColumn.ColumnName))
            {
                ModelState.AddModelError("Error", string.Format("{0} is reserved, please choose another column name", reportColumn.ColumnName));
                return View(reportColumn);
            }
            if (db.ReportColumns.FirstOrDefault(a => a.Id != reportColumn.Id && a.BusinessId == CurrentBusinessId && a.ServiceId == reportColumn.ServiceId && a.ColumnName.ToLower().Replace(" ", "") == reportColumn.ColumnName.ToLower().Replace(" ", "")) != null)
            {
                ViewBag.RolePrivilegesList = new SelectList(db.Roles.Where(a => a.BusinessId == CurrentBusinessId).OrderBy(a => a.Name), "Id", "Name");
                ViewBag.DepartmentPrivilegesList = new SelectList(db.Departments.Where(a => a.BusinessId == CurrentBusinessId).OrderBy(a => a.DepartmentName), "Id", "DepartmentName");
                //ViewBag.UserPrivilegesList = new SelectList(db.Users.Where(a => a.BusinessId == CurrentUser.BusinessId && a.IsActive == true && !a.Roles.Select(y => y.Name).Contains("BusinessAdmin") && !a.Roles.Select(y => y.Name).Contains("SuperAdmin") && !a.Roles.Select(y => y.Name).Contains("MasterAdmin")), "Id", "FirstName");
                ViewBag.UserPrivilegesList = new SelectList(db.Users.Where(a => a.BusinessId == CurrentUser.BusinessId && a.IsActive == true && !a.Roles.Select(y => y.Name).Contains("BusinessAdmin") && !a.Roles.Select(y => y.Name).Contains("SuperAdmin") && !a.Roles.Select(y => y.Name).Contains("MasterAdmin")).Select(s => new
                {
                    Id = s.Id,
                    UserName = s.FirstName + " " + s.LastName + " @" + s.UserName
                }), "Id", "UserName").OrderBy(a => a.Text);
                ViewBag.ServiceId = new SelectList(db.LookupEnrolledServices.Where(a => a.BusinessId == CurrentBusinessId).OrderBy(a => a.ServiceName), "Id", "ServiceName", reportColumn.ServiceId);
                ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName", reportColumn.CreatedBy);
                ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "FirstName", reportColumn.UpdatedBy);
                ModelState.AddModelError("Error", "Already Used");
                return View(reportColumn);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var model = db.ReportColumns.Find(reportColumn.Id);
                    string oldColumnName = model.ColumnName;

                    if (model != null)
                    {
                        model.ServiceId = reportColumn.ServiceId;
                        model.ColumnName = reportColumn.ColumnName.Replace(" ", "");
                        model.IsMandatory = reportColumn.IsMandatory;
                        model.DisplayInFilter = reportColumn.DisplayInFilter;
                        model.UpdatedBy = CurrentUserId;
                        model.UpdatedOn = System.DateTime.UtcNow;
                        model.ShowInGrid = reportColumn.ShowInGrid;
                        model.DisplayName = reportColumn.DisplayName;
                        model.ColumnType = reportColumn.ColumnType;
                        model.InputType = reportColumn.InputType;
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
                                db.SalesUserPrivileges.Add(new SalesUserPrivilege
                                {
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

                        var objUserColumnVisibility = db.UserColumnVisibilities.Where(a => a.BusinessId == CurrentBusinessId && a.ColumnName == oldColumnName && a.ServiceId == model.ServiceId);
                        if (objUserColumnVisibility.Count() > 0)
                            foreach (var item in objUserColumnVisibility)
                            {
                                item.ColumnName = reportColumn.ColumnName;
                                item.DisplayName = reportColumn.DisplayName;
                            }

                        db.SaveChanges();
                    }
                    return RedirectToAction("ReportColumns", new { id = reportColumn.ServiceId });
                }
                ViewBag.RolePrivilegesList = new SelectList(db.Roles.Where(a => a.BusinessId == CurrentBusinessId).OrderBy(a => a.Name), "Id", "Name");
                ViewBag.DepartmentPrivilegesList = new SelectList(db.Departments.Where(a => a.BusinessId == CurrentBusinessId).OrderBy(a => a.DepartmentName), "Id", "DepartmentName");
                //ViewBag.UserPrivilegesList = new SelectList(db.Users.Where(a => a.BusinessId == CurrentUser.BusinessId && a.IsActive == true && !a.Roles.Select(y => y.Name).Contains("BusinessAdmin") && !a.Roles.Select(y => y.Name).Contains("SuperAdmin") && !a.Roles.Select(y => y.Name).Contains("MasterAdmin")), "Id", "FirstName");
                ViewBag.UserPrivilegesList = new SelectList(db.Users.Where(a => a.BusinessId == CurrentUser.BusinessId && a.IsActive == true && !a.Roles.Select(y => y.Name).Contains("BusinessAdmin") && !a.Roles.Select(y => y.Name).Contains("SuperAdmin") && !a.Roles.Select(y => y.Name).Contains("MasterAdmin")).Select(s => new
                {
                    Id = s.Id,
                    UserName = s.FirstName + " " + s.LastName + " @" + s.UserName
                }), "Id", "UserName").OrderBy(a => a.Text);
                ViewBag.ServiceId = new SelectList(db.LookupEnrolledServices.Where(a => a.BusinessId == CurrentBusinessId).OrderBy(a => a.ServiceName), "Id", "ServiceName", reportColumn.ServiceId);
                ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName", reportColumn.CreatedBy);
                ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "FirstName", reportColumn.UpdatedBy);
                return View(reportColumn);
            }
        }
        public ActionResult ReportColumnDetails(int? id)
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
        public ActionResult ReportColumnDelete(int? id)
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

        [HttpPost]
        public ActionResult ReportColumnDelete(int id)
        {
            ReportColumn reportColumn = db.ReportColumns.Find(id);
            try
            {
                db.ReportColumns.Remove(reportColumn);
                if (db.SaveChanges() > 0)
                {
                    db.UserColumnVisibilities.Where(a => a.BusinessId == CurrentBusinessId && a.ColumnName == reportColumn.ColumnName && a.ServiceId == reportColumn.ServiceId).Delete();
                }
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("Error", "There are some releted item in database, please delete those first");
                return View("ReportColumnDelete", reportColumn);
            }
            return RedirectToAction("ReportColumns", new { id = reportColumn.ServiceId });
        }

        #region Input lookup
        public ActionResult ColumnLookups(int id)
        {
            var ReportColumns = db.ReportColumns.FirstOrDefault(a => a.BusinessId == CurrentBusinessId && a.Id == id);
            ViewBag.Serviceid = ReportColumns.ServiceId;
            ViewBag.ColumnId = id;
            ViewBag.ColumnName = ReportColumns.ColumnName;
            var lookupService = db.LookupServiceColumns.Where(a => a.ColumnId == id).ToList();
            return View(lookupService);
        }
        public ActionResult Add(int id)
        {
            return View(new LookupServiceColumnsModel { ColumnId = id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(LookupServiceColumnsModel entity)
        {
            if (ModelState.IsValid)
            {
                var model = db.LookupServiceColumns.Add(new LookupServiceColumn
                {
                    Text = entity.Text,
                    ColumnId = entity.ColumnId,
                    CreatedBy = CurrentUserId,
                    CreatedOn = System.DateTime.UtcNow,
                });
                db.SaveChanges();
                return RedirectToAction("ColumnLookups", new { id = entity.ColumnId });
            }
            return View(entity);
        }
        public ActionResult Update(int id)
        {
            var model = db.LookupServiceColumns.Where(a => a.Id == id).Select(a => new LookupServiceColumnsModel { Id = a.Id, Text = a.Text, ColumnId = a.ColumnId }).FirstOrDefault();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(LookupServiceColumnsModel entity)
        {
            if (ModelState.IsValid)
            {
                var model = db.LookupServiceColumns.FirstOrDefault(a => a.Id == entity.Id);
                model.Text = entity.Text;
                model.UpdatedBy = CurrentUserId;
                model.UpdatedOn = System.DateTime.UtcNow;
                db.SaveChanges();
                return RedirectToAction("ColumnLookups", new { id = entity.ColumnId });
            }
            return View(entity);
        }
        public ActionResult LookupServiceColumnDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupServiceColumn LookupServiceColumn = db.LookupServiceColumns.Find(id);
            if (LookupServiceColumn == null)
            {
                return HttpNotFound();
            }
            return View(LookupServiceColumn);
        }

        [HttpPost]
        public ActionResult LookupServiceColumnDelete(int id)
        {
            LookupServiceColumn LookupServiceColumn = db.LookupServiceColumns.Find(id);
            try
            {
                db.LookupServiceColumns.Remove(LookupServiceColumn);
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("Error", "There are some releted item in database, please delete those first");
                return View("LookupServiceColumnDelete", LookupServiceColumn);
            }
            return RedirectToAction("ColumnLookups", new { id = LookupServiceColumn.ColumnId });
        }
        #endregion
        #endregion

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
