using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class RemainingItemEntry
    {
        public string OrderNo { get; set; }
        public string ItemNo { get; set; }
        public string ItemDesc { get; set; }
        public decimal RequestedQuantity { get; set; }
        public decimal RemainingQuantity { get; set; }
    }
}
