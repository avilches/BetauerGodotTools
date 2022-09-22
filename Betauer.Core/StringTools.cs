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
            return numBytes < 0
                ? $"-{_PositiveHumanReadableBytes(Math.Abs(numBytes))}"
                : _PositiveHumanReadableBytes(numBytes);
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

        public static string FastFormatDateTime(DateTime dateTime, char dateSep = '-') {
            return string.Create(23, dateTime, (chars, dt) => {
                //yyyy-mm-dd hh:hh:hh:hhh
                Write4Chars(chars, 0, dt.Year);
                chars[4] = dateSep;
                Write2Chars(chars, 5, dt.Month);
                chars[7] = dateSep;
                Write2Chars(chars, 8, dt.Day);
                chars[10] = ' ';
                Write2Chars(chars, 11, dt.Hour);
                chars[13] = ':';
                Write2Chars(chars, 14, dt.Minute);
                chars[16] = ':';
                Write2Chars(chars, 17, dt.Second);
                chars[19] = '.';
                Write2Chars(chars, 20, dt.Millisecond / 10);
                chars[22] = Digit(dt.Millisecond % 10);
            });
        }

        private static void Write2Chars(in Span<char> chars, int offset, int value) {
            chars[offset] = Digit(value / 10);
            chars[offset + 1] = Digit(value % 10);
        }

        private static void Write4Chars(in Span<char> chars, int offset, int value) {
            chars[offset] = Digit(value / 1000);
            chars[offset + 1] = Digit(value / 100 % 10);
            chars[offset + 2] = Digit(value / 10 % 10);
            chars[offset + 3] = Digit(value % 10);
        }

        private static char Digit(int value) {
            return (char)(value + '0');
        }
    }
}