using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using EBP.Business.Database;
using EBP.Business.Filter;
using EBP.Business;
using EBP.Business.Entity;
using EmgenexBusinessPortal.Controllers;
using EBP.Business.Repository;
using EBP.Business.Entity.Users;
using EBP.Business.Entity.Privileges;
using EmgenexBusinessPortal.Models;
using GM.Identity.Models;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using EmgenexBusinessPortal.Areas.BusinessSettings.Models;
using EBP.Business.Helpers;
using GM.Identity;
using System.Globalization;
using System.Configuration;
using GM.Identity.Config;
using EBP.Business.Notifications;

namespace EmgenexBusinessPortal.Areas.BusinessSettings.Controllers
{
    [RoutePrefix("Users")]

    public class ApiUsersController : ApiBaseController
    {
        private GMUserManager _userManager;

        public ApiUsersController()
        {
        }

        public ApiUsersController(GMUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public GMUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<GMUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }
        protected int CurrentUserId { get { return 2; } }
        protected int? CurrentBusinessId { get { return 1; } }
        [HttpPost]
        [Route("getbyfilter")]
        [Route("")]
        public IHttpActionResult GetByFilter(FilterUsers filter)
        {
            var repository = new RepositoryUsers();
            if (filter == null)
                filter = new FilterUsers { PageSize = 25, CurrentPage = 1 };
            var response = repository.GetUsers(filter, CurrentBusinessId.Value);
            return Ok<DataResponse<EntityList<EntityUsers>>>(response);
        }
        [Route("getUserbyid/{Id}")]
        [Route("{Id}")]
        public IHttpActionResult GetUserById(int? Id)
        {
            var response = new DataResponse<EntityUsers>();
            var repository = new RepositoryUsers();
            if (Id.HasValue)
            {
                response = repository.GetUserById(Id.Value);
            }
            else
            {
                response.Model = new EntityUsers();
            }
            return Ok<DataResponse>(response);
        }

        [Route("getprivilegebyuserid/{UserId}")]
        [Route("{UserId}/privileges")]
        public IHttpActionResult GetPrivilegeById(int? UserId)
        {
            var response = new DataResponse<EntityPrivileges>();
            var repository = new RepositoryUsers();
            if (UserId.HasValue)
            {
                response = repository.GetPrivilegeById(UserId.Value, CurrentBusinessId.Value);
            }
            else
            {
                response.Model = new EntityPrivileges();
            }
            return Ok<DataResponse>(response);
        }
        [HttpPost]
        [Route("setprivileges/{UserId}")]
        [Route("{UserId}/setprivileges")]
        public IHttpActionResult SetPrivileges(int? UserId, List<EBP.Business.Entity.Privileges.Privileges> entity)
        {
            var response = new DataResponse<VMPrivilege>();
            var repository = new RepositoryUsers();
            response = repository.SetPrivileges(entity, UserId.Value, CurrentBusinessId.Value);
            if (response.Status == DataResponseStatus.OK)
            {
                //Privilege mail
                if (response.Model != null)
                {
                    //var RootPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                    //var UserPrivileges = string.Join("<br/>", response.Model.UserPrivileges);
                    //var mail = new GMEmail();
                    //var content = response.Model.UserPrivileges.Count() > 1 ? "New privileges added by" + CurrentUser.FirstName : "New privilege added by" + CurrentUser.FirstName;
                    //string emailBody = TemplateManager.AddPrivilegestoUser(RootPath, content, response.Model.UserName, UserPrivileges);
                    //mail.SendDynamicHTMLEmail(response.Model.Email, "New Privilege Added", emailBody, "", "");
                }
            }
            return Ok<DataResponse>(response);
        }
        [HttpPost]
        [Route("ToggleStatus/{id}")]
        [Route("{id}/ToggleStatus")]
        public IHttpActionResult ToggleStatus(int id)
        {
            var repository = new RepositoryUsers();
            var response = repository.ToggleStatus(id);
            return Ok<DataResponse>(response);
        }

        [HttpPost]
        [Route("setpassword/{UserId}")]
        public async Task<IHttpActionResult> SetPassword(int UserId, SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var RemovePassword = await UserManager.RemovePasswordAsync(model.UserId);
                var result = await UserManager.AddPasswordAsync(model.UserId, model.NewPassword);
                if (result.Succeeded)
                {
                    return Ok<dynamic>(new { Status = HttpStatusCode.OK });
                }
                else
                {
                    return Ok<dynamic>(new { Status = HttpStatusCode.BadRequest });
                }
            }
            else
            {
                return Ok<dynamic>(new { Status = HttpStatusCode.BadRequest });
            }
        }
        [HttpPost]
        [Route("Save")]
        public async Task<IHttpActionResult> InsertUserData(BusinessUserModels model)
        {
            var response = new DataResponse<EntityUsers>();

            if (model.RoleIds.Count == 0)
            {
                return Ok<dynamic>(new { Status = HttpStatusCode.BadRequest, Model = "Plese select at least one role." });
            }
            if (ModelState.IsValid)
            {
                if (model.Id > 0)
                {
                    var user = await UserManager.FindByIdAsync(model.Id);
                    if (user == null)
                    {
                        return Ok<dynamic>(new { Status = HttpStatusCode.BadRequest });
                    }

                    user.FirstName = model.FirstName;
                    user.MiddleName = model.MiddleName;
                    user.LastName = model.LastName;
                    user.PhoneNumber = model.PhoneNumber;
                    user.Email = model.Email;
                    var userUpdate = UserManager.Update(user);
                    if (!userUpdate.Succeeded)
                    {
                        user.ClearCache(user.UserName);
                        return Ok<dynamic>(new { Status = HttpStatusCode.BadRequest, Model = userUpdate.Errors.First() });
                    }
                    IdentityResult result = null;

                    IdentityResult removerole = await UserManager.RemoveFromRoleAsync(user.Id, string.Empty);

                    foreach (var roleId in model.RoleIds)
                    {
                        result = await UserManager.AddToRoleAsync(user.Id, roleId);
                    }
                    var entity = new EntityProfile
                    {
                        UserId = user.Id,
                        WorkEmail = model.WorkEmail,
                        HomePhone = model.HomePhone,
                        AdditionalPhone = model.AdditionalPhone,
                        AddressLine1 = model.AddressLine1,
                        AddressLine2 = model.AddressLine2,
                        City = model.City,
                        Zip = model.Zip,
                        StateId = model.StateId,
                        CreatedBy = CurrentUserId,
                        UpdatedBy = CurrentUserId,
                        Startdate = string.IsNullOrEmpty(model.StartDate) ? (DateTime?)null : DateTime.ParseExact(model.StartDate, "MM-dd-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None),
                        RoleIds = model.RoleIds,
                        DepartmentIds = model.DepartmentIds
                    };
                    response = new RepositoryUsers().UpdateUserProfile(entity);
                }
                else
                {
                    model.Password = model.ConfirmPassword = GeneralHelpers.GeneratePassword(3, 2, 2);
                    if (ModelState.IsValid)
                    {
                        var user = new GMUser { FirstName = model.FirstName, MiddleName = model.MiddleName, LastName = model.LastName, PhoneNumber = model.PhoneNumber, UserName = model.Email, Email = model.Email, BusinessId = CurrentBusinessId, IsActive = true };
                        var adminresult = await UserManager.CreateAsync(user, model.Password);

                        if (adminresult.Succeeded)
                        {
                            var entity = new EntityProfile
                            {
                                UserId = user.Id,
                                WorkEmail = model.WorkEmail,
                                HomePhone = model.HomePhone,
                                AdditionalPhone = model.AdditionalPhone,
                                AddressLine1 = model.AddressLine1,
                                AddressLine2 = model.AddressLine2,
                                City = model.City,
                                Zip = model.Zip,
                                StateId = model.StateId,
                                CreatedBy = CurrentUserId,
                                UpdatedBy = CurrentUserId,
                                Startdate = string.IsNullOrEmpty(model.StartDate) ? (DateTime?)null : DateTime.ParseExact(model.StartDate, "MM-dd-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None),
                                RoleIds = model.RoleIds,
                                DepartmentIds = model.DepartmentIds
                            };
                            response = new RepositoryUsers().insertUserProfile(entity);
                            if (model.RoleIds != null)
                            {
                                IdentityResult result = null;
                                foreach (var roleId in model.RoleIds)
                                {
                                    result = await UserManager.AddToRoleAsync(user.Id, roleId.ToString());
                                }
                            }
                            //var RootPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                            //var ReturnUrl = ConfigurationManager.AppSettings["BaseUrl"] + CurrentUser.RelativeUrl.Replace(" ", "-");
                            //var UserDepartments = string.Join("<br/>", response.Model.UserDepartments);
                            ////Email to user
                            //var mail = new GMEmail();
                            //string emailBody = TemplateManager.NewUser(RootPath, CurrentUser.FirstName, user.FirstName, CurrentUser.BusinessName, model.Email, model.Password, UserDepartments, ReturnUrl);
                            //mail.SendDynamicHTMLEmail(model.Email, "New user registration", emailBody, "", "");
                        }
                        else
                        {
                            return Ok<dynamic>(new { Status = HttpStatusCode.BadRequest, Model = adminresult.Errors.First() });
                        }
                    }

                }
                return Ok<DataResponse>(response);
            }
            else
            {
                var errorList = ModelState.Where(a => a.Value.Errors.Any()).Select(s => new
                {
                    Key = s.Key.Split('.').Last(),
                    Message = s.Value.Errors[0].ErrorMessage
                });
                return Ok<dynamic>(new { Status = HttpStatusCode.BadRequest, Model = errorList });
            }
        }
        [HttpPost]
        [Route("delete/{userid}")]
        [Route("{userid}/delete")]
        public IHttpActionResult Delete(int userid)
        {
            var repository = new RepositoryUsers();
            var response = repository.Delete(userid);
            return Ok<DataResponse>(response);
        }
    }
}