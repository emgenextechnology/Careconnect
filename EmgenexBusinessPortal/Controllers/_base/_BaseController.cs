
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using GM.Identity.Models;

namespace EmgenexBusinessPortal.Controllers
{
    public class BusinessModel
    {
        // GET: Admin/BusinessModel
        public int BusinessId { get; set; }
        public string BusinessName { get; set; }
    }

    public class BaseController : Controller
    {
       
        public BaseController()
        {

        }

        public BaseController(GMUserManager userManager, GMSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
        public int CurrentUserId
        {
            get
            {
                return User.Identity.GetUserId<int>();
            }
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.bizId = RouteData.Values["businessId"];
            ViewBag.CurrentUserName = "Guest";

            

            base.OnActionExecuting(filterContext);
        }

        private GMUserManager _userManager;
        public GMUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<GMUserManager>();
            }
            protected set
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
            protected set
            {
                _roleManager = value;
            }
        }

        private GMSignInManager _signInManager;

        public GMSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<GMSignInManager>();
            }
            protected set { _signInManager = value; }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }
    }
}