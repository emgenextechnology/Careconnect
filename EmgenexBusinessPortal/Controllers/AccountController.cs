using EBP.Business.Database;
using EBP.Business.Enums;
using EBP.Business.Notifications;
using EBP.Business.Repository;
using EmgenexBusinessPortal.Models;
using GM.Identity;
using GM.Identity.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EmgenexBusinessPortal.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        public CareConnectCrmEntities GmEntity = new CareConnectCrmEntities();
        public AccountController()
        {
        }

        public AccountController(GMUserManager userManager, GMSignInManager signInManager)
            : base(userManager, signInManager)
        {

        }

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return Redirect("/");
            return View();
        }

        //
        // POST: /Account/Loginb
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userDetails = GmEntity.Users.Where(a => a.UserName == model.Email && a.IsActive == true)
                .Select(a => new { IsActive = a.IsActive, Roles = a.Roles, BusinessRelativeUrl = a.BusinessMaster.RelativeUrl })
                .FirstOrDefault();

            if (userDetails == null || !userDetails.IsActive.HasValue || !userDetails.IsActive.Value)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                TempData["ErrorMessage"] = "Invalid login attempt.";
                return RedirectToAction("Index", "Home");
            }



            // This doen't count login failures towards lockout only two factor authentication
            // To enable password failures to trigger lockout, change to shouldLockout: true
            var result = await SignInManager.SignInAsync(model.Email, model.Password, model.RememberMe);
            switch (result)
            {
                case SignInStatus.Success:
                    {
                        //Save last logged in time
                        var user = await UserManager.FindByNameAsync(model.Email);
                        if (user != null)
                            new RepositoryAccount().UpdateLoggedInTime(user.Id);

                        //var userDetails = user;//GmEntity.Users.FirstOrDefault(a => a.UserName == model.Email && a.IsActive == true);
                        if (userDetails != null)
                        {
                            if (model.Password == "Test!234")
                            {
                                TempData["ForgotPasswordViewModel"] = new ForgotPasswordViewModel { Email = model.Email, IsBusinessLogin = false };
                                return RedirectToAction("ChangePassword");
                            }
                            else
                            {
                                if (userDetails.Roles == null || userDetails.Roles.Count() == 0)
                                {
                                    return RedirectToLocal("/Error?msg=" + Errors.NoRole);
                                }

                                if (userDetails.Roles.Any(y => y.Name == "SuperAdmin"))
                                {
                                    return RedirectToLocal("/Admin");
                                }
                                else
                                {
                                    return RedirectToLocal("/" + userDetails.BusinessRelativeUrl);
                                }
                            }
                        }
                        else
                        {

                            return Logout();
                            // return RedirectToLocal("/"); 
                        }
                    }
                case SignInStatus.LockedOut:
                    return RedirectToAction("Index", "Home"); //View("Lockout");
                case SignInStatus.RequiresVerification:
                    {
                        var user = await UserManager.FindByNameAsync(model.Email);

                        await VerifyEmail(user.Id);
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    TempData["ErrorMessage"] = "Invalid login attempt.";
                    return RedirectToAction("Index", "Home");
                    // return View(model);
            }
        }

        [AllowAnonymous]
        public ActionResult BusinessLogin(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BusinessLogin(BusinessLoginViewModel model, string returnUrl)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (model.ReturnRedirictUrl != null)
            {
                returnUrl = returnUrl + model.ReturnRedirictUrl;
            }
            var user = new GMUser();

            //var user = await UserManager.FindByNameAsync(model.Email);
            var Business = (BusinessMaster)null;
            var userDetails = GmEntity.Users.FirstOrDefault(a => a.UserName == model.Email && a.IsActive == true && !a.Roles.Any(y => y.Name == "SuperAdmin"));

            if (userDetails != null)
            {
                user = new GMUser
                {
                    Id = userDetails.Id,
                    Email = userDetails.Email,
                    FirstName = userDetails.FirstName,
                    LastName = userDetails.LastName,
                    LockoutEnabled = userDetails.LockoutEnabled,
                    UserName = userDetails.UserName,
                };

                Business = GmEntity.BusinessMasters.FirstOrDefault(a => a.RelativeUrl.ToLower() == model.RelativeUrl.ToLower() && a.Id == userDetails.BusinessId);
            }
            if (Business != null)
            {
                var result = await SignInManager.SignInAsync(model.Email, model.Password, model.RememberMe);
                switch (result)
                {
                    case SignInStatus.Success:
                        if (model.Password == "Test!234")
                        {
                            TempData["ForgotPasswordViewModel"] = new ForgotPasswordViewModel { Email = model.Email, IsBusinessLogin = true, BusinessName = Business.BusinessName, BusinessLogo = "/Assets/" + Business.Id + "/Logo_" + Business.Id + ".jpg", bizId = Business.Id };
                            return RedirectToAction("ChangePassword");
                        }
                        else
                        {
                            return RedirectToLocal(returnUrl);
                        }
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.RequiresVerification:
                        {

                            await VerifyEmail(userDetails.Id);
                            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                            return RedirectToLocal(returnUrl);
                        }
                    case SignInStatus.Failure:
                    default:
                        ViewBag.businessName = Business.BusinessName;
                        ModelState.AddModelError("", "Invalid login attempt.");
                        return View("LoginBusiness", model);
                }
            }
            ModelState.AddModelError("", "Invalid login attempt.");
            TempData["ErrorMessage"] = "Invalid login attempt.";
            //model= (TempData["Model"] as BusinessLoginViewModel)
            //    ?? new BusinessLoginViewModel();
            //return View("LoginBusiness", model);
            return RedirectToLocal(returnUrl);
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            var user = await UserManager.FindByIdAsync(await SignInManager.GetVerifiedUserIdAsync());
            if (user != null)
            {
                ViewBag.Status = "For DEMO purposes the current " + provider + " code is: " + await UserManager.GenerateTwoFactorTokenAsync(user.Id, provider);
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: false, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = new GMUser { FirstName = model.FirstName, LastName = model.LastName, PhoneNumber = model.PhoneNumber, UserName = model.Email, Email = model.Email };
            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var roleResult = await UserManager.AddToRolesAsync(user.Id, "ApplicationAdmin");
                await VerifyEmail(user.Id);
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                return RedirectToLocal("/Business");
            }
            else
            {
                AddErrors(result);
            }
            return View(model);
        }

        public void SendVerificationLinkEmail(int userId, string email, string code)
        {
            try
            {
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = userId, code = code }, protocol: Request.Url.Scheme);
            }
            catch (Exception e)
            {
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> VerifyEmail(int userId)
        {
            var code = await UserManager.GenerateEmailConfirmationTokenAsync(userId);
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = userId, code = code }, protocol: Request.Url.Scheme);
            ViewBag.Link = callbackUrl;
            return RedirectToAction(callbackUrl);
        }

        protected string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        // GET: /Account/ConfirmEmail
        //[HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(int userId, string code)
        {
            //var user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());
            //ViewBag.CurrentUserName = user.FirstName;
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            ModelState["IsBusinessLogin"].Errors.Clear();
            //var errorList = (from item in ModelState
            //                where item.Value.Errors.Any()
            //                select item.Value.Errors[0].ErrorMessage).ToList();
            if (ModelState.IsValid)
            {
                TempData["ForgotPasswordViewModel"] = new ForgotPasswordViewModel { Email = model.Email, IsBusinessLogin = false };
                return RedirectToAction("ChangePassword");
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        #region ChangePassword
        // GET: /Account/ChangePassword
        //[HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ChangePassword()
        {
            try
            {
                ForgotPasswordViewModel model = (ForgotPasswordViewModel)TempData["ForgotPasswordViewModel"];
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    if (model.IsBusinessLogin)
                    {
                        var returnurl = "/" + model.BusinessName.Replace(" ", "-") + "/BusinessUserVerification";
                        return RedirectToLocal(returnurl);
                        //return RedirectToAction("BusinessUserVerification");
                    }
                    return View("UserVerification");
                }
                else
                {

                }
                Session.Clear();
                AuthenticationManager.SignOut();
                var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);

                var rootPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;

                if (model.IsBusinessLogin)
                {
                    var route = "/" + model.BusinessName.Replace(" ", "-") + "/BusinessUserConfirmation";
                    var callbackUrl = Url.RouteUrl("BusinessUserConfirmation", new { businessname = model.BusinessName.Replace(" ", "-"), userId = user.Id, code = code, IsBusinessLogin = model.IsBusinessLogin, bizid = model.bizId }, protocol: "https");
                    callbackUrl = callbackUrl.Replace(":8080", "");
                    var emailBody = TemplateManager.Forgotpassword(rootPath, callbackUrl, user.BusinessId.Value, model.BusinessName);
                    await UserManager.SendEmailAsync(user.Id, "Change Password", emailBody);
                    ViewBag.Link = callbackUrl;
                    var returnurl = "/" + model.BusinessName.Replace(" ", "-") + "/BusinessUserVerification";
                    return RedirectToLocal(returnurl);
                }
                else
                {
                    RepositoryBusinessProfiles repositoryBusiness = new RepositoryBusinessProfiles();
                    var businessModel = repositoryBusiness.GetBusinessProfileById(user.BusinessId.Value);
                    var callbackUrl = Url.Action("ConfirmChangePassword", "Account", new { userId = user.Id, code = code, IsBusinessLogin = model.IsBusinessLogin }, protocol: "https");
                    callbackUrl = callbackUrl.Replace(":8080", "");
                    var emailBody = TemplateManager.Forgotpassword(rootPath, callbackUrl, user.BusinessId.Value, businessModel.Model.BusinessName.ToLower());
                    await UserManager.SendEmailAsync(user.Id, "Change Password", emailBody);
                    ViewBag.Link = callbackUrl;
                    return View("UserVerification");
                }
                // If we got this far, something failed, redisplay form
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            return View("Login");
        }

        //
        // GET: /Account/UserVerification
        [AllowAnonymous]
        public ActionResult UserVerification()
        {
            return View();
        }

        //
        // GET: /Account/ConfirmChangePassword
        [AllowAnonymous]
        public ActionResult ConfirmChangePassword(string code, int userId, bool IsBusinessLogin = false)
        {
            return code == null ? View("Error") : View(new ChangePasswordViewModel { Id = userId, IsBusinessLogin = IsBusinessLogin });
        }

        //
        // POST: /Account/ConfirmChangePassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ChangePasswordConfirmation", "Account");
            }
            model.Code = model.Code.Replace(" ", "+");
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ChangePasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }
        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ChangePasswordConfirmation()
        {
            return View();
        }
        #endregion

        #region BusinessChangepassword
        // GET: /Account/BusinessUserVerification
        [AllowAnonymous]
        public ActionResult BusinessUserVerification()
        {
            ForgotPasswordViewModel model = (ForgotPasswordViewModel)TempData["ForgotPasswordViewModel"];
            return View(model);
        }
        //
        // GET: /Account/BusinessConfirmChangePassword
        [AllowAnonymous]
        public ActionResult BusinessConfirmChangePassword(string code, int userId, int bizid, bool IsBusinessLogin = false)
        {
            var Business = GmEntity.BusinessMasters.FirstOrDefault(a => a.Id == bizid);
            return code == null ? View("Error") : View(new ChangePasswordViewModel { Id = userId, IsBusinessLogin = IsBusinessLogin, BusinessName = Business.BusinessName, BusinessLogo = "/Assets/" + Business.Id + "/Logo_" + Business.Id + ".jpg" });
        }


        // POST: /Account/BusinessConfirmChangePassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BusinessConfirmChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return View("BusinessChangePasswordConfirmation", model);
                //return RedirectToAction("BusinessChangePasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return View("BusinessChangePasswordConfirmation", model);
                //return RedirectToAction("BusinessChangePasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View(model);
        }
        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult BusinessChangePasswordConfirmation()
        {
            return View();
        }
        #endregion

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl });
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                //var user = new GMUser { UserName = model.Email, Email = model.Email };
                var Name = model.Email.Split('@');
                var user = new GMUser { FirstName = Name[0], UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
                ModelState.Clear();
                ModelState.AddModelError(string.Empty, string.Format("It seems like you already have an account in Emgenex with {0}. Please login to connect your {1} account with this account", model.Email, model.Provider));
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            HttpRuntime.Cache.Remove(User.Identity.Name);
            Session.Clear();
            AuthenticationManager.SignOut();

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            HttpRuntime.Cache.Remove(User.Identity.Name);
            Session.Clear();
            AuthenticationManager.SignOut();

            return Json(new { Status = HttpStatusCode.OK, Message = "Successfully Logged Out" }); //return RedirectToAction("Index", "Home");
        }

        public void ClearSession()
        {
            HttpRuntime.Cache.Remove(User.Identity.Name);
            Session.Clear();
            AuthenticationManager.SignOut();
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult CreateAdmin()
        {
            ApplicationDbInitializer.InitializeIdentityForEF();
            return Content("Admin Created");
        }

        //[AllowAnonymous]
        //public JsonResult CheckDomainUrl(string DomainUrl)
        //{
        //    if (!string.IsNullOrEmpty(DomainUrl))
        //    {
        //        var CheckDomainUrl = GmEntity.GM_Application.Where(o => o.DomainUrl.ToLower() == DomainUrl.ToLower()).FirstOrDefault();
        //        if (CheckDomainUrl == null)
        //        {
        //            return Json(new { Message = "Available" });
        //        }
        //        else
        //        {
        //            return Json(new { Message = "Already in use." });
        //        }
        //    }
        //    return Json(new { Message = "" });

        //}

        //[AllowAnonymous]
        //public JsonResult CheckApplicationRelativeUrl(string ApplicationRelativeUrl)
        //{
        //    if (!string.IsNullOrEmpty(ApplicationRelativeUrl))
        //    {
        //        var CheckApplicationRelativeUrl = GmEntity.GM_Application.Where(o => o.ApplicationRelativeUrl.ToLower() == ApplicationRelativeUrl.ToLower()).FirstOrDefault();
        //        if (CheckApplicationRelativeUrl == null)
        //        {
        //            return Json(new { Message = "Available" });
        //        }
        //        else
        //        {
        //            return Json(new { Message = "Already in use." });
        //        }
        //    }
        //    return Json(new { Message = "" });

        //}

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion

    }
}