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
		PlaceGun();
		// TODO: add these weapons
		// ItemRepository.Create<WeaponMeleeItem>("Knife", "K1").Configure();
		
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
	}

	private void PlaceMetalbar() {
		var metalbar = ItemRepository.Create<WeaponMeleeItem>("Metalbar", "M1").Configure(ItemConfigManager.Metalbar, 9f);
		PlacePickable(PickableItemNodeFactory.Get(), metalbar, GetPositionFromMarker("ItemSpawn/Metalbar"));
	}
	
	private void PlaceGun() {
		var slowGun = ItemRepository.Create<WeaponRangeItem>("Slow Gun", "SG").Configure(ItemConfigManager.SlowGun, 6f);
		slowGun.DelayBetweenShots = 0.2f;
		PlacePickable(PickableItemNodeFactory.Get(), slowGun, GetPositionFromMarker("ItemSpawn/Gun"));
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
