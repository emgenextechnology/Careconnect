using EBP.Business;
using EBP.Business.Database;
using EBP.Business.Helpers;
//using EmgenexBusinessPortal.Areas.Admin.Controllers._base;
//using EmgenexBusinessPortal.Areas.Admin.Models;
using GM.Identity;
using GM.Identity.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using EmgenexBusinessPortal.Helpers;
using EBP.Business.Entity;
using EBP.Business.Repository;
using EBP.Business.Filter;
using EBP.Business.Entity.Business;
using EmgenexBusinessPortal.Models;
using System.Web.Http.Controllers;
using EBP.Business.Notifications;
using GM.Identity.Config;

namespace EmgenexBusinessPortal.Controllers
{
    [RoutePrefix("api/business")]
    [ApiAuthorize]
    public class ApiBusinessController : ApiBaseController
    {
        private CareConnectCrmEntities db = new CareConnectCrmEntities();
        private RepositoryBusinessProfiles repository = new RepositoryBusinessProfiles();
        private GMUserManager _userManager;
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

        [HttpPost]
        [Route("")]
        public IHttpActionResult GetByFilter(FilterBusiness filter)
        {
            if (filter == null)
            {
                filter = new FilterBusiness();
                filter.PageSize = 25;
                filter.CurrentPage = 1;
            }

            var response = repository.GetAllBusinesses(filter);
            return Ok<DataResponse<EntityList<EntityBusinessMaster>>>(response);
        }

        [HttpPost]
        [Route("save")]
        public async Task<IHttpActionResult> CreateBusiness(BusinessMasterModel model)
        {
            var response = new DataResponse<EntityBusinessMaster>();

            var userManager = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<GMUserManager>();
            var roleManager = System.Web.HttpContext.Current.GetOwinContext().Get<GMRoleManager>();
            if (ModelState.IsValid)
            {
                if (db.BusinessMasters.Any(a => a.BusinessName.ToLower() == model.BusinessName.ToLower()))
                {
                    var error = string.Format("{0} is already used.", model.BusinessName);
                    return Ok<dynamic>(new { Status = HttpStatusCode.BadRequest, Model = error });
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
                IdentityResult result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {                    
                    var roleName = "BusinessAdmin";
                    var role = roleManager.FindByName(roleName);
                    if (role == null)
                    {
                        db.Roles.Add(new Role { Name = roleName, Description = roleName, IsActive = true, CreatedBy = 1, CreatedOn = System.DateTime.UtcNow });
                        db.SaveChanges();
                    }
                    UserManager.AddToRolesAsync(user.Id, roleName);                 
                }
                else
                {
                    var error = string.Format("{0} is already used.", model.Email);
                    return Ok<dynamic>(new { Status = HttpStatusCode.BadRequest, Model = error });
                }

                var entityBusinessProfile = new EntityBusinessMaster();
                entityBusinessProfile.BusinessName = model.BusinessName;
                entityBusinessProfile.RelativeUrl = model.BusinessName.ToLower().Replace(" ", "-");
                entityBusinessProfile.Description = model.Description;
                entityBusinessProfile.CreatedBy = 1;
                response = repository.Insert(entityBusinessProfile);

                if (response.Model.Id>0)
                {
                    var userDetails = db.Users.FirstOrDefault(a => a.Id == user.Id);
                    userDetails.UserProfiles.Add(new UserProfile { CreatedBy = 1, CreatedOn = DateTime.UtcNow, UserId = userDetails.Id });
                    userDetails.BusinessId = response.Model.Id;
                    db.SaveChanges();

                    model.RootPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                    string ReturnUrl = ConfigurationManager.AppSettings["BaseUrl"];
                    ReturnUrl = ReturnUrl + model.BusinessName.Replace(" ", "-");
                    string RelativeUrl = model.BusinessName.ToLower().Replace(" ", "-");

                    string managerEmailBody = string.Empty;
                    var mail = new GMEmail();
                    managerEmailBody = TemplateManager.NewBusinessCreate(model.RootPath, model.BusinessName, user.UserName, model.Password, model.FirstName, ReturnUrl, response.Model.Id, RelativeUrl);
                    mail.SendDynamicHTMLEmail(model.Email, "New Business Created", managerEmailBody, CurrentUser.OtherEmails);

                    //string rootPath = ConfigurationManager.AppSettings["FolderPath"];
                    string rootPath = HttpContext.Current.Server.MapPath("~/Assets");
                    string path = Path.Combine(rootPath, response.Model.Id.ToString());
                    DirectoryInfo dir = new DirectoryInfo(path);
                    if (!dir.Exists)
                        dir.Create();
                }
            }

            return Ok<DataResponse>(response);
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult UpdateBusiness(int id, BusinessMasterModel model)
        {
            var response = new DataResponse<EntityBusinessMaster>();

            if (ModelState.IsValid)
            {
                var entityBusinessProfile = new EntityBusinessMaster();
                entityBusinessProfile.Id = id;
                entityBusinessProfile.BusinessName = model.BusinessName;
                entityBusinessProfile.Description = model.Description;
                entityBusinessProfile.UpdatedBy = 1;
                entityBusinessProfile.IsActive = model.IsActive;
                entityBusinessProfile.UpdatedOn = System.DateTime.UtcNow;
                response = repository.UpdateBusiness(entityBusinessProfile);
            }

            return Ok<DataResponse>(response);
        }

        [Route("{id}")]
        public IHttpActionResult GetBusinessById(int id)
        {

            var response = new DataResponse<EntityBusinessMaster>();
            if (id != 0)
            {
                response = repository.GetBusinessById(id);

                return Ok<DataResponse>(response);
            }
            else
            {
                return null;
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeleteBusinessById(int id)
        {
            var response = new DataResponse<bool>();
            response = repository.DeleteBusiness(id);
            return Ok<DataResponse>(response);
        }
    }
}