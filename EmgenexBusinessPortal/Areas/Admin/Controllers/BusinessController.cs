using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EBP.Business;
using EBP.Business.Database;
using EmgenexBusinessPortal.Areas.Admin.Models;
using GM.Identity.Models;
using Microsoft.AspNet.Identity.Owin;
using GM.Identity;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Configuration;

using Microsoft.AspNet.Identity;
using EmgenexBusinessPortal.Helpers;
using EBP.Business.Helpers;

namespace EmgenexBusinessPortal.Areas.Admin.Controllers
{
    [Authorize]
    public class BusinessController : BaseController
    {
        public BusinessController()
        {
        }

        public BusinessController(GMUserManager userManager, GMRoleManager roleManager)
        {

            UserManager = userManager;
            RoleManager = roleManager;
        }

        private GMUserManager _userManager;
        public GMUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<GMUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private GMRoleManager _roleManager;
        public GMRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<GMRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
        // GET: Admin/Business
        public ActionResult Index(string SearchKey, int page = 1, bool IsPartial = false)
        {
            var Business = db.BusinessMasters.Where(a => 1 == 1);
            //ViewBag.Count = Business.Count();
            ViewBag.page = page;
            if (!string.IsNullOrEmpty(SearchKey))
            {
                Business = Business.Where(ua => ua.BusinessName.ToLower().Contains(SearchKey.ToLower()));
                ViewBag.SearchKey = SearchKey;
            }
            var totalRecord = Business.Count();
            var pager = new Pager(Business.Count(), page, 10);
            ViewBag.StartPage = pager.StartPage;
            ViewBag.EndPage = pager.EndPage;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPage = pager.TotalPages;
            var query = Business.OrderByDescending(a => a.CreatedOn).Skip(((page - 1) * pager.PageSize)).Take(pager.PageSize).ToList();
            if (IsPartial)
            {
                return PartialView("_PartialBusinessList", query);
            }
            return View(query);
        }

        // GET: Admin/Business/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusinessMaster businessMaster = db.BusinessMasters.Find(id);
            if (businessMaster == null)
            {
                return HttpNotFound();
            }
            return View(businessMaster);
        }

        // GET: Admin/Business/Create
        public async Task<ActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(BusinessModel model)
        {
            var userManager = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<GMUserManager>();
            var roleManager = System.Web.HttpContext.Current.GetOwinContext().Get<GMRoleManager>();
            if (ModelState.IsValid)
            {
                if (db.BusinessMasters.Any(a => a.BusinessName.ToLower() == model.BusinessName.ToLower()))
                {
                    ModelState.AddModelError("Error", string.Format("{0} is already used.", model.BusinessName));
                    return View(model);
                }

                model.Password = GeneralHelpers.GeneratePassword(3, 2, 2);
                var user = new GMUser
                {
                    FirstName = model.FirstName,
                    MiddleName = model.MiddleName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    UserName = model.Email,
                    Email = model.Email,
                    IsActive = true,
                };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    string ReturnUrl = ConfigurationManager.AppSettings["BaseUrl"];
                    ViewData.Model = new EmgenexBusinessPortal.Models.EmailModel.BusinessEmailModel
                    {
                        CurentUserName = CurrentUser.FirstName,
                        FirstName = model.FirstName,
                        BusinessName = model.BusinessName,
                        UserName = model.Email,
                        Password = model.Password,
                        ReturnUrl = ReturnUrl + model.BusinessName.Replace(" ", "-")
                    };
                    var roleName = "BusinessAdmin";
                    var role = roleManager.FindByName(roleName);
                    if (role == null)
                    {
                        db.Roles.Add(new Role { Name = roleName, Description = roleName, IsActive = true, CreatedBy = CurrentUser.Id, CreatedOn = System.DateTime.UtcNow });
                        db.SaveChanges();
                    }
                    await UserManager.AddToRolesAsync(user.Id, roleName);
                    var emailBody = RazorHelper.RenderRazorViewToString("~/Views/Shared/Email/AddNewBusiness.cshtml", this);
                    await UserManager.SendEmailAsync(user.Id, "New Business", emailBody);
                }
                else
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View(model);
                }
                var Business = db.BusinessMasters.Add(new BusinessMaster
                {
                    BusinessName = model.BusinessName,
                    RelativeUrl = model.BusinessName.ToLower().Replace(" ", "-"),
                    Description = model.Description,
                    CreatedBy = CurrentUser.Id,
                    CreatedOn = System.DateTime.UtcNow,
                    IsActive = true,
                    Status = 1
                });
                if (db.SaveChanges() > 0)
                {
                    var userDetails = db.Users.FirstOrDefault(a => a.Id == user.Id);
                    userDetails.UserProfiles.Add(new UserProfile { CreatedBy = CurrentUser.Id, CreatedOn = DateTime.UtcNow, UserId = userDetails.Id });
                    userDetails.BusinessId = Business.Id;
                    db.SaveChanges();
                    //string rootPath = ConfigurationManager.AppSettings["FolderPath"];
                    string rootPath = Server.MapPath("~/Assets");
                    string path = Path.Combine(rootPath, Business.Id.ToString());
                    DirectoryInfo dir = new DirectoryInfo(path);
                    if (!dir.Exists)
                        dir.Create();
                }
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Admin/Business/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusinessMaster businessMaster = db.BusinessMasters.Find(id);
            if (businessMaster == null)
            {
                return HttpNotFound();
            }
            return View(businessMaster);
        }
        [HttpPost]
        public ActionResult Edit(BusinessMaster businessMaster)
        {
            if (!ModelState.IsValid) return View(businessMaster);
            if (db.BusinessMasters.Any(a => (a.Id != businessMaster.Id && a.BusinessName.ToLower() == businessMaster.BusinessName.ToLower())))
            {
                ModelState.AddModelError("Error", string.Format("{0} is already used.", businessMaster.BusinessName));
                return View(businessMaster);
            }
            var model = db.BusinessMasters.FirstOrDefault(a => a.Id == businessMaster.Id);
            model.BusinessName = businessMaster.BusinessName;
            model.Description = businessMaster.Description;
            model.RelativeUrl = model.BusinessName.ToLower().Replace(" ", "-");
            model.IsActive = businessMaster.IsActive;
            model.UpdatedBy = CurrentUser.Id;
            model.UpdatedOn = System.DateTime.UtcNow;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        // GET: Admin/Business/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusinessMaster businessMaster = db.BusinessMasters.Find(id);
            if (businessMaster == null)
            {
                return HttpNotFound();
            }
            return View(businessMaster);
        }

        // POST: Admin/Business/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BusinessMaster businessMaster = db.BusinessMasters.Find(id);
            try
            {
                db.BusinessMasters.Remove(businessMaster);
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("Error", "There are some releted item in database, please delete those first");
                return View("Delete", businessMaster);
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
