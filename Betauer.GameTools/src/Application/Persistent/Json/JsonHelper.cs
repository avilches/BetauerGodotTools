using System;
using System.Text.Json;

namespace Betauer.Application.Persistent.Json;

public static class JsonHelper {
    public static string GetPropertyName(ref Utf8JsonReader reader) {
        if (reader.TokenType != JsonTokenType.PropertyName) 
            throw new JsonException($"Wrong token: '{reader.TokenType}': expected property name");
        var propertyName = reader.GetString()!;
        if (string.IsNullOrEmpty(propertyName) || string.IsNullOrWhiteSpace(propertyName)) 
            throw new JsonException($"Failed to get property name: blank or empty: '{propertyName}'");
        return propertyName!;
    }

    public static bool GetBool(ref Utf8JsonReader reader) {
        if (reader.TokenType == JsonTokenType.True) return true;
        if (reader.TokenType == JsonTokenType.False) return false;
        throw new JsonException($"Wrong token: '{reader.TokenType}': expected bool")
    }

    public static bool GetBoolOr(ref Utf8JsonReader reader, bool @default) {
        if (reader.TokenType == JsonTokenType.Null) return @default;
        return GetBool(ref reader);
    }

    public static string GetString(ref Utf8JsonReader reader) {
        if (reader.TokenType == JsonTokenType.String) return reader.GetString()!; 
        throw new JsonException($"Wrong token: '{reader.TokenType}': expected string");
    }

    public static string GetStringOr(ref Utf8JsonReader reader, string @default) {
        if (reader.TokenType == JsonTokenType.Null) return @default;
        return GetString(ref reader);
    }

    public static int GetInt(ref Utf8JsonReader reader) {
        if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out var result)) return result;
        throw new JsonException($"Wrong token: '{reader.TokenType}': expected int");
    }

    public static int GetIntOr(ref Utf8JsonReader reader, int @default) {
        if (reader.TokenType == JsonTokenType.Null) return @default;
        return GetInt(ref reader);
    }

    public static long GetLong(ref Utf8JsonReader reader) {
        if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt64(out var result)) return result;
        throw new JsonException($"Wrong token: '{reader.TokenType}': expected long");
    }

    public static long GetLongOr(ref Utf8JsonReader reader, long @default) {
        if (reader.TokenType == JsonTokenType.Null) return @default;
        return GetLong(ref reader);
    }

    public static float GetFloat(ref Utf8JsonReader reader) {
        if (reader.TokenType == JsonTokenType.Number && reader.TryGetSingle(out var result)) return result;
        throw new JsonException($"Wrong token: '{reader.TokenType}': expected float");
    }

    public static float GetFloatOr(ref Utf8JsonReader reader, float @default) {
        if (reader.TokenType == JsonTokenType.Null) return @default;
        return GetFloat(ref reader);
    }

    public static double GetDouble(ref Utf8JsonReader reader) {
        if (reader.TokenType == JsonTokenType.Number && reader.TryGetDouble(out var result)) return result;
        throw new JsonException($"Wrong token: '{reader.TokenType}': expected double");
    }

    public static double GetDoubleOr(ref Utf8JsonReader reader, double @default) {
        if (reader.TokenType == JsonTokenType.Null) return @default;
        return GetDouble(ref reader);
    }

    public static DateTime GetDate(ref Utf8JsonReader reader) {
        if (reader.TokenType == JsonTokenType.String && reader.TryGetDateTime(out var result)) return result;
        throw new JsonException($"Wrong token: '{reader.TokenType}': expected date");
    }

    public static DateTime GetDateOr(ref Utf8JsonReader reader, DateTime @default) {
        if (reader.TokenType == JsonTokenType.Null) return @default;
        return GetDate(ref reader);
    }

    public static DateTimeOffset GetDateOffset(ref Utf8JsonReader reader) {
        if (reader.TokenType == JsonTokenType.String && reader.TryGetDateTimeOffset(out var result)) return result;
        throw new JsonException($"Wrong token: '{reader.TokenType}': expected date offset");
    }

    public static DateTimeOffset GetDateOffsetOr(ref Utf8JsonReader reader, DateTimeOffset @default) {
        if (reader.TokenType == JsonTokenType.Null) return @default;
        return GetDateOffset(ref reader);
    }
}