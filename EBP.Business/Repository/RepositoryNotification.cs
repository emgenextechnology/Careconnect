using EBP.Business.Entity;
using EBP.Business.Entity.EntityNotificationSettings;
using EBP.Business.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBP.Business.Repository
{
    public class RepositoryNotification : _Repository
    {

        public void Save(List<EntityNotification> notificationlist, int CurrentUserId, int targetId, int targetType, int notificationType, int createdBy, string createdByName, DateTime createdOn, string subject = null)
        {
            try
            {
                base.DBInit();
                if (notificationlist.Count > 0)
                {
                    foreach (var item in notificationlist)
                    {
                        if (item.UserId == CurrentUserId)
                            continue;
                        DBEntity.Notifications.Add(new Database.Notification
                        {
                            Message = string.Format(item.Message, createdByName, subject),
                            TargetId = targetId,
                            TargetType = targetType,
                            NotificationType = notificationType,
                            NotificationUsers = new List<Database.NotificationUser>
                            {
                                new Database.NotificationUser{
                                    UserId = item.UserId
                                }
                            },
                            CreatedUserName = createdByName,
                            CreatedOn = createdOn,
                            CreatedBy = createdBy
                        });

                        var objUserProfile = DBEntity.UserProfiles.FirstOrDefault(a => a.UserId == item.UserId);
                        if (objUserProfile != null)
                        {
                            objUserProfile.NotificationCount = objUserProfile.NotificationCount.HasValue ?
                                objUserProfile.NotificationCount.Value + 1 : 1;
                        }
                    }

                    DBEntity.SaveChanges();
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
        }

        public DataResponse<Notification> GetAllNotifications(int userId, int? businessId, int skip, int take)
        {
            var response = new DataResponse<Notification>();
            try
            {
                base.DBInit();

                var query = DBEntity.Notifications.Where(a => a.NotificationUsers.Any(b => b.UserId == userId) && a.NotificationType == (int)NotificationType.Normal);

                var selectQuery = query.Select(a => new EntityNotification
                    {
                        BusinessId = businessId,
                        NotificationId = a.Id,
                        CreatedUserName = a.CreatedUserName,
                        UserId = a.CreatedBy,
                        target = a.TargetType,
                        Message = a.Message,
                        TargetId = a.TargetId,
                        IsRead = a.NotificationUsers.FirstOrDefault().IsRead,
                        CreatedOn = a.CreatedOn,
                        CreatedBy = a.CreatedBy
                    }).OrderByDescending(o => o.CreatedOn);

                var dataResponse = GetList<EntityNotification>(selectQuery, skip, take);

                response.Model = new Notification();
                response.Model.NotificationList = dataResponse.Model;
                response.Message = dataResponse.Message;
                response.Status = dataResponse.Status;
                response.Model.UnreadCount = query.Count(b => b.NotificationUsers.Any(n => n.IsRead != true));

                var objUserProfile = DBEntity.UserProfiles.FirstOrDefault(a => a.UserId == userId);
                if (skip == 0)
                {
                    objUserProfile.LastNotifiedOn = DateTime.UtcNow;
                }
                base.DBSaveUpdate(objUserProfile);
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

        public DataResponse<EntityList<EntityNotificationSettings>> GetAllNotificationSettings(int userId)
        {
            var response = new DataResponse<EntityList<EntityNotificationSettings>>();
            try
            {
                base.DBInit();

                var query = from p in DBEntity.LookupNotificationTypes
                            join m in DBEntity.UserNotificationMappers.Where(e => e.UserId == userId) on p.Id equals m.NotificationTypeId
                                      into joined
                            from j in joined.DefaultIfEmpty()
                            select new EntityNotificationSettings
                                      {
                                          NotificationTypeId = p.Id,
                                          NotificationType = p.Title,
                                          Status = j.Status == null ? true : j.Status
                                      };

                response = GetList<EntityNotificationSettings>(query.OrderByDescending(o => o.NotificationTypeId), 0, 50);
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

        public DataResponse<EntityNotificationSettings> SaveNotification(EntityNotificationSettings entity)
        {
            DataResponse<EntityNotificationSettings> response = new DataResponse<EntityNotificationSettings>();
            try
            {
                base.DBInit();
                var model = DBEntity.UserNotificationMappers.FirstOrDefault(a => a.NotificationTypeId == entity.NotificationTypeId & a.UserId == entity.UserId);
                if (model != null)
                {
                    if (entity.Status != null)
                    {
                        model.Status = entity.Status;
                        model.UpdatedBy = entity.UserId;
                        model.UpdatedOn = DateTime.UtcNow;
                    }
                    base.DBSaveUpdate(model);
                }
                else
                {
                    var data = DBEntity.UserNotificationMappers.Add(new Database.UserNotificationMapper
                       {
                           UserId = entity.UserId,
                           NotificationTypeId = entity.NotificationTypeId,
                           Status = entity.Status,
                           CreatedBy = entity.UserId,
                           CreatedOn = DateTime.UtcNow
                       });
                    base.DBSave(data);
                }
            }
            catch (Exception ex)
            {
                response.ThrowError(ex);
            }

            finally
            {
                base.DBClose();
            }
            return response;
        }

        public DataResponse UpdateNotification(long id, long userId)
        {
            var response = new DataResponse();
            try
            {
                DBInit();
                var model = DBEntity.NotificationUsers.Where(t => t.NotificationId == id).FirstOrDefault();
                model.IsRead = true;
                var rowCount = DBSaveUpdate(model);
                if (rowCount > 0)
                {
                    response.CreateResponse(DataResponseStatus.OK);
                }
                else
                {
                    response.CreateResponse(DataResponseStatus.NotModified);
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

        public DataResponse<UnReadNotification> GetUnReadNotificationCount(int userId)
        {
            var response = new DataResponse<UnReadNotification>();
            try
            {
                base.DBInit();
                var userProfile = DBEntity.UserProfiles.Where(a => a.UserId == userId).FirstOrDefault();

                var notifyCount = DBEntity.NotificationUsers.Count(a => a.UserId == userId && (a.Notification.CreatedOn >= userProfile.LastNotifiedOn || userProfile.LastNotifiedOn == null));
                //if (notifyCount == 0)
                //{
                //    notifyCount = DBEntity.NotificationUsers.Count(a => a.UserId == userId);
                //}
                //var notifyCount = DBEntity.Notifications.Where(a => a.CreatedOn >= getNotificationStatus.LastNotifiedOn && a.NotificationUsers.Any(b => b.UserId == userId)).Count();

                UnReadNotification model = new UnReadNotification();
                model.NotificationCount = notifyCount;
                response.Model = model;
                response.Status = DataResponseStatus.OK;
                response.Message = "Success";
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
