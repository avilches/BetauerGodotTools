using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Betauer.Application.Lifecycle.Attributes;
using Betauer.Application.Persistent;
using Betauer.Application.Persistent.Json;
using Betauer.DI.Attributes;
using Godot;
using Veronenger.Game.RTS.HUD;
using Veronenger.Game.RTS.World;
using Veronenger.RTS.Assets.Trees;

namespace Veronenger.Game.RTS;

[Configuration]
[Loader("GameLoader", Tag = MainResources.GameLoaderTag)]
[Scene.Transient<RtsGameView>(Name = "RtsGameView")]
public class RtsMainResources {
}

[Configuration]
[Loader("GameLoader", Tag = GameLoaderTag)]
[Scene.Transient<Trees>(Name = "TreesFactory")]
[Scene.Transient<RtsWorld>(Name = "RtsWorldFactory")]
[Scene.Transient<RtsPlayerHud>(Name = "RtsPlayerHud")]
public class RtsGameResources {
	[Transient<RtsHud>] RtsHud RtsHudFactory => new RtsHud();

	public const string GameLoaderTag = "rts";
}


public interface IRtsSaveObject : ISaveObject {
}

[Configuration]
public class RtsGameConfig {
	[Singleton] public JsonGameLoader<RtsSaveGameMetadata> RtsGameObjectLoader() {
		var loader = new JsonGameLoader<RtsSaveGameMetadata>();
		loader.WithJsonSerializerOptions(options => {
			options.AllowTrailingCommas = true;
			options.WriteIndented = true;
			options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
		});
		loader.Scan<IRtsSaveObject>();
		return loader;
	}
}

[Singleton]
public class RtsConfig {
	public readonly List<float> ZoomLevels = new() { 0.0625f, 0.125f, 0.25f, 0.5f, 1f, 2f, 4f, 8f, 16f, 32f, 64f};
	public readonly float DefaultZoom = 2f;
	public float ZoomTime = 0.15f;

}