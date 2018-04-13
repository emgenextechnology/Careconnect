using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EBP.Business.Entity.UserProfile;
using EBP.Business.Entity;
using EBP.Business.Entity.Business;

namespace EBP.Business.Repository
{
    public class RepositoryUserProfile : _Repository
    {
        public EntityUser GetUserbyEmail(string email)
        {
            EntityUser entity = new EntityUser();
            try
            {
                base.DBInit();
                var query = DBEntity.Users.Where(a => a.Email == email);
                var response = base.GetFirst(query).Model;
                if (response != null)
                {
                    entity = new EntityUser
                    {
                        Id = response.Id,
                        BusinessId = response.BusinessId,
                        FirstName = response.FirstName,
                        LastName = response.LastName,
                        EmailAddress = response.Email,
                        PhoneNumber = response.PhoneNumber
                    };
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
            return entity;
        }

        public EntityUser GetUserbyUsername(string username)
        {
            EntityUser entity = new EntityUser();
            try
            {
                base.DBInit();
                var query = DBEntity.Users.Where(a => a.UserName == username);
                var response = base.GetFirst(query).Model;
                if (response != null)
                {
                    entity = new EntityUser
                    {
                        Id = response.Id,
                        BusinessId = response.BusinessId,
                        FirstName = response.FirstName,
                        MiddleName = response.MiddleName,
                        LastName = response.LastName,
                        EmailAddress = response.Email,
                        PhoneNumber = response.PhoneNumber
                    };
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
            return entity;
        }

        public EntityUser GetUserbyId(int id)
        {
            EntityUser entity = new EntityUser();
            try
            {
                base.DBInit();
                var query = DBEntity.Users.Where(a => a.Id == id);
                var response = base.GetFirst(query).Model;
                if (response != null)
                {
                    entity = new EntityUser
                    {
                        Id = response.Id,
                        BusinessId = response.BusinessId,
                        FirstName = response.FirstName,
                        LastName = response.LastName,
                        EmailAddress = response.Email,
                        PhoneNumber = response.PhoneNumber,
                        BusinessName = response.BusinessMaster.BusinessName,
                        RelativeUrl = response.BusinessMaster.RelativeUrl,
                        
                    };
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
            return entity;
        }

        public DataResponse<EntityUser> GetCurrentUserbyId(int UserId)
        {
            var response = new DataResponse<EntityUser>();
            try
            {
                base.DBInit();

                var query = DBEntity.Users.Where(a => a.Id == UserId).Select(a => new EntityUser
                {
                    Id = a.Id,
                    BusinessId = a.BusinessId,
                    FirstName = a.FirstName,
                    MiddleName = a.MiddleName,
                    LastName = a.LastName,
                    EmailAddress = a.Email,
                    PhoneNumber = a.PhoneNumber
                });
                response = GetFirst<EntityUser>(query);
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
        public DataResponse<EntityUser> GetCurrentUserDeviceTokenbyId(int UserId)
        {
            var response = new DataResponse<EntityUser>();
            try
            {
                base.DBInit();

                var query = DBEntity.UserProfiles.Where(a => a.UserId == UserId).Select(a => new EntityUser
                {
                 Id=a.UserId.Value,
                 DeviceToken=a.DeviceId
                });
                response = GetFirst<EntityUser>(query);
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

        public DataResponse<bool> UpdateDeviceId(int id, string DeviceId)
        {
            DataResponse<bool> response = new DataResponse<bool>();
            try
            {
                base.DBInit();
                var model = DBEntity.UserProfiles.Where(a => a.UserId == id).Select(a => a).FirstOrDefault();
                if (model != null)
                {

                    model.DeviceId = DeviceId;
                    if (base.DBSaveUpdate(model) > 0)
                    {
                        response.CreateResponse(true, DataResponseStatus.OK);
                    }
                    else
                    {
                        response.CreateResponse(false, DataResponseStatus.NotModified);
                    }
                }
                else
                {
                    response.CreateResponse(false, DataResponseStatus.NoContent);
                }
            }
            catch (Exception ex)
            {
                response.ThrowError(ex);
            }
            finally
            {
                DBClose();
            }

            return response;
        }
        public string[] GetUserPrivilages(int id)
        {
            string[] privileges = null;
            try
            {
                base.DBInit();

                var roles = DBEntity.Users.FirstOrDefault(a => a.Id == id).Roles.Select(a => a.Id).ToList();
                foreach (var item in roles)
                {
                    var rolePrivileges = DBEntity.RolePrivileges.Where(b => b.RoleId == item).Select(c => c.Privilege.PrivilegeKey);
                    if (privileges == null)
                        privileges = rolePrivileges.ToArray();
                    else
                        privileges = privileges.Union(rolePrivileges).ToArray();
                }

                var departments = DBEntity.UserDepartments.Where(a => a.UserId == id).Select(a => a.DepartmentId).ToList();
                foreach (var item in departments)
                {
                    var departmentPrivileges = DBEntity.DepartmentPrivileges.Where(b => b.DepartmentId == item).Select(c => c.Privilege.PrivilegeKey);
                    if (privileges == null)
                        privileges = departmentPrivileges.ToArray();
                    else
                        privileges = privileges.Union(departmentPrivileges).ToArray();
                }

                var userPrivileges = DBEntity.UserPrivileges.Where(a => a.UserId == id && a.IsDeny != true).Select(a => a.Privilege.PrivilegeKey);
                if (privileges == null)
                    privileges = userPrivileges.ToArray();
                else
                    privileges = privileges.Union(userPrivileges).ToArray();

                var deniedPrivilages = DBEntity.UserPrivileges.Where(a => a.UserId == id && a.IsDeny == true).Select(a => a.Privilege.PrivilegeKey).ToArray();
                privileges = privileges.Except(deniedPrivilages).ToArray();
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }
            return privileges;
        }

        public DataResponse UpdateUser(EntityUser entity)
        {
            var response = new DataResponse();
            try
            {
                base.DBInit();
                if (!DBEntity.Users.Any(a => a.Email == entity.EmailAddress))
                {
                    var modelItem = DBEntity.Users.FirstOrDefault(a => a.Id == entity.Id);
                    modelItem.FirstName = entity.FirstName;
                    modelItem.LastName = entity.LastName;
                    modelItem.Email = entity.EmailAddress;
                    modelItem.PhoneNumber = entity.PhoneNumber;
                    base.DBSaveUpdate(modelItem);
                }
                else
                {
                    response.Status = DataResponseStatus.InternalServerError;
                    response.Message = "Email already exists";
                    response.Id = entity.Id;
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

        public List<EntitySelectItem> GetCurrentUserPrivilages(int id)
        {
            List<EntitySelectItem> privilages = null;
            try
            {
                base.DBInit();
                var roles = DBEntity.Users.FirstOrDefault(a => a.Id == id).Roles.Select(a => a.Id).ToList();
                foreach (var item in roles)
                {
                    var s = DBEntity.RolePrivileges.Where(b => b.RoleId == item).Select(c => new EntitySelectItem { Text = c.Privilege.Title, Id = c.Privilege.Id });
                    if (privilages == null)
                        privilages = s.ToList();
                    else
                        privilages = privilages.Union(s).ToList();
                }
                var departments = DBEntity.Users.FirstOrDefault(a => a.Id == id).Departments.Select(a => a.Id).ToList();
                foreach (var item in departments)
                {
                    var s = DBEntity.DepartmentPrivileges.Where(b => b.DepartmentId == item).Select(c => new EntitySelectItem { Text = c.Privilege.Title, Id = c.Privilege.Id });
                    if (privilages == null)
                        privilages = s.ToList();
                    else
                        privilages = privilages.Union(s).ToList();
                }
                privilages = privilages.GroupBy(x => x.Text).Select(x => x.First()).ToList();

            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }
            return privilages;
        }

        public DataResponse<EntityList<EntitySelectItem>> GetAllUserRoles(int userId)
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            try
            {
                base.DBInit();
                var query = DBEntity.Roles.Where(a => a.Users.Any(n => n.Id == userId)).Select(a => new EntitySelectItem { Id = a.Id, Value = a.Name, Text = a.Name }).OrderBy(o => o.Text);
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

        public DataResponse<EntityList<EntitySelectItem>> GetAllUserDepartments(int userId)
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            try
            {
                base.DBInit();
                var query = DBEntity.UserDepartments.Where(a => a.UserId == userId).Select(a => new EntitySelectItem { Id = a.DepartmentId, Value = a.Department.DepartmentName, Text = a.Department.DepartmentName }).OrderBy(a => a.Id);
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

        public EntityBusiness GetBusinessbyId(int BusinessId)
        {
            EntityBusiness entity = new EntityBusiness();
            try
            {
                base.DBInit();
                var query = DBEntity.BusinessMasters.Where(a => a.Id == BusinessId);
                var response = base.GetFirst(query).Model;
                if (response != null)
                {
                    entity = new EntityBusiness
                    {
                        BusinessName = response.BusinessName,
                        DomainUrl = response.DomainUrl,
                        RelativeUrl = response.RelativeUrl,
                    };
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
            return entity;
        }

        public DataResponse<EntityUser> Update(EntityUser entity)
        {
            var response = new DataResponse<EntityUser>();
            try
            {
                base.DBInit();
                var model = DBEntity.Users.FirstOrDefault(a => a.Id == entity.Id);

                model.FirstName = entity.FirstName;
                model.MiddleName = entity.MiddleName;
                model.LastName = entity.LastName;
                model.PhoneNumber = entity.PhoneNumber;
                if (DBEntity.SaveChanges() < 0)
                {
                    response.CreateResponse(DataResponseStatus.InternalServerError);
                }
                else
                {
                    return GetCurrentUserbyId(model.Id);
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

        public string NotificationEnabledEmails(string Email, string NotificationKey)
        {
            List<string> emails = null;
            List<string> lstReturn = new List<string>();
            try
            {
                if (!string.IsNullOrEmpty(Email))
                {
                    base.DBInit();
                    emails = Email.Split(',').ToList();

                    foreach (var item in emails)
                    {
                        bool isDisabled = DBEntity.UserNotificationMappers.Any(a => a.User.Email == item && a.LookupNotificationType.NotificationKey == NotificationKey && a.Status == false);

                        if (!isDisabled)
                            lstReturn.Add(item);
                    }
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }
            return string.Join(",", lstReturn);
        }
    }
}
