using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmgenexBusinessPortal.Areas.Business.Models
{
    public class ServiceModel
    {
        // GET: Business/ServiceModel
        public int ServiceId { get; set; }

        public bool Status { get; set; }
    }
    public class EnrolledServiceModel
    {
        public int Id { get; set; }
        public int BusinessId { get; set; }
        [Required]
        public string ServiceName { get; set; }
        [Required]
        public string ServiceDecription { get; set; }
        public bool? IsActive { get; set; }
        public bool? Status { get; set; }
        public string ServiceColor { get; set; }
        public int? ImportMode { get; set; }
        public string BoxUrl { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public int? OldId { get; set; }

        public int? ServiceId { get; set; }
        public int? Protocol { get; set; }
        public string HostName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int? PortNumber { get; set; }
        public string RemotePath { get; set; }
    }
}