using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EBP.Business.Entity.Practice
{
    public class EntityProvider
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public int AddressIndex { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        private int? _degreeId;
        public int? DegreeId { get { return _degreeId; } set { _degreeId = value == 0 ? null : value; } }
        public string DegreeName { get; set; }
        public string NPI { get; set; }
        public string ShortCode { get; set; }
        public EntityProviderAddress Address { get; set; }
    }
}
