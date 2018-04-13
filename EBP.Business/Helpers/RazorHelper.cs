using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EBP.Business
{
    public static class RazorHelper
    {
        public static string RenderRazorViewToString(string viewName, Controller controller)// ViewDataDictionary viewData, ControllerContext context, TempDataDictionary tempData)
        {
            //ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                //new Exception(viewName).Log();
                var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(controller.ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

    }
}
