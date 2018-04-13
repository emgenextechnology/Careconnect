using EBP.Business;
using EBP.Business.Database;
using EBP.Business.Entity.Account;
using EBP.Business.Entity.Practice;
using EBP.Business.Enums;
using EBP.Business.Filter;
using EBP.Business.Repository;
using EmgenexBusinessPortal.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace EmgenexBusinessPortal.Areas.Business.Controllers
{
    [BusinessAuthorize]
    public class AddressController : BaseController
    {
        public ActionResult Index(string SearchKey, int page = 1, bool IsPartial = false)
        {

            string[] superRoles = { "EDTADSLOC" };
            bool hasSuperRight = HasRight(superRoles);

            if (!hasSuperRight)
                return RedirectToAction("Index", "BusinessDashboard");

            var Addresses = db.PracticeAddressMappers.Where(a => a.Practice.BusinessId == CurrentBusinessId).Select(s => new EntityPracticeAddress
            {
                Id = s.Address.Id,
                PracticeName = s.Practice.PracticeName,
                TypeId = s.Address.AddressTypeId,
                LocationId = s.Address.LocationId,
                Line1 = s.Address.Line1,
                Line2 = s.Address.Line2,
                City = s.Address.City,
                Zip = s.Address.Zip,
                AddressTypeId = s.Address.AddressTypeId,
                StateId = s.Address.StateId,
                State = s.Address.LookupState.StateName,
                Phone = s.Address.Phones.Select(p => new EntityPracticePhone
                {
                    PhoneId = p.Id,
                    PhoneNumber = p.PhoneNumber,
                    Extension = p.Extension
                })
            }).ToList();
            Addresses.ForEach(a => a.AddressType = EnumHelper.GetEnumName<AddressType>(a.AddressTypeId));
            if (!string.IsNullOrEmpty(SearchKey))
            {
                Addresses = Addresses.Where(ua =>
                    (ua.LocationId != null && ua.LocationId.ToLower().Contains(SearchKey.ToLower())) ||
                    ua.PracticeName.ToLower().Contains(SearchKey.ToLower()) ||
                    ua.Line1.ToLower().Contains(SearchKey.ToLower()) ||
                    ua.AddressType.ToLower().Contains(SearchKey.ToLower()) ||
                    ua.City.ToLower().Contains(SearchKey.ToLower()) ||
                    ua.State.ToLower().Contains(SearchKey.ToLower()) ||
                    ua.Zip.ToLower().Contains(SearchKey.ToLower())
                    ).ToList();
                ViewBag.SearchKey = SearchKey;
            }
            var count = Addresses.Count();
            ViewBag.Count = count;
            ViewBag.page = page;
            var pager = new Pager(count, page, 10);
            ViewBag.StartPage = pager.StartPage;
            ViewBag.EndPage = pager.EndPage;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPage = pager.TotalPages;
            var query = Addresses.OrderByDescending(a => a.Id).Skip(((page - 1) * pager.PageSize)).Take(pager.PageSize).ToList();
            if (IsPartial)
            {
                return PartialView("_PartialAddressList", query);
            }
            return View(query);
        }

        public ActionResult AddLocationId(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Address Address = db.Addresses.Find(id);
            if (Address == null)
            {
                return HttpNotFound();
            }

            return View(new EntityPracticeAddress
            {
                PracticeName = Address.PracticeAddressMappers.FirstOrDefault().Practice.PracticeName,
                AddressType = EnumHelper.GetEnumName<AddressType>(Address.AddressTypeId),
                Line1 = Address.Line1,
                Line2 = Address.Line2,
                LocationId = Address.LocationId,
                City = Address.City,
                State = Address.LookupState.StateName,
                Zip = Address.Zip
            });
        }

        [HttpPost]
        public ActionResult AddLocationId(EntityPracticeAddress Address)
        {
            var addressModel = db.Addresses;
            var model = db.Addresses.FirstOrDefault(a => a.Id == Address.Id);
            if (!string.IsNullOrEmpty(Address.LocationId) && db.Addresses.FirstOrDefault(a => a.Id != Address.Id && a.LocationId == Address.LocationId) != null)
            {
                ModelState.AddModelError("Error", "Already Used");
                return View(Address);
            }
            if (model != null)
            {
                model.LocationId = Address.LocationId;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

    }
}