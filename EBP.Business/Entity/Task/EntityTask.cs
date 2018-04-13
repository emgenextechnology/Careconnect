using EBP.Business.Entity.Practice;
using EBP.Business.Enums;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EBP.Business.Entity.Task
{
    public class EntityTask
    {

        public DateTime CreatedOn = DateTime.UtcNow;
        public DateTime? UpdatedOn = DateTime.UtcNow;

        public bool IsApiCall { get; set; }

        private string FilePath
        {
            get
            {
                if (IsApiCall)
                    return CurrentBusinessId != null && TaskId > 0 ?
                        string.Format("{0}/{1}/{2}/{3}/{4}", ConfigurationManager.AppSettings["PortalUrl"] ?? "http://api.careconnect.com", "Assets", CurrentBusinessId.ToString(), "Task", TaskId.ToString()) :
                        string.Empty;
                return CurrentBusinessId != null && TaskId > 0 ?
                    string.Format("{0}/{1}/{2}/{3}/{4}", ConfigurationManager.AppSettings["BaseUrl"] ?? "http://api.careconnect.com", "Assets", CurrentBusinessId.ToString(), "Task", TaskId.ToString()) :
                    string.Empty;
            }
        }

        public int TaskId { get; set; }

        public int CurrentUserId { get; set; }

        public int? CurrentBusinessId { get; set; }

        [Required(ErrorMessage = "Subject is required.")]
        public String Subject { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string TaskDescription { get; set; }

        public string ShortDescription { get { return StripHTML(TaskDescription); } }

        public int? TaskRequestTypeId { get; set; }

        public DateTime? ClosingDate { get; set; }

        public int? UpdatedBy { get; set; }

        public int? PriorityTypeId { get; set; }

        public string PriorityType { get; set; }

        public string ReferenceNumber { get; set; }

        public int? PracticeId { get; set; }

        public DateTime? TargetDate { get; set; }

        public bool IsActive { get; set; }

        public bool IsToday
        {
            get
            {
                return this.TargetDate.HasValue ? this.TargetDate.Value.Date == DateTime.Now.Date : false;
            }
        }

        public int CreatedBy { get; set; }

        public string CreatedByName { get; set; }

        public string UpdatedByName { get; set; }

        public string TaskType { get; set; }

        public UserDetails RequestedUser { get; set; }

        public int? StatusId { get; set; }

      //  public int TaskStatusId { get; set; }
        public string Status { get; set; }

        public int? PracticeSpecialityTypeId { get; set; }

        public DateTime? AlertDate { get; set; }

        private string alertMessage;
        public string AlertMessage { get { return TaskId > 0 ? string.Format("Reminder on Task: HLP{0} - {1}", ReferenceNumber, Subject) : alertMessage; } set { alertMessage = value; } }

        public string PracticeName { get; set; }

        public bool IsMeWatcher
        {
            get
            {
                return (WatchersList != null ? WatchersList.Any(a => a.UserId == CurrentUserId) : false) && (AssignedUsersList != null ? AssignedUsersList.Count(a => a.UserId == CurrentUserId) == 0 && RequestedUser.UserId != CurrentUserId : false);
            }
        }

        public bool HasDeleteRight { get; set; }

        [Required(ErrorMessage = "Please specify the recipient.")]
        public int AssignedTo { get; set; }

        public List<FilesUploaded> Files { get; set; }

        private IEnumerable<FilesUploaded> filesList { get; set; }
        public IEnumerable<FilesUploaded> FilesList
        {
            get
            {
                try
                {
                    return filesList != null && filesList.Count() > 0 ? filesList.Select(a => new FilesUploaded
                    {
                        Id = a.Id,
                        FilePath = string.Format("{0}/{1}", FilePath, a.FileName),
                        FileType = Path.GetExtension(a.FileName.Replace(".", "")),
                        FileName = a.FileName
                    }) : null;
                }
                catch (Exception ex)
                {
                    ex.Log();
                    return null;
                }
            }
            set
            {
                filesList = value;
            }
        }

        public IEnumerable<string> Watchers { get; set; }

        public IEnumerable<UserDetails> AssignedUsersList { get; set; }

        public IEnumerable<UserDetails> WatchersList { get; set; }

        public ICollection<Database.TaskItemOrder> TaskItemOrders { get; set; }

        public string ReturnUrl { get; set; }

        public string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>|&nbsp;", String.Empty);
        }
    }

    public class UserDetails
    {
        public int UserId { get; set; }

        public string Name { get; set; }

        public string ProfilePhoto { get; set; }

        public string Email { get; set; }
    }

    public class FilesUploaded
    {
        public int Id { get; set; }

        public string Base64 { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public int FileSize { get; set; }

        public string FileType { get; set; }

    }

    public class EntityTaskLog
    {
        public DateTime CreatedOn { get; set; }

        public string ReferenceNumber { get { return TaskId > 0 ? string.Format("HLP{0}", TaskId.ToString().PadLeft(8, '0')) : null; } }

        public string Subject { get; set; }

        public int? TaskId { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public IEnumerable<EntityTaskHistory> TaskLogList { get; set; }
    }

    public class EntityTaskHistory
    {

        public string Action { get; set; }

        public DateTime? AlertDate { get; set; }

        public string AssignedToList { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public int? StatusId { get; set; }

        public string Status { get { return EnumHelper.GetEnumName<TaskStatuses>(StatusId.Value); } }

        public string Subject { get; set; }

        public string Description { get; set; }

        public string PracticeName { get; set; }

        public DateTime? TargetDate { get; set; }

        public string WatchersList { get; set; }

    }

    public class TaskDates
    {
        public int TaskId { get; set; }

        public DateTime TargetDate { get; set; }

        public DateTime AlertDate { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ListRequired : RequiredAttribute
    {
        public override bool IsValid(object value)
        {
            var list = value as IEnumerable;
            return list != null && list.GetEnumerator().MoveNext();
        }
    }
}
