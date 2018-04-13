using EmgenexBusinessPortal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EmgenexBusinessPortal
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.RouteExistingFiles = true;

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.IgnoreRoute("admin/{*pathInfo}");

            routes.MapRoute(
                name: "adminRoute",
                url: "admin/{*url}",
                defaults: new { controller = "Home", action = "AdminStatic", url = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "businessRoute",
                url: "business/{*url}",
                defaults: new { controller = "Home", action = "BusinessStatic" }
            );

            //routes.MapRoute(
            //name: "Application1Override",
            //url: "Admin/{*.}",
            //defaults: new { controller = "Admin", action = "Index" }
            //);

            //RouteTable.Routes.MapPageRoute("AnythingNonApi", "Admin/{*.}", "~/Admin/index.html");

            routes.MapRoute(
                name: "ChangePassword",
                url: "{businessname}/ChangePassword",
                defaults: new { controller = "Manage", action = "ChangePassword" },
                constraints: new { businessname = "^((?!(Account)).)*$" }
            );
            routes.MapRoute(
                name: "SetPassword",
                url: "Manage/SetPassword",
                defaults: new { controller = "Manage", action = "SetPassword" },
                constraints: new RouteConstraints("")
            );
            routes.MapRoute(
                 name: "Manage",
                 url: "Manage/{id}",
                 defaults: new { controller = "Manage", action = "Index", id = UrlParameter.Optional },
                constraints: new RouteConstraints("")
             );
            routes.MapRoute(
                name: "NotificationSettings",
                url: "{businessname}/NotificationSettings",
                constraints: new RouteConstraints(""),
                defaults: new { controller = "UserNotificationSettings", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
               name: "BusinessDash",
               url: "{businessname}",
               constraints: new RouteConstraints(""),
               defaults: new { controller = "Home", action = "business", id = UrlParameter.Optional }
           );

            routes.MapRoute(
               name: "CareconnectNotified",
               url: "{businessname}",
               constraints: new RouteConstraints(""),
               defaults: new { controller = "Home", action = "CareconnectNotified", id = UrlParameter.Optional }
           );

            //routes.MapRoute(
            // name: "BusinessSettings",
            // url: "{businessname}/settings",
            // constraints: new RouteConstraints(""),
            // defaults: new { controller = "BusinessDashboard", action = "index", area = "business" }
            //).DataTokens.Add("area", "business");

            routes.MapRoute(
             name: "bizTest",
             url: "biz",
             constraints: new RouteConstraints(""),
             defaults: new { controller = "TestH", action = "index", area = "biz" }
         ).DataTokens.Add("area", "biz");

            routes.MapRoute(
               name: "UserVerification",
               url: "{businessname}/BusinessUserVerification",
               constraints: new RouteConstraints(""),
               defaults: new { controller = "Account", action = "BusinessUserVerification", businessname = UrlParameter.Optional }
           );
            routes.MapRoute(
               name: "BusinessUserConfirmation",
               url: "{businessname}/BusinessUserConfirmation",
               constraints: new RouteConstraints(""),
               defaults: new { controller = "Account", action = "BusinessConfirmChangePassword", businessname = UrlParameter.Optional }
           );

            routes.MapRoute(
                name: "AccountPdf",
                url: "Account/Pdf/{id}",
                constraints: new RouteConstraints(""),
                defaults: new { controller = "Home", action = "AccountPdf", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "LeadPdf",
                url: "Lead/Pdf/{id}",
                constraints: new RouteConstraints(""),
                defaults: new { controller = "Home", action = "LeadPdf", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                constraints: new RouteConstraints(""),
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
