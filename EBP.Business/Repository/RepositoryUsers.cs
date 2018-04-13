using EBP.Business.Database;
using EBP.Business.Entity;
using EBP.Business.Entity.Privileges;
using EBP.Business.Entity.Users;
using EBP.Business.Filter;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Linq.Dynamic;

namespace EBP.Business.Repository
{
    public class RepositoryUsers : _Repository
    {
        public DataResponse<EntityList<EntityUsers>> GetUsers(FilterUsers filter, int currentBusineId, int take = 10, int skip = 0)
        {
            var response = new DataResponse<EntityList<EntityUsers>>();
            try
            {
                if (filter != null)
                {
                    take = filter.Take;
                    skip = filter.Skip;
                }

                base.DBInit();

                var query = DBEntity.Users.Where(a => a.BusinessId == currentBusineId && !a.Roles.Select(y => y.Name).Contains("BusinessAdmin") && !a.Roles.Select(y => y.Name).Contains("SuperAdmin") && !a.Roles.Select(y => y.Name).Contains("MasterAdmin"));
                if (filter != null)
                {
                    if (!String.IsNullOrEmpty(filter.KeyWords))
                    {
                        query = query.Where(ua => (
                    ua.FirstName + " " + ua.MiddleName).ToLower().Contains(filter.KeyWords.ToLower())
                    || (ua.FirstName + " " + ua.LastName).ToLower().Contains(filter.KeyWords.ToLower())
                    || (ua.FirstName + " " + ua.MiddleName + " " + ua.LastName).ToLower().Contains(filter.KeyWords.ToLower())
                    || ua.FirstName.ToLower().Contains(filter.KeyWords.ToLower())
                    || ua.MiddleName.ToLower().Contains(filter.KeyWords.ToLower())
                    || ua.LastName.ToLower().Contains(filter.KeyWords.ToLower())
                    || ua.Email.ToLower().Contains(filter.KeyWords.ToLower())
                    || ua.UserName.ToLower().Contains(filter.KeyWords.ToLower())
                    || ua.Roles.Any(t => t.Name.ToLower().Contains(filter.KeyWords.ToLower()))
                    || ua.Departments.Any(t => t.DepartmentName.ToLower().Contains(filter.KeyWords.ToLower()))
                    || ua.PhoneNumber.ToLower().Contains(filter.KeyWords.ToLower()));
                    }
                }

                var selectQuery = query.Select(a => new EntityUsers
                {
                    Id = a.Id,
                    FirstName = a.FirstName,
                    MiddleName = a.MiddleName,
                    LastName = a.LastName,
                    UserName = a.UserName,
                    Email = a.Email,
                    IsActive = a.IsActive,
                    PhoneNumber = a.PhoneNumber,
                    BusinessId = a.BusinessId,
                    RoleIds = a.Roles.Select(b => b.Id).ToList(),
                    UserRoleNames = a.Roles.Select(p => p.Name),
                    UserDepartmentName = a.UserDepartments2.Select(b => b.Department.DepartmentName),
                    LastLoggedInTime = a.UserProfiles.FirstOrDefault().LastLoggedInTime,
                });

                if (string.IsNullOrEmpty(filter.SortKey) || string.IsNullOrEmpty(filter.SortOrder))
                {
                    selectQuery = selectQuery.OrderByDescending(o => o.Id);
                }
                else
                {
                    string orderBy = string.Format("{0} {1}", filter.SortKey, filter.SortOrder);
                    selectQuery = selectQuery.OrderBy(orderBy);
                }

                response = GetList<EntityUsers>(selectQuery, skip, take);
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

        public DataResponse<EntityUsers> GetUserById(int UserId)
        {
            var response = new DataResponse<EntityUsers>();
            try
            {
                base.DBInit();

                var query = DBEntity.Users.Where(a => a.Id == UserId).Select(a => new EntityUsers
                {
                    Id = a.Id,
                    FirstName = a.FirstName,
                    MiddleName = a.MiddleName,
                    LastName = a.LastName,
                    UserName = a.UserName,
                    Email = a.Email,
                    PhoneNumber = a.PhoneNumber,
                    HomePhone = a.UserProfiles.Select(p => p.HomePhone).FirstOrDefault(),
                    AdditionalPhone = a.UserProfiles.Select(p => p.AdditionalPhone).FirstOrDefault(),
                    WorkEmail = a.UserProfiles.Select(p => p.WorkEmail).FirstOrDefault(),
                    AddressLine1 = a.UserProfiles.Select(p => p.AddressLine1).FirstOrDefault(),
                    AddressLine2 = a.UserProfiles.Select(p => p.AddressLine2).FirstOrDefault(),
                    City = a.UserProfiles.Select(p => p.City).FirstOrDefault(),
                    StateId = a.UserProfiles.Select(p => p.StateId).FirstOrDefault(),
                    State = a.LookupStates.Select(p => p.StateName).FirstOrDefault(),
                    Zip = a.UserProfiles.Select(p => p.Zip).FirstOrDefault(),
                    BusinessId = a.BusinessId,
                    IsActive = a.IsActive,
                    LastLoggedInTime = a.UserProfiles.FirstOrDefault().LastLoggedInTime,
                    RoleIds = a.Roles.Select(b => b.Id).ToList(),
                    // a.RepServiceMappers.Where(b => b.RepId == a.Id).Select(p => p.LookupEnrolledService.ServiceName),
                    // UserRoleNames=a.Roles.Where(b=>b.u)
                   // UserRoles = a.Roles.Select(p => p.Name),
                    DepartmentIds = a.UserDepartments2.Select(b => b.DepartmentId).ToList(),
                    UserDepartmentName = a.UserDepartments2.Select(b => b.Department.DepartmentName),
                    //StartDate = a.UserProfiles.Select(p => p.Startdate==null?null:p.Startdate.GetValueOrDefault().ToString("MM-dd-yyyy")),
                    StartDate = a.UserProfiles.Select(p => p.Startdate).FirstOrDefault() == null ? null : a.UserProfiles.Select(p => p.Startdate).FirstOrDefault().ToString()//.GetValueOrDefault().ToString("MM-dd-yyyy"),
                });
                response = GetFirst<EntityUsers>(query);
                if (!string.IsNullOrEmpty(response.Model.StartDate))
                    response.Model.StartDate = DateTime.Parse(response.Model.StartDate).ToString("MM/dd/yyyy");
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

        public DataResponse<EntityPrivileges> GetPrivilegeById(int UserId, int CurrentBusinessId)
        {
            var response = new DataResponse<EntityPrivileges>();
            try
            {
                base.DBInit();
                var model = new EntityPrivileges();
                model.UserId = UserId;
                model.User = new RepositoryUserProfile().GetUserbyId(UserId);
                var UserPrivilage = DBEntity.UserPrivileges.Where(a => a.UserId == UserId && a.BusinessId == CurrentBusinessId).ToList();
                model.UserDepartments = DBEntity.UserDepartments.Where(a => a.UserId == UserId).Select(a => new DepartmentPrivileges
                {
                    DepartmentName = a.Department.DepartmentName,
                    Privileges = a.Department.DepartmentPrivileges.Where(b => b.DepartmentId == a.DepartmentId).Select(c => new SelectListItem
                    {
                        Value = c.Privilege.Id.ToString(),
                        Text = c.Privilege.Title
                    })
                }).ToList();
                model.UserRoles = DBEntity.Roles.Where(a => a.BusinessId == CurrentBusinessId && a.Users.Any(u => u.Id == UserId)).Select(a => new DepartmentPrivileges
                {
                    DepartmentName = a.Name,
                    Privileges = a.RolePrivileges.Where(b => b.RoleId == a.Id).Select(c => new SelectListItem
                    {
                        Value = c.Privilege.Id.ToString(),
                        Text = c.Privilege.Title
                    })
                }).ToList();
                model.Modules = DBEntity.Privileges.GroupBy(a => a.ModuleId).Select(g => new Modulesmodel
                {
                    ModuleName = g.FirstOrDefault(a => a.ModulesMaster.Id == g.Key).ModulesMaster.Title,
                    UserPrivileges = g.Select(b => new Privileges
                    {
                        Id = b.Id,
                        Name = b.Title,
                    }).ToList()

                }).OrderByDescending(a => a.ModuleName).ToList();
                model.Modules.ForEach(a =>
                    a.UserPrivileges.ForEach(b =>
                    {
                        b.Deny = UserPrivilage.Any(c => c.PrivilegeId == b.Id) ? (bool?)UserPrivilage.Any(c => c.PrivilegeId == b.Id && c.IsDeny == true) : null;
                        b.Allow = b.Deny == null ? null : !b.Deny;
                    })
                 );
                response.Model = model;
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

        public DataResponse ToggleStatus(int UserId)
        {
            var response = new DataResponse<bool?>();
            try
            {
                base.DBInit();

                var model = DBEntity.Users.FirstOrDefault(a => a.Id == UserId);
                response.Model = model.IsActive;
                response.Id = UserId;
                if (model != null)
                {
                    model.IsActive = model.IsActive == false ? true : false;
                    model.LockoutEnabled = model.IsActive == false ? true : false;
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

        public DataResponse<VMPrivilege> SetPrivileges(List<Privileges> entity, int CurrentUserId, int CurrentBusinessId)
        {
            var response = new DataResponse<VMPrivilege>();
            try
            {
                base.DBInit();
                if (entity != null)
                {
                    var userId = entity.FirstOrDefault().UserId;
                    IEnumerable<UserPrivilege> UserPrivileges = DBEntity.UserPrivileges.Where(a => a.UserId == userId & a.BusinessId == CurrentBusinessId).ToList();
                    if (UserPrivileges.Count() > 0)
                    {
                        DBEntity.UserPrivileges.RemoveRange(UserPrivileges);
                        int DeleteStatus = DBEntity.SaveChanges();
                        if (DeleteStatus > 0)
                        {
                            response.Message = "Sucessfully Saved";
                            response.Status = DataResponseStatus.OK;
                            response.Model = null;
                        }
                    }
                    else
                    {
                        response.Message = "Failed!";
                        response.Status = DataResponseStatus.InternalServerError;
                        response.Model = null;
                    }

                    foreach (var privilege in entity)
                    {

                        if (privilege.Id != 0 && privilege.Deny != null)
                        {
                            DBEntity.UserPrivileges.Add(new UserPrivilege
                            {
                                UserId = privilege.UserId,
                                PrivilegeId = privilege.Id,
                                IsDeny = privilege.Deny,
                                CreatedOn = DateTime.UtcNow,
                                CreatedBy = CurrentUserId,
                                BusinessId = CurrentBusinessId
                            });
                        }
                    }

                    int result = DBEntity.SaveChanges();
                    if (result > 0)
                    {
                        response.Message = "Sucessfully Saved";
                        response.Status = DataResponseStatus.OK;

                        var _userPrivileges = DBEntity.UserPrivileges.Where(a => a.UserId == userId).Select(a => new PrivilegeDetails
                        {
                            Title = a.Privilege.Title,
                            Description = a.Privilege.Description
                        }).ToList();

                        var UserPrivilegeNames = DBEntity.UserPrivileges.Where(a => a.UserId == userId).Select(a => a.Privilege.Title).ToList();
                        var UserModel = DBEntity.Users.FirstOrDefault(a => a.Id == userId);
                        response.Model = new VMPrivilege
                        {
                            UserPrivilegesList=_userPrivileges,
                            UserPrivileges = UserPrivilegeNames,
                            UserName = UserModel.FirstName,
                            Email = UserModel.Email
                        };
                    }
                }
                else
                {
                    response.Message = "Failed!";
                    response.Status = DataResponseStatus.InternalServerError;
                    response.Model = null;
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

        //public DataResponse SetPrivileges(List<Privileges> entity, int CurrentUserId, int CurrentBusinessId)
        //{
        //    var response = new DataResponse<bool?>();
        //    try
        //    {
        //        base.DBInit();
        //        if (entity != null)
        //        {
        //            var userId = entity.FirstOrDefault().UserId;
        //            IEnumerable<UserPrivilege> UserPrivileges = DBEntity.UserPrivileges.Where(a => a.UserId == userId & a.BusinessId == CurrentBusinessId).ToList();
        //            if (UserPrivileges.Count() > 0)
        //            {
        //                DBEntity.UserPrivileges.RemoveRange(UserPrivileges);
        //                int DeleteStatus = DBEntity.SaveChanges();
        //                if (DeleteStatus > 0)
        //                {
        //                    response.Message = "Sucessfully Saved";
        //                    response.Status = DataResponseStatus.OK;
        //                }
        //            }
        //            else
        //            {
        //                response.Message = "Failed!";
        //                response.Status = DataResponseStatus.InternalServerError;
        //            }

        //            foreach (var privilege in entity)
        //            {

        //                if (privilege.Id != 0)
        //                {
        //                    DBEntity.UserPrivileges.Add(new UserPrivilege
        //                    {
        //                        UserId = privilege.UserId,
        //                        PrivilegeId = privilege.Id,
        //                        IsDeny = privilege.Deny,
        //                        CreatedOn = DateTime.UtcNow,
        //                        CreatedBy = CurrentUserId,
        //                        BusinessId = CurrentBusinessId
        //                    });
        //                }
        //            }

        //            int result = DBEntity.SaveChanges();
        //            if (result > 0)
        //            {
        //                response.Message = "Sucessfully Saved";
        //                response.Status = DataResponseStatus.OK;
        //            }
        //        }
        //        else
        //        {
        //            response.Message = "Failed!";
        //            response.Status = DataResponseStatus.InternalServerError;
        //        }

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

        public DataResponse<EntityUsers> UpdateUserProfile(EntityProfile entity)
        {
            var response = new DataResponse<EntityUsers>();
            try
            {
                base.DBInit();
                bool result = false;
                #region Prepare model
                var model = DBEntity.UserProfiles.FirstOrDefault(a => a.UserId == entity.Id);
                if (model != null)
                {
                    model.WorkEmail = entity.WorkEmail;
                    model.HomePhone = entity.HomePhone;
                    model.AdditionalPhone = entity.AdditionalPhone;
                    model.AddressLine1 = entity.AddressLine1;
                    model.AddressLine2 = entity.AddressLine2;
                    model.City = entity.City;
                    model.Zip = entity.Zip;
                    model.StateId = entity.StateId;
                    model.UpdatedBy = entity.UpdatedBy;
                    model.UpdatedOn = entity.UpdatedOn;
                    model.Startdate = entity.Startdate == null ? null : entity.Startdate;
                    if (base.DBSaveUpdate(model) > 0)
                    {
                        result = true;
                    }
                }
                else
                {
                    model = DBEntity.UserProfiles.Add(new UserProfile
                    {
                        UserId = entity.UserId,
                        WorkEmail = entity.WorkEmail,
                        HomePhone = entity.HomePhone,
                        AdditionalPhone = entity.AdditionalPhone,
                        AddressLine1 = entity.AddressLine1,
                        AddressLine2 = entity.AddressLine2,
                        City = entity.City,
                        Zip = entity.Zip,
                        StateId = entity.StateId,
                        CreatedOn = DateTime.UtcNow,
                        CreatedBy = entity.CreatedBy,
                        Startdate = entity.Startdate
                    });

                    if (DBEntity.SaveChanges() > 0)
                    {
                        result = true;
                    }
                }
                #endregion

                if (result == true)
                {
                    IEnumerable<UserDepartment> UserDepartment = DBEntity.UserDepartments.Where(a => a.UserId == entity.UserId).ToList();
                    if (UserDepartment.Count() > 0)
                    {
                        DBEntity.UserDepartments.RemoveRange(UserDepartment);
                        DBEntity.SaveChanges();
                    }
                    if (entity.DepartmentIds != null && entity.DepartmentIds.Count() > 0)
                    {
                        foreach (var item in entity.DepartmentIds)
                        {
                            DBEntity.UserDepartments.Add(new UserDepartment { UserId = entity.UserId.Value, DepartmentId = item, CreatedBy = entity.CreatedBy, CreatedOn = DateTime.UtcNow });
                            DBEntity.SaveChanges();
                        }
                    }
                    return GetUserById(model.UserId.Value);
                }
                else
                {
                    response.CreateResponse(DataResponseStatus.InternalServerError);
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

        public DataResponse<EntityUsers> insertUserProfile(EntityProfile entity)
        {
            var response = new DataResponse<EntityUsers>();
            try
            {
                base.DBInit();

                var model = new Database.UserProfile
                {
                    UserId = entity.UserId,
                    WorkEmail = entity.WorkEmail,
                    HomePhone = entity.HomePhone,
                    AdditionalPhone = entity.AdditionalPhone,
                    AddressLine1 = entity.AddressLine1,
                    AddressLine2 = entity.AddressLine2,
                    City = entity.City,
                    Zip = entity.Zip,
                    StateId = entity.StateId,
                    CreatedBy = entity.CreatedBy,
                    CreatedOn = entity.CreatedOn,
                    UpdatedBy = entity.UpdatedBy,
                    UpdatedOn = entity.UpdatedOn,
                    Startdate = entity.Startdate
                };

                if (base.DBSave(model) > 0)
                {
                    if (entity.DepartmentIds != null && entity.DepartmentIds.Count() > 0)
                    {
                        foreach (var item in entity.DepartmentIds)
                        {
                            DBEntity.UserDepartments.Add(new UserDepartment { UserId = entity.UserId.Value, DepartmentId = item, CreatedBy = entity.CreatedBy, CreatedOn = entity.CreatedOn });
                            DBEntity.SaveChanges();
                        }
                    }
                    return GetUserById(model.UserId.Value);
                }
                else
                {
                    response.CreateResponse(DataResponseStatus.InternalServerError);
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

        public DataResponse<bool> Delete(int userid)
        {
            var response = new DataResponse<bool>();
            try
            {
                base.DBInit();
                User user = DBEntity.Users.Find(userid);
                try
                {
                    DBEntity.Users.Remove(user);
                    if (DBEntity.SaveChanges() > 0)
                    {
                        response.Status = DataResponseStatus.OK;
                        response.Message = "Successfully Deleted.";
                        response.Model = true;
                    }
                }
                catch (DbUpdateException ex)
                {
                    response.Status = DataResponseStatus.InternalServerError;
                    response.Message = "There are some releted item in database, please delete those first.";
                    response.Model = false;
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

        public bool IsUserNameExists(string username)
        {
            try
            {
                base.DBInit();

                bool isExists = DBEntity.Users.Any(a => a.UserName == username);

                return isExists;
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }

            return false;
        }
    }
}
