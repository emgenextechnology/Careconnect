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
using EmgenexBusinessPortal.Helpers;
using EmgenexBusinessPortal.Areas.Business.Models;

namespace EmgenexBusinessPortal.Areas.Business.Controllers
{
    [BusinessAuthorize]
    public class RepGroupsController : BaseController
    {
        // GET: Business/RepGroups
        public ActionResult Index(string SearchKey, int page = 1, bool IsPartial = false)
        {
            var repGroups = db.RepGroups.Where(a => a.BusinessId == CurrentUser.BusinessId).OrderByDescending(a => a.CreatedOn).Include(r => r.BusinessMaster).Include(r => r.User).Include(r => r.User1);

            if (!string.IsNullOrEmpty(SearchKey))
            {
                repGroups = repGroups.Where(ua => ua.RepGroupName.ToLower().Contains(SearchKey.ToLower())
                    || ua.RepgroupManagerMappers.Any(c=>c.User.FirstName.ToLower().Contains(SearchKey.ToLower()))
                    || ua.RepgroupManagerMappers.Any(a => a.User.Email.ToLower().Contains(SearchKey.ToLower()))
                    || ua.Reps.Any(a => a.User2.Email.ToLower().Contains(SearchKey.ToLower()))
                    || ua.RepgroupManagerMappers.Any(c => c.User.Email.ToLower().Contains(SearchKey.ToLower()))
                    || ua.BusinessMaster.BusinessName.ToLower().Contains(SearchKey.ToLower()));
                ViewBag.SearchKey = SearchKey;
            }
            var count = repGroups.Count();
            ViewBag.Count = count;
            ViewBag.page = page;
            var pager = new Pager(count, page, 10);
            ViewBag.StartPage = pager.StartPage;
            ViewBag.EndPage = pager.EndPage;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPage = pager.TotalPages;
            var query = repGroups.OrderByDescending(a => a.CreatedOn).Skip(((page - 1) * pager.PageSize)).Take(pager.PageSize).ToList();
            if (IsPartial)
            {
                return PartialView("_PartialRepGroupList", query);
            }
            return View(query);
        }

        // GET: Business/RepGroups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RepGroup repGroup = db.RepGroups.Find(id);
            if (repGroup == null)
            {
                return HttpNotFound();
            }
            return View(repGroup);
        }

        // GET: Business/RepGroups/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Business/RepGroups/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RepGroupModel repGroup)
        {

            if (ModelState.IsValid)
            {
                var RepGroup = db.RepGroups.Add(new RepGroup
                {
                    BusinessId = CurrentUser.BusinessId,
                    RepGroupName = repGroup.RepGroupName,
                    Description = repGroup.Description,
                    CreatedBy = CurrentUser.Id,
                    CreatedOn = DateTime.UtcNow,
                    IsActive = true,
                    SalesDirectorId = repGroup.SalesDirectorId,
                });
                if (db.SaveChanges() > 0)
                {
                    if (repGroup.ManagerIds.Count() > 0)
                    {
                        foreach (var item in repGroup.ManagerIds)
                        {
                            db.RepgroupManagerMappers.Add(new RepgroupManagerMapper
                            {
                                RepGroupId = RepGroup.Id,
                                ManagerId = (int)item,
                                CreatedBy = CurrentUser.Id,
                                CreatedOn = DateTime.UtcNow
                            });
                            db.SaveChanges();
                        }
                    }
                }
                return RedirectToAction("Index");
            }

            return View(repGroup);
        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RepGroup repGroup = db.RepGroups.Find(id);
            if (repGroup == null)
            {
                return HttpNotFound();
            }

            var managers = db.RepgroupManagerMappers.Where(t => t.RepGroupId == repGroup.Id).Select(a => (int?)a.ManagerId).ToList();
            return View(new RepGroupModel { Id = repGroup.Id, RepGroupName = repGroup.RepGroupName, Description = repGroup.Description, IsActive = repGroup.IsActive, SalesDirectorId = repGroup.SalesDirectorId, ManagerIds = managers });
        }

        // POST: Business/RepGroups/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RepGroupModel repGroup)
        {

            if (ModelState.IsValid)
            {
                var repGroupModel = db.RepGroups.FirstOrDefault(a => a.Id == repGroup.Id);
                repGroupModel.RepGroupName = repGroup.RepGroupName;
                repGroupModel.Description = repGroup.Description;
                repGroupModel.IsActive = repGroup.IsActive;
                repGroupModel.UpdatedBy = CurrentUser.Id;
                repGroupModel.SalesDirectorId = repGroup.SalesDirectorId > 0 ? repGroup.SalesDirectorId : null;
                repGroupModel.UpdatedOn = System.DateTime.UtcNow;
                var ManagerIds = db.RepgroupManagerMappers.Where(t => t.RepGroupId == repGroup.Id).Select(a => a.ManagerId).ToList();

                foreach (var item in ManagerIds)
                {
                    var delete = db.RepgroupManagerMappers.Where(a => a.RepGroupId == repGroup.Id & a.ManagerId == item).FirstOrDefault();
                    db.RepgroupManagerMappers.Remove(delete);
                    db.SaveChanges();
                }
                if (repGroup.ManagerIds != null)
                {
                    foreach (var item in repGroup.ManagerIds)
                    {
                        if (repGroup.ManagerIds.Count() > 0)
                        {
                            db.RepgroupManagerMappers.Add(new RepgroupManagerMapper
                            {
                                ManagerId = (int)item,
                                RepGroupId = repGroup.Id,
                                CreatedBy = CurrentUser.Id,
                                CreatedOn = System.DateTime.UtcNow,
                                UpdatedBy = CurrentUser.Id,
                                UpdatedOn = System.DateTime.UtcNow,
                            });
                            db.SaveChanges();
                        }
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(repGroup);
        }

        // GET: Business/RepGroups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RepGroup repGroup = db.RepGroups.Find(id);
            if (repGroup == null)
            {
                return HttpNotFound();
            }
            return View(repGroup);
        }

        // POST: Business/RepGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RepGroup repGroup = db.RepGroups.Find(id);
            try
            {
                db.RepGroups.Remove(repGroup);
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("Error", "There are some releted item in database, please delete those first");
                return View("Delete", repGroup);
            }
            return RedirectToAction("Index");
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
