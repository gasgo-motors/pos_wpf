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
    
    public partial class Customer
    {
        public string No_ { get; set; }
        public string Name { get; set; }
        public Nullable<int> Type { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country_RegionCode { get; set; }
        public string PostCode { get; set; }
        public string Contact { get; set; }
        public string PhoneNo_ { get; set; }
        public Nullable<decimal> CreditLimit_LCY_ { get; set; }
        public string CustomerPriceGroup { get; set; }
        public string VATRegistrationNo_ { get; set; }
        public string PaymentTermsCode { get; set; }
        public string InvoiceDisc_Group { get; set; }
        public string PaymentMethodCode { get; set; }
        public Nullable<decimal> Balance { get; set; }
        public Nullable<bool> PriceIncludingVAT { get; set; }
        public string E_Mail { get; set; }
        public string VendorNo_ { get; set; }
        public Nullable<bool> NeedsVATInvoice { get; set; }
        public Nullable<bool> IsNewCustomer { get; set; }
        public string VisitWeekDays { get; set; }
        public Nullable<decimal> SalesBudgetAmount { get; set; }
        public Nullable<decimal> SalesActualAmount { get; set; }
        public Nullable<decimal> RecommendedSalesAmount { get; set; }
        public Nullable<int> RecommendedVisitsMonth { get; set; }
        public Nullable<System.Guid> ModifiedUserID { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string FullName { get; set; }
        public string ShipToAddress { get; set; }
        public string Mobile_ { get; set; }
        public string AreaCode { get; set; }
        public string Customer_Posting_Group { get; set; }
    }
}
