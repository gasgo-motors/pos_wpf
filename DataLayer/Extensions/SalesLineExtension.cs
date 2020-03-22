using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Models;

namespace DataLayer
{
    public partial class SalesLine : ISalesLine
    {
        public  string OrderTypeString
        {
            get
            {
                if (OrderType == 1) return "Q";
                if (OrderType == 2) return "SP";
                if (OrderType == 3) return "CQ";
                return "";
            }
        }

        public DateTime? PostingDate { get; set; }
    }

    public partial class PostedSalesLine : ISalesLine
    {
        public string OrderTypeString
        {
            get
            {
                if (OrderType == 1) return "Q";
                if (OrderType == 2) return "SP";
                if (OrderType == 3) return "CQ";
                return "";
            }
        }
        public DateTime? PostingDate { get; set; }
    }

    public partial class ReleasedSalesLine : ISalesLine
    {
        public bool IsNew { get; set; }
        public string OrderTypeString
        {
            get
            {
                if (OrderType == 1) return "Q";
                if (OrderType == 2) return "SP";
                if (OrderType == 3) return "CQ";
                return "";
            }
        }

        public decimal? QuantityF2
        {
            get { return Quantity.HasValue ? (decimal?)Math.Round(Quantity.Value, 2) : null; }
        }

        public decimal? UnitPriceF2
        {
            get { return UnitPrice.HasValue ? (decimal?)Math.Round(UnitPrice.Value, 2) : null; }
        }

        public decimal? AmountIncludingVATF2
        {
            get { return AmountIncludingVAT.HasValue ? (decimal?)Math.Round(AmountIncludingVAT.Value, 2) : null; }
        }

        public DateTime? PostingDate { get; set; }

    }
}
