using System;
using System.Linq;
using Betauer.Core;
using Betauer.Core.Nodes;
using Betauer.Core.Pool;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Godot;
using Veronenger.Character.Npc;
using Veronenger.Character.Player;
using Veronenger.Config;
using Veronenger.Managers;
using Veronenger.Persistent;
using Veronenger.Transient;

namespace Veronenger.Worlds;

public partial class WorldScene : Node {
	[Inject] private ItemConfigManager ItemConfigManager { get; set; }
	[Inject] private ItemRepository ItemRepository { get; set; }
	[Inject] private PlatformManager PlatformManager { get; set; }
	[Inject] private StageManager StageManager { get; set; }
	[Inject] private IPool<PickableItemNode> PickableItemNodeFactory { get; set; }
	[Inject] private IFactory<PlayerNode> PlayerFactory { get; set; }
	[Inject] private IPool<ProjectileTrail> ProjectilePool { get; set; }
	[Inject] private IPool<ZombieNode> ZombiePool { get; set; }

	public override void _Ready() {
		GetNode("EnemySpawn").GetChildren().OfType<Marker2D>().ForEach(m => {
			if (m.Visible) ZombieSpawn(this, m.GlobalPosition);
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
		PlaceSlowGun();
		PlaceGun();
		PlaceShotgun();
		PlaceMachineGun();
	}

	private void PlaceMetalbar() {
		var metalbar = ItemRepository
			.Create<WeaponMeleeItem>("Metalbar", "M1")
			.Configure(ItemConfigManager.Metalbar, damageBase: 9f, enemiesPerHit: 2);
		PlacePickable(PickableItemNodeFactory.Get(), metalbar, GetPositionFromMarker("ItemSpawn/Metalbar"));
	}
	
	private void PlaceSlowGun() {
		var range = ItemRepository
			.Create<WeaponRangeItem>("Slow Gun", "SG")
			.Configure(ItemConfigManager.SlowGun, AmmoType.Bullet, damageBase: 6f, delayBetweenShots: 0.2f, magazineSize: 22);
		PlacePickable(PickableItemNodeFactory.Get(), range, GetPositionFromMarker("ItemSpawn/Gun"));
	}
	
	private void PlaceGun() {
		var range = ItemRepository
			.Create<WeaponRangeItem>("Gun", "G")
			.Configure(ItemConfigManager.Gun, AmmoType.Bullet, damageBase: 9f, delayBetweenShots: 0.4f, magazineSize: 12);
		PlacePickable(PickableItemNodeFactory.Get(), range, GetPositionFromMarker("ItemSpawn/Gun"));
	}

	private void PlaceShotgun() {
		var range = ItemRepository
			.Create<WeaponRangeItem>("Shotgun", "ShG")
			.Configure(ItemConfigManager.Shotgun, AmmoType.Cartridge, damageBase: 22f, delayBetweenShots: 1f, enemiesPerHit: 2, magazineSize: 8);
		PlacePickable(PickableItemNodeFactory.Get(), range, GetPositionFromMarker("ItemSpawn/Gun"));
	}

	private void PlaceMachineGun() {
		var range = ItemRepository
			.Create<WeaponRangeItem>("MachineGun", "MG")
			.Configure(ItemConfigManager.MachineGun, AmmoType.Bullet, damageBase: 4f, delayBetweenShots: 0.05f, enemiesPerHit: 1, magazineSize: 30, auto: true);
		PlacePickable(PickableItemNodeFactory.Get(), range, GetPositionFromMarker("ItemSpawn/Gun"));
	}

	private void PlacePickable(PickableItemNode pickableItemNode, PickableItem item, Vector2 position) {
		item.LinkNode(pickableItemNode);
		pickableItemNode.AddTo(this, () => pickableItemNode.CharacterBody2D.GlobalPosition = position);
	}

	public void ZombieSpawn(Node scene, Vector2 position) {
		var zombieNode = ZombiePool.Get();
		Console.WriteLine("ZombieSpawn "+zombieNode.GetInstanceId());
		var zombieItem = ItemRepository.Create<NpcItem>("Zombie").Configure(ItemConfigManager.ZombieConfig);
		zombieItem.LinkNode(zombieNode);
		zombieNode.AddTo(scene, () => zombieNode.GlobalPosition = position);
	}

	public ProjectileTrail NewBullet() {
		var projectileTrail = ProjectilePool.Get();
		Console.WriteLine("NewBullet "+projectileTrail.GetInstanceId());
		projectileTrail.AddTo(this);
		return projectileTrail;
	}

	public void AddPlayerToScene() {
		var playerNode = PlayerFactory.Get();
		AddChild(playerNode);
		playerNode.Ready += () => playerNode.GlobalPosition = GetPositionFromMarker("SpawnPlayer");
		ItemRepository.CreatePlayer(playerNode, ItemConfigManager.PlayerConfig);
	}

	public void InstantiateNewZombie() {
		var position = GetNode("EnemySpawn").GetChildren().OfType<Marker2D>().First().GlobalPosition;
		ZombieSpawn(this, position);
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
