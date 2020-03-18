using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    partial class PostCodeCity
    {
        public string PostCodeAndCity
        {
            get { return string.Format("{0} - {1}", this.PostCode, this.City); }
        }
    }
}
