using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EmgenexBusinessPortal.Models
{
    public class VMNoteModel
    {

        public int Id { get; set; }

        public int ParentTypeId { get; set; }

        public int ParentId { get; set; }

        public string Description { get; set; }
    }

}