using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Betauer.Application;
using Betauer.Application.Lifecycle.Attributes;
using Betauer.Application.Monitor;
using Betauer.Application.Persistent;
using Betauer.Application.Persistent.Json;
using Betauer.Application.Screen;
using Betauer.Application.Settings;
using Betauer.Application.Settings.Attributes;
using Betauer.Camera;
using Betauer.Camera.Follower;
using Betauer.DI.Attributes;
using Betauer.Input;
using Betauer.Input.Attributes;
using Betauer.Input.Joypad;
using Godot;
using Pcg;
using Veronenger.Game.HUD;
using Veronenger.Game.Platform;
using Veronenger.Game.UI;
using Veronenger.Game.UI.Settings;

namespace Veronenger.Game.Platform; 

[Configuration]
public class GameConfig {
	[Singleton] public JsonGameLoader<MySaveGameMetadata> GameObjectLoader() {
		var loader = new JsonGameLoader<MySaveGameMetadata>();
		loader.WithJsonSerializerOptions(options => {
			options.AllowTrailingCommas = true;
			options.WriteIndented = true;
			options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
		});
		return loader;
	}
	
	[Transient] public StageController StageControllerFactory => new(CollisionLayerConstants.LayerStageArea);
	[Transient] public StageCameraController StageCameraControllerFactory => new(CollisionLayerConstants.LayerStageArea);

}