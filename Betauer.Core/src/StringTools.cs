using System;
using System.Collections.Generic;

namespace Betauer.Core;

public static class StringTools {
    public static string Concatenated<T>(this IEnumerable<T> items, string e = "") =>
        string.Join(e, items);


    public static string PadCenter(this string source, int totalWidth, char paddingChar = ' ') {
        if (totalWidth <= source.Length) return source;

        var padding = totalWidth - source.Length;
        var padLeft = padding / 2 + source.Length;
        return source.PadLeft(padLeft, paddingChar).PadRight(totalWidth, paddingChar);
    }


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
            return $"{numBytes / 1024d:0.00} KB";

        if (numBytes < 1073741824)
            return $"{numBytes / 1048576d:0.00} MB";

        if (numBytes < 1099511627776)
            return $"{numBytes / 1073741824d:0.00} GB";

        if (numBytes < 1125899906842624)
            return $"{numBytes / 1099511627776d:0.00} TB";

        if (numBytes < 1152921504606846976)
            return $"{numBytes / 1125899906842624d:0.00} PB";

        return $"{numBytes / 1152921504606846976d:0.00} EB";
    }

    public static string FastFormatDateTime(DateTime dateTime, char dateSep = '-') {
        return string.Create(23, dateTime, (buffer, dt) => {
            //yyyy-mm-dd hh:mm:ss.mmm
            var pos = 0;
            pos = Write4Digits(buffer, dt.Year, pos);
            pos = Write(buffer, dateSep, pos);
            pos = Write2Digits(buffer, dt.Month, pos);
            pos = Write(buffer, dateSep, pos);
            pos = Write2Digits(buffer, dt.Day, pos);
            pos = Write(buffer, ' ', pos);
            pos = Write2Digits(buffer, dt.Hour, pos);
            pos = Write(buffer, ':', pos);
            pos = Write2Digits(buffer, dt.Minute, pos);
            pos = Write(buffer, ':', pos);
            pos = Write2Digits(buffer, dt.Second, pos);
            pos = Write(buffer, '.', pos);
            pos = Write2Digits(buffer, dt.Millisecond / 10, pos);
            pos = Write(buffer, DigitToChar(dt.Millisecond % 10), pos);
        });
    }

    public static string JoinString(string[] data) {
        return JoinString(data, (char)0);
    }

    public static string JoinString(string[] data, char sep) {
        var hasSep = sep != (char)0;
        var size = hasSep ? data.Length - 1 : 0;
        for (var i = 0; i < data.Length; i++) size += data[i].Length;

        var join = string.Create(size, data, (buffer, fields) => {
            var pos = 0;
            var dtLength = fields.Length;
            for (var i = 0; i < dtLength; i++) {
                if (hasSep && i > 0) pos = Write(buffer, sep, pos);
                pos = Write(buffer, fields[i], pos);
            }
        });
        return join;
    }

    public static int Write2Digits(in Span<char> chars, int value, int pos) {
        chars[pos] = DigitToChar(value / 10);
        chars[pos + 1] = DigitToChar(value % 10);
        return pos + 2;
    }

    public static int Write4Digits(in Span<char> chars, int value, int pos) {
        chars[pos] = DigitToChar(value / 1000);
        chars[pos + 1] = DigitToChar(value / 100 % 10);
        chars[pos + 2] = DigitToChar(value / 10 % 10);
        chars[pos + 3] = DigitToChar(value % 10);
        return pos + 4;
    }

    public static int Write(Span<char> buffer, char sep, int pos) {
        buffer[pos] = sep;
        return pos + 1;
    }

    public static int Write(Span<char> buffer, ReadOnlySpan<char> str, int pos) {
        var strLength = str.Length;
        for (var i = 0; i < strLength; i++) buffer[i + pos] = str[i];
        return pos + strLength;
    }

    public static char DigitToChar(int value) {
        return (char)(value + '0');
    }
}