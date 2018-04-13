using EBP.Business.Database;
using EBP.Business.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace EmgenexBusinessPortal.Helpers
{
    public static class LookUps
    {
        public static IEnumerable<SelectListItem> GetRepGroups(int businessId)
        {
            var model = new List<SelectListItem>();

            try
            {
                using (var db = new CareConnectCrmEntities())
                {
                    model = new SelectList(db.RepGroups.Where(a => a.BusinessId == businessId).OrderBy(a => a.RepGroupName), "Id", "RepGroupName").ToList();
                }
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {

            }

            return model;
        }
        public static IEnumerable<SelectListItem> GetSalesDirectors(int businessId)
        {
            var model = new List<SelectListItem>();

            try
            {
                using (var db = new CareConnectCrmEntities())
                {
                    model = new SelectList(db.Users.Where(a => a.BusinessId == businessId && a.IsActive == true
                        && !a.Roles.Select(y => y.Name).Contains("BusinessAdmin")
                        && !a.Roles.Select(y => y.Name).Contains("SuperAdmin")
                        && !a.Roles.Select(y => y.Name).Contains("MasterAdmin")
                        && (a.Roles.Any(r => r.RolePrivileges.Any(rp => rp.Privilege.PrivilegeKey == "MNGALLSLSTMS"))
                        || a.UserPrivileges2.Any(b => b.Privilege.PrivilegeKey == "MNGALLSLSTMS")))
                        .Select(s => new
                        {
                            Id = s.Id,
                            UserName = s.FirstName + " " + s.LastName
                        }).OrderBy(a => a.UserName), "Id", "UserName").ToList();
                }
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {

            }

            return model;
        }
        public static IEnumerable<SelectListItem> GetManagers(int businessId)
        {
            var model = new List<SelectListItem>();

            try
            {
                using (var db = new CareConnectCrmEntities())
                {
                    //model = new SelectList(db.Users.Where(a => a.BusinessId == businessId && a.IsActive == true && a.Roles.Select(b => b.Name.Replace(" ", "").ToLower()).Contains("salesmanager")).OrderBy(a => a.FirstName).Select(s => new
                    model = new SelectList(db.Users.Where(a => a.BusinessId == businessId && a.IsActive == true).OrderBy(a => a.FirstName).Select(s => new
                    {
                        Id = s.Id,
                        UserName = s.FirstName + " " + s.LastName
                    }), "Id", "UserName").ToList();
                }
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {

            }

            return model;
        }
        public static IEnumerable<SelectListItem> GetAllBusiness()
        {
            var model = new List<SelectListItem>();

            try
            {
                using (var db = new CareConnectCrmEntities())
                {
                    model = new SelectList(db.BusinessMasters.OrderBy(a => a.BusinessName), "Id", "BusinessName").ToList();
                }
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {

            }

            return model;
        }
        public static List<SelectListItem> GetSalesColumnType()
        {
            var model = new List<SelectListItem>();
            foreach (SalesColumnType Type in Enum.GetValues(typeof(SalesColumnType)))
            {
                string ColumnType = Regex.Replace(Type.ToString(), "([a-z])([A-Z])", @"$1 $2"); ;
                model.Add(new SelectListItem { Value = ((int)Type).ToString(), Text = ColumnType });
            }
            return model;
        }
        public static List<SelectListItem> GetInputType()
        {
            var model = new List<SelectListItem>();
            foreach (SalesInputType Type in Enum.GetValues(typeof(SalesInputType)))
            {
                string InputType = Regex.Replace(Type.ToString(), "([a-z])([A-Z])", @"$1 $2"); ;
                model.Add(new SelectListItem { Value = ((int)Type).ToString(), Text = InputType });
            }
            return model;
        }
        public static List<SelectListItem> GetImportMode()
        {
            var model = new List<SelectListItem>();
            foreach (ServiceReportImportModes Type in Enum.GetValues(typeof(ServiceReportImportModes)))
            {
                string ColumnType = Regex.Replace(Type.ToString(), "([a-z])([A-Z])", @"$1 $2"); ;
                model.Add(new SelectListItem { Value = ((int)Type).ToString(), Text = ColumnType });
            }
            return model;
        }
        public static List<SelectListItem> GetFtpProtocol()
        {
            var model = new List<SelectListItem>();
            foreach (FTPProtocol Type in Enum.GetValues(typeof(FTPProtocol)))
            {
                string FTPProtocol = Regex.Replace(Type.ToString(), "([a-z])([A-Z])", @"$1 $2"); ;
                model.Add(new SelectListItem { Value = ((int)Type).ToString(), Text = FTPProtocol });
            }
            return model;
        }
    }
}