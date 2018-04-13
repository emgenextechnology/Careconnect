using EBP.Business.Entity.Practice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmgenexBusinessPortal.Models
{
    public class PDFModel 
    {
        // GET: PDFModel
        public string Content { get; set; }

        public int Id { get; set; }

        public bool IsAccount { get; set; }
    }

    public class PDFViewModel
    {

        public IEnumerable<EntityProvider> Providers { get; set; }

        public IEnumerable<EntityPracticeAddress> Address { get; set; }

        public EntityPractice Practice { get; set; }

        public string CreatedByName { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? EnrolledDate { get; set; }

        public EBP.Business.Entity.Rep.EntityRep Rep { get; set; }
    }
}