using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Extensions
{
    public class GenJouranlView
    {

        public string AccountNo_ { get; set; }

        public string Bal_AccountNo_ { get; set; }
        public string Bal_AccountName { get; set; }
        public string PaymentMethodCode { get; set; }
        public string DocumentNo { get; set; }

        public int LineNo { get; set; }
        public Nullable<System.DateTime> PostingDate { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<int> AccountType { get; set; }
        public Nullable<int> Bal_AccountType { get; set; }

        public Nullable<System.DateTime> PostingDateReal { get; set; }

        public bool IsHeaden { get; set; }

        public string AccountTypeName
        {
            get
            {
                switch (AccountType)
                {
                    case 0:
                        return "G/L Account";
                    case 1:
                        return "Customer";
                    case 2:
                        return "Vendor";
                    case 3:
                        return "Bank Account";
                }
                return "";
            }
        }

        public string BalAccountTypeName
        {
            get
            {
                switch (Bal_AccountType)
                {
                    case 0:
                        return "G/L Account";
                    case 1:
                        return "Customer";
                    case 2:
                        return "Vendor";
                    case 3:
                        return "Bank Account";
                }
                return "";
            }
        }

    }
}
