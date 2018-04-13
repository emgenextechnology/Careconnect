using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace EmgenexBusinessPortal
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Application["LiveSessionsCount"] = 0;
        }

        protected void Application_EndRequest(Object sender, EventArgs e)
        {
            string cookieName = System.Web.Security.FormsAuthentication.FormsCookieName;
            if (Response.Cookies.Count > 0)
            {
                foreach (string s in Response.Cookies.AllKeys)
                {
                    if (s == cookieName)
                    {
                        Response.Cookies[cookieName].HttpOnly = false;
                    }
                }
            }
        }

        void Session_Start(object sender, EventArgs e)
        {
            Application["LiveSessionsCount"] = ((int)Application["LiveSessionsCount"]) + 1;
        }

        void Session_End(object sender, EventArgs e)
        {
            Application["LiveSessionsCount"] = ((int)Application["LiveSessionsCount"]) - 1;
        }
    }
}
