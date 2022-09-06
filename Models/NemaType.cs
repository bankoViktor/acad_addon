using System;
using System.Collections.Generic;
using System.Linq;

namespace Addon.Models
{
    internal static class NemaType
    {
        public static IEnumerable<string> Values => new string[]
        {
             "1",
             "2",
             "3",
             "3R",
             "3S",
             "4",
             "4X",
             "5",
             "6",
             "6P",
             "12",
             "13",
        };

        public static bool IsValid(string value) => !string.IsNullOrWhiteSpace(value) &&
            Values.Any(v => v.Equals(value, StringComparison.InvariantCultureIgnoreCase));
    }
}
