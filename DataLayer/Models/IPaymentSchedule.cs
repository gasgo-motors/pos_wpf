using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models
{
    public interface  IPaymentSchedule
    {
         int EntryNo { get; set; }
         Nullable<int> EntryType { get; set; }
         string CustomerNo { get; set; }
         string DocumentNo { get; set; }
         Nullable<System.DateTime> Date { get; set; }
         Nullable<decimal> Amount { get; set; }
         Nullable<decimal> RemeiningAmount { get; set; }
    }
}
