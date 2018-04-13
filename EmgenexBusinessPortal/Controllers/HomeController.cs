using EBP.Business.Database;
using EmgenexBusinessPortal.Models;
using GM.Identity;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using EBP.Business.Repository;
using EBP.Business.Notifications;
using EBP.Business;
using Microsoft.Owin.Security;
using System.Configuration;

namespace EmgenexBusinessPortal.Controllers
{

    public class EmgenBizController : Controller
    {

        protected override IAsyncResult BeginExecute(System.Web.Routing.RequestContext requestContext, AsyncCallback callback, object state)
        {
            return base.BeginExecute(requestContext, callback, state);
        }

        protected override void OnActionExecuting(ActionExecutingContext ctx)
        {

        }
    }

    public class HomeController : EmgenBizController
    {

        public ActionResult AdminStatic()
        {
            return File("~/admin/index.html", "text/html");
        }

        public ActionResult BusinessStatic()
        {
            return File("~/business/index.html", "text/html");
        }
        public ActionResult Index()
        {
            var errMsg = TempData["ErrorMessage"] as string;
            ModelState.AddModelError("", errMsg);
            if (User.Identity.IsAuthenticated)
            {
                var GmEntity = new CareConnectCrmEntities();
                var userDetails = GmEntity.Users.FirstOrDefault(a => a.UserName == User.Identity.Name);
                if (userDetails != null)
                {
                    if (userDetails.Roles.Any(y => y.Name == "SuperAdmin"))
                    {
                        Response.Redirect("/Admin");
                    }
                    else
                    {
                        Response.Redirect(userDetails.BusinessMaster.RelativeUrl);
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "Account");
                }
            }

            return View();
        }

        //[BusinessAuthorize]
        public ActionResult Business()
        {
            var errMsg = TempData["ErrorMessage"] as string;
            ModelState.AddModelError("", errMsg);
            var db = new CareConnectCrmEntities();
            ViewBag.version = ConfigurationManager.AppSettings["AppVersion"] ?? "1.0";

            if (RouteData.Values["businessname"] != null)
            {

                var businussUrl = RouteData.Values["businessname"].ToString();
                var business = db.BusinessMasters.FirstOrDefault(a => a.RelativeUrl.ToLower() == businussUrl.Replace(" ", "-").ToLower());

                if (business != null)
                    ViewBag.DefaultDateRange = business.DateRange ?? 3;

                if (business == null)
                    return RedirectToAction("NoPermission", new { title = "" });

                if (User.Identity.IsAuthenticated)
                {
                    if (db.Users.Count(a => a.UserName == User.Identity.Name && a.BusinessId == business.Id) == 0 || business == null)

                        return RedirectToAction("NoPermission", new { title = business.BusinessName });
                }

                //if (!User.Identity.IsAuthenticated) { return RedirectToAction("Login", "Account", new { returnUrl="/"+BusinussUrl }); }
                if (business != null)
                {
                    //check in db if biz url exists if so load business object and pass to view
                    //if business doest not exists go to business login
                    ViewBag.businessName = business.BusinessName;
                    ViewBag.Logo = "/Assets/" + business.Id + "/Logo_" + business.Id + ".jpg";
                    ViewBag.RelativeUrl = business.RelativeUrl;
                    ViewBag.Title = "Home Page";
                    return View();
                }
                else
                {
                    return RedirectToAction("BusinessContact", new { title = ViewBag.businessName });
                }
            }
            return View();

        }

        [AllowAnonymous]
        public ActionResult BusinessContact(string title)
        {
            ViewBag.Title = "";
            if (!string.IsNullOrEmpty(title))
                ViewBag.Title = title;

            return View();
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        [AllowAnonymous]
        public ActionResult NoPermission(string title)
        {
            ViewBag.Title = "";
            if (!string.IsNullOrEmpty(title))
                ViewBag.Title = title;

            AuthenticationManager.SignOut();
            return View();
        }

        [AllowAnonymous]
        public ActionResult CareconnectNotified(string path)
        {
            ViewBag.Title = "";
            if (!string.IsNullOrEmpty(path))
                ViewBag.Title = path;

            return View();
        }

        public ActionResult LeadPdf(int id)
        {
            return pdf(new PDFModel { IsAccount = false, Id = id });
        }

        public ActionResult AccountPdf(int id)
        {
            return pdf(new PDFModel { IsAccount = true, Id = id });
        }

        public FileStreamResult pdf(PDFModel PDFModel)
        {
            if (PDFModel.IsAccount)
            {
                var repository = new RepositoryAccount();
                var model = repository.GetAccountById(PDFModel.Id).Model;
                ViewData.Model = new PDFViewModel
                {
                    Practice = model.Practice,
                    Providers = model.Practice.Providers,
                    Address = model.Practice.Address,
                    CreatedByName = model.CreatedByName,
                    CreatedOn = model.CreatedOn,
                    EnrolledDate = model.EnrolledDate,
                    Rep = model.Rep,
                };

                PDFModel.Content = RazorHelper.RenderRazorViewToString("~/Views/Shared/accountPDF.cshtml", this);
            }
            else
            {
                var repository = new RepositoryLead();
                var model = repository.GetLeadById(PDFModel.Id).Model;
                ViewData.Model = new PDFViewModel
                {
                    Practice = model.Practice,
                    Providers = model.Practice.Providers,
                    Address = model.Practice.Address,
                    CreatedByName = model.CreatedByName,
                    CreatedOn = model.CreatedOn,
                    Rep = model.Rep,
                };

                PDFModel.Content = RazorHelper.RenderRazorViewToString("~/Views/Shared/leadPDF.cshtml", this);
            }
            var accountPdfContent = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                                <!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">
                                <html xmlns=""http://www.w3.org/1999/xhtml"" xml:lang=""en"" lang=""en"">
                                <head>
                                    <title>Account Details</title>
                                </head>
                                <body>" + PDFModel.Content + "</body></html>";

            var leadPdfContent = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                                <!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">
                                <html xmlns=""http://www.w3.org/1999/xhtml"" xml:lang=""en"" lang=""en"">
                                <head>
                                    <title>Lead Details</title>
                                </head>
                                <body>" + PDFModel.Content + "</body></html>";

            var bytes = System.Text.Encoding.UTF8.GetBytes(PDFModel.IsAccount ? accountPdfContent : leadPdfContent);

            using (var input = new MemoryStream(bytes))
            {
                var output = new MemoryStream();

                var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 0, 0, 0, 0);
                var writer = PdfWriter.GetInstance(document, output);

                writer.CloseStream = false;
                document.Open();

                var xmlWorker = XMLWorkerHelper.GetInstance();
                xmlWorker.ParseXHtml(writer, document, input, default(Stream));

                document.Close();
                output.Position = 0;

                return new FileStreamResult(output, "application/pdf");
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionResult Adminpanel()
        {
            return View();
        }

        public ActionResult GetTodoList()
        {
            return Json(new
            {
                Count = 0,
                CountEnded = 0,
                Id = 1,
                Name = "List 1",
                Task = new
                {
                    Ended = false,
                    Id = 1,
                    ListId = 1,
                    Name = "Task0101"
                }
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
