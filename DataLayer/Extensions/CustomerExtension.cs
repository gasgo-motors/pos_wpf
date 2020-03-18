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

        public string Address1 { get; set; }

        public string City1 { get; set; }
    }
}
