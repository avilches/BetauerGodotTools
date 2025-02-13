using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Betauer.Core;

/// <summary>
/// Reads a line like this
///
/// tag1 tag2 tag3,tag4 intnumber=12 floatnumber= 2.3 str1="string with spaces" attribute3=string att4=true
///
/// and returns a ParseResult with a HashSet of tags and the attributes as a Dictionary<string, object>
/// Tags are strings
/// Attributes could be bool, string, int, float
/// </summary>
public static partial class AttributeParser {
    // Permite números, letras y "_-+@#&/" al inicio y en el medio, y también puntos en el medio
    private static readonly Regex ValidNamePattern = MyRegex();

    public static string ForbiddenChars = ", ' \" =";

    [GeneratedRegex("^[^,'\"=]+$", RegexOptions.Compiled)]
    private static partial Regex MyRegex();

    public class ParseResult(StringComparer comparer) : IEquatable<ParseResult> {
        public HashSet<string> Tags { get; } = new(comparer);
        public Dictionary<string, object> Attributes { get; } = new(comparer);

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ParseResult)obj);
        }

        public bool Equals(ParseResult other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            if (!Tags.SetEquals(other.Tags)) return false;
            if (Attributes.Count != other.Attributes.Count) return false;

            foreach (var pair in Attributes) {
                if (!other.Attributes.TryGetValue(pair.Key, out var otherValue)) return false;
                if (pair.Value == null && otherValue == null) continue;
                if (pair.Value == null || otherValue == null) return false;

                // Comparación especial para strings si estamos usando StringComparer
                if (pair.Value is string s1 && otherValue is string s2) {
                    if (!comparer.Equals(s1, s2)) return false;
                }
                // Para otros tipos, usar Equals normal
                else if (!pair.Value.Equals(otherValue)) {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = Tags.Count;
                foreach (var tag in Tags.OrderBy(t => t, comparer)) {
                    hashCode = (hashCode * 397) ^ comparer.GetHashCode(tag);
                }
                foreach (var pair in Attributes.OrderBy(p => p.Key, comparer)) {
                    hashCode = (hashCode * 397) ^ comparer.GetHashCode(pair.Key);
                    hashCode = (hashCode * 397) ^ (pair.Value?.GetHashCode() ?? 0);
                }
                return hashCode;
            }
        }

        public override string ToString() {
            return $"{string.Join(",", Tags.OrderBy(s => s))} {string.Join(" ", Attributes.Select(pair => pair.Key + "=" + Print(pair.Value)).OrderBy(t => t))}".Trim();
        }

        public static string Print(object pairValue) {
            return pairValue switch {
                bool b => b.ToString().ToLowerInvariant(),
                byte or sbyte or short or ushort or int or uint or long or ulong =>
                    ((IFormattable)pairValue).ToString(null, CultureInfo.InvariantCulture),
                float f1 => f1.ToString("0.0###########", CultureInfo.InvariantCulture),
                double d1 => d1.ToString("0.0###########", CultureInfo.InvariantCulture),
                string s => FormatString(s),
                _ => pairValue.ToString() ?? string.Empty
            };
        }

        private static bool NeedsQuotes(string s) {
            return s.Any(c => char.IsWhiteSpace(c) || c == '"' || c == '\'' || c == '\\' || c == '\n' || c == '\r' || c == '\t');
        }

        private static string FormatString(string s) {
            if (!NeedsQuotes(s)) return '"'+s+'"';

            var singleQuotes = s.Count(c => c == '\'');
            var doubleQuotes = s.Count(c => c == '"');

            // Si no hay comillas en el string, usar comillas dobles por defecto
            if (singleQuotes == 0 && doubleQuotes == 0) {
                return $"\"{EscapeSpecialChars(s)}\"";
            }

            // Si hay comillas dobles y no hay simples, usar comillas simples
            if (doubleQuotes > 0 && singleQuotes == 0) {
                return $"'{EscapeSpecialChars(s)}'";
            }

            // Si solo hay comillas simples o hay ambos tipos, usar comillas dobles y escapar las dobles
            return $"\"{EscapeSpecialChars(s).Replace("\"", "\\\"")}\"";
        }

        private static string EscapeSpecialChars(string s) {
            return s
                .Replace("\\", "\\\\")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");
        }
    }

    public class ParseException(string message) : Exception(message);

    private static bool IsValidName(string name) {
        return !string.IsNullOrEmpty(name) && ValidNamePattern.IsMatch(name);
    }

    public static ParseResult Parse(string input, bool ignoreCase = false) {
        var result = new ParseResult(ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
        var currentIndex = 0;

        while (currentIndex < input.Length) {
            // Skip whitespace
            while (currentIndex < input.Length && char.IsWhiteSpace(input[currentIndex])) {
                currentIndex++;
            }

            if (currentIndex >= input.Length) break;

            // Check for comments only when preceded by whitespace or at start
            if (currentIndex == 0 || char.IsWhiteSpace(input[currentIndex - 1])) {
                var c = input[currentIndex];

                // Check for // style comments
                if (c == '/' && currentIndex + 1 < input.Length && input[currentIndex + 1] == '/') {
                    break; // Skip rest of the line
                }

                // Check for # style comments when it's a standalone #
                if (c == '#') {
                    // Si es un # al inicio o después de un espacio, y está seguido por un espacio
                    // o si es un # solo, es un comentario
                    if (currentIndex + 1 >= input.Length ||
                        char.IsWhiteSpace(input[currentIndex + 1])) {
                        break; // Skip rest of the line
                    }
                }
            }

            // Find next token end (space or end of string)
            var tokenStart = currentIndex;
            var inQuotes = false;
            var quoteChar = '\0';
            var escaped = false;
            var tokenEnd = currentIndex;

            while (currentIndex < input.Length) {
                var c = input[currentIndex];

                if (escaped) {
                    escaped = false;
                } else {
                    if (c == '\\') {
                        escaped = true;
                    } else if (c == '"' || c == '\'') {
                        if (!inQuotes) {
                            inQuotes = true;
                            quoteChar = c;
                        } else if (c == quoteChar) {
                            inQuotes = false;
                        }
                    } else if (!inQuotes && char.IsWhiteSpace(c)) {
                        break;
                    }
                }

                tokenEnd = currentIndex;
                currentIndex++;
            }

            // Use tokenEnd + 1 to include the last character
            var token = input[tokenStart..(tokenEnd + 1)].Trim();
            if (string.IsNullOrEmpty(token)) {
                currentIndex++;
                continue;
            }

            // If token contains '=', it's an attribute, otherwise it's a tag
            if (token.Contains('=')) {
                ParseAttribute(token, result.Attributes);
            } else {
                var tags = token.Split(',');
                foreach (var tag in tags.Select(t => t.Trim()).Where(t => !string.IsNullOrEmpty(t))) {
                    if (!IsValidName(tag)) {
                        throw new ParseException($"Invalid tag name: '{tag}'. Forbidden chars are {ForbiddenChars}");
                    }
                    result.Tags.Add(tag);
                }
            }

            currentIndex++;
        }

        return result;
    }

    private static void ParseAttribute(string token, Dictionary<string, object> attributes) {
        var separatorIndex = token.IndexOf('=');
        if (separatorIndex <= 0) {
            throw new ParseException($"Invalid attribute format: '{token}'. Expected 'name=value'");
        }

        var key = token[..separatorIndex].Trim();
        if (!IsValidName(key)) {
            throw new ParseException($"Invalid attribute name: '{key}'. Forbidden chars are {ForbiddenChars}");
        }

        var valueStr = token[(separatorIndex + 1)..].Trim();
        if (string.IsNullOrEmpty(valueStr)) {
            throw new ParseException($"Empty value for attribute: '{key}'");
        }

        // Try parse as boolean
        if (valueStr.Equals("true", StringComparison.OrdinalIgnoreCase)) {
            SetAttribute(attributes, key, true);
            return;
        }
        if (valueStr.Equals("false", StringComparison.OrdinalIgnoreCase)) {
            SetAttribute(attributes, key, false);
            return;
        }

        // Try parse as number if it's not quoted
        if (!valueStr.StartsWith('"') && !valueStr.StartsWith('\'')) {
            // Try parse as integer
            if (int.TryParse(valueStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intValue)) {
                SetAttribute(attributes, key, intValue);
                return;
            }

            // Try parse as float
            if (float.TryParse(valueStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var floatValue)) {
                SetAttribute(attributes, key, floatValue);
                return;
            }
        }

        // Check for properly closed quotes
        if (valueStr.StartsWith('"')) {
            if (!valueStr.EndsWith('"')) {
                throw new ParseException($"Unclosed double quote in attribute value: '{key}'");
            }
            SetAttribute(attributes, key, UnescapeString(valueStr[1..^1]));
            return;
        }

        if (valueStr.StartsWith('\'')) {
            if (!valueStr.EndsWith('\'')) {
                throw new ParseException($"Unclosed single quote in attribute value: '{key}'");
            }
            SetAttribute(attributes, key, UnescapeString(valueStr[1..^1]));
            return;
        }

        // If no quotes, treat as plain string
        SetAttribute(attributes, key, valueStr);
    }

    private static void SetAttribute(Dictionary<string, object> attributes, string key, object value) {
        if (!attributes.TryAdd(key, value)) {
            throw new ParseException($"Duplicate attribute: '{key}'");
        }
    }

    private static string UnescapeString(string str) {
        return str
            .Replace("\\\"", "\"")
            .Replace("\\'", "'")
            .Replace(@"\\", "\\")
            .Replace("\\n", "\n")
            .Replace("\\r", "\r")
            .Replace("\\t", "\t");
    }
}