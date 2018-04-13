using EBP.Api.Models;
using EBP.Business;
using EBP.Business.Entity;
using EBP.Business.Entity.Task;
using EBP.Business.Enums;
using EBP.Business.Filter;
using EBP.Business.Notifications;
using EBP.Business.Repository;
using GM.Identity.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;

namespace EBP.Api.Controllers
{
    [RoutePrefix("tasks")]
    [ApiAuthorize]
    public class ApiTaskController : ApiBaseController
    {
        [HttpPost]
        [Route("getbyfilter")]
        public IHttpActionResult GetByFilter(FilterTask filter)
        {
            string[] allowedRoles = { "RDTSK" };
            string[] superRoles = { "RDTSKALL" };
            string[] taskDeletePrivileges = { "DLTALLINTSK" };

            if (HasRight(allowedRoles) || HasRight(superRoles))
            {
                var repository = new RepositoryTask();

                var response = repository.GetTasks(filter, CurrentUserId, CurrentBusinessId, HasSuperRight(superRoles), HasRight(taskDeletePrivileges), true);
                return Ok<DataResponse<EntityList<EntityTask>>>(response);
            }
            else
            {
                return Ok<DataResponse>(null);
            }
        }

        [Route("getfilter")]
        public IHttpActionResult GetFilter()
        {
            //return Ok(new FilterLead { Periods=new int[]{-1,-7,} });
            return Ok(new FilterTask());
        }

        [Route("getTaskbyid/{TaskId}")]
        public IHttpActionResult GetTaskById(int? TaskId)
        {
            var response = new DataResponse<EntityTask>();
            var repository = new RepositoryTask();
            if (TaskId.HasValue)
            {
                response = repository.GetTaskById(TaskId.Value, CurrentUserId, CurrentBusinessId, true);
            }
            else
            {
                response.Model = new EntityTask();
            }
            return Ok<DataResponse>(response);
        }

        [Route("gettasklog/{id}")]
        public IHttpActionResult GetTaskLogs(int id)
        {
            var response = new RepositoryTask().GetTaskLogs(id, CurrentUserId);
            return Ok<DataResponse<EntityTaskLog>>(response);
        }

        [HttpPost]
        [Route("Save")]
        public IHttpActionResult InsertTaskData(EntityTask model)
        {
            var response = new DataResponse<EntityTask>();
            var TaskId = 0;

            if (ModelState.IsValid)
            {
                model.UpdatedBy = model.CreatedBy = model.CurrentUserId = CurrentUserId;
                model.CurrentBusinessId = CurrentBusinessId;
                model.CreatedByName = string.Format("{0} {1}", CurrentUser.FirstName, CurrentUser.LastName);

                if (model.TaskId > 0)
                {
                    var updateResponse = new RepositoryTask().Update(model);
                    if (updateResponse.Status == DataResponseStatus.OK)
                        TaskId = (int)updateResponse.Id;
                }
                else
                {
                    response = new RepositoryTask().Insert(model);
                    TaskId = response.Model.TaskId;

                    #region Send email to users in assigned to and watchers list

                    try
                    {
                        var rootPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                        var ReturnUrl = ConfigurationManager.AppSettings["PortalUrl"] + CurrentUser.BusinessName.Replace(" ", "-") + "#/tasks/" + response.Model.ReferenceNumber;
                        var Subject = "Task " + response.Model.ReferenceNumber + " - " + response.Model.Subject + "";
                        var mail = new GMEmail();
                        string toEmails = null,
                            practiceName = null,
                            priorityType = null,
                            targetDate = null;
                        if (model.PracticeId.HasValue)
                            practiceName = new RepositoryLookups().GetPracticeNameById(model.PracticeId.Value);
                        if (model.PriorityTypeId.HasValue)
                            priorityType = EnumHelper.GetEnumName<TaskPriorities>(model.PriorityTypeId.Value);
                        targetDate = model.TargetDate.HasValue ? model.TargetDate.ToString() : "Not Set";

                        foreach (var item in response.Model.AssignedUsersList)
                        {
                            if (item.UserId == CurrentUserId)
                                continue;

                            var emailBody = TemplateManager.NewTask(rootPath, item.Name, "", model.CreatedByName, model.Subject, targetDate, model.TaskDescription, practiceName, priorityType, ReturnUrl, false, CurrentBusinessId.Value, CurrentUser.RelativeUrl);
                            try
                            {
                                toEmails = new RepositoryUserProfile().NotificationEnabledEmails(item.Email, "TSKASSGN");
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

                        foreach (var item in response.Model.WatchersList)
                        {
                            if (item.UserId == CurrentUserId)
                                continue;

                            var AssignedUsers = string.Join(",", response.Model.AssignedUsersList.Select(a => a.Name));

                            var emailBody = TemplateManager.NewTask(rootPath, item.Name, AssignedUsers, model.CreatedByName, model.Subject, targetDate, model.TaskDescription, practiceName, priorityType, ReturnUrl, true, CurrentBusinessId.Value, CurrentUser.RelativeUrl);
                            try
                            {
                                toEmails = new RepositoryUserProfile().NotificationEnabledEmails(item.Email, "TSKCC");
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

                #region Upload file

                if (model.Files != null && model.Files.Count > 0)
                {
                    List<string> FilesList = new List<string>();

                    foreach (var file in model.Files)
                    {
                        string FileName = SaveFile(file.Base64, file.FileName, TaskId);
                        FilesList.Add(FileName);
                    }

                    bool isImagesSaved = new RepositoryTask().SaveFiles(FilesList, TaskId, model.TaskId > 0);
                }

                #endregion

                response = new RepositoryTask().GetTaskById(TaskId, CurrentUserId, CurrentBusinessId, true);

                return Ok<DataResponse>(response);
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
        }

        [HttpPost]
        [Route("togglestatus/{id}")]
        public IHttpActionResult ToggleStatus(int id)
        {
            string[] taskDeletePrivileges = { "DLTALLINTSK" };
            var repository = new RepositoryTask();
            var response = repository.ToggleStatus(id, CurrentUserId, HasRight(taskDeletePrivileges));
            return Ok<DataResponse>(response);
        }

        [HttpPost]
        [Route("togglepriority/{id}")]
        public IHttpActionResult TogglePriority(int id)
        {
            var repository = new RepositoryTask();
            var response = repository.TogglePriority(id, CurrentUserId);
            response.Message = "success";
            response.Status = DataResponseStatus.OK;
            return Ok<DataResponse>(response);
        }

        [HttpPost]
        [Route("setstatus/{taskid}/{statusid}")]
        public IHttpActionResult SetTaskStatus(int taskid, int statusid)
        {
            var repository = new RepositoryTask();
            var UpdatedByName = CurrentUser.FirstName + " " + CurrentUser.LastName;
            var response = repository.SetStatus(taskid, statusid, CurrentUserId, UpdatedByName);

            SendEmailNotification(ConfigurationManager.AppSettings["BaseUrl"] + CurrentUser.BusinessName.Replace(" ", "-") + "#/tasks/", taskid);

            response.Status = DataResponseStatus.OK;
            return Ok<DataResponse>(response);
        }

        [HttpPost]
        [Route("delete/{taskid}")]
        [Route("{taskid}/delete")]
        public IHttpActionResult ToggleActiveStatus(int taskid)
        {
            var repository = new RepositoryTask();
            string[] taskDeletePrivileges = { "DLTALLINTSK" };
            var response = repository.ToggleStatus(taskid, CurrentUserId, HasRight(taskDeletePrivileges));

            SendEmailNotification(null, taskid);

            response.Message = "Success";
            response.Status = DataResponseStatus.OK;
            return Ok<DataResponse>(response);
        }


        [HttpPost]
        [Route("deletefile/{id}")]
        public IHttpActionResult DeleteFile(int id)
        {
            var repository = new RepositoryTask();
            var response = repository.DeleteFile(id);
            return Ok<DataResponse>(response);
        }

        [HttpPost]
        [Route("saveduedate")]
        public IHttpActionResult SaveDueDate(TaskDates tarskDates)
        {
            var repository = new RepositoryTask();
            var response = repository.SaveDueDate(tarskDates.TargetDate, tarskDates.TaskId, CurrentUserId);
            return Ok<DataResponse>(response);
        }

        [HttpPost]
        [Route("savealertdate")]
        public IHttpActionResult SaveAlertDate(TaskDates tarskDates)
        {
            var repository = new RepositoryTask();
            var response = repository.SaveAlertDate(tarskDates.AlertDate, tarskDates.TaskId, CurrentUserId);
            return Ok<DataResponse>(response);
        }

        public class TaskDates
        {
            public int TaskId { get; set; }

            public DateTime TargetDate { get; set; }

            public DateTime AlertDate { get; set; }
        }

        /// <summary>
        /// Save file to the Task folder
        /// </summary>
        /// <param name="base64String">Base64 String</param>
        /// <param name="fileName">Name of uploaded file with extension</param>
        /// <param name="taskId">TaskId generated in the Database</param>
        /// <returns></returns>
        private string SaveFile(string base64String, string fileName, int taskId)
        {
            string rootPath = HttpContext.Current.Server.MapPath("~/Assets"),
                //fileNameWithoutExt = string.Format("{0:yyyyMMddhhmmssfff}", DateTime.Now),
                fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName),
                extension = Path.GetExtension(fileName);
            fileName = string.Format("{0}{1}", fileNameWithoutExt, extension);
            string fileDirectory = Path.Combine(rootPath, CurrentBusinessId.Value.ToString(), "Task", taskId.ToString());
            if (!Directory.Exists(fileDirectory))
                Directory.CreateDirectory(fileDirectory);
            string fullPath = Path.Combine(fileDirectory, fileName);

            int count = 1;
        isExist:
            if (File.Exists(fullPath))
            {
                fileName = string.Format("{0}{1}{2}", fileNameWithoutExt, count, extension);
                fullPath = Path.Combine(fileDirectory, fileName);
                count++;
                goto isExist;
            }

            var bytes = Convert.FromBase64String(base64String);
            using (var ms = new MemoryStream(bytes))
            {
                using (var fs = new FileStream(fullPath, FileMode.Create))
                {
                    ms.WriteTo(fs);
                    ms.Close();
                }
            }
            return fileName;
        }

        private void SendEmailNotification(string returnUrl, int taskId)
        {
            var repository = new RepositoryTask(); var objTask = repository.GetTaskById(taskId, CurrentUserId, CurrentBusinessId);
            EntityTask model = objTask.Model;
            var CreatedByName = CurrentUser.FirstName + " " + CurrentUser.LastName;
            var ReturnUrl = ConfigurationManager.AppSettings["BaseUrl"] + CurrentUser.BusinessName.Replace(" ", "-") + "#/tasks/" + model.ReferenceNumber;
            var rootPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
            var Subject = "Task " + model.ReferenceNumber + " - " + model.Subject + "";
            var mail = new GMEmail();
            string toEmails = null,
                practiceName = null,
                priorityType = null,
                targetDate = null,
                status = null;
            if (model.PracticeId.HasValue)
                practiceName = new RepositoryLookups().GetPracticeNameById(model.PracticeId.Value);
            if (model.PriorityTypeId.HasValue)
                priorityType = EnumHelper.GetEnumName<TaskPriorities>(model.PriorityTypeId.Value);
            targetDate = model.TargetDate.HasValue ? model.TargetDate.ToString() : "Not Set";
            var AssignedUsers = string.Join(",", model.AssignedUsersList.Select(a => a.Name));
            if (!string.IsNullOrEmpty(returnUrl))
                returnUrl += model.ReferenceNumber;

            if (model.StatusId.HasValue)
                status = Regex.Replace(EnumHelper.GetEnumName<TaskStatuses>(model.StatusId.Value), "([A-Z]{1,2}|[0-9]+)", " $1").Trim();

            if (CurrentUserId != model.RequestedUser.UserId)
            {
                var emailBody = TemplateManager.UpdateOrDeleteTask(rootPath, model.RequestedUser.Name, null, CreatedByName, model.Subject,
                    targetDate, model.TaskDescription, practiceName, priorityType, status, ReturnUrl, false, CurrentBusinessId.Value, CurrentUser.RelativeUrl);
                mail.SendDynamicHTMLEmail(model.RequestedUser.Email, Subject, emailBody, CurrentUser.OtherEmails);
            }

            foreach (var item in model.AssignedUsersList)
            {
                if (item.UserId == CurrentUserId)
                    continue;

                toEmails = new RepositoryUserProfile().NotificationEnabledEmails(item.Email, "TSKSTATNFN");
                if (!string.IsNullOrEmpty(toEmails))
                {
                    var emailBody = TemplateManager.UpdateOrDeleteTask(rootPath, item.Name, null, CreatedByName, model.Subject,
                        targetDate, model.TaskDescription, practiceName, priorityType, status, ReturnUrl, false, CurrentBusinessId.Value, CurrentUser.RelativeUrl);
                    mail.SendDynamicHTMLEmail(toEmails, Subject, emailBody, CurrentUser.OtherEmails);
                }
            }

            foreach (var item in model.WatchersList)
            {
                toEmails = new RepositoryUserProfile().NotificationEnabledEmails(item.Email, "TSKSTATNFN");
                if (!string.IsNullOrEmpty(toEmails))
                {
                    var emailBody = TemplateManager.UpdateOrDeleteTask(rootPath, item.Name, AssignedUsers, CreatedByName, model.Subject,
                        targetDate, model.TaskDescription, practiceName, priorityType, status, ReturnUrl, true, CurrentBusinessId.Value, CurrentUser.RelativeUrl);
                    mail.SendDynamicHTMLEmail(toEmails, Subject, emailBody, CurrentUser.OtherEmails);
                }
            }
        }
    }
}
