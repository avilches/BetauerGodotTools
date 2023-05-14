using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;
using Godot.Collections;

namespace Betauer.Application.Persistent.Json;

public class Vector2IConverter : JsonConverter<Vector2I> {
    public override Vector2I Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        if (reader.TokenType != JsonTokenType.StartObject) {
            throw new JsonException($"JsonTokenType was of type {reader.TokenType}, only objects are supported");
        }
        var dictionary = new Dictionary<string, int>();
        while (reader.Read()) {
            if (reader.TokenType == JsonTokenType.EndObject) {
                if (dictionary.TryGetValue("x", out var x) && 
                    dictionary.TryGetValue("y", out var y)) {
                    return new Vector2I(x, y);
                }
                throw new JsonException($"Missing fields x or y");
            }
            var propertyName = JsonHelper.GetPropertyName(ref reader);
            if (propertyName != "x" && propertyName != "y") {
                throw new JsonException($"Invalid property name {propertyName}");
            }
            reader.Read();
            var value = JsonHelper.GetInt(ref reader);
            dictionary.Add(propertyName, value);
        }
        throw new JsonException($"Too many tokens, expected EndObject");
    }


    public override void Write(Utf8JsonWriter writer, Vector2I value, JsonSerializerOptions options) {
        writer.WriteStartObject();
        writer.WritePropertyName("x");
        writer.WriteNumberValue(value.X);
        writer.WritePropertyName("y");
        writer.WriteNumberValue(value.Y);
        writer.WriteEndObject();
    }
}