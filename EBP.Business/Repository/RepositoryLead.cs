using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EBP.Business.Entity;
using EBP.Business.Entity.Lead;
using EBP.Business.Entity.Practice;
using EBP.Business.Filter;
using System.Web.UI.WebControls;
using System.Linq.Expressions;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using EBP.Business.Enums;
using EBP.Business.Resource;
using EBP.Business.Entity.Rep;
using EBP.Business.Entity.EntityNotificationSettings;
using System.Linq.Dynamic;

namespace EBP.Business.Repository
{
    public class RepositoryLead : _Repository
    {

        public DataResponse<EntityList<EntityLead>> GetLeads(FilterLead filter, int? businessId, int currentUserId, bool isBuzAdmin, bool isSalesManager, bool isSalesDirector, int take = 10, int skip = 0)
        {
            var response = new DataResponse<EntityList<EntityLead>>();
            try
            {
                if (filter != null)
                {
                    take = filter.Take;
                    skip = filter.Skip;
                }

                base.DBInit();

                var query = DBEntity.Leads.Where(a => a.IsConverted == false && a.BusinessId == businessId);

                if (!isBuzAdmin)
                {
                    // Note : implemented property isSalesManager
                    //var reps = DBEntity.Reps.Any(a => a.RepGroup.RepgroupManagerMappers.Any(b => b.ManagerId == currentUserId)); //---ManagerChange     var reps = DBEntity.Reps.Any(a => a.RepGroup.ManagerId == currentUserId);
                    if (isSalesDirector || isSalesManager)
                    {
                        //Manager
                        query = query.Where(a => a.Rep.UserId == currentUserId || a.Rep.RepGroup.RepgroupManagerMappers.Any(b => b.ManagerId == currentUserId));// || a.Rep.RepGroup.SalesDirectorId == currentUserId
                        //---ManagerChange     query = query.Where(a => a.Rep.UserId == currentUserId || a.Rep.RepGroup.ManagerId == currentUserId);
                    }
                    else
                    {
                        //Rep
                        query = query.Where(a => a.Rep.UserId == currentUserId);
                    }
                }
                if (filter != null)
                {
                    if (!String.IsNullOrEmpty(filter.KeyWords))
                        query = query.Where(a => a.Practice.PracticeName.ToLower().Contains(filter.KeyWords.ToLower())
                            || a.Practice.PracticeProviderMappers.Any(p => ((p.Provider.FirstName + " " + p.Provider.MiddleName + " " + p.Provider.LastName).ToLower()).Contains(filter.KeyWords.ToLower()))
                            || a.Practice.PracticeProviderMappers.Any(p => ((p.Provider.FirstName + " " + p.Provider.MiddleName + " " + p.Provider.LastName + ", " + p.Provider.LookupDegree.ShortCode).ToLower()).Contains(filter.KeyWords.ToLower()))
                            || a.Practice.PracticeProviderMappers.Any(p => ((p.Provider.FirstName + " " + p.Provider.LastName).ToLower()).Contains(filter.KeyWords.ToLower()))
                             || a.Practice.PracticeProviderMappers.Any(p => ((p.Provider.FirstName + " " + p.Provider.LastName + ", " + p.Provider.LookupDegree.ShortCode).ToLower()).Contains(filter.KeyWords.ToLower()))
                            || a.Practice.PracticeProviderMappers.Any(p => ((p.Provider.FirstName + p.Provider.MiddleName + p.Provider.LastName + p.Provider.NPI).ToLower()).Contains(filter.KeyWords.ToLower())));

                    if (filter.IsActive != null)
                        query = query.Where(a => a.IsActive == filter.IsActive);
                    else
                        query = query.Where(a => a.IsActive == true);

                    if (filter.HasFlag.HasValue)
                        query = query.Where(a => a.HasFlag == filter.HasFlag);

                    if (filter.HasTask.HasValue)
                        query = query.Where(a => a.Practice.Tasks.Any(b => b.Status != 3));

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
                                query = query.Where(a => a.CreatedOn > dt);
                                break;

                            case Periods.Yesterday:
                                dt = DateTime.UtcNow.AddHours(-12);
                                endDt = dt.AddHours(-12);
                                query = query.Where(a => a.CreatedOn < dt && a.CreatedOn > endDt);
                                break;


                            case Periods.ThisWeek:

                                dt = DateTime.UtcNow.AddDays(-7);
                                query = query.Where(a => a.CreatedOn > dt);
                                break;


                            case Periods.LastWeek:

                                dt = DateTime.UtcNow.AddDays(-7);
                                endDt = dt.AddDays(-7);

                                query = query.Where(a => a.CreatedOn < dt && a.CreatedOn > endDt);

                                break;


                            case Periods.ThisMonth:

                                dt = DateTime.UtcNow.AddDays(-30);

                                query = query.Where(a => a.CreatedOn > dt);
                                break;

                            case Periods.LastMonth:

                                dt = DateTime.UtcNow.AddDays(-30);
                                endDt = dt.AddDays(-30);


                                query = query.Where(a => a.CreatedOn < dt && a.CreatedOn > endDt);
                                break;

                            case Periods.ThisYear:

                                dt = DateTime.UtcNow.AddDays(-365);

                                query = query.Where(a => a.CreatedOn > dt);
                                break;

                            case Periods.LastYear:

                                dt = DateTime.UtcNow.AddDays(-365);
                                endDt = dt.AddDays(-365);


                                query = query.Where(a => a.CreatedOn < dt && a.CreatedOn > endDt);
                                break;

                            default:
                                break;
                        }

                        #endregion
                    }

                    if (filter.RepGroupIds != null && filter.RepGroupIds.Length > 0)
                        query = query.Where(a => filter.RepGroupIds.Contains((int)a.Practice.Rep.RepGroupId));

                    if (filter.RepIds != null && filter.RepIds.Length > 0)
                        query = query.Where(a => filter.RepIds.Contains((int)a.RepId));
                }

                var selectQuery = query.Select(a => new EntityLead
                {
                    LeadId = a.Id,
                    LeadSourceName = a.LookupLeadSource.LeadSource,
                    OtherLeadSource = a.OtherLeadSource,
                    Practice = new EntityPractice
                    {
                        Id = a.Practice.Id,
                        Name = a.Practice.PracticeName,
                    },
                    Rep = new EntityRep
                    {
                        Id = a.RepId,
                        GroupId = a.Rep.RepGroupId,
                        GroupName = a.Rep.RepGroup.RepGroupName,
                        FirstName = a.Rep.User2.FirstName,
                        LastName = a.Rep.User2.LastName,
                        Username = a.Rep.User2.UserName
                    },
                    TaskCount = a.Practice.Tasks.Count(),
                    Status = a.LeadStatus == 1 ? "New" : "Transacted",
                    HasFlag = a.HasFlag,
                    HasTask = filter.HasTask.HasValue ? true : a.Practice.Tasks.Count(task => task.Status != 3) > 0,
                    IsActive = a.IsActive,
                    CreatedBy = a.CreatedBy,
                    CreatedOn = a.CreatedOn,
                    UpdatedBy = a.UpdatedBy,
                    UpdatedOn = a.UpdatedOn
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
                                selectQuery = selectQuery.OrderBy(o => o.Practice.Name);
                            else
                                selectQuery = selectQuery.OrderByDescending(o => o.Practice.Name);
                            break;
                        case "RepGroupName":
                            if (filter.OrderBy == "asc")
                                selectQuery = selectQuery.OrderBy(o => o.Rep.GroupName);
                            else
                                selectQuery = selectQuery.OrderByDescending(o => o.Rep.GroupName);
                            break;
                        case "RepName":
                            if (filter.OrderBy == "asc")
                                selectQuery = selectQuery.OrderBy(o => o.Rep.FirstName + " " + o.Rep.LastName);
                            else
                                selectQuery = selectQuery.OrderByDescending(o => o.Rep.FirstName + " " + o.Rep.LastName);
                            break;
                        default:
                            string orderBy = string.Format("{0} {1}", filter.SortKey, filter.OrderBy);
                            selectQuery = selectQuery.OrderBy(orderBy);
                            break;
                    }
                }

                response = GetList<EntityLead>(selectQuery, skip, take);
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

        public DataResponse<EntityLead> GetLeadById(int leadId)
        {
            var response = new DataResponse<EntityLead>();
            try
            {
                base.DBInit();
                var query = DBEntity.Leads.Where(a => a.Id == leadId && a.IsConverted == false).Select(a => new EntityLead
                {
                    LeadId = a.Id,
                    LeadSourceId = a.LeadSourceId,
                    LeadSourceName = a.LookupLeadSource.LeadSource,
                    LeadServiceIntrest = a.LeadServiceIntrest,
                    OtherLeadSource = a.OtherLeadSource,
                    IsActive = a.IsActive,
                    Status = a.LeadStatus == 1 ? "New" : "Transacted ",
                    Rep = new EntityRep
                    {
                        Id = a.RepId,
                        GroupId = a.Rep.RepGroupId,
                        GroupName = a.Rep.RepGroup.RepGroupName,
                        FirstName = a.Rep.User2.FirstName,
                        LastName = a.Rep.User2.LastName,
                        Username = a.Rep.User2.UserName,
                        Managers = a.Rep.RepGroup.RepgroupManagerMappers.Select(b => new Manager
                        {
                            FirstName = b.User.FirstName,
                            LastName = b.User.LastName,
                            UserName = a.User.UserName,
                            Email = b.User.Email
                        }),
                    },
                    HasFlag = a.HasFlag,
                    HasTask = a.Practice.Tasks.Count > 0,
                    PracticeId = a.Practice.Id,

                    Practice = new EntityPractice
                    {
                        Id = a.Practice.Id,
                        Name = a.Practice.PracticeName,
                        SpecialityTypeId = a.Practice.PracticeTypeId,
                        SpecialityType = a.Practice.PracticeSpecialityType,
                        Email = a.Practice.Email,
                        //Fax = a.Practice.FaxNumber,
                        Specialities = a.Practice.PracticePracticeSpecialityMappers.Select(s => new EntityPracticeSpeciality
                        {
                            PracticeSpecialityId = s.LookupPracticeSpecialityType.Id,
                            NewSpecialityName = s.LookupPracticeSpecialityType.PracticeSpecialityType
                        }),
                        Providers = a.Practice.PracticeProviderMappers.Select(s => new EntityProvider
                        {
                            ProviderId = s.Provider.Id,
                            FirstName = s.Provider.FirstName,
                            MiddleName = s.Provider.MiddleName,
                            LastName = s.Provider.LastName,
                            DegreeId = s.Provider.DegreeId,
                            DegreeName = s.Provider.LookupDegree.DegreeName,
                            ShortCode = s.Provider.LookupDegree.ShortCode,
                            NPI = s.Provider.NPI,
                            Address = s.Provider.PracticeProviderLocationMappers.Where(b => b.PracticeId == a.PracticeId).Select(m => new EntityProviderAddress
                            {
                                AddressId = m.Address.Id,
                                Line1 = m.Address.Line1,
                                Line2 = m.Address.Line2,
                                City = m.Address.City,
                                Zip = m.Address.Zip,
                                Fax = m.Address.Fax, //?? a.Practice.FaxNumber,
                                AddressTypeId = m.Address.AddressTypeId,
                                StateId = m.Address.StateId,
                                State = m.Address.LookupState.StateName,
                                Phone = m.Address.Phones.Select(p => new EntityPracticePhone
                                {
                                    PhoneId = p.Id,
                                    PhoneNumber = p.PhoneNumber,
                                    Extension = p.Extension
                                })
                            }).FirstOrDefault(),
                        }),
                        Address = a.Practice.PracticeAddressMappers.Select(s => new EntityPracticeAddress
                        {
                            AddressId = s.Address.Id,
                            Line1 = s.Address.Line1,
                            Line2 = s.Address.Line2,
                            City = s.Address.City,
                            Zip = s.Address.Zip,
                            Fax = s.Address.Fax,
                            AddressTypeId = s.Address.AddressTypeId,
                            ManagerName = s.ManagerName,
                            StateId = s.Address.StateId,
                            State = s.Address.LookupState.StateName,
                            Phone = s.Address.Phones.Select(p => new EntityPracticePhone
                            {
                                PhoneId = p.Id,
                                PhoneNumber = p.PhoneNumber,
                                Extension = p.Extension
                            })
                        })
                    },
                    CreatedOn = a.CreatedOn,
                    CreatedBy = a.CreatedBy,
                    CreatedByName = a.User.FirstName + " " + a.User.LastName,

                    UpdatedOn = a.UpdatedOn,
                    UpdatedBy = a.UpdatedBy,
                    UpdatedByName = a.User1.FirstName + " " + a.User1.LastName
                });
                response = GetFirst<EntityLead>(query);
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

        public DataResponse<EntityLead> Insert(EntityLead entity, bool isNameOnly = false)
        {
            var response = new DataResponse<EntityLead>();
            //var managers = DBEntity.RepgroupManagerMappers.Where(t => t.RepGroupId == entity.RepGroupId).Select(a => (int?)a.ManagerId).FirstOrDefault();
            // entity.ManagerId = managers.Value;
            try
            {
                base.DBInit();

                if (entity.RepId == null)
                {
                    var objRep = DBEntity.Reps.Where(a => a.UserId == entity.CurrentUserId).FirstOrDefault();
                    if (objRep != null)
                        entity.RepId = objRep.Id;
                }

                if (isNameOnly)
                {
                    #region Name Only Case
                    var practiceModel = new Database.Lead
                    {
                        IsActive = entity.IsActive,
                        RepId = entity.RepId,
                        LeadStatus = (int)LeadStatus.New,
                        IsConverted = entity.IsConverted,
                        BusinessId = entity.BusinessId,
                        Practice = new Database.Practice
                        {
                            RepId = entity.RepId,
                            PracticeName = entity.Practice.Name,
                            BusinessId = entity.BusinessId,
                            CreatedOn = entity.CreatedOn,
                            CreatedBy = entity.CreatedBy
                        },
                        CreatedOn = entity.CreatedOn,
                        CreatedBy = entity.CreatedBy
                    };

                    if (base.DBSave(practiceModel) > 0)
                    {
                        entity.LeadId = practiceModel.Id;

                        var RepDetails = DBEntity.Reps.FirstOrDefault(a => a.Id == practiceModel.RepId);
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
                                Message = NotificationMessages.LeadManagerNotification
                            });
                        }
                        else if (entity.RepId == entity.CreatedBy)
                        {

                            notificationlist.Add(new EntityNotification
                            {
                                UserId = MangerId,
                                Message = NotificationMessages.LeadRepNotification
                            });
                        }
                        else
                        {

                            notificationlist.Add(new EntityNotification
                            {
                                UserId = RepDetails.User2.Id,
                                Message = NotificationMessages.LeadManagerNotification
                            });

                            notificationlist.Add(new EntityNotification
                            {
                                UserId = MangerId,
                                Message = NotificationMessages.LeadRepNotification
                            });
                        }

                        new RepositoryNotification().Save(notificationlist, entity.CurrentUserId, entity.LeadId, (int)NotificationTargetType.Lead, (int)NotificationType.Normal, entity.CreatedBy, entity.CreatedByName, entity.CreatedOn);

                        #endregion

                        exitNotify:
                        response.CreateResponse<EntityLead>(entity, DataResponseStatus.OK);
                    }
                    else
                    {
                        response.CreateResponse(DataResponseStatus.InternalServerError);
                    }
                    return response;
                    #endregion
                }

                #region Genrarate PracticeProviderMapper

                var entityProviders = entity.Practice.Providers;
                List<Database.PracticeProviderMapper> PracticeProviderMappers = new List<Database.PracticeProviderMapper>();
                foreach (var oProvider in entityProviders)
                {
                    var provider = DBEntity.Providers.FirstOrDefault(a => a.NPI == oProvider.NPI);
                    int providerId = provider != null ? provider.Id : 0;
                    if (providerId > 0)
                    {
                        PracticeProviderMappers.Add(new Database.PracticeProviderMapper
                        {
                            AddressIndex = oProvider.AddressIndex,
                            ProviderId = providerId,
                            CreatedBy = entity.CreatedBy,
                            CreatedOn = entity.CreatedOn
                        });
                    }
                    else
                    {
                        PracticeProviderMappers.Add(new Database.PracticeProviderMapper
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

                #region Leads

                #region Backup Name Only Case
                //var model = new Database.Lead
                //{
                //    LeadSourceId = entity.LeadSourceId,
                //    LeadServiceIntrest = entity.LeadServiceIntrest,
                //    OtherLeadSource = entity.OtherLeadSource,
                //    IsActive = entity.IsActive,
                //    RepId = entity.RepId,
                //    LeadStatus = (int)LeadStatus.New,
                //    IsConverted = entity.IsConverted,
                //    BusinessId = entity.BusinessId,
                //    Practice = new Database.Practice
                //    {
                //        ContactPreferenceId = entity.ContactPreferenceId,
                //        RepId = entity.RepId,
                //        PracticeName = entity.Practice.Name,
                //        PracticeTypeId = entity.PracticeTypeId == 0 ? null : entity.PracticeTypeId,
                //        PracticeSpecialityType = entity.Practice.SpecialityType,
                //        Email = entity.Practice.Email,
                //        FaxNumber = entity.Practice.Fax,
                //        BusinessId = entity.BusinessId,
                //        PracticePracticeSpecialityMappers = (entity.Practice.Specialities == null || !string.IsNullOrEmpty(entity.Practice.SpecialityType)) ? null : entity.Practice.Specialities.Select(a => new Database.PracticePracticeSpecialityMapper
                //        {
                //            PracticeSpecialityId = a.PracticeSpecialityId,
                //            CreatedBy = entity.CreatedBy,
                //            CreatedOn = entity.CreatedOn
                //        }).ToList(),
                //        PracticeAddressMappers = entity.Practice.Address == null ? null : entity.Practice.Address.Select(a => new Database.PracticeAddressMapper
                //        {
                //            Address = new Database.Address
                //            {
                //                AddressIndex = a.AddressIndex,
                //                Line1 = a.Line1,
                //                Line2 = a.Line2,
                //                City = a.City,
                //                Zip = a.Zip,
                //                AddressTypeId = a.AddressTypeId,
                //                StateId = a.StateId,
                //                CreatedBy = entity.CreatedBy,
                //                CreatedOn = entity.CreatedOn,
                //                Phones = a.Phone == null ? null : a.Phone.Select(b => new Database.Phone
                //                {
                //                    PhoneNumber = b.PhoneNumber,
                //                    Extension = b.Extension,
                //                    PhoneTypeId = 1,
                //                    CreatedOn = entity.CreatedOn,
                //                    CreatedBy = entity.CreatedBy
                //                }).ToList(),
                //            },
                //            ManagerName = a.ManagerName,
                //            CreatedBy = entity.CreatedBy,
                //            CreatedOn = entity.CreatedOn,
                //        }).ToList(),
                //        PracticeProviderMappers = PracticeProviderMappers,

                //        CreatedOn = entity.CreatedOn,
                //        CreatedBy = entity.CreatedBy
                //    },
                //    CreatedOn = entity.CreatedOn,
                //    CreatedBy = entity.CreatedBy
                //};
                #endregion

                var model = new Database.Lead
                {
                    LeadSourceId = entity.LeadSourceId,
                    LeadServiceIntrest = entity.LeadServiceIntrest,
                    OtherLeadSource = entity.OtherLeadSource,
                    IsActive = entity.IsActive,
                    RepId = entity.RepId,
                    LeadStatus = (int)LeadStatus.New,
                    IsConverted = entity.IsConverted,
                    BusinessId = entity.BusinessId,
                    Practice = new Database.Practice
                    {
                        ContactPreferenceId = entity.ContactPreferenceId,
                        RepId = entity.RepId,
                        PracticeName = entity.Practice.Name,
                        PracticeTypeId = entity.PracticeTypeId == 0 ? null : entity.PracticeTypeId,
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
                            },
                            ManagerName = a.ManagerName,
                            CreatedBy = entity.CreatedBy,
                            CreatedOn = entity.CreatedOn,
                        }).ToList(),
                        PracticeProviderMappers = PracticeProviderMappers,

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

                    model.Practice.PracticeProviderMappers.ToList().ForEach(s =>
                        DBEntity.PracticeProviderLocationMappers.Add(new Database.PracticeProviderLocationMapper
                        {
                            PracticeId = model.Practice.Id,
                            ProviderId = s.ProviderId,
                            AddressId = s.Practice.PracticeAddressMappers
                                            .FirstOrDefault(f => f.Address.AddressIndex == s.Practice.PracticeProviderMappers
                                                .FirstOrDefault(d => d.Provider.Id == s.ProviderId).AddressIndex).AddressId,
                            CreatedBy = entity.CreatedBy,
                            CreatedOn = entity.CreatedOn
                        }));

                    #endregion

                    DBEntity.SaveChanges();

                    entity.LeadId = model.Id;

                    var RepDetails = DBEntity.Reps.FirstOrDefault(a => a.Id == model.RepId);

                    if (RepDetails.RepGroup.RepgroupManagerMappers.Count > 0)
                    {
                        entity.Rep = new EntityRep
                        {
                            Managers = RepDetails.RepGroup.RepgroupManagerMappers.Select(b => new Manager
                            {
                                Id = b.User.Id,
                                FirstName = b.User.FirstName,
                                MiddleName = b.User.MiddleName,
                                LastName = b.User.LastName,
                                Email = b.User.Email
                            }).ToList(),

                            FirstName = RepDetails.User2.FirstName,
                            MiddleName = RepDetails.User2.MiddleName,
                            LastName = RepDetails.User2.LastName,
                            Username = RepDetails.User2.UserName,
                            RepEmail = RepDetails.User2.Email
                        };
                    }
                    else
                    {
                        entity.Rep = new EntityRep
                        {
                            FirstName = RepDetails.User2.FirstName,
                            MiddleName = RepDetails.User2.MiddleName,
                            LastName = RepDetails.User2.LastName,
                            Username = RepDetails.User2.UserName,
                            RepEmail = RepDetails.User2.Email
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
                            Message = NotificationMessages.LeadManagerNotification
                        });
                    }
                    else if (entity.RepId == entity.CreatedBy)
                    {

                        notificationlist.Add(new EntityNotification
                        {
                            UserId = MangerId,
                            Message = NotificationMessages.LeadRepNotification
                        });
                    }
                    else
                    {
                        notificationlist.Add(new EntityNotification
                        {
                            UserId = RepDetails.User2.Id,
                            Message = NotificationMessages.LeadManagerNotification
                        });

                        notificationlist.Add(new EntityNotification
                        {
                            UserId = MangerId,
                            Message = NotificationMessages.LeadRepNotification
                        });
                    }

                    new RepositoryNotification().Save(notificationlist, entity.CurrentUserId, entity.LeadId, (int)NotificationTargetType.Lead, (int)NotificationType.Normal, entity.CreatedBy, entity.CreatedByName, entity.CreatedOn);

                    #endregion

                    response.CreateResponse<EntityLead>(entity, DataResponseStatus.OK);
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

        public DataResponse<EntityLead> Update(EntityLead entity, bool isNameOnly = false)
        {
            var response = new DataResponse<EntityLead>();
            try
            {
                base.DBInit();

                var model = DBEntity.Leads.FirstOrDefault(a => a.Id == entity.LeadId);

                if (isNameOnly)
                {
                    #region Name Only Case
                    #region Lead

                    model.Id = entity.LeadId;
                    model.RepId = entity.RepId;
                    model.UpdatedBy = entity.UpdatedBy;
                    model.UpdatedOn = entity.UpdatedOn;

                    #endregion

                    #region Practice

                    model.Practice.RepId = entity.RepId;
                    model.Practice.PracticeName = entity.Practice.Name;
                    model.Practice.UpdatedBy = entity.UpdatedBy;
                    model.Practice.UpdatedOn = entity.UpdatedOn;

                    #endregion

                    if (base.DBSaveUpdate(model) > 0)
                    {
                        if (entity.Rep == null)
                            goto exitNotify;

                        var RepDetails = DBEntity.Reps.FirstOrDefault(a => a.Id == model.RepId);
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

                        //    FirstName = model.Rep.User2.FirstName,
                        //    LastName = model.Rep.User2.LastName,
                        //    Username = model.Rep.User2.UserName,
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
                                UserId = model.Rep.User2.Id,
                                Message = NotificationMessages.LeadManagerUpdateNotification
                            });
                        }
                        else if (entity.RepId == entity.CreatedBy)
                        {

                            notificationlist.Add(new EntityNotification
                            {
                                UserId = MangerId,
                                Message = NotificationMessages.LeadRepUpdateNotification
                            });
                        }
                        else
                        {
                            notificationlist.Add(new EntityNotification
                            {
                                UserId = model.Rep.User2.Id,
                                Message = NotificationMessages.LeadManagerUpdateNotification
                            });

                            notificationlist.Add(new EntityNotification
                            {
                                UserId = MangerId,
                                Message = NotificationMessages.LeadRepUpdateNotification
                            });
                        }

                        new RepositoryNotification().Save(notificationlist, entity.UpdatedBy.Value, entity.LeadId, (int)NotificationTargetType.Lead, (int)NotificationType.Normal, entity.CreatedBy, entity.CreatedByName, entity.CreatedOn);

                        #endregion

                        exitNotify:
                        return GetLeadById(model.Id);
                    }
                    #endregion
                }

                if (entity.RepId == null)
                {
                    var objRep = DBEntity.Reps.Where(a => a.UserId == entity.CurrentUserId).FirstOrDefault();
                    if (objRep != null)
                        entity.RepId = objRep.Id;
                }

                #region Lead

                model.Id = entity.LeadId;
                model.RepId = entity.RepId;
                model.LeadSourceId = entity.LeadSourceId;
                model.LeadServiceIntrest = entity.LeadServiceIntrest;
                model.OtherLeadSource = entity.OtherLeadSource;
                model.IsActive = entity.IsActive;
                model.UpdatedBy = entity.UpdatedBy;
                model.UpdatedOn = entity.UpdatedOn;

                #endregion

                #region Practice

                model.Practice.RepId = entity.RepId;
                //model.Practice.FaxNumber = entity.Practice.Fax;
                model.Practice.PracticeName = entity.Practice.Name;
                model.Practice.PracticeTypeId = entity.PracticeTypeId;
                model.Practice.PracticeSpecialityType = entity.Practice.SpecialityType;
                model.Practice.UpdatedBy = entity.UpdatedBy;
                model.Practice.UpdatedOn = entity.UpdatedOn;

                #endregion

                #region Practice Specialities

                int mpscount = model.Practice.PracticePracticeSpecialityMappers.Count;
                for (int i = 0; i < mpscount; i++)
                {
                    DBEntity.Entry(model.Practice.PracticePracticeSpecialityMappers.ElementAtOrDefault(0)).State = EntityState.Deleted;
                }

                foreach (var item in entity.Practice.Specialities)
                {
                    if (string.IsNullOrEmpty(entity.Practice.SpecialityType))
                    {
                        model.Practice.PracticePracticeSpecialityMappers.Add(
                                    new Database.PracticePracticeSpecialityMapper
                                    {
                                        PracticeId = model.Practice.Id,
                                        PracticeSpecialityId = item.PracticeSpecialityId,
                                        CreatedBy = entity.CreatedBy,
                                        CreatedOn = entity.CreatedOn
                                    });
                    }
                    else
                    {
                        model.Practice.PracticeSpecialityType = entity.Practice.SpecialityType;
                    }
                }

                #endregion

                #region Practice Address

                var existingAddressIds = model.Practice.PracticeAddressMappers.Select(a => a.AddressId).ToList();

                foreach (var item in entity.Practice.Address)
                {
                    var modelItem = model.Practice.PracticeAddressMappers.FirstOrDefault(a => a.AddressId == item.Id);
                    if (existingAddressIds.Contains(item.Id))
                    {
                        DBEntity.Entry(modelItem).State = EntityState.Modified;
                        DBEntity.Entry(modelItem.Address).State = EntityState.Modified;
                        existingAddressIds.Remove(item.Id);
                    }

                    if (modelItem != null && item.Id != 0)
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
                        model.Practice.PracticeAddressMappers.Add(new Database.PracticeAddressMapper
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
                    var modelItem = model.Practice.PracticeAddressMappers.FirstOrDefault(a => a.AddressId == item);
                    DBEntity.Entry(modelItem).State = EntityState.Deleted;
                }

                #endregion

                #region Providers

                var existingProviderIds = model.Practice.PracticeProviderMappers.Select(a => a.Provider.Id).ToList();

                foreach (var item in entity.Practice.Providers)
                {
                    var modelItem = model.Practice.PracticeProviderMappers.FirstOrDefault(a => a.Provider.Id == item.Id && a.Provider.NPI == item.NPI);
                    //var isExists = model.Practice.PracticeProviderMappers.Any(a => a.Provider.Id == item.Id && a.Provider.NPI == item.NPI);

                    if (modelItem != null && existingProviderIds.Contains(item.Id))
                    {
                        DBEntity.Entry(modelItem).State = EntityState.Modified;
                        existingProviderIds.Remove(item.Id);

                        modelItem.Provider.DegreeId = item.DegreeId;
                        modelItem.Provider.AddressIndex = item.AddressIndex;
                    }
                    //if (item.Id != 0)
                    //{
                    //    modelItem.Provider.AddressIndex = item.AddressIndex;
                    //    modelItem.Provider.FirstName = item.FirstName;
                    //    modelItem.Provider.LastName = item.MiddleName;
                    //    modelItem.Provider.LastName = item.LastName;
                    //    modelItem.Provider.NPI = item.NPI;
                    //    modelItem.Provider.DegreeId = item.DegreeId;
                    //    modelItem.Provider.UpdatedBy = entity.UpdatedBy;
                    //    modelItem.Provider.UpdatedOn = entity.UpdatedOn;
                    //}
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

                        model.Practice.PracticeProviderMappers.Add(
                         new Database.PracticeProviderMapper
                         {
                             Provider = provider,
                             CreatedBy = entity.CreatedBy,
                             CreatedOn = entity.CreatedOn,
                         });

                        //int providerId = provider != null ? provider.Id : 0;
                        //if (providerId > 0)
                        //{
                        //    model.Practice.PracticeProviderMappers.Add(new Database.PracticeProviderMapper
                        //    {
                        //        AddressIndex = item.AddressIndex,
                        //        ProviderId = providerId,
                        //        CreatedBy = entity.CreatedBy,
                        //        CreatedOn = entity.CreatedOn
                        //    });
                        //}
                        //else
                        //{
                        //    model.Practice.PracticeProviderMappers.Add(
                        //     new Database.PracticeProviderMapper
                        //     {
                        //         Provider = new Database.Provider
                        //         {
                        //             AddressIndex = item.AddressIndex,
                        //             FirstName = item.FirstName,
                        //             MiddleName = item.MiddleName,
                        //             LastName = item.LastName,
                        //             NPI = item.NPI,
                        //             DegreeId = item.DegreeId,
                        //             IsActive = true,
                        //             CreatedBy = entity.CreatedBy,
                        //             CreatedOn = entity.CreatedOn
                        //         },
                        //         CreatedBy = entity.CreatedBy,
                        //         CreatedOn = entity.CreatedOn
                        //     });
                        //}
                    }
                }

                foreach (var item in existingProviderIds)
                {
                    var modelItem = model.Practice.PracticeProviderMappers.FirstOrDefault(a => a.Provider.Id == item);
                    DBEntity.Entry(modelItem).State = EntityState.Deleted;
                }

                #endregion

                if (base.DBSaveUpdate(model) > 0)
                {
                    #region Practice Provider Address Mapper

                    var oPracticeProviderLocationList = new List<Database.PracticeProviderLocationMapper>();

                    model.Practice.PracticeProviderMappers.ToList().ForEach(s =>
                        oPracticeProviderLocationList.Add(new Database.PracticeProviderLocationMapper
                        {
                            PracticeId = model.Practice.Id,
                            ProviderId = s.ProviderId,
                            AddressId = s.Practice.PracticeAddressMappers
                                            .FirstOrDefault(f => f.Address.AddressIndex == s.Practice.PracticeProviderMappers
                                                .FirstOrDefault(d => d.Provider.Id == s.ProviderId).Provider.AddressIndex).AddressId,
                            CreatedBy = entity.CreatedBy,
                            CreatedOn = entity.CreatedOn
                        }));

                    var existingPracticeProviderAddressList = model.Practice.PracticeProviderLocationMappers.Select(a => a.Id).ToList();


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
                                CreatedOn = item.CreatedOn
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

                    DBEntity.SaveChanges();

                    entity.Rep = new EntityRep
                    {
                        FirstName = model.Rep.User2.FirstName,
                        LastName = model.Rep.User2.LastName,
                        Username = model.Rep.User2.UserName,
                    };

                    #region Save Notification Data
                    //new code
                    var RepDetails = DBEntity.Reps.FirstOrDefault(a => a.Id == model.RepId);
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
                    //end
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
                            UserId = model.Rep.User2.Id,
                            Message = NotificationMessages.LeadManagerUpdateNotification
                        });
                    }
                    else if (entity.RepId == entity.CreatedBy)
                    {

                        notificationlist.Add(new EntityNotification
                        {
                            UserId = MangerId,
                            Message = NotificationMessages.LeadRepUpdateNotification
                        });
                    }
                    else
                    {
                        notificationlist.Add(new EntityNotification
                        {
                            UserId = model.Rep.User2.Id,
                            Message = NotificationMessages.LeadManagerUpdateNotification
                        });

                        notificationlist.Add(new EntityNotification
                        {
                            UserId = MangerId,
                            Message = NotificationMessages.LeadRepUpdateNotification
                        });
                    }

                    new RepositoryNotification().Save(notificationlist, entity.CurrentUserId, entity.LeadId, (int)NotificationTargetType.Lead, (int)NotificationType.Normal, entity.CreatedBy, entity.CreatedByName, entity.CreatedOn);

                    #endregion
                    exitNotify:
                    return GetLeadById(model.Id);
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

        public DataResponse SetFlag(int leadId)
        {
            var response = new DataResponse<bool?>();
            try
            {
                base.DBInit();

                var model = DBEntity.Leads.FirstOrDefault(a => a.Id == leadId);
                response.Model = model.HasFlag;

                if (model != null)
                {
                    model.HasFlag = ((model.HasFlag == null) || (model.HasFlag == false)) ? true : false;
                }
                if (base.DBSaveUpdate(model) == 1)
                {
                    response.Model = model.HasFlag;
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

        public DataResponse SetActiveStatus(int leadId)
        {
            var response = new DataResponse<bool?>();
            try
            {
                base.DBInit();

                var model = DBEntity.Leads.FirstOrDefault(a => a.Id == leadId);
                response.Model = model.IsActive;

                if (model != null)
                {
                    model.IsActive = model.IsActive == false ? true : false;
                }
                if (base.DBSaveUpdate(model) == 1)
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

    }
}