using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmgenexBusinessPortal.Areas.Business.Models
{
    public class RepGroupModel
    {
        // GET: Business/RepGroupModel
        public int Id { get; set; }
        [Required]
        public string RepGroupName { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [Display (Name= "Manager")]
        public List<int?> ManagerIds { get; set; }

        public bool IsActive { get; set; }

        public int? SalesDirectorId { get; set; }
    }
    public class RepGroupViewModel
    {

    }
}