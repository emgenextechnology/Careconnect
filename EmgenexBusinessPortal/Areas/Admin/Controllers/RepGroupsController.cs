using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EmgenexBusinessPortal.Areas.Admin.Database;

namespace EmgenexBusinessPortal.Areas.Admin.Controllers
{
    [Authorize]
    public class RepGroupsController : Controller
    {
        private EmgenBiz2016Entities db = new EmgenBiz2016Entities();

        // GET: Admin/RepGroups
        public ActionResult Index()
        {
            var lookupRepGroups = db.LookupRepGroups.Include(l => l.LookupRepGroupStatu).Include(l => l.User).Include(l => l.User1).Include(l => l.User2);
            return View(lookupRepGroups.ToList());
        }

        // GET: Admin/RepGroups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupRepGroup lookupRepGroup = db.LookupRepGroups.Find(id);
            if (lookupRepGroup == null)
            {
                return HttpNotFound();
            }
            return View(lookupRepGroup);
        }

        // GET: Admin/RepGroups/Create
        public ActionResult Create()
        {
            ViewBag.StatusId = new SelectList(db.LookupRepGroupStatus, "Id", "StatusType");
            ViewBag.ManagerId = new SelectList(db.Users, "Id", "UserName");
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName");
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        // POST: Admin/RepGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,RepGroupName,Decription,StatusId,ManagerId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupRepGroup lookupRepGroup)
        {
            if (ModelState.IsValid)
            {
                lookupRepGroup.CreatedBy = 1;
                lookupRepGroup.CreatedOn = DateTime.Now;
                db.LookupRepGroups.Add(lookupRepGroup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StatusId = new SelectList(db.LookupRepGroupStatus, "Id", "StatusType", lookupRepGroup.StatusId);
            ViewBag.ManagerId = new SelectList(db.Users, "Id", "UserName", lookupRepGroup.ManagerId);
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupRepGroup.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupRepGroup.UpdatedBy);
            return View(lookupRepGroup);
        }

        // GET: Admin/RepGroups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupRepGroup lookupRepGroup = db.LookupRepGroups.Find(id);
            if (lookupRepGroup == null)
            {
                return HttpNotFound();
            }
            ViewBag.StatusId = new SelectList(db.LookupRepGroupStatus, "Id", "StatusType", lookupRepGroup.StatusId);
            ViewBag.ManagerId = new SelectList(db.Users, "Id", "UserName", lookupRepGroup.ManagerId);
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupRepGroup.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupRepGroup.UpdatedBy);
            return View(lookupRepGroup);
        }

        // POST: Admin/RepGroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,RepGroupName,Decription,StatusId,ManagerId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] LookupRepGroup lookupRepGroup)
        {
            if (ModelState.IsValid)
            {
                lookupRepGroup.UpdatedBy = 1;
                lookupRepGroup.UpdatedOn = DateTime.Now;
                db.Entry(lookupRepGroup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StatusId = new SelectList(db.LookupRepGroupStatus, "Id", "StatusType", lookupRepGroup.StatusId);
            ViewBag.ManagerId = new SelectList(db.Users, "Id", "UserName", lookupRepGroup.ManagerId);
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "UserName", lookupRepGroup.CreatedBy);
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "UserName", lookupRepGroup.UpdatedBy);
            return View(lookupRepGroup);
        }

        // GET: Admin/RepGroups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LookupRepGroup lookupRepGroup = db.LookupRepGroups.Find(id);
            if (lookupRepGroup == null)
            {
                return HttpNotFound();
            }
            return View(lookupRepGroup);
        }

        // POST: Admin/RepGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LookupRepGroup lookupRepGroup = db.LookupRepGroups.Find(id);
            db.LookupRepGroups.Remove(lookupRepGroup);
            db.SaveChanges();
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
