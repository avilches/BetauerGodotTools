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
using Veronenger.Game.Platform.Character.Npc;
using Veronenger.Game.Platform.Character.Player;
using Veronenger.Game.Platform.HUD;
using Veronenger.Game.Platform.Items;
using Veronenger.Game.Platform.World;

namespace Veronenger.Game.Platform;

[Configuration]
[Loader("GameLoader", Tag = "main")]
[Scene.Transient<PlatformGameView>(Name = "PlatformGameViewFactory")]
public class PlatformMainResources {
}

[Configuration]
[Loader("GameLoader", Tag = "platform")]
[PoolContainer<Node>("PlatformPoolNodeContainer")]
[Resource<Texture2D>("Pickups", "res://Game/Platform/Items/Assets/pickups.png")]
[Resource<Texture2D>("Pickups2", "res://Game/Platform/Items/Assets/pickups2.png")]
[Resource<Texture2D>("LeonKnifeAnimationSprite", "res://Game/Platform/Character/Player/Assets/Leon-knife.png")]
[Resource<Texture2D>("LeonMetalbarAnimationSprite", "res://Game/Platform/Character/Player/Assets/Leon-metalbar.png")]
[Resource<Texture2D>("LeonGun1AnimationSprite", "res://Game/Platform/Character/Player/Assets/Leon-gun1.png")]
[Scene.Transient<PlatformWorld>(Name = "PlatformWorldFactory")]
[Scene.Transient<PlayerHud>(Name = "PlayerHudFactory")]
[Scene.Transient<InventorySlot>(Name = "InventorySlotFactory")]
[Scene.NodePool<PlayerNode>(Name = "PlayerPool")]
[Scene.NodePool<PickableItemNode>(Name = "PickableItemPool")]
[Scene.NodePool<ProjectileTrail>(Name = "ProjectilePool")]
[Scene.NodePool<ZombieNode>(Name = "ZombiePool")]
public class PlatformGameResources {
	[Transient<PlatformHud>] PlatformHud PlatformHudFactory => new PlatformHud();
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
	[Singleton] public IHolder<PlatformWorld> PlatformWorldHolder => Holder.Chain<IGameView, PlatformWorld>("PlatformGameViewHolder", (gameView) => (PlatformWorld)gameView.GetWorld()); 
	[Singleton] public IHolder<PlatformHud> HudCanvasHolder => Holder.Chain<IGameView, PlatformHud>("PlatformGameViewHolder", (gameView) => ((PlatformGameView)gameView).PlatformHud); 
}