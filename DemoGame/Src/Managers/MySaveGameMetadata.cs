using System.Text.Json.Serialization;
using Betauer.Application.Persistent;

namespace Veronenger.Managers;

public class MySaveGameMetadata : Metadata {

    [JsonPropertyName("GamePercent")] public int GamePercent { get; set; } = 0;
}