//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EBP.DataMigration.Database.Old
{
    using System;
    using System.Collections.Generic;
    
    public partial class Lead
    {
        public int LeadId { get; set; }
        public int PracticeId { get; set; }
        public int LeadSource { get; set; }
        public string LeadServiceIntrest { get; set; }
        public Nullable<bool> Status { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public string OtherLeadSource { get; set; }
    }
}
