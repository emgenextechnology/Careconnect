using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EBP.Business.Repository;
using EBP.Business;
using EBP.Business.Entity;
using EBP.Business.Entity.Lead;
using EBP.Business.Filter;
using Newtonsoft.Json;
using EBP.Api.Models;
using EBP.Business.Entity.Practice;
using EBP.Business.Entity.Note;
using GM.Identity.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Configuration;
using EBP.Business.Notifications;
using GM.Identity.Config;
using EBP.Business.Entity.EntityNotificationSettings;
using EBP.Business.Enums;

namespace EBP.Api.Controllers
{
    [RoutePrefix("{Type:regex(lead|leads)}")]
    //[RoutePrefix("leads")]
    [ApiAuthorize]
    public class ApiLeadController : ApiBaseController
    {
        private GMUserManager _userManager;
        public GMUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<GMUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [HttpPost]
        [Route("getbyfilter")]
        [Route("")]
        public IHttpActionResult GetByFilter(FilterLead filter)
        {
            string[] allowedRoles = { "RDLD" };
            string[] superRoles = { "RDLDALL" };
            bool hasSuperRight = HasSuperRight(superRoles);

            if (HasSuperRight(allowedRoles) || hasSuperRight)
            {
                var repository = new RepositoryLead();
                var response = repository.GetLeads(filter, CurrentUser.BusinessId, CurrentUserId, hasSuperRight, IsSalesManager, IsSalesDirector);
                return Ok<DataResponse<EntityList<EntityLead>>>(response);
            }
            else
            {
                return Ok<DataResponse>(null);
            }
        }

        [Route("getfilter")]
        public IHttpActionResult GetFilter()
        {
            return Ok(new FilterLead());
        }

        [Route("getlead/{Id}")]
        [Route("{Id}")]
        public IHttpActionResult GetLeadById(int? Id)
        {
            var response = new DataResponse<EntityLead>();
            var repository = new RepositoryLead();
            if (Id.HasValue)
            {
                response = repository.GetLeadById(Id.Value);
            }
            else
            {
                response.Model = new EntityLead();
                response.Model.Practice = new Business.Entity.Practice.EntityPractice();
            }
            return Ok<DataResponse>(response);
        }

        #region Add Lead
        //[HttpPost]
        //[Route("Save")]
        //public IHttpActionResult InsertLeadData(VMLead model)
        //{
        //    //using (HttpClient httpClient = new HttpClient())
        //    //{
        //    //    return httpClient.GetStringAsync("https://npiregistry.cms.hhs.gov/api/?number=" + NPI);
        //    //}
        //    // if  CurrentuserId is rep= rep

        //    var repository = new RepositoryLead();
        //    var response = new DataResponse<EntityLead>();

        //    if (!HasRight(new string[] { "WRREP" }) && IsRep)
        //    {
        //        ModelState["model.RepId"].Errors.Clear();
        //        ModelState["model.RepGroupId"].Errors.Clear();
        //    }
        //    if (model.Providers == null)
        //    {
        //        ModelState["model.PracticeAddressLine1"].Errors.Clear();
        //        ModelState["model.City"].Errors.Clear();
        //        ModelState["model.StateId"].Errors.Clear();
        //        ModelState["model.Zip"].Errors.Clear();
        //        ModelState["model.PhoneNumber"].Errors.Clear();
        //    }
        //    if (ModelState.IsValid)
        //    {
        //        var entityLead = new EntityLead
        //        {
        //            LeadSourceId = model.LeadSourceId,
        //            LeadServiceIntrest = model.ServiceInterest,
        //            IsActive = true,
        //            RepGroupId = model.RepGroupId,
        //            RepId = model.RepId,
        //            ContactPreferenceId = 1,
        //            OtherLeadSource = model.OtherLeadSource,
        //            IsConverted = false,
        //        };

        //        entityLead.Practice = new EntityPractice
        //        {
        //            Name = model.PracticeName,
        //            SpecialityTypeId = model.SpecialityTypeId,
        //            Fax = model.Fax,
        //            Address = new List<EntityPracticeAddress>{
        //                    new EntityPracticeAddress{  
        //                        Id = model.AddressId == null ? 0 : model.AddressId.Value,
        //                        AddressIndex = -1,
        //                        City = model.City, 
        //                        Line1 = model.PracticeAddressLine1, 
        //                        Line2 = model.PracticeAddressLine2, 
        //                        AddressTypeId = 1,
        //                        StateId = model.StateId ?? 0,
        //                        Zip= model.Zip ,
        //                        Phone = new List<EntityPracticePhone> { 
        //                            new EntityPracticePhone { 
        //                                PhoneNumber = model.PhoneNumber,
        //                                Extension = model.PhoneExtension
        //                            }
        //                        }
        //                    }
        //                }
        //        };

        //        entityLead.Practice.Specialities = new List<EntityPracticeSpeciality>();
        //        var specialities = new List<EntityPracticeSpeciality>();

        //        entityLead.PracticeTypeId = model.SpecialityTypeId == 0 ? null : model.SpecialityTypeId;
        //        if (model.SpecialityTypeId == 1)
        //        {
        //            specialities.Add(new EntityPracticeSpeciality { PracticeSpecialityId = model.SpecialityId ?? 0 });
        //        }
        //        else if (model.SpecialityTypeId == 2 && model.SpecialityIds != null && model.SpecialityIds.Count() > 0)
        //        {
        //            foreach (string sId in model.SpecialityIds)
        //            {
        //                specialities.Add(new EntityPracticeSpeciality { PracticeSpecialityId = int.Parse(sId) });
        //            }
        //        }
        //        else if (model.SpecialityTypeId == 3 && !string.IsNullOrEmpty(model.NewSpectialityName))
        //        {
        //            entityLead.Practice.SpecialityType = model.NewSpectialityName;
        //        }
        //        if (specialities.Count > 0)
        //            entityLead.Practice.Specialities = specialities;
        //        if (model.Locations != null)
        //        {
        //            var addressList = new List<EntityPracticeAddress>();

        //            model.Locations.ForEach(a => addressList.Add(new EntityPracticeAddress
        //            {
        //                Id = a.AddressId == null ? 0 : a.AddressId.Value,
        //                AddressIndex = a.AddressIndex.Value,
        //                City = a.City,
        //                Line1 = a.AddressLine1,
        //                Line2 = a.AddressLine2,
        //                AddressTypeId = 2,
        //                StateId = a.StateId ?? 0,
        //                Zip = a.Zip,
        //                ManagerName = a.ManagerName,
        //                Phone = new List<EntityPracticePhone> { 
        //                    new EntityPracticePhone { 
        //                        PhoneNumber = a.PhoneNumber,
        //                        Extension = a.Extension 
        //                    } 
        //                }
        //            }));

        //            if (addressList.Count > 0)
        //                entityLead.Practice.Address = entityLead.Practice.Address.Concat(addressList);
        //        }
        //        if (model.Providers != null)
        //        {
        //            var isMultipleProviders = model.Providers.GroupBy(a => a.NPI).Any(a => a.Count() > 1);
        //            if (isMultipleProviders)
        //            {
        //                return Ok<dynamic>(new { Status = HttpStatusCode.BadRequest, Model = new { Key = "NPI", Message = "NPI is Repeating" } });
        //            }

        //            var providerList = new List<EntityProvider>();

        //            model.Providers.ForEach(a =>
        //                providerList.Add(
        //                new EntityProvider
        //                {
        //                    Id = a.Id == null ? 0 : a.Id.Value,
        //                    DegreeId = a.DegreeId ?? 0,
        //                    FirstName = a.FirstName,
        //                    LastName = a.LastName,
        //                    MiddleName = a.MiddleName,
        //                    NPI = a.NPI,
        //                    AddressIndex = a.Location != null ? (a.Location.AddressIndex == null ? -1 : a.Location.AddressIndex.Value) : -1,
        //                    Address = a.Location != null ? new EntityProviderAddress
        //                    {
        //                        Id = a.Location.AddressId == null ? 0 : a.Location.AddressId.Value,
        //                        City = a.Location.City,
        //                        Line1 = a.Location.AddressLine1,
        //                        Line2 = a.Location.AddressLine2,
        //                        AddressTypeId = 2,
        //                        StateId = a.Location.StateId.Value,
        //                        Zip = a.Location.Zip,
        //                        ManagerName = a.Location.ManagerName,
        //                        Phone = a.Location == null ? null : new List<EntityPracticePhone> { 
        //                            new EntityPracticePhone { 
        //                                PhoneNumber = a.Location.PhoneNumber,
        //                                Extension = a.Location.Extension 
        //                            }
        //                        }
        //                    } : null
        //                }));

        //            if (providerList.Count > 0)
        //                entityLead.Practice.Providers = providerList;
        //        }
        //        entityLead.UpdatedBy = entityLead.CreatedBy = entityLead.CurrentUserId = CurrentUser.Id;
        //        entityLead.BusinessId = CurrentUser.BusinessId;
        //        entityLead.CreatedByName = string.Format("{0} {1}", CurrentUser.FirstName, CurrentUser.LastName);

        //        if (model.Id > 0)
        //        {
        //            entityLead.LeadId = model.Id ?? 0;
        //            response = repository.Update(entityLead);
        //        }
        //        else
        //        {
        //            response = repository.Insert(entityLead);
        //            var responseModel = response.Model;

        //            if (responseModel.LeadId != null)
        //            {

        //                EmailNotification emailNotify = new EmailNotification
        //                {
        //                    PracticeName = model.PracticeName,
        //                    //ManagerFirstName = responseModel.Rep.Manager.FirstName,
        //                    //ManagerLastName = responseModel.Rep.Manager.LastName,
        //                    ManagerNames = responseModel.Rep.Managers.Count() > 0 ? string.Join(", ", responseModel.Rep.Managers.Select(a => a.Name)) : null,//responseModel.Rep.Manager.Name
        //                    ManagerEmails = responseModel.Rep.Managers.Count() > 0 ? string.Join(", ", responseModel.Rep.Managers.Select(a => a.Email)) : null,//responseModel.Rep.Manager.Email
        //                    CurrentUserFirstName = CurrentUser.FirstName,
        //                    CurrentUserLastName = CurrentUser.LastName,
        //                    RootPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath,
        //                    ReturnUrl = ConfigurationManager.AppSettings["BaseUrl"],
        //                };
        //                NewLeadEmailNotification(emailNotify);
        //            }

        //            #region Backup

        //            //try
        //            //{
        //            //    model.ManagerName = response.Model.Rep.ManagerName;
        //            //    model.CreatedByName = response.Model.CreatedByName;
        //            //    model.ReturnUrl = ConfigurationManager.AppSettings["BaseUrl"];
        //            //    var rootPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
        //            //    var emailBody = TemplateManager.NewLead(rootPath, model.ManagerName, model.RepId, model.CreatedByName, model.PracticeName, model.ReturnUrl);
        //            //    var mail = new GMEmail();
        //            //    string toEmails = null;

        //            //    {
        //            //        toEmails = new RepositoryUserProfile().NotificationEnabledEmails(response.Model.Rep.Manager.ManagerEmail, "LDNFN");
        //            //        if (!string.IsNullOrEmpty(toEmails))
        //            //        {
        //            //            mail.SendDynamicHTMLEmail(toEmails, "CareConnect Systems: New Lead-Notification", emailBody, "", "");
        //            //        }
        //            //    }

        //            //    {
        //            //        toEmails = new RepositoryUserProfile().NotificationEnabledEmails(response.Model.Rep.RepEmail, "LDNFN");
        //            //        if (!string.IsNullOrEmpty(toEmails))
        //            //        {
        //            //            mail.SendDynamicHTMLEmail(toEmails, "CareConnect Systems: New Lead-Notification", emailBody, "", "");
        //            //        }
        //            //    }

        //            //    string otherEmails = ConfigurationManager.AppSettings["OtherEmails"];
        //            //    string[] OtherEmails = !string.IsNullOrEmpty(otherEmails) ? ConfigurationManager.AppSettings["OtherEmails"].Split(',') : null;
        //            //    if (OtherEmails != null && OtherEmails.Count() > 0)
        //            //    {
        //            //        foreach (var item in OtherEmails)
        //            //        {
        //            //            mail.SendDynamicHTMLEmail(item, "New Lead - Notification", emailBody, "", "");
        //            //        }
        //            //    }

        //            //}
        //            //catch (Exception ex)
        //            //{
        //            //    ex.Log();
        //            //}

        //            #endregion
        //        }
        //    }
        //    else
        //    {
        //        var errorList = ModelState.Where(a => a.Value.Errors.Any()).Select(s => new
        //        {
        //            Key = s.Key.Split('.').Last(),
        //            Message = s.Value.Errors[0].ErrorMessage
        //        });
        //        return Ok<dynamic>(new { Status = HttpStatusCode.BadRequest, Model = errorList });
        //    }
        //    return Ok<DataResponse>(response);
        //}
        #endregion


        [HttpPost]
        [Route("Save")]
        public IHttpActionResult InsertLeadData(VMLead model)
        {
            List<string> adddress = new List<string>()
            {
                "model.PracticeAddressLine1",
                "model.City",
                "model.StateId",
                "model.Zip",
                "model.PhoneNumber"
            };

            List<string> reporrepgroup = null;

            bool hasAnyAddressFields = adddress.Any(a => ModelState.IsValidField(a)),
                hasAllAddressFields = adddress.All(a => ModelState.IsValidField(a));

            if (!HasRight(new string[] { "WRREP" }) && IsRep)
            {
                ModelState["model.RepId"].Errors.Clear();
                ModelState["model.RepGroupId"].Errors.Clear();
            }
            else
            {
                reporrepgroup = new List<string>()
                {
                    "model.RepId",
                    "model.RepGroupId"
                };
            }

            if (hasAnyAddressFields && !hasAllAddressFields)
            {
                var errorList = ModelState.Where(a => a.Value.Errors.Any()).Select(s => new
                {
                    Key = s.Key.Split('.').Last(),
                    Message = s.Value.Errors[0].ErrorMessage
                });
                return Ok<dynamic>(new { Status = HttpStatusCode.BadRequest, Model = errorList });
            }

            var isRepandRepgroupSelected = reporrepgroup != null && reporrepgroup.Any(a => ModelState.IsValidField(a));

            if (!isRepandRepgroupSelected)
            {
                var errorList = ModelState.Where(a => a.Value.Errors.Any() && reporrepgroup.Contains(a.Key)).Select(s => new
                {
                    Key = s.Key.Split('.').Last(),
                    Message = s.Value.Errors[0].ErrorMessage
                });
                return Ok<dynamic>(new { Status = HttpStatusCode.BadRequest, Model = errorList });
            }

            var repository = new RepositoryLead();
            var response = new DataResponse<EntityLead>();
            if (!string.IsNullOrEmpty(model.PracticeName))
            {
                #region Fully Valid

                var entityLead = new EntityLead
                {
                    LeadSourceId = model.LeadSourceId,
                    LeadServiceIntrest = model.ServiceInterest,
                    IsActive = true,
                    RepGroupId = model.RepGroupId,
                    RepId = model.RepId,
                    ContactPreferenceId = 1,
                    OtherLeadSource = model.OtherLeadSource,
                    IsConverted = false,
                };

                entityLead.Practice = new EntityPractice
                {
                    Name = model.PracticeName,
                    SpecialityTypeId = model.SpecialityTypeId,
                    //Fax = model.Fax
                };

                if (hasAllAddressFields)
                {
                    //entityLead.Practice.Fax = model.Fax;
                    entityLead.Practice.Address = new List<EntityPracticeAddress>{
                            new EntityPracticeAddress{
                                Id = model.AddressId == null ? 0 : model.AddressId.Value,
                                AddressIndex = -1,
                                City = model.City,
                                Line1 = model.PracticeAddressLine1,
                                Line2 = model.PracticeAddressLine2,
                                AddressTypeId = 1,
                                StateId = model.StateId ?? 0,
                                Zip= model.Zip,
                                Fax = model.Fax,
                                Phone = new List<EntityPracticePhone> {
                                    new EntityPracticePhone {
                                        PhoneNumber = model.PhoneNumber,
                                        Extension = model.PhoneExtension
                                    }
                                }
                            }
                        };
                }

                entityLead.Practice.Specialities = new List<EntityPracticeSpeciality>();
                var specialities = new List<EntityPracticeSpeciality>();

                entityLead.PracticeTypeId = model.SpecialityTypeId == 0 ? null : model.SpecialityTypeId;
                if (model.SpecialityTypeId == 1)
                {
                    specialities.Add(new EntityPracticeSpeciality { PracticeSpecialityId = model.SpecialityId ?? 0 });
                }
                else if (model.SpecialityTypeId == 2 && model.SpecialityIds != null && model.SpecialityIds.Count() > 0)
                {
                    foreach (string sId in model.SpecialityIds)
                    {
                        specialities.Add(new EntityPracticeSpeciality { PracticeSpecialityId = int.Parse(sId) });
                    }
                }
                else if (model.SpecialityTypeId == 3 && !string.IsNullOrEmpty(model.NewSpectialityName))
                {
                    entityLead.Practice.SpecialityType = model.NewSpectialityName;
                }
                if (specialities.Count > 0)
                    entityLead.Practice.Specialities = specialities;

                #region Save Location
                if (model.Locations != null && model.Locations.Count() > 0)
                {
                    var addressList = new List<EntityPracticeAddress>();

                    model.Locations.ForEach(a => addressList.Add(new EntityPracticeAddress
                    {
                        Id = a.AddressId == null ? 0 : a.AddressId.Value,
                        AddressIndex = a.AddressIndex.Value,
                        City = a.City,
                        Line1 = a.AddressLine1,
                        Line2 = a.AddressLine2,
                        AddressTypeId = 2,
                        StateId = a.StateId ?? 0,
                        Zip = a.Zip,
                        ManagerName = a.ManagerName,
                        Phone = new List<EntityPracticePhone> {
                            new EntityPracticePhone {
                                PhoneNumber = a.PhoneNumber,
                                Extension = a.Extension
                            }
                        }
                    }));

                    if (addressList.Count > 0)
                        entityLead.Practice.Address = entityLead.Practice.Address.Concat(addressList);
                }
                #endregion

                #region Save Providers

                if (hasAnyAddressFields && hasAllAddressFields)
                {
                    if (model.Providers != null && model.Providers.Count() > 0)
                    {
                        var isMultipleProviders = model.Providers.GroupBy(a => a.NPI).Any(a => a.Count() > 1);
                        if (isMultipleProviders)
                        {
                            return Ok<dynamic>(new { Status = HttpStatusCode.BadRequest, Model = new { Key = "NPI", Message = "NPI is Repeating" } });
                        }

                        var providerList = new List<EntityProvider>();

                        model.Providers.ForEach(a =>
                            providerList.Add(
                            new EntityProvider
                            {
                                Id = a.Id == null ? 0 : a.Id.Value,
                                DegreeId = a.DegreeId ?? 0,
                                FirstName = a.FirstName,
                                LastName = a.LastName,
                                MiddleName = a.MiddleName,
                                NPI = a.NPI,
                                AddressIndex = a.Location != null ? (a.Location.AddressIndex == null ? -1 : a.Location.AddressIndex.Value) : -1,
                                Address = a.Location != null ? new EntityProviderAddress
                                {
                                    Id = a.Location.AddressId == null ? 0 : a.Location.AddressId.Value,
                                    City = a.Location.City,
                                    Line1 = a.Location.AddressLine1,
                                    Line2 = a.Location.AddressLine2,
                                    AddressTypeId = 2,
                                    StateId = a.Location.StateId.Value,
                                    Zip = a.Location.Zip,
                                    ManagerName = a.Location.ManagerName,
                                    Phone = a.Location == null ? null : new List<EntityPracticePhone> {
                                    new EntityPracticePhone {
                                        PhoneNumber = a.Location.PhoneNumber,
                                        Extension = a.Location.Extension
                                    }
                                }
                                } : null
                            }));

                        if (providerList.Count > 0)
                            entityLead.Practice.Providers = providerList;
                    }
                }
                #endregion

                entityLead.UpdatedBy = entityLead.CreatedBy = entityLead.CurrentUserId = CurrentUser.Id;
                entityLead.BusinessId = CurrentUser.BusinessId;
                entityLead.CreatedByName = string.Format("{0} {1}", CurrentUser.FirstName, CurrentUser.LastName);

                if (model.Id > 0)
                {

                    entityLead.LeadId = model.Id ?? 0;
                    response = repository.Update(entityLead);
                }
                else
                {
                    string services = string.Empty, providers = string.Empty, practiceAddress = string.Empty;

                    response = repository.Insert(entityLead);
                    var responseModel = response.Model;

                    if (responseModel.Practice.Providers != null && responseModel.Practice.Providers.Count() > 0)
                    {
                        foreach (var item in responseModel.Practice.Providers)
                        {
                            providers = providers + item.FirstName + " " + item.LastName + " (" + item.NPI + ") <br />";
                        }
                    }

                    var primaryAddress = responseModel.Practice.Address.FirstOrDefault();
                    if (primaryAddress != null)
                    {
                        var stateName = new RepositoryLookups().GetAllStates().Model.List.Where(a => a.Id == primaryAddress.StateId).First().Value;
                        practiceAddress = string.Format("{0}, {1}, {2}, {3}, {4}", primaryAddress.Line1, primaryAddress.Line2, primaryAddress.City, stateName, primaryAddress.Zip);
                    }

                    if (responseModel.LeadId > 0)
                    {
                        EmailNotification emailNotify = new EmailNotification
                        {
                            PracticeName = model.PracticeName,
                            PracticeAddress = practiceAddress,
                            Providers = providers,
                            Services = services,
                            RepFirstName = responseModel.Rep.FirstName,
                            RepMiddleName = responseModel.Rep.MiddleName,
                            RepLastName = responseModel.Rep.LastName,
                            RepEmail = responseModel.Rep.RepEmail,
                            Managers = responseModel.Rep.Managers,
                            CurrentUserFirstName = CurrentUser.FirstName,
                            CurrentUserLastName = CurrentUser.LastName,
                            RootPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath,
                            ReturnUrl = ConfigurationManager.AppSettings["BaseUrl"],
                        };
                        NewLeadEmailNotification(emailNotify);
                    }
                }

                #endregion
            }
            else
            {
                var errorList = ModelState.Where(a => a.Value.Errors.Any()).Select(s => new
                {
                    Key = s.Key.Split('.').Last(),
                    Message = s.Value.Errors[0].ErrorMessage
                });
                return Ok<dynamic>(new { Status = HttpStatusCode.BadRequest, Model = errorList });
            }
            return Ok<DataResponse>(response);
        }

        [HttpPost]
        [Route("toggleflag/{id}")]
        [Route("{id}/toggleflag")]
        public IHttpActionResult SetFlag(int id)
        {
            var repository = new RepositoryLead();
            var response = repository.SetFlag(id);
            response.Status = DataResponseStatus.OK;
            return Ok<DataResponse>(response);
        }

        [HttpPost]
        [Route("togglestatus/{id}")]
        [Route("{id}/togglestatus")]
        public IHttpActionResult SetLeadStatus(int id)
        {
            var repository = new RepositoryLead();
            var response = repository.SetActiveStatus(id);
            response.Status = DataResponseStatus.OK;
            return Ok<DataResponse>(response);
        }

        [Route("getvmlead/{id}")]
        [Route("getvmlead/{id}/edit")]
        public IHttpActionResult GetVmLeadObject(int? id)
        {
            var response = new DataResponse<VMLead>();
            if (id.HasValue)
            {
                var repository = new RepositoryLead();
                var model = repository.GetLeadById(id.Value).Model;
                if (model != null)
                {
                    var PracticeAddress = model.Practice.Address.Where(a => a.AddressTypeId == 1).Select(a => new VMLocation
                    {
                        AddressId = a.AddressId,
                        PhoneId = a.Phone.Count() > 0 ? a.Phone.First().PhoneId : 0,
                        AddressLine1 = a.Line1,
                        AddressLine2 = a.Line2,
                        PhoneNumber = a.Phone.Count() > 0 ? a.Phone.First().PhoneNumber : null,
                        Extension = a.Phone.Count() > 0 ? a.Phone.First().Extension : null,
                        City = a.City,
                        Fax = a.Fax,
                        ManagerName = a.ManagerName,
                        StateId = a.StateId,
                        Zip = a.Zip
                    }).FirstOrDefault();

                    int? SpecialityTypeId = model.Practice.SpecialityTypeId,
                        RepId = model.Rep.Id,
                        RepGroupId = model.Rep.GroupId;

                    response.Model = new VMLead
                    {
                        Id = id.Value,
                        PracticeName = model.Practice.Name,
                        PracticeId = model.PracticeId,
                        SpecialityTypeId = SpecialityTypeId == null ? 0 : (int)SpecialityTypeId,
                        ServiceInterest = model.LeadServiceIntrest,
                        RepGroupId = RepGroupId == null ? 0 : (int)RepGroupId,
                        RepId = RepId == null ? 0 : (int)RepId,
                        AddressId = PracticeAddress != null ? PracticeAddress.AddressId : null,
                        PracticeAddressLine1 = PracticeAddress != null ? PracticeAddress.AddressLine1 : null,
                        PracticeAddressLine2 = PracticeAddress != null ? PracticeAddress.AddressLine2 : null,
                        PhoneNumber = PracticeAddress != null ? PracticeAddress.PhoneNumber : null,
                        PhoneExtension = PracticeAddress != null ? PracticeAddress.Extension : null,
                        City = PracticeAddress != null ? PracticeAddress.City : null,
                        Fax = PracticeAddress != null ? PracticeAddress.Fax : null,
                        StateId = PracticeAddress != null ? PracticeAddress.StateId : null,
                        Zip = PracticeAddress != null ? PracticeAddress.Zip : null,
                        LeadSourceId = model.LeadSourceId,
                        NewSpectialityName = model.Practice.SpecialityType,
                        SpecialityId = model.Practice.Specialities.Count() == 1 ? model.Practice.Specialities.First().PracticeSpecialityId : 0,
                        SpecialityIds = model.Practice.Specialities.Count() > 0 ? model.Practice.Specialities.Select(a => a.PracticeSpecialityId.ToString()).ToArray() : null,
                        OtherLeadSource = model.OtherLeadSource
                    };

                    response.Model.Locations = model.Practice.Address.Where(s => s.AddressTypeId != 1).Select(a => new VMLocation
                    {
                        AddressId = a.AddressId,
                        PhoneId = a.Phone.Count() > 0 ? a.Phone.FirstOrDefault().PhoneId : 0,
                        AddressLine1 = a.Line1,
                        AddressLine2 = a.Line2,
                        PhoneNumber = a.Phone.Count() > 0 ? a.Phone.First().PhoneNumber : null,
                        Extension = a.Phone.Count() > 0 ? a.Phone.First().Extension : null,
                        City = a.City,
                        AddressTypeId = a.AddressTypeId,
                        ManagerName = a.ManagerName,
                        StateId = a.StateId,
                        Zip = a.Zip
                    }).ToList();

                    response.Model.Providers = model.Practice.Providers.Select(a => new VMProvider
                    {
                        Id = a.ProviderId,
                        DegreeId = a.DegreeId,
                        FirstName = a.FirstName,
                        LastName = a.LastName,
                        MiddleName = a.MiddleName,
                        NPI = a.NPI,
                        Location = a.Address != null ? new VMLocation
                        {
                            AddressId = a.Address.AddressId,
                            PhoneId = a.Address.Phone.Count() > 0 ? a.Address.Phone.FirstOrDefault().PhoneId : 0,
                            AddressLine1 = a.Address.Line1,
                            AddressLine2 = a.Address.Line2,
                            PhoneNumber = a.Address.Phone.Count() > 0 ? a.Address.Phone.First().PhoneNumber : null,
                            Extension = a.Address.Phone.Count() > 0 ? a.Address.Phone.First().Extension : null,
                            City = a.Address.City,
                            ManagerName = a.Address.ManagerName,
                            StateId = a.Address.StateId,
                            Zip = a.Address.Zip,
                            AddressTypeId = a.Address.AddressTypeId
                        } : response.Model.Locations.FirstOrDefault()
                    }).ToList();
                    response.Status = DataResponseStatus.OK;
                    return Ok<DataResponse>(response);
                }
            }

            response.Model = new VMLead();
            response.Status = DataResponseStatus.OK;
            return Ok<DataResponse>(response);
        }

        [Route("getlocationobject")]
        [Route("locationobject")]
        public IHttpActionResult GetLocationObject()
        {
            return Ok<VMLocation>(new VMLocation());
        }

        [Route("getproviderobject")]
        [Route("providerobject")]
        public IHttpActionResult GetProviderObject()
        {
            return Ok<VMProvider>(new VMProvider());
        }

        private void NewLeadEmailNotification(EmailNotification objEmailNotify)
        {
            try
            {
                var mail = new GMEmail();

                //email to manager
                {
                    if (objEmailNotify.Managers != null && objEmailNotify.Managers.Count() > 0)
                    {
                        foreach (var item in objEmailNotify.Managers)
                        {
                            if (CurrentUser.Email != item.Email)
                            {
                                string toEmails = new RepositoryUserProfile().NotificationEnabledEmails(item.Email, "LDNFN");
                                if (!string.IsNullOrEmpty(toEmails))
                                {
                                    string emailBody = TemplateManager.NewPracticeToManager(objEmailNotify.RootPath, objEmailNotify.Services, objEmailNotify.Providers, objEmailNotify.PracticeAddress, item.Name, objEmailNotify.RepName, objEmailNotify.CreatedByName, objEmailNotify.PracticeName, objEmailNotify.ReturnUrl, NotificationTargetType.Lead, CurrentBusinessId.Value, CurrentUser.RelativeUrl);
                                    mail.SendDynamicHTMLEmail(toEmails, "New Lead Created", emailBody, CurrentUser.OtherEmails);
                                }
                            }
                        }
                    }
                }

                //email to Rep
                {
                    if (CurrentUser.Email != objEmailNotify.RepEmail)
                    {
                        string toEmails = new RepositoryUserProfile().NotificationEnabledEmails(objEmailNotify.RepEmail, "LDNFN");
                        if (!string.IsNullOrEmpty(toEmails))
                        {
                            string emailBody = TemplateManager.NewPracticeToRep(objEmailNotify.RootPath, objEmailNotify.Services, objEmailNotify.Providers, objEmailNotify.PracticeAddress, objEmailNotify.RepName, objEmailNotify.CreatedByName, objEmailNotify.PracticeName, objEmailNotify.ReturnUrl, NotificationTargetType.Lead, CurrentBusinessId.Value, CurrentUser.RelativeUrl);
                            mail.SendDynamicHTMLEmail(toEmails, "New Lead Assigned to You", emailBody, CurrentUser.OtherEmails);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Log();
            }
        }
    }
}
