using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Betauer.Reflection;
using Godot;
using File = System.IO.File;

namespace Betauer {
    public static class StringTools {
        public static string HumanReadableBytes(this long numBytes) {
            return numBytes < 0 ? 
                $"-{_PositiveHumanReadableBytes(Math.Abs(numBytes))}" :
                _PositiveHumanReadableBytes(numBytes);
        }

        private static string _PositiveHumanReadableBytes(long numBytes) {
            if (numBytes == 0) return "0 B";
            
            if (numBytes < 1024)
                return $"{numBytes.ToString()} B";

            if (numBytes < 1048576)
                return $"{numBytes / 1024d:0.##} KB";

            if (numBytes < 1073741824)
                return $"{numBytes / 1048576d:0.##} MB";

            if (numBytes < 1099511627776)
                return $"{numBytes / 1073741824d:0.##} GB";

            if (numBytes < 1125899906842624)
                return $"{numBytes / 1099511627776d:0.##} TB";

            if (numBytes < 1152921504606846976)
                return $"{numBytes / 1125899906842624d:0.##} PB";

            return $"{numBytes / 1152921504606846976d:0.##} EB";
        }
    }
}