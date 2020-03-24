using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public partial class Customer
    {
        public decimal? PaymentSchedule { get; set; }

        public bool CurrentDatePaymentSchedule
        {
            get { return PaymentSchedule > 0; }
        }

        public bool CustomerPostingGroupIChecked { 
            get
            {
                return this.Customer_Posting_Group != "CUSTF_LTD";
            }
            set
            {
                this.Customer_Posting_Group =  value ? "CUSTL_LTD" : "CUSTF_LTD";
            }
        }

        public string Address1 { get; set; }

        public string City1 { get; set; }
    }
}
