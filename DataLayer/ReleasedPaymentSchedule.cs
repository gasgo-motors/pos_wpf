//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataLayer
{
    using System;
    using System.Collections.Generic;
    
    public partial class ReleasedPaymentSchedule
    {
        public int EntryNo { get; set; }
        public Nullable<int> EntryType { get; set; }
        public string CustomerNo { get; set; }
        public string DocumentNo { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> RemeiningAmount { get; set; }
        public Nullable<System.Guid> ModifiedUserID { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }
}
