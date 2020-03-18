using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models
{
    public  interface IGenJournalLine
    {
         string TemplateName { get; set; }
         string BatchName { get; set; }
         int LineNo_ { get; set; }
         Nullable<System.DateTime> PostingDate { get; set; }
         Nullable<int> DocumentType { get; set; }
         string DocumentNo { get; set; }
         Nullable<int> AccountType { get; set; }
         string AccountNo_ { get; set; }
         string Description { get; set; }
         Nullable<int> Bal_AccountType { get; set; }
         string Bal_AccountNo_ { get; set; }
         Nullable<decimal> Amount { get; set; }
         string CurrencyCode { get; set; }
         Nullable<decimal> CurrencyFactor { get; set; }
         string Salespers__Purch_Code { get; set; }
         string ResponsibilityCenter { get; set; }
         string PlasticCardNo { get; set; }
         string PresentCardNo { get; set; }
         string PaymentMethodCode { get; set; }
         string PaymentMethodCodeGeo { get; set; }
         Nullable<System.Guid> ModifiedUserID { get; set; }
         Nullable<System.DateTime> ModifiedDate { get; set; }

         Nullable<bool> IsSent { get; set; }

         string AccountTypeName { get; }
        string BalAccountTypeName { get;  }

        bool IsNew { get; set; }

        bool? IsGeneral { get; set; }
    }
}
