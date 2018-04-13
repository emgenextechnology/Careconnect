using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EBP.Business.Entity;
using EBP.Business.Entity.Lead;
using EBP.Business.Entity.Practice;
using EBP.Business.Filter;
using System.Web.UI.WebControls;
using System.Linq.Expressions;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using EBP.Business.Entity.Task;
using EBP.Business.Enums;
using EntityFramework.Extensions;
using System.ComponentModel.DataAnnotations;
using EBP.Business.Resource;
using EBP.Business.Entity.EntityNotificationSettings;

namespace EBP.Business.Repository
{
    public class RepositoryTask : _Repository
    {
        public DataResponse<EntityList<EntityTask>> GetTasks(FilterTask filter, int currentUserId, int? currentBusinessId, bool hasRight, bool hasDeletePermission, bool isApiCall = false, int take = 10, int skip = 0)
        {
            var response = new DataResponse<EntityList<EntityTask>>();
            try
            {
                if (filter != null)
                {
                    take = filter.Take;
                    skip = filter.Skip;
                }

                base.DBInit();

                var query = DBEntity.Tasks.Where(a => a.IsActive == true);

                //var reps = new List<int>();
                //reps = DBEntity.Reps.Where(a => a.RepGroup.RepgroupManagerMappers.Any(b => b.ManagerId == currentUserId) || a.RepGroup.SalesDirectorId == currentUserId).Select(a => a.UserId).ToList();
                ////---ManagerChange    reps = DBEntity.Reps.Where(a => a.RepGroup.ManagerId == currentUserId).Select(a => a.UserId).ToList();
                //reps.Add(currentUserId);

                query = query.Where(a => a.BusinessId == currentBusinessId);

                //if (!hasRight)
                //    query = query.Where(a => a.TaskUsers.Any(tu => reps.Contains(tu.UserId)) || a.RequestedBy == currentUserId);

                if (!hasRight)
                    query = query.Where(a => a.TaskUsers.Any(tu => tu.UserId == currentUserId) || a.RequestedBy == currentUserId);

                if (filter != null)
                {
                    if (filter.PracticeID.HasValue)
                    {
                        query = query.Where(a => a.PracticeId == filter.PracticeID);
                    }

                    if (!String.IsNullOrEmpty(filter.KeyWords))
                        query = query.Where(a => a.Practice.PracticeName.ToLower().Contains(filter.KeyWords)

                            || a.Subject.ToLower().Contains(filter.KeyWords)
                            || a.TaskDescription.ToLower().Contains(filter.KeyWords)

                            || a.TaskUsers.Any(p => p.User.FirstName.ToLower().Contains(filter.KeyWords))
                            || a.TaskUsers.Any(p => p.User.LastName.ToLower().Contains(filter.KeyWords))

                            || a.User.FirstName.ToLower().Contains(filter.KeyWords)
                            || a.User.LastName.ToLower().Contains(filter.KeyWords)

                            || a.ReferenceNumber.Contains(filter.KeyWords)
                            );
                    if (filter.AssignedOrRequest == (int)AssignedOrRequest.AssignedToMe)
                        query = query.Where(a => a.TaskUsers.Any(b => b.IsWatcher == false && b.User.Id == currentUserId));

                    if (filter.AssignedOrRequest == (int)AssignedOrRequest.RequestedByMe)
                        query = query.Where(a => a.CreatedBy == currentUserId);

                    if (filter.IsActive != null)
                        query = query.Where(a => a.IsActive == filter.IsActive);

                    if (!String.IsNullOrEmpty(filter.ReferenceNumber))
                        query = query.Where(a => a.ReferenceNumber.Contains(filter.ReferenceNumber));

                    if (filter.RequestType > 0)
                        query = query.Where(a => a.TaskRequestTypeId == filter.RequestType);
                    else if (filter.RequestType == -1)
                        query = query.Where(a => a.TaskRequestTypeId == null);

                    if (filter.Status > 0)
                        query = query.Where(a => a.Status == filter.Status);

                    if (filter.RequestedBy > 0)
                        query = query.Where(a => a.RequestedBy == filter.RequestedBy);

                    if (filter.AssignedTo > 0)
                        query = query.Where(a => a.TaskUsers.Any(b => b.IsWatcher == false && b.User.Id == filter.AssignedTo));

                    if (filter.Priority > 0)
                        query = query.Where(a => a.PriorityTypeId == filter.Priority);

                    if (filter.DueOn > 0)
                    {
                        Periods periodFilter = (Periods)filter.DueOn;
                        var dt = DateTime.UtcNow;
                        var endDt = DateTime.UtcNow;
                        switch (periodFilter)
                        {
                            case Periods.Today:
                                dt = DateTime.UtcNow.AddHours(-12);
                                query = query.Where(a => a.TargetDate > dt);
                                break;

                            case Periods.Yesterday:
                                dt = DateTime.UtcNow.AddHours(-12);
                                endDt = dt.AddHours(-12);
                                query = query.Where(a => a.TargetDate < dt && a.TargetDate > endDt);
                                break;


                            case Periods.ThisWeek:

                                dt = DateTime.UtcNow.AddDays(-7);
                                query = query.Where(a => a.TargetDate > dt);
                                break;


                            case Periods.LastWeek:

                                dt = DateTime.UtcNow.AddDays(-7);
                                endDt = dt.AddDays(-7);

                                query = query.Where(a => a.TargetDate < dt && a.TargetDate > endDt);

                                break;

                            case Periods.ThisMonth:

                                dt = DateTime.UtcNow.AddDays(-30);

                                query = query.Where(a => a.TargetDate > dt);
                                break;

                            case Periods.LastMonth:

                                dt = DateTime.UtcNow.AddDays(-30);
                                endDt = dt.AddDays(-30);


                                query = query.Where(a => a.TargetDate < dt && a.TargetDate > endDt);
                                break;

                            case Periods.ThisYear:

                                dt = DateTime.UtcNow.AddDays(-365);

                                query = query.Where(a => a.TargetDate > dt);
                                break;

                            case Periods.LastYear:

                                dt = DateTime.UtcNow.AddDays(-365);
                                endDt = dt.AddDays(-365);


                                query = query.Where(a => a.TargetDate < dt && a.TargetDate > endDt);
                                break;

                            default:
                                break;
                        }
                    }
                }

                var selectQuery = query.Select(a => new EntityTask
                {
                    CurrentUserId = currentUserId,
                    HasDeleteRight = hasDeletePermission || currentUserId == a.User.Id,
                    CurrentBusinessId = currentBusinessId.Value,
                    TaskId = a.Id,
                    Subject = a.Subject,
                    TaskDescription = a.TaskDescription,
                    TaskRequestTypeId = a.TaskRequestTypeId,
                    TaskType = a.LookupTaskType.TaskType,
                    PriorityTypeId = a.PriorityTypeId,
                    ClosingDate = a.ClosingDate,
                    TargetDate = a.TargetDate,
                    TaskItemOrders = a.TaskItemOrders,
                    PracticeId = a.PracticeId,
                    PracticeName = a.Practice.PracticeName,
                    ReferenceNumber = a.ReferenceNumber,
                    IsActive = a.IsActive,
                    AssignedUsersList = a.TaskUsers.Where(c => c.IsWatcher == false).Select(b => new UserDetails { Name = b.User.FirstName + " " + b.User.LastName, UserId = b.UserId }),
                    WatchersList = a.TaskUsers.Where(c => c.IsWatcher == true).Select(b => new UserDetails { Name = b.User.FirstName + " " + b.User.LastName, UserId = b.UserId }),
                    RequestedUser = new UserDetails { Name = a.User.FirstName + " " + a.User.LastName, UserId = a.User.Id },
                    FilesList = a.TaskAttachments.Where(c => c.IsActive != false).Select(d => new FilesUploaded { Id = d.Id, FileName = d.FileName }),
                    StatusId = a.Status,
                    AlertDate = a.TaskUserAlerts.Where(b => b.UserId == currentUserId).FirstOrDefault().AlertDate,
                    IsApiCall = isApiCall,
                    CreatedBy = a.CreatedBy,
                    CreatedOn = a.CreatedOn,
                    UpdatedBy = a.UpdatedBy,
                    UpdatedOn = a.UpdatedOn
                }).OrderBy(a => a.StatusId == (int)TaskStatuses.Completed).ThenByDescending(a => a.UpdatedOn);

                response = GetList<EntityTask>(selectQuery, skip, take);

                response.Model.List.ForEach(a =>
                {
                    if (a.PriorityTypeId.HasValue)
                        a.PriorityType = EnumHelper.GetEnumName<TaskPriorities>(a.PriorityTypeId.Value);
                    if (a.StatusId.HasValue)
                        a.Status = EnumHelper.GetEnumName<TaskStatuses>(a.StatusId.Value);
                    a.CreatedOn = a.CreatedOn.ToLocalTime();
                });
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

        public DataResponse<EntityTask> GetTaskById(int TaskId, int currentUserId, int? currentBusinessId, bool isApiCall = false)
        {
            var response = new DataResponse<EntityTask>();
            try
            {
                base.DBInit();
                TimeZoneInfo estZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                var query = DBEntity.Tasks.Where(a => a.Id == TaskId).Select(a => new EntityTask
                {
                    CurrentBusinessId = currentBusinessId,
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
                    AlertDate = a.TaskUserAlerts.Where(b => b.UserId == currentUserId).FirstOrDefault().AlertDate,
                    IsApiCall = isApiCall,
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

        public DataResponse<EntityTask> Insert(EntityTask entity)
        {
            var response = new DataResponse<EntityTask>();
            try
            {
                base.DBInit();

                var model = new Database.Task
                {
                    RequestedBy = entity.CurrentUserId,
                    Subject = entity.Subject,
                    TaskDescription = entity.TaskDescription,
                    BusinessId = entity.CurrentBusinessId,
                    TaskRequestTypeId = entity.TaskRequestTypeId,
                    PracticeId = entity.PracticeId,
                    PriorityTypeId = entity.PriorityTypeId,
                    TargetDate = entity.TargetDate,
                    ClosingDate = entity.ClosingDate,
                    IsActive = true,
                    Status = (int)TaskStatuses.New,
                    CreatedOn = entity.CreatedOn,
                    CreatedBy = entity.CreatedBy,
                    UpdatedOn = entity.UpdatedOn,
                    UpdatedBy = entity.UpdatedBy
                };

                if (entity.AlertDate.HasValue)
                {
                    var taskUserAlerts = new List<Database.TaskUserAlert> {
                        new Database.TaskUserAlert
                        {
                            UserId = entity.CurrentUserId,
                            AlertDate = entity.AlertDate.Value,
                            Message = entity.AlertMessage,
                            CreatedOn = entity.CreatedOn,
                            CreatedBy = entity.CreatedBy,
                        }
                    };
                    model.TaskUserAlerts = taskUserAlerts;
                }
                var notificationlist = new List<EntityNotification>();

                if (entity.AssignedTo > 0)
                {
                    model.TaskUsers.Add(new Database.TaskUser
                    {
                        UserId = entity.AssignedTo,
                        IsWatcher = false,
                        CreatedOn = entity.CreatedOn,
                        CreatedBy = entity.CreatedBy
                    });

                    notificationlist.Add(new EntityNotification
                    {
                        UserId = entity.AssignedTo,
                        Message = NotificationMessages.TaskUserNotification
                    });
                }

                if (entity.Watchers != null)
                    foreach (var user in entity.Watchers)
                    {
                        model.TaskUsers.Add(new Database.TaskUser
                        {
                            UserId = int.Parse(user),
                            IsWatcher = true,
                            CreatedOn = entity.CreatedOn,
                            CreatedBy = entity.CreatedBy
                        });

                        notificationlist.Add(new EntityNotification
                        {
                            UserId = int.Parse(user),
                            Message = NotificationMessages.TaskUserNotification
                        });
                    }

                if (base.DBSave(model) > 0)
                {

                    #region Save Notification Data

                    new RepositoryNotification().Save(notificationlist, entity.CurrentUserId, model.Id, (int)NotificationTargetType.Task, (int)NotificationType.Normal, model.CreatedBy, entity.CreatedByName, model.CreatedOn, model.Subject);

                    #endregion

                    #region Save to Log

                    var objTask = new Database.Task
                    {
                        Id = model.Id,
                        RequestedBy = model.RequestedBy,
                        Subject = model.Subject,
                        TaskDescription = model.TaskDescription,
                        BusinessId = model.BusinessId,
                        TaskRequestTypeId = model.TaskRequestTypeId,
                        ReferenceNumber = model.ReferenceNumber,
                        PracticeId = model.PracticeId,
                        PriorityTypeId = model.PriorityTypeId,
                        TargetDate = model.TargetDate,
                        ClosingDate = model.ClosingDate,
                        IsActive = model.IsActive,
                        Status = model.Status,
                        TaskUsers = model.TaskUsers,
                        TaskUserAlerts = model.TaskUserAlerts,
                        User = model.User,
                        CreatedOn = model.CreatedOn,
                        CreatedBy = model.CreatedBy
                    };

                    if (!SaveLog(objTask, "Created"))
                    {
                        new Exception("Task log failed!  Task:" + entity.TaskId).Log();
                    }

                    #endregion

                    return GetTaskById(model.Id, entity.CurrentUserId, entity.CurrentBusinessId);
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

        public DataResponse Update(EntityTask entity)
        {
            var response = new DataResponse<EntityTask>();
            try
            {
                base.DBInit();

                entity.UpdatedOn = DateTime.UtcNow;

                #region Prepare model
                var model = DBEntity.Tasks.FirstOrDefault(a => a.Id == entity.TaskId);

                model.RequestedBy = entity.CurrentUserId;
                model.Subject = entity.Subject;
                model.PracticeId = entity.PracticeId;
                model.TaskDescription = entity.TaskDescription;
                model.PriorityTypeId = entity.PriorityTypeId;
                model.TaskRequestTypeId = entity.TaskRequestTypeId;
                model.TargetDate = entity.TargetDate;
                model.ClosingDate = entity.ClosingDate;
                model.UpdatedOn = entity.UpdatedOn;
                model.UpdatedBy = entity.UpdatedBy;

                List<EntityNotification> notificationlist = new List<EntityNotification>();
                if (model.Status != entity.StatusId)
                {
                    model.Status = entity.StatusId;
                }
                //    notificationlist = model.TaskUsers.Where(a => a.UserId != entity.CurrentUserId).Select(a => new EntityNotification
                //    {
                //        UserId = a.UserId,
                //        Message = NotificationMessages.TaskStatusUpdateUserNotification
                //    }).ToList();

                //    if (model.CreatedBy != entity.CurrentUserId)
                //        notificationlist.Add(new EntityNotification
                //        {
                //            UserId = model.CreatedBy,
                //            Message = NotificationMessages.TaskStatusUpdateUserNotification
                //        });
                //}
                if (entity.AlertDate.HasValue)
                {
                    var modelItem = model.TaskUserAlerts.FirstOrDefault(a => a.UserId == entity.CurrentUserId);
                    if (modelItem != null)
                    {
                        DBEntity.Entry(modelItem).State = EntityState.Modified;

                        modelItem.AlertDate = entity.AlertDate.Value;
                        modelItem.Message = entity.AlertMessage;
                        modelItem.UpdatedOn = entity.UpdatedOn;
                        modelItem.UpdatedBy = entity.UpdatedBy;
                    }
                    else
                    {
                        var taskUserAlerts = new Database.TaskUserAlert
                        {
                            TaskId = model.Id,
                            UserId = entity.CurrentUserId,
                            AlertDate = entity.AlertDate.Value,
                            Message = entity.AlertMessage,
                            CreatedOn = entity.UpdatedOn.Value,
                            CreatedBy = entity.UpdatedBy.Value,
                        };
                        model.TaskUserAlerts.Add(taskUserAlerts);
                    }
                }

                //delete all previous assignments
                DBEntity.TaskUsers.Where(a => a.TaskId == model.Id).Delete();

                if (entity.AssignedTo > 0)
                {
                    model.TaskUsers.Add(new Database.TaskUser
                    {
                        UserId = entity.AssignedTo,
                        IsWatcher = false,
                        CreatedOn = entity.UpdatedOn.Value,
                        CreatedBy = entity.UpdatedBy.Value
                    });

                    //    if (entity.CurrentUserId != entity.AssignedTo)
                    notificationlist.Add(new EntityNotification
                    {
                        UserId = entity.AssignedTo,
                        Message = NotificationMessages.TaskUpdateUserNotification
                    });
                }

                if (entity.Watchers != null)
                    foreach (var user in entity.Watchers)
                    {

                        model.TaskUsers.Add(new Database.TaskUser
                        {
                            UserId = int.Parse(user),
                            IsWatcher = true,
                            CreatedOn = entity.UpdatedOn.Value,
                            CreatedBy = entity.UpdatedBy.Value
                        });

                        notificationlist.Add(new EntityNotification
                        {
                            UserId = int.Parse(user),
                            Message = NotificationMessages.TaskUpdateUserNotification
                        });

                    }

                notificationlist.Add(new EntityNotification
                {
                    UserId = model.CreatedBy,
                    Message = NotificationMessages.TaskUpdateUserNotification
                });
                #endregion

                #region Delete removed files from db on update

                IEnumerable<int> incomingFileLists = null;
                IEnumerable<Database.TaskAttachment> existingFiles = null;

                if (entity.FilesList != null && entity.FilesList.Count() > 0)
                {
                    incomingFileLists = entity.FilesList.Select(a => a.Id);
                }
                if (incomingFileLists != null && incomingFileLists.Count() > 0)
                    existingFiles = model.TaskAttachments.Where(a => !incomingFileLists.Contains(a.Id));
                else
                    existingFiles = model.TaskAttachments;

                if (existingFiles.Count() > 0)
                    foreach (var item in existingFiles)
                    {
                        item.IsActive = false;
                    }

                #endregion

                if (base.DBSaveUpdate(model) > 0)
                {
                    #region Save Notification Data

                    new RepositoryNotification().Save(notificationlist, entity.CurrentUserId, model.Id, (int)NotificationTargetType.Task, (int)NotificationType.Normal, model.UpdatedBy.Value, entity.CreatedByName, model.UpdatedOn.Value, model.Subject);

                    #endregion

                    #region Save to Log
                    var objTask = new Database.Task
                    {
                        Id = model.Id,
                        RequestedBy = entity.CurrentUserId,
                        Subject = model.Subject,
                        TaskDescription = model.TaskDescription,
                        BusinessId = model.BusinessId,
                        TaskRequestTypeId = model.TaskRequestTypeId,
                        ReferenceNumber = model.ReferenceNumber,
                        PracticeId = model.PracticeId,
                        PriorityTypeId = model.PriorityTypeId,
                        TargetDate = model.TargetDate,
                        ClosingDate = model.ClosingDate,
                        IsActive = model.IsActive,
                        Status = model.Status,
                        TaskUsers = model.TaskUsers,
                        TaskUserAlerts = model.TaskUserAlerts,
                        User = model.User,
                        CreatedOn = model.UpdatedOn.Value,
                        CreatedBy = model.UpdatedBy.Value
                    };

                    if (!SaveLog(objTask))
                    {
                        new Exception("Task log failed!  Task:" + entity.TaskId).Log();
                    }
                    #endregion

                    //return GetTaskById(model.Id, entity.CurrentUserId, entity.CurrentBusinessId);
                    return new DataResponse { Status = DataResponseStatus.OK, Id = model.Id };
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

        public bool SaveLog(Database.Task taskModel, string action = "Updated")
        {
            base.DBInit();

            if (taskModel != null)
            {
                DateTime? AlertDate = null;
                if (taskModel.TaskUserAlerts != null && taskModel.TaskUserAlerts.Count() > 0)
                    AlertDate = taskModel.TaskUserAlerts.FirstOrDefault().AlertDate;
                var logModel = new Database.TaskLog
                {
                    AlertDate = AlertDate,
                    TaskId = taskModel.Id,
                    RequestedBy = taskModel.RequestedBy,
                    Subject = taskModel.Subject,
                    TaskDescription = taskModel.TaskDescription,
                    BusinessId = taskModel.BusinessId,
                    TaskRequestTypeId = taskModel.TaskRequestTypeId,
                    ReferenceNumber = taskModel.ReferenceNumber,
                    PracticeId = taskModel.PracticeId,
                    PriorityTypeId = taskModel.PriorityTypeId,
                    TargetDate = taskModel.TargetDate,
                    ClosingDate = taskModel.ClosingDate,
                    IsActive = taskModel.IsActive,
                    Status = taskModel.Status,
                    Action = action,
                    CreatedOn = taskModel.CreatedOn,
                    CreatedBy = taskModel.CreatedBy
                };

                if (taskModel.TaskUsers != null)
                {
                    foreach (var taskUser in taskModel.TaskUsers)
                    {
                        var objUser = DBEntity.Users.FirstOrDefault(a => a.Id == taskUser.UserId);

                        if (taskUser.IsWatcher.HasValue && taskUser.IsWatcher.Value)
                            logModel.Watchers = string.Format("{0}{1}", string.IsNullOrEmpty(logModel.Watchers) ? string.Empty : logModel.Watchers + ",",
                                string.Format("{{\"Name\":\"{0} {1}\",\"Id\":{2}}}", objUser.FirstName, objUser.LastName, objUser.Id));
                        else
                            logModel.AssignedTo = string.Format("{{\"Name\":\"{0} {1}\",\"Id\":{2}}}", objUser.FirstName, objUser.LastName, objUser.Id);
                    }
                }

                logModel.Watchers = string.Format("[{0}]", logModel.Watchers);
                logModel.AssignedTo = string.Format("[{0}]", logModel.AssignedTo);

                return base.DBSave(logModel) > 0;
            }

            return false;
        }

        public DataResponse ToggleStatus(int TaskId, int currentUserID, bool hasDeletePermission)
        {
            var response = new DataResponse<bool?>();
            try
            {
                base.DBInit();
                var model = DBEntity.Tasks.FirstOrDefault(a => a.Id == TaskId);
                model.UpdatedBy = currentUserID;
                model.UpdatedOn = DateTime.UtcNow;
                if (model != null && (hasDeletePermission || currentUserID == model.User.Id))
                {
                    model.IsActive = (model.IsActive == false) ? true : false;
                }
                if (base.DBSaveUpdate(model) == 1)
                {
                    var objTask = new Database.Task
                    {
                        Id = model.Id,
                        RequestedBy = model.RequestedBy,
                        Subject = model.Subject,
                        TaskDescription = model.TaskDescription,
                        BusinessId = model.BusinessId,
                        TaskRequestTypeId = model.TaskRequestTypeId,
                        ReferenceNumber = model.ReferenceNumber,
                        PracticeId = model.PracticeId,
                        PriorityTypeId = model.PriorityTypeId,
                        TargetDate = model.TargetDate,
                        ClosingDate = model.ClosingDate,
                        IsActive = model.IsActive,
                        Status = model.Status,
                        TaskUsers = model.TaskUsers,
                        TaskUserAlerts = model.TaskUserAlerts,
                        User = model.User,
                        CreatedOn = model.UpdatedOn.Value,
                        CreatedBy = model.UpdatedBy.Value
                    };
                    SaveLog(objTask);
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

        public DataResponse TogglePriority(int TaskId, int currentUserID)
        {
            var response = new DataResponse<int?>();
            try
            {
                base.DBInit();
                var model = DBEntity.Tasks.FirstOrDefault(a => a.Id == TaskId);
                response.Model = model.PriorityTypeId;
                model.UpdatedBy = currentUserID;
                model.UpdatedOn = DateTime.UtcNow;
                if (model != null)
                {
                    model.PriorityTypeId = (model.PriorityTypeId == (int?)TaskPriorities.High) ? (int)TaskPriorities.Low : (int)TaskPriorities.High;
                }
                if (base.DBSaveUpdate(model) == 1)
                {
                    var objTask = new Database.Task
                    {
                        Id = model.Id,
                        RequestedBy = model.RequestedBy,
                        Subject = model.Subject,
                        TaskDescription = model.TaskDescription,
                        BusinessId = model.BusinessId,
                        TaskRequestTypeId = model.TaskRequestTypeId,
                        ReferenceNumber = model.ReferenceNumber,
                        PracticeId = model.PracticeId,
                        PriorityTypeId = model.PriorityTypeId,
                        TargetDate = model.TargetDate,
                        ClosingDate = model.ClosingDate,
                        IsActive = model.IsActive,
                        Status = model.Status,
                        TaskUsers = model.TaskUsers,
                        TaskUserAlerts = model.TaskUserAlerts,
                        User = model.User,
                        CreatedOn = model.UpdatedOn.Value,
                        CreatedBy = model.UpdatedBy.Value
                    };
                    SaveLog(objTask);
                    response.Model = model.PriorityTypeId;
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

        public DataResponse SetStatus(int Taskid, int StatusId, int currentUserID, string updatedByName)
        {
            var response = new DataResponse<int?>();
            try
            {
                base.DBInit();
                var model = DBEntity.Tasks.FirstOrDefault(a => a.Id == Taskid);

                model.UpdatedBy = currentUserID;
                model.UpdatedOn = DateTime.UtcNow;
                model.Status = StatusId;

                if (base.DBSaveUpdate(model) > 0)
                {
                    var objTask = new Database.Task
                    {
                        Id = model.Id,
                        RequestedBy = model.RequestedBy,
                        Subject = model.Subject,
                        TaskDescription = model.TaskDescription,
                        BusinessId = model.BusinessId,
                        TaskRequestTypeId = model.TaskRequestTypeId,
                        ReferenceNumber = model.ReferenceNumber,
                        PracticeId = model.PracticeId,
                        PriorityTypeId = model.PriorityTypeId,
                        TargetDate = model.TargetDate,
                        ClosingDate = model.ClosingDate,
                        IsActive = model.IsActive,
                        Status = model.Status,
                        TaskUsers = model.TaskUsers,
                        TaskUserAlerts = model.TaskUserAlerts,
                        User = model.User,
                        CreatedOn = model.UpdatedOn.Value,
                        CreatedBy = model.UpdatedBy.Value
                    };
                    SaveLog(objTask);
                    response.Model = model.Status;

                    #region Save Notification Data

                    var notificationlist = new List<EntityNotification>();

                    notificationlist = model.TaskUsers.Select(a => new EntityNotification
                    {
                        UserId = a.UserId,
                        Message = NotificationMessages.TaskStatusUpdateUserNotification
                    }).ToList();


                    notificationlist.Add(new EntityNotification
                    {
                        UserId = model.CreatedBy,
                        Message = NotificationMessages.TaskStatusUpdateUserNotification
                    });

                    new RepositoryNotification().Save(notificationlist, currentUserID, model.Id, (int)NotificationTargetType.Task, (int)NotificationType.Normal, model.CreatedBy, updatedByName, model.CreatedOn, model.Subject);

                    #endregion
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

        public DataResponse<bool> DeleteFile(int FileId)
        {
            var response = new DataResponse<bool>();
            try
            {
                base.DBInit();

                var model = DBEntity.TaskAttachments.FirstOrDefault(a => a.Id == FileId);
                if (model != null)
                {
                    var task = DBEntity.Tasks.Where(a => a.Id == model.TaskId).FirstOrDefault();

                    DateTime? AlertDate = null;
                    var taskAlert = task.TaskUserAlerts.FirstOrDefault();
                    if (taskAlert != null)
                        AlertDate = taskAlert.AlertDate;

                    var logModel = new Database.TaskLog
                    {
                        Action = "File Deleted",
                        AlertDate = AlertDate,
                        BusinessId = task.BusinessId,
                        ClosingDate = task.ClosingDate,
                        CreatedBy = task.CreatedBy,
                        CreatedOn = task.CreatedOn,
                        Files = model.FileName,
                        IsActive = task.IsActive,
                        PracticeId = task.PracticeId,
                        PriorityTypeId = task.PriorityTypeId,
                        ReferenceNumber = task.ReferenceNumber,
                        RequestedBy = task.RequestedBy,
                        Status = task.Status,
                        Subject = task.Subject,
                        TargetDate = task.TargetDate,
                        TaskDescription = task.TaskDescription,
                        TaskId = task.Id,
                        TaskRequestTypeId = task.TaskRequestTypeId
                    };

                    if (task.TaskUsers != null)
                        foreach (var taskUser in task.TaskUsers)
                        {
                            if (taskUser.IsWatcher.HasValue && taskUser.IsWatcher.Value)
                                logModel.Watchers = string.Format("{0}{1}", string.IsNullOrEmpty(logModel.Watchers) ? string.Empty : logModel.Watchers + ",",
                                    string.Format("{{\"Name\":\"{0} {1}\",\"Id\":{2}}}", taskUser.User.FirstName, taskUser.User.FirstName, taskUser.User.Id));
                            else
                                logModel.AssignedTo = string.Format("{0}{1}", string.IsNullOrEmpty(logModel.AssignedTo) ? string.Empty : logModel.AssignedTo + ",",
                                    string.Format("{{\"Name\":\"{0} {1}\",\"Id\":{2}}}", taskUser.User.FirstName, taskUser.User.FirstName, taskUser.User.Id));
                        }

                    logModel.Watchers = string.Format("[{0}]", logModel.Watchers);
                    logModel.AssignedTo = string.Format("[{0}]", logModel.AssignedTo);

                    EntityAdd<Database.TaskLog>(logModel);

                    model.IsActive = false;
                }
                if (DBEntity.SaveChanges() > 0)
                {
                    response.Model = true;
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

        public bool SaveFiles(List<string> FilesList, int TaskId, bool isEdit)
        {
            try
            {
                base.DBInit();
                foreach (var fileName in FilesList)
                {
                    var attachmentModel = new Database.TaskAttachment
                    {
                        FileName = fileName,
                        TaskId = TaskId
                    };

                    EntityAdd<Database.TaskAttachment>(attachmentModel);
                }

                var log = DBEntity.TaskLogs.Where(a => a.TaskId == TaskId).OrderByDescending(o => o.Id).FirstOrDefault();
                if (log != null)
                {
                    log.Files = string.Join(",", FilesList.ToArray());
                }

                if (DBEntity.SaveChanges() <= 0)
                    return false;
            }
            catch (Exception ex)
            {
                ex.Log();
                return false;
            }
            finally
            {
                base.DBClose();
            }
            return true;
        }

        public DataResponse<EntityTaskLog> GetTaskLogs(int id, int currentUserId)
        {
            var response = new DataResponse<EntityTaskLog>();
            try
            {
                base.DBInit();

                var reps = new List<int>();
                reps = DBEntity.Reps.Where(a => a.RepGroup.RepgroupManagerMappers.Any(b => b.ManagerId == currentUserId)).Select(a => a.UserId).ToList();// || a.RepGroup.SalesDirectorId == currentUserId
                //---ManagerChange    reps = DBEntity.Reps.Where(a => a.RepGroup.ManagerId == currentUserId).Select(a => a.UserId).ToList();
                reps.Add(currentUserId);

                var query = DBEntity.Tasks
                    .Where(a => a.Id == id && (a.TaskUsers.Any(tu => reps.Contains(tu.UserId)) || a.RequestedBy == currentUserId))
                    .Select(a => new EntityTaskLog
                    {
                        CreatedOn = a.CreatedOn,
                        Subject = a.Subject,
                        TaskId = a.Id,
                        UpdatedOn = a.UpdatedOn,
                        TaskLogList = a.TaskLogs.Select(b => new EntityTaskHistory
                        {
                            Action = b.Action,
                            AlertDate = b.AlertDate,
                            AssignedToList = b.AssignedTo,
                            CreatedOn = b.CreatedOn,
                            CreatedBy = b.User.FirstName + " " + b.User.LastName,
                            Description = b.TaskDescription,
                            PracticeName = b.Practice.PracticeName,
                            StatusId = b.Status,
                            Subject = b.Subject,
                            TargetDate = b.TargetDate,
                            WatchersList = b.Watchers,
                        }).OrderByDescending(o => o.CreatedOn)
                    });

                response = GetFirst<EntityTaskLog>(query); ;

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

        public DataResponse SaveDueDate(DateTime TargetDate, int TaskId, int currentUserID)
        {
            var response = new DataResponse();
            try
            {
                base.DBInit();
                var model = DBEntity.Tasks.FirstOrDefault(a => a.Id == TaskId);
                if (model != null)
                {
                    model.TargetDate = TargetDate;
                    model.UpdatedBy = currentUserID;
                    model.UpdatedOn = DateTime.UtcNow;

                    if (base.DBSaveUpdate(model) == 1)
                    {
                        var objTask = new Database.Task
                        {
                            Id = model.Id,
                            RequestedBy = model.RequestedBy,
                            Subject = model.Subject,
                            TaskDescription = model.TaskDescription,
                            BusinessId = model.BusinessId,
                            TaskRequestTypeId = model.TaskRequestTypeId,
                            ReferenceNumber = model.ReferenceNumber,
                            PracticeId = model.PracticeId,
                            PriorityTypeId = model.PriorityTypeId,
                            TargetDate = model.TargetDate,
                            ClosingDate = model.ClosingDate,
                            IsActive = model.IsActive,
                            Status = model.Status,
                            TaskUsers = model.TaskUsers,
                            TaskUserAlerts = model.TaskUserAlerts,
                            User = model.User,
                            CreatedOn = model.UpdatedOn.Value,
                            CreatedBy = model.UpdatedBy.Value
                        };
                        SaveLog(objTask);
                        response.Status = DataResponseStatus.OK;
                    }
                    else
                        response.Status = DataResponseStatus.NotModified;
                }
                else
                    response.Status = DataResponseStatus.NotModified;
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

        public DataResponse SaveAlertDate(DateTime AlertDate, int TaskId, int currentUserId)
        {
            var response = new DataResponse();
            try
            {
                base.DBInit();
                var model = DBEntity.TaskUserAlerts.FirstOrDefault(a => a.Task.Id == TaskId);
                if (model != null)
                {
                    model.AlertDate = AlertDate;
                    model.UpdatedBy = currentUserId;
                    model.UpdatedOn = DateTime.UtcNow;

                    if (base.DBSaveUpdate(model) == 1)
                    {

                        response.Status = DataResponseStatus.OK;
                    }
                    else
                        response.Status = DataResponseStatus.NotModified;
                }
                else
                    response.Status = DataResponseStatus.NotModified;
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