using EBP.Business.Database;
using EBP.Business.Entity;
using EBP.Business.Entity.Departments;
using EBP.Business.Entity.Roles;
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
    public class RepositoryRoles : _Repository
    {
        public DataResponse<EntityList<EntityRoles>> GetRoles(FilterRoles filter, int currentBusineId, int take = 10, int skip = 0)
        {
            var response = new DataResponse<EntityList<EntityRoles>>();
            try
            {
                if (filter != null)
                {
                    take = filter.Take;
                    skip = filter.Skip;
                }
                base.DBInit();
                var query = DBEntity.Roles.Where(a => a.BusinessId == currentBusineId);
                if (filter != null)
                {
                    if (!String.IsNullOrEmpty(filter.KeyWords))
                    {
                        query = query.Where(a => a.Name.ToLower().Contains(filter.KeyWords));
                    }
                }

                var selectQuery = query.Select(a => new EntityRoles
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    Status = a.Status,
                    CreatedBy = a.CreatedBy,
                    CreatedOn = a.CreatedOn,
                    BusinessId = a.BusinessId,
                    IsActive = a.IsActive,
                    OldId = a.OldId,
                });

                if (string.IsNullOrEmpty(filter.SortKey) || string.IsNullOrEmpty(filter.SortOrder))
                {
                    selectQuery = selectQuery.OrderByDescending(o => o.CreatedOn);
                }
                else
                {
                    string orderBy = string.Format("{0} {1}", filter.SortKey, filter.SortOrder);
                    selectQuery = selectQuery.OrderBy(orderBy);
                }
                response = GetList<EntityRoles>(selectQuery, skip, take);
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

        public DataResponse<EntityRoles> GetRolesById(int RolesId)
        {
            var response = new DataResponse<EntityRoles>();
            try
            {
                base.DBInit();

                var query = DBEntity.Roles.Where(a => a.Id == RolesId).Select(a => new EntityRoles
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    Status = a.Status,
                    CreatedBy = a.CreatedBy,
                    //CreatedUser = a.Users.FirstOrDefault().FirstName + " " + a.Users.FirstOrDefault().LastName,
                    CreatedOn = a.CreatedOn,
                    BusinessId = a.BusinessId,
                    IsActive = a.IsActive,
                    OldId = a.OldId,
                    RolePrivilegeIds = a.RolePrivileges.Select(b => b.PrivilegeId).ToList()
                });

                response = GetFirst<EntityRoles>(query);
                response.Model.RolePrivilege = DBEntity.Privileges.GroupBy(a => a.ModuleId).Select(b => new EntityModules
                {
                    ModuleName = b.FirstOrDefault(a => a.ModulesMaster.Id == a.ModuleId).ModulesMaster.Title,
                    Privileges = b.Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Selected = response.Model.RolePrivilegeIds.Contains(c.Id),
                        Text = c.Title
                    })
                }).OrderByDescending(a => a.ModuleName).ToList();
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

        public DataResponse<EntityRoles> Update(EntityRoles entity)
        {
            var response = new DataResponse<EntityRoles>();
            try
            {
                base.DBInit();

                var model = DBEntity.Roles.FirstOrDefault(a => a.Id == entity.Id);
                model.Name = entity.Name;
                model.Description = entity.Description;
                model.IsActive = entity.IsActive;

                if (base.DBSaveUpdate(model) > 0)
                {
                    IEnumerable<RolePrivilege> RolesPrivilege = DBEntity.RolePrivileges.Where(a => a.RoleId == model.Id).ToList();
                    if (RolesPrivilege.Count() > 0)
                    {
                        DBEntity.RolePrivileges.RemoveRange(RolesPrivilege);
                        DBEntity.SaveChanges();
                    }
                    if (entity.RolePrivilegeIds != null && entity.RolePrivilegeIds.Count() > 0)
                    {
                        foreach (var item in entity.RolePrivilegeIds)
                        {
                            DBEntity.RolePrivileges.Add(new RolePrivilege { RoleId = model.Id, PrivilegeId = item, CreatedBy = entity.CreatedBy.Value, CreatedOn = entity.CreatedOn });
                            DBEntity.SaveChanges();
                        }
                    }
                    return GetRolesById(model.Id);
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

        public DataResponse<EntityRoles> Insert(EntityRoles entity)
        {
            var response = new DataResponse<EntityRoles>();
            try
            {
                base.DBInit();

                var model = new Database.Role
                {
                    Name = entity.Name,
                    Description = entity.Description,
                    BusinessId = entity.BusinessId,
                    Status = 1,
                    IsActive = true,
                    CreatedOn = entity.CreatedOn,
                    CreatedBy = entity.CreatedBy

                };

                if (base.DBSave(model) > 0)
                {
                    if (entity.RolePrivilegeIds != null && entity.RolePrivilegeIds.Count() > 0)
                    {
                        foreach (var item in entity.RolePrivilegeIds)
                        {
                            DBEntity.RolePrivileges.Add(new RolePrivilege { RoleId = model.Id, PrivilegeId = item, CreatedBy = entity.CreatedBy.Value, CreatedOn = entity.CreatedOn });
                            DBEntity.SaveChanges();
                        }
                    }
                    entity.Id = model.Id;
                    response.CreateResponse(entity, DataResponseStatus.OK);
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

        public DataResponse<bool> Delete(int roleid)
        {
            var response = new DataResponse<bool>();
            try
            {
                base.DBInit();
                Role role = DBEntity.Roles.Find(roleid);
                try
                {
                    DBEntity.Roles.Remove(role);
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
    }
}
