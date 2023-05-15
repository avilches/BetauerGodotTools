using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;

namespace Betauer.Application.Persistent.Json;

public class ColorConverter : JsonConverter<Color> {
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        reader.Read();
        return new Color(reader.GetString());
    }

    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options) {
        writer.WriteStringValue(value.ToHtml(true));
    }
}