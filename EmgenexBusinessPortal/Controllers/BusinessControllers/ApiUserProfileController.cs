using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GM.Identity.Models;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using EBP.Business.Filter;
using EBP.Business.Repository;
using EBP.Business;
using EBP.Business.Entity;
using EBP.Business.Entity.Users;
using EBP.Business.Helpers;
using GM.Identity;
using System.Threading.Tasks;

using System.Globalization;
using EmgenexBusinessPortal.Models;
using EBP.Business.Entity.Privileges;
using EBP.Business.Database;
using System.Configuration;
using GM.Identity.Config;
using EBP.Business.Notifications;
using System.Text;

namespace EmgenexBusinessPortal.Controllers
{
    [ApiAuthorize]
    [RoutePrefix("api/users")]
    public class ApiUserProfileController : ApiBaseController
    {
        private GMUserManager _userManager;
        RepositoryUsers repository = new RepositoryUsers();
        public ApiUserProfileController()
        {
        }

        CareConnectCrmEntities DBEntity = new CareConnectCrmEntities();
        public ApiUserProfileController(GMUserManager userManager,
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

        [HttpPost]
        [Route("")]
        public IHttpActionResult GetByFilter(FilterUsers filter)
        {

            if (filter == null)
            {
                filter = new FilterUsers();
                filter.PageSize = 25;
                filter.CurrentPage = 1;
            }
            var response = repository.GetUsers(filter, CurrentBusinessId.Value);
            return Ok<DataResponse<EntityList<EntityUsers>>>(response);
        }

        [Route("{Id}")]
        public async Task<IHttpActionResult> GetUserById(int? Id)
        {
            var response = new DataResponse<EntityUsers>();

            if (Id.HasValue)
            {
                response = repository.GetUserById(Id.Value);
                var userRoles = await UserManager.GetRolesAsync(Id.Value);
                response.Model.UserRoles = DBEntity.Roles.Where(a => a.BusinessId == CurrentBusinessId.Value).Select(x => new SelectedItem()
                {
                    Selected = response.Model.RoleIds.Contains(x.Id),
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).OrderBy(a => a.Value).ToList();
                response.Model.UserRoleNames = response.Model.UserRoles.Where(a => a.Selected == true).Select(a => a.Text);

                var UserDepartment = DBEntity.UserDepartments.Where(a => a.UserId == response.Model.Id).Select(a => a.Department.DepartmentName);
                response.Model.UserDepartments = DBEntity.Departments.Where(a => a.BusinessId == CurrentBusinessId).Select(x => new SelectedItem()
                {
                    Selected = response.Model.DepartmentIds.Contains(x.Id),
                    Text = x.DepartmentName,
                    Value = x.Id.ToString()
                }).OrderBy(a => a.Value).ToList();
                response.Model.UserDepartmentName = response.Model.UserDepartments.Where(a => a.Selected == true).Select(a => a.Text);
            }
            else
            {
                response.Model = new EntityUsers();
            }
            return Ok<DataResponse>(response);
        }

        [HttpPost]
        [Route("Save")]
        public async Task<IHttpActionResult> InsertUserData(UserViewModel model)
        {
            var response = new DataResponse<EntityUsers>();

            try
            {
                if (model.RoleIds.Count == 0)
                {
                    return Ok<dynamic>(new { Status = HttpStatusCode.BadRequest, Model = "Plese select at least one role." });
                }

                model.Password = model.ConfirmPassword = GeneralHelpers.GeneratePassword(3, 2, 2);

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
                        DepartmentIds = model.DepartmentIds
                    };

                    if (model.RoleIds != null)
                    {
                        IdentityResult result = null;
                        foreach (var roleId in model.RoleIds)
                        {
                            result = await UserManager.AddToRoleAsync(user.Id, roleId.ToString());
                        }
                    }
                    response = repository.insertUserProfile(entity);

                    if (model.RepGroupId > 0)
                    {
                        new RepositoryReps().Insert(new EBP.Business.Entity.Rep.EntityReps { UserId = user.Id, RepGroupId = model.RepGroupId, CreatedBy = CurrentUserId });
                    }

                    try
                    {
                        var RootPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                        var ReturnUrl = ConfigurationManager.AppSettings["BaseUrl"] + CurrentUser.RelativeUrl.Replace(" ", "-");

                        string UserDepartments = string.Empty;
                        if (response.Model.UserDepartmentName != null && response.Model.UserDepartmentName.Count() > 0)
                            UserDepartments = string.Join("<br/>", response.Model.UserDepartmentName);
                        //Email to user
                        var mail = new GMEmail();
                        string emailBody = TemplateManager.NewUserCreate(RootPath, CurrentUser.BusinessName, user.UserName, model.Password, user.FirstName, UserDepartments, ReturnUrl);
                        mail.SendDynamicHTMLEmail(model.Email, "New user registration", emailBody, "", "");
                    }
                    catch (Exception ex)
                    {
                        ex.Log();
                    }
                }
                else
                {
                    return Ok<dynamic>(new { Status = HttpStatusCode.BadRequest, Model = adminresult.Errors.First() });
                }
                return Ok<DataResponse>(response);
            }
            catch (Exception ex)
            {
                ex.Log();
            }

            return Ok<DataResponse>(response);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> UpdateUserData(UserViewModel model)
        {
            var response = new DataResponse<EntityUsers>();

            if (model.RoleIds.Count == 0)
            {
                return Ok<dynamic>(new { Status = HttpStatusCode.BadRequest, Model = "Plese select at least one role." });
            }
            if (ModelState.IsValid)
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

                var entity = new EntityProfile
                {
                    Id = model.Id,
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
                    UpdatedOn = DateTime.UtcNow,
                    DepartmentIds = model.DepartmentIds
                };

                if (!string.IsNullOrEmpty(model.StartDate))
                {
                    entity.Startdate = DateTime.ParseExact(model.StartDate.ToString(), "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);
                }
                else
                {
                    entity.Startdate = null;
                }

                #region Role
                if (model.RoleIds != null)
                {

                    IdentityResult result = null;
                    IdentityResult removerole = await UserManager.RemoveFromRoleAsync(user.Id, string.Empty);
                    foreach (var roleId in model.RoleIds)
                    {
                        result = await UserManager.AddToRoleAsync(user.Id, roleId.ToString());
                    }
                }
                #endregion

                response = repository.UpdateUserProfile(entity);
                //var RootPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //var ReturnUrl = ConfigurationManager.AppSettings["BaseUrl"] + CurrentUser.RelativeUrl.Replace(" ", "-");
                //var UserDepartments = string.Join("<br/>", response.Model.UserDepartments);
                ////Email to user
                //var mail = new GMEmail();
                //string emailBody = TemplateManager.NewUser(RootPath, CurrentUser.FirstName, user.FirstName, CurrentUser.BusinessName, model.Email, model.Password, UserDepartments, ReturnUrl);
                //mail.SendDynamicHTMLEmail(model.Email, "New user registration", emailBody, "", "");

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

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(int id)
        {
            var response = repository.Delete(id);
            return Ok<DataResponse>(response);
        }

        [Route("{UserId}/privileges")]
        public IHttpActionResult GetAllPrivilegeByUser(int? UserId)
        {
            var response = new DataResponse<EntityPrivileges>();

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

        [HttpPut]
        [Route("{UserId}/setprivileges")]
        public IHttpActionResult SetPrivileges(int? UserId, List<Modulesmodels> model)
        {
            var response = new DataResponse<VMPrivilege>();
            List<Privileges> entity = new List<Privileges>();

            foreach (var module in model)
                foreach (var privilege in module.UserPrivileges)
                {
                    if (privilege.Id != 0)
                    {
                        entity.Add(new Privileges
                        {
                            UserId = UserId.Value,
                            Id = privilege.Id,
                            Name = privilege.Name,
                            Deny = privilege.Deny,
                            businessId = CurrentBusinessId
                        });
                    }
                }

            response = repository.SetPrivileges(entity, UserId.Value, CurrentBusinessId.Value);
            if (response.Status == DataResponseStatus.OK)
            {
                //Privilege mail
                if (response.Model != null)
                {
                    var RootPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                    var ReturnUrl = ConfigurationManager.AppSettings["BaseUrl"] + CurrentUser.RelativeUrl.Replace(" ", "-");

                    string Subject = string.Empty;
                    if (response.Model.UserPrivilegesList != null && response.Model.UserPrivileges.Count() > 0)
                        Subject = "Privileges";
                    else
                        Subject = "Privilege";
                    
                    //Building an HTML string.
                    StringBuilder html = new StringBuilder();
                    int index = 1;
                    foreach (var item in response.Model.UserPrivilegesList)
                    {
                        //UserDepartments = string.Join("<br/>", response.Model.UserDepartmentName);
                        html.Append("<tr><td style = border:1px solid #f0f0f0;border-width:1px 0;padding:11px 0;vertical-align:baseline>" +
                            "<div style = margin: 0 12px 0 0;border :2px solid #e6e6e6;border-radius :10px 31px;padding :7px 12px;background :#f5f5f5;>");
                        html.Append(index);
                        html.Append("</div>" +
                                    "</td>" +
                                    "<td style = border:1px solid #f0f0f0;border-width:1px 0;padding:11px 0;width:100%>" +
                                    "<div style =font-family:Roboto-Regular,Helvetica,Arial,sans-serif;font-size:13px;color:#202020;line-height:1.5;font-weight:bold>");
                        html.Append(item.Title);
                        html.Append("</div>" +
                                    "<div style = font-family:Roboto-Regular,Helvetica,Arial,sans-serif;font-size:13px;color:#202020;line-height:1.5>");
                        html.Append(item.Description);
                        html.Append("</div>" +
                                    "</td>" +
                                   " </tr>");
                        index++;
                    }

                    string AssignedUserName = response.Model.UserName;
                    var mail = new GMEmail();

                    string emailBody = TemplateManager.AddPrivilegesToUser(RootPath, Subject,AssignedUserName,html.ToString(),CurrentUser.FirstName,ReturnUrl);
                    mail.SendDynamicHTMLEmail(response.Model.Email, "New Privilege Added", emailBody, "", "");
                }
            }
            return Ok<DataResponse>(response);
        }

        [HttpPost]
        [Route("{userid}/togglestatus")]
        public IHttpActionResult ToggleStatus(int userid)
        {
            var response = repository.ToggleStatus(userid);
            return Ok<DataResponse>(response);
        }

        [HttpPost]
        [Route("{userid}/setpassword")]
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
    }
}
