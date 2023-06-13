using System.Text.Json;
using System.Text.Json.Serialization;
using Betauer.Application.Lifecycle.Attributes;
using Betauer.Application.Lifecycle.Pool;
using Betauer.Application.Persistent;
using Betauer.Application.Persistent.Json;
using Betauer.Camera;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Betauer.DI.Holder;
using Godot;
using Veronenger.Game.Character.Npc;
using Veronenger.Game.Character.Player;
using Veronenger.Game.HUD;
using Veronenger.Game.Items;
using Veronenger.Game.Worlds.Platform;
using Veronenger.Game.Worlds.RTS;

namespace Veronenger.Game.Platform;

[Configuration]
[Loader("GameLoader", Tag = "main")]
[Scene.Transient<PlatformGameView>("PlatformGameViewFactory")]
public class PlatformMainResources {
}

[Configuration]
[Loader("GameLoader", Tag = "game")]
[Resource<Texture2D>("Pickups", "res://Game/Items/Assets/pickups.png")]
[Resource<Texture2D>("Pickups2", "res://Game/Items/Assets/pickups2.png")]
[Resource<Texture2D>("LeonKnifeAnimationSprite", "res://Game/Character/Player/Assets/Leon-knife.png")]
[Resource<Texture2D>("LeonMetalbarAnimationSprite", "res://Game/Character/Player/Assets/Leon-metalbar.png")]
[Resource<Texture2D>("LeonGun1AnimationSprite", "res://Game/Character/Player/Assets/Leon-gun1.png")]
[Scene.Transient<WorldPlatform>("WorldPlatformFactory")]
[Scene.Transient<PlayerHud>("PlayerHudFactory")]
[Scene.Transient<PlayerNode>("PlayerNodeFactory")]
[Scene.Transient<ZombieNode>("ZombieNodeFactory")]
[Scene.Transient<InventorySlot>("InventorySlotFactory")]
[Scene.Transient<ProjectileTrail>("ProjectileTrailFactory")]
[Scene.Transient<PickableItemNode>("PickableItemFactory")]
public class PlatformGameResources {
	[Transient<HudCanvas>] HudCanvas HudCanvasFactory => new HudCanvas();
}

[Configuration]
[PoolContainer<Node>("PlatformPoolNodeContainer")]
public class PoolConfig {
	[Pool] NodePool<PlayerNode> PlayerPool => new("PlayerNodeFactory");
	[Pool] NodePool<ZombieNode> ZombiePool => new("ZombieNodeFactory");
	[Pool] NodePool<ProjectileTrail> ProjectilePool => new("ProjectileTrailFactory");
	[Pool] NodePool<PickableItemNode> PickableItemPool => new("PickableItemFactory");
}


public interface IPlatformSaveObject : ISaveObject {
}

[Configuration]
public class PlatformGameConfig {
	[Singleton] public JsonGameLoader<PlatformSaveGameMetadata> PlatformGameObjectLoader() {
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
	[Singleton] public IMutableHolder<IGameView> PlatformGameViewHolder => Holder.From<IGameView>("PlatformGameViewFactory"); 
	[Singleton] public IHolder<WorldPlatform> WorldPlatformHolder => Holder.Chain<IGameView, WorldPlatform>("PlatformGameViewHolder", (gameView) => (WorldPlatform)gameView.GetWorld()); 
	[Singleton] public IHolder<HudCanvas> HudCanvasHolder => Holder.Chain<IGameView, HudCanvas>("PlatformGameViewHolder", (gameView) => ((PlatformGameView)gameView).HudCanvas); 
}