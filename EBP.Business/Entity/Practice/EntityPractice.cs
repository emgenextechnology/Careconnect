using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EBP.Business.Entity.Practice
{
    public class EntityPractice
    {
        public EntityPractice()
        {
            Providers = new List<EntityProvider>();
            Address = new List<EntityPracticeAddress>();
        }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public int Id { get; set; }

        public string Name { get; set; }

        public int? SpecialityTypeId { get; set; }

        public string KeyWord { get; set; }

        //public string Fax { get; set; }

        public string Email { get; set; }

        public string ReportDeliveryEmail { get; set; }

        public string SpecialityType { get; set; }

        public int? ContactPreferenceId { get; set; }

        public string ReportDeliveryFax { get; set; }
        
        public IEnumerable<EntityPracticeSpeciality> Specialities { get; set; }

        public IEnumerable<EntityProvider> Providers { get; set; }

        public IEnumerable<EntityPracticeAddress> Address { get; set; }

        public IEnumerable<EntityPracticeContact> Contact { get; set; }
    }
}
