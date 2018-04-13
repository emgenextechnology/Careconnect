using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EBP.Business.Repository;
using EBP.Business;
using EBP.Business.Entity;
using EBP.Business.Filter;
using EBP.Business.Entity.Note;
using EmgenexBusinessPortal.Models;
using System.Configuration;
using GM.Identity.Config;
using EBP.Business.Notifications;
using EBP.Business.Enums;

namespace EmgenexBusinessPortal.Controllers
{
    [RoutePrefix("{Type:regex(leads|tasks|accounts)}/{ParentId}")]
    [ApiAuthorize]
    public class ApiNoteController : ApiBaseController
    {
        [HttpPost]
        [Route("notes")]
        public IHttpActionResult Index(FilterNote filter)
        {
            var routeDataValues = ControllerContext.RouteData.Values;
            int parentId;
            string typeValue = routeDataValues["Type"].ToString();
            bool hasParentId = int.TryParse(routeDataValues["ParentId"].ToString(), out parentId);
            if (hasParentId)
            {
                filter.ParentId = parentId;
                switch (typeValue)
                {
                    case "tasks":
                        filter.ParentTypeId = (int)NoteType.Task;
                        break;
                    case "leads":
                    case "accounts":
                        filter.ParentTypeId = (int)NoteType.Lead;
                        break;
                }

                var repository = new RepositoryNote();
                var response = repository.GetAllNotes(filter, CurrentBusinessId.Value);
                return Ok<DataResponse<EntityList<EntityNote>>>(response);
            }
            else
            {
                return Ok();
            }
        }

        [HttpPost]
        [Route("save")]
        public IHttpActionResult InsertNotedata(VMNoteModel entity)
        {
            var repository = new RepositoryNote();
            var response = new DataResponse();
            if (ModelState.IsValid)
            {
                string typeValue = ControllerContext.RouteData.Values["Type"].ToString();
                entity.ParentId = int.Parse(ControllerContext.RouteData.Values["ParentId"].ToString());

                switch (typeValue)
                {
                    case "tasks":
                        entity.ParentTypeId = (int)NoteType.Task;
                        break;
                    case "leads":
                        entity.ParentTypeId = (int)NoteType.Lead;
                        break;
                }

                var model = new EntityNote
                {
                    Id = entity.Id,
                    ParentId = entity.ParentId,
                    ParentTypeId = entity.ParentTypeId,
                    CreatedBy = CurrentUser.Id,
                    UpdatedBy = CurrentUser.Id,
                    Description = entity.Description,
                };

                response = repository.SaveNote(model);
                if (entity.ParentTypeId == (int)NoteType.Task && response.Message == "OK")
                {
                    #region Send email to users in assigned to and watchers list
                    var CreatedByName = string.Format("{0} {1}", CurrentUser.FirstName, CurrentUser.LastName);
                    var TaskModel = new RepositoryTask().GetTaskById(model.ParentId, CurrentUserId, CurrentBusinessId);
                    try
                    {
                        var rootPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                        var ReturnUrl = ConfigurationManager.AppSettings["BaseUrl"] + CurrentUser.BusinessName.Replace(" ", "-") + "#/tasks/" + TaskModel.Model.ReferenceNumber;
                        var Subject = "Task " + TaskModel.Model.ReferenceNumber + " - " + TaskModel.Model.Subject + "";
                        var mail = new GMEmail();
                        string toEmails = null;
                        if (CurrentUserId != TaskModel.Model.RequestedUser.UserId)
                        {
                            try
                            {
                                var emailBody = TemplateManager.NewNote(rootPath, TaskModel.Model.RequestedUser.Name, CreatedByName, TaskModel.Model.Subject, ReturnUrl, entity.Description, CurrentBusinessId.Value, CurrentUser.RelativeUrl);
                                mail.SendDynamicHTMLEmail(TaskModel.Model.RequestedUser.Email, Subject, emailBody, CurrentUser.OtherEmails);
                            }
                            catch (Exception ex)
                            {
                                ex.Log();
                            }
                        }
                        var UserList = TaskModel.Model.AssignedUsersList.Concat(TaskModel.Model.WatchersList);
                        foreach (var item in UserList)
                        {
                            if (item.UserId == CurrentUserId)
                                continue;

                            var emailBody = TemplateManager.NewNote(rootPath, item.Name, CreatedByName, TaskModel.Model.Subject, ReturnUrl, entity.Description, CurrentBusinessId.Value, CurrentUser.RelativeUrl);
                            try
                            {
                                toEmails = new RepositoryUserProfile().NotificationEnabledEmails(item.Email, "TSKNOTENFN");
                                if (!string.IsNullOrEmpty(toEmails))
                                {
                                    mail.SendDynamicHTMLEmail(item.Email, Subject, emailBody, CurrentUser.OtherEmails);
                                }
                            }
                            catch (Exception ex)
                            {
                                ex.Log();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.Log();
                    }

                    #endregion
                }
            }
            else
            {
                var errorList = ModelState.Where(a => a.Value.Errors.Any()).Select(s => new
                {
                    Key = s.Key.Split('.').Last(),
                    Message = s.Value.Errors[0].ErrorMessage
                });
                return Ok<dynamic>(new { Status = HttpStatusCode.BadRequest, Model = errorList });
            }
            return Ok<DataResponse>(response);
        }

        [HttpPost]
        [Route("delete/{id}")]
        public IHttpActionResult DeleteNote(int id)
        {
            var DeleteNote = new RepositoryNote().DeleteNote(id);
            if (DeleteNote)
                return Ok<dynamic>(new { IsSuccess = 1, Status = 200, Message = "Note deleted!" });
            else
                return Ok<dynamic>(new { IsSuccess = 0, Message = "Note Notfound!" });
        }
    }
}