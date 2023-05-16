using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Betauer.Application.Persistent;

public class SaveGame {
    [JsonPropertyName("Version")] public int Version { get; set; }
    [JsonPropertyName("CreateDate")] public DateTime CreateDate { get; set; }
    [JsonPropertyName("UpdateDate")] public DateTime UpdateDate { get; set; }
    
    [JsonIgnore] public DateTime ReadDate { get; set; }
    [JsonIgnore] public List<SaveObject> GameObjects { get; set; }
}