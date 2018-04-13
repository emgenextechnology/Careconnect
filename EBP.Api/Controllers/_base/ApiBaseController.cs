using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using GM.Identity.Models;
using Microsoft.AspNet.Identity;
using EBP.Business.Database;
using EBP.Api.Models;
using System.Configuration;
namespace EBP.Api.Controllers
{
    public class ApiBaseController : ApiController
    {
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

        protected bool IsBuzAdmin
        {
            get
            {
                return CurrentUser.IsBuzAdmin();
            }
        }

        protected bool IsSalesManager
        {
            get
            {
                return CurrentUser.IsSalesManager();
            }
        }

        protected bool IsSalesDirector
        {
            get
            {
                return CurrentUser.IsSalesDirector();
            }
        }

        public bool HasRight(string[] roles = null)
        {
            if (CurrentUser.Roles.Contains("BusinessAdmin"))
                return true;
            return CurrentUser.UserPrivileges.Any(a => roles != null && roles.Any(b => b == a));
        }

        public bool HasSuperRight(string[] roles = null)
        {
            if (CurrentUser.Roles.Contains("BusinessAdmin"))
                return true;
            return CurrentUser.UserPrivileges.Any(a => roles != null && roles.Any(b => b == a));
        }
    }

}