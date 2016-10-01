using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAIS.Helpers
{
    public static class Extensions
    {
        public static bool CompareAndEquals(this string self, params string[] compareTo)
        {
            foreach (var item in compareTo)
            {
                if (self.Equals(item, StringComparison.CurrentCultureIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}