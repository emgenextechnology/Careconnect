using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EBP.NotificationApp.Services
{
    public class aps
    {
        public string alert { get; set; }
        public string sound { get; set; }
        public int badge { get; set; }
    }
    public class ApplePushPayLoad
    {
        public aps aps { get; set; }
        public long NotificationId { get; set; }
        public int TargetId { get; set; }

        public int TargetType { get; set; }
    }
}