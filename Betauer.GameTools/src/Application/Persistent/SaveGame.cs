using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Betauer.Application.Persistent;

public class SaveGame {
    [JsonPropertyName("CreateDate")] public DateTime CreateDate { get; set; } = DateTime.Now;
    [JsonPropertyName("UpdateDate")] public DateTime UpdateDate { get; set; } = DateTime.Now;

    [JsonIgnore] public string Name { get; set; }
    [JsonIgnore] public string MetadataFileName { get; set; }
    [JsonIgnore] public string SavegameFileName { get; set; }
    [JsonIgnore] public DateTime ReadDate { get; set; } = DateTime.Now;
    [JsonIgnore] public long Size { get; set; } = 0;
    [JsonIgnore] public LoadStatus LoadStatus { get; set; } = LoadStatus.Ok;
    [JsonIgnore] public bool Ok => LoadStatus == LoadStatus.Ok;

    [JsonIgnore] public List<SaveObject> GameObjects { get; set; }
}