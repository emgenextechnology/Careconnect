using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EBP.Business.Entity.Practice
{
    public class EntityPracticeSpeciality
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)] 
        public int Id { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)] 
        public int PracticeId { get; set; }
        public int PracticeSpecialityId { get; set; }
        public string NewSpecialityName { get; set; }
    }
}
