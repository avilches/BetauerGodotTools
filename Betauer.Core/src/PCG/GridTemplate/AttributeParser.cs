using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Betauer.Core.PCG.GridTemplate;

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

    public static string ForbiddenChars => ", ' \" =";
    [GeneratedRegex("^[^,'\"=]+$", RegexOptions.Compiled)]
    private static partial Regex MyRegex();

    public class ParseResult(StringComparer comparer) {
        public HashSet<string> Tags { get; } = new (comparer);
        public Dictionary<string, object> Attributes { get; } = new(comparer);
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
            SetAttribute(attributes, key,  UnescapeString(valueStr[1..^1]));
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

    private static void SetAttribute(Dictionary<string,object> attributes, string key, object value) {
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