using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;
using Godot.Collections;

namespace Betauer.Application.Persistent.Json;

public class Vector3Converter : JsonConverter<Vector3> {
    public override Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        if (reader.TokenType != JsonTokenType.StartObject) {
            throw new JsonException($"JsonTokenType was of type {reader.TokenType}, only objects are supported");
        }
        var dictionary = new Dictionary<string, float>();
        while (reader.Read()) {
            if (reader.TokenType == JsonTokenType.EndObject) {
                if (dictionary.TryGetValue("x", out var x) && 
                    dictionary.TryGetValue("y", out var y) && 
                    dictionary.TryGetValue("z", out var z)) {
                    return new Vector3(x, y, z);
                }
                throw new JsonException($"Missing fields x, y or z");
            }
            var propertyName = JsonHelper.GetPropertyName(ref reader);
            if (propertyName != "x" && propertyName != "y" && propertyName != "z") {
                throw new JsonException($"Invalid property name {propertyName}");
            }
            reader.Read();
            var value = JsonHelper.GetFloat(ref reader);
            dictionary.Add(propertyName, value);
        }
        throw new JsonException($"Too many tokens, expected EndObject");
    }


    public override void Write(Utf8JsonWriter writer, Vector3 value, JsonSerializerOptions options) {
        writer.WriteStartObject();
        writer.WriteNumber("x", value.X);
        writer.WriteNumber("y", value.Y);
        writer.WriteNumber("z", value.Z);
        writer.WriteEndObject();
    }
}