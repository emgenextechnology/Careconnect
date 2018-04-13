using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EBP.Api.Models
{
    public class VMNoteModel
    {

        public int Id { get; set; }

        public int ParentTypeId { get; set; }

        public int ParentId { get; set; }

        public string Description { get; set; }
        public string Datestring { get; set; }
        
    }

}