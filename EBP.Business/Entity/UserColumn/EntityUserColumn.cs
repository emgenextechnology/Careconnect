using EBP.Business.Entity.Practice;
using EBP.Business.Enums;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EBP.Business.Entity.UserColumn
{
    public class EntityUserColumn
    {
        public int Id { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public int UserId { get; set; }

        public int? ServiceId { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public int? BusinessId { get; set; }

        public int ModuleId { get; set; }

        public string ColumnName { get; set; }

        public bool IsChecked { get; set; }

        public int CreatedBy { get; set; }

    }
}
