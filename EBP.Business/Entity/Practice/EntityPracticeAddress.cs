using EBP.Business.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EBP.Business.Entity.Practice
{
    public class EntityPracticeAddress : EntityPracticeContact
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public int Id { get; set; }

        public string PracticeName { get; set; }

        public string LocationId { get; set; }

        public int AddressIndex { get; set; }

        public int AddressTypeId { get; set; }

        string _addressType;
        public string AddressType
        {
            get
            {
                if (!string.IsNullOrEmpty(_addressType))
                    return _addressType;

                if (AddressTypeId > 1)
                    return EnumHelper.GetEnumName<AddressType>(AddressTypeId);

                return "";
            }
            set
            {
                _addressType = value;
            }
        }


        public int TypeId { get; set; }

        public string Line1 { get; set; }

        public string Line2 { get; set; }

        public string City { get; set; }

        public int StateId { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public string Fax { get; set; }

        public IEnumerable<EntityPracticePhone> Phone { get; set; }
    }
}
