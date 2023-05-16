using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Betauer.Application.Persistent;

public class SaveGame {
    [JsonPropertyName("Version")] public int Version { get; set; }
    [JsonPropertyName("CreateDate")] public DateTime CreateDate { get; set; } = DateTime.Now;
    [JsonPropertyName("UpdateDate")] public DateTime UpdateDate { get; set; } = DateTime.Now;

    [JsonIgnore] public string Name { get; set; }
    [JsonIgnore] public string? ErrorMessage { get; set; }
    [JsonIgnore] public bool Ok => ErrorMessage == null;
    [JsonIgnore] public DateTime ReadDate { get; set; } = DateTime.Now;
    [JsonIgnore] public List<SaveObject> GameObjects { get; set; }
}