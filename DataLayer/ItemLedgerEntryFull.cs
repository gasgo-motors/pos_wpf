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
    
    public partial class ItemLedgerEntryFull
    {
        public int Id { get; set; }
        public int DocumentType { get; set; }
        public int EntryType { get; set; }
        public string ItemNo { get; set; }
        public bool BaseInvoiceNoIsC { get; set; }
        public string LocationCode { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public Nullable<System.DateTime> PostingDate { get; set; }
    }
}
