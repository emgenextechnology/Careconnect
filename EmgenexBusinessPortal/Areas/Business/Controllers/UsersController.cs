using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using EBP.Business.Database;
using GM.Identity.Models;
using System.Threading.Tasks;
using EmgenexBusinessPortal.Models;
using GM.Identity;
using EBP.Business.Repository;

using EntityFramework.Extensions;
using System.Data.Entity.Infrastructure;
using EBP.Business;
using System.Configuration;
using System.Globalization;
using EmgenexBusinessPortal.Helpers;
using EBP.Business.Helpers;
using System.Text;
using System.IO;
using System.Xml.Linq;
using GM.Identity.Config;

namespace EmgenexBusinessPortal.Areas.Business.Controllers
{
    [BusinessAuthorize]
    public class UsersController : BaseController
    {
        //private EmegenexBiz2016Entities db = new EmegenexBiz2016Entities();

        public UsersController()
        {
        }

        public UsersController(GMUserManager userManager, GMRoleManager roleManager)
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
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private string MapperFilePath
        {
            get
            {
                var mapperFilePath = Server.MapPath(Path.Combine("~/Assets", "Users", "UserMapper.xml"));

                if (!System.IO.File.Exists(mapperFilePath))
                    return null;

                return mapperFilePath;
            }
        }

        // GET: Business/Users
        public ActionResult Index(string SearchKey, int page = 1, bool IsPartial = false)
        {
            var users = db.Users.Where(a => a.BusinessId == CurrentUser.BusinessId && !a.Roles.Select(y => y.Name).Contains("BusinessAdmin") && !a.Roles.Select(y => y.Name).Contains("SuperAdmin") && !a.Roles.Select(y => y.Name).Contains("MasterAdmin"));

            if (!string.IsNullOrEmpty(SearchKey))
            {
                users = users.Where(ua => (
                    ua.FirstName + " " + ua.MiddleName).ToLower().Contains(SearchKey.ToLower())
                    || (ua.FirstName + " " + ua.LastName).ToLower().Contains(SearchKey.ToLower())
                    || (ua.FirstName + " " + ua.MiddleName + " " + ua.LastName).ToLower().Contains(SearchKey.ToLower())
                    || ua.FirstName.ToLower().Contains(SearchKey.ToLower())
                    || ua.MiddleName.ToLower().Contains(SearchKey.ToLower())
                    || ua.LastName.ToLower().Contains(SearchKey.ToLower())
                    || ua.Email.ToLower().Contains(SearchKey.ToLower())
                    || ua.UserName.ToLower().Contains(SearchKey.ToLower())
                    || ua.Roles.Any(t => t.Name.ToLower().Contains(SearchKey.ToLower()))
                    || ua.Departments.Any(t => t.DepartmentName.ToLower().Contains(SearchKey.ToLower()))
                    || ua.PhoneNumber.ToLower().Contains(SearchKey.ToLower()));
                ViewBag.SearchKey = SearchKey;
            }
            var count = users.Count();
            ViewBag.Count = count;
            ViewBag.page = page;
            var pager = new Pager(count, page, 10);
            ViewBag.StartPage = pager.StartPage;
            ViewBag.EndPage = pager.EndPage;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPage = pager.TotalPages;
            var query = users.OrderByDescending(a => a.Id).Skip(((page - 1) * pager.PageSize)).Take(pager.PageSize).ToList();
            if (IsPartial)
            {
                return PartialView("_PartialUsersList", query);
            }
            return View(query);
        }

        // GET: Business/Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Business/Users/Create
        public ActionResult Create()
        {
            ViewBag.Department = new SelectList(db.Departments.Where(a => a.BusinessId == CurrentUser.BusinessId).OrderBy(a => a.DepartmentName), "Id", "DepartmentName");
            ViewBag.RoleId = new SelectList(db.Roles.Where(a => a.BusinessId == CurrentUser.BusinessId).OrderBy(a => a.Name), "Id", "Name");
            ViewBag.StateId = new SelectList(db.LookupStates.Where(a => a.IsActive == true).OrderBy(a => a.StateName), "Id", "StateName");
            return View();
        }

        // POST: Business/Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterViewModel userViewModel, int[] selectedDepartments, bool IsXml = false, params string[] selectedRoles)
        {
            if (!IsXml)
            {
                ModelState["Password"].Errors.Clear();
                ModelState["ConfirmPassword"].Errors.Clear();
            }
            userViewModel.Password = userViewModel.ConfirmPassword = GeneralHelpers.GeneratePassword(3, 2, 2);
            ViewBag.RoleId = new SelectList(db.Roles.Where(a => a.BusinessId == CurrentUser.BusinessId).OrderBy(a => a.Name), "Id", "Name");
            ViewBag.Department = new SelectList(db.Departments.Where(a => a.BusinessId == CurrentUser.BusinessId).OrderBy(a => a.DepartmentName), "Id", "DepartmentName");
            ViewBag.StateId = new SelectList(db.LookupStates.Where(a => a.IsActive == true).OrderBy(a => a.StateName), "Id", "StateName");
            if (selectedRoles == null)
            {
                ModelState.AddModelError("Error", "Plese select at least one role.");
                return View(userViewModel);
            }
            if (ModelState.IsValid)
            {
                var user = new GMUser { FirstName = userViewModel.FirstName, MiddleName = userViewModel.MiddleName, LastName = userViewModel.LastName, PhoneNumber = userViewModel.PhoneNumber, UserName = userViewModel.Email, Email = userViewModel.Email, BusinessId = CurrentBusinessId, IsActive = true };
                var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);

                //Add User to the selected Roles 
                if (adminresult.Succeeded)
                {
                    db.UserProfiles.Add(new UserProfile
                    {
                        UserId = user.Id,
                        WorkEmail = userViewModel.WorkEmail,
                        //ALternateEmail = userViewModel.ALternateEmail,
                        HomePhone = userViewModel.HomePhone,
                        AdditionalPhone = userViewModel.AdditionalPhone,
                        //Address = userViewModel.Address,
                        AddressLine1 = userViewModel.AddressLine1,
                        AddressLine2 = userViewModel.AddressLine2,
                        City = userViewModel.City,
                        Zip = userViewModel.Zip,
                        StateId = userViewModel.StateId,
                        CreatedOn = System.DateTime.UtcNow,
                        CreatedBy = CurrentUserId,
                        UpdatedOn = System.DateTime.UtcNow,
                        UpdatedBy = CurrentUserId,
                        Startdate = string.IsNullOrEmpty(userViewModel.StartDate) ? (DateTime?)null : DateTime.ParseExact(userViewModel.StartDate, "MM-dd-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None)
                    });
                    db.SaveChanges();

                    if (selectedRoles != null)
                    {
                        IdentityResult result = null;
                        foreach (var roleId in selectedRoles)
                        {
                            result = await UserManager.AddToRoleAsync(user.Id, roleId);
                        }
                    }
                    if (selectedDepartments != null && selectedDepartments.Count() > 0)
                    {
                        foreach (var item in selectedDepartments)
                        {
                            db.UserDepartments.Add(new UserDepartment { UserId = user.Id, DepartmentId = item, CreatedBy = CurrentUser.Id, CreatedOn = DateTime.UtcNow });
                            db.SaveChanges();
                        }
                    }
                    var UserDepartments = db.UserDepartments.Where(a => a.UserId == user.Id).Select(a => a.Department.DepartmentName);
                    string ReturnUrl = ConfigurationManager.AppSettings["BaseUrl"];
                    ViewData.Model = new EmgenexBusinessPortal.Models.EmailModel.UserEmailModel
                    {
                        CurentUserName = CurrentUser.FirstName,
                        FirstName = user.FirstName,
                        BusinessName = CurrentUser.BusinessName,
                        UserName = userViewModel.Email,
                        Password = userViewModel.Password,
                        UserDepartments = UserDepartments,
                        ReturnUrl = ReturnUrl + CurrentUser.RelativeUrl.Replace(" ", "-")
                    };
                    var emailBody = RazorHelper.RenderRazorViewToString("~/Views/Shared/Email/AddNewUser.cshtml", this);
                    await UserManager.SendEmailAsync(user.Id, "New user registration", emailBody);
                }
                else
                {
                    ModelState.AddModelError("", adminresult.Errors.First());
                    return View(userViewModel);
                }
                if (IsXml)
                {
                    return Json(true);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            return View(userViewModel);
        }

        // GET: Business/Users/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (db == null)
                db = new CareConnectCrmEntities();

            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var userRoles = await UserManager.GetRolesAsync(user.Id);
            var UserDepartment = db.UserDepartments.Where(a => a.UserId == user.Id).Select(a => a.Department.DepartmentName);

            var UsersPrivilegeList = db.UserPrivileges.Where(a => a.UserId == user.Id && a.BusinessId == CurrentUser.BusinessId).Select(a => a.PrivilegeId);

            var privileges = db.Privileges.Select(x => new SelectListItem()
            {
                Selected = UsersPrivilegeList.Contains(x.Id),
                Text = x.Title,
                Value = x.Id.ToString()
            }).ToList();
            var UserProfile = db.UserProfiles.FirstOrDefault(a => a.UserId == user.Id);
            var model = new EditUserViewModel()
            {

                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                UserName = user.UserName,
                RolesList = db.Roles.Where(a => a.BusinessId == CurrentUser.BusinessId).Select(x => new SelectListItem()
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList(),
                Departments = db.Departments.Where(a => a.BusinessId == CurrentUser.BusinessId).Select(x => new SelectListItem()
                {
                    Selected = UserDepartment.Contains(x.DepartmentName),
                    Text = x.DepartmentName,
                    Value = x.Id.ToString()
                }).ToList()

            };
            if (UserProfile != null)
            {
                model.WorkEmail = UserProfile.WorkEmail;
                //model.ALternateEmail = UserProfile.ALternateEmail;
                model.HomePhone = UserProfile.HomePhone;
                model.AdditionalPhone = UserProfile.AdditionalPhone;
                //model.Address = UserProfile.Address;
                model.AddressLine1 = UserProfile.AddressLine1;
                model.AddressLine2 = UserProfile.AddressLine2;
                model.City = UserProfile.City;
                model.Zip = UserProfile.Zip;
                model.StateId = UserProfile.StateId;
                model.CreatedBy = UserProfile.CreatedBy;
                model.CreatedOn = UserProfile.CreatedOn;
                //model.add
                model.StartDate = UserProfile.Startdate == null ? null : UserProfile.Startdate.GetValueOrDefault().ToString("MM-dd-yyyy");
            }

            // var stateList = db.LookupStates.Where(a => a.IsActive == true).Select(a=>new { Id= a.Id, StateName=a.StateName}).OrderBy(a => a.StateName).ToList();

            // ViewBag.StateId = new SelectList(stateList, "Id", "StateName", UserProfile == null ? null : UserProfile.StateId);

            if (TempData["EditError"] != null)
            {
                ViewData = (ViewDataDictionary)TempData["EditError"];
            }

            return View(model);
        }

        // POST: Business/Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Email,Id,FirstName,MiddleName,LastName,PhoneNumber,CreatedBy,CreatedOn,AdditionalPhone,WorkEmail,ALternateEmail,HomePhone,CellPhone,Address,StartDate,AddressLine1,AddressLine2,City,Zip,StateId")] EditUserViewModel editUser, int[] selectedDepartments, params string[] selectedRoles)
        {
            if (selectedRoles == null)
            {
                ModelState.AddModelError("Error", "Plese select at least one role.");
                TempData["EditError"] = ViewData;

                return RedirectToAction("Edit", new { id = editUser.Id });
            }
            var userRoles = await UserManager.GetRolesAsync(editUser.Id);
            var UserProfile = db.UserProfiles.FirstOrDefault(a => a.UserId == editUser.Id);
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(editUser.Id);

                if (user == null)
                {
                    return HttpNotFound();
                }

                user.FirstName = editUser.FirstName;
                user.MiddleName = editUser.MiddleName;
                user.LastName = editUser.LastName;
                user.PhoneNumber = editUser.PhoneNumber;
                user.Email = editUser.Email;
                //user.UserName = editUser.Email;
                var userUpdate = UserManager.Update(user);
                if (!userUpdate.Succeeded)
                {

                    user.ClearCache(user.UserName);

                    ModelState.AddModelError("", userUpdate.Errors.First());
                    //editUser.RolesList = db.Roles.Where(a => a.BusinessId == CurrentUser.BusinessId).ToList().Select(x => new SelectListItem()
                    //{
                    //    Selected = userRoles.Contains(x.Name),
                    //    Text = x.Name,
                    //    Value = x.Id.ToString()
                    //});
                    //editUser.Departments = new SelectList(db.Departments.Where(a => a.BusinessId == CurrentUser.BusinessId).OrderBy(a=>a.DepartmentName), "Id", "DepartmentName");
                    TempData["EditError"] = ViewData;

                    return RedirectToAction("Edit", new { id = editUser.Id });
                }

                #region Role

                selectedRoles = selectedRoles ?? new string[] { };

                IdentityResult result = null;

                IdentityResult removerole = await UserManager.RemoveFromRoleAsync(user.Id, string.Empty);

                foreach (var roleId in selectedRoles)
                {
                    result = await UserManager.AddToRoleAsync(user.Id, roleId);
                }

                #endregion

                #region Departments

                IEnumerable<UserDepartment> UserDepartment = db.UserDepartments.Where(a => a.UserId == user.Id).ToList();
                if (UserDepartment.Count() > 0)
                {
                    db.UserDepartments.RemoveRange(UserDepartment);
                    db.SaveChanges();
                }
                if (selectedDepartments != null && selectedDepartments.Count() > 0)
                {
                    foreach (var item in selectedDepartments)
                    {
                        db.UserDepartments.Add(new UserDepartment { UserId = user.Id, DepartmentId = item, CreatedBy = CurrentUser.Id, CreatedOn = DateTime.UtcNow });
                        db.SaveChanges();
                    }
                }
                #endregion
                #region User Profile
                if (UserProfile != null)
                {
                    UserProfile.WorkEmail = editUser.WorkEmail;
                    //UserProfile.ALternateEmail = editUser.ALternateEmail;
                    UserProfile.HomePhone = editUser.HomePhone;
                    UserProfile.AdditionalPhone = editUser.AdditionalPhone;
                    //UserProfile.Address = editUser.Address;
                    UserProfile.AddressLine1 = editUser.AddressLine1;
                    UserProfile.AddressLine2 = editUser.AddressLine2;
                    UserProfile.City = editUser.City;
                    UserProfile.Zip = editUser.Zip;
                    UserProfile.StateId = editUser.StateId;
                    UserProfile.CreatedBy = editUser.CreatedBy;
                    UserProfile.CreatedOn = editUser.CreatedOn;
                    UserProfile.UpdatedBy = CurrentUserId;
                    UserProfile.UpdatedOn = System.DateTime.UtcNow;
                    if (!string.IsNullOrEmpty(editUser.StartDate))
                    {
                        UserProfile.Startdate = DateTime.ParseExact(editUser.StartDate, "MM-dd-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);
                    }
                    else
                    {
                        UserProfile.Startdate = null;
                    }
                    db.SaveChanges();
                }
                else
                {
                    db.UserProfiles.Add(new UserProfile
                    {
                        UserId = user.Id,
                        WorkEmail = editUser.WorkEmail,
                        HomePhone = editUser.HomePhone,
                        AdditionalPhone = editUser.AdditionalPhone,
                        AddressLine1 = editUser.AddressLine1,
                        AddressLine2 = editUser.AddressLine2,
                        City = editUser.City,
                        Zip = editUser.Zip,
                        StateId = editUser.StateId,
                        CreatedOn = System.DateTime.UtcNow,
                        CreatedBy = CurrentUserId,
                        UpdatedOn = System.DateTime.UtcNow,
                        UpdatedBy = CurrentUserId,
                        Startdate = string.IsNullOrEmpty(editUser.StartDate) ? (DateTime?)null : DateTime.ParseExact(editUser.StartDate, "MM-dd-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None)
                    });
                    db.SaveChanges();
                }

                #endregion

                return RedirectToAction("Index");
            }

            //ModelState.AddModelError("", "Something failed.");
            ViewBag.StateId = new SelectList(db.LookupStates.Where(a => a.IsActive == true).OrderBy(a => a.StateName), "Id", "StateName", UserProfile == null ? null : UserProfile.StateId);

            TempData["EditError"] = ViewData;

            return RedirectToAction("Edit", new { id = editUser.Id });
        }

        public ActionResult Privileges(int id, DepartmentPrivilegesModel model)
        {
            model.UserId = id;
            model.User = new RepositoryUserProfile().GetUserbyId(id);
            var UserPrivilage = db.UserPrivileges.Where(a => a.UserId == id && a.BusinessId == CurrentUser.BusinessId).ToList();
            model.UserDepartments = db.UserDepartments.Where(a => a.UserId == id).Select(a => new DepartmentPrivileges
            {
                DepartmentName = a.Department.DepartmentName,
                Privileges = a.Department.DepartmentPrivileges.Where(b => b.DepartmentId == a.DepartmentId).Select(c => new SelectListItem
                {
                    Value = c.Privilege.Id.ToString(),
                    Text = c.Privilege.Title
                })
            });
            model.UserRoles = db.Roles.Where(a => a.BusinessId == CurrentUser.BusinessId && a.Users.Any(u => u.Id == id)).Select(a => new DepartmentPrivileges
            {
                DepartmentName = a.Name,
                Privileges = a.RolePrivileges.Where(b => b.RoleId == a.Id).Select(c => new SelectListItem
                {
                    Value = c.Privilege.Id.ToString(),
                    Text = c.Privilege.Title
                })
            });
            model.Modules = db.Privileges.GroupBy(a => a.ModuleId).Select(g => new Modulesmodel
            {
                ModuleName = g.FirstOrDefault(a => a.ModulesMaster.Id == g.Key).ModulesMaster.Title,
                UserPrivileges = g.Select(b => new PrivilegesModel
                {
                    Id = b.Id,
                    Name = b.Title,
                }).ToList()

            }).OrderByDescending(a => a.ModuleName).ToList();
            model.Modules.ForEach(a => a.UserPrivileges.ForEach(b => b.Deny = UserPrivilage.Any(c => c.PrivilegeId == b.Id) ? (bool?)UserPrivilage.Any(c => c.PrivilegeId == b.Id && c.IsDeny == true) : null));
            return View(model);
        }

        [HttpPost]
        public JsonResult SetPrivileges(List<PrivilegesModel> entity)
        {
            var responseMessage = string.Empty;
            bool isSucess = false;
            if (entity != null)
            {
                var userId = entity.FirstOrDefault().UserId;
                int userPrivileges = db.UserPrivileges.Where(a => a.UserId == userId).Delete();
                if (userPrivileges > 0)
                {
                    responseMessage = "Sucessfully Saved";
                    isSucess = true;
                }
                else
                {
                    responseMessage = "Failed!";
                    isSucess = false;
                }

                foreach (var privilege in entity)
                {
                    if (privilege.Id != 0)
                    {
                        db.UserPrivileges.Add(new UserPrivilege
                        {
                            UserId = privilege.UserId,
                            PrivilegeId = privilege.Id,
                            IsDeny = privilege.Deny,
                            CreatedOn = DateTime.UtcNow,
                            CreatedBy = CurrentUser.Id,
                            BusinessId = CurrentUser.BusinessId
                        });
                    }
                }

                int result = db.SaveChanges();
                if (result > 0)
                {
                    var _userPrivileges = db.UserPrivileges.Where(a => a.UserId == userId).Select(a => new EmgenexBusinessPortal.Models.EmailModel.UserPrivilege
                    {
                        Title = a.Privilege.Title,
                        Description = a.Privilege.Description
                    }).ToList();

                    var UserDetails = db.Users.Find(userId);

                    UserDetails.ClearCache(UserDetails.UserName);

                    ViewData.Model = new EmgenexBusinessPortal.Models.EmailModel.UserPrivilegeModel
                    {
                        CreatedUserFirstName = CurrentUser.FirstName,
                        CreatedUserMiddleName = CurrentUser.MiddleName,
                        CreatedUserLastName = CurrentUser.LastName,
                        UserPrivileges = _userPrivileges,
                        AssignedUserFirstName = UserDetails.FirstName,
                        AssignedUserMiddleName = UserDetails.MiddleName,
                        AssignedUserLastName = UserDetails.LastName,
                        BusinessName = CurrentUser.BusinessName,
                    };
                    var emailBody = RazorHelper.RenderRazorViewToString("~/Views/Shared/Email/AddPrivilegestoUser.cshtml", this);
                    UserManager.SendEmailAsync(userId, "New Privilege Added", emailBody);
                    responseMessage = "Sucessfully Saved";
                    isSucess = true;
                }
            }
            return Json(new { Sucess = isSucess, message = responseMessage });
        }

        // GET: Business/Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Business/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            try
            {
                db.Users.Remove(user);
                //user.IsActive = false;
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("Error", "There are some releted item in database, please delete those first");
                return View("Delete", user);
            }
            return RedirectToAction("Index");
        }
        public ActionResult ToggleStatus(int UserId, bool IsActive)
        {
            User user = db.Users.Find(UserId);
            user.IsActive = IsActive;
            user.LockoutEnabled = IsActive;
            db.SaveChanges();
            return Json(true);
        }

        public async Task<ActionResult> SetUserPassword(int id)
        {
            var User = await UserManager.FindByIdAsync(id);
            ViewBag.UserName = User.FirstName;
            return View(new SetPasswordViewModel { UserId = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetUserPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var RemovePassword = await UserManager.RemovePasswordAsync(model.UserId);
                var result = await UserManager.AddPasswordAsync(model.UserId, model.NewPassword);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                AddErrors(result);
            }
            return View(model);
        }

        public async Task<ActionResult> Importexcel()
        {
            if (Request.Files != null && Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                var fileName = file.FileName;
                string rootPath = Server.MapPath("~/Assets"),
                fileNameWithoutExt = string.Format("{0:yyyyMMddhhmmssfff}", DateTime.Now),
                extension = Path.GetExtension(fileName);
                fileName = string.Format("{0}{1}", fileNameWithoutExt, extension);
                string fileDirectory = Path.Combine(rootPath, CurrentBusinessId.Value.ToString(), "Users", "Uploads");
                if (!Directory.Exists(fileDirectory))
                    Directory.CreateDirectory(fileDirectory);
                string fullPath = Path.Combine(fileDirectory, fileName);

                int count = 1;
                isExist:
                if (System.IO.File.Exists(fullPath))
                {
                    fileName = string.Format("{0}{1}{2}", fileNameWithoutExt, count, extension);
                    fullPath = Path.Combine(fileDirectory, fileName);
                    count++;
                    goto isExist;
                }
                file.SaveAs(fullPath);

                XmlHelper xmlHelper = new XmlHelper();

                string excelFile = Server.MapPath(Path.Combine("~/Assets", CurrentBusinessId.Value.ToString(), "Users", "Uploads", fileName.ToString()));

                xmlHelper.XmlMapper = XDocument.Load(this.MapperFilePath);

                int RecordCount;
                using (StreamReader sr = new StreamReader(excelFile))
                {
                    Stream ExcelStream = sr.BaseStream;
                    xmlHelper.xmlString = new ExcelToXml().GetXMLString(ref ExcelStream, true, out RecordCount);
                    ExcelStream.Close();
                };
                XDocument XmlMapper = xmlHelper.XmlMapper;
                var xmlUserData = XmlMapper.Descendants("Users");
                for (int i = 0; i < RecordCount; i++)
                {
                    var xmlUsers = xmlUserData.Select(u =>
                                                                  new XMLRegisterModel
                                                                  {
                                                                      FirstName = xmlHelper.GetNodeValue(ref u, "FirstName").Value,
                                                                      MiddleName = xmlHelper.GetNodeValue(ref u, "MiddleName").Value,
                                                                      LastName = xmlHelper.GetNodeValue(ref u, "LastName").Value,
                                                                      Email = xmlHelper.GetNodeValue(ref u, "Email").Value,
                                                                      PhoneNumber = xmlHelper.GetNodeValue(ref u, "Phone").Value,
                                                                      WorkEmail = xmlHelper.GetNodeValue(ref u, "WorkEmail").Value,
                                                                      AdditionalPhone = xmlHelper.GetNodeValue(ref u, "AdditionalPhone").Value,
                                                                      AddressLine1 = xmlHelper.GetNodeValue(ref u, "AddressLine1").Value,
                                                                      AddressLine2 = xmlHelper.GetNodeValue(ref u, "AddressLine2").Value,
                                                                      City = xmlHelper.GetNodeValue(ref u, "City").Value,
                                                                      State = xmlHelper.GetNodeValue(ref u, "State").Value,
                                                                      Zip = xmlHelper.GetNodeValue(ref u, "Zip").Value,
                                                                      UserName = xmlHelper.GetNodeValue(ref u, "Email").Value,
                                                                      RoleName = xmlHelper.GetNodeValue(ref u, "Role").Value,
                                                                      DepartmentName = xmlHelper.GetNodeValue(ref u, "Department").Value,
                                                                      StartDate = xmlHelper.GetNodeValue(ref u, "StartDate").Value,
                                                                      SalesTeam = xmlHelper.GetNodeValue(ref u, "SalesTeam").Value,
                                                                      Services = xmlHelper.GetNodeValue(ref u, "Services").Value,
                                                                  }).FirstOrDefault();
                    if (xmlUsers != null)
                    {
                        #region Create Role
                        xmlUsers.RoleName = xmlUsers.RoleName != null ? xmlUsers.RoleName.Trim() : null;
                        xmlUsers.DepartmentName = xmlUsers.DepartmentName != null ? xmlUsers.DepartmentName.Trim() : null;
                        xmlUsers.SalesTeam = xmlUsers.SalesTeam != null ? xmlUsers.SalesTeam.Trim() : null;
                        if (!string.IsNullOrEmpty(xmlUsers.RoleName))
                        {
                            var roleModel = db.Roles.Count(a => a.BusinessId == CurrentBusinessId && a.Name.ToLower() == xmlUsers.RoleName.ToLower());
                            if (roleModel == 0)
                            {
                                db.Roles.Add(new Role
                                {
                                    Name = xmlUsers.RoleName,
                                    Description = xmlUsers.RoleName,
                                    BusinessId = CurrentBusinessId,
                                    IsActive = true,
                                    CreatedBy = CurrentUser.Id,
                                    CreatedOn = DateTime.UtcNow
                                });
                                db.SaveChanges();
                            }
                        }
                        #endregion

                        #region Create Department
                        if (!string.IsNullOrEmpty(xmlUsers.DepartmentName))
                        {
                            var DepartmentModel = db.Departments.Count(a => a.BusinessId == CurrentBusinessId && a.DepartmentName.ToLower() == xmlUsers.DepartmentName.ToLower());
                            if (DepartmentModel == 0)
                            {
                                db.Departments.Add(new Department
                                {
                                    DepartmentName = xmlUsers.DepartmentName,
                                    Description = xmlUsers.DepartmentName,
                                    BusinessId = CurrentBusinessId,
                                    IsActive = true,
                                    CreatedBy = CurrentUser.Id,
                                    CreatedOn = DateTime.UtcNow
                                });
                                db.SaveChanges();
                            }
                        }
                        #endregion

                        string[] selectedRoles = db.Roles.Where(a => a.BusinessId == CurrentBusinessId && a.Name.ToLower() == xmlUsers.RoleName.ToLower()).Select(a => a.Id.ToString()).ToArray();
                        int[] selectedDepartments = db.Departments.Where(a => a.BusinessId == CurrentBusinessId && a.DepartmentName.ToLower() == xmlUsers.DepartmentName.ToLower()).Select(a => a.Id).ToArray();

                        xmlUsers.Password = xmlUsers.ConfirmPassword = GeneralHelpers.GeneratePassword(3, 2, 2);
                        if (selectedRoles == null)
                        {
                        }
                        //if (ModelState.IsValid)
                        //{
                        var user = new GMUser { FirstName = xmlUsers.FirstName, MiddleName = xmlUsers.MiddleName, LastName = xmlUsers.LastName, PhoneNumber = xmlUsers.PhoneNumber, UserName = xmlUsers.Email, Email = xmlUsers.Email, BusinessId = CurrentBusinessId, IsActive = true };
                        var adminresult = await UserManager.CreateAsync(user, xmlUsers.Password);

                        if (adminresult.Succeeded)
                        {
                            db.UserProfiles.Add(new UserProfile
                            {
                                UserId = user.Id,
                                WorkEmail = xmlUsers.WorkEmail,
                                HomePhone = xmlUsers.HomePhone,
                                AdditionalPhone = xmlUsers.AdditionalPhone,
                                AddressLine1 = xmlUsers.AddressLine1,
                                AddressLine2 = xmlUsers.AddressLine2,
                                City = xmlUsers.City,
                                Zip = xmlUsers.Zip,
                                StateId = xmlUsers.StateId,
                                CreatedOn = System.DateTime.UtcNow,
                                CreatedBy = CurrentUserId,
                                UpdatedOn = System.DateTime.UtcNow,
                                UpdatedBy = CurrentUserId,
                                Startdate = string.IsNullOrEmpty(xmlUsers.StartDate) ? (DateTime?)null : DateTime.ParseExact(xmlUsers.StartDate, "MM-dd-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None)
                            });
                            db.SaveChanges();

                            if (selectedRoles != null)
                            {
                                IdentityResult result = null;
                                foreach (var roleId in selectedRoles)
                                {
                                    result = await UserManager.AddToRoleAsync(user.Id, roleId);
                                }
                            }

                            if (selectedDepartments != null && selectedDepartments.Count() > 0)
                            {
                                foreach (var item in selectedDepartments)
                                {
                                    db.UserDepartments.Add(new UserDepartment { UserId = user.Id, DepartmentId = item, CreatedBy = CurrentUser.Id, CreatedOn = DateTime.UtcNow });
                                    db.SaveChanges();
                                }
                            }
                            #region Create SalesTeam

                            if (!string.IsNullOrEmpty(xmlUsers.SalesTeam))
                            {
                                var RepGroupModel = db.RepGroups.FirstOrDefault(a => a.BusinessId == CurrentBusinessId && a.RepGroupName.ToLower() == xmlUsers.SalesTeam.ToLower());
                                if (RepGroupModel == null)
                                {
                                    var RepGroup = db.RepGroups.Add(new RepGroup
                                    {
                                        BusinessId = CurrentUser.BusinessId,
                                        RepGroupName = xmlUsers.SalesTeam,
                                        Description = xmlUsers.SalesTeam,
                                        CreatedBy = CurrentUser.Id,
                                        CreatedOn = DateTime.UtcNow,
                                        IsActive = true,
                                    });
                                    if (db.SaveChanges() > 0)
                                    {
                                        #region Selected Roles
                                        switch (xmlUsers.RoleName.ToLower())
                                        {
                                            case "sales rep":
                                                var Reps = db.Reps.Add(new Rep
                                                {
                                                    UserId = user.Id,
                                                    RepGroupId = RepGroup.Id,
                                                    IsActive = true,
                                                    CreatedBy = CurrentUserId,
                                                    CreatedOn = System.DateTime.UtcNow,
                                                });
                                                if (db.SaveChanges() > 0)
                                                {
                                                    var Services = db.LookupEnrolledServices.Where(a => a.BusinessId == CurrentBusinessId && a.IsActive == true).Select(a => a.Id).ToList();
                                                    if (Services != null && Services.Count() > 0)
                                                    {
                                                        foreach (var item in Services)
                                                        {
                                                            db.RepServiceMappers.Add(new RepServiceMapper { ServiceId = item, RepId = Reps.Id, CreatedBy = CurrentUser.Id, CreatedOn = DateTime.UtcNow });
                                                            db.SaveChanges();
                                                        }
                                                    }
                                                }
                                                break;

                                            case "sales manager":
                                                db.RepgroupManagerMappers.Add(new RepgroupManagerMapper
                                                {
                                                    RepGroupId = RepGroup.Id,
                                                    ManagerId = user.Id,
                                                    CreatedBy = CurrentUser.Id,
                                                    CreatedOn = DateTime.UtcNow
                                                });
                                                db.SaveChanges();

                                                #region SalesRep

                                                var salesReps = db.Reps.Add(new Rep
                                                {
                                                    UserId = user.Id,
                                                    RepGroupId = RepGroup.Id,
                                                    IsActive = true,
                                                    CreatedBy = CurrentUserId,
                                                    CreatedOn = System.DateTime.UtcNow,
                                                });
                                                if (db.SaveChanges() > 0)
                                                {
                                                    var Services = db.LookupEnrolledServices.Where(a => a.BusinessId == CurrentBusinessId && a.IsActive == true).Select(a => a.Id).ToList();
                                                    if (Services != null && Services.Count() > 0)
                                                    {
                                                        foreach (var item in Services)
                                                        {
                                                            db.RepServiceMappers.Add(new RepServiceMapper { ServiceId = item, RepId = salesReps.Id, CreatedBy = CurrentUser.Id, CreatedOn = DateTime.UtcNow });
                                                            db.SaveChanges();
                                                        }
                                                    }
                                                }

                                                #endregion

                                                break;

                                            case "sales director":
                                                //RepGroup.SalesDirectorId = user.Id;
                                                db.SaveChanges();
                                                break;

                                        }
                                        #endregion
                                    }
                                }
                                else
                                {
                                    #region Selected Roles

                                    switch (xmlUsers.RoleName.ToLower())
                                    {
                                        case "sales rep":
                                            var Reps = db.Reps.Add(new Rep
                                            {
                                                UserId = user.Id,
                                                RepGroupId = RepGroupModel.Id,
                                                IsActive = true,
                                                CreatedBy = CurrentUserId,
                                                CreatedOn = System.DateTime.UtcNow,
                                            });

                                            if (db.SaveChanges() > 0)
                                            {
                                                var Services = db.LookupEnrolledServices.Where(a => a.BusinessId == CurrentBusinessId && a.IsActive == true).Select(a => a.Id).ToList();
                                                if (Services != null && Services.Count() > 0)
                                                {
                                                    foreach (var item in Services)
                                                    {
                                                        db.RepServiceMappers.Add(new RepServiceMapper { ServiceId = item, RepId = Reps.Id, CreatedBy = CurrentUser.Id, CreatedOn = DateTime.UtcNow });
                                                        db.SaveChanges();
                                                    }
                                                }
                                            }
                                            break;

                                        case "sales manager":
                                            db.RepgroupManagerMappers.Add(new RepgroupManagerMapper
                                            {
                                                RepGroupId = RepGroupModel.Id,
                                                ManagerId = user.Id,
                                                CreatedBy = CurrentUser.Id,
                                                CreatedOn = DateTime.UtcNow
                                            });
                                            db.SaveChanges();
                                            #region SalesRep

                                            var salesReps = db.Reps.Add(new Rep
                                            {
                                                UserId = user.Id,
                                                RepGroupId = RepGroupModel.Id,
                                                IsActive = true,
                                                CreatedBy = CurrentUserId,
                                                CreatedOn = System.DateTime.UtcNow,
                                            });
                                            if (db.SaveChanges() > 0)
                                            {
                                                var Services = db.LookupEnrolledServices.Where(a => a.BusinessId == CurrentBusinessId && a.IsActive == true).Select(a => a.Id).ToList();
                                                if (Services != null && Services.Count() > 0)
                                                {
                                                    foreach (var item in Services)
                                                    {
                                                        db.RepServiceMappers.Add(new RepServiceMapper { ServiceId = item, RepId = salesReps.Id, CreatedBy = CurrentUser.Id, CreatedOn = DateTime.UtcNow });
                                                        db.SaveChanges();
                                                    }
                                                }
                                            }

                                            #endregion
                                            break;

                                        case "sales director":
                                            RepGroupModel.SalesDirectorId = user.Id;
                                            db.SaveChanges();
                                            break;
                                    }

                                    #endregion
                                }
                            }

                            #endregion


                            #region Email Notification
                            var UserDepartments = db.UserDepartments.Where(a => a.UserId == user.Id).Select(a => a.Department.DepartmentName);
                            string ReturnUrl = ConfigurationManager.AppSettings["BaseUrl"];
                            try
                            {
                                ViewData.Model = new EmgenexBusinessPortal.Models.EmailModel.UserEmailModel
                                {
                                    CurentUserName = CurrentUser.FirstName,
                                    FirstName = user.FirstName,
                                    BusinessName = CurrentUser.BusinessName,
                                    UserName = xmlUsers.Email,
                                    Password = xmlUsers.Password,
                                    UserDepartments = UserDepartments,
                                    ReturnUrl = ReturnUrl + CurrentUser.RelativeUrl.Replace(" ", "-")
                                };

                                var emailBody = RazorHelper.RenderRazorViewToString("~/Views/Shared/Email/AddNewUser.cshtml", this);

                                if (ConfigurationManager.AppSettings["IsInDemo"] == "true")
                                {
                                    string DemoEmailId = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["DemoEmailId"]) ? ConfigurationManager.AppSettings["DemoEmailId"] : null;
                                    var mail = new GMEmail();
                                    if (DemoEmailId != null)
                                    {
                                        mail.SendDynamicHTMLEmail(DemoEmailId, "New user registration", emailBody, "", "");
                                    }
                                }
                                else
                                {
                                    await UserManager.SendEmailAsync(user.Id, "New user registration", emailBody);
                                }
                            }
                            catch (Exception ex)
                            {
                                ex.Log();
                            }
                            #endregion

                        }
                        else
                        {
                            ModelState.AddModelError("", adminresult.Errors.First());
                        }
                        //}
                        xmlHelper.RemoveFirstChild();
                    }
                }
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
