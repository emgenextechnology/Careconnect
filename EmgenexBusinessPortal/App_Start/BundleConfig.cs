using System.Web;
using System.Web.Optimization;

namespace EmgenexBusinessPortal
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/scripts/plugins").Include(
                      "~/Controlpanel/Plugins/Moment/moment.min.js",
                      "~/Controlpanel/Plugins/Moment/angular-moment.js",
                      "~/Controlpanel/Plugins/Masking/masks.js?i=0",
                      "~/Controlpanel/Plugins/Datepicker/js/datetimepicker.js",
                      "~/Controlpanel/Plugins/Datepicker/js/datetimepicker.templates.js",
                      "~/Controlpanel/Scripts/tiny.js",
                      "~/Controlpanel/Plugins/NiceScroll/jquery.nicescroll.min.js",
                      "~/Controlpanel/Plugins/Select2/select2.js"));

            //bundles.Add(new ScriptBundle("~/scripts/controllers1").Include(
            //        "~/Controlpanel/Scripts/CareConnect/Services/authInterceptorService.js",
            //        "~/Controlpanel/Scripts/CareConnect/app.js",
            //        "~/Controlpanel/Scripts/CareConnect/Services/authService.js",
            //        "~/Controlpanel/Scripts/CareConnect/Services/tokensManagerService.js",
            //        "~/Controlpanel/Scripts/CareConnect/Services/leadService.js"
            //        //"~/Controlpanel/Scripts/CareConnect/Controllers/authController.js",
            //        //"~/Controlpanel/Scripts/CareConnect/Controllers/headerController.js",
            //        //"~/Controlpanel/Scripts/CareConnect/Controllers/dashboardController.js",
            //        //"~/Controlpanel/Scripts/CareConnect/Controllers/leadController.js",
            //        //"~/Controlpanel/Scripts/CareConnect/Controllers/accountController.js",
            //        //"~/Controlpanel/Scripts/CareConnect/Controllers/tasksController.js",
            //        //"~/Controlpanel/Scripts/CareConnect/Controllers/salesController.js",
            //        //"~/Controlpanel/Scripts/CareConnect/Controllers/marketingController.js"
            //        ));



            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Controlpanel/Content/css/bootstrap.min.css",
                      "~/Controlpanel/Content/css/font-awesome.min.css",
                      "~/Controlpanel/Content/css/custom.css",
                      "~/Controlpanel/Content/css/dataTables.bootstrap.min.css",
                      "~/Controlpanel/Content/css/bootstrap-select.min.css",
                      "~/Controlpanel/Content/css/floating-nav.css",
                      "~/Controlpanel/Content/css/awesome-bootstrap-checkbox.css",
                      "~/Controlpanel/Content/css/select2.min.css",
                      "~/Controlpanel/Content/css/bootstrap-datepicker3.min.css",
                      "~/Controlpanel/Plugins/DataTable/css/angular-datatables.css",
                      "~/Controlpanel/Plugins/Datepicker/css/datetimepicker.css",
                      "~/Controlpanel/Content/css/placeholder-title-transition.css",
                      "~/Controlpanel/Scripts/c3-angular-directive/css/c3.min.css",
                      "~/Content/style.css",
                      "~/Controlpanel/Plugins/ui-select/common.css"
                      ));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = true;
        }
    }
}
