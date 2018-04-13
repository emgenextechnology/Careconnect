using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBP.NotificationApp.Entity.User
{
    public class EntityProfile
    {
        public DateTime CreatedOn = DateTime.UtcNow;
        public DateTime? UpdatedOn = DateTime.UtcNow;
        public int Id { get; set; }
        public int? UserId { get; set; }
        public DateTime? Startdate { get; set; }
        public string WorkEmail { get; set; }
        public string HomePhone { get; set; }
        public string AdditionalPhone { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public int? StateId { get; set; }
        public string Zip { get; set; }
        public int? NotificationCount { get; set; }
        public string DeviceId { get; set; }
        public DateTime? LastLoggedInTime { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public List<string> RoleIds { get; set; }
        public List<int> DepartmentIds { get; set; }
    }
}
