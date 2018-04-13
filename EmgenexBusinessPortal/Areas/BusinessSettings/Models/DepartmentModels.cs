using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmgenexBusinessPortal.Areas.BusinessSettings.Models
{
    public class VMDepartment
    {
        // GET: BusinessSettings/VMDepartment
        public int Id { get; set; }
        [Required]
        public string DepartmentName { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public List<int> DepartmentPrivilegeIds { get; set; }
    }
}