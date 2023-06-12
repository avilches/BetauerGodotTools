using System.Text.Json.Serialization;
using Betauer.Application.Persistent;

namespace Veronenger.Game.Platform;

public class PlatformSaveGameMetadata : Metadata {

    [JsonPropertyName("GamePercent")] public int GamePercent { get; set; } = 0;
}