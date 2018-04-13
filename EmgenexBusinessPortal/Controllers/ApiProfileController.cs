using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EmgenexBusinessPortal.Models;
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace EmgenexBusinessPortal.Controllers
{
    [ApiAuthorizeAttribute]
    public class ProfileController : ApiBaseController
    {
        public IHttpActionResult Get()
        {
            var tokenResponse = new JObject(
                new JProperty("FullName", CurrentUser.FullName),
                new JProperty("UserName", CurrentUser.UserName),
                new JProperty("FirstName", CurrentUser.FirstName),
                new JProperty("MiddleName", CurrentUser.MiddleName),
                new JProperty("LastName", CurrentUser.LastName),
                new JProperty("PhoneNumber", CurrentUser.PhoneNumber),
                new JProperty("FilePath", CurrentUser.FilePath),
                new JProperty("Business", CurrentUser.BusinessName),
                new JProperty("DomainUrl", CurrentUser.DomainUrl),
                new JProperty("IsRep", CurrentUser.IsRep),
                new JProperty("RelativeUrl", CurrentUser.RelativeUrl),
                new JProperty("UserPrivilages", CurrentUser.UserPrivileges),
                new JProperty("UserRoles", CurrentUser.Roles),
                new JProperty("UserDepartments", CurrentUser.Departments),
                new JProperty("DefaultDateRange", CurrentUser.DefaultDateRange),
                new JProperty("SalesGroupBy", CurrentUser.SalesGroupBy),
                new JProperty("IsProfileFilled", !string.IsNullOrEmpty(CurrentUser.Email)));

            return Ok<dynamic>(new { IsSuccess = 1, Status = 200, Model = tokenResponse, Message = "Success!" });
        }
    }

    [ApiAuthorize]
    public class BusinessUserProfileController : ApiBaseController
    {
        public IHttpActionResult Get()
        {
            var model = new BusinessUserProfileModel()
            {
                FullName = CurrentUser.FullName.ToString(),
                BusinessName = CurrentUser.BusinessName,
                FilePath = CurrentUser.FilePath,
                Email = CurrentUser.Email,
                LogoUrl = CurrentUser.LogoUrl
            };

            return Ok<dynamic>(new { IsSuccess = 1, Status = 200, Model = model, Message = "Success!" });
        }
    }
}
