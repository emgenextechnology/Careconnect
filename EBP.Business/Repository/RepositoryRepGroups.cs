using EBP.Business.Database;
using EBP.Business.Entity;
using EBP.Business.Entity.RepGroups;
using EBP.Business.Filter;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;
using EBP.Business.Enums;

namespace EBP.Business.Repository
{
    public class RepositoryRepGroups : _Repository
    {
        public DataResponse<EntityList<EntityRepGroups>> GetRepGroups(FilterRepGroups filter, int currentBusineId, int take = 10, int skip = 0)
        {
            var response = new DataResponse<EntityList<EntityRepGroups>>();
            try
            {
                if (filter != null)
                {
                    take = filter.Take;
                    skip = filter.Skip;
                }
                base.DBInit();
                var query = DBEntity.RepGroups.Where(a => a.BusinessId == currentBusineId);
                if (filter != null)
                {
                    if (!String.IsNullOrEmpty(filter.KeyWords))
                    {
                        query = query.Where(a => a.RepGroupName.ToLower().Contains(filter.KeyWords)
                            || a.RepgroupManagerMappers.Any(b => b.User.FirstName.ToLower().Contains(filter.KeyWords.ToLower()))
                            || a.RepgroupManagerMappers.Any(b => b.User.Email.ToLower().Contains(filter.KeyWords.ToLower()))
                            || a.Reps.Any(b => b.User2.Email.ToLower().Contains(filter.KeyWords.ToLower()))
                            || a.RepgroupManagerMappers.Any(b => b.User.Email.ToLower().Contains(filter.KeyWords.ToLower()))
                            || a.BusinessMaster.BusinessName.ToLower().Contains(filter.KeyWords.ToLower()));
                    }
                }
                var selectQuery = query.Select(a => new EntityRepGroups
                {
                    Id = a.Id,
                    RepGroupName = a.RepGroupName,
                    Description = a.Description,
                    CreatedBy = a.CreatedBy,
                    CreatedOn = a.CreatedOn,
                    UpdatedBy = a.UpdatedBy,
                    UpdatedOn = a.UpdatedOn,
                    BusinessId = a.BusinessId,
                    IsActive = a.IsActive,
                    OldId = a.OldId,
                    //SalesDirectorId = a.SalesDirectorId,
                    CreatedByName = a.User.FirstName + " " + a.User.LastName,
                    UpdatedByName = a.User1.FirstName + " " + a.User1.LastName,
                    SalesDirectors = a.RepgroupManagerMappers.Where(b => b.UserRole == (int)RepgroupUserType.Director).Select(b => b.User.FirstName + " " + a.User.LastName),
                    RepGroupManagerIds = a.RepgroupManagerMappers.Select(b => b.ManagerId).ToList(),
                    Managers = a.RepgroupManagerMappers.Where(b => b.User.IsActive == true).Select(b => b.User.FirstName + " " + b.User.LastName)
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

                response = GetList<EntityRepGroups>(selectQuery, skip, take);
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

        public DataResponse<EntityRepGroups> GetRepGroupById(int RepGroupId)
        {
            var response = new DataResponse<EntityRepGroups>();
            try
            {
                base.DBInit();

                var query = DBEntity.RepGroups.Where(a => a.Id == RepGroupId).Select(a => new EntityRepGroups
                {
                    Id = a.Id,
                    RepGroupName = a.RepGroupName,
                    Description = a.Description,
                    CreatedBy = a.CreatedBy,
                    CreatedOn = a.CreatedOn,
                    UpdatedBy = a.UpdatedBy,
                    UpdatedOn = a.UpdatedOn,
                    BusinessId = a.BusinessId,
                    IsActive = a.IsActive,
                    OldId = a.OldId,
                    SalesDirectorIds = a.RepgroupManagerMappers.Where(b => b.UserRole == (int)RepgroupUserType.Director).Select(b => b.ManagerId).ToList(),
                    CreatedByName = a.User.FirstName + " " + a.User.LastName,
                    UpdatedByName = a.User1.FirstName + " " + a.User1.LastName,
                    SalesDirectors = a.RepgroupManagerMappers.Where(b => b.UserRole == (int)RepgroupUserType.Director).Select(b => b.User.FirstName + " " + b.User.LastName),
                    RepGroupManagerIds = a.RepgroupManagerMappers.Where(b => b.UserRole == (int)RepgroupUserType.Manager).Select(b => b.ManagerId).ToList(),
                    Managers = a.RepgroupManagerMappers.Select(b => b.User.FirstName + " " + b.User.LastName),
                    SalesReps = a.Reps.Where(r => r.User2.IsActive == true).Select(p => p.User2.FirstName + " " + p.User2.LastName)
                });
                response = GetFirst<EntityRepGroups>(query);

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

        public DataResponse<EntityRepGroups> Update(EntityRepGroups entity)
        {
            var response = new DataResponse<EntityRepGroups>();
            try
            {
                base.DBInit();

                entity.UpdatedOn = DateTime.UtcNow;

                #region Prepare model
                var model = DBEntity.RepGroups.FirstOrDefault(a => a.Id == entity.Id);
                model.RepGroupName = entity.RepGroupName;
                model.Description = entity.Description;
                model.IsActive = entity.IsActive;
                model.UpdatedOn = entity.UpdatedOn;
                model.UpdatedBy = entity.UpdatedBy;
                //model.SalesDirectorId = entity.SalesDirectorId > 0 ? entity.SalesDirectorId : null;
                #endregion

                if (base.DBSaveUpdate(model) > 0)
                {
                    IEnumerable<RepgroupManagerMapper> RepGroupManagerIds = DBEntity.RepgroupManagerMappers.Where(a => a.RepGroupId == model.Id && a.UserRole == (int)RepgroupUserType.Manager).ToList();
                    if (RepGroupManagerIds.Count() > 0)
                    {
                        DBEntity.RepgroupManagerMappers.RemoveRange(RepGroupManagerIds);
                        DBEntity.SaveChanges();
                    }
                    if (entity.RepGroupManagerIds != null && entity.RepGroupManagerIds.Count() > 0)
                    {
                        foreach (var item in entity.RepGroupManagerIds)
                        {
                            DBEntity.RepgroupManagerMappers.Add(new RepgroupManagerMapper { RepGroupId = model.Id, ManagerId = item, UserRole = (int)RepgroupUserType.Manager, CreatedBy = entity.CreatedBy, CreatedOn = entity.CreatedOn });
                            DBEntity.SaveChanges();
                        }
                    }

                    IEnumerable<RepgroupManagerMapper> RepGroupDirectorsIds = DBEntity.RepgroupManagerMappers.Where(a => a.RepGroupId == model.Id && a.UserRole == (int)RepgroupUserType.Director).ToList();
                    if (RepGroupDirectorsIds.Count() > 0)
                    {
                        DBEntity.RepgroupManagerMappers.RemoveRange(RepGroupDirectorsIds);
                        DBEntity.SaveChanges();
                    }

                    if (entity.SalesDirectorIds != null && entity.SalesDirectorIds.Count() > 0)
                    {
                        foreach (var item in entity.SalesDirectorIds)
                        {
                            DBEntity.RepgroupManagerMappers.Add(new RepgroupManagerMapper { RepGroupId = model.Id, ManagerId = item, UserRole = (int)RepgroupUserType.Director, CreatedBy = entity.CreatedBy, CreatedOn = entity.CreatedOn });
                            DBEntity.SaveChanges();
                        }
                    }

                    return GetRepGroupById(model.Id);
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

        public DataResponse<EntityRepGroups> Insert(EntityRepGroups entity)
        {
            var response = new DataResponse<EntityRepGroups>();
            try
            {
                base.DBInit();

                var model = new Database.RepGroup
                {
                    BusinessId = entity.BusinessId,
                    RepGroupName = entity.RepGroupName,
                    Description = entity.Description,
                    CreatedBy = entity.CreatedBy,
                    CreatedOn = entity.CreatedOn,
                    IsActive = true,
                    //SalesDirectorId = entity.SalesDirectorId,
                };

                if (base.DBSave(model) > 0)
                {
                    if (entity.SalesDirectorIds != null && entity.SalesDirectorIds.Count() > 0)
                    {
                        foreach (var item in entity.SalesDirectorIds)
                        {
                            DBEntity.RepgroupManagerMappers.Add(new RepgroupManagerMapper { RepGroupId = model.Id, ManagerId = item, UserRole = (int)RepgroupUserType.Director, CreatedBy = entity.CreatedBy, CreatedOn = entity.CreatedOn });
                            DBEntity.SaveChanges();
                        }
                    }
                    if (entity.RepGroupManagerIds != null && entity.RepGroupManagerIds.Count() > 0)
                    {
                        foreach (var item in entity.RepGroupManagerIds)
                        {
                            DBEntity.RepgroupManagerMappers.Add(new RepgroupManagerMapper { RepGroupId = model.Id, ManagerId = item, UserRole = (int)RepgroupUserType.Manager, CreatedBy = entity.CreatedBy, CreatedOn = entity.CreatedOn });
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

        public DataResponse<bool> Delete(int repgroupid)
        {
            var response = new DataResponse<bool>();
            try
            {
                base.DBInit();
                RepGroup repGroup = DBEntity.RepGroups.Find(repgroupid);
                try
                {
                    DBEntity.RepGroups.Remove(repGroup);
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
