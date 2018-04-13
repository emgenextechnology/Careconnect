using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBP.Business.Entity.EntityNotificationSettings
{
   public class EntityNotificationSettings
    {
        public int UserId { get; set; }
        public int NotificationTypeId { get; set; }
        public string NotificationType { get; set; }
        public bool Status { get; set; }
    }
}
