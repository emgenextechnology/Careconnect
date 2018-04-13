using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmgenexBusinessPortal.Models
{
    public class NotificationsettingModel
    {
           public string Name { get; set; }
           //public IQueryable<SelectListItem> Notifications { get; set; }
           public IQueryable<NotificationModel> Notifications { get; set; }

           public int UserId { get; set; }
    }
    public class NotificationModel
    { 
        public int UserId { get; set; }
        public int NotificationTypeId { get; set; }
        public string NotificationType { get; set; }
        public bool Status { get; set; }
    }
}