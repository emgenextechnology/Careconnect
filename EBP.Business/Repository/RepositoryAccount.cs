using EBP.Business.Entity;
using EBP.Business.Entity.Account;
using EBP.Business.Entity.EntityNotificationSettings;
using EBP.Business.Entity.Practice;
using EBP.Business.Entity.Rep;
using EBP.Business.Enums;
using EBP.Business.Filter;
using EBP.Business.Resource;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;


namespace EBP.Business.Repository
{
    public class RepositoryAccount : _Repository
    {
        public DataResponse<EntityList<EntityAccount>> GetAccount(FilterAccount filter, int? businessId, int currentUserId, bool isBuzAdmin, bool isSalesManager, bool isSalesDirector, int take = 10, int skip = 0)
        {
            var response = new DataResponse<EntityList<EntityAccount>>();
            try
            {
                if (filter != null)
                {
                    take = filter.Take;
                    skip = filter.Skip;
                }

                base.DBInit();

                var query = DBEntity.Accounts.Join(
                    DBEntity.Leads,
                    accounts => accounts.LeadId,
                    lead => lead.Id,
                    (accounts, lead) => new { Accounts = accounts, Lead = lead }
                    ).Where(a => a.Lead.IsConverted == true && a.Accounts.BusinessId == businessId);

                if (!isBuzAdmin)
                {
                    // Note : implemented property isSalesManager
                    //var reps = DBEntity.Reps.Any(a => a.RepGroup.RepgroupManagerMappers.Any(b => b.ManagerId == currentUserId));//---ManagerChange     var reps = DBEntity.Reps.Any(a => a.RepGroup.ManagerId == currentUserId);
                    if (isSalesDirector || isSalesManager)
                    {
                        //Manager
                        query = query.Where(a => a.Lead.Rep.UserId == currentUserId || a.Lead.Rep.RepGroup.RepgroupManagerMappers.Any(b => b.ManagerId == currentUserId)); //|| a.Lead.Rep.RepGroup.SalesDirectorId == currentUserId
                        //---ManagerChange     query = query.Where(a => a.Lead.Rep.UserId == currentUserId || a.Lead.Rep.RepGroup.ManagerId == currentUserId);
                    }
                    else
                    {
                        //Rep
                        query = query.Where(a => a.Lead.Rep.UserId == currentUserId);
                    }
                }

                if (filter != null)
                {
                    if (!String.IsNullOrEmpty(filter.KeyWords))
                        query = query.Where(a => a.Lead.Practice.PracticeName.ToLower().Contains(filter.KeyWords.ToLower())
                            || a.Lead.Practice.PracticeProviderMappers.Any(p => ((p.Provider.FirstName + p.Provider.MiddleName + p.Provider.LastName + p.Provider.NPI).ToLower()).Contains(filter.KeyWords.ToLower()))
                            );

                    //if (filter.IsActive != null)
                    //    query = query.Where(a => a.IsActive == filter.IsActive);
                    //else
                    //    query = query.Where(a => a.IsActive == true);

                    if (filter.HasFlag.HasValue)
                        query = query.Where(a => a.Lead.HasFlag == filter.HasFlag);

                    if (filter.HasTask.HasValue)
                        query = query.Where(a => a.Lead.Practice.Tasks.Any(b => b.Status != 3 && (b.TaskUsers.Any(tu => tu.UserId == currentUserId) || b.RequestedBy == currentUserId)));

                    if (filter.HasReports.HasValue)
                        query = query.Where(a => a.Lead.Practice.ReportMasters.Any());

                    if (filter.AccountStatus.HasValue)
                    {
                        //var oneMonth = DateTime.Now.AddMonths(-1);
                        //var twoMonth = DateTime.Now.AddMonths(-2);
                        //var threeMonth = DateTime.Now.AddMonths(-3);
                        //var twoDays = DateTime.Now.AddDays(-2);
                        var sixtyDays = DateTime.UtcNow.AddDays(-60);
                        switch (filter.AccountStatus)
                        {
                            case 1:
                                query = query.Where(a => a.Lead.Practice.ReportMasters.Count() == 0 && a.Accounts.CreatedOn > sixtyDays);
                                break;

                            case 2:
                                query = query.Where(a => a.Lead.Practice.ReportMasters.Any(r => r.CreatedOn >= sixtyDays) && a.Accounts.CreatedOn >= sixtyDays);
                                break;

                            case 3:
                                query = query.Where(a => !a.Lead.Practice.ReportMasters.Any(r => r.CreatedOn >= sixtyDays) && a.Accounts.CreatedOn <= sixtyDays);
                                break;

                            case 4:
                                query = query.Where(a => a.Accounts.IsActive == false);
                                break;
                        }
                    }
                    //else
                    //    query = query.Where(a => a.Accounts.IsActive == true);

                    if (filter.Period != null)
                    {
                        Periods periodFilter = (Periods)filter.Period;
                        var dt = DateTime.UtcNow;
                        var endDt = DateTime.UtcNow;

                        #region Periods

                        switch (periodFilter)
                        {
                            case Periods.Today:
                                dt = DateTime.UtcNow.AddHours(-12);
                                query = query.Where(a => a.Accounts.CreatedOn > dt);
                                break;

                            case Periods.Yesterday:
                                dt = DateTime.UtcNow.AddHours(-12);
                                endDt = dt.AddHours(-12);
                                query = query.Where(a => a.Accounts.CreatedOn < dt && a.Accounts.CreatedOn > endDt);
                                break;


                            case Periods.ThisWeek:

                                dt = DateTime.UtcNow.AddDays(-7);
                                query = query.Where(a => a.Accounts.CreatedOn > dt);
                                break;


                            case Periods.LastWeek:

                                dt = DateTime.UtcNow.AddDays(-7);
                                endDt = dt.AddDays(-7);

                                query = query.Where(a => a.Accounts.CreatedOn < dt && a.Accounts.CreatedOn > endDt);

                                break;


                            case Periods.ThisMonth:

                                dt = DateTime.UtcNow.AddDays(-30);

                                query = query.Where(a => a.Accounts.CreatedOn > dt);
                                break;

                            case Periods.LastMonth:

                                dt = DateTime.UtcNow.AddDays(-30);
                                endDt = dt.AddDays(-30);


                                query = query.Where(a => a.Accounts.CreatedOn < dt && a.Accounts.CreatedOn > endDt);
                                break;

                            case Periods.ThisYear:

                                dt = DateTime.UtcNow.AddDays(-365);

                                query = query.Where(a => a.Accounts.CreatedOn > dt);
                                break;

                            case Periods.LastYear:

                                dt = DateTime.UtcNow.AddDays(-365);
                                endDt = dt.AddDays(-365);


                                query = query.Where(a => a.Accounts.CreatedOn < dt && a.Accounts.CreatedOn > endDt);
                                break;

                            default:
                                break;
                        }

                        #endregion
                    }

                    if (filter.RepGroupIds != null && filter.RepGroupIds.Length > 0)
                        query = query.Where(a => filter.RepGroupIds.Contains((int)a.Lead.Practice.Rep.RepGroupId));

                    if (filter.RepIds != null && filter.RepIds.Length > 0)
                        query = query.Where(a => filter.RepIds.Contains((int)a.Lead.RepId));

                    //if (filter.AccountStatuses != null && filter.AccountStatuses.Length > 0)
                    // query = query.Where(a => filter.AccountStatuses.Contains((int)a.Lead.LeadStatus));

                    if (filter.ProviderIds != null && filter.ProviderIds.Length > 0)
                    {
                        var practiceIds = DBEntity.PracticeProviderMappers.Where(a => filter.ProviderIds.Contains(a.ProviderId)).Select(a => a.PracticeId).ToArray();
                        query = query.Where(a => practiceIds.Contains(a.Lead.PracticeId));
                    }

                    if (filter.ServiceIds != null && filter.ServiceIds.Length > 0)
                    {
                        var practiceIds = DBEntity.PracticeServiceMappers.Where(a => filter.ServiceIds.Contains(a.EnrolledServiceId)).Select(a => a.PracticeId).ToArray();
                        query = query.Where(a => practiceIds.Contains(a.Lead.PracticeId));
                    }
                }

                var _3monthsBefore = Statics.UtcNow.AddMonths(-3);
                var _2daysBefore = Statics.UtcNow.AddDays(-2);

                var selectQuery = query.Select(a => new EntityAccount
                {
                    Id = a.Accounts.Id,
                    ServiceNames = a.Lead.Practice.PracticeServiceMappers.Select(b => b.LookupEnrolledService.ServiceName),
                    ProviderNames = a.Lead.Practice.PracticeProviderMappers.Select(b => b.Provider.FirstName + " " + b.Provider.MiddleName + " " + b.Provider.LastName),
                    ProvidersCount = a.Lead.Practice.PracticeProviderMappers.Count,
                    NotesCount = DBEntity.Notes.Where(k => k.ParentId == a.Lead.Practice.Id && k.IsDeleted!=false).Count(),
                    LeadId = a.Lead.Id,
                    Practice = new EntityPractice
                    {
                        Id = a.Lead.Practice.Id,
                        Name = a.Lead.Practice.PracticeName,
                        ReportDeliveryEmail = a.Lead.Practice.ReportDeliveryPreference,
                        ReportDeliveryFax = a.Lead.Practice.ReportDeliveryFax
                    },
                    Rep = new EntityRep
                    {
                        Id = a.Lead.RepId,
                        GroupId = a.Lead.Rep.RepGroupId,
                        GroupName = a.Lead.Rep.RepGroup.RepGroupName,
                        FirstName = a.Lead.Rep.User2.FirstName,
                        LastName = a.Lead.Rep.User2.LastName,
                        Username = a.Lead.Rep.User2.UserName,
                    },
                    IsActive = a.Accounts.IsActive,
                    EnrolledDate = a.Accounts.EnrolledDate,
                    Status = a.Lead.LeadStatus == 1 ? "New" : "Transacted",
                    HasFlag = a.Lead.HasFlag,
                    HasTask = filter.HasTask.HasValue ? true : a.Lead.Practice.Tasks.Count(task => task.Status != 3 && (task.TaskUsers.Any(tu => tu.UserId == currentUserId) || task.RequestedBy == currentUserId)) > 0,
                    CreatedBy = a.Accounts.CreatedBy,
                    CreatedOn = a.Accounts.CreatedOn,
                    UpdatedBy = a.Accounts.UpdatedBy,
                    UpdatedOn = a.Accounts.UpdatedOn,
                    LastActivity = a.Lead.Practice.ReportMasters.OrderByDescending(o => o.Id).FirstOrDefault(),
                    ReportCount = a.Lead.Practice.ReportMasters.Count()
                });

                if (string.IsNullOrEmpty(filter.SortKey))
                {
                    selectQuery = selectQuery.OrderByDescending(o => o.CreatedOn);
                }
                else
                {
                    switch (filter.SortKey)
                    {
                        case "PracticeName":
                            if (filter.OrderBy == "asc")
                                selectQuery = selectQuery.OrderBy(o => o.Practice.Name + " " + filter.OrderBy);
                            else
                                selectQuery = selectQuery.OrderByDescending(o => o.Practice.Name + " " + filter.OrderBy);
                            break;
                        case "ServicesCount":
                            if (filter.OrderBy == "asc")
                                selectQuery = selectQuery.OrderBy(o => ((IEnumerable<string>)o.ServiceNames).Count());
                            else
                                selectQuery = selectQuery.OrderByDescending(o => ((IEnumerable<string>)o.ServiceNames).Count());
                            break;
                        case "ProvidersCount":
                            if (filter.OrderBy == "asc")
                                selectQuery = selectQuery.OrderBy(o => o.ProvidersCount);
                            else
                                selectQuery = selectQuery.OrderByDescending(o => o.ProvidersCount);
                            break;
                        case "RepGroupName":
                            if (filter.OrderBy == "asc")
                                selectQuery = selectQuery.OrderBy(o => o.Rep.GroupName + " " + filter.OrderBy);
                            else
                                selectQuery = selectQuery.OrderByDescending(o => o.Rep.GroupName + " " + filter.OrderBy);
                            break;
                        case "RepName":
                            if (filter.OrderBy == "asc")
                                selectQuery = selectQuery.OrderBy(o => o.Rep.FirstName + " " + o.Rep.LastName);
                            else
                                selectQuery = selectQuery.OrderByDescending(o => o.Rep.FirstName + " " + o.Rep.LastName);
                            break;
                        case "ActivityStatus":
                            if (filter.OrderBy == "asc")
                                selectQuery = selectQuery.OrderBy(o => o.LastActivity.CreatedOn).ThenBy(t => t.CreatedOn);
                            else
                                selectQuery = selectQuery.OrderByDescending(o => o.LastActivity.CreatedOn).ThenByDescending(t => t.CreatedOn);
                            break;
                        case "NotesCount":
                            if (filter.OrderBy == "asc")
                                selectQuery = selectQuery.OrderBy(o => o.NotesCount + " " + filter.OrderBy);
                            else
                                selectQuery = selectQuery.OrderByDescending(o => o.NotesCount + " " + filter.OrderBy);
                            break;
                        //case "ActivityStatus":
                        //    if (filter.OrderBy == "asc")
                        //        selectQuery = selectQuery.OrderBy(o => o.LastActivityDate + " " + filter.OrderBy);
                        //    else
                        //        selectQuery = selectQuery.OrderByDescending(o => o.LastActivityDate + " " + filter.OrderBy);
                        //    break;
                        default:
                            string orderBy = string.Format("{0} {1}", filter.SortKey, filter.OrderBy);
                            selectQuery = selectQuery.OrderBy(orderBy);
                            break;
                    }
                }
                response = GetList<EntityAccount>(selectQuery, skip, take);
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }
            return response;
        }

        public DataResponse<EntityAccount> GetAccountById(int AccountId)
        {
            var response = new DataResponse<EntityAccount>();
            try
            {
                base.DBInit();
                var query = DBEntity.Accounts.Where(a => a.Id == AccountId && a.Lead.IsConverted == true).Select(a => new EntityAccount
                {
                    Id = a.Id,
                    LeadId = a.LeadId,
                    LeadSourceId = a.Lead.LeadSourceId,
                    LeadSourceName = a.Lead.LookupLeadSource.LeadSource,
                    LeadServiceIntrest = a.Lead.LeadServiceIntrest,
                    OtherLeadSource = a.Lead.OtherLeadSource,
                    IsActive = a.IsActive,
                    NotesCount = DBEntity.Notes.Where(k => k.ParentId == a.Lead.Practice.Id).Count(),
                    ServicesCount = a.Lead.Practice.PracticeServiceMappers.Count,
                    ProviderNames = a.Lead.Practice.PracticeProviderMappers.Select(b => b.Provider.FirstName + " " + b.Provider.MiddleName + " " + b.Provider.LastName),
                    ProvidersCount = a.Lead.Practice.PracticeProviderMappers.Count,
                    Rep = new EntityRep
                    {
                        Id = a.Lead.RepId,
                        GroupId = a.Lead.Rep.RepGroupId,
                        GroupName = a.Lead.Rep.RepGroup.RepGroupName,
                        FirstName = a.Lead.Rep.User2.FirstName,
                        LastName = a.Lead.Rep.User2.LastName,
                        Username = a.Lead.Rep.User2.UserName,
                        Managers = a.Lead.Rep.RepGroup.RepgroupManagerMappers.Select(b => new Manager
                        {
                            FirstName = b.User.FirstName,
                            LastName = b.User.LastName,
                            Email = b.User.Email
                        }),
                        //Manager = new Manager
                        //{
                        //    FirstName = a.Lead.Rep.RepGroup.User2.FirstName,
                        //    LastName = a.Lead.Rep.RepGroup.User2.LastName,
                        //    UserName = a.Lead.Rep.RepGroup.User2.UserName
                        //}
                    },
                    HasFlag = a.Lead.HasFlag,
                    HasTask = a.Lead.Practice.Tasks.Count > 0,
                    EnrolledDate = a.EnrolledDate,
                    ServiceIds = a.Lead.Practice.PracticeServiceMappers.Select(b => b.EnrolledServiceId.ToString()),
                    ServiceNames = a.Lead.Practice.PracticeServiceMappers.Select(b => b.LookupEnrolledService.ServiceName),
                    Practice = new EntityPractice
                    {
                        Id = a.Lead.Practice.Id,
                        Name = a.Lead.Practice.PracticeName,
                        SpecialityType = a.Lead.Practice.PracticeSpecialityType,
                        SpecialityTypeId = a.Lead.Practice.PracticeTypeId,
                        Email = a.Lead.Practice.Email,
                        //Fax = a.Lead.Practice.FaxNumber,
                        ContactPreferenceId = a.Lead.Practice.ContactPreferenceId,
                        ReportDeliveryEmail = a.Lead.Practice.ReportDeliveryPreference,
                        ReportDeliveryFax = a.Lead.Practice.ReportDeliveryFax,
                        Specialities = a.Lead.Practice.PracticePracticeSpecialityMappers.Select(s => new EntityPracticeSpeciality
                        {
                            PracticeSpecialityId = s.LookupPracticeSpecialityType.Id,
                            NewSpecialityName = s.LookupPracticeSpecialityType.PracticeSpecialityType
                        }),
                        Providers = a.Lead.Practice.PracticeProviderMappers.Where(b => b.PracticeId == a.Lead.PracticeId).Select(s => new EntityProvider
                        {
                            ProviderId = s.Provider.Id,
                            FirstName = s.Provider.FirstName,
                            MiddleName = s.Provider.MiddleName,
                            LastName = s.Provider.LastName,
                            DegreeId = s.Provider.DegreeId,
                            DegreeName = s.Provider.LookupDegree.DegreeName,
                            ShortCode = s.Provider.LookupDegree.ShortCode,
                            NPI = s.Provider.NPI,
                            Address = s.Provider.PracticeProviderLocationMappers.Where(b => b.PracticeId == a.Lead.Practice.Id).Select(m => new EntityProviderAddress
                            {
                                TypeId = m.Address.AddressTypeId,
                                AddressId = m.Address.Id,
                                Line1 = m.Address.Line1,
                                Line2 = m.Address.Line2,
                                City = m.Address.City,
                                Zip = m.Address.Zip,
                                Fax = m.Address.Fax, //?? a.Lead.Practice.FaxNumber,
                                AddressTypeId = m.Address.AddressTypeId,
                                StateId = m.Address.StateId,
                                State = m.Address.LookupState.StateName,
                                Phone = m.Address.Phones.Select(p => new EntityPracticePhone
                                {
                                    PhoneId = p.Id,
                                    PhoneNumber = p.PhoneNumber,
                                    Extension = p.Extension
                                }),
                            }).FirstOrDefault()
                        }),
                        Address = a.Lead.Practice.PracticeAddressMappers.Select(s => new EntityPracticeAddress
                        {
                            TypeId = s.Address.AddressTypeId,
                            AddressId = s.Address.Id,
                            Line1 = s.Address.Line1,
                            Line2 = s.Address.Line2,
                            City = s.Address.City,
                            Zip = s.Address.Zip,
                            Fax = s.Address.Fax,
                            AddressTypeId = s.Address.AddressTypeId,
                            StateId = s.Address.StateId,
                            State = s.Address.LookupState.StateName,
                            Phone = s.Address.Phones.Select(p => new EntityPracticePhone
                            {
                                PhoneId = p.Id,
                                PhoneNumber = p.PhoneNumber,
                                Extension = p.Extension
                            })
                        }),
                        Contact = a.Lead.Practice.PracticeContacts.Select(f => new EntityPracticeContact
                        {
                            AddressId = f.AddressId ?? 0,
                            BillingContact = f.BillingContact,
                            BillingContactEmail = f.BillingContactEmail,
                            BillingContactPhone = f.BillingContactPhone,
                            ManagerName = f.ManagerName,
                            ManagerEmail = f.ManagerEmail,
                            ManagerPhone = f.ManagerPhone,
                            officedayshrs = f.officedayshrs,
                        })
                    },
                    CreatedOn = a.CreatedOn,
                    CreatedBy = a.CreatedBy,
                    CreatedByName = a.User.FirstName + " " + a.User.LastName,

                    UpdatedOn = a.UpdatedOn,
                    UpdatedBy = a.UpdatedBy,
                    UpdatedByName = a.User1.FirstName + " " + a.User1.LastName
                });
                response = GetFirst<EntityAccount>(query);

                foreach (var item in response.Model.Practice.Address)
                {
                    var objContact = response.Model.Practice.Contact.Where(b => b.AddressId == item.AddressId).FirstOrDefault();
                    if (objContact != null)
                    {
                        item.ManagerName = objContact.ManagerName;
                        item.ManagerEmail = objContact.ManagerEmail;
                        item.ManagerPhone = objContact.ManagerPhone;
                        item.BillingContact = objContact.BillingContact;
                        item.BillingContactEmail = objContact.BillingContactEmail;
                        item.BillingContactPhone = objContact.BillingContactPhone;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }
            return response;
        }

        public DataResponse Insert(EntityAccount entity)
        {
            var response = new DataResponse<EntityAccount>();
            try
            {
                base.DBInit();

                #region Genrarate PracticeProviderMapper

                var entityProviders = entity.Practice.Providers;
                List<Database.PracticeProviderMapper> oPracticeProviderMappers = new List<Database.PracticeProviderMapper>();
                foreach (var oProvider in entityProviders)
                {
                    var provider = DBEntity.Providers.FirstOrDefault(a => a.NPI == oProvider.NPI);
                    int providerId = provider != null ? provider.Id : 0;
                    if (providerId > 0)
                    {
                        oPracticeProviderMappers.Add(new Database.PracticeProviderMapper
                        {
                            AddressIndex = oProvider.AddressIndex,
                            ProviderId = providerId,
                            CreatedBy = entity.CreatedBy,
                            CreatedOn = entity.CreatedOn
                        });
                    }
                    else
                    {
                        oPracticeProviderMappers.Add(new Database.PracticeProviderMapper
                        {
                            AddressIndex = oProvider.AddressIndex,
                            Provider = new Database.Provider
                            {
                                FirstName = oProvider.FirstName,
                                MiddleName = oProvider.MiddleName,
                                LastName = oProvider.LastName,
                                IsActive = true,
                                NPI = oProvider.NPI,
                                DegreeId = oProvider.DegreeId,
                                CreatedBy = entity.CreatedBy,
                                CreatedOn = entity.CreatedOn,
                            },
                            CreatedBy = entity.CreatedBy,
                            CreatedOn = entity.CreatedOn
                        });

                    }
                }

                #endregion

                #region Genrarate PracticeServiceMapper

                List<Database.PracticeServiceMapper> oPracticeServiceMapper = new List<Database.PracticeServiceMapper>();

                string[] serviceIds = ((string[])entity.ServiceIds);

                if (serviceIds != null && serviceIds.Count() > 0)
                {
                    foreach (var serviceId in serviceIds)
                    {
                        oPracticeServiceMapper.Add(new Database.PracticeServiceMapper
                        {
                            EnrolledServiceId = Convert.ToInt32(serviceId),
                            CreatedBy = entity.CreatedBy,
                            CreatedOn = entity.CreatedOn
                        });
                    }
                }

                #endregion

                if (entity.RepId == null)
                {
                    var objRep = DBEntity.Reps.Where(a => a.UserId == entity.CurrentUserId).FirstOrDefault();
                    entity.RepId = objRep.Id;
                }

                #region Accounts

                var model = new Database.Account
                {
                    EnrolledDate = entity.EnrolledDate,
                    IsActive = true,
                    BusinessId = entity.BusinessId,
                    Lead = new Database.Lead
                    {
                        IsConverted = entity.IsConverted,
                        LeadSourceId = entity.LeadSourceId,
                        LeadServiceIntrest = entity.LeadServiceIntrest,
                        OtherLeadSource = entity.OtherLeadSource,
                        IsActive = true,
                        RepId = entity.RepId,
                        LeadStatus = (int)LeadStatus.Transacted,
                        BusinessId = entity.BusinessId,
                        Practice = new Database.Practice
                        {
                            ReportDeliveryPreference = entity.Practice.ReportDeliveryEmail,
                            ReportDeliveryFax = entity.Practice.ReportDeliveryFax,
                            ContactPreferenceId = entity.Practice.ContactPreferenceId,
                            RepId = entity.RepId,
                            PracticeName = entity.Practice.Name,
                            PracticeTypeId = entity.PracticeTypeId,
                            PracticeSpecialityType = entity.Practice.SpecialityType,
                            Email = entity.Practice.Email,
                            //FaxNumber = entity.Practice.Fax,
                            BusinessId = entity.BusinessId,
                            PracticePracticeSpecialityMappers = (entity.Practice.Specialities == null || !string.IsNullOrEmpty(entity.Practice.SpecialityType)) ? null : entity.Practice.Specialities.Select(a => new Database.PracticePracticeSpecialityMapper
                            {
                                PracticeSpecialityId = a.PracticeSpecialityId,
                                CreatedBy = entity.CreatedBy,
                                CreatedOn = entity.CreatedOn
                            }).ToList(),
                            PracticeAddressMappers = entity.Practice.Address == null ? null : entity.Practice.Address.Select(a => new Database.PracticeAddressMapper
                            {
                                Address = new Database.Address
                                {
                                    AddressIndex = a.AddressIndex,
                                    Line1 = a.Line1,
                                    Line2 = a.Line2,
                                    City = a.City,
                                    Zip = a.Zip,
                                    Fax = a.Fax,
                                    AddressTypeId = a.AddressTypeId,
                                    StateId = a.StateId,
                                    CreatedBy = entity.CreatedBy,
                                    CreatedOn = entity.CreatedOn,
                                    Phones = a.Phone == null ? null : a.Phone.Select(b => new Database.Phone
                                    {
                                        PhoneNumber = b.PhoneNumber,
                                        Extension = b.Extension,
                                        PhoneTypeId = 1,
                                        CreatedOn = entity.CreatedOn,
                                        CreatedBy = entity.CreatedBy
                                    }).ToList(),
                                    PracticeContacts = new List<Database.PracticeContact>{
                                        new Database.PracticeContact
                                        {
                                            BillingContact=a.BillingContact,
                                            BillingContactEmail=a.BillingContactEmail,
                                            BillingContactPhone=a.BillingContactPhone,
                                            ManagerName=a.ManagerName,
                                            ManagerEmail=a.ManagerEmail,
                                            ManagerPhone=a.ManagerPhone,
                                            officedayshrs=a.officedayshrs,
                                            CreatedOn = entity.CreatedOn,
                                            CreatedBy = entity.CreatedBy
                                        }
                                    }
                                },
                                ManagerName = a.ManagerName,
                                CreatedBy = entity.CreatedBy,
                                CreatedOn = entity.CreatedOn,
                            }).ToList(),
                            PracticeProviderMappers = oPracticeProviderMappers,
                            PracticeServiceMappers = oPracticeServiceMapper,
                            CreatedOn = entity.CreatedOn,
                            CreatedBy = entity.CreatedBy
                        },
                        CreatedOn = entity.CreatedOn,
                        CreatedBy = entity.CreatedBy
                    },
                    CreatedOn = entity.CreatedOn,
                    CreatedBy = entity.CreatedBy
                };

                #endregion

                if (base.DBSave(model) > 0)
                {
                    #region Practice Provider Address Mapper

                    model.Lead.Practice.PracticeProviderMappers.ToList().ForEach(s =>
                        DBEntity.PracticeProviderLocationMappers.Add(new Database.PracticeProviderLocationMapper
                        {
                            PracticeId = model.Lead.Practice.Id,
                            ProviderId = s.ProviderId,
                            AddressId = s.Practice.PracticeAddressMappers
                                            .FirstOrDefault(f => f.Address.AddressIndex == s.Practice.PracticeProviderMappers
                                                .FirstOrDefault(d => d.Provider.Id == s.ProviderId).AddressIndex).AddressId,
                            CreatedBy = entity.CreatedBy,
                            CreatedOn = entity.CreatedOn
                        }));

                    #endregion

                    model.Lead.Practice.PracticeAddressMappers.ToList().ForEach(a => a.Address.PracticeContacts.FirstOrDefault().PracticeId = model.Lead.Practice.Id);

                    DBEntity.SaveChanges();

                    entity.LeadId = model.Id;

                    var RepDetails = DBEntity.Reps.FirstOrDefault(a => a.Id == model.Lead.RepId);
                    if (RepDetails.RepGroup.RepgroupManagerMappers.Count > 0)
                    {
                        entity.Rep = new EntityRep
                        {
                            Managers = RepDetails.RepGroup.RepgroupManagerMappers.Select(b => new Manager
                            {
                                Id = b.User.Id,
                                FirstName = b.User.FirstName,
                                LastName = b.User.LastName,
                                Email = b.User.Email
                            }).ToList(),

                            FirstName = RepDetails.User2.FirstName,
                            LastName = RepDetails.User2.LastName,
                            Username = RepDetails.User2.UserName,
                        };
                    }
                    else
                    {
                        entity.Rep = new EntityRep
                        {
                            //Manager = new Manager
                            //{
                            //    Id = RepDetails.RepGroup.User2.Id,
                            //    FirstName = RepDetails.RepGroup.User2.FirstName,
                            //    LastName = RepDetails.RepGroup.User2.LastName,
                            //    UserName = RepDetails.RepGroup.User2.UserName,
                            //},

                            FirstName = RepDetails.User2.FirstName,
                            LastName = RepDetails.User2.LastName,
                            Username = RepDetails.User2.UserName,
                        };
                    }

                    #region Save Notification Data

                    List<EntityNotification> notificationlist = new List<EntityNotification>();
                    int MangerId = 0;

                    if (entity.Rep.Managers.Count() > 0)
                    {
                        var mangerDetails = entity.Rep.Managers.FirstOrDefault();
                        MangerId = mangerDetails.Id;
                    }
                    if (MangerId == entity.CreatedBy)
                    {

                        notificationlist.Add(new EntityNotification
                        {
                            UserId = RepDetails.User2.Id,
                            Message = NotificationMessages.AccountManagerNotification
                        });
                    }
                    else if (entity.RepId == entity.CreatedBy)
                    {

                        notificationlist.Add(new EntityNotification
                        {
                            UserId = MangerId,
                            Message = NotificationMessages.AccountRepNotification
                        });
                    }
                    else
                    {
                        notificationlist.Add(new EntityNotification
                        {
                            UserId = RepDetails.User2.Id,
                            Message = NotificationMessages.AccountManagerNotification
                        });

                        notificationlist.Add(new EntityNotification
                        {
                            UserId = MangerId,
                            Message = NotificationMessages.AccountRepNotification
                        });
                    }

                    new RepositoryNotification().Save(notificationlist, entity.CurrentUserId, model.Id, (int)NotificationTargetType.Account, (int)NotificationType.Normal, entity.CreatedBy, entity.CreatedByName, entity.CreatedOn);

                    #endregion

                    response.CreateResponse(DataResponseStatus.OK, model.Id);
                }
                else
                {
                    response.CreateResponse(DataResponseStatus.InternalServerError);
                }
            }
            catch (Exception ex)
            {
                response.ThrowError(ex);
            }
            finally
            {
                base.DBClose();
            }
            return response;
        }

        public DataResponse Update(EntityAccount entity)
        {
            var response = new DataResponse<EntityAccount>();
            try
            {
                base.DBInit();

                var model = DBEntity.Accounts.FirstOrDefault(a => a.Id == entity.Id);

                if (entity.RepId == null)
                {
                    var objRep = DBEntity.Reps.Where(a => a.UserId == entity.CurrentUserId).FirstOrDefault();
                    entity.RepId = objRep.Id;
                }

                #region Account

                model.EnrolledDate = entity.EnrolledDate;
                model.UpdatedBy = entity.UpdatedBy;
                model.UpdatedOn = entity.UpdatedOn;

                #endregion

                #region Lead

                model.Lead.RepId = entity.RepId;
                model.Lead.LeadSourceId = entity.LeadSourceId;
                model.Lead.LeadServiceIntrest = entity.LeadServiceIntrest;
                model.Lead.OtherLeadSource = entity.OtherLeadSource;
                model.UpdatedBy = entity.UpdatedBy;
                model.UpdatedOn = entity.UpdatedOn;

                #endregion

                #region PracticeServiceMappers

                int servicescount = model.Lead.Practice.PracticeServiceMappers.Count;
                for (int i = 0; i < servicescount; i++)
                {
                    DBEntity.Entry(model.Lead.Practice.PracticeServiceMappers.ElementAtOrDefault(0)).State = EntityState.Deleted;
                }
                List<Database.PracticeServiceMapper> oPracticeServiceMapper = new List<Database.PracticeServiceMapper>();

                string[] serviceIds = ((string[])entity.ServiceIds);

                if (serviceIds != null && serviceIds.Count() > 0)
                {
                    foreach (var serviceId in serviceIds)
                    {
                        oPracticeServiceMapper.Add(new Database.PracticeServiceMapper
                        {
                            EnrolledServiceId = Convert.ToInt32(serviceId),
                            CreatedBy = entity.CreatedBy,
                            CreatedOn = entity.CreatedOn,
                            UpdatedBy = entity.UpdatedBy,
                            UpdatedOn = entity.UpdatedOn
                        });
                    }
                }

                #endregion

                #region Practice

                model.Lead.Practice.RepId = entity.RepId;
                model.Lead.Practice.PracticeName = entity.Practice.Name;
                //model.Lead.Practice.FaxNumber = entity.Practice.Fax;
                model.Lead.Practice.PracticeTypeId = entity.PracticeTypeId;
                model.Lead.Practice.PracticeSpecialityType = entity.Practice.SpecialityType;
                model.Lead.Practice.ContactPreferenceId = entity.Practice.ContactPreferenceId;
                model.Lead.Practice.PracticeServiceMappers = oPracticeServiceMapper;
                model.Lead.Practice.UpdatedBy = entity.UpdatedBy;
                model.Lead.Practice.UpdatedOn = entity.UpdatedOn;
                model.Lead.Practice.ReportDeliveryPreference = entity.Practice.ReportDeliveryEmail;
                model.Lead.Practice.ReportDeliveryFax = entity.Practice.ReportDeliveryFax;

                #endregion

                #region PracticeContact

                var existingPracticeContactIds = model.Lead.Practice.PracticeContacts.Select(a => a.Id).ToList();

                foreach (var item in entity.Practice.Contact)
                {
                    var modelItem = model.Lead.Practice.PracticeContacts.FirstOrDefault(a => a.PracticeId == model.Lead.Practice.Id && a.AddressId == item.AddressId);

                    if (modelItem != null)
                    {
                        DBEntity.Entry(modelItem).State = EntityState.Modified;
                        existingPracticeContactIds.Remove(modelItem.Id);

                        modelItem.BillingContact = item.BillingContact;
                        modelItem.BillingContactEmail = item.BillingContactEmail;
                        modelItem.BillingContactPhone = item.BillingContactPhone;
                        modelItem.ManagerName = item.ManagerName;
                        modelItem.ManagerPhone = item.ManagerPhone;
                        modelItem.ManagerEmail = item.ManagerEmail;
                        modelItem.UpdateBy = entity.UpdatedBy;
                        modelItem.UpdatedOn = entity.UpdatedOn;
                    }
                    else
                    {
                        var PracticeContact = new Database.PracticeContact
                        {
                            AddressId = item.AddressId,
                            PracticeId = model.Lead.Practice.Id,
                            BillingContact = item.BillingContact,
                            BillingContactEmail = item.BillingContactEmail,
                            BillingContactPhone = item.BillingContactPhone,
                            ManagerName = item.ManagerName,
                            ManagerPhone = item.ManagerPhone,
                            ManagerEmail = item.ManagerEmail,
                            CreatedOn = entity.CreatedOn,
                            CreatedBy = entity.CreatedBy
                        };
                        DBEntity.PracticeContacts.Add(PracticeContact);
                    }
                }

                foreach (var item in existingPracticeContactIds)
                {
                    var modelItem = DBEntity.PracticeContacts.FirstOrDefault(a => a.Id == item);
                    DBEntity.Entry(modelItem).State = EntityState.Deleted;
                }

                #endregion

                #region Practice Specialities

                int mpscount = model.Lead.Practice.PracticePracticeSpecialityMappers.Count;
                for (int i = 0; i < mpscount; i++)
                {
                    DBEntity.Entry(model.Lead.Practice.PracticePracticeSpecialityMappers.ElementAtOrDefault(0)).State = EntityState.Deleted;
                }
                foreach (var item in entity.Practice.Specialities)
                {
                    if (string.IsNullOrEmpty(entity.Practice.SpecialityType))
                    {
                        model.Lead.Practice.PracticePracticeSpecialityMappers.Add(
                                    new Database.PracticePracticeSpecialityMapper
                                    {
                                        PracticeId = model.Lead.Practice.Id,
                                        PracticeSpecialityId = item.PracticeSpecialityId,
                                        CreatedBy = entity.CreatedBy,
                                        CreatedOn = entity.CreatedOn
                                    });
                    }
                    else
                    {
                        model.Lead.Practice.PracticeSpecialityType = entity.Practice.SpecialityType;
                    }
                }
                #endregion

                #region Practice Address

                var existingAddressIds = model.Lead.Practice.PracticeAddressMappers.Select(a => a.AddressId).ToList();

                foreach (var item in entity.Practice.Address)
                {
                    var modelItem = model.Lead.Practice.PracticeAddressMappers.FirstOrDefault(a => a.AddressId == item.AddressId);
                    if (existingAddressIds.Contains(item.AddressId))
                    {
                        DBEntity.Entry(modelItem).State = EntityState.Modified;
                        DBEntity.Entry(modelItem.Address).State = EntityState.Modified;
                        existingAddressIds.Remove(item.AddressId);
                    }

                    if (item.AddressId != 0)
                    {
                        modelItem.Address.AddressIndex = item.AddressIndex;
                        modelItem.Address.Line1 = item.Line1;
                        modelItem.Address.Line2 = item.Line2;
                        modelItem.Address.City = item.City;
                        modelItem.Address.Zip = item.Zip;
                        modelItem.Address.Fax = item.Fax;
                        modelItem.Address.AddressTypeId = item.AddressTypeId;
                        modelItem.Address.StateId = item.StateId;
                        modelItem.Address.UpdatedBy = entity.UpdatedBy;
                        modelItem.Address.UpdatedOn = entity.UpdatedOn;

                        var phone = item.Phone.FirstOrDefault();
                        var modelPhones = modelItem.Address.Phones.FirstOrDefault();
                        modelPhones.PhoneNumber = phone.PhoneNumber;
                        modelPhones.Extension = phone.Extension;
                        modelPhones.PhoneTypeId = 1;
                        modelItem.ManagerName = item.ManagerName;
                        modelPhones.UpdatedBy = entity.UpdatedBy;
                        modelPhones.UpdatedOn = entity.UpdatedOn;
                    }
                    else
                    {
                        var address = new Database.Address
                        {
                            AddressIndex = item.AddressIndex,
                            Line1 = item.Line1,
                            Line2 = item.Line2,
                            City = item.City,
                            Zip = item.Zip,
                            Fax = item.Fax,
                            AddressTypeId = item.AddressTypeId,
                            StateId = item.StateId,
                            CreatedBy = entity.CreatedBy,
                            CreatedOn = entity.CreatedOn,
                            Phones = item.Phone == null ? null : item.Phone.Select(b => new Database.Phone
                            {
                                PhoneNumber = b.PhoneNumber,
                                Extension = b.Extension,
                                PhoneTypeId = 1,
                                CreatedBy = entity.CreatedBy,
                                CreatedOn = entity.CreatedOn,
                            }).ToList()
                        };
                        model.Lead.Practice.PracticeAddressMappers.Add(new Database.PracticeAddressMapper
                        {
                            Address = address,
                            ManagerName = item.ManagerName,
                            CreatedBy = entity.CreatedBy,
                            CreatedOn = entity.CreatedOn
                        });
                    }
                }

                foreach (var item in existingAddressIds)
                {
                    var modelItem = model.Lead.Practice.PracticeAddressMappers.FirstOrDefault(a => a.AddressId == item);
                    DBEntity.Entry(modelItem).State = EntityState.Deleted;
                }

                #endregion

                #region Providers

                var existingProviderIds = model.Lead.Practice.PracticeProviderMappers.Select(a => a.Provider.Id).ToList();

                foreach (var item in entity.Practice.Providers)
                {
                    var modelItem = model.Lead.Practice.PracticeProviderMappers.FirstOrDefault(a => a.Provider.Id == item.Id && a.Provider.NPI == item.NPI);
                    //var isExists = model.Lead.Practice.PracticeProviderMappers.Any(a => a.Provider.Id == item.Id && a.Provider.NPI == item.NPI);
                    if (modelItem != null && existingProviderIds.Contains(item.Id))
                    {
                        DBEntity.Entry(modelItem).State = EntityState.Modified;
                        existingProviderIds.Remove(item.Id);

                        modelItem.Provider.AddressIndex = item.AddressIndex;
                        //modelItem.Provider.FirstName = item.FirstName;
                        //modelItem.Provider.LastName = item.MiddleName;
                        //modelItem.Provider.LastName = item.LastName;n
                        //modelItem.Provider.NPI = item.NPI;
                        modelItem.Provider.DegreeId = item.DegreeId;
                        //modelItem.Provider.UpdatedBy = entity.UpdatedBy;
                        //modelItem.Provider.UpdatedOn = entity.UpdatedOn;
                    }
                    else
                    {
                        var provider = DBEntity.Providers.FirstOrDefault(a => a.NPI == item.NPI);

                        if (provider == null)
                        {
                            provider = new Database.Provider
                            {
                                AddressIndex = item.AddressIndex,
                                FirstName = item.FirstName,
                                MiddleName = item.MiddleName,
                                LastName = item.LastName,
                                NPI = item.NPI,
                                DegreeId = item.DegreeId,
                                IsActive = true,
                                CreatedBy = entity.CreatedBy,
                                CreatedOn = entity.CreatedOn,
                            };
                        }
                        else
                        {
                            provider.AddressIndex = item.AddressIndex;
                        }

                        model.Lead.Practice.PracticeProviderMappers.Add(
                         new Database.PracticeProviderMapper
                         {
                             Provider = provider,
                             CreatedBy = entity.CreatedBy,
                             CreatedOn = entity.CreatedOn,
                         });

                    }
                }

                foreach (var item in existingProviderIds)
                {
                    var modelItem = model.Lead.Practice.PracticeProviderMappers.FirstOrDefault(a => a.Provider.Id == item);
                    DBEntity.Entry(modelItem).State = EntityState.Deleted;
                }

                #endregion

                if (base.DBSaveUpdate(model) > 0)
                {
                    #region Practice Provider Address Mapper

                    var oPracticeProviderLocationList = new List<Database.PracticeProviderLocationMapper>();

                    model.Lead.Practice.PracticeProviderMappers.ToList().ForEach(s =>
                        oPracticeProviderLocationList.Add(new Database.PracticeProviderLocationMapper
                        {
                            PracticeId = model.Lead.Practice.Id,
                            ProviderId = s.ProviderId,
                            AddressId = s.Practice.PracticeAddressMappers
                                            .FirstOrDefault(f => f.Address.AddressIndex == s.Practice.PracticeProviderMappers
                                                .FirstOrDefault(d => d.Provider.Id == s.ProviderId).Provider.AddressIndex).AddressId,
                            CreatedBy = entity.CreatedBy,
                            CreatedOn = entity.CreatedOn,
                        }));

                    var existingPracticeProviderAddressList = model.Lead.Practice.PracticeProviderLocationMappers.Select(a => a.Id).ToList();


                    foreach (var item in oPracticeProviderLocationList)
                    {
                        var modelItem = DBEntity.PracticeProviderLocationMappers.FirstOrDefault(a => a.ProviderId == item.ProviderId & a.PracticeId == item.PracticeId);
                        if (modelItem != null)
                        {
                            DBEntity.Entry(modelItem).State = EntityState.Modified;
                            existingPracticeProviderAddressList.Remove(modelItem.Id);

                            modelItem.AddressId = item.AddressId;
                            modelItem.UpdatedBy = entity.UpdatedBy;
                            modelItem.UpdatedOn = entity.UpdatedOn;
                        }
                        else
                        {
                            var PracticeProviderLocationMapper = new Database.PracticeProviderLocationMapper
                            {
                                PracticeId = item.PracticeId,
                                ProviderId = item.ProviderId,
                                AddressId = item.AddressId,
                                CreatedBy = item.CreatedBy,
                                CreatedOn = item.CreatedOn,
                            };
                            DBEntity.PracticeProviderLocationMappers.Add(PracticeProviderLocationMapper);
                        }
                    }

                    foreach (var item in existingPracticeProviderAddressList)
                    {
                        var modelItem = DBEntity.PracticeProviderLocationMappers.FirstOrDefault(a => a.Id == item);
                        DBEntity.Entry(modelItem).State = EntityState.Deleted;
                    }

                    #endregion

                    #region Practice Contacts

                    foreach (var item in model.Lead.Practice.PracticeAddressMappers)
                    {
                        var modelItem = item.Address.PracticeContacts.FirstOrDefault(s => s.PracticeId == null);
                        if (modelItem != null)
                        {
                            modelItem.PracticeId = model.Lead.Practice.Id;
                        }
                    }

                    #endregion

                    DBEntity.SaveChanges();


                    var RepDetails = DBEntity.Reps.FirstOrDefault(a => a.Id == entity.RepId);
                    if (RepDetails == null)
                        goto exitNotify;

                    if (RepDetails.RepGroup.RepgroupManagerMappers.Count > 0)
                    {
                        entity.Rep = new EntityRep
                        {
                            Managers = RepDetails.RepGroup.RepgroupManagerMappers.Select(b => new Manager
                            {
                                Id = b.User.Id,
                                FirstName = b.User.FirstName,
                                LastName = b.User.LastName,
                                Email = b.User.Email
                            }).ToList(),

                            FirstName = RepDetails.User2.FirstName,
                            LastName = RepDetails.User2.LastName,
                            Username = RepDetails.User2.UserName,
                        };
                    }
                    else
                    {
                        entity.Rep = new EntityRep
                        {
                            FirstName = RepDetails.User2.FirstName,
                            LastName = RepDetails.User2.LastName,
                            Username = RepDetails.User2.UserName,
                        };
                    }

                    //entity.Rep = new EntityRep
                    //{
                    //    //Manager = new Manager
                    //    //{
                    //    //    Id = model.Lead.Rep.RepGroup.User2.Id,
                    //    //    FirstName = model.Lead.Rep.RepGroup.User2.FirstName,
                    //    //    LastName = model.Lead.Rep.RepGroup.User2.LastName,
                    //    //    UserName = model.Lead.Rep.RepGroup.User2.UserName,
                    //    //},

                    //    FirstName = model.Lead.Rep.User2.FirstName,
                    //    LastName = model.Lead.Rep.User2.LastName,
                    //    Username = model.Lead.Rep.User2.UserName,
                    //};

                    #region Save Notification Data

                    List<EntityNotification> notificationlist = new List<EntityNotification>();
                    int MangerId = 0;

                    if (entity.Rep.Managers.Count() > 0)
                    {

                        var mangerDetails = entity.Rep.Managers.FirstOrDefault();
                        MangerId = mangerDetails.Id;
                    }
                    if (MangerId == entity.CreatedBy)
                    {

                        notificationlist.Add(new EntityNotification
                        {
                            UserId = model.Lead.Rep.User2.Id,
                            Message = NotificationMessages.AccountManagerUpdateNotification
                        });
                    }
                    else if (entity.RepId == entity.CreatedBy)
                    {
                        notificationlist.Add(new EntityNotification
                        {
                            UserId = MangerId,
                            Message = NotificationMessages.AccountRepUpdateNotification
                        });
                    }
                    else
                    {
                        notificationlist.Add(new EntityNotification
                        {
                            UserId = model.Lead.Rep.User2.Id,
                            Message = NotificationMessages.AccountManagerUpdateNotification
                        });

                        notificationlist.Add(new EntityNotification
                        {
                            UserId = MangerId,
                            Message = NotificationMessages.AccountRepUpdateNotification
                        });
                    }

                    new RepositoryNotification().Save(notificationlist, entity.CurrentUserId, model.Id, (int)NotificationTargetType.Account, (int)NotificationType.Normal, entity.CreatedBy, entity.CreatedByName, entity.CreatedOn);

                    #endregion
                    exitNotify:
                    return GetAccountById(model.Id);
                }
                else
                {
                    response.CreateResponse(DataResponseStatus.InternalServerError);
                }
                base.DBClose();
            }
            catch (Exception ex)
            {
                response.ThrowError(ex);
            }
            return response;
        }

        public DataResponse ConvertToAccount(EntityAccount entity)
        {
            var response = new DataResponse<EntityAccount>();
            try
            {
                base.DBInit();

                var leadModel = DBEntity.Leads.FirstOrDefault(a => a.Id == entity.LeadId);

                #region Lead

                leadModel.Id = entity.LeadId;
                leadModel.IsConverted = true;
                leadModel.RepId = entity.RepId;
                leadModel.LeadSourceId = entity.LeadSourceId;
                leadModel.LeadServiceIntrest = entity.LeadServiceIntrest;
                leadModel.OtherLeadSource = entity.OtherLeadSource;
                leadModel.UpdatedBy = entity.UpdatedBy;
                leadModel.UpdatedOn = entity.UpdatedOn;
                leadModel.LeadStatus = (int)LeadStatus.Transacted;

                #endregion

                #region PracticeServiceMappers

                int servicescount = leadModel.Practice.PracticeServiceMappers.Count;
                for (int i = 0; i < servicescount; i++)
                {
                    DBEntity.Entry(leadModel.Practice.PracticeServiceMappers.ElementAtOrDefault(0)).State = EntityState.Deleted;
                }
                List<Database.PracticeServiceMapper> oPracticeServiceMapper = new List<Database.PracticeServiceMapper>();

                string[] serviceIds = ((string[])entity.ServiceIds);

                if (serviceIds != null && serviceIds.Count() > 0)
                {
                    foreach (var serviceId in serviceIds)
                    {
                        oPracticeServiceMapper.Add(new Database.PracticeServiceMapper
                        {
                            EnrolledServiceId = Convert.ToInt32(serviceId),
                            CreatedBy = entity.CreatedBy,
                            CreatedOn = entity.CreatedOn,
                            UpdatedBy = entity.UpdatedBy,
                            UpdatedOn = entity.UpdatedOn
                        });
                    }
                }

                #endregion

                #region Practice

                leadModel.Practice.RepId = entity.RepId;
                leadModel.Practice.PracticeName = entity.Practice.Name;
                //leadModel.Practice.FaxNumber = entity.Practice.Fax;
                leadModel.Practice.PracticeTypeId = entity.PracticeTypeId;
                leadModel.Practice.PracticeSpecialityType = entity.Practice.SpecialityType;
                leadModel.Practice.ContactPreferenceId = entity.Practice.ContactPreferenceId;
                leadModel.Practice.PracticeServiceMappers = oPracticeServiceMapper;
                leadModel.Practice.UpdatedBy = entity.UpdatedBy;
                leadModel.Practice.UpdatedOn = entity.UpdatedOn;
                leadModel.Practice.ReportDeliveryPreference = entity.Practice.ReportDeliveryEmail;
                leadModel.Practice.ReportDeliveryFax = entity.Practice.ReportDeliveryFax;

                #endregion

                #region Practice Specialities

                int mpscount = leadModel.Practice.PracticePracticeSpecialityMappers.Count;
                for (int i = 0; i < mpscount; i++)
                {
                    DBEntity.Entry(leadModel.Practice.PracticePracticeSpecialityMappers.ElementAtOrDefault(0)).State = EntityState.Deleted;
                }

                foreach (var item in entity.Practice.Specialities)
                {
                    if (string.IsNullOrEmpty(entity.Practice.SpecialityType))
                    {
                        leadModel.Practice.PracticePracticeSpecialityMappers.Add(
                                    new Database.PracticePracticeSpecialityMapper
                                    {
                                        PracticeId = leadModel.Practice.Id,
                                        PracticeSpecialityId = item.PracticeSpecialityId,
                                        CreatedBy = entity.CreatedBy,
                                        CreatedOn = entity.CreatedOn
                                    });
                    }
                    else
                    {
                        leadModel.Practice.PracticeSpecialityType = entity.Practice.SpecialityType;
                    }
                }

                #endregion

                #region Practice Address

                var existingAddressIds = leadModel.Practice.PracticeAddressMappers.Select(a => a.AddressId).ToList();

                foreach (var item in entity.Practice.Address)
                {
                    #region Building PracticeContact

                    List<Database.PracticeContact> practiceContacts = new List<Database.PracticeContact>{
                                        new Database.PracticeContact
                                        {
                                            BillingContact=item.BillingContact,
                                            BillingContactEmail=item.BillingContactEmail,
                                            BillingContactPhone=item.BillingContactPhone,
                                            ManagerName=item.ManagerName,
                                            ManagerEmail=item.ManagerEmail,
                                            ManagerPhone=item.ManagerPhone,
                                            officedayshrs=item.officedayshrs,
                                            CreatedOn = entity.CreatedOn,
                                            CreatedBy = entity.CreatedBy
                                        }
                                    };

                    #endregion

                    var modelItem = leadModel.Practice.PracticeAddressMappers.FirstOrDefault(a => a.AddressId == item.AddressId);
                    if (existingAddressIds.Contains(item.AddressId))
                    {
                        DBEntity.Entry(modelItem).State = EntityState.Modified;
                        DBEntity.Entry(modelItem.Address).State = EntityState.Modified;
                        existingAddressIds.Remove(item.AddressId);
                    }

                    if (item.AddressId != 0)
                    {
                        modelItem.Address.AddressIndex = item.AddressIndex;
                        modelItem.PracticeId = leadModel.Practice.Id;
                        modelItem.Address.Line1 = item.Line1;
                        modelItem.Address.Line2 = item.Line2;
                        modelItem.Address.City = item.City;
                        modelItem.Address.Zip = item.Zip;
                        modelItem.Address.Fax = item.Fax;
                        modelItem.Address.AddressTypeId = item.AddressTypeId;
                        modelItem.Address.StateId = item.StateId;
                        modelItem.Address.UpdatedBy = entity.UpdatedBy;
                        modelItem.Address.UpdatedOn = entity.UpdatedOn;

                        modelItem.Address.PracticeContacts = practiceContacts;

                        var phone = item.Phone.FirstOrDefault();
                        var modelPhones = modelItem.Address.Phones.FirstOrDefault();
                        modelPhones.PhoneNumber = phone.PhoneNumber;
                        modelPhones.Extension = phone.Extension;
                        modelPhones.PhoneTypeId = 1;
                        modelItem.ManagerName = item.ManagerName;
                        modelPhones.UpdatedBy = entity.UpdatedBy;
                        modelPhones.UpdatedOn = entity.UpdatedOn;
                    }
                    else
                    {
                        var address = new Database.Address
                        {
                            AddressIndex = item.AddressIndex,
                            Line1 = item.Line1,
                            Line2 = item.Line2,
                            City = item.City,
                            Zip = item.Zip,
                            AddressTypeId = item.AddressTypeId,
                            StateId = item.StateId,
                            CreatedBy = entity.CreatedBy,
                            CreatedOn = entity.CreatedOn,
                            Phones = item.Phone == null ? null : item.Phone.Select(b => new Database.Phone
                            {
                                PhoneNumber = b.PhoneNumber,
                                Extension = b.Extension,
                                PhoneTypeId = 1,
                                CreatedBy = entity.CreatedBy,
                                CreatedOn = entity.CreatedOn,
                            }).ToList(),
                            PracticeContacts = practiceContacts
                        };

                        leadModel.Practice.PracticeAddressMappers.Add(new Database.PracticeAddressMapper
                        {
                            PracticeId = leadModel.Practice.Id,
                            Address = address,
                            ManagerName = item.ManagerName,
                            CreatedBy = entity.CreatedBy,
                            CreatedOn = entity.CreatedOn
                        });
                    }
                }

                foreach (var item in existingAddressIds)
                {
                    var modelItem = leadModel.Practice.PracticeAddressMappers.FirstOrDefault(a => a.AddressId == item);
                    DBEntity.Entry(modelItem).State = EntityState.Deleted;
                }

                #endregion

                #region Providers

                var existingProviderIds = leadModel.Practice.PracticeProviderMappers.Select(a => a.Provider.Id).ToList();

                foreach (var item in entity.Practice.Providers)
                {
                    var modelItem = leadModel.Practice.PracticeProviderMappers.FirstOrDefault(a => a.Provider.Id == item.Id);
                    if (existingProviderIds.Contains(item.Id))
                    {
                        DBEntity.Entry(modelItem).State = EntityState.Modified;
                        existingProviderIds.Remove(item.Id);
                    }

                    if (item.Id != 0)
                    {
                        modelItem.Provider.AddressIndex = item.AddressIndex;
                        modelItem.Provider.FirstName = item.FirstName;
                        modelItem.Provider.LastName = item.MiddleName;
                        modelItem.Provider.LastName = item.LastName;
                        modelItem.Provider.NPI = item.NPI;
                        modelItem.Provider.DegreeId = item.DegreeId;
                        modelItem.Provider.UpdatedBy = entity.UpdatedBy;
                        modelItem.Provider.UpdatedOn = entity.UpdatedOn;
                    }
                    else
                    {
                        leadModel.Practice.PracticeProviderMappers.Add(
                         new Database.PracticeProviderMapper
                         {
                             Provider = new Database.Provider
                             {
                                 AddressIndex = item.AddressIndex,
                                 FirstName = item.FirstName,
                                 MiddleName = item.MiddleName,
                                 LastName = item.LastName,
                                 IsActive = true,
                                 NPI = item.NPI,
                                 DegreeId = item.DegreeId,
                                 CreatedBy = entity.CreatedBy,
                                 CreatedOn = entity.CreatedOn
                             },
                             CreatedBy = entity.CreatedBy,
                             CreatedOn = entity.CreatedOn
                         });
                    }
                }

                foreach (var item in existingProviderIds)
                {
                    var modelItem = leadModel.Practice.PracticeProviderMappers.FirstOrDefault(a => a.Provider.Id == item);
                    DBEntity.Entry(modelItem).State = EntityState.Deleted;
                }

                #endregion

                var accountModel = new Database.Account
                {
                    LeadId = leadModel.Id,
                    EnrolledDate = entity.EnrolledDate,
                    IsActive = true,
                    CreatedBy = entity.CreatedBy,
                    CreatedOn = entity.CreatedOn,
                    BusinessId = entity.BusinessId,

                    Lead = leadModel
                };

                DBEntity.Entry(accountModel.Lead).State = EntityState.Modified;

                if (base.DBSave(accountModel) > 0)
                {
                    #region Practice Provider Address Mapper

                    var oPracticeProviderLocationList = new List<Database.PracticeProviderLocationMapper>();

                    leadModel.Practice.PracticeProviderMappers.ToList().ForEach(s =>
                        oPracticeProviderLocationList.Add(new Database.PracticeProviderLocationMapper
                        {
                            PracticeId = leadModel.Practice.Id,
                            ProviderId = s.ProviderId,
                            AddressId = s.Practice.PracticeAddressMappers
                                            .FirstOrDefault(f => f.Address.AddressIndex == s.Practice.PracticeProviderMappers
                                                .FirstOrDefault(d => d.Provider.Id == s.ProviderId).Provider.AddressIndex).AddressId,
                            CreatedBy = entity.CreatedBy,
                            CreatedOn = entity.CreatedOn,
                        }));

                    var existingPracticeProviderAddressList = leadModel.Practice.PracticeProviderLocationMappers.Select(a => a.Id).ToList();


                    foreach (var item in oPracticeProviderLocationList)
                    {
                        var modelItem = DBEntity.PracticeProviderLocationMappers.FirstOrDefault(a => a.ProviderId == item.ProviderId & a.PracticeId == item.PracticeId);
                        if (modelItem != null)
                        {
                            DBEntity.Entry(modelItem).State = EntityState.Modified;
                            existingPracticeProviderAddressList.Remove(modelItem.Id);

                            modelItem.AddressId = item.AddressId;
                            modelItem.UpdatedBy = entity.UpdatedBy;
                            modelItem.UpdatedOn = entity.UpdatedOn;
                        }
                        else
                        {
                            var PracticeProviderLocationMapper = new Database.PracticeProviderLocationMapper
                            {
                                PracticeId = item.PracticeId,
                                ProviderId = item.ProviderId,
                                AddressId = item.AddressId,
                                CreatedBy = item.CreatedBy,
                                CreatedOn = item.CreatedOn,
                            };
                            DBEntity.PracticeProviderLocationMappers.Add(PracticeProviderLocationMapper);
                        }
                    }

                    foreach (var item in existingPracticeProviderAddressList)
                    {
                        var modelItem = DBEntity.PracticeProviderLocationMappers.FirstOrDefault(a => a.Id == item);
                        DBEntity.Entry(modelItem).State = EntityState.Deleted;
                    }

                    #endregion

                    #region Practice Contacts

                    foreach (var item in leadModel.Practice.PracticeAddressMappers)
                    {
                        var modelItem = item.Address.PracticeContacts.FirstOrDefault(s => s.PracticeId == null);
                        if (modelItem != null)
                        {
                            modelItem.PracticeId = leadModel.Practice.Id;
                        }
                    }

                    #endregion

                    DBEntity.SaveChanges();

                    response.CreateResponse(DataResponseStatus.OK, accountModel.Id);
                }
                else
                {
                    response.CreateResponse(DataResponseStatus.InternalServerError);
                }
            }
            catch (Exception ex)
            {
                ex.Log();
                response.ThrowError(ex);
            }
            finally
            {
                base.DBClose();
            }
            return response;
        }

        public DataResponse SetFlag(int AccountId)
        {
            var response = new DataResponse<bool?>();
            try
            {
                base.DBInit();

                var model = DBEntity.Accounts.FirstOrDefault(a => a.Id == AccountId);
                response.Model = model.Lead.HasFlag;

                if (model != null)
                {
                    model.Lead.HasFlag = ((model.Lead.HasFlag == null) || (model.Lead.HasFlag == false)) ? true : false;
                }
                if (base.DBSaveUpdate(model) > 0)
                {
                    response.Model = model.Lead.HasFlag;
                }
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }
            return response;
        }

        public DataResponse SetActiveStatus(int AccountId)
        {
            var response = new DataResponse<bool?>();

            try
            {
                base.DBInit();

                var model = DBEntity.Accounts.FirstOrDefault(a => a.Id == AccountId);
                response.Model = model.IsActive;

                if (model != null)
                {
                    model.IsActive = ((model.IsActive == null) || (model.IsActive == false)) ? true : false;
                }
                if (base.DBSaveUpdate(model) > 0)
                {
                    response.Model = model.IsActive;
                }
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }

            return response;
        }

        public DataResponse<EntityGroupManagerDetails> RepDetailsByAccountId(int? AccountId)
        {
            var response = new DataResponse<EntityGroupManagerDetails>();
            try
            {
                base.DBInit();
                var query = DBEntity.Accounts.Where(a => a.Id == AccountId && a.Lead.IsConverted == true).Select(a => new EntityGroupManagerDetails
                {
                    Id = a.Id,
                    LeadId = a.LeadId,
                    RepId = a.Lead.RepId,
                    RepEmail = a.Lead.Rep.User2.Email,
                    RepFirstName = a.Lead.Rep.User2.FirstName,
                    RepMiddleName = a.Lead.Rep.User2.MiddleName,
                    RepLastName = a.Lead.Rep.User2.LastName,
                    Managers = a.Lead.Rep.RepGroup.RepgroupManagerMappers.Select(b => new Manager
                    {
                        FirstName = b.User.FirstName,
                        MiddleName = b.User.MiddleName,
                        LastName = b.User.LastName,
                        Email = b.User.Email
                    }),
                });
                response = GetFirst<EntityGroupManagerDetails>(query);
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }
            return response;
        }

        public void UpdateLoggedInTime(int userId)
        {
            try
            {
                base.DBInit();
                var userProfile = DBEntity.UserProfiles.FirstOrDefault(a => a.UserId == userId);
                if (userProfile != null)
                {
                    userProfile.LastLoggedInTime = DateTime.UtcNow;
                    base.DBSaveUpdate(userProfile);
                }
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }
        }
    }
}
