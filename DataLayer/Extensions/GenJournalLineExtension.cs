using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Models;

namespace DataLayer
{
    public partial class GenJournalLine : IGenJournalLine
    {
        public bool IsNew { get; set; }

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

    public partial class PostedGenJournalLine : IGenJournalLine
    {
        public bool IsNew { get; set; }
        public bool? IsGeneral { get; set; }
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

    public partial class ReleasedGenJournalLine : IGenJournalLine
    {
        public bool IsNew { get; set; }
        public bool? IsGeneral { get; set; }
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
