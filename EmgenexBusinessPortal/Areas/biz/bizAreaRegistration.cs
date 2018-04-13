using System.Web.Mvc;

namespace EmgenexBusinessPortal.Areas.biz
{
    public class bizAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "biz";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "biz_default",
                "biz/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}