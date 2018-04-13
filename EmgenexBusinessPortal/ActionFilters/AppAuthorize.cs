
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using EBP.Business.Repository;
using EBP.Business.Database;
using EmgenexBusinessPortal.Areas.Business.Models;
using EmgenexBusinessPortal.Models;
using System.Web.Http;
using System.Net.Http;
using System.Net;

namespace EmgenexBusinessPortal
{
    public class BusinessAuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized)
            {
                return false;
            }

            UserDetailsModel userModel = null;

            if (HttpRuntime.Cache[httpContext.User.Identity.Name] != null)
            {
                userModel = (UserDetailsModel)HttpRuntime.Cache[httpContext.User.Identity.Name];
            }
            else
            {

                var db = new CareConnectCrmEntities();
                // var currentUserId = httpContext.User.Identity.GetUserId<int>();

                userModel = db.Users.Where(a => a.UserName == httpContext.User.Identity.Name).Select(currentUser => new UserDetailsModel
                {
                    Id = currentUser.Id,
                    FirstName = currentUser.FirstName,
                    MiddleName = currentUser.MiddleName,
                    LastName = currentUser.LastName,
                    BusinessId = currentUser.BusinessId,
                    Email = currentUser.Email,
                    PhoneNumber = currentUser.PhoneNumber,
                    UserName = currentUser.UserName,
                    BusinessName = currentUser.BusinessMaster.BusinessName,
                    RelativeUrl = currentUser.BusinessMaster.RelativeUrl,
                    IRoles = currentUser.Roles,
                    IDepartments = currentUser.UserDepartments,
                    OtherEmails = currentUser.BusinessMaster.OtherEmails,
                    LogoUrl = currentUser.BusinessMaster.Logo
                }).FirstOrDefault();

                var profileRepository = new RepositoryUserProfile();

                if (userModel.BusinessId == null || userModel.BusinessId == 0)
                {
                    httpContext.Items["CurrentUser"] = userModel;
                    httpContext.Items["CurrentUserName"] = userModel.FirstName + " " + userModel.LastName;
                    httpContext.Items["CurrentBusinessName"] = userModel.BusinessName.Replace(" ", "-");
                    return false;
                }

                userModel.UserPrivileges = profileRepository.GetUserPrivilages(userModel.Id);

                HttpRuntime.Cache[httpContext.User.Identity.Name] = userModel;

            }
            if (!userModel.Roles.Contains("BusinessAdmin"))
            {
                if (!userModel.UserPrivileges.Contains("RDSETTINGS"))
                    return false;
            }

            httpContext.Items["CurrentUser"] = userModel;
            httpContext.Items["CurrentUserName"] = userModel.FirstName + " " + userModel.LastName;
            httpContext.Items["CurrentBusinessName"] = userModel.BusinessName.Replace(" ", "-");

            return true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                var userModel = filterContext.HttpContext.Items["CurrentUser"] as CurrentUserDetails;
                if (userModel == null)
                {
                    HttpContext.Current.Response.Redirect("~/account/logout");
                }
                if (userModel != null && ((userModel.Roles != null && userModel.Roles.Count() > 0 && userModel.Roles.Contains("MasterAdmin")) || userModel.UserPrivileges.Contains("RDSETTINGS")))
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(
                            new
                            {
                                controller = "AdminDashboard",
                                action = "Index",
                                area = "Admin"
                            })
                        );
                }
                else if ((userModel.Roles != null && userModel.Roles.Count() == 0)
                    && (userModel.Departments != null && userModel.Departments.Count() == 0)
                    && (userModel.UserPrivileges != null && userModel.UserPrivileges.Count() == 0))
                {
                    HttpContext.Current.Response.Redirect("account/logout");
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(
                            new
                            {
                                controller = "Home",
                                action = "Index",
                                area = ""
                            }));
                }
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new
                        {
                            controller = "Home",
                            action = "Index",
                            area = ""
                        }));
            }
        }
    }

    public class ApiAuthorizeAttribute : System.Web.Http.AuthorizeAttribute
    {
        public ApiAuthorizeAttribute()
        {

        }

        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            var httpContext = HttpContext.Current;

            if (!httpContext.User.Identity.IsAuthenticated)
            {
                throw new HttpException(403, "Access Denied");
            }

            var userName = httpContext.User.Identity.Name;

            //if (actionContext.Request.Headers.GetValues("User") != null)
            //{
            //    var userNameHeader = actionContext.Request.Headers.GetValues("User");
            //    userName = userNameHeader.FirstOrDefault();
            //    var genericPrincipal = new System.Security.Principal.GenericPrincipal(new System.Security.Principal.GenericIdentity(userName), null);
            //    actionContext.RequestContext.Principal = genericPrincipal;
            //}

            UserDetailsModel userModel = null;

            if (HttpRuntime.Cache[httpContext.User.Identity.Name] != null)
            {
                userModel = (UserDetailsModel)HttpRuntime.Cache[httpContext.User.Identity.Name];
            }
            else
            {
                var db = new CareConnectCrmEntities();
                // var currentUserId = httpContext.User.Identity.GetUserId<int>();
                var query = db.Users.Where(a => a.UserName == userName);
                if (userName == "" && httpContext.User.Identity.IsAuthenticated)
                {
                    query = db.Users.Where(a => a.UserName == httpContext.User.Identity.Name);
                }

                userModel = query
                    .Select(currentUser => new UserDetailsModel
                    {
                        Id = currentUser.Id,
                        FirstName = currentUser.FirstName,
                        MiddleName = currentUser.MiddleName,
                        LastName = currentUser.LastName,
                        BusinessId = currentUser.BusinessId,
                        Email = currentUser.Email,
                        PhoneNumber = currentUser.PhoneNumber,
                        UserName = currentUser.UserName,
                        BusinessName = currentUser.BusinessMaster.BusinessName,
                        DomainUrl = currentUser.BusinessMaster.DomainUrl,
                        RelativeUrl = currentUser.BusinessMaster.RelativeUrl,
                        OtherEmails = currentUser.BusinessMaster.OtherEmails,
                        IsRep = currentUser.Reps2.Any(),
                        IsSalesManager = currentUser.RepgroupManagerMappers.Any(),
                        DefaultDateRange = currentUser.BusinessMaster.DateRange,
                        IRoles = currentUser.Roles,
                        IDepartments = currentUser.UserDepartments,
                        SalesGroupBy = currentUser.BusinessMaster.SalesGroupBy,
                        LogoUrl = currentUser.BusinessMaster.Logo
                    }).FirstOrDefault();

                var profileRepository = new RepositoryUserProfile();

                userModel.UserPrivileges = profileRepository.GetUserPrivilages(userModel.Id);

                HttpRuntime.Cache[httpContext.User.Identity.Name] = userModel;
            }
            if (userModel.BusinessId == null || userModel.BusinessId == 0)
            {
                httpContext.Items["CurrentUser"] = userModel;
                httpContext.Items["CurrentBusinessName"] = userModel.BusinessName;
                return;
            }

            httpContext.Items["CurrentUser"] = userModel;
            httpContext.Items["CurrentBusinessName"] = userModel.BusinessName;
        }

        bool SkipAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            return actionContext.Request.Headers.GetValues("User") != null;
        }

        protected override bool IsAuthorized(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            return true;

            #region Unreachable Code Block

            var userName = "";
            if (actionContext.Request.Headers.GetValues("User") != null)
            {

                var userNameHeader = actionContext.Request.Headers.GetValues("User");
                userName = userNameHeader.FirstOrDefault();
            }
            else
            {
                var isAuthorized = base.IsAuthorized(actionContext);

                if (!isAuthorized)
                {

                    //if (actionContext.Request.Headers.GetValues("User") != null)
                    //{
                    //   var userNameHeader = actionContext.Request.Headers.GetValues("User");
                    //   userName = userNameHeader.FirstOrDefault();

                    //TODO: //add additional security validation cases
                    //    goto userLoggedIn;
                    //}

                    return false;
                }
            }
            userLoggedIn:


            var httpContext = HttpContext.Current;

            if (httpContext.Items["CurrentUser"] != null)
            {

            }

            var db = new CareConnectCrmEntities();
            // var currentUserId = httpContext.User.Identity.GetUserId<int>();
            var query = db.Users.Where(a => a.UserName == userName);
            if (userName == "")
            {
                query = db.Users.Where(a => a.UserName == httpContext.User.Identity.Name);
            }

            var userModel = query
                .Select(currentUser => new UserDetailsModel
                {
                    Id = currentUser.Id,
                    FirstName = currentUser.FirstName,
                    MiddleName = currentUser.MiddleName,
                    LastName = currentUser.LastName,
                    BusinessId = currentUser.BusinessId,
                    Email = currentUser.Email,
                    PhoneNumber = currentUser.PhoneNumber,
                    UserName = currentUser.UserName,
                    BusinessName = currentUser.BusinessMaster.BusinessName,
                    RelativeUrl = currentUser.BusinessMaster.RelativeUrl,
                    OtherEmails = currentUser.BusinessMaster.OtherEmails,
                    IRoles = currentUser.Roles,
                    IDepartments = currentUser.UserDepartments
                }).FirstOrDefault();

            var profileRepository = new RepositoryUserProfile();

            //userModel.Roles = profileRepository.GetAllUserRoles(userModel.Id).Model.ToNameArray(); //new string[] { "MasterAdmin", "SuperAdmin" };
            //userModel.Departments = profileRepository.GetAllUserDepartments(userModel.Id).Model.ToNameArray();

            if (userModel.BusinessId == null || userModel.BusinessId == 0)
            {
                httpContext.Items["CurrentUser"] = userModel;
                httpContext.Items["CurrentBusinessName"] = userModel.BusinessName;
                return false;
            }

            userModel.UserPrivileges = profileRepository.GetUserPrivilages(userModel.Id);
            httpContext.Items["CurrentUser"] = userModel;
            httpContext.Items["CurrentBusinessName"] = userModel.BusinessName;

            return true;

            #endregion
        }

        protected override void HandleUnauthorizedRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            base.HandleUnauthorizedRequest(actionContext);
        }
    }

    public class ApiAuthorizeAttribute1 : System.Web.Mvc.AuthorizeAttribute
    {
        public ApiAuthorizeAttribute1()
        {

        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
        }

        public override bool Match(object obj)
        {
            return base.Match(obj);
        }

        protected override HttpValidationStatus OnCacheAuthorization(HttpContextBase httpContext)
        {
            return base.OnCacheAuthorization(httpContext);
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized)
            {
                return false;
            }
            UserDetailsModel userModel = null;
            if (httpContext.Session["currentUserModel"] != null)
            {
                userModel = (UserDetailsModel)httpContext.Session["currentUserModel"];
            }
            else
            {
                var db = new CareConnectCrmEntities();
                var currentUserId = httpContext.User.Identity.GetUserId<int>();

                var query = db.Users.Where(a => a.Id == currentUserId);

                userModel = query
                    .Select(currentUser => new UserDetailsModel
                    {
                        Id = currentUser.Id,
                        FirstName = currentUser.FirstName,
                        MiddleName = currentUser.MiddleName,
                        LastName = currentUser.LastName,
                        BusinessId = currentUser.BusinessId,
                        Email = currentUser.Email,
                        PhoneNumber = currentUser.PhoneNumber,
                        UserName = currentUser.UserName,
                        BusinessName = currentUser.BusinessMaster.BusinessName,
                        RelativeUrl = currentUser.BusinessMaster.RelativeUrl,
                        OtherEmails = currentUser.BusinessMaster.OtherEmails,
                        IRoles = currentUser.Roles,
                        IDepartments = currentUser.UserDepartments
                    }).FirstOrDefault();
                var profileRepository = new RepositoryUserProfile();

                userModel.UserPrivileges = profileRepository.GetUserPrivilages(userModel.Id);

                httpContext.Session["currentUserModel"] = userModel;
            }

            if (userModel.BusinessId == null || userModel.BusinessId == 0)
            {
                httpContext.Items["CurrentUser"] = userModel;
                httpContext.Items["CurrentBusinessName"] = userModel.BusinessName;
                return false;
            }

            httpContext.Items["CurrentUser"] = userModel;
            httpContext.Items["CurrentBusinessName"] = userModel.BusinessName;

            return true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new HttpUnauthorizedResult();
        }
    }
}