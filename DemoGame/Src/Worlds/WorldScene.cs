using System.Collections.Generic;
using System.Linq;
using Betauer.Application.Persistent;
using Betauer.Core;
using Betauer.Core.Nodes;
using Betauer.Core.Pool;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Betauer.Input.Joypad;
using Godot;
using Veronenger.Character.Npc;
using Veronenger.Character.Player;
using Veronenger.Config;
using Veronenger.Managers;
using Veronenger.Persistent;
using Veronenger.Transient;

namespace Veronenger.Worlds;

public partial class WorldScene : Node {
	[Inject] private ConfigManager ConfigManager { get; set; }
	[Inject] private GameObjectRepository GameObjectRepository { get; set; }
	[Inject] private PlatformManager PlatformManager { get; set; }
	[Inject] private StageManager StageManager { get; set; }
	[Inject] private IFactory<PlayerNode> PlayerFactory { get; set; }
	[Inject] private IPool<PickableItemNode> PickableItemPool { get; set; }
	[Inject] private IPool<ProjectileTrail> ProjectilePool { get; set; }
	[Inject] private IPool<ZombieNode> ZombiePool { get; set; }
	[Inject] private PlayerConfig PlayerConfig { get; set; }

	public List<PlayerNode> Players { get; } = new();

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
		var metalbar = GameObjectRepository
			.Create<WeaponMeleeGameObject>("Metalbar")
			.Configure(ConfigManager.Metalbar, damageBase: 9f, enemiesPerHit: 2);
		PlacePickable(metalbar, GetPositionFromMarker("ItemSpawn/Metalbar"));
	}
	
	private void PlaceSlowGun() {
		var range = GameObjectRepository
			.Create<WeaponRangeGameObject>("Slow Gun")
			.Configure(ConfigManager.SlowGun, AmmoType.Bullet, damageBase: 6f, delayBetweenShots: 0.2f, magazineSize: 22);
		PlacePickable(range, GetPositionFromMarker("ItemSpawn/Gun"));
	}
	
	private void PlaceGun() {
		var range = GameObjectRepository
			.Create<WeaponRangeGameObject>("Gun")
			.Configure(ConfigManager.Gun, AmmoType.Bullet, damageBase: 9f, delayBetweenShots: 0.3f, magazineSize: 12);
		PlacePickable(range, GetPositionFromMarker("ItemSpawn/Gun"));
	}

	private void PlaceShotgun() {
		var range = GameObjectRepository
			.Create<WeaponRangeGameObject>("Shotgun")
			.Configure(ConfigManager.Shotgun, AmmoType.Cartridge, damageBase: 22f, delayBetweenShots: 0.5f, enemiesPerHit: 2, magazineSize: 8);
		PlacePickable(range, GetPositionFromMarker("ItemSpawn/Gun"));
	}

	private void PlaceMachineGun() {
		var range = GameObjectRepository
			.Create<WeaponRangeGameObject>("MachineGun")
			.Configure(ConfigManager.MachineGun, AmmoType.Bullet, damageBase: 4f, delayBetweenShots: 0.05f, enemiesPerHit: 1, magazineSize: 30, auto: true);
		PlacePickable(range, GetPositionFromMarker("ItemSpawn/Gun"));
	}

	public void PlacePickable(PickableGameObject gameObject, Vector2 position, Vector2? velocity = null) {
		PickableItemNode pickableItemNode = PickableItemPool.Get();
		gameObject.LinkNode(pickableItemNode);
		this.AddChild(pickableItemNode, () => pickableItemNode.Spawn(position, velocity));
	}

	public void PlayerDrop(PickableGameObject gameObject, Vector2 position, Vector2? velocity = null) {
		PickableItemNode pickableItemNode = PickableItemPool.Get();
		gameObject.LinkNode(pickableItemNode);
		this.AddChild(pickableItemNode, () => pickableItemNode.PlayerDrop(position, velocity));
	}

	public void ZombieSpawn(Node scene, Vector2 position) {
		var zombieNode = ZombiePool.Get();
		var zombieItem = GameObjectRepository.Create<NpcGameObject>("Zombie").Configure(ConfigManager.ZombieConfig);
		zombieItem.LinkNode(zombieNode);
		scene.AddChild(zombieNode, () => zombieNode.GlobalPosition = position);
	}

	public ProjectileTrail NewBullet() {
		var projectileTrail = ProjectilePool.Get();
		AddChild(projectileTrail);
		return projectileTrail;
	}

	public PlayerNode AddPlayerToScene(PlayerMapping playerMapping, SubViewport subViewport) {
		var playerNode = PlayerFactory.Get();
		playerNode.Name = $"Player{playerMapping.Player}";
		playerNode.SetPlayerMapping(playerMapping);
		playerNode.Ready += () => {
			playerNode.SetViewport(subViewport);
			playerNode.GlobalPosition = GetPositionFromMarker("SpawnPlayer") + new Vector2(playerMapping.Player * 100, 0);
			playerNode.Camera2D.MakeCurrent();
		};
		CreatePlayer(playerNode, ConfigManager.PlayerConfig);
		AddChild(playerNode);
		
		return playerNode;
	}

	public bool IsPlayer(CharacterBody2D player) {
		return Players.Find(p => p.CharacterBody2D == player) != null;
	}

	public void RemoveAllPlayers() {
		Players.Clear();
	}

	public PlayerGameObject CreatePlayer(PlayerNode playerNode, PlayerConfig playerConfig) {
		Players.Add(playerNode);
		var playerItem = GameObjectRepository.Create<PlayerGameObject>("Player1").Configure(playerConfig);
		playerItem.LinkNode(playerNode);
		return playerItem;
	}

	 
	public PlayerNode ClosestPlayer(Vector2 globalPosition) {
		return Players.OrderBy(p => p.GlobalPosition.DistanceSquaredTo(globalPosition)).First();
	}

	public void InstantiateNewZombie() {
		var position = GetNode("EnemySpawn").GetChildren().OfType<Marker2D>().First().GlobalPosition;
		ZombieSpawn(this, position);
	}

	public Vector2 GetPositionFromMarker(string path, bool freeMarker = false) {
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
				if (player is CharacterBody2D character && IsPlayer(character)) CandleOn(light);
			});
	}

	private void CandleOn(PointLight2D light) {
		light.Enabled = true;
		light.Color = new Color("ffd1c8");
		light.TextureScale = 0.8f;
		// light.ShadowEnabled = false;
	}
}
