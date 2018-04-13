using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using EBP.Business.Database;
using EmgenexBusinessPortal.Models;
using GM.Identity.Models;
using Microsoft.AspNet.Identity.Owin;
using EBP.Business.Repository;
using System.IO;
namespace EmgenexBusinessPortal.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {

        protected CareConnectCrmEntities db = new CareConnectCrmEntities();
        public CurrentUserDetails CurrentUser
        {
            get
            {
                var currentUserId = User.Identity.GetUserId<int>();
                var CurrentUser = db.Users.FirstOrDefault(a => a.Id == currentUserId);
                var UserPrivileges = new RepositoryUserProfile().GetUserPrivilages(currentUserId);
                // var UserPrivilage = db.UserPrivileges.FirstOrDefault(a => a.UserId == currentUserId);
                var model = new CurrentUserDetails()
                {
                    Id = CurrentUser.Id,
                    FirstName = CurrentUser.FirstName,
                    BusinessId = CurrentUser.BusinessId,
                    Email = CurrentUser.Email,
                    PhoneNumber = CurrentUser.PhoneNumber,
                    UserName = CurrentUser.UserName,
                    UserPrivileges=UserPrivileges
                };
                //if (UserPrivilage != null)
                //{
                //    model.UserPrivilage = new UserPrivilage { PrivilegeId = UserPrivilage.PrivilegeId, PrivilegeName = UserPrivilage.Privilege.Title, IsDeny = UserPrivilage.IsDeny };
                //}
               
                return model;
            }
        }
        public UserPrivilege UserPrivilege
        {
            get
            {
                var currentUserId = User.Identity.GetUserId<int>();
                return db.UserPrivileges.FirstOrDefault(a => a.UserId == currentUserId);
            }
        }
        public class CurrentUserDetails
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public Nullable<int> BusinessId { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string UserName { get; set; }

            public string[] UserPrivileges { get; set; }
        }
        //protected string RenderRazorViewToString(string viewName, object model)
        //{
        //    ViewData.Model = model;
        //    using (var sw = new StringWriter())
        //    {
        //        new Exception(viewName).Log();

        //        var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
        //        var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
        //        viewResult.View.Render(viewContext, sw);
        //        viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
        //        return sw.GetStringBuilder().ToString();
        //    }
        //}
        //public class UserPrivilage
        //{
        //    public int PrivilegeId { get; set; }
        //    public Nullable<bool> IsDeny { get; set; }

        //    public string PrivilegeName { get; set; }
        //}

        //public CurrentUserModel CurrentUserdetails
        //{
        //    get
        //    {
        //        var model = new CurrentUserModel();
        //        var currentUserId = User.Identity.GetUserId<int>();
        //        model.UserDetails=db.Users.FirstOrDefault(a => a.Id == currentUserId);
        //        model.UserPrivilege = db.UserPrivileges.FirstOrDefault(a => a.UserId == currentUserId );
        //        return model;
        //    }
        //}

        //public class CurrentUserModel
        //{
        //    public User UserDetails { get; set; }
        //    public UserPrivilege UserPrivilege { get; set; }
        //}

    }

}