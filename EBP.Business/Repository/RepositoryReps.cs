using EBP.Business.Database;
using EBP.Business.Entity;
using EBP.Business.Entity.Rep;
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
    public class RepositoryReps : _Repository
    {
        public DataResponse<EntityList<EntityReps>> GetReps(FilterReps filter, int currentBusineId, int take = 10, int skip = 0)
        {
            var response = new DataResponse<EntityList<EntityReps>>();
            try
            {
                if (filter != null)
                {
                    take = filter.Take;
                    skip = filter.Skip;
                }

                base.DBInit();

                var query = DBEntity.Reps.Where(a => a.User.BusinessId == currentBusineId);
                if (filter != null)
                {
                    if (!String.IsNullOrEmpty(filter.KeyWords))
                    {
                        query = query.Where(ua => ua.User2.FirstName.ToLower().Contains(filter.KeyWords.ToLower())
                    || ua.User2.LastName.ToLower().Contains(filter.KeyWords.ToLower())
                    || (ua.User2.FirstName + " " + ua.User2.MiddleName).ToLower().Contains(filter.KeyWords.ToLower())
                    || (ua.User2.FirstName + " " + ua.User2.LastName).ToLower().Contains(filter.KeyWords.ToLower())
                    || (ua.User2.FirstName + " " + ua.User2.MiddleName + " " + ua.User2.LastName).ToLower().Contains(filter.KeyWords.ToLower())
                    || ua.User2.UserName.ToLower().Contains(filter.KeyWords.ToLower())
                    || ua.RepGroup.RepGroupName.ToLower().Contains(filter.KeyWords.ToLower()));
                    }
                }

                var selectQuery = query.Select(a => new EntityReps
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    RepName = a.User2.FirstName + " " + a.User2.LastName,
                    RepGroupName = a.RepGroup.RepGroupName,
                    RepGroupId = a.RepGroupId,
                    CreatedBy = a.CreatedBy,
                    CreatedOn = a.CreatedOn,
                    UpdatedBy = a.UpdatedBy,
                    UpdatedOn = a.UpdatedOn,
                    SignonDate = a.SignonDate,
                    IsActive = a.IsActive,
                    OldId = a.OldId,
                    CreatedByName = a.User.FirstName + " " + a.User.LastName,
                    UpdatedByName = a.User1.FirstName + " " + a.User1.LastName,
                    ServiceIds = a.RepServiceMappers.Select(b => b.ServiceId).ToList(),
                    RepGroupManagerNames = a.RepGroup.RepgroupManagerMappers.Select(b => b.User.FirstName + "" + b.User.LastName),
                    ServiceNames = a.RepServiceMappers.Where(b => b.RepId == a.Id).Select(p => p.LookupEnrolledService.ServiceName),
                    DirectorNames = a.RepGroup.RepgroupManagerMappers.Select(b => b.User.LastName)
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

                response = GetList<EntityReps>(selectQuery, skip, take);
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

        public DataResponse<EntityReps> GetRepById(int RepId)
        {
            var response = new DataResponse<EntityReps>();
            try
            {
                base.DBInit();

                var query = DBEntity.Reps.Where(a => a.Id == RepId).Select(a => new EntityReps
                {
                    Id = a.Id,
                    RepName = a.User2.FirstName + " " + a.User2.LastName,
                    RepGroupName = a.RepGroup.RepGroupName,
                    UserId = a.UserId,
                    RepGroupId = a.RepGroupId,
                    CreatedBy = a.CreatedBy,
                    CreatedOn = a.CreatedOn,
                    UpdatedBy = a.UpdatedBy,
                    UpdatedOn = a.UpdatedOn,
                    SignonDate = a.SignonDate,
                    IsActive = a.IsActive,
                    OldId = a.OldId,
                    CreatedByName = a.User.FirstName + " " + a.User.LastName,
                    UpdatedByName = a.User1.FirstName + " " + a.User1.LastName,
                    ServiceIds = a.RepServiceMappers.Select(b => b.ServiceId).ToList()
                });

                response = GetFirst<EntityReps>(query);
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

        public DataResponse<EntityReps> Update(EntityReps entity)
        {
            var response = new DataResponse<EntityReps>();
            try
            {
                base.DBInit();

                entity.UpdatedOn = DateTime.UtcNow;

                var model = DBEntity.Reps.FirstOrDefault(a => a.Id == entity.Id);
                model.UserId = entity.UserId;
                model.RepGroupId = entity.RepGroupId;
                model.IsActive = entity.IsActive;
                model.UpdatedOn = entity.UpdatedOn;
                model.UpdatedBy = entity.UpdatedBy;

                if (base.DBSaveUpdate(model) > 0)
                {
                    IEnumerable<RepServiceMapper> RepManagerIds = DBEntity.RepServiceMappers.Where(a => a.RepId == model.Id).ToList();
                    if (RepManagerIds.Count() > 0)
                    {
                        DBEntity.RepServiceMappers.RemoveRange(RepManagerIds);
                        DBEntity.SaveChanges();
                    }
                    if (entity.ServiceIds != null && entity.ServiceIds.Count() > 0)
                    {
                        foreach (var item in entity.ServiceIds)
                        {
                            DBEntity.RepServiceMappers.Add(new RepServiceMapper { RepId = entity.Id, ServiceId = item, CreatedBy = model.CreatedBy, CreatedOn = model.CreatedOn });
                            DBEntity.SaveChanges();
                        }
                    }
                    return GetRepById(model.Id);
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

        public DataResponse<EntityReps> Insert(EntityReps entity)
        {
            var response = new DataResponse<EntityReps>();
            try
            {
                base.DBInit();

                var model = new Database.Rep
                {
                    UserId = entity.UserId,
                    RepGroupId = entity.RepGroupId,
                    CreatedBy = entity.CreatedBy,
                    CreatedOn = entity.CreatedOn,
                    IsActive = true,
                };
                if (base.DBSave(model) > 0)
                {
                    if (entity.ServiceIds != null && entity.ServiceIds.Count() > 0)
                    {
                        foreach (var item in entity.ServiceIds)
                        {
                            DBEntity.RepServiceMappers.Add(new RepServiceMapper { ServiceId = item, RepId = model.Id, CreatedBy = entity.CreatedBy, CreatedOn = entity.CreatedOn });
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

        public DataResponse<bool> Delete(int repid)
        {
            var response = new DataResponse<bool>();
            try
            {
                base.DBInit();
                Rep rep = DBEntity.Reps.Find(repid);
                try
                {
                    DBEntity.RepServiceMappers.RemoveRange(DBEntity.RepServiceMappers.Where(u => u.RepId == repid));
                    DBEntity.SaveChanges();
                    DBEntity.Reps.Remove(rep);
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
