using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common
{
    public static class StringExtensions
    {
        public static string? Truncate(this string? value, int maxLength)
        {
            return value?.Length > maxLength
                ? value.Substring(0, maxLength)
                : value;
        }
    }
}
