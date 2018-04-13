using EBP.NotificationApp.Entity;
using EBP.NotificationApp.Entity._base;
using EBP.NotificationApp.Entity.Task;
using EBP.NotificationApp.Enums;
using EBP.NotificationApp.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBP.NotificationApp.Repository
{
    public class RepositoryNotification : _Repository
    {

        public void Save(List<EntityNotification> notificationlist, int targetId, int targetType, int notificationType, int createdBy, string createdByName, DateTime createdOn)
        {
            try
            {
                base.DBInit();
                if (notificationlist.Count > 0)
                {
                    foreach (var item in notificationlist)
                    {
                        DBEntity.Notifications.Add(new Database.Notification
                        {
                            Message = string.Format(item.Message, createdByName),
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

        public List<EntityNotificationUsers> GetAllUserNotifications()
        {
            var response = new List<EntityNotificationUsers>();
            try
            {
                base.DBInit();

                var query = DBEntity.NotificationUsers.Where(a => a.IsPushSent != true);

                response = query.Select(a => new EntityNotificationUsers
                {
                    Id = a.Id,
                    NotificationId = a.NotificationId,
                    IsPushSent = a.IsPushSent,
                    IsRead = a.IsRead,
                    IsSent = a.IsSent,
                    UserId = a.UserId
                }).OrderByDescending(o => o.Id).ToList();

                //  var dataResponse = GetList<EntityNotification>(selectQuery, skip, take);

                // response.Model = new List<Notification>();
                // response = selectQuery;
                // response.Message = dataResponse.Message;
                //  response.Status = dataResponse.Status;
                // response.Model.UnreadCount = query.Count(b => b.NotificationUsers.Any(n => n.IsRead != true));

                //var objUserProfile = DBEntity.UserProfiles.FirstOrDefault(a => a.UserId == userId);
                //if (skip == 0)
                //{
                //    objUserProfile.LastNotifiedOn = DateTime.UtcNow;
                //}
                //base.DBSaveUpdate(objUserProfile);
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

        //public DataResponse<EntityList<EntityNotificationSettings>> GetAllNotificationSettings(int userId)
        //{
        //    var response = new DataResponse<EntityList<EntityNotificationSettings>>();
        //    try
        //    {
        //        base.DBInit();

        //        var query = from p in DBEntity.LookupNotificationTypes
        //                    join m in DBEntity.UserNotificationMappers.Where(e => e.UserId == userId) on p.Id equals m.NotificationTypeId
        //                              into joined
        //                    from j in joined.DefaultIfEmpty()
        //                    select new EntityNotificationSettings
        //                    {
        //                        NotificationTypeId = p.Id,
        //                        NotificationType = p.Title,
        //                        Status = j.Status == null ? true : j.Status
        //                    };

        //        response = GetList<EntityNotificationSettings>(query.OrderByDescending(o => o.NotificationTypeId), 0, 50);
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

        //public DataResponse<EntityNotificationSettings> SaveNotification(EntityNotificationSettings entity)
        //{
        //    DataResponse<EntityNotificationSettings> response = new DataResponse<EntityNotificationSettings>();
        //    try
        //    {
        //        base.DBInit();
        //        var model = DBEntity.UserNotificationMappers.FirstOrDefault(a => a.NotificationTypeId == entity.NotificationTypeId & a.UserId == entity.UserId);
        //        if (model != null)
        //        {
        //            if (entity.Status != null)
        //            {
        //                model.Status = entity.Status;
        //                model.UpdatedBy = entity.UserId;
        //                model.UpdatedOn = DateTime.UtcNow;
        //            }
        //            base.DBSaveUpdate(model);
        //        }
        //        else
        //        {
        //            var data = DBEntity.UserNotificationMappers.Add(new Database.UserNotificationMapper
        //            {
        //                UserId = entity.UserId,
        //                NotificationTypeId = entity.NotificationTypeId,
        //                Status = entity.Status,
        //                CreatedBy = entity.UserId,
        //                CreatedOn = DateTime.UtcNow
        //            });
        //            base.DBSave(data);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.ThrowError(ex);
        //    }

        //    finally
        //    {
        //        base.DBClose();
        //    }
        //    return response;
        //}

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
                ex.Log();

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
                var getNotificationStatus = DBEntity.UserProfiles.Where(a => a.UserId == userId).FirstOrDefault();
                var notifyCount = DBEntity.NotificationUsers.Count(a => a.UserId == userId && a.Notification.CreatedOn >= getNotificationStatus.LastNotifiedOn);
                if(notifyCount==0)
                {
                    notifyCount = DBEntity.NotificationUsers.Count(a => a.UserId == userId);
                }
               // var notifyCount = DBEntity.Notifications.Where(a => a.CreatedOn >= getNotificationStatus.LastNotifiedOn).Count();

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

        public DataResponse<EntityNotification> GetNotificationById(int NotificationId)
        {
            DataResponse<EntityNotification> response = new DataResponse<EntityNotification>();
            try
            {
                base.DBInit();

                var query = base.DBEntity.Notifications.Where(a => a.Id == NotificationId).FirstOrDefault();
                var entity = new EntityNotification
                {
                    NotificationId = query.Id,
                    Message = query.Message,
                    TargetId=query.TargetId,       
                    TargetTypeId=query.TargetType,
                    UserId=query.NotificationUsers.FirstOrDefault().UserId
                    
                };

                if (query != null)
                {

                    response.CreateResponse(entity, DataResponseStatus.OK);
                }
                else
                {
                    response.CreateResponse(DataResponseStatus.InternalServerError);
                }
            }
            catch (Exception ex)
            {
                //response.ThrowError(ex);
                ex.Log();
            }

            finally
            {
                base.DBClose();
            }
            return response;
        }

        public DataResponse UpdateNotificationPushStatus(long id, long userId)
        {
            var response = new DataResponse();
            try
            {
                DBInit();
                var model = DBEntity.NotificationUsers.Where(t => t.NotificationId == id).FirstOrDefault();
                model.IsPushSent = true;
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
                ex.Log();

            }

            finally
            {
                DBClose();
            }
            return response;
        }


        public DataResponse<EntityTask> GetTaskById(int TaskId)
        {
            var response = new DataResponse<EntityTask>();
            try
            {
                base.DBInit();

                var query = DBEntity.Tasks.Where(a => a.Id == TaskId).Select(a => new EntityTask
                {
                    //CurrentBusinessId = currentBusinessId,
                    TaskId = a.Id,
                    TaskDescription = a.TaskDescription,
                    TaskRequestTypeId = a.TaskRequestTypeId,
                    TaskType = a.LookupTaskType.TaskType,
                    PriorityTypeId = a.PriorityTypeId,
                    AssignedUsersList = a.TaskUsers.Where(c => c.IsWatcher == false).Select(b => new UserDetails { Name = b.User.FirstName + " " + b.User.LastName, UserId = b.UserId, Email = b.User.Email }),
                    WatchersList = a.TaskUsers.Where(c => c.IsWatcher == true).Select(b => new UserDetails { Name = b.User.FirstName + " " + b.User.LastName, UserId = b.UserId, Email = b.User.Email }),
                    //AssignedTo = a.TaskUsers.Where(c => c.IsWatcher == false).Select(b => b.User.Id.ToString()),
                    AssignedTo = a.TaskUsers.FirstOrDefault(b => b.IsWatcher == false).User.Id,
                    Watchers = a.TaskUsers.Where(c => c.IsWatcher == true).Select(b => b.User.Id.ToString()),
                    FilesList = a.TaskAttachments.Where(c => c.IsActive != false).Select(d => new FilesUploaded { Id = d.Id, FileName = d.FileName }),
                    RequestedUser = new UserDetails { Name = a.User.FirstName + " " + a.User.LastName, UserId = a.User.Id, Email = a.User.Email },
                    ClosingDate = a.ClosingDate,
                    StatusId = a.Status,
                    TargetDate = a.TargetDate,
                    TaskItemOrders = a.TaskItemOrders,
                    PracticeId = a.PracticeId,
                    PracticeName = a.Practice.PracticeName,
                    ReferenceNumber = a.ReferenceNumber,
                    IsActive = a.IsActive,
                    PracticeSpecialityTypeId = a.Practice.PracticeTypeId,
                    Subject = a.Subject,
                    CreatedBy = a.CreatedBy,
                    CreatedByName = a.User.FirstName + " " + a.User.LastName,
                   // AlertDate = a.TaskUserAlerts.Where(b => b.UserId == currentUserId).FirstOrDefault().AlertDate,
                   // IsApiCall = isApiCall,
                    CreatedOn = a.CreatedOn,
                    UpdatedBy = a.UpdatedBy,
                    UpdatedByName = a.User.FirstName + " " + a.User.LastName,
                    UpdatedOn = a.UpdatedOn,
                });

                response = GetFirst<EntityTask>(query);

                if (response.Model.StatusId.HasValue)
                    response.Model.Status = EnumHelper.GetEnumName<TaskStatuses>(response.Model.StatusId.Value);
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
