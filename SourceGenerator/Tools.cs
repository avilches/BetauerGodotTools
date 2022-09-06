using System;
using System.Linq;

namespace Generator {
    public static class Tools {
        public static string CamelCase(this string name) =>
            name.Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => char.ToUpperInvariant(s[0]) + s.Substring(1, s.Length - 1))
                .Aggregate(string.Empty, (s1, s2) => s1 + s2);
    }
}