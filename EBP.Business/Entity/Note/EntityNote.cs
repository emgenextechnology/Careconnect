using EBP.Business.Entity.Practice;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EBP.Business.Entity.Note
{
    public class EntityNote
    {
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public int Id { get; set; }

        public string Description { get; set; }

        public int ParentTypeId { get; set; }

        public int ParentId { get; set; }
        public int? BusinessId { get; set; }
        public string CreatedByName { get; set; }

        public int CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public UserDetails UserDetails { get; set; }

        public bool IsDayAgo
        {
            get
            {
                return CreatedOn < DateTime.UtcNow.AddHours(-24);
            }
        }
    }
    public class UserDetails
    {
        public int UserId { get; set; }

        public string Name { get; set; }

        public string ProfilePhoto
        {
            get
            {
                return
                    string.Format("{0}/{1}/{2}/{3}/{4}.jpg", ConfigurationManager.AppSettings["BaseUrl"] ?? "http://api.careconnect.com/", "Assets", BusinessId.ToString(), "Users", UserId.ToString());
            }
        }

        public string date { get; set; }

        public int BusinessId { get; set; }
    }
}
