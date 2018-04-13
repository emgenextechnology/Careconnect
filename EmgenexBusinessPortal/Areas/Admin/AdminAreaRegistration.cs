using System.Web.Mvc;

namespace EmgenexBusinessPortal.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
               "Admin_default_users",
               "Admin/users/{action}/{id}",
               new { action = "Index", controller = "AdminUsers", id = UrlParameter.Optional }
           );
            
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { action = "Index", controller="AdminDashboard", id = UrlParameter.Optional }
            );

        }
    }
}