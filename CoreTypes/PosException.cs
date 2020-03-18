using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreTypes
{
    public class PosException : Exception
    {
        public PosException(string message) : base(message)
        {

        }
    }
}
