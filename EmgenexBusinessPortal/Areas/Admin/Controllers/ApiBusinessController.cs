using EBP.Business;
using EBP.Business.Database;
using EBP.Business.Helpers;
using EmgenexBusinessPortal.Areas.Admin.Controllers._base;
using EmgenexBusinessPortal.Areas.Admin.Models;
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

namespace EmgenexBusinessPortal.Areas.Admin.Controllers
{
    //[RoutePrefix("api/admin/business")]
    public class ApiBusinessController : ApiController
    {
        private CareConnectCrmEntities db = new CareConnectCrmEntities();
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


        //[HttpPost]
        [HttpGet]
        public IHttpActionResult GetByFilter()
        {
            FilterTask filter = new FilterTask();
            var repository = new RepositoryBusinessProfiles();
            var response = repository.GetAllList("ddd",filter);
            return Ok<DataResponse<EntityList<EntityBusinessProfile>>>(response);
        }


        //[Route("save")]     
        //public async Task<IHttpActionResult> GetLeadById(BusinessModel model)
        //{
        //    var response = new DataResponse<BusinessModel>();
        //    var userManager = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<GMUserManager>();
        //    var roleManager = System.Web.HttpContext.Current.GetOwinContext().Get<GMRoleManager>();
        //    if (ModelState.IsValid)
        //    {
        //        //if (db.BusinessMasters.Any(a => a.BusinessName.ToLower() == model.BusinessName.ToLower()))
        //        //{
        //        //    ModelState.AddModelError("Error", string.Format("{0} is already used.", model.BusinessName));
        //        //    return View(model);
        //        //}

        //        model.Password = GeneralHelpers.GeneratePassword(3, 2, 2);
        //        var user = new GMUser
        //        {
        //            FirstName = model.FirstName,
        //            MiddleName = model.MiddleName,
        //            LastName = model.LastName,
        //            PhoneNumber = model.PhoneNumber,
        //            UserName = model.Email,
        //            Email = model.Email,
        //            IsActive = true,
        //        };
        //        IdentityResult result = await UserManager.CreateAsync(user, model.Password);

        //        if (result.Succeeded)
        //        {
        //            string ReturnUrl = ConfigurationManager.AppSettings["BaseUrl"];
        //            //ViewData.Model = new EmgenexBusinessPortal.Models.EmailModel.BusinessEmailModel
        //            //{
        //            //    CurentUserName = CurrentUser.FirstName,
        //            //    FirstName = model.FirstName,
        //            //    BusinessName = model.BusinessName,
        //            //    UserName = model.Email,
        //            //    Password = model.Password,
        //            //    ReturnUrl = ReturnUrl + model.BusinessName.Replace(" ", "-")
        //            //};
        //            var roleName = "BusinessAdmin";
        //            var role = roleManager.FindByName(roleName);
        //            if (role == null)
        //            {
        //                db.Roles.Add(new Role { Name = roleName, Description = roleName, IsActive = true, CreatedBy = CurrentUser.Id, CreatedOn = System.DateTime.UtcNow });
        //                db.SaveChanges();
        //            }
        //            UserManager.AddToRolesAsync(user.Id, roleName);
        //            //var emailBody = RazorHelper.RenderRazorViewToString("~/Views/Shared/Email/AddNewBusiness.cshtml",);
        //            //UserManager.SendEmailAsync(user.Id, "New Business", emailBody);
        //        }
              
        //        var Business = db.BusinessMasters.Add(new BusinessMaster
        //        {
        //            BusinessName = model.BusinessName,
        //            RelativeUrl = model.BusinessName.ToLower().Replace(" ", "-"),
        //            Description = model.Description,
        //            CreatedBy = CurrentUser.Id,
        //            CreatedOn = System.DateTime.UtcNow,
        //            IsActive = true,
        //            Status = 1
        //        });
        //        if (db.SaveChanges() > 0)
        //        {
        //            var userDetails = db.Users.FirstOrDefault(a => a.Id == user.Id);
        //            userDetails.UserProfiles.Add(new UserProfile { CreatedBy = CurrentUser.Id, CreatedOn = DateTime.UtcNow, UserId = userDetails.Id });
        //            userDetails.BusinessId = Business.Id;
        //            db.SaveChanges();
        //            //string rootPath = ConfigurationManager.AppSettings["FolderPath"];
        //            string rootPath = HttpContext.Current.Server.MapPath("~/Assets");
        //            string path = Path.Combine(rootPath, Business.Id.ToString());
        //            DirectoryInfo dir = new DirectoryInfo(path);
        //            if (!dir.Exists)
        //                dir.Create();
        //        }
               
        //    }

        //    return Ok<DataResponse>(response);
        //}





    }
}
