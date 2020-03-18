using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreTypes
{
    public abstract class Singleton<T> where T : new()
    {
        private static readonly Lazy<T> lazy =
            new Lazy<T>(() => new T());

        public static T Current { get { return lazy.Value; } }
    }
}
