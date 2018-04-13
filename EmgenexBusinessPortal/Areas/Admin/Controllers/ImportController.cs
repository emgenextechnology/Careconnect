using EBP.Business.Database;
using EBP.Business.Helpers;
using EmgenexBusinessPortal.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;
using EBP.Business.Enums;
using System.Threading.Tasks;
using System.Dynamic;
using System.Text;
using System.Collections;
using GM.Identity.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using EmgenexBusinessPortal.Models;
using GM.Identity;
using System.Globalization;
using System.Configuration;
using EBP.Business;
using GM.Identity.Config;

namespace EmgenexBusinessPortal.Areas.Admin.Controllers
{
    public class ImportController : BaseController
    {
        public ImportController()
        {
        }

        public ImportController(GMUserManager userManager, GMRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        private GMUserManager _userManager;
        public GMUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<GMUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private GMRoleManager _roleManager;
        public GMRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<GMRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        private string MapperFilePath
        {
            get
            {
                var mapperFilePath = Server.MapPath(Path.Combine("~/MappingConfig", "UserMapper.xml"));

                if (!System.IO.File.Exists(mapperFilePath))
                    return null;

                return mapperFilePath;
            }
        }

        // GET: Admin/Import
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index(ViewModel model)
        {
            int businessId = model.BusinessId.Value,
                currentUserId = CurrentUser.Id;
            if (model.IsAccount)
            {
                #region AccountImport

                if (ModelState.IsValid)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        model.File.InputStream.CopyTo(memoryStream);
                    }

                    if (model.File != null)
                    {
                        string filePath = Server.MapPath(Path.Combine("~/Assets", businessId.ToString(), "Sales", "Sales-Archives", "Uploads", "Imports"));
                        if (!Directory.Exists(filePath))
                            Directory.CreateDirectory(filePath);
                        string excelFile = Path.Combine(filePath, string.Format("{0}.{1}", DateTime.Now.ToString("MMddyyhhmmssttfff"), "xlsx"));

                        model.File.SaveAs(excelFile);

                        int RecordCount;
                        XmlDocument xmlDocument = new XmlDocument();
                        using (StreamReader sr = new StreamReader(excelFile))
                        {
                            Stream ExcelStream = sr.BaseStream;
                            xmlDocument.LoadXml(new ExcelToXml().GetXMLString(ref ExcelStream, true, out RecordCount));
                            IEnumerable<XElement> xElements = xmlDocument.ToXDocument().Descendants("RowItem");

                            var accountData = xElements.Select(u =>
                                new AccountModel
                                {
                                    PracticeName = (string)u.Element("PracticeName"),
                                    AddressLine1 = (string)u.Element("AddressLine1"),
                                    AddressLine2 = (string)u.Element("AddressLine2"),
                                    City = (string)u.Element("City"),
                                    State = (string)u.Element("State"),
                                    ZipCode = (string)u.Element("ZipCode"),
                                    PhoneNumber = (string)u.Element("PhoneNumber"),
                                    ReportDeliveryFax = (string)u.Element("ReportDeliveryFax"),
                                    EnrolledServices = (string)u.Element("EnrolledServices"),
                                    NPI = (string)u.Element("NPI"),
                                    SalesTeam = (string)u.Element("SalesTeam"),
                                    SalesRep = (string)u.Element("SalesRep"),
                                });

                            foreach (var item in accountData)
                            {
                                try
                                {
                                    using (CareConnectCrmEntities DBEntity = new CareConnectCrmEntities())
                                    {
                                        #region Genrarate PracticeProviderMapper

                                        List<PracticeProviderMapper> oPracticeProviderMappers = new List<PracticeProviderMapper>();
                                        var provider = DBEntity.Providers.FirstOrDefault(a => a.NPI == item.NPI);
                                        int providerId = provider != null ? provider.Id : 0;
                                        if (providerId > 0)
                                        {
                                            oPracticeProviderMappers.Add(new PracticeProviderMapper
                                            {
                                                AddressIndex = 1,
                                                ProviderId = providerId,
                                                CreatedBy = currentUserId,
                                                CreatedOn = DateTime.UtcNow
                                            });
                                            item.HasProvider = true;
                                        }
                                        else
                                        {
                                            dynamic obj = null;
                                            using (HttpClient httpClient = new HttpClient())
                                            {
                                                var json = await GetObjectsAsync(item.NPI);
                                                var serializer = new JavaScriptSerializer();
                                                serializer.RegisterConverters(new[] { new DynamicJsonConverter() });

                                                obj = serializer.Deserialize(json.ToString(), typeof(object));
                                            }

                                            int resultCount = 0;
                                            //var s = obj.result_count;
                                            int.TryParse(Convert.ToString(obj.result_count), out resultCount);
                                            if (resultCount > 0)
                                            {
                                                string firstName = string.Empty, middleName = string.Empty, lastName = string.Empty,
                                                    enumerationType = obj.results[0].enumeration_type;
                                                switch (enumerationType)
                                                {
                                                    case "NPI-1":
                                                        try { firstName = obj.results[0].basic.first_name; }
                                                        catch { }
                                                        try { middleName = obj.results[0].basic.middle_name; }
                                                        catch { }
                                                        try { lastName = obj.results[0].basic.last_name; }
                                                        catch { }
                                                        break;
                                                    case "NPI-2":
                                                        try { firstName = obj.results[0].basic.authorized_official_first_name; }
                                                        catch { }
                                                        try { middleName = obj.results[0].basic.authorized_official_middle_name; }
                                                        catch { }
                                                        try { lastName = obj.results[0].basic.authorized_official_last_name; }
                                                        catch { }
                                                        break;
                                                }
                                                oPracticeProviderMappers.Add(new PracticeProviderMapper
                                                {
                                                    AddressIndex = 1,
                                                    Provider = new Provider
                                                    {
                                                        FirstName = firstName,
                                                        MiddleName = middleName,
                                                        LastName = lastName,
                                                        IsActive = true,
                                                        NPI = item.NPI,
                                                        CreatedBy = currentUserId,
                                                        CreatedOn = DateTime.UtcNow,
                                                    },
                                                    CreatedBy = currentUserId,
                                                    CreatedOn = DateTime.UtcNow
                                                });
                                                item.HasProvider = true;
                                            }
                                        }

                                        #endregion

                                        #region Genrarate PracticeServiceMapper

                                        List<PracticeServiceMapper> oPracticeServiceMapper = new List<PracticeServiceMapper>();

                                        var services = DBEntity.LookupEnrolledServices.Where(a => a.BusinessId == businessId).ToList();

                                        if (services != null && services.Count() > 0)
                                        {
                                            foreach (var serviceItem in services)
                                            {
                                                oPracticeServiceMapper.Add(new PracticeServiceMapper
                                                {
                                                    EnrolledServiceId = Convert.ToInt32(serviceItem.Id),
                                                    CreatedBy = currentUserId,
                                                    CreatedOn = DateTime.UtcNow
                                                });
                                            }
                                        }

                                        #endregion

                                        if (!string.IsNullOrEmpty(item.SalesRep))
                                        {
                                            string salesRep = item.SalesRep.Replace(" ", "").ToLower(),
                                                repGroup = item.SalesTeam.Replace(" ", "").ToLower();

                                            var objRep = DBEntity.Reps.Where(a => a.User2.BusinessId == businessId && (a.User2.FirstName.Trim() + a.User2.MiddleName.Trim() + a.User2.LastName.Trim()).ToLower().Contains(salesRep) && a.RepGroup.RepGroupName.Replace(" ", "").ToLower().Trim() == repGroup).ToList();
                                            if (objRep.Count == 1)
                                            {
                                                item.RepId = objRep.FirstOrDefault().Id;
                                            }
                                            else
                                            {
                                                model.FailedRecords.Add(item);
                                                continue;
                                            }
                                        }

                                        if (!string.IsNullOrEmpty(item.State))
                                        {
                                            string stateCode = item.State.Trim();
                                            var objState = DBEntity.LookupStates.Where(a => a.StateCode == stateCode).ToList();
                                            if (objState.Count == 1)
                                            {
                                                item.StateId = objState.FirstOrDefault().Id;
                                            }
                                        }

                                        if (item.RepId <= 0 || item.StateId <= 0 || !item.HasProvider.HasValue)
                                        {
                                            model.FailedRecords.Add(item);
                                            continue;
                                        }

                                        #region Accounts

                                        var accountModel = new Account
                                        {
                                            IsActive = true,
                                            BusinessId = businessId,
                                            Lead = new Lead
                                            {
                                                IsConverted = true,
                                                IsActive = true,
                                                RepId = item.RepId,
                                                LeadStatus = (int)LeadStatus.Transacted,
                                                BusinessId = businessId,
                                                Practice = new Practice
                                                {
                                                    ReportDeliveryFax = item.ReportDeliveryFax,
                                                    RepId = item.RepId,
                                                    PracticeName = item.PracticeName,
                                                    BusinessId = businessId,
                                                    PracticeAddressMappers = new List<PracticeAddressMapper> {
                                                    new PracticeAddressMapper
                                                    {
                                                        Address = new Address
                                                        {
                                                            AddressIndex = 1,
                                                            Line1 = item.AddressLine1,
                                                            Line2 = item.AddressLine2,
                                                            City = item.City,
                                                            Zip = item.ZipCode,
                                                            AddressTypeId = (int)AddressType.Primary,
                                                            StateId = item.StateId.HasValue ? item.StateId.Value : 0,
                                                            CreatedBy = currentUserId,
                                                            CreatedOn = DateTime.UtcNow,
                                                            Phones = new List<Phone>
                                                            {
                                                                new Phone
                                                                {
                                                                    PhoneNumber = item.PhoneNumber,
                                                                    PhoneTypeId = 1,
                                                                    CreatedOn = DateTime.UtcNow,
                                                                    CreatedBy = currentUserId
                                                                }
                                                            },
                                                        },
                                                        CreatedBy = currentUserId,
                                                        CreatedOn = DateTime.UtcNow,
                                                    }
                                                },
                                                    PracticeProviderMappers = oPracticeProviderMappers,
                                                    PracticeServiceMappers = oPracticeServiceMapper,
                                                    CreatedOn = DateTime.UtcNow,
                                                    CreatedBy = currentUserId
                                                },
                                                CreatedOn = DateTime.UtcNow,
                                                CreatedBy = currentUserId
                                            },
                                            CreatedOn = DateTime.UtcNow,
                                            CreatedBy = currentUserId
                                        };

                                        accountModel = DBEntity.Accounts.Add(accountModel);

                                        #endregion

                                        if (DBEntity.SaveChanges() > 0)
                                        {
                                            #region Practice Provider Address Mapper

                                            accountModel.Lead.Practice.PracticeProviderMappers.ToList().ForEach(s =>
                                                DBEntity.PracticeProviderLocationMappers.Add(new PracticeProviderLocationMapper
                                                {
                                                    PracticeId = accountModel.Lead.Practice.Id,
                                                    ProviderId = s.ProviderId,
                                                    AddressId = s.Practice.PracticeAddressMappers
                                                                    .FirstOrDefault(f => f.Address.AddressIndex == s.Practice.PracticeProviderMappers
                                                                        .FirstOrDefault(d => d.Provider.Id == s.ProviderId).AddressIndex).AddressId,
                                                    CreatedBy = currentUserId,
                                                    CreatedOn = DateTime.UtcNow
                                                }));

                                            #endregion

                                            DBEntity.SaveChanges();
                                        }
                                        else
                                        {
                                            model.FailedRecords.Add(item);
                                        }
                                    }
                                }
                                catch (System.Data.Entity.Validation.DbEntityValidationException e)
                                {
                                    foreach (var eve in e.EntityValidationErrors)
                                    {
                                        foreach (var ve in eve.ValidationErrors)
                                        {
                                            string s = string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    model.FailedRecords.Add(item);
                                    ex.Log();
                                }
                            }

                            ExcelStream.Close();
                        };
                    }
                }
                return View(model);
                #endregion
            }
            else
            {
                #region UserImport
                var returnModel = await ImportUser(model, businessId, currentUserId);
                return View(returnModel);
                #endregion
            }
        }

        public async Task<ViewModel> ImportUser(ViewModel model, int businessId, int currentUserId)
        {
            if (Request.Files != null && Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                var fileName = file.FileName;
                string rootPath = Server.MapPath("~/Assets"),
                fileNameWithoutExt = string.Format("{0:yyyyMMddhhmmssfff}", DateTime.Now),
                extension = Path.GetExtension(fileName);
                fileName = string.Format("{0}{1}", fileNameWithoutExt, extension);
                string fileDirectory = Path.Combine(rootPath, businessId.ToString(), "Users", "Uploads");
                if (!Directory.Exists(fileDirectory))
                    Directory.CreateDirectory(fileDirectory);
                string fullPath = Path.Combine(fileDirectory, fileName);

                int count = 1;
                isExist:
                if (System.IO.File.Exists(fullPath))
                {
                    fileName = string.Format("{0}{1}{2}", fileNameWithoutExt, count, extension);
                    fullPath = Path.Combine(fileDirectory, fileName);
                    count++;
                    goto isExist;
                }
                file.SaveAs(fullPath);

                XmlHelper xmlHelper = new XmlHelper();

                string excelFile = Server.MapPath(Path.Combine("~/Assets", businessId.ToString(), "Users", "Uploads", fileName.ToString()));

                xmlHelper.XmlMapper = XDocument.Load(this.MapperFilePath);

                int RecordCount;
                using (StreamReader sr = new StreamReader(excelFile))
                {
                    Stream ExcelStream = sr.BaseStream;
                    xmlHelper.xmlString = new ExcelToXml().GetXMLString(ref ExcelStream, true, out RecordCount);
                    ExcelStream.Close();
                };
                XDocument XmlMapper = xmlHelper.XmlMapper;
                var xmlUserData = XmlMapper.Descendants("Users");
                for (int i = 0; i < RecordCount; i++)
                {
                    var xmlUsers = xmlUserData.Select(u => new XMLRegisterModel
                    {
                        FirstName = xmlHelper.GetNodeValue(ref u, "FirstName").Value,
                        MiddleName = xmlHelper.GetNodeValue(ref u, "MiddleName").Value,
                        LastName = xmlHelper.GetNodeValue(ref u, "LastName").Value,
                        Email = xmlHelper.GetNodeValue(ref u, "Email").Value,
                        PhoneNumber = xmlHelper.GetNodeValue(ref u, "Phone").Value,
                        WorkEmail = xmlHelper.GetNodeValue(ref u, "WorkEmail").Value,
                        AdditionalPhone = xmlHelper.GetNodeValue(ref u, "AdditionalPhone").Value,
                        AddressLine1 = xmlHelper.GetNodeValue(ref u, "AddressLine1").Value,
                        AddressLine2 = xmlHelper.GetNodeValue(ref u, "AddressLine2").Value,
                        City = xmlHelper.GetNodeValue(ref u, "City").Value,
                        State = xmlHelper.GetNodeValue(ref u, "State").Value,
                        Zip = xmlHelper.GetNodeValue(ref u, "Zip").Value,
                        UserName = xmlHelper.GetNodeValue(ref u, "Email").Value,
                        RoleName = xmlHelper.GetNodeValue(ref u, "Role").Value,
                        DepartmentName = xmlHelper.GetNodeValue(ref u, "Department").Value,
                        StartDate = xmlHelper.GetNodeValue(ref u, "StartDate").Value,
                        SalesTeam = xmlHelper.GetNodeValue(ref u, "SalesTeam").Value,
                        Services = xmlHelper.GetNodeValue(ref u, "Services").Value,
                    }).FirstOrDefault();

                    if (xmlUsers != null)
                    {
                        #region Create Role
                        xmlUsers.RoleName = xmlUsers.RoleName != null ? xmlUsers.RoleName.Trim() : null;
                        xmlUsers.DepartmentName = xmlUsers.DepartmentName != null ? xmlUsers.DepartmentName.Trim() : null;
                        xmlUsers.SalesTeam = xmlUsers.SalesTeam != null ? xmlUsers.SalesTeam.Trim() : null;
                        if (!string.IsNullOrEmpty(xmlUsers.RoleName))
                        {
                            var roleModel = db.Roles.Count(a => a.BusinessId == businessId && a.Name.ToLower() == xmlUsers.RoleName.ToLower());
                            if (roleModel == 0)
                            {
                                db.Roles.Add(new Role
                                {
                                    Name = xmlUsers.RoleName,
                                    Description = xmlUsers.RoleName,
                                    BusinessId = businessId,
                                    IsActive = true,
                                    CreatedBy = currentUserId,
                                    CreatedOn = DateTime.UtcNow
                                });
                                db.SaveChanges();
                            }
                        }
                        #endregion

                        #region Create Department
                        if (!string.IsNullOrEmpty(xmlUsers.DepartmentName))
                        {
                            var DepartmentModel = db.Departments.Count(a => a.BusinessId == businessId && a.DepartmentName.ToLower() == xmlUsers.DepartmentName.ToLower());
                            if (DepartmentModel == 0)
                            {
                                db.Departments.Add(new Department
                                {
                                    DepartmentName = xmlUsers.DepartmentName,
                                    Description = xmlUsers.DepartmentName,
                                    BusinessId = businessId,
                                    IsActive = true,
                                    CreatedBy = currentUserId,
                                    CreatedOn = DateTime.UtcNow
                                });
                                db.SaveChanges();
                            }
                        }
                        #endregion

                        string[] selectedRoles = db.Roles.Where(a => a.BusinessId == businessId && a.Name.ToLower() == xmlUsers.RoleName.ToLower()).Select(a => a.Id.ToString()).ToArray();
                        int[] selectedDepartments = db.Departments.Where(a => a.BusinessId == businessId && a.DepartmentName.ToLower() == xmlUsers.DepartmentName.ToLower()).Select(a => a.Id).ToArray();

                        xmlUsers.Password = xmlUsers.ConfirmPassword = GeneralHelpers.GeneratePassword(3, 2, 2);
                        if (selectedRoles == null)
                        {
                        }
                        //if (ModelState.IsValid)
                        //{
                        var user = new GMUser { FirstName = xmlUsers.FirstName, MiddleName = xmlUsers.MiddleName, LastName = xmlUsers.LastName, PhoneNumber = xmlUsers.PhoneNumber, UserName = xmlUsers.Email, Email = xmlUsers.Email, BusinessId = businessId, IsActive = true };
                        var adminresult = await UserManager.CreateAsync(user, xmlUsers.Password);

                        if (adminresult.Succeeded)
                        {
                            db.UserProfiles.Add(new UserProfile
                            {
                                UserId = user.Id,
                                WorkEmail = xmlUsers.WorkEmail,
                                HomePhone = xmlUsers.HomePhone,
                                AdditionalPhone = xmlUsers.AdditionalPhone,
                                AddressLine1 = xmlUsers.AddressLine1,
                                AddressLine2 = xmlUsers.AddressLine2,
                                City = xmlUsers.City,
                                Zip = xmlUsers.Zip,
                                StateId = xmlUsers.StateId,
                                CreatedOn = System.DateTime.UtcNow,
                                CreatedBy = currentUserId,
                                UpdatedOn = System.DateTime.UtcNow,
                                UpdatedBy = currentUserId,
                                Startdate = string.IsNullOrEmpty(xmlUsers.StartDate) ? (DateTime?)null : DateTime.ParseExact(xmlUsers.StartDate, "MM-dd-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None)
                            });
                            db.SaveChanges();

                            if (selectedRoles != null)
                            {
                                IdentityResult result = null;
                                foreach (var roleId in selectedRoles)
                                {
                                    result = await UserManager.AddToRoleAsync(user.Id, roleId);
                                }
                            }

                            if (selectedDepartments != null && selectedDepartments.Count() > 0)
                            {
                                foreach (var item in selectedDepartments)
                                {
                                    db.UserDepartments.Add(new UserDepartment { UserId = user.Id, DepartmentId = item, CreatedBy = currentUserId, CreatedOn = DateTime.UtcNow });
                                    db.SaveChanges();
                                }
                            }
                            #region Create SalesTeam

                            if (!string.IsNullOrEmpty(xmlUsers.SalesTeam))
                            {
                                var RepGroupModel = db.RepGroups.FirstOrDefault(a => a.BusinessId == businessId && a.RepGroupName.ToLower() == xmlUsers.SalesTeam.ToLower());
                                if (RepGroupModel == null)
                                {
                                    var RepGroup = db.RepGroups.Add(new RepGroup
                                    {
                                        BusinessId = businessId,
                                        RepGroupName = xmlUsers.SalesTeam,
                                        Description = xmlUsers.SalesTeam,
                                        CreatedBy = currentUserId,
                                        CreatedOn = DateTime.UtcNow,
                                        IsActive = true,
                                    });
                                    if (db.SaveChanges() > 0)
                                    {
                                        #region Selected Roles
                                        switch (xmlUsers.RoleName.ToLower())
                                        {
                                            case "sales rep":
                                                var Reps = db.Reps.Add(new Rep
                                                {
                                                    UserId = user.Id,
                                                    RepGroupId = RepGroup.Id,
                                                    IsActive = true,
                                                    CreatedBy = currentUserId,
                                                    CreatedOn = System.DateTime.UtcNow,
                                                });
                                                if (db.SaveChanges() > 0)
                                                {
                                                    var Services = db.LookupEnrolledServices.Where(a => a.BusinessId == businessId && a.IsActive == true).Select(a => a.Id).ToList();
                                                    if (Services != null && Services.Count() > 0)
                                                    {
                                                        foreach (var item in Services)
                                                        {
                                                            db.RepServiceMappers.Add(new RepServiceMapper { ServiceId = item, RepId = Reps.Id, CreatedBy = currentUserId, CreatedOn = DateTime.UtcNow });
                                                            db.SaveChanges();
                                                        }
                                                    }
                                                }
                                                break;

                                            case "sales manager":
                                                db.RepgroupManagerMappers.Add(new RepgroupManagerMapper
                                                {
                                                    RepGroupId = RepGroup.Id,
                                                    ManagerId = user.Id,
                                                    CreatedBy = currentUserId,
                                                    CreatedOn = DateTime.UtcNow
                                                });
                                                db.SaveChanges();

                                                #region SalesRep

                                                var salesReps = db.Reps.Add(new Rep
                                                {
                                                    UserId = user.Id,
                                                    RepGroupId = RepGroup.Id,
                                                    IsActive = true,
                                                    CreatedBy = currentUserId,
                                                    CreatedOn = System.DateTime.UtcNow,
                                                });
                                                if (db.SaveChanges() > 0)
                                                {
                                                    var Services = db.LookupEnrolledServices.Where(a => a.BusinessId == businessId && a.IsActive == true).Select(a => a.Id).ToList();
                                                    if (Services != null && Services.Count() > 0)
                                                    {
                                                        foreach (var item in Services)
                                                        {
                                                            db.RepServiceMappers.Add(new RepServiceMapper { ServiceId = item, RepId = salesReps.Id, CreatedBy = currentUserId, CreatedOn = DateTime.UtcNow });
                                                            db.SaveChanges();
                                                        }
                                                    }
                                                }

                                                #endregion

                                                break;

                                            case "sales director":
                                                RepGroup.SalesDirectorId = user.Id;
                                                db.SaveChanges();

                                                #region SalesRep
                                                var salesRep = db.Reps.Add(new Rep
                                                {
                                                    UserId = user.Id,
                                                    RepGroupId = RepGroup.Id,
                                                    IsActive = true,
                                                    CreatedBy = currentUserId,
                                                    CreatedOn = System.DateTime.UtcNow,
                                                });
                                                if (db.SaveChanges() > 0)
                                                {
                                                    var Services = db.LookupEnrolledServices.Where(a => a.BusinessId == businessId && a.IsActive == true).Select(a => a.Id).ToList();
                                                    if (Services != null && Services.Count() > 0)
                                                    {
                                                        foreach (var item in Services)
                                                        {
                                                            db.RepServiceMappers.Add(new RepServiceMapper { ServiceId = item, RepId = salesRep.Id, CreatedBy = currentUserId, CreatedOn = DateTime.UtcNow });
                                                            db.SaveChanges();
                                                        }
                                                    }
                                                }

                                                #endregion
                                                break;
                                        }
                                        #endregion
                                    }
                                }
                                else
                                {
                                    #region Selected Roles

                                    switch (xmlUsers.RoleName.ToLower())
                                    {
                                        case "sales rep":
                                            var Reps = db.Reps.Add(new Rep
                                            {
                                                UserId = user.Id,
                                                RepGroupId = RepGroupModel.Id,
                                                IsActive = true,
                                                CreatedBy = currentUserId,
                                                CreatedOn = System.DateTime.UtcNow,
                                            });

                                            if (db.SaveChanges() > 0)
                                            {
                                                var Services = db.LookupEnrolledServices.Where(a => a.BusinessId == businessId && a.IsActive == true).Select(a => a.Id).ToList();
                                                if (Services != null && Services.Count() > 0)
                                                {
                                                    foreach (var item in Services)
                                                    {
                                                        db.RepServiceMappers.Add(new RepServiceMapper { ServiceId = item, RepId = Reps.Id, CreatedBy = currentUserId, CreatedOn = DateTime.UtcNow });
                                                        db.SaveChanges();
                                                    }
                                                }
                                            }
                                            break;

                                        case "sales manager":
                                            db.RepgroupManagerMappers.Add(new RepgroupManagerMapper
                                            {
                                                RepGroupId = RepGroupModel.Id,
                                                ManagerId = user.Id,
                                                CreatedBy = currentUserId,
                                                CreatedOn = DateTime.UtcNow
                                            });
                                            db.SaveChanges();
                                            #region SalesRep
                                            var salesReps = db.Reps.Add(new Rep
                                            {
                                                UserId = user.Id,
                                                RepGroupId = RepGroupModel.Id,
                                                IsActive = true,
                                                CreatedBy = currentUserId,
                                                CreatedOn = System.DateTime.UtcNow,
                                            });
                                            if (db.SaveChanges() > 0)
                                            {
                                                var Services = db.LookupEnrolledServices.Where(a => a.BusinessId == businessId && a.IsActive == true).Select(a => a.Id).ToList();
                                                if (Services != null && Services.Count() > 0)
                                                {
                                                    foreach (var item in Services)
                                                    {
                                                        db.RepServiceMappers.Add(new RepServiceMapper { ServiceId = item, RepId = salesReps.Id, CreatedBy = currentUserId, CreatedOn = DateTime.UtcNow });
                                                        db.SaveChanges();
                                                    }
                                                }
                                            }

                                            #endregion
                                            break;

                                        case "sales director":
                                            RepGroupModel.SalesDirectorId = user.Id;
                                            db.SaveChanges();

                                            #region SalesRep
                                            var salesRep = db.Reps.Add(new Rep
                                            {
                                                UserId = user.Id,
                                                RepGroupId = RepGroupModel.Id,
                                                IsActive = true,
                                                CreatedBy = currentUserId,
                                                CreatedOn = System.DateTime.UtcNow,
                                            });
                                            if (db.SaveChanges() > 0)
                                            {
                                                var Services = db.LookupEnrolledServices.Where(a => a.BusinessId == businessId && a.IsActive == true).Select(a => a.Id).ToList();
                                                if (Services != null && Services.Count() > 0)
                                                {
                                                    foreach (var item in Services)
                                                    {
                                                        db.RepServiceMappers.Add(new RepServiceMapper { ServiceId = item, RepId = salesRep.Id, CreatedBy = currentUserId, CreatedOn = DateTime.UtcNow });
                                                        db.SaveChanges();
                                                    }
                                                }
                                            }
                                            #endregion
                                            break;
                                    }

                                    #endregion
                                }
                            }
                            #endregion

                            #region Email Notification
                            var BusinessModel = db.BusinessMasters.FirstOrDefault(a => a.Id == businessId);
                            var UserDepartments = db.UserDepartments.Where(a => a.UserId == user.Id).Select(a => a.Department.DepartmentName);
                            string ReturnUrl = ConfigurationManager.AppSettings["BaseUrl"];
                            try
                            {
                                ViewData.Model = new EmgenexBusinessPortal.Models.EmailModel.UserEmailModel
                                {
                                    CurentUserName = CurrentUser.FirstName,
                                    FirstName = user.FirstName,
                                    BusinessName = BusinessModel.BusinessName,
                                    UserName = xmlUsers.Email,
                                    Password = xmlUsers.Password,
                                    UserDepartments = UserDepartments,
                                    ReturnUrl = ReturnUrl + BusinessModel.RelativeUrl.Replace(" ", "-")
                                };

                                var emailBody = RazorHelper.RenderRazorViewToString("~/Views/Shared/Email/AddNewUser.cshtml", this);

                                if (ConfigurationManager.AppSettings["IsInDemo"] == "true")
                                {
                                    string DemoEmailId = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["DemoEmailId"]) ? ConfigurationManager.AppSettings["DemoEmailId"] : null;
                                    var mail = new GMEmail();
                                    if (DemoEmailId != null)
                                    {
                                        mail.SendDynamicHTMLEmail(DemoEmailId, "New user registration", emailBody, "", "");
                                    }
                                }
                                else
                                {
                                    await UserManager.SendEmailAsync(user.Id, "New user registration", emailBody);
                                }
                            }
                            catch (Exception ex)
                            {
                                ex.Log();
                            }
                            #endregion

                        }
                        else
                        {
                            xmlUsers.ErrorMessage = adminresult.Errors.First();
                            model.UserFailedRecords.Add(xmlUsers);
                        }
                        //}
                        xmlHelper.RemoveFirstChild();
                    }
                }
            }
            model.BusinessId = null;
            return model;
        }

        public async Task<string> GetObjectsAsync(string NPI)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    return await httpClient.GetStringAsync("https://npiregistry.cms.hhs.gov/api/?number=" + NPI);
                }
            }
            catch (Exception ex)
            {
                ex.Log();
                return "{}";
            }
        }

    }

    public class ViewModel
    {
        public int? BusinessId { get; set; }

        public ViewModel()
        {
            FailedRecords = new List<AccountModel>();
            UserFailedRecords = new List<XMLRegisterModel>();
        }

        public HttpPostedFileBase File { get; set; }

        public List<AccountModel> FailedRecords { get; set; }

        public List<XMLRegisterModel> UserFailedRecords { get; set; }

        public bool IsAccount { get; set; }
    }

    public sealed class DynamicJsonConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            return type == typeof(object) ? new DynamicJsonObject(dictionary) : null;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get { return new ReadOnlyCollection<Type>(new List<Type>(new[] { typeof(object) })); }
        }

        private sealed class DynamicJsonObject : DynamicObject
        {
            private readonly IDictionary<string, object> _dictionary;

            public DynamicJsonObject(IDictionary<string, object> dictionary)
            {
                if (dictionary == null)
                    throw new ArgumentNullException("dictionary");
                _dictionary = dictionary;
            }

            public override string ToString()
            {
                var sb = new StringBuilder("{");
                ToString(sb);
                return sb.ToString();
            }

            private void ToString(StringBuilder sb)
            {
                var firstInDictionary = true;
                foreach (var pair in _dictionary)
                {
                    if (!firstInDictionary)
                        sb.Append(",");
                    firstInDictionary = false;
                    var value = pair.Value;
                    var name = pair.Key;
                    if (value is string)
                    {
                        sb.AppendFormat("{0}:\"{1}\"", name, value);
                    }
                    else if (value is IDictionary<string, object>)
                    {
                        new DynamicJsonObject((IDictionary<string, object>)value).ToString(sb);
                    }
                    else if (value is ArrayList)
                    {
                        sb.Append(name + ":[");
                        var firstInArray = true;
                        foreach (var arrayValue in (ArrayList)value)
                        {
                            if (!firstInArray)
                                sb.Append(",");
                            firstInArray = false;
                            if (arrayValue is IDictionary<string, object>)
                                new DynamicJsonObject((IDictionary<string, object>)arrayValue).ToString(sb);
                            else if (arrayValue is string)
                                sb.AppendFormat("\"{0}\"", arrayValue);
                            else
                                sb.AppendFormat("{0}", arrayValue);

                        }
                        sb.Append("]");
                    }
                    else
                    {
                        sb.AppendFormat("{0}:{1}", name, value);
                    }
                }
                sb.Append("}");
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                if (!_dictionary.TryGetValue(binder.Name, out result))
                {
                    // return null to avoid exception.  caller can check for null this way...
                    result = null;
                    return true;
                }

                result = WrapResultObject(result);
                return true;
            }

            public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
            {
                if (indexes.Length == 1 && indexes[0] != null)
                {
                    if (!_dictionary.TryGetValue(indexes[0].ToString(), out result))
                    {
                        // return null to avoid exception.  caller can check for null this way...
                        result = null;
                        return true;
                    }

                    result = WrapResultObject(result);
                    return true;
                }

                return base.TryGetIndex(binder, indexes, out result);
            }

            private static object WrapResultObject(object result)
            {
                var dictionary = result as IDictionary<string, object>;
                if (dictionary != null)
                    return new DynamicJsonObject(dictionary);

                var arrayList = result as ArrayList;
                if (arrayList != null && arrayList.Count > 0)
                {
                    return arrayList[0] is IDictionary<string, object>
                        ? new List<object>(arrayList.Cast<IDictionary<string, object>>().Select(x => new DynamicJsonObject(x)))
                        : new List<object>(arrayList.Cast<object>());
                }

                return result;
            }
        }
    }

    public class AccountModel
    {
        [Required]
        public string AddressLine1 { get; internal set; }

        public string AddressLine2 { get; internal set; }

        [Required]
        public string City { get; internal set; }

        public string EnrolledServices { get; internal set; }

        public bool? HasProvider { get; set; }

        public string NPI { get; internal set; }

        [Required]
        public string PhoneNumber { get; internal set; }

        [Required]
        public string PracticeName { get; internal set; }

        [Required]
        public int? RepId { get; internal set; }

        public string ReportDeliveryFax { get; internal set; }

        public string SalesRep { get; internal set; }

        public string SalesTeam { get; internal set; }

        public string State { get; internal set; }

        [Required]
        public int? StateId { get; internal set; }

        [Required]
        public string ZipCode { get; internal set; }
    }

    public class UserModel
    {
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string ErrorMessage { get; set; }
    }
}