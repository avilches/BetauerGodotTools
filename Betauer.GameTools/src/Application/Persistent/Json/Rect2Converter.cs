using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;
using Godot.Collections;

namespace Betauer.Application.Persistent.Json;

public class Rect2Converter : JsonConverter<Rect2> {
    public override Rect2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        if (reader.TokenType != JsonTokenType.StartObject) {
            throw new JsonException($"JsonTokenType was of type {reader.TokenType}, only objects are supported");
        }
        var dictionary = new Dictionary<string, float>();
        while (reader.Read()) {
            if (reader.TokenType == JsonTokenType.EndObject) {
                if (dictionary.TryGetValue("x", out var x) && 
                    dictionary.TryGetValue("y", out var y) &&
                    dictionary.TryGetValue("width", out var width) &&
                    dictionary.TryGetValue("height", out var height)) {
                    return new Rect2(x, y, width, height);
                }
                throw new JsonException($"Missing fields x or y");
            }
            var propertyName = JsonHelper.GetPropertyName(ref reader);
            if (propertyName != "x" && 
                propertyName != "y" && 
                propertyName != "width" && 
                propertyName != "height") {
                throw new JsonException($"Invalid property name {propertyName}");
            }
            reader.Read();
            var value = JsonHelper.GetFloat(ref reader);
            dictionary.Add(propertyName, value);
        }
        throw new JsonException($"Too many tokens, expected EndObject");
    }


    public override void Write(Utf8JsonWriter writer, Rect2 value, JsonSerializerOptions options) {
        writer.WriteStartObject();
        writer.WriteNumber("x", value.Position.X);
        writer.WriteNumber("y", value.Position.Y);
        writer.WriteNumber("width", value.Size.X);
        writer.WriteNumber("height", value.Size.Y);
        writer.WriteEndObject();
    }
}