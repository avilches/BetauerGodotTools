using System;
using System.Text.Json.Serialization;

namespace Betauer.Application.Persistent;

public class Metadata {
    [JsonIgnore] public string Name { get; set; }
    [JsonIgnore] public DateTime ReadDate { get; set; } = DateTime.Now;

    [JsonPropertyName("Hash")] public string Hash { get; set; }
    [JsonPropertyName("CreateDate")] public DateTime CreateDate { get; set; } = DateTime.Now;
    [JsonPropertyName("UpdateDate")] public DateTime UpdateDate { get; set; } = DateTime.Now;
}