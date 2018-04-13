using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;

[assembly: OwinStartup(typeof(EBP.Api.Startup))]

namespace EBP.Api
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //app.UseCookieAuthentication()
            ConfigureAuth(app);

            //app.UseCookieAuthentication(new CookieAuthenticationOptions
            //{
            //    AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
            //    LoginPath = new PathString("/Account/Login"),
            //    CookieSecure = CookieSecureOption.Always
            //});
        }
    }
}
