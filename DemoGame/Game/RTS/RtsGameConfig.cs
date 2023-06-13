using System.Text.Json;
using System.Text.Json.Serialization;
using Betauer.Application.Lifecycle.Attributes;
using Betauer.Application.Lifecycle.Pool;
using Betauer.Application.Persistent;
using Betauer.Application.Persistent.Json;
using Betauer.Camera;
using Betauer.DI.Attributes;
using Betauer.DI.Holder;
using Godot;
using Veronenger.Game.Character.Npc;
using Veronenger.Game.Character.Player;
using Veronenger.Game.HUD;
using Veronenger.Game.Items;
using Veronenger.Game.Platform;
using Veronenger.Game.Worlds.Platform;
using Veronenger.Game.Worlds.RTS;

namespace Veronenger.Game.RTS;

[Configuration]
[Loader("GameLoader", Tag = "main")]
[Scene.Transient<TerrainGameView>("TerrainGameViewFactory")]
public class PlatformMainResources {
}

[Configuration]
[Loader("GameLoader", Tag = "game")]
[Scene.Transient<Terrain>("TerrainFactory")]
public class PlatformGameResources {
}

[Configuration]
[PoolContainer<Node>("RtsPoolNodeContainer")]
public class PoolConfig {
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
	
	[Singleton] public IMutableHolder<IGameView> TerrainGameViewHolder => Holder.From<IGameView>("TerrainGameViewFactory"); 
}