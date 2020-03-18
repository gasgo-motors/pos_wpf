using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public partial class RegionBudgetActual
    {
        public decimal Percent
        {
            get
            {
                return BudgetAmount == 0 ? 0m : (Math.Round(((ActualAmount.Value) / BudgetAmount.Value), 2, MidpointRounding.AwayFromZero))*100m;
            }
        }
    }
}
