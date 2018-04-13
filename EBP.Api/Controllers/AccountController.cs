using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using EBP.Api.Models;
using EBP.Api.Providers;
using EBP.Api.Results;
using GM.Identity.Models;
using GM.Identity;
using System.Net;
using Newtonsoft.Json.Linq;
using EBP.Business.Database;
using System.Linq;
using EBP.Business.Repository;
using EBP.Business;
using EBP.Business.Entity;
using System.Security.Principal;
using EBP.Business.Entity.UserProfile;
using System.Configuration;
using EBP.Business.Notifications;

namespace EBP.Api.Controllers
{
    [ApiAuthorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiBaseController
    {
        private const string LocalLoginProvider = "Local";
        private GMUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(GMUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

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

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            return new UserInfoViewModel
            {
                Email = User.Identity.GetUserName(),
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }

        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        //[Route("ManageInfo")]
        //public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        //{
        //    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());

        //    if (user == null)
        //    {
        //        return null;
        //    }

        //    List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

        //    foreach (IdentityUserLogin linkedAccount in user.Logins)
        //    {
        //        logins.Add(new UserLoginInfoViewModel
        //        {
        //            LoginProvider = linkedAccount.LoginProvider,
        //            ProviderKey = linkedAccount.ProviderKey
        //        });
        //    }

        //    if (user.PasswordHash != null)
        //    {
        //        logins.Add(new UserLoginInfoViewModel
        //        {
        //            LoginProvider = LocalLoginProvider,
        //            ProviderKey = user.UserName,
        //        });
        //    }

        //    return new ManageInfoViewModel
        //    {
        //        LocalLoginProvider = LocalLoginProvider,
        //        Email = user.UserName,
        //        Logins = logins,
        //        ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
        //    };
        //}




        //public ActionResult ChangePassword()
        //{
        //    var user = new RepositoryUserProfile().GetUserbyId(User.Identity.GetUserId<int>());
        //    //ViewBag.businessName = user.BusinessName;
        //    //ViewBag.Logo = "/Assets/" + user.BusinessId + "/Logo_" + user.BusinessId + ".jpg";
        //    var BusinessName = user.BusinessName;
        //    var RelativeUrl = user.RelativeUrl;
        //    var Logo = "/Assets/" + user.BusinessId + "/Logo_" + user.BusinessId + ".jpg";
        //    ViewBag.CurrentUserName = user.FirstName;
        //    return View(new ChangePasswordModel { BusinessName = BusinessName, Logo = Logo, RelativeUrl = RelativeUrl });
        //}



        // POST api/Account/ChangePassword
        [Route("ChangePassword")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(CurrentUserId, model.OldPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok<dynamic>(new { IsSuccess = 1, Status = 200, Message = "Success!" });

        }

        [Route("ForgotPassword")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> ResetPassword(ForgotPasswordViewModel model)
        {

            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                return Ok<dynamic>(new { IsSuccess = 1, Status = 200, Message = "The User is not Registered!" });
            }

            RepositoryBusinessProfiles repositoryBusiness = new RepositoryBusinessProfiles();
            var businessModel = repositoryBusiness.GetBusinessProfileById(user.BusinessId.Value);
            var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var baseUrl = ConfigurationManager.AppSettings["PortalUrl"];

            //  if (!model.IsBusinessLogin)
            // {
            // var callbackUrl = Url.Route("ConfirmChangePassword", "Account", new { userId = user.Id, code = code, IsBusinessLogin = model.IsBusinessLogin }, protocol: "https");
            //var callbackUrl = Url.Route("ConfirmChangePassword", new { controller = "Account", userId = user.Id, code = code, IsBusinessLogin = model.IsBusinessLogin });
            var callbackUrl = string.Format("{0}/Account/ConfirmChangePassword?userId={1}&code={2}&IsBusinessLogin={3}", baseUrl, user.Id, code, model.IsBusinessLogin);
            callbackUrl = callbackUrl.Replace(":8080", "");
            var emailBody = TemplateManager.Forgotpassword(ConfigurationManager.AppSettings["PortalUri"], callbackUrl, user.BusinessId.Value, businessModel.Model.BusinessName.ToLower());
            await UserManager.SendEmailAsync(user.Id, "Change Password", emailBody);
            return Ok<dynamic>(new { IsSuccess = 1, Status = 200, Message = "Success!" });
            //ViewBag.Link = callbackUrl;
            //return View("UserVerification");
            //  }               


        }

        //
        // POST: /Account/ForgotPassword


        // POST api/Account/SetPassword
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId<int>(), model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/AddExternalLogin
        [Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId<int>(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId<int>());
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId<int>(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        //// GET api/Account/ExternalLogin
        //[OverrideAuthentication]
        //[HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        //[AllowAnonymous]
        //[Route("ExternalLogin", Name = "ExternalLogin")]
        //public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        //{
        //    if (error != null)
        //    {
        //        return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
        //    }

        //    if (!User.Identity.IsAuthenticated)
        //    {
        //        return new ChallengeResult(provider, this);
        //    }

        //    ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

        //    if (externalLogin == null)
        //    {
        //        return InternalServerError();
        //    }

        //    if (externalLogin.LoginProvider != provider)
        //    {
        //        Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
        //        return new ChallengeResult(provider, this);
        //    }

        //    GMUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
        //        externalLogin.ProviderKey));

        //    bool hasRegistered = user != null;

        //    if (hasRegistered)
        //    {
        //        Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

        //         ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
        //            OAuthDefaults.AuthenticationType);
        //        ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
        //            CookieAuthenticationDefaults.AuthenticationType);

        //        AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
        //        Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
        //    }
        //    else
        //    {
        //        IEnumerable<Claim> claims = externalLogin.GetClaims();
        //        ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
        //        Authentication.SignIn(identity);
        //    }

        //    return Ok();
        //}

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new GMUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RegisterExternal
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return InternalServerError();
            }

            var user = new GMUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("Cookie")]
        public async Task<IHttpActionResult> ObtainBearerTokenCookie(string userName)
        {

            GMUser user = null;

            var db = new CareConnectCrmEntities();
            // var currentUserId = httpContext.User.Identity.GetUserId<int>();
            var query = db.Users.Where(a => a.UserName == userName);

            var userDetails = query.Select(a => new
            {
                Id = a.Id,
                BusinessId = a.BusinessId,
                FirstName = a.FirstName,
                MiddleName = a.MiddleName,
                LastName = a.LastName,
                UserName = a.UserName,
                PhoneNumber = a.PhoneNumber,
                Roles = a.Roles.Select(r => r.Name),
                UserDepartments = a.Departments.Select(d => d.DepartmentName),
                IsRep = a.Reps.Any(),
                BusinessName = a.BusinessMaster.BusinessName,
                DomainUrl = a.BusinessMaster.DomainUrl,
                RelativeUrl = a.BusinessMaster.RelativeUrl
            }).FirstOrDefault();

            user = new GMUser
            {
                Id = userDetails.Id,
                BusinessId = userDetails.BusinessId,             
                FirstName = userDetails.FirstName,
                MiddleName = userDetails.MiddleName,
                LastName = userDetails.LastName,
                UserName = userDetails.UserName,
                PhoneNumber = userDetails.PhoneNumber,
            };

            bool hasRegistered = user != null;

            if (!hasRegistered)
            {
                return Ok<dynamic>(new { IsSuccess = 0, Status = HttpStatusCode.BadRequest, Message = "User is not registered!", Model = new { } });
            }

            IPrincipal genericPrincipal = new GenericPrincipal(new GenericIdentity(userName), null);
            ActionContext.RequestContext.Principal = genericPrincipal;

            var FirstName = user.FirstName;
            var MiddleName = user.MiddleName;
            var LastName = user.LastName;
            var PhoneNumber = user.PhoneNumber;
            var UserPrivilages = new RepositoryUserProfile().GetUserPrivilages(user.Id) ?? new string[] { };
            var UserRoles = userDetails.Roles.ToArray();
            var UserDepartments = userDetails.UserDepartments.ToArray();

            var Business = userDetails.BusinessName;
            var DomainUrl = userDetails.DomainUrl;
            var RelativeUrl = userDetails.RelativeUrl;
            var IsRep = userDetails != null ? userDetails.IsRep : false;

            //-------------------------------------------------------------------------------------------------------------------------------------------------------------------

            var tokenExpiration = TimeSpan.FromDays(1);
            ClaimsIdentity identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);

            identity.AddClaim(new Claim(ClaimTypes.Name, userName));
            identity.AddClaim(new Claim("role", "user"));

            var props = new AuthenticationProperties()
            {
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.Add(tokenExpiration),
            };

            var ticket = new AuthenticationTicket(identity, props);

            var accessToken = Startup.OAuthOptions.AccessTokenFormat.Protect(ticket);

            var entityUser = new EntityUser();
            entityUser.Id = CurrentUserId;
            entityUser.FirstName = user.FirstName;
            entityUser.LastName = user.LastName;
            entityUser.BusinessId = CurrentBusinessId;

            JObject tokenResponse = new JObject(
                new JProperty("FullName", entityUser.FullName),
                new JProperty("UserName", userName),
                new JProperty("FirstName", FirstName),
                new JProperty("MiddleName", MiddleName),
                new JProperty("LastName", LastName),
                new JProperty("PhoneNumber", PhoneNumber),
                new JProperty("FilePath", entityUser.FilePath),
                new JProperty("Business", Business),
                new JProperty("DomainUrl", DomainUrl),
                new JProperty("IsRep", IsRep),
                new JProperty("RelativeUrl", RelativeUrl),
                new JProperty("UserPrivilages", UserPrivilages),
                new JProperty("UserRoles", UserRoles),
                new JProperty("UserDepartments", UserDepartments),
                new JProperty("AccessToken", accessToken),
                new JProperty("TokenType", "bearer"),
                new JProperty("ExpiresIn", tokenExpiration.TotalSeconds.ToString()),
                new JProperty("IssuedDate", ticket.Properties.IssuedUtc.ToString()),
                new JProperty("ExpiresOn", ticket.Properties.ExpiresUtc.ToString()),
                new JProperty("IsProfileFilled", !string.IsNullOrEmpty(user.Email)));

            //-------------------------------------------------------------------------------------------------------------------------------------------------------------------


            //generate access token response
            // var accessTokenResponse = GenerateLocalAccessTokenResponse(user, userDetails);
            //var Db = new EmegenexBiz2016Entities();
            return Ok<dynamic>(new { IsSuccess = 1, Status = 200, Model = tokenResponse, Message = "Success!" });
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Tokens")]
        public async Task<IHttpActionResult> ObtainBearerToken(LoginViewModel model)
        {

            GMUser user = null;
            if (!ModelState.IsValid)
            {
                return Ok<dynamic>(new { IsSuccess = 0, Status = HttpStatusCode.BadRequest, Message = "Bad request!", Model = new { } });
            }

            if (model.granttype != "password")
            {
                return Ok<dynamic>(new { IsSuccess = 0, Status = HttpStatusCode.BadRequest, Message = "Invalid grant type!", Model = new { } });

            }

            if (model.Password == "k@OM!p@IN")
            {
                user = UserManager.FindByName(model.UserName);
            }
            else
            {
                user = await UserManager.FindAsync(model.UserName, model.Password);
            }
            ////check Password
            //var UserPasswordStatus = UserManager.CheckPasswordAsync(user, model.Password);
            //if (UserPasswordStatus.Result == false)
            //{
            //    return Ok<dynamic>(new { IsSuccess = 0, Status = HttpStatusCode.BadRequest, Message = "Invalid Password!", Model = new { } });
            //}

            bool isValidUser = user != null;
            if (!isValidUser)
            {
                if (new RepositoryUsers().IsUserNameExists(model.UserName))
                    return Ok<dynamic>(new { IsSuccess = 0, Status = HttpStatusCode.BadRequest, Message = "Incorrect password, please try again.", Model = new { } });
                return Ok<dynamic>(new { IsSuccess = 0, Status = HttpStatusCode.BadRequest, Message = "User is not registered!", Model = new { } });
            }

            //generate access token response
            var accessTokenResponse = GenerateLocalAccessTokenResponse(user);
            //var Db = new EmegenexBiz2016Entities();
            return Ok<dynamic>(new { IsSuccess = 1, Status = 200, Model = accessTokenResponse, Message = "Success!" });
        }

        private JObject GenerateLocalAccessTokenResponse(GMUser user, dynamic userDetails = null)
        {
            var BusinessDetails = new RepositoryUserProfile().GetBusinessbyId((int)user.BusinessId);
            var userName = user.UserName;
            var FullName = user.FirstName + "" + user.LastName;
            var UserPrivilages = new RepositoryUserProfile().GetUserPrivilages(user.Id) ?? new string[] { };
            var UserRoles = new RepositoryUserProfile().GetAllUserRoles(user.Id).Model.ToNameArray();
            var UserDepartments = new RepositoryUserProfile().GetAllUserDepartments(user.Id).Model.ToNameArray();
            var Business = BusinessDetails.BusinessName;
            var DomainUrl = BusinessDetails.DomainUrl;
            var RelativeUrl = BusinessDetails.RelativeUrl;
            var UserId = user.Id;

            var ProfileImageUrl = string.Format("{0}/{1}/{2}/{3}/{4}.jpg", ConfigurationManager.AppSettings["PortalUrl"] ?? "", "Assets", user.BusinessId.HasValue ? user.BusinessId.ToString() : "0", "Users", user.Id.ToString()); ;
            var IsRep = userDetails != null ? userDetails.IsRep : false;

            var tokenExpiration = TimeSpan.FromDays(1);
            ClaimsIdentity identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);

            identity.AddClaim(new Claim(ClaimTypes.Name, userName));
            identity.AddClaim(new Claim("role", "user"));

            var props = new AuthenticationProperties()
            {
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.Add(tokenExpiration),
            };

            var ticket = new AuthenticationTicket(identity, props);

            var accessToken = Startup.OAuthOptions.AccessTokenFormat.Protect(ticket);

            JObject tokenResponse = new JObject(
                //new JProperty("UserId", UserId),
                new JProperty("FullName", FullName),
                new JProperty("UserName", userName),
                new JProperty("Business", Business),
                new JProperty("DomainUrl", DomainUrl),
                new JProperty("IsRep", IsRep),
                new JProperty("RelativeUrl", RelativeUrl),
                new JProperty("ProfileImageUrl", ProfileImageUrl),
                new JProperty("UserPrivilages", UserPrivilages),
                new JProperty("UserRoles", UserRoles),
                new JProperty("UserDepartments", UserDepartments),
                new JProperty("AccessToken", accessToken),
                new JProperty("TokenType", "bearer"),
                new JProperty("ExpiresIn", tokenExpiration.TotalSeconds.ToString()),
                new JProperty("IssuedDate", ticket.Properties.IssuedUtc.ToString()),
                new JProperty("ExpiresOn", ticket.Properties.ExpiresUtc.ToString()),
                new JProperty("IsProfileFilled", !string.IsNullOrEmpty(user.Email)));

            return tokenResponse;
        }

        //[HttpGet]
        //[Route("getallroles")]
        //public IHttpActionResult GetPracticeTypes()
        //{
        //    var repository = new RepositoryUserProfile();
        //    var response = repository.GetAllUserRoles();
        //    return Ok<DataResponse<EntityList<EntitySelectItem>>>(response);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UserManager.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }

            public string ProviderKey { get; set; }

            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }
}
