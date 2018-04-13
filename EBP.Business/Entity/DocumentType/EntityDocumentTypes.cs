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

namespace EBP.Business.Entity.DocumentType
{
    public class EntityDocumentTypes
    {
        public DateTime CreatedOn = DateTime.UtcNow;
        public DateTime? UpdatedOn = DateTime.UtcNow;
        public int Id { get; set; }
        public string DocumentType { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public int? BusinessId { get; set; }
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }
    }
}
