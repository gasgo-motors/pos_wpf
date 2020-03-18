using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models
{
     public interface ISalesHeader
    {
         string No_ { get; set; }
         int DocumentType { get; set; }
         Nullable<System.DateTime> DueDate { get; set; }
         Nullable<System.DateTime> PostingDate { get; set; }
         Nullable<System.DateTime> ShipmentDate { get; set; }
         string Sell_toCustomerNo { get; set; }
         string Sell_toCustomerName { get; set; }
         string Sell_toAddress { get; set; }
         string Sell_toCity { get; set; }
         string Sell_toPostCode { get; set; }
         string Sell_toCountry { get; set; }
         string Ship_toAddress { get; set; }
         string Ship_toAddressCode { get; set; }
         string Ship_toCity { get; set; }
         string Ship_toCountry { get; set; }
         Nullable<int> Status { get; set; }
         string PaymentTermsCode { get; set; }
         string PaymentMethodCode { get; set; }
         string ShipmentMethodCode { get; set; }
         string CurrencyCode { get; set; }
         Nullable<decimal> CurrencyFactor { get; set; }
         string CustomerPriceGroup { get; set; }
         Nullable<bool> PricesIncludingVat { get; set; }
         Nullable<int> InvoiceDiscountCalc { get; set; }
         Nullable<decimal> InvoiceDiscountAmount { get; set; }
         string SalespersonCode { get; set; }
         Nullable<decimal> Amount { get; set; }
         Nullable<decimal> AmountIncludingVat { get; set; }
         string VATRegistrationNo_ { get; set; }
         string ExternalDocumentNo_ { get; set; }
         string ShippingAgentCode { get; set; }
         string ResponsibilityCenter { get; set; }
         Nullable<bool> VatInvoice { get; set; }
         Nullable<bool> Waybill { get; set; }
         string PlasticCardNo { get; set; }
         Nullable<bool> IsEmployee { get; set; }
         string Guid { get; set; }
         string ModifiedUser { get; set; }
         Nullable<System.DateTime> ModifiedDate { get; set; }
         string PostingDescription { get; set; }

        IEnumerable<ISalesLine> SalesLines { get; set; }
        IEnumerable<IPaymentSchedule> PaymentSchedules { get; set; }
        IEnumerable<IGenJournalLine> JournalLines { get; set; }

        OrderBaseTypes OrderBaseType { get; }

        Customer CurrentCustomer { get;set;}
    }
}
