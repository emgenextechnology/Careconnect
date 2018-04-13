using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using EBP.Business.Database;
using EBP.Business.Repository;
using EmgenexBusinessPortal.Areas.Business.Models;
using EmgenexBusinessPortal.Models;

namespace EmgenexBusinessPortal.Areas.Business.Controllers
{
    public class BaseController : Controller
    {

        protected CareConnectCrmEntities db = new CareConnectCrmEntities();

        public int CurrentUserId
        {
            get
            {
                return CurrentUser.Id;
            }
        }

        protected int? CurrentBusinessId { get { return CurrentUser.BusinessId; } }

        public UserDetailsModel CurrentUser
        {
            get
            {
                var model = HttpContext.Items["CurrentUser"] as UserDetailsModel;
                return model;
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