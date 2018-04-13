using EBP.Business.Entity.Practice;
using EBP.Business.Entity.Rep;
using EBP.Business.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EBP.Business.Entity.EntityNotificationSettings
{
    public class Notification
    {
        public int UnreadCount { get; set; }

        public EntityList<EntityNotification> NotificationList { get; set; }

    }

    public class EntityNotification
    {
        public int target;

        public int CreatedBy { get; set; }

        public string CreatedUserName { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Message { get; set; }

        public int TargetId { get; set; }

        public int UserId { get; set; }

        public int? BusinessId { get; set; }

        private bool? _value;
        public bool? IsRead
        {
            get { return _value ?? false; }
            set { _value = value; }
        }

        public int NotificationId { get; set; }

        public string TargetType { get { return EnumHelper.GetEnumName<NotificationTargetType>(target); } }

        public string ProfilePicUrl
        {
            get
            {
                return
                    string.Format("{0}/{1}/{2}/{3}/{4}.jpg", ConfigurationManager.AppSettings["BaseUrl"] ?? "", "Assets", BusinessId.HasValue ? BusinessId.ToString() : "0", "Users", CreatedBy.ToString());
            }
        }
    }

    public class EmailNotification
    {
        public string PracticeName { get; set; }

        public string PracticeAddress { get; set; }

        public string Services { get; set; }

        public string Providers { get; set; }

        public string CurrentUserFirstName { get; set; }

        public string CurrentUserLastName { get; set; }

        public string CreatedByName
        {
            get
            {
                return string.Format("{0} {1}", this.CurrentUserFirstName, this.CurrentUserLastName);
            }
        }

        public string ReturnUrl { get; set; }

        public string RepFirstName { get; set; }

        public string RepMiddleName { get; set; }

        public string RepLastName { get; set; }

        public string RepName
        {
            get
            {
                return string.Format("{0} {1} {2}", this.RepFirstName, this.RepMiddleName, this.RepLastName);
            }
        }

        public string RepEmail { get; set; }

        //public string ManagerFirstName { get; set; }

        //public string ManagerMiddleName { get; set; }

        //public string ManagerLastName { get; set; }

        //public string ManagerNames { get; set; }

        //public string ManagerEmails { get; set; }

        public string RootPath { get; set; }

        public IEnumerable<Manager> Managers { get; set; }
    }

    public class UnReadNotification
    {
        public int NotificationCount { get; set; }
    }
}
