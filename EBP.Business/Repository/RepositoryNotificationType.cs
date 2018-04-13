using EBP.Business.Entity;
using EBP.Business.Entity.Notification;
using EBP.Business.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;

namespace EBP.Business.Repository
{
    public class RepositoryNotificationType : _Repository
    {
        public DataResponse<EntityList<EntityNotificationType>> GetAllNotificationType(FilterNotificationType filter, int take = 10, int skip = 0)
        {
            var response = new DataResponse<EntityList<EntityNotificationType>>();
            try
            {
                base.DBInit();

                if (filter != null)
                {
                    take = filter.Take;
                    skip = filter.Skip;
                }
                var query = DBEntity.LookupNotificationTypes.Where(a => 1 == 1);
                if (!string.IsNullOrEmpty(filter.KeyWords))
                {
                    query = query.Where(ua => ua.Title.ToLower().Contains(filter.KeyWords.ToLower()) || ua.NotificationKey.ToLower().Contains(filter.KeyWords.ToLower()));
                }

                var selectQuery = query.Select(a => new EntityNotificationType
                {
                    Id = a.Id,
                    Title = a.Title,
                    NotificationKey = a.NotificationKey,
                    CreatedUser = a.User.FirstName,
                    CreatedOn = a.CreatedOn,
                    Updateduser = a.User1 == null ? null : a.User1.FirstName,
                    UpdatedOn = a.UpdatedOn

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

                response = GetList<EntityNotificationType>(selectQuery, skip, take);
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

        public DataResponse<EntityNotificationType> Insert(EntityNotificationType entity)
        {
            var response = new DataResponse<EntityNotificationType>();
            try
            {
                base.DBInit();

                var model = new Database.LookupNotificationType
                {
                    Title = entity.Title,
                    NotificationKey = entity.NotificationKey,
                    CreatedBy = entity.CreatedBy,
                    CreatedOn = System.DateTime.UtcNow
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

        public DataResponse<EntityNotificationType> GetNoificationTypeById(int id)
        {
            var response = new DataResponse<EntityNotificationType>();
            try
            {
                base.DBInit();

                var query = DBEntity.LookupNotificationTypes.Where(a => a.Id == id).Select(a => new EntityNotificationType
                {
                    Id = a.Id,
                    Title = a.Title,
                    NotificationKey = a.NotificationKey,
                    CreatedUser = a.User.FirstName,
                    CreatedOn = a.CreatedOn,
                    Updateduser = a.User1 == null ? null : a.User1.FirstName,
                    UpdatedOn = a.UpdatedOn
                });
                response = GetFirst<EntityNotificationType>(query);
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

        public DataResponse<EntityNotificationType> UpdateNoificationTypeById(EntityNotificationType entity)
        {
            var response = new DataResponse<EntityNotificationType>();
            try
            {
                base.DBInit();

                var model = DBEntity.LookupNotificationTypes.FirstOrDefault(a => a.Id == entity.Id);
                model.Title = entity.Title;
                model.NotificationKey = entity.NotificationKey;
                model.UpdatedOn = entity.UpdatedOn;
                model.UpdatedBy = entity.UpdatedBy;

                if (base.DBSaveUpdate(model) > 0)
                {
                    return GetNoificationTypeById(model.Id);
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

        public DataResponse<bool> DeleteNotificationType(int id)
        {
            var response = new DataResponse<bool>();
            try
            {
                base.DBInit();
                var model = DBEntity.LookupNotificationTypes.Find(id);
                DBEntity.LookupNotificationTypes.Remove(model);
                if (DBEntity.SaveChanges() > 0)
                {
                    response.Message = "Success";
                    response.Status = DataResponseStatus.OK;
                    response.Model = true;
                }
            }
            catch (Exception ex)
            {
                ex.Log();
                response.Message = "Error, There are some related item in database, please delete those first";
                response.Model = false;
            }
            finally
            {
                base.DBClose();
            }
            return response;
        }
    }
}
