using EBP.Business.Entity;
using EBP.Business.Entity.EntityNotificationSettings;
using EBP.Business.Entity.Note;
using EBP.Business.Enums;
using EBP.Business.Filter;
using EBP.Business.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBP.Business.Repository
{
    public class RepositoryNote : _Repository
    {
        public DataResponse<EntityList<EntityNote>> GetAllNotes(FilterNote filter, int currentBusineId, int take = 10, int skip = 0)
        {
            var response = new DataResponse<EntityList<EntityNote>>();
            if (filter != null)
            {
                take = filter.Take;
                skip = filter.Skip;
            }
            base.DBInit();

            var query = DBEntity.Notes.Where(a => a.ParentTypeId == filter.ParentTypeId & a.ParentId == filter.ParentId && a.IsDeleted != true)
                .Select(a => new EntityNote
                {
                    Id = a.Id,
                    Description = a.Description,
                    ParentTypeId = a.ParentTypeId,
                    ParentId = a.ParentId,
                    CreatedBy = a.CreatedBy,
                    CreatedOn = a.CreatedOn,
                    UserDetails = new UserDetails { UserId = a.CreatedBy, Name = a.User.FirstName + " " + a.User.LastName, date = a.CreatedOn.ToString(), BusinessId = currentBusineId }
                }).OrderByDescending(o => o.CreatedOn);

            response = GetList<EntityNote>(query, skip, take);

            response.Model.List.Reverse();

            base.DBClose();

            return response;
        }

        public DataResponse<EntityNote> GetNoteById(int ParentId, int ParentTypeId)
        {
            DataResponse<EntityNote> response = new DataResponse<EntityNote>();
            try
            {
                base.DBInit();



                base.DBInit();

                var query = DBEntity.Notes.Where(a => a.ParentTypeId == ParentTypeId & a.ParentId == ParentId).OrderByDescending(a => a.Id).FirstOrDefault();
                var entity = new EntityNote
                {
                    Id = query.Id,
                    ParentId = query.ParentId,
                    ParentTypeId = query.ParentTypeId,
                    Description = query.Description,
                    CreatedBy = query.CreatedBy,
                    CreatedOn = query.CreatedOn,
                    UserDetails = new UserDetails { UserId = query.CreatedBy, Name = query.User.FirstName + " " + query.User.LastName, date = query.CreatedOn.ToString() }

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
                response.ThrowError(ex);
            }

            finally
            {
                base.DBClose();
            }
            return response;
        }

        public DataResponse<EntityNote> SaveNote(EntityNote entity)
        {
            DataResponse<EntityNote> response = new DataResponse<EntityNote>();
            try
            {
                base.DBInit();
                var model = new Database.Note
                {
                    Id = entity.Id,
                    ParentId = entity.ParentId,
                    ParentTypeId = entity.ParentTypeId,
                    Description = entity.Description,
                    CreatedBy = entity.CreatedBy,
                    CreatedOn = DateTime.UtcNow,
                };

                int rowCount = entity.Id > 0 ? base.DBSaveUpdate(model) : base.DBSave(model);

                if (rowCount > 0)
                {
                    if (entity.ParentTypeId == (int)NoteType.Task)
                    {
                        var TaskModel = new RepositoryTask().GetTaskById(model.ParentId, entity.CreatedBy, entity.BusinessId);
                        if (TaskModel.Model != null)
                        {
                            //Add To Notification Table
                            List<EntityNotification> notificationlist = new List<EntityNotification>();

                            if (TaskModel.Model.AssignedTo > 0)
                            {
                                notificationlist.Add(new EntityNotification
                                {
                                    UserId = TaskModel.Model.AssignedTo,
                                    Message = NotificationMessages.TaskNoteNotification
                                });
                            }

                            if (TaskModel.Model.Watchers != null)
                                foreach (var user in TaskModel.Model.Watchers)
                                {

                                    notificationlist.Add(new EntityNotification
                                    {
                                        UserId = int.Parse(user),
                                        Message = NotificationMessages.TaskNoteNotification
                                    });

                                }

                            notificationlist.Add(new EntityNotification
                            {
                                UserId = TaskModel.Model.CreatedBy,
                                Message = NotificationMessages.TaskNoteNotification
                            });

                            #region Save Notification Data

                            new RepositoryNotification().Save(notificationlist, entity.CreatedBy, model.Id, (int)NotificationTargetType.Task, (int)NotificationType.Normal, model.CreatedBy, entity.CreatedByName, model.CreatedOn, TaskModel.Model.Subject);

                            #endregion
                        }
                    }
                    entity.Id = model.Id;

                    entity.UserDetails = DBEntity.Users.Where(a => a.Id == model.CreatedBy).Select(a => new UserDetails { UserId = model.CreatedBy, Name = a.FirstName + "" + a.LastName, date = model.CreatedOn.ToString() }).FirstOrDefault();
                    response.CreateResponse(entity, DataResponseStatus.OK);
                }
                else
                {
                    response.CreateResponse(DataResponseStatus.InternalServerError);
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

        public bool DeleteNote(int id)
        {
            try
            {
                DBInit();

                var model = DBEntity.Notes.FirstOrDefault(m => m.Id == id);

                if (model != null)
                {
                    model.IsDeleted = true;
                    if (base.DBSaveUpdate(model) > 0)
                    {
                        return true;
                    }
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

            return false;
        }
    }
}
