using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EBP.Business.Database;
using System.Data.Entity.Infrastructure;
using EmgenexBusinessPortal.Areas.Business.Models;
using System.Globalization;
using EmgenexBusinessPortal.Helpers;

namespace EmgenexBusinessPortal.Areas.Business.Controllers
{
    [BusinessAuthorize]
    public class RepsController : BaseController
    {
        // GET: Business/Reps
        public ActionResult Index(string SearchKey, int page = 1, bool IsPartial = false)
        {
            var reps = db.Reps.Where(a => a.User2.BusinessId == CurrentBusinessId).Include(r => r.RepGroup).Include(r => r.User).Include(r => r.User1).Include(r => r.User2);
            if (!string.IsNullOrEmpty(SearchKey))
            {
                reps = reps.Where(ua => ua.User2.FirstName.ToLower().Contains(SearchKey.ToLower())
                    || ua.User2.LastName.ToLower().Contains(SearchKey.ToLower())
                    || (ua.User2.FirstName + " " + ua.User2.MiddleName).ToLower().Contains(SearchKey.ToLower())
                    || (ua.User2.FirstName + " " + ua.User2.LastName).ToLower().Contains(SearchKey.ToLower())
                    || (ua.User2.FirstName + " " + ua.User2.MiddleName + " " + ua.User2.LastName).ToLower().Contains(SearchKey.ToLower())
                    || ua.User2.UserName.ToLower().Contains(SearchKey.ToLower())
                    || ua.RepGroup.RepGroupName.ToLower().Contains(SearchKey.ToLower()));
                ViewBag.SearchKey = SearchKey;
            }
            var count = reps.Count();
            ViewBag.Count = count;
            ViewBag.page = page;
            var pager = new Pager(count, page, 10);
            ViewBag.StartPage = pager.StartPage;
            ViewBag.EndPage = pager.EndPage;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPage = pager.TotalPages;
            var query = reps.OrderByDescending(a => a.CreatedOn).Skip(((page - 1) * pager.PageSize)).Take(pager.PageSize).ToList();
            if (IsPartial)
            {
                return PartialView("_PartialRepsList", query);
            }
            return View(query);
        }

        // GET: Business/Reps/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rep rep = db.Reps.Find(id);
            if (rep == null)
            {
                return HttpNotFound();
            }
            return View(rep);
        }
        // GET: Business/Reps/Create
        public ActionResult Create()
        {
            var RepIds = db.Reps.Select(a => a.UserId);
            ViewBag.UserId = new SelectList(db.Users.Where(a => a.BusinessId == CurrentBusinessId && a.IsActive == true && a.Id != CurrentUserId && !RepIds.Contains(a.Id)).Select(s => new
            {
                Id = s.Id,
                UserName = s.FirstName + " " + s.LastName
            }), "Id", "UserName");
            ViewBag.Services = new SelectList(db.LookupEnrolledServices.Where(a => a.BusinessId == CurrentBusinessId && a.IsActive == true), "Id", "ServiceName");
            return View();
        }

        // POST: Business/Reps/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RepModel rep, params int[] selectedServices)
        {
            if (ModelState.IsValid)
            {
                var Reps = db.Reps.Add(new Rep
                {
                    UserId = rep.UserId,
                    RepGroupId = rep.RepGroupId,
                    IsActive = true,
                    CreatedBy = CurrentUserId,
                    CreatedOn = System.DateTime.UtcNow,
                    //SignonDate = string.IsNullOrEmpty(rep.SignonDate) ? (DateTime?)null : DateTime.ParseExact(rep.SignonDate, "MM-dd-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None)
                });
                if (db.SaveChanges() > 0)
                {
                    if (selectedServices != null && selectedServices.Count() > 0)
                    {
                        foreach (var item in selectedServices)
                        {
                            db.RepServiceMappers.Add(new RepServiceMapper { ServiceId = item, RepId = Reps.Id, CreatedBy = CurrentUser.Id, CreatedOn = DateTime.UtcNow });
                            db.SaveChanges();
                        }
                    }
                }
                return RedirectToAction("Index");
            }
            var RepIds = db.Reps.Select(a => a.UserId);
            ViewBag.UserId = new SelectList(db.Users.Where(a => a.BusinessId == CurrentBusinessId && a.IsActive == true && a.Id != CurrentUserId && !RepIds.Contains(a.Id)).Select(s => new
            {
                Id = s.Id,
                UserName = s.FirstName + " " + s.LastName + " @" + s.UserName
            }), "Id", "UserName");
            ViewBag.Services = new SelectList(db.LookupEnrolledServices.Where(a => a.BusinessId == CurrentBusinessId && a.IsActive == true), "Id", "ServiceName");
            return View(rep);
        }
        // GET: Business/Reps/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rep rep = db.Reps.Find(id);
            var RepIds = db.Reps.Where(a => a.Id != id).Select(a => a.UserId);
            if (rep == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.Users.Where(a => a.BusinessId == CurrentBusinessId && a.IsActive == true && a.Id != CurrentUserId && !RepIds.Contains(a.Id)).Select(s => new
            {
                Id = s.Id,
                UserName = s.FirstName + " " + s.LastName + " @" + s.UserName
            }).ToList(), "Id", "UserName", rep.UserId).OrderBy(b => b.Text);
            var ServiceList = db.RepServiceMappers.Where(a => a.RepId == id).Select(a => a.ServiceId);
            ViewBag.Services = db.LookupEnrolledServices.Where(a => a.BusinessId == CurrentBusinessId && a.IsActive == true).Select(x => new SelectListItem()
            {
                Selected = ServiceList.Contains(x.Id),
                Text = x.ServiceName,
                Value = x.Id.ToString()
            });
            return View(new RepModel
            {
                Id = rep.Id,
                //SignonDate =rep.SignonDate ==null?null: rep.SignonDate.GetValueOrDefault().ToString("MM-dd-yyyy"),
                RepGroupId = rep.RepGroupId,
                UserId = rep.UserId,
                IsActive = rep.IsActive
            });
        }

        // POST: Business/Reps/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RepModel rep, params int[] selectedServices)
        {
            var RepIds = db.Reps.Where(a => a.Id != rep.Id).Select(a => a.UserId);
            if (ModelState.IsValid)
            {
                var model = db.Reps.Find(rep.Id);
                model.UserId = rep.UserId;
                model.RepGroupId = rep.RepGroupId;
                model.IsActive = rep.IsActive;
                model.UpdatedBy = CurrentUserId;
                model.UpdatedOn = System.DateTime.UtcNow;
                //if (!string.IsNullOrEmpty(rep.SignonDate))
                //{ 
                //    model.SignonDate = DateTime.ParseExact(rep.SignonDate, "MM-dd-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);
                //}
                //else
                //{
                //    model.SignonDate = null;
                //}
                if (db.SaveChanges() > 0)
                {
                    IEnumerable<RepServiceMapper> Services = db.RepServiceMappers.Where(a => a.RepId == rep.Id).ToList();
                    if (Services.Count() > 0)
                    {
                        db.RepServiceMappers.RemoveRange(Services);
                        db.SaveChanges();
                    }
                    if (selectedServices != null && selectedServices.Count() > 0)
                    {
                        foreach (var item in selectedServices)
                        {
                            db.RepServiceMappers.Add(new RepServiceMapper { RepId = rep.Id, ServiceId = item, CreatedBy = CurrentUser.Id, CreatedOn = DateTime.UtcNow });
                            db.SaveChanges();
                        }
                    }
                }
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.Users.Where(a => a.BusinessId == CurrentBusinessId && a.IsActive == true && a.Id != CurrentUserId && !RepIds.Contains(a.Id)).Select(s => new
            {
                Id = s.Id,
                UserName = s.FirstName + " " + s.LastName + " @" + s.UserName
            }), "Id", "UserName");
            var ServiceList = db.RepServiceMappers.Where(a => a.RepId == rep.Id).Select(a => a.ServiceId);
            ViewBag.Services = db.LookupEnrolledServices.Where(a => a.BusinessId == CurrentBusinessId && a.IsActive == true).Select(x => new SelectListItem()
            {
                Selected = ServiceList.Contains(x.Id),
                Text = x.ServiceName,
                Value = x.Id.ToString()
            });
            return View(rep);
        }

        // GET: Business/Reps/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rep rep = db.Reps.Find(id);
            if (rep == null)
            {
                return HttpNotFound();
            }
            return View(rep);
        }

        // POST: Business/Reps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Rep rep = db.Reps.Find(id);
            try
            {
                db.RepServiceMappers.RemoveRange(db.RepServiceMappers.Where(u => u.RepId == id));
                //db.SaveChanges();
                var DeleteItem = db.Reps.Remove(rep);
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("Error", "There are some releted item in database, please delete those first");
                rep = db.Reps.Find(id);
                return View("Delete", rep);
            }
            return RedirectToAction("Index");
        }

        //public string GetManagerName(int id)
        //{
        //    var objManager = db.RepGroups.Where(a => a.Id == id).FirstOrDefault();
        //    int managerId = 0;
        //    string managerName = string.Empty;
        //    if (objManager != null)
        //    {
        //        managerId = objManager.ManagerId.HasValue ? objManager.ManagerId.Value : 0;
        //    }

        //    var objUser = db.Users.Where(a => a.Id == managerId).FirstOrDefault();
        //    if (objUser != null)
        //    {
        //        managerName = string.Format("{0} {1}", objUser.FirstName, objUser.LastName);
        //    }
        //    return managerName;
        //}

        public string GetManagerIds(int id)
        {
            var objManager = db.RepGroups.Where(a => a.Id == id).FirstOrDefault();
            List<int> managerIds = new List<int>();
            string managerName = string.Empty;
            if (objManager != null)
            {
                managerIds = db.RepgroupManagerMappers.Where(t => t.RepGroupId == id).Select(a => a.ManagerId).ToList();
            }

            return string.Join(",", managerIds);
        }

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
