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

namespace EmgenexBusinessPortal.Areas.Admin.Controllers
{
    public class AccountImportController : BaseController
    {
        // GET: Admin/AccountImport
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index(ViewModel model)
        {
            int businessId = model.BusinessId,
                currentUserId = CurrentUser.Id;

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
                                        var s = obj.result_count;
                                        int.TryParse(Convert.ToString(obj.result_count), out resultCount);
                                        if (resultCount > 0)
                                        {
                                            oPracticeProviderMappers.Add(new PracticeProviderMapper
                                            {
                                                AddressIndex = 1,
                                                Provider = new Provider
                                                {
                                                    FirstName = obj.results[0].basic.first_name,
                                                    MiddleName = obj.results[0].basic.middle_name,
                                                    LastName = obj.results[0].basic.last_name,
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
                                        string salesRep = item.SalesRep.Replace(" ", ""),
                                            repGroup = item.SalesTeam.Replace(" ", "");

                                        var objRep = DBEntity.Reps.Where(a => a.User2.BusinessId == businessId && (a.User2.FirstName.Trim() + a.User2.MiddleName.Trim() + a.User2.LastName.Trim()).Contains(salesRep) && a.RepGroup.RepGroupName.Replace(" ", "").Trim() == repGroup).ToList();
                                        if (objRep.Count == 1)
                                        {
                                            item.RepId = objRep.FirstOrDefault().Id;
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
                                        //if (!TryValidateModel(item))
                                        //{
                                        model.FailedRecords.Add(item);
                                        continue;
                                        //}
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
        public int BusinessId { get; set; }
        public ViewModel()
        {
            FailedRecords = new List<AccountModel>();
        }
        //[Required, FileExtensions(Extensions = "xlsx", ErrorMessage = "Specify a EXCEL file")]
        public HttpPostedFileBase File { get; set; }

        public List<AccountModel> FailedRecords { get; set; }
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

        #region Nested type: DynamicJsonObject

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

        #endregion
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
}