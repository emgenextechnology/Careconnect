using System.Web.Mvc;

namespace EmgenexBusinessPortal.Areas.Business
{
    public class BusinessAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Business1";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            //  context.MapRoute(
            //    "BusinessDashboard",
            //    "Business",
            //    new { action = "Index", controller = "BusinessDashboard", id = UrlParameter.Optional }
            //);
            //  context.MapRoute(
            //      "Business_default",
            //      "Business/{controller}/{action}/{id}",
            //      new { action = "Index", id = UrlParameter.Optional }
            //  );
        }
    }
}