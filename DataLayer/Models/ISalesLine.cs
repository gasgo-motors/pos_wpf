using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models
{
    public interface ISalesLine
    {
         string DocumentNo_ { get; set; }
         int DocumentType { get; set; }
         int LineNo_ { get; set; }
         Nullable<int> Type { get; set; }
         string No_ { get; set; }
         string No2 { get; set; }
         string Description { get; set; }
         string LargeDescription { get; set; }
         Nullable<decimal> Quantity { get; set; }
         Nullable<decimal> UnitPrice { get; set; }
         string Sell_toCustomerNo { get; set; }
         string LocationCode { get; set; }
         Nullable<decimal> Inv_DiscountAmount { get; set; }
         Nullable<decimal> LineDiscountPercent { get; set; }
         Nullable<decimal> LineDiscountAmount { get; set; }
         Nullable<decimal> Amount { get; set; }
         Nullable<decimal> AmountIncludingVAT { get; set; }
         Nullable<decimal> GrossWeight { get; set; }
         Nullable<decimal> NetWeight { get; set; }
         Nullable<decimal> UnitVolume { get; set; }
         string UnitOfMeasureCode { get; set; }
         Nullable<decimal> QuantityBase { get; set; }
         string ResponsibilityCenter { get; set; }
         string ItemCategoryCode { get; set; }
         string ProductGroupCode { get; set; }
         string ProdSubGroupCode { get; set; }
         Nullable<int> OrderType { get; set; }
         string ModifiedUser { get; set; }
         Nullable<System.DateTime> ModifiedDate { get; set; }

        string OrderTypeString { get;  }


         string Service_Provider { get; set; }
         string Service_Provider_Name { get; set; }
         string Customer_Vehicle { get; set; }
    }
}
