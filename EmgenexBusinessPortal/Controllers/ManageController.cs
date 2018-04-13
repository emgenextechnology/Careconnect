using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.IO;
using EBP.Business.Database;
using GM.Identity.Models;
using EmgenexBusinessPortal.Models;
using GM.Identity;
using System.Threading.Tasks;
using EBP.Business.Repository;

namespace EmgenexBusinessPortal.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        public ManageController()
        {
        }

        public ManageController(GMUserManager userManager)
        {
            UserManager = userManager;
        }

        private GMUserManager _userManager;
        public GMUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<GMUserManager>();
            }
            private set
            {
                _userManager = value;
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

        private CareConnectCrmEntities Db = new CareConnectCrmEntities();
        // GET: /Account/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            //var userid = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());
            //ViewBag.CurrentUserName = userid.FirstName;
            var user = new RepositoryUserProfile().GetUserbyId(User.Identity.GetUserId<int>());
            ViewBag.businessName = user.BusinessName;
            ViewBag.RelativeUrl = user.RelativeUrl;
            ViewBag.Logo = "/Assets/" + user.BusinessId + "/Logo_" + user.BusinessId + ".jpg";
            ViewBag.CurrentUserName = user.FirstName;
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";

            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(User.Identity.GetUserId<int>()),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(User.Identity.GetUserId<int>()),
                Logins = await UserManager.GetLoginsAsync(User.Identity.GetUserId<int>()),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(User.Identity.GetUserId<int>().ToString())
            };
            return View(model);
        }
        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            var user = new RepositoryUserProfile().GetUserbyId(User.Identity.GetUserId<int>());
            //ViewBag.businessName = user.BusinessName;
            //ViewBag.Logo = "/Assets/" + user.BusinessId + "/Logo_" + user.BusinessId + ".jpg";
            var BusinessName = user.BusinessName;
            var RelativeUrl = user.RelativeUrl;
            var Logo = "/Assets/" + user.BusinessId + "/Logo_" + user.BusinessId + ".jpg";
            ViewBag.CurrentUserName = user.FirstName;
            return View(new ChangePasswordModel { BusinessName = BusinessName, Logo = Logo, RelativeUrl = RelativeUrl });
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId<int>(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());
                if (user != null)
                {
                    await SignInAsync(user, isPersistent: false);
                }
                return View(new ChangePasswordModel { BusinessName = model.BusinessName, Logo = model.Logo, Result = "Password changed successfully." });
                //return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        public async Task<ActionResult> SetPassword()
        {
            var userid = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());
            ViewBag.CurrentUserName = userid.FirstName;
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId<int>(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());
                    if (user != null)
                    {
                        await SignInAsync(user, isPersistent: false);
                    }
                    return Redirect("/");
                    //return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        private async System.Threading.Tasks.Task SignInAsync(GMUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, await user.GenerateUserIdentityAsync(UserManager));
        }
        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId<int>());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }
        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }
    }
}