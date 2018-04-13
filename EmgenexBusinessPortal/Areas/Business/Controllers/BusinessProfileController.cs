using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EBP.Business.Database;
using System.IO;
using System.Globalization;
using EmgenexBusinessPortal.Areas.Business.Models;
using System.Text.RegularExpressions;

namespace EmgenexBusinessPortal.Areas.Business.Controllers
{
    [BusinessAuthorize]
    public class BusinessProfileController : BaseController
    {
        //private EmegenexBiz2016Entities db = new EmegenexBiz2016Entities();

        // GET: Business/BusinessProfile/Edit
        public ActionResult Edit()
        {
            if (CurrentBusinessId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusinessMaster businessMaster = db.BusinessMasters.Find(CurrentBusinessId);
            if (businessMaster == null)
            {
                return HttpNotFound();
            }
            if (businessMaster.OtherEmails != null)
            {
                businessMaster.OtherEmails = "[" + string.Join(",", businessMaster.OtherEmails.Split(',').Select(x => string.Format("\"{0}\"", x)).ToList()) + "]";
            }

            //var model = new BusinessProfileModel
            //{
            //    Id = businessMaster.Id,
            //    BusinessName = businessMaster.BusinessName,
            //    Description = businessMaster.Description,
            //    DomainUrl = businessMaster.DomainUrl,
            //    RelativeUrl = businessMaster.RelativeUrl,
            //    DateFrom = businessMaster.DateFrom == null ? null : businessMaster.DateFrom.GetValueOrDefault().ToString("MM-dd-yyyy"),
            //    DateTo = businessMaster.DateTo == null ? null : businessMaster.DateTo.GetValueOrDefault().ToString("MM-dd-yyyy"),
            //    IsActive = businessMaster.IsActive,
            //    State = businessMaster.State,
            //    City = businessMaster.City,
            //    Country = businessMaster.Country,
            //    Status = businessMaster.Status,
            //    About = businessMaster.About,
            //    Address = businessMaster.Address
            //};
            return View(businessMaster);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BusinessName,OtherEmails,Description,DomainUrl,RelativeUrl,Address,Logo,Country,State,City,About,IsActive,Status,CreatedBy,CreatedOn,UpdatedBy,UpdatedOn,DateRange,SalesGroupBy")] BusinessMaster businessMaster)
        {

            if (ModelState.IsValid)
            {
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];

                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = "Logo_" + businessMaster.Id + ".jpg";

                        DirectoryInfo dir = new DirectoryInfo(Server.MapPath("~/Assets/" + businessMaster.Id));

                        if (!dir.Exists)
                            dir.Create();

                        var path = Path.Combine(Server.MapPath("~/Assets/" + businessMaster.Id), fileName);
                        businessMaster.Logo = fileName;
                        file.SaveAs(path);
                    }
                }
                if (!string.IsNullOrEmpty(businessMaster.DomainUrl))
                { businessMaster.DomainUrl = businessMaster.DomainUrl.Replace(" ", "-"); }
                businessMaster.RelativeUrl = businessMaster.RelativeUrl.Replace(" ", "-");
                businessMaster.OtherEmails = businessMaster.OtherEmails == null ? null : Regex.Replace(businessMaster.OtherEmails, @"[\[\]\""]+", "");
                businessMaster.OtherEmails = string.IsNullOrEmpty(businessMaster.OtherEmails) ? null : businessMaster.OtherEmails;
                businessMaster.UpdatedBy = CurrentUserId;
                businessMaster.UpdatedOn = System.DateTime.UtcNow;
                db.Entry(businessMaster).State = EntityState.Modified;
                if (db.SaveChanges() > 0)
                    ModelState.AddModelError("Result", "Sucessfully Saved");
                return View(businessMaster);
            }
            return View(businessMaster);
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(BusinessProfileModel businessMaster)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        if (Request.Files.Count > 0)
        //        {
        //            var file = Request.Files[0];

        //            if (file != null && file.ContentLength > 0)
        //            {
        //                var fileName = "Logo_" + businessMaster.Id + ".jpg";

        //                DirectoryInfo dir = new DirectoryInfo(Server.MapPath("~/Assets/" + businessMaster.Id));

        //                if (!dir.Exists)
        //                    dir.Create();

        //                var path = Path.Combine(Server.MapPath("~/Assets/" + businessMaster.Id), fileName);
        //                businessMaster.Logo = fileName;
        //                file.SaveAs(path);
        //            }
        //        }
        //        BusinessMaster model = db.BusinessMasters.Find(businessMaster.Id);
        //        if (model != null)
        //        {
        //            model.DomainUrl = businessMaster.DomainUrl == null ? null : businessMaster.DomainUrl.Replace(" ", "-");
        //            model.BusinessName = businessMaster.BusinessName;
        //            model.Description = businessMaster.Description;
        //            model.City = businessMaster.City;
        //            model.State = businessMaster.State;
        //            model.Country = businessMaster.Country;
        //            model.IsActive = businessMaster.IsActive;
        //            model.Address = businessMaster.Address;
        //            model.About = businessMaster.About;
        //            model.RelativeUrl = businessMaster.RelativeUrl.Replace(" ", "-");
        //            model.UpdatedBy = CurrentUserId;
        //            model.UpdatedOn = System.DateTime.UtcNow;
        //            model.DateFrom = string.IsNullOrEmpty(businessMaster.DateFrom) ? (DateTime?)null : DateTime.ParseExact(businessMaster.DateFrom, "MM-dd-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);
        //            model.DateTo = string.IsNullOrEmpty(businessMaster.DateTo) ? (DateTime?)null : DateTime.ParseExact(businessMaster.DateTo, "MM-dd-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);

        //            if (db.SaveChanges() > 0)
        //                ModelState.AddModelError("Result", "Sucessfully Saved");
        //            return View(businessMaster);
        //        }
        //    }
        //    return View(businessMaster);
        //}
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
