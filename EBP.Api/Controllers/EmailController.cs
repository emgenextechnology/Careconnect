using EBP.Api.Models;
using EBP.Business;
using EBP.Business.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace EBP.Api.Controllers
{
    public class EmailController : Controller
    {
        // GET: Email
        public ActionResult Index()
        {
            return View();
        }

        //public string GetEmailBody()
        //{
        //    var emailBody = RazorHelper.RenderRazorViewToString("Email/AddNewBusiness", this);
        //    return emailBody;
        //}


        //internal string GetEmailBody(VMLead model)
        //{
        //    ViewData.Model = new EBP.Api.Models.EmailModel.LeadEmailModel
        //    {
        //        ManagerName=model.ManagerName,
        //        RepId=model.RepId,
        //        RepName=model.RepName,
        //    };
        //    var emailBody = RazorHelper.RenderRazorViewToString("~/Views/Shared/Email/NewLeadToManager.cshtml", this );
        //    return emailBody;
        //}


        internal string GetEmailBody(VMLead model)
        {
            ViewData.Model = new EBP.Api.Models.EmailModel.LeadEmailModel
            {
                ManagerName = model.ManagerName,
                RepId = model.RepId,
                RepName = model.RepName,
            };
            var emailBody = RazorHelper.RenderRazorViewToString("~/Views/Shared/Email/NewLeadToManager.cshtml", this);
            return emailBody;
        }
    }
}