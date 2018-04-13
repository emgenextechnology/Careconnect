using EmgenexBusinessPortal.Models;
using EBP.Business;
using EBP.Business.Entity;
using EBP.Business.Entity.Account;
using EBP.Business.Entity.Note;
using EBP.Business.Entity.Practice;
using EBP.Business.Filter;
using EBP.Business.Notifications;
using EBP.Business.Repository;
using GM.Identity.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EBP.Business.Entity.EntityNotificationSettings;
using EBP.Business.Enums;

namespace EmgenexBusinessPortal.Controllers
{
    //[RoutePrefix("{Type:regex(account|accounts)}")]
    [RoutePrefix("accounts")]
    [ApiAuthorize]
    public class ApiAccountController : ApiBaseController
    {
        [HttpPost]
        [Route("getbyfilter")]
        [Route("")]
        public IHttpActionResult GetByFilter(FilterAccount filter)
        {
            string[] allowedRoles = { "RDACNT" };
            string[] superRoles = { "RDACNTALL" };
            bool hasSuperRight = HasRight(superRoles);

            if (HasRight(allowedRoles) || hasSuperRight)
            {
                if (filter == null)
                {
                    filter = new FilterAccount();
                    filter.PageSize = 25;
                    filter.CurrentPage = 1;
                }
                var repository = new RepositoryAccount();
                var response = repository.GetAccount(filter, CurrentUser.BusinessId, CurrentUserId, hasSuperRight, IsSalesManager, IsSalesDirector);
                return Ok<DataResponse<EntityList<EntityAccount>>>(response);
            }
            else
            {
                return Ok<DataResponse>(null);
            }
        }

        [Route("getfilter")]
        public IHttpActionResult GetFilter()
        {
            return Ok(new FilterAccount());
        }

        [Route("getaccount/{Id}")]
        [Route("{Id}")]
        public IHttpActionResult GetAccountById(int? Id)
        {
            var response = new DataResponse<EntityAccount>();
            var repository = new RepositoryAccount();
            if (Id.HasValue)
            {
                response = repository.GetAccountById(Id.Value);
            }
            else
            {
                response.Model = new EntityAccount();
                response.Model.Practice = new EntityPractice();
            }
            return Ok<DataResponse>(response);
        }

        [Route("getvmaccount/{id}")]
        [Route("getvmaccount/{id}/edit")]
        public IHttpActionResult GetVmAccountObject(int? id)
        {
            var response = new DataResponse<VMAccount>();
            if (id.HasValue)
            {
                var repository = new RepositoryAccount();
                var model = repository.GetAccountById(id.Value).Model;
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
                        StateId = a.StateId,
                        Zip = a.Zip,
                        AddressTypeId = a.TypeId,
                    }).FirstOrDefault();

                    var practiceContact = model.Practice.Contact.FirstOrDefault(a => a.AddressId == (PracticeAddress != null ? PracticeAddress.AddressId : 0));
                    if (practiceContact != null)
                    {
                        PracticeAddress.ManagerName = practiceContact.ManagerName;
                        PracticeAddress.ManagerEmail = practiceContact.ManagerEmail;
                        PracticeAddress.ManagerPhone = practiceContact.ManagerPhone;
                        PracticeAddress.BillingContact = practiceContact.BillingContact;
                        PracticeAddress.BillingContactEmail = practiceContact.BillingContactEmail;
                        PracticeAddress.BillingContactPhone = practiceContact.BillingContactPhone;
                        PracticeAddress.WorkingHours = practiceContact.officedayshrs;
                    }

                    int? SpecialityTypeId = model.Practice.SpecialityTypeId,
                        RepId = model.Rep.Id,
                        RepGroupId = model.Rep.GroupId;

                    response.Model = new VMAccount
                    {
                        Id = id.Value,
                        EnrolledDate = model.EnrolledDate,
                        PracticeName = model.Practice.Name,
                        PracticeId = model.Practice.Id,
                        SpecialityTypeId = SpecialityTypeId == null ? 0 : (int)SpecialityTypeId,
                        ServiceInterest = model.LeadServiceIntrest,
                        RepGroupId = RepGroupId == null ? 0 : (int)RepGroupId,
                        RepId = RepId == null ? 0 : (int)RepId,
                        AddressId = PracticeAddress == null ? 0 : PracticeAddress.AddressId,
                        PracticeAddressLine1 = PracticeAddress == null ? null : PracticeAddress.AddressLine1,
                        PracticeAddressLine2 = PracticeAddress == null ? null : PracticeAddress.AddressLine2,
                        PhoneNumber = PracticeAddress == null ? null : PracticeAddress.PhoneNumber,
                        PhoneExtension = PracticeAddress == null ? null : PracticeAddress.Extension,
                        City = PracticeAddress == null ? null : PracticeAddress.City,
                        Fax = PracticeAddress == null ? null : PracticeAddress.Fax,
                        StateId = PracticeAddress == null ? null : PracticeAddress.StateId,
                        Zip = PracticeAddress == null ? null : PracticeAddress.Zip,
                        LeadSourceId = model.LeadSourceId,
                        NewSpectialityName = model.Practice.SpecialityType,
                        SpecialityId = model.Practice.Specialities.Count() == 1 ? model.Practice.Specialities.First().PracticeSpecialityId : 0,
                        SpecialityIds = model.Practice.Specialities.Count() > 0 ? model.Practice.Specialities.Select(a => a.PracticeSpecialityId.ToString()).ToArray() : null,
                        EnrolledServices = ((string[])model.ServiceIds),
                        MethodOfContact = model.Practice.ContactPreferenceId,
                        ReportDeliveryEmail = model.Practice.ReportDeliveryEmail,
                        ReportDeliveryFax = model.Practice.ReportDeliveryFax,


                        ManagerName = practiceContact != null ? practiceContact.ManagerName : null,
                        ManagerEmail = practiceContact != null ? practiceContact.ManagerEmail : null,
                        ManagerPhone = practiceContact != null ? practiceContact.ManagerPhone : null,
                        BillingContact = practiceContact != null ? practiceContact.BillingContact : null,
                        BillingContactEmail = practiceContact != null ? practiceContact.BillingContactEmail : null,
                        BillingContactPhone = practiceContact != null ? practiceContact.BillingContactPhone : null,
                        WorkingHours = practiceContact != null ? practiceContact.officedayshrs : null
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
                        StateId = a.StateId,
                        Zip = a.Zip,
                        Fax = a.Fax,
                        AddressTypeId = a.TypeId,
                    }).ToList();

                    foreach (var location in response.Model.Locations)
                    {
                        var contact = model.Practice.Contact.FirstOrDefault(a => a.AddressId == location.AddressId);
                        if (contact != null)
                        {
                            location.ManagerName = contact.ManagerName;
                            location.ManagerEmail = contact.ManagerEmail;
                            location.ManagerPhone = contact.ManagerPhone;
                            location.BillingContact = contact.BillingContact;
                            location.BillingContactEmail = contact.BillingContactEmail;
                            location.BillingContactPhone = contact.BillingContactPhone;
                            location.WorkingHours = contact.officedayshrs;
                        }
                    }

                    response.Model.Providers = model.Practice.Providers.Select(a => new VMProvider
                    {
                        Id = a.ProviderId,
                        DegreeName = a.DegreeName,
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
                            Fax = a.Address.Fax,
                            AddressTypeId = a.Address.AddressTypeId
                        } : response.Model.Locations.FirstOrDefault()
                    }).ToList();

                    return Ok<DataResponse>(response);
                }
                else
                {
                    return Ok<dynamic>(new { IsSuccess = 0, Status = HttpStatusCode.InternalServerError, Message = "No record found!" });

                }
            }

            response.Model = new VMAccount();
            //response.Model.EnrolledDate = DateTime.UtcNow;
            return Ok<DataResponse>(response);
        }

        [HttpPost]
        [Route("Save")]
        [Route("CovertLead")]
        public IHttpActionResult InsertAccountData(VMAccount model)
        {

            if (!IsRep && (model.RepId == null || model.RepId <= 0))
            {
                List<dynamic> errorList = new List<dynamic>();

                if (!HasRight(new string[] { "RDREPGRPALL" }))
                {
                    errorList.Add(new { Message = "You do not have right to read repgroup" });
                }

                if (!HasRight(new string[] { "WRREP" }))
                {
                    errorList.Add(new { Message = "You do not have right to create rep" });
                }

                if (errorList != null && errorList.Count <= 0)
                    errorList.Add(new { Message = "RepGroup and Rep are required" });

                return Ok<dynamic>(new { Status = HttpStatusCode.PreconditionFailed, ResponseMessage = errorList });
            }

            var repository = new RepositoryAccount();
            var response = new DataResponse();
            if (ModelState.IsValid)
            {

                var entityAccount = new EntityAccount
                {
                    EnrolledDate = model.EnrolledDate,
                    IsActive = model.IsActive,
                    LeadSourceId = model.LeadSourceId,
                    LeadServiceIntrest = model.ServiceInterest,
                    RepGroupId = model.RepGroupId,
                    RepId = model.RepId,
                    IsConverted = true,
                    ServiceIds = model.EnrolledServices
                };
                entityAccount.Practice = new EntityPractice
                {
                    //Fax = model.Fax,
                    ReportDeliveryEmail = model.ReportDeliveryEmail,
                    ReportDeliveryFax = model.ReportDeliveryFax,
                    Name = model.PracticeName,
                    SpecialityTypeId = model.SpecialityTypeId,
                    ContactPreferenceId = model.MethodOfContact,
                    Address = new List<EntityPracticeAddress>
                    {
                        new EntityPracticeAddress{
                            Id = model.AddressId ?? 0,
                            AddressIndex = -1,
                            City = model.City,
                            Line1 = model.PracticeAddressLine1,
                            Line2 = model.PracticeAddressLine2,
                            AddressTypeId = 1,
                            StateId = model.StateId ?? 0,
                            Zip= model.Zip,
                            Fax= model.Fax,
                             Phone = new List<EntityPracticePhone> {
                                new EntityPracticePhone {
                                    PhoneNumber = model.PhoneNumber,
                                    Extension = model.PhoneExtension
                                }
                            },
                            AddressId=model.AddressId??0,
                            ManagerName = model.ManagerName,
                            ManagerEmail=model.ManagerEmail,
                            ManagerPhone=model.ManagerPhone,
                            BillingContact=model.BillingContact,
                            BillingContactEmail=model.BillingContactEmail,
                            BillingContactPhone=model.BillingContactPhone,
                            officedayshrs=model.WorkingHours,
                        }
                    },
                    Contact = new List<EntityPracticeContact>
                    {
                        new EntityPracticeContact
                        {
                            BillingContact=model.BillingContact,
                            BillingContactEmail=model.BillingContactEmail,
                            BillingContactPhone=model.BillingContactPhone,
                            ManagerName=model.ManagerName,
                            ManagerPhone=model.ManagerPhone,
                            ManagerEmail=model.ManagerEmail,
                            officedayshrs=model.WorkingHours,
                            AddressId=model.AddressId??0
                        }
                    }
                };

                var specialities = new List<EntityPracticeSpeciality>();

                entityAccount.Practice.Specialities = new List<EntityPracticeSpeciality>();
                entityAccount.PracticeTypeId = model.SpecialityTypeId == 0 ? null : model.SpecialityTypeId;
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
                    entityAccount.Practice.SpecialityType = model.NewSpectialityName;
                }

                if (specialities.Count > 0)
                    entityAccount.Practice.Specialities = specialities;

                if (model.Locations != null)
                {
                    var addressList = new List<EntityPracticeAddress>();
                    var contactList = new List<EntityPracticeContact>();
                    model.Locations.ToList().ForEach(a => addressList.Add(new EntityPracticeAddress
                    {
                        Id = a.AddressId ?? 0,
                        AddressIndex = a.AddressIndex.Value,
                        Line1 = a.AddressLine1,
                        Line2 = a.AddressLine2,
                        City = a.City,
                        AddressTypeId = 2,
                        StateId = a.StateId ?? 0,
                        Zip = a.Zip,
                        Fax = a.Fax,
                        Phone = new List<EntityPracticePhone> {
                            new EntityPracticePhone {
                                PhoneNumber = a.PhoneNumber,
                                Extension = a.Extension
                            }
                        },
                        AddressId = a.AddressId ?? 0,
                        ManagerName = a.ManagerName,
                        ManagerEmail = a.ManagerEmail,
                        ManagerPhone = a.ManagerPhone,
                        BillingContact = a.BillingContact,
                        BillingContactEmail = a.BillingContactEmail,
                        BillingContactPhone = a.BillingContactPhone,
                        officedayshrs = a.WorkingHours,
                    }));

                    if (addressList.Count > 0)
                        entityAccount.Practice.Address = entityAccount.Practice.Address.Concat(addressList);

                    entityAccount.Practice.Address.ToList().ForEach(a => contactList.Add(new EntityPracticeContact
                    {
                        AddressId = a.AddressId,
                        ManagerName = a.ManagerName,
                        ManagerEmail = a.ManagerEmail,
                        ManagerPhone = a.ManagerPhone,
                        BillingContact = a.BillingContact,
                        BillingContactEmail = a.BillingContactEmail,
                        BillingContactPhone = a.BillingContactPhone,
                        officedayshrs = a.officedayshrs,
                    }));

                    if (contactList.Count > 0)
                        entityAccount.Practice.Contact = contactList;
                }
                if (model.Providers != null)
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
                            Id = a.Id ?? 0,
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
                                Fax = a.Location.Fax,
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
                        entityAccount.Practice.Providers = providerList;
                }
                entityAccount.UpdatedBy = entityAccount.CreatedBy = entityAccount.CurrentUserId = CurrentUser.Id;
                entityAccount.BusinessId = CurrentUser.BusinessId;
                entityAccount.CreatedByName = string.Format("{0} {1}", CurrentUser.FirstName, CurrentUser.LastName);

                if (model.Id > 0) //Update
                {
                    entityAccount.Id = model.Id ?? 0;
                    response = repository.Update(entityAccount);
                }
                else
                {
                    if (model.LeadId > 0) //Converted from Lead
                    {
                        entityAccount.LeadId = model.LeadId;
                        response = repository.ConvertToAccount(entityAccount);
                    }
                    else //New account
                    {
                        response = repository.Insert(entityAccount);
                    }

                    if (response.Id != null)
                    {
                        string services = string.Empty, providers = string.Empty, practiceAddress = string.Empty;

                        if (entityAccount.ServiceIds != null)
                        {
                            var ids = ((IEnumerable<string>)entityAccount.ServiceIds).Select(a => Convert.ToInt32(a)).ToArray();
                            services = string.Join(", ", new RepositoryLookups().GetServicesById(ids));
                        }

                        foreach (var item in entityAccount.Practice.Providers)
                        {
                            providers = providers + item.FirstName + " " + item.LastName + " (" + item.NPI + ") <br />";
                        }

                        var primaryAddress = entityAccount.Practice.Address.First();

                        var stateName = new RepositoryLookups().GetAllStates().Model.List.Where(a => a.Id == primaryAddress.StateId).First().Value;
                        practiceAddress = string.Format("{0}, {1}, {2}, {3}, {4}", primaryAddress.Line1, primaryAddress.Line2, primaryAddress.City, stateName, primaryAddress.Zip);

                        EntityGroupManagerDetails RepModel = repository.RepDetailsByAccountId(response.Id).Model;

                        EmailNotification emailNotify = new EmailNotification
                        {
                            PracticeName = model.PracticeName,
                            PracticeAddress = practiceAddress,
                            Providers = providers,
                            Services = services,
                            RepFirstName = RepModel.RepFirstName,
                            RepMiddleName = RepModel.RepMiddleName,
                            RepLastName = RepModel.RepLastName,
                            RepEmail = RepModel.RepEmail,
                            Managers = RepModel.Managers,
                            CurrentUserFirstName = CurrentUser.FirstName,
                            CurrentUserLastName = CurrentUser.LastName,
                            RootPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath,
                            ReturnUrl = ConfigurationManager.AppSettings["BaseUrl"],
                        };
                        NewAccountEmailNotification(emailNotify);
                    }
                }
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
            var repository = new RepositoryAccount();
            var response = repository.SetFlag(id);
            return Ok<DataResponse>(response);
        }

        [HttpPost]
        [Route("togglestatus/{id}")]
        [Route("{id}/togglestatus")]
        public IHttpActionResult SetLeadStatus(int id)
        {
            var repository = new RepositoryAccount();
            var response = repository.SetActiveStatus(id);
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
        [AllowAnonymous]
        public IHttpActionResult GetProviderObject()
        {
            return Ok<VMProvider>(new VMProvider());
        }

        private void NewAccountEmailNotification(EmailNotification objEmailNotify)
        {
            try
            {
                var mail = new GMEmail();
                string managerEmailBody = string.Empty;

                //email to manager
                {
                    if (objEmailNotify.Managers != null && objEmailNotify.Managers.Count() > 0)
                    {
                        foreach (var item in objEmailNotify.Managers)
                        {
                            if (CurrentUser.Email != item.Email)
                            {
                                string toEmails = new RepositoryUserProfile().NotificationEnabledEmails(item.Email, "ACCNFN");
                                if (!string.IsNullOrEmpty(toEmails))
                                {
                                    managerEmailBody = TemplateManager.NewPracticeToManager(objEmailNotify.RootPath, objEmailNotify.Services, objEmailNotify.Providers, objEmailNotify.PracticeAddress, item.Name, objEmailNotify.RepName, objEmailNotify.CreatedByName, objEmailNotify.PracticeName, objEmailNotify.ReturnUrl, NotificationTargetType.Account, CurrentBusinessId.Value, CurrentUser.RelativeUrl);
                                    mail.SendDynamicHTMLEmail(toEmails, "New Account Created", managerEmailBody, CurrentUser.OtherEmails);
                                }
                            }
                        }
                    }
                }

                //email to Rep
                {
                    if (CurrentUser.Email != objEmailNotify.RepEmail)
                    {
                        string toEmails = new RepositoryUserProfile().NotificationEnabledEmails(objEmailNotify.RepEmail, "ACCNFN");
                        if (!string.IsNullOrEmpty(toEmails))
                        {
                            var emailBody = TemplateManager.NewPracticeToRep(objEmailNotify.RootPath, objEmailNotify.Services, objEmailNotify.Providers, objEmailNotify.PracticeAddress, objEmailNotify.RepName, objEmailNotify.CreatedByName, objEmailNotify.PracticeName, objEmailNotify.ReturnUrl, NotificationTargetType.Account, CurrentBusinessId.Value, CurrentUser.RelativeUrl);
                            mail.SendDynamicHTMLEmail(toEmails, "New Account Assigned to You", emailBody, CurrentUser.OtherEmails);
                        }
                    }
                }

                if (CurrentBusinessId == 1)
                {
                    var addressEmailBody = TemplateManager.NewAccountAddress(objEmailNotify.RootPath, objEmailNotify.Services, objEmailNotify.Providers, objEmailNotify.PracticeAddress, objEmailNotify.CreatedByName, objEmailNotify.PracticeName, objEmailNotify.RepName, CurrentBusinessId.Value, CurrentUser.RelativeUrl);
                    string addressNotification = ConfigurationManager.AppSettings["AddressNotification"];
                    string[] AddressNotification = !string.IsNullOrEmpty(addressNotification) ? addressNotification.Split(',') : null;
                    if (AddressNotification != null && AddressNotification.Count() > 0)
                    {
                        foreach (var item in AddressNotification)
                        {
                            mail.SendDynamicHTMLEmail(item, "New Location Added", addressEmailBody, CurrentUser.OtherEmails);
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
