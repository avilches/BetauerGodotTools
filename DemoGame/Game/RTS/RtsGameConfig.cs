using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Betauer.Application.Lifecycle.Attributes;
using Betauer.Application.Persistent;
using Betauer.Application.Persistent.Json;
using Betauer.DI.Attributes;
using Betauer.DI.Holder;
using Godot;
using Veronenger.Game.RTS.HUD;
using Veronenger.Game.RTS.World;

namespace Veronenger.Game.RTS;

[Configuration]
[Loader("GameLoader", Tag = "main")]
[Scene.Transient<RtsGameView>("RtsGameViewFactory")]
public class RtsMainResources {
}

[Configuration]
[Loader("GameLoader", Tag = "rts")]
[Scene.Transient<RtsWorld>("RtsWorldFactory")]
[Scene.Transient<RtsPlayerHud>("RtsPlayerHudFactory")]
public class RtsGameResources {
	[Transient<RtsHud>] RtsHud RtsHudFactory => new RtsHud();
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
	
	[Singleton] public IMutableHolder<IGameView> RtsGameViewHolder => Holder.From<IGameView>("RtsGameViewFactory"); 
}

[Singleton]
public class RtsConfig {
	public readonly List<float> ZoomLevels = new() { 0.0625f, 0.125f, 0.25f, 0.5f, 1f, 2f, 4f};
	public readonly float DefaultZoom = 1;
	public readonly float ZoomTime = 0.25f;

}