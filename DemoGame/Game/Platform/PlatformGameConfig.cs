using System.Text.Json;
using System.Text.Json.Serialization;
using Betauer.Application.Lifecycle.Attributes;
using Betauer.Application.Persistent;
using Betauer.Application.Persistent.Json;
using Betauer.Camera;
using Betauer.DI;
using Betauer.DI.Attributes;
using Veronenger.Game.Worlds.Platform;

namespace Veronenger.Game.Platform;

[Configuration]
[Loader("GameLoader", Tag = "main")]
[Scene.Transient<GameView>("GameViewFactory")]
public class PlatformResources {
}

public interface IPlatformSaveObject : ISaveObject {
}

[Configuration]
public class PlatformGameConfig {
	[Singleton] public JsonGameLoader<PlatformSaveGameMetadata> GameObjectLoader() {
		var loader = new JsonGameLoader<PlatformSaveGameMetadata>();
		loader.WithJsonSerializerOptions(options => {
			options.AllowTrailingCommas = true;
			options.WriteIndented = true;
			options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
		});
		loader.Scan<IPlatformSaveObject>();
		return loader;
	}
	
	[Transient] public StageController StageControllerFactory => new(CollisionLayerConstants.LayerStageArea);
	[Transient] public StageCameraController StageCameraControllerFactory => new(CollisionLayerConstants.LayerStageArea);
	[Singleton] public IHolder<IGameView> GameViewHolder => new Holder<IGameView>("GameViewFactory"); 
}