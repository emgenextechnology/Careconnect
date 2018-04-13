using System.Web.Mvc;

namespace EmgenexBusinessPortal.Areas.BusinessSettings
{
    public class BusinessSettingsAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "BusinessSettings";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "BusinessSettings_default",
                "BusinessSettings/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}