using GM.Identity;
using GM.Identity.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;


//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.EntityFramework;
//using Microsoft.AspNet.Identity.Owin;
//using Microsoft.Owin;
//using Microsoft.Owin.Security.Cookies;
//using Microsoft.Owin.Security.DataProtection;
//using Microsoft.Owin.Security.Google;
//using Owin;
//using System;
//using GM.Designer.Models;
//using GM.Identity.Models;
//using GM.Identity;

namespace EmgenexBusinessPortal
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(GMDbContext.Create);
            app.CreatePerOwinContext<GMUserManager>(GMUserManager.Create);
            app.CreatePerOwinContext<GMRoleManager>(GMRoleManager.Create);
            app.CreatePerOwinContext<GMSignInManager>(GMSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/"),
                ExpireTimeSpan = TimeSpan.FromMinutes(20),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<GMUserManager, GMUser, int>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentityCallback: (manager, user) => user.GenerateUserIdentityAsync(manager),
                        getUserIdCallback: (id)=>(id.GetUserId<int>()))
                }
            });
            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //////app.UseFacebookAuthentication(
            //////   appId: "506217052744470",
            //////   appSecret: "cf12dc5726aa035190abad363858494b");

            //////app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //////{
            //////    ClientId = "472603296826-kn9nm7um4umvgllkk8n4274ee68uj51f.apps.googleusercontent.com",
            //////    ClientSecret = "8aY4qZFNFyrLAq0egGwNrmqR"
            //////});
        }
    }
}