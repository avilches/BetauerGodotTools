using System.Linq;
using Betauer.Core;
using Betauer.Core.Nodes;
using Betauer.Core.Pool;
using Betauer.DI;
using Godot;
using Veronenger.Config;
using Veronenger.Managers;
using Veronenger.Persistent;

namespace Veronenger.Worlds;

public partial class World3 : Node {
	[Inject] private Game Game { get; set; }
	[Inject] private ItemConfigManager ItemConfigManager { get; set; }
	[Inject] private ItemRepository ItemRepository { get; set; }
	[Inject] private PlatformManager PlatformManager { get; set; }
	[Inject] private StageManager StageManager { get; set; }
	[Inject] private IPool<PickableItemNode> PickableItemNodeFactory { get; set; }

	public override void _Ready() {
		GetNode("EnemySpawn").GetChildren().OfType<Marker2D>().ForEach(m => {
			if (m.Visible) Game.ZombieSpawn(this, m.GlobalPosition);
		});

		GetChildren().OfType<TileMap>().ForEach(PlatformManager.ConfigureTileMapCollision);
		GetChildren().OfType<CanvasModulate>().ForEach(cm => cm.Visible = true);
		GetNode<Node>("Lights").GetChildren().OfType<PointLight2D>().ForEach(light => {
			if (light.Name.ToString().StartsWith("Candle")) {
				CandleOff(light);
			}
		});
		PlatformManager.ConfigurePlatformsCollisions();
		GetNode<Node>("Stages").GetChildren().OfType<Area2D>().ForEach(StageManager.ConfigureStage);

		PlaceMetalbar();
	}

	private void PlaceMetalbar() {
		
		// TODO: add these weapons
		// ItemRepository.Create<WeaponMeleeItem>("Knife", "K1").Configure();
        
		// var slowGun = ItemRepository.AddRangeWeapon(ItemConfigManager.SlowGun, "Gun", 6f, "SG");
		// slowGun.DelayBetweenShots = 0.2f;
		//
		// var gun = ItemRepository.AddRangeWeapon(ItemConfigManager.Gun, "Gun", 9f, "G");
		// gun.DelayBetweenShots = 0.4f;
		//
		// var shotgun = ItemRepository.AddRangeWeapon(ItemConfigManager.Shotgun, "Shotgun", 22f,"SG-");
		// shotgun.DelayBetweenShots = 1f;
		// shotgun.EnemiesPerHit = 2;
		//
		// var machinegun = ItemRepository.AddRangeWeapon(ItemConfigManager.MachineGun, "Maching gun", 4, "MG");
		// machinegun.DelayBetweenShots = 0.05f;
		// machinegun.EnemiesPerHit = 3;
		// machinegun.Auto = true;

		var metalbarPosition = GetPositionFromMarker("MetalbarSpawn");

		var pickableItemNode = PickableItemNodeFactory.Get();
		var metalbar = ItemRepository.Create<WeaponMeleeItem>("Metalbar", "M1")
			.Configure(ItemConfigManager.Metalbar, 9f);
		metalbar.LinkNode(pickableItemNode);
		pickableItemNode.AddToScene(this, () => pickableItemNode.CharacterBody2D.GlobalPosition = metalbarPosition);
	}

	public Vector2 GetPositionFromMarker(string path, bool freeMarker = true) {
		var marker = GetNode<Marker2D>(path);
		var globalPosition = marker.GlobalPosition;
		if (freeMarker) marker.QueueFree();
		return globalPosition;
	}

	private void CandleOff(PointLight2D light) {
		light.Enabled = true;
		light.Color = new Color("ffd1c8");
		light.TextureScale = 0.2f;
		light.ShadowEnabled = true;
		light.ShadowFilter = Light2D.ShadowFilterEnum.None;
		light.GetNode<Area2D>("Area2D")
			?.OnBodyEntered(LayerConstants.LayerPlayerBody, (player) => {
				if (player is CharacterBody2D character && ItemRepository.IsPlayer(character)) CandleOn(light);
			});
	}

	private void CandleOn(PointLight2D light) {
		light.Enabled = true;
		light.Color = new Color("ffd1c8");
		light.TextureScale = 0.8f;
		// light.ShadowEnabled = false;
	}

	
}
