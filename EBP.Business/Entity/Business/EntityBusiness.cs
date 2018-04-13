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

namespace EBP.Business.Entity.Business
{
    public class EntityBusiness
    {
        public string BusinessName { get; set; }

        public string DomainUrl { get; set; }

        public string RelativeUrl { get; set; }
    }
    public class EntityBusinessProfile
    {

        public DateTime CreatedOn = DateTime.UtcNow;
        public DateTime? UpdatedOn = DateTime.UtcNow;
        public int Id { get; set; }
        public string BusinessName { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Logo { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string About { get; set; }
        public string RelativeUrl { get; set; }
        public string DomainUrl { get; set; }
        public bool IsActive { get; set; }
        public int? Status { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public string CreatedUser { get; set; }
        public string Updateduser { get; set; }
        public int? DateRange { get; set; }
        public int? SalesGroup { get; set; }
        public string OtherEmails { get; set; }
        public int? BusinessId { get; set; }

        public string LogoPath
        {
            get
            {
                return
                   string.Format("{0}/Assets/{1}/Logo_{1}.jpg", ConfigurationManager.AppSettings["BaseUrl"] ?? ConfigurationManager.AppSettings["BaseUrl"] ?? "https://crm.careconnectsystems.com", BusinessId);

            }
        }     

    }
}
