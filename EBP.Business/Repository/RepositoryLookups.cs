using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EBP.Business.Entity;
using EBP.Business.Enums;
using EBP.Business.Helpers;
using EBP.Business.Resource;

namespace EBP.Business.Repository
{
    public class RepositoryLookups : _Repository
    {
        public DataResponse<EntityList<EntitySelectItem>> GetPractiesSpecialities()
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            try
            {
                var cachedResponse = response.GetCached<EntityList<EntitySelectItem>>(CacheKeys.PracticeSpecialities);

                if (cachedResponse.IsOk())
                    return cachedResponse;

                base.DBInit();
                var query = DBEntity.LookupPracticeSpecialityTypes.Where(s => s.IsActive == true).Select(s => new EntitySelectItem
                {
                    Id = s.Id,
                    Value = s.PracticeSpecialityType
                }).OrderBy(o => o.Value);
                response = GetList<EntitySelectItem>(query, 0, 100);

                response.CacheIt<EntityList<EntitySelectItem>>(CacheKeys.PracticeSpecialities);
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

        public DataResponse<EntityList<EntitySelectItem>> GetPracticeTypes()
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            try
            {

                var cachedData = response.GetCached<EntityList<EntitySelectItem>>(CacheKeys.PracticeTypes);
                if (cachedData.IsOk())
                    return cachedData;

                base.DBInit();
                var query = DBEntity.LookupPracticeTypes.Where(s => s.IsActive == true).Select(s => new EntitySelectItem
                {
                    Id = s.Id,
                    Value = s.PracticeType
                }).OrderBy(o => o.Value);
                response = GetList<EntitySelectItem>(query, 0, 100);

                if (response.IsOk())
                    response.CacheIt<EntityList<EntitySelectItem>>(CacheKeys.PracticeTypes);
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

        public DataResponse<EntityList<EntitySelectItem>> GetAllStates()
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();

            var cachedResponse = response.GetCached<EntityList<EntitySelectItem>>(CacheKeys.States);

            if (cachedResponse.IsOk())
                return cachedResponse;

            try
            {
                base.DBInit();
                var query = DBEntity.LookupStates.Where(s => s.IsActive == true).Select(s => new EntitySelectItem
                {
                    Id = s.Id,
                    Value = s.StateName
                }).OrderBy(o => o.Value);
                response = GetList<EntitySelectItem>(query, 0, 100);

                if (response.IsOk())
                    response.CacheIt<EntityList<EntitySelectItem>>(CacheKeys.States);

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

        public DataResponse<EntityList<EntitySelectItem>> GetAllCountriess()
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();

            try
            {
                base.DBInit();
                var query = DBEntity.LookupCountries.Where(s => s.IsActive == true).Select(s => new EntitySelectItem
                {
                    Id = s.Id,
                    Value = s.CountryName
                }).OrderBy(o => o.Value);
                response = GetList<EntitySelectItem>(query, 0, 100);
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

        public DataResponse<EntityList<EntitySelectItem>> GetAllPrivilegeModules()
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();

            try
            {
                base.DBInit();
                var query = DBEntity.ModulesMasters.Where(s => s.IsActive == true).Select(s => new EntitySelectItem
                {
                    Id = s.Id,
                    Value = s.Title
                }).OrderBy(o => o.Value);
                response = GetList<EntitySelectItem>(query, 0, 100);
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

        public DataResponse<EntityList<EntitySelectItem>> GetAllDegrees()
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();

            var cachedResponse = response.GetCached<EntityList<EntitySelectItem>>(CacheKeys.Degrees);

            if (cachedResponse.IsOk())
                return cachedResponse;

            try
            {
                base.DBInit();
                var query = DBEntity.LookupDegrees.Where(s => s.IsActive == true).Select(s => new EntitySelectItem
                {
                    Id = s.Id,
                    Value = s.DegreeName
                }).OrderBy(o => o.Value);
                response = GetList<EntitySelectItem>(query, 0, 100);

                if (response.IsOk())
                    response.CacheIt<EntityList<EntitySelectItem>>(CacheKeys.Degrees);
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

        public DataResponse<EntityList<EntitySelectItem>> GetAllRepGroups(int? businessId, int currentUserId, bool isBuzAdmin, bool isSalesManager, bool isSalesDirector)
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            try
            {
                base.DBInit();

                var query = DBEntity.RepGroups.Where(a => a.BusinessId == businessId);

                if (isSalesDirector || isSalesManager)
                    query = query.Where(a => a.RepgroupManagerMappers.Any(b => b.ManagerId == currentUserId));// || a.SalesDirectorId == currentUserId//---ManagerChange   query = query.Where(a => a.ManagerId == currentUserId);

                var selectQuery = query.Select(s => new EntitySelectItem
                {
                    Id = s.Id,
                    Value = s.RepGroupName
                }).OrderBy(o => o.Value);

                response = GetList<EntitySelectItem>(selectQuery, 0, 100);
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

        public DataResponse<EntityList<EntitySelectItem>> GetAllReps(int? businessId, int currentUserId, bool isBuzAdmin, bool isSalesManager, bool isSalesDirector)
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            try
            {
                base.DBInit();
                var query = DBEntity.Reps.Where(a => a.User2.BusinessId == businessId);

                if (isSalesDirector || isSalesManager)
                    query = query.Where(a => a.RepGroup.RepgroupManagerMappers.Any(b => b.ManagerId == currentUserId));// || a.RepGroup.SalesDirectorId == currentUserId //---ManagerChange      query = query.Where(a => a.RepGroup.ManagerId == currentUserId);

                var selectQuery = query.Select(s => new EntitySelectItem
                {
                    Id = s.Id,
                    Value = s.User2.FirstName.Trim() + " " + s.User2.LastName.Trim(), //+ " @ " + s.User2.UserName,
                    ParentId = s.RepGroupId
                }).OrderBy(o => o.Value);

                response = GetList<EntitySelectItem>(selectQuery, 0, 1000);

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

        public DataResponse<EntityList<EntitySelectItem>> GetAllRepsByGroupId(int?[] repGroupIds, int? businessId, int currentUserId, bool isSalesManager, bool isSalesDirector, bool isSameGroup = false)
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            try
            {
                base.DBInit();
                var query = DBEntity.Reps.Where(s => s.User2.BusinessId == businessId);

                if (isSameGroup)
                {
                    // Note : implemented property isSalesManager
                    //var reps = DBEntity.Reps.Any(a => a.RepGroup.RepgroupManagerMappers.Any(b => b.ManagerId == currentUserId));//---ManagerChange     var reps = DBEntity.Reps.Any(a => a.RepGroup.ManagerId == currentUserId);
                    if (isSalesDirector || isSalesManager)
                    {
                        //Manager
                        query = query.Where(a => a.RepGroup.RepgroupManagerMappers.Any(b => b.ManagerId == currentUserId));// || a.RepGroup.SalesDirectorId == currentUserId
                        //---ManagerChange      query = query.Where(a => a.RepGroup.ManagerId == currentUserId);
                    }
                    else
                    {
                        //Rep
                        var repGroup = DBEntity.Reps.Where(a => a.UserId == currentUserId).FirstOrDefault();
                        if (repGroup != null)
                        {
                            int repGroupId = repGroup.RepGroupId.Value;
                            query = query.Where(a => a.RepGroupId == repGroupId);
                        }
                        else
                            return response;
                    }
                }

                if (repGroupIds != null && repGroupIds.Length > 0)
                    query = query.Where(a => repGroupIds.Contains(a.RepGroupId));

                var selectQuery = query.Select(s => new EntitySelectItem
                {
                    Id = s.Id,
                    Value = s.User2.FirstName.Trim() + " " + s.User2.LastName.Trim() //+ " @ " + s.User2.UserName,
                }).OrderBy(o => o.Value);

                response = GetList<EntitySelectItem>(selectQuery, 0, 1000);
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

        public DataResponse<EntityList<EntitySelectItem>> GetAllTaskUsers(int? businessId, int currentUserId, bool isRep, bool isSalesManager, bool isBuzAdmin, bool isSalesDirector)
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            try
            {
                base.DBInit();

                var query = DBEntity.Users.Where(a => a.IsActive == true && a.BusinessId == businessId);

                if (!isBuzAdmin && isSalesManager)
                {
                    query = query.Where(a => a.Reps2.Any(b => b.RepGroup.RepgroupManagerMappers.Any(c => c.ManagerId == currentUserId)) ||
                        DBEntity.RepgroupManagerMappers.Any(b => b.ManagerId == a.Id && b.UserRole == (int)RepgroupUserType.Director) ||
                        a.Roles.Any(b => b.Id == 15));
                }
                else if (isRep)
                {
                    var objRep = DBEntity.Reps.FirstOrDefault(a => a.UserId == currentUserId);
                    int? repGroupId = objRep.RepGroupId;
                    if (repGroupId != null)
                    {
                        var managerIds = objRep.RepGroup.RepgroupManagerMappers.Select(a => a.ManagerId);

                        query = query.Where(a => managerIds.Contains(a.Id) || a.Roles.Any(b => b.Id == 15));
                    }
                }

                if (!isBuzAdmin && !isSalesManager && isSalesDirector)
                {
                    query = query.Where(a => a.Reps2.Any(b => b.RepGroup.RepgroupManagerMappers.Any(c => c.ManagerId == currentUserId && c.UserRole == (int)RepgroupUserType.Director)));
                }

                var selectQuery = query.Select(s => new EntitySelectItem
                {
                    Id = s.Id,
                    Value = s.FirstName.Trim() + " " + s.LastName.Trim(),
                }).OrderBy(o => o.Value);

                response = GetList<EntitySelectItem>(selectQuery, 0, 1000);
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

        public DataResponse<EntityList<EntitySelectItem>> GetAllLeadSources()
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            try
            {

                var cachedResponse = response.GetCached<EntityList<EntitySelectItem>>(CacheKeys.LeadSources);

                if (cachedResponse.IsOk())
                    return cachedResponse;

                base.DBInit();
                var query = DBEntity.LookupLeadSources.Where(s => s.IsActive == true).Select(s => new EntitySelectItem
                {
                    Id = s.Id,
                    Value = s.LeadSource
                }).OrderBy(o => o.Value);
                response = GetList<EntitySelectItem>(query, 0, 100);

                if (response.IsOk())
                    response.CacheIt<EntityList<EntitySelectItem>>(CacheKeys.LeadSources);

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

        public DataResponse<EntityList<EntitySelectItem>> GetAllProviders(int? currentBusinessId, int currentUserId, bool IsSalesManager, bool isSalesDirector, bool isRep, string searchKey = "")
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            try
            {
                base.DBInit();

                var query = DBEntity.Providers.Where(s => s.IsActive == true && s.PracticeProviderMappers.Any(a => a.Practice.BusinessId == currentBusinessId && a.ProviderId == s.Id));

                if (isSalesDirector || IsSalesManager)
                    query = query.Where(a => a.PracticeProviderMappers.Any(b => b.Practice.Rep.RepGroup.RepgroupManagerMappers.Any(c => c.ManagerId == currentUserId)) ||
                        a.PracticeProviderMappers.Any(b => b.Practice.Rep.RepGroup.RepgroupManagerMappers.Any(c => c.ManagerId == currentUserId && c.UserRole == (int)RepgroupUserType.Director)));
                //---ManagerChange      query = query.Where(a => a.PracticeProviderMappers.Any(b => b.Practice.Rep.RepGroup.ManagerId == currentUserId));
                else if (isRep)
                    query = query.Where(a => a.PracticeProviderMappers.Any(b => b.Practice.Rep.UserId == currentUserId));

                if (!string.IsNullOrEmpty(searchKey))
                    query = query.Where(a => (a.FirstName + " " + a.MiddleName + " " + a.LastName).Contains(searchKey));

                var selectQuery = query.Select(s => new EntitySelectItem
                {
                    Id = s.Id,
                    Value = s.FirstName + " " + s.LastName
                }).OrderBy(o => o.Value);

                response = GetList<EntitySelectItem>(selectQuery, 0, 2000);
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

        public DataResponse<EntityList<EntitySelectItem>> GetAllServices(int currentBusinessId, int currentUserId, bool isSalesManager, bool isSalesDirector, bool isRep, bool isBuzAdmin)
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            try
            {
                base.DBInit();

                var query = DBEntity.LookupEnrolledServices.Where(s => s.IsActive == true && s.BusinessId == currentBusinessId);

                if (!isBuzAdmin)
                {
                    if (isSalesManager || isSalesDirector || isRep)
                        query = query.Where(s =>
                            //s.RepServiceMappers.Any(a => a.Rep.RepGroup.SalesDirectorId == currentUserId) ||
                            s.RepServiceMappers.Any(a => a.Rep.RepGroup.RepgroupManagerMappers.Any(b => b.ManagerId == currentUserId)) ||
                            s.RepServiceMappers.Any(a => a.Rep.UserId == currentUserId));
                }

                //if (isSalesDirector)
                //    query = query.Where(s => s.RepServiceMappers.Any(a => a.Rep.RepGroup.SalesDirectorId == currentUserId));

                //else if (isSalesManager)
                //    query = query.Where(s => s.RepServiceMappers.Any(a => a.Rep.RepGroup.RepgroupManagerMappers.Any(b => b.ManagerId == currentUserId)));
                ////---ManagerChange      query = query.Where(s => s.RepServiceMappers.Any(a => a.Rep.RepGroup.ManagerId == currentUserId));
                //else if (isRep)
                //    query = query.Where(s => s.RepServiceMappers.Any(a => a.Rep.UserId == currentUserId));

                var selectQuery = query.Select(s => new EntitySelectItem
                {
                    Id = s.Id,
                    Value = s.ServiceName,
                    IsSelected = s.Status,
                    Text = s.ServiceColor//temp setting
                }).OrderBy(o => o.IsSelected != true);//OrderBy(o => o.Value.ToString().ToLower() != "pharmacogenetics");

                response = GetList<EntitySelectItem>(selectQuery, 0, 100);
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

        public DataResponse<EntityList<EntitySelectItem>> GetAllRepServices(int currentBusinessId)
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();

            try
            {
                base.DBInit();
                var query = DBEntity.LookupEnrolledServices.Where(s => s.IsActive == true && s.BusinessId == currentBusinessId).Select(b => new EntitySelectItem
                {
                    Value = b.Id.ToString(),
                    Text = b.ServiceName
                }).OrderByDescending(a => a.Value);
                response = GetList<EntitySelectItem>(query, 0, 100);
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

        public List<string> GetServicesById(int[] ids)
        {

            var serviceList = new List<string>();
            try
            {
                base.DBInit();

                serviceList = DBEntity.LookupEnrolledServices.Where(s => ids.Contains(s.Id)).Select(a => a.ServiceName).ToList();

            }
            catch (Exception ex)
            {

                ex.Log();
            }
            finally
            {
                base.DBClose();
            }

            return serviceList;

        }

        /*
        public DataResponse<EntityList<EntitySelectItem>> GetProvidersByServiceId(int serviceId, int? currentBusinessId)
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            try
            {
                base.DBInit();

                var query = DBEntity.ReportMasters.Where(a => a.ServiceId == serviceId && a.BusinessId == currentBusinessId)
                    .Select(s => new EntitySelectItem
                    {
                        Id = s.Provider.Id,
                        Value = s.Provider.FirstName + " " + s.Provider.MiddleName + " " + s.Provider.LastName + " (" + s.Provider.NPI + ")"
                    }).OrderBy(o => o.Value).DistinctBy(a => a.Id);

                response = GetList(query, 0, 100);
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
        */

        public DataResponse<EntityList<EntitySelectItem>> GetAllTaskTypes(int? currentBusinessId)
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            try
            {
                var cachedResponse = response.GetCached<EntityList<EntitySelectItem>>(CacheKeys.TaskType + currentBusinessId);

                if (cachedResponse.IsOk())
                    return cachedResponse;

                base.DBInit();
                var query = DBEntity.LookupTaskTypes.Where(a => a.BusinessId == currentBusinessId).Select(s => new EntitySelectItem
                {
                    Id = s.Id,
                    Value = s.TaskType
                }).OrderBy(o => o.Value);
                response = GetList<EntitySelectItem>(query, 0, 100);

                response.CacheIt<EntityList<EntitySelectItem>>(CacheKeys.TaskType + currentBusinessId);
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

        public DataResponse<EntityList<EntitySelectItem>> GetAllmarketingCategories(int currentBusinessId)
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            try
            {
                base.DBInit();
                var query = DBEntity.LookupMarketingCategories.Where(a => a.BusinessId == currentBusinessId).Select(s => new EntitySelectItem
                {
                    Id = s.Id,
                    Value = s.Category
                }).OrderBy(o => o.Value);
                response = GetList<EntitySelectItem>(query, 0, 100);
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

        public DataResponse<EntityList<EntitySelectItem>> GetAllDocumentTypes(int currentBusinessId)
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            try
            {
                base.DBInit();
                var query = DBEntity.LookupDocumentTypes.Where(a => a.BusinessId == currentBusinessId).Select(s => new EntitySelectItem
                {
                    Id = s.Id,
                    Value = s.DocumentType
                }).OrderBy(o => o.Value);
                response = GetList<EntitySelectItem>(query, 0, 100);
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

        public DataResponse<EntityList<EntitySelectItem>> GetAllUsersByBusinessId(int currentBusinessId)
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            try
            {
                base.DBInit();
                var query = DBEntity.Users.Where(a => a.BusinessId == currentBusinessId).Select(s => new EntitySelectItem
                {
                    Id = s.Id,
                    Value = s.FirstName.Trim() + " " + s.LastName.Trim(),
                }).OrderBy(o => o.Value);
                response = GetList<EntitySelectItem>(query, 0, 100);
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

        public string GetServiceNameById(int serviceId)
        {
            try
            {
                base.DBInit();

                var objService = DBEntity.LookupEnrolledServices.FirstOrDefault(a => a.Id == serviceId);
                if (objService != null)
                    return objService.ServiceName;
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }
            return null;
        }

        public DataResponse<EntityList<EntitySelectItem>> GetServiceColumnLookup(int columnId)
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            try
            {
                base.DBInit();
                var query = DBEntity.LookupServiceColumns.Where(a => a.ColumnId == columnId).Select(s => new EntitySelectItem
                {
                    Id = s.Id,
                    Value = s.Text,
                }).OrderBy(o => o.Value);
                response = GetList<EntitySelectItem>(query, 0, 100);
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

        public string GetPracticeNameById(int practiceId)
        {
            try
            {
                base.DBInit();

                var objService = DBEntity.Practices.FirstOrDefault(a => a.Id == practiceId);
                if (objService != null)
                    return objService.PracticeName;
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }
            return null;
        }

        public DataResponse<EntityList<EntitySelectItem>> GetAllDirectors(int? businessId)
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            try
            {
                base.DBInit();

                var query = DBEntity.Users.Where(a => a.BusinessId == businessId && a.IsActive == true
                         && !a.Roles.Select(y => y.Name).Contains("BusinessAdmin")
                         && !a.Roles.Select(y => y.Name).Contains("SuperAdmin")
                         && !a.Roles.Select(y => y.Name).Contains("MasterAdmin")
                         && (a.Roles.Any(r => r.RolePrivileges.Any(rp => rp.Privilege.PrivilegeKey == "MNGALLSLSTMS"))
                         || a.UserPrivileges2.Any(b => b.Privilege.PrivilegeKey == "MNGALLSLSTMS")));

                var selectquery = query.Select(s => new EntitySelectItem
                {
                    Id = s.Id,
                    Value = s.FirstName + " " + s.LastName
                }).OrderBy(a => a.Value).ToList();
                response = GetList<EntitySelectItem>(selectquery, 0, 1000);

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
        public DataResponse<EntityList<EntitySelectItem>> GetAllMangers(int? businessId)
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            try
            {
                base.DBInit();

                var query = DBEntity.Users.Where(a => a.BusinessId == businessId && a.IsActive == true);

                var selectquery = query.Select(s => new EntitySelectItem
                {
                    Id = s.Id,
                    Value = s.FirstName + " " + s.LastName
                }).OrderBy(a => a.Value).ToList();
                response = GetList<EntitySelectItem>(selectquery, 0, 1000);

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

        public DataResponse<EntityList<EntitySelectItem>> GetAllUsersForRep(int currentBusinessId, int currentUserId, int repUserId = 0)
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            try
            {
                base.DBInit();
                var RepIds = DBEntity.Reps.Select(a => a.UserId);
                if (repUserId != 0)
                {
                    RepIds = RepIds.Where(a => a != repUserId);
                }
                var query = DBEntity.Users.Where(a => a.BusinessId == currentBusinessId && a.IsActive == true && a.Id != currentUserId && !RepIds.Contains(a.Id)).Select(s => new EntitySelectItem
                {
                    Id = s.Id,
                    Value = s.FirstName.Trim() + " " + s.LastName.Trim(),
                }).OrderBy(o => o.Value);
                response = GetList<EntitySelectItem>(query, 0, 100);
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
        public DataResponse<EntityList<EntitySelectItem>> GetAllUsersForServiceColumn(int currentBusinessId, int currentUserId)
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            try
            {
                base.DBInit();
                var RepIds = DBEntity.Reps.Select(a => a.UserId);

                var query = DBEntity.Users.Where(a => a.BusinessId == currentBusinessId && a.IsActive == true && !a.Roles.Select(y => y.Name).Contains("BusinessAdmin") && !a.Roles.Select(y => y.Name).Contains("SuperAdmin") && !a.Roles.Select(y => y.Name).Contains("MasterAdmin")).Select(s => new EntitySelectItem
                {
                    Id = s.Id,
                    Value = s.FirstName.Trim() + " " + s.LastName.Trim(),
                }).OrderBy(o => o.Value);
                response = GetList<EntitySelectItem>(query, 0, 100);
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

        public DataResponse<EntityList<PrivilegeModulesmodel>> GetAllPrivileges()
        {
            var response = new DataResponse<EntityList<PrivilegeModulesmodel>>();

            try
            {
                base.DBInit();
                var query = DBEntity.Privileges.GroupBy(a => a.ModuleId).Select(b => new PrivilegeModulesmodel
                {
                    ModuleName = b.FirstOrDefault(a => a.ModulesMaster.Id == a.ModuleId).ModulesMaster.Title,
                    Privileges = b.Select(c => new EntitySelectItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Title
                    })
                }).OrderByDescending(a => a.ModuleName);
                response = GetList<PrivilegeModulesmodel>(query, 0, 100);
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

        public DataResponse<EntityList<EntitySelectItem>> GetAllDepartments(int? businessId)
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            try
            {
                base.DBInit();

                var query = DBEntity.Departments.Where(a => a.BusinessId == businessId).OrderBy(a => a.DepartmentName);

                var selectquery = query.Select(s => new EntitySelectItem
                {
                    Id = s.Id,
                    Value = s.Id.ToString(),
                    Text = s.DepartmentName
                }).OrderBy(a => a.Value).ToList();
                response = GetList<EntitySelectItem>(selectquery, 0, 1000);

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

        public DataResponse<EntityList<EntitySelectItem>> GetAllRoles(int? businessId)
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            try
            {
                base.DBInit();

                var query = DBEntity.Roles.Where(a => a.BusinessId == businessId).OrderBy(a => a.Name);

                var selectquery = query.Select(s => new EntitySelectItemExt
                {
                    Id = s.Id,
                    Value = s.Id.ToString(),
                    Text = s.Name,
                    IsSalesRep = s.Name.ToLower().Contains("sales rep")
                }).OrderBy(a => a.Value).ToList();
                response = GetList<EntitySelectItem>(selectquery, 0, 1000);

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
        //public DataResponse<EntityList<int>> GetAllManagersByRepGroupId(int RepGroupId)
        //{
        //    var response = new DataResponse<EntityList<int>>();
        //    try
        //    {
        //        base.DBInit();
        //        var selectquery = DBEntity.RepgroupManagerMappers.Where(t => t.RepGroupId == RepGroupId).Select(a => a.ManagerId);
        //        response = GetList<int>(selectquery);
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.Log();
        //    }
        //    finally
        //    {
        //        base.DBClose();
        //    }
        //    return response;
        //}
        public DataResponse<string> GetAllManagersByRepGroupId(int RepGroupId)
        {
            var response = new DataResponse<string>();
            try
            {
                base.DBInit();
                var selectquery = DBEntity.RepgroupManagerMappers.Where(t => t.RepGroupId == RepGroupId).Select(a => a.User.FirstName + " " + a.User.MiddleName + " " + a.User.LastName).ToList();
                var ManagerIds = string.Join(",", selectquery);
                response.Model = ManagerIds;
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

    public partial class EntitySelectItemExt : EntitySelectItem
    {
        public bool IsSalesRep { get; set; }
    }
}
