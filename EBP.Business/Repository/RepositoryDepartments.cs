using EBP.Business.Database;
using EBP.Business.Entity;
using EBP.Business.Entity.Departments;
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
    public class RepositoryDepartments : _Repository
    {
        public DataResponse<EntityList<EntityDepartments>> GetDepartments(FilterDepartments filter, int? currentBusineId, int take = 10, int skip = 0)
        {
            var response = new DataResponse<EntityList<EntityDepartments>>();
            try
            {
                if (filter != null)
                {
                    take = filter.Take;
                    skip = filter.Skip;
                }
                base.DBInit();
                var query = DBEntity.Departments.Where(a => a.BusinessId == currentBusineId);
                if (filter != null)
                {
                    if (!String.IsNullOrEmpty(filter.KeyWords))
                    {
                        query = query.Where(a => a.DepartmentName.ToLower().Contains(filter.KeyWords));
                    }
                }
                var selectQuery = query.Select(a => new EntityDepartments
                {
                    Id = a.Id,
                    DepartmentName = a.DepartmentName,
                    Description = a.Description,
                    StatusId = a.StatusId,
                    CreatedBy = a.CreatedBy,
                    CreatedOn = a.CreatedOn,
                    UpdatedBy = a.UpdatedBy,
                    UpdatedOn = a.UpdatedOn,
                    BusinessId = a.BusinessId,
                    IsActive = a.IsActive,
                    OldId = a.OldId,
                    CreatedByName = a.User.FirstName + " " + a.User.LastName,
                    UpdatedByName = a.User1.FirstName + " " + a.User1.LastName,
                    DepartmentPrivilegeIds = a.DepartmentPrivileges.Select(b => b.PrivilegeId).ToList()
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

                response = GetList<EntityDepartments>(selectQuery, skip, take);
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

        public DataResponse<EntityDepartments> GetDepartmentById(int DepartmentId)
        {
            var response = new DataResponse<EntityDepartments>();
            try
            {
                base.DBInit();

                var query = DBEntity.Departments.Where(a => a.Id == DepartmentId).Select(a => new EntityDepartments
                {
                    Id = a.Id,
                    DepartmentName = a.DepartmentName,
                    Description = a.Description,
                    StatusId = a.StatusId,
                    CreatedBy = a.CreatedBy,
                    CreatedOn = a.CreatedOn,
                    UpdatedBy = a.UpdatedBy,
                    UpdatedOn = a.UpdatedOn,
                    BusinessId = a.BusinessId,
                    IsActive = a.IsActive,
                    OldId = a.OldId,
                    CreatedByName = a.User.FirstName + " " + a.User.LastName,
                    UpdatedByName = a.User1.FirstName + " " + a.User1.LastName,
                    DepartmentPrivilegeIds = a.DepartmentPrivileges.Select(b => b.PrivilegeId).ToList(),
                    Users = a.UserDepartments.Select(b => b.User2.FirstName + " " + b.User2.LastName).ToList()
                });
                response = GetFirst<EntityDepartments>(query);
                response.Model.DepartmentPrivilege = DBEntity.Privileges.GroupBy(a => a.ModuleId).Select(b => new EntityModules
                {
                    ModuleName = b.FirstOrDefault(a => a.ModulesMaster.Id == a.ModuleId).ModulesMaster.Title,
                    Privileges = b.Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Selected = response.Model.DepartmentPrivilegeIds.Contains(c.Id),
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

        public DataResponse<EntityDepartments> Update(EntityDepartments entity)
        {
            var response = new DataResponse<EntityDepartments>();
            try
            {
                base.DBInit();

                #region Prepare model
                var model = DBEntity.Departments.FirstOrDefault(a => a.Id == entity.Id);
                model.DepartmentName = entity.DepartmentName;
                model.Description = entity.Description;
                model.IsActive = entity.IsActive;
                model.UpdatedOn = entity.UpdatedOn;
                model.UpdatedBy = entity.UpdatedBy;
                #endregion

                if (base.DBSaveUpdate(model) > 0)
                {
                    IEnumerable<DepartmentPrivilege> departmentPrivilege = DBEntity.DepartmentPrivileges.Where(a => a.DepartmentId == model.Id).ToList();
                    if (departmentPrivilege.Count() > 0)
                    {
                        DBEntity.DepartmentPrivileges.RemoveRange(departmentPrivilege);
                        DBEntity.SaveChanges();
                    }
                    if (entity.DepartmentPrivilegeIds != null && entity.DepartmentPrivilegeIds.Count() > 0)
                    {
                        foreach (var item in entity.DepartmentPrivilegeIds)
                        {
                            DBEntity.DepartmentPrivileges.Add(new DepartmentPrivilege
                            {
                                DepartmentId = model.Id,
                                PrivilegeId = item,
                                CreatedBy = model.CreatedBy,
                                CreatedOn = model.CreatedOn,
                                UpdatedBy = entity.UpdatedBy,
                                UpdatedOn = entity.UpdatedOn
                            });
                            DBEntity.SaveChanges();
                        }
                    }
                    return GetDepartmentById(model.Id);
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

        public DataResponse<EntityDepartments> Insert(EntityDepartments entity)
        {
            var response = new DataResponse<EntityDepartments>();
            try
            {
                base.DBInit();

                var model = new Database.Department
                {
                    DepartmentName = entity.DepartmentName,
                    Description = entity.Description,
                    BusinessId = entity.BusinessId,
                    StatusId = 1,
                    IsActive = true,
                    CreatedOn = entity.CreatedOn,
                    CreatedBy = entity.CreatedBy

                };
                if (base.DBSave(model) > 0)
                {
                    if (entity.DepartmentPrivilegeIds != null && entity.DepartmentPrivilegeIds.Count() > 0)
                    {
                        foreach (var item in entity.DepartmentPrivilegeIds)
                        {
                            DBEntity.DepartmentPrivileges.Add(new DepartmentPrivilege { DepartmentId = model.Id, PrivilegeId = item, CreatedBy = entity.CreatedBy, CreatedOn = entity.CreatedOn });
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

        public DataResponse<bool> Delete(int Id)
        {
            var response = new DataResponse<bool>();
            try
            {
                base.DBInit();
                Department department = DBEntity.Departments.Find(Id);
                try
                {
                    DBEntity.Departments.Remove(department);
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
