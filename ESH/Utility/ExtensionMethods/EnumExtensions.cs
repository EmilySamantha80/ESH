using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESH.Utility.ExtensionMethods
{
    public static class EnumExtensions
    {
        public static string ValueString(this Enum value)
        {
            return System.Convert.ToInt32(value).ToString();
        }
    }
}
