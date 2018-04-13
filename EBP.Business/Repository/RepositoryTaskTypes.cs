using EBP.Business.Database;
using EBP.Business.Entity;
using EBP.Business.Entity.TaskType;
using EBP.Business.Filter;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;

namespace EBP.Business.Repository
{
    public class RepositoryTaskTypes : _Repository
    {
        public DataResponse<EntityList<EntityTaskTypes>> GetTaskTypes(FilterTaskTypes filter, int currentBusineId, int take = 10, int skip = 0)
        {
            var response = new DataResponse<EntityList<EntityTaskTypes>>();
            try
            {
                if (filter != null)
                {
                    take = filter.Take;
                    skip = filter.Skip;
                }
                base.DBInit();
                var query = DBEntity.LookupTaskTypes.Where(a => a.BusinessId == currentBusineId);
                if (filter != null)
                {
                    if (!String.IsNullOrEmpty(filter.KeyWords))
                    {
                        query = query.Where(a => a.TaskType.ToLower().Contains(filter.KeyWords));
                    }
                }

                var selectQuery = query.Select(a => new EntityTaskTypes
                {
                    Id = a.Id,
                    TaskType = a.TaskType,
                    CreatedBy = a.CreatedBy,
                    CreatedOn = a.CreatedOn,
                    UpdatedBy = a.UpdatedBy,
                    UpdatedOn = a.UpdatedOn,
                    BusinessId = a.BusinessId,
                    CreatedByName = a.User.FirstName + " " + a.User.LastName,
                    UpdatedByName = a.User1.FirstName + " " + a.User1.LastName,
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
                response = GetList<EntityTaskTypes>(selectQuery, skip, take);
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

        public DataResponse<EntityTaskTypes> GetTaskTypeById(int TaskTypesId)
        {
            var response = new DataResponse<EntityTaskTypes>();
            try
            {
                base.DBInit();

                var query = DBEntity.LookupTaskTypes.Where(a => a.Id == TaskTypesId).Select(a => new EntityTaskTypes
                {
                    Id = a.Id,
                    TaskType = a.TaskType,
                    CreatedBy = a.CreatedBy,
                    CreatedOn = a.CreatedOn,
                    UpdatedOn = a.UpdatedOn,
                    UpdatedBy = a.UpdatedBy,
                    BusinessId = a.BusinessId,
                    CreatedByName = a.User.FirstName + " " + a.User.LastName,
                    UpdatedByName = a.User1.FirstName + " " + a.User1.LastName,
                });

                response = GetFirst<EntityTaskTypes>(query);
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

        public DataResponse<EntityTaskTypes> Update(EntityTaskTypes entity)
        {
            var response = new DataResponse<EntityTaskTypes>();
            try
            {
                base.DBInit();

                var model = DBEntity.LookupTaskTypes.FirstOrDefault(a => a.Id == entity.Id);
                model.TaskType = entity.TaskType;
                model.UpdatedBy = entity.UpdatedBy;
                model.UpdatedOn = entity.UpdatedOn;

                if (base.DBSaveUpdate(model) > 0)
                {
                    return GetTaskTypeById(model.Id);
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

        public DataResponse<EntityTaskTypes> Insert(EntityTaskTypes entity)
        {
            var response = new DataResponse<EntityTaskTypes>();
            try
            {
                base.DBInit();

                var model = new Database.LookupTaskType
                {
                    TaskType = entity.TaskType,
                    BusinessId = entity.BusinessId,
                    CreatedOn = entity.CreatedOn,
                    CreatedBy = entity.CreatedBy

                };
                if (base.DBSave(model) > 0)
                {
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

        public DataResponse<bool> Delete(int tasktypeid)
        {
            var response = new DataResponse<bool>();
            try
            {
                base.DBInit();
                LookupTaskType lookupTaskType = DBEntity.LookupTaskTypes.Find(tasktypeid);
                try
                {
                    DBEntity.LookupTaskTypes.Remove(lookupTaskType);
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
