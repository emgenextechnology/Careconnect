using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using EBP.Api.Models;
using EBP.Business.Database;
using EBP.Business.Repository;
using System.Web.Http.Controllers;
using System.Security.Principal;

namespace EBP.Api
{
    public class ApiAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var httpContext = HttpContext.Current;
            var userName = "";
            if (actionContext.Request.Headers.Any(a => a.Key == "User") && actionContext.Request.Headers.GetValues("User") != null)
            {
                var userNameHeader = actionContext.Request.Headers.GetValues("User");
                userName = userNameHeader.FirstOrDefault();
                IPrincipal genericPrincipal = new GenericPrincipal(new GenericIdentity(userName), null);
                actionContext.RequestContext.Principal = genericPrincipal;
            }

            if (httpContext.User.Identity.Name == "")
                return;

            var db = new CareConnectCrmEntities();
            // var currentUserId = httpContext.User.Identity.GetUserId<int>();
            var query = db.Users.Where(a => a.UserName == httpContext.User.Identity.Name);
            if (userName == "" && httpContext.User.Identity.IsAuthenticated)
            {
                query = db.Users.Where(a => a.UserName == httpContext.User.Identity.Name);
            }

            var userModel = db.Users.Where(a => a.UserName == httpContext.User.Identity.Name).Select(currentUser => new UserDetailsModel
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
                IRoles = currentUser.Roles,
                IDepartments = currentUser.UserDepartments
            }).FirstOrDefault();

            if (userModel == null)
                return;

            var profileRepository = new RepositoryUserProfile();

            if (userModel.BusinessId == null || userModel.BusinessId == 0)
            {
                httpContext.Items["CurrentUser"] = userModel;
                httpContext.Items["CurrentBusinessName"] = userModel.BusinessName;
                return;
            }

            userModel.UserPrivileges = profileRepository.GetUserPrivilages(userModel.Id);

            httpContext.Items["CurrentUser"] = userModel;
            httpContext.Items["CurrentBusinessName"] = userModel.BusinessName;
        }

        bool SkipAuthorization(HttpActionContext actionContext)
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
                    LastName = currentUser.LastName,
                    BusinessId = currentUser.BusinessId,
                    Email = currentUser.Email,
                    PhoneNumber = currentUser.PhoneNumber,
                    UserName = currentUser.UserName,
                    BusinessName = currentUser.BusinessMaster.BusinessName,
                    IRoles = currentUser.Roles,
                    IsSalesManager = currentUser.RepgroupManagerMappers.Any(),
                    IDepartments = currentUser.UserDepartments
                }).FirstOrDefault();
            var profileRepository = new RepositoryUserProfile();

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

    }

}