using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using EmgenexBusinessPortal.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using GM.Identity.Models;

namespace EmgenexBusinessPortal.Areas.BusinessSettings.Controllers
{
    public class ApiBaseController : ApiController
    {
        protected void CacheIt<T>(string name, T data)
        {
            HttpRuntime.Cache[name] = data;
        }

        protected T GetCached<T>(string name, T data)
        {
            var obj = HttpRuntime.Cache[name];
            if (obj != null)
                return (T)obj;
            else
                return default(T);
        }

        private GMUserManager _userManager;
        public GMUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<GMUserManager>();
            }
            protected set
            {
                _userManager = value;
            }
        }

        protected int CurrentUserId { get { return CurrentUser.Id; } }

        protected int? CurrentBusinessId { get { return CurrentUser.BusinessId; } }

        protected UserDetailsModel CurrentUser
        {
            get
            {
                var model = HttpContext.Current.Items["CurrentUser"] as UserDetailsModel;
                return model;
            }
        }

        protected string[] CurrentUserRoles
        {
            get
            {
                return CurrentUser.Roles;
            }
        }

        protected string[] CurrentUserDepartments
        {
            get
            {
                return CurrentUser.Departments;
            }
        }

        protected string[] CurrentUserPrivileges
        {
            get
            {
                return CurrentUser.UserPrivileges;
            }
        }

        protected bool IsRep
        {
            get
            {
                return CurrentUser.IsSalesRep();
            }
        }

        protected bool IsSalesManager
        {
            get
            {
                return CurrentUser.IsSalesManager() && !IsBuzAdmin;
            }
        }

        protected bool IsSalesDirector
        {
            get
            {
                return CurrentUser.IsSalesDirector();
            }
        }

        protected bool IsBuzAdmin
        {
            get
            {
                return CurrentUser.Roles.Contains("BusinessAdmin");
            }
        }

        public bool HasRight(string[] roles = null)
        {
            if (CurrentUser.Roles.Contains("BusinessAdmin"))
                return true;
            return CurrentUser.UserPrivileges.Any(a => roles != null && roles.Any(b => b == a));
        }


    }
}