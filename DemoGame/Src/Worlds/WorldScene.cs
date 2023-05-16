using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Application.Lifecycle.Pool;
using Betauer.Application.Persistent;
using Betauer.Camera;
using Betauer.Core;
using Betauer.Core.Nodes;
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
	[Inject] private JoypadPlayersMapping JoypadPlayersMapping { get; set; }
	
	[Inject] private IFactory<StageController> StageControllerFactory { get; set; }
	[Inject] private NodePool<PlayerNode> PlayerPool { get; set; }
	[Inject] private NodePool<PickableItemNode> PickableItemPool { get; set; }
	[Inject] private NodePool<ProjectileTrail> ProjectilePool { get; set; }
	[Inject] private NodePool<ZombieNode> ZombiePool { get; set; }

	public List<PlayerNode> Players { get; } = new();

	public WorldScene() {
		var bullets = new Node();
		bullets.Name = "Bullets";
		AddChild(bullets);
	}

	public override void _Ready() {
		GetChildren().OfType<CanvasModulate>().ForEach(cm => cm.Visible = true);
		PlatformManager.ConfigurePlatformsCollisions();
		var stageController = StageControllerFactory.Get();
		GetNode<Node>("Stages").GetChildren().OfType<Area2D>().ForEach(stageController.ConfigureStage);
		GetChildren().OfType<TileMap>().ForEach(PlatformManager.ConfigureTileMapCollision);
	}
	
	public void StartNewGame() {
		GetNode("EnemySpawn").GetChildren().OfType<Marker2D>().ForEach(m => {
			if (m.Visible) AddNewZombie(m.GlobalPosition);
		});
		GetNode<Node>("Lights").GetChildren().OfType<PointLight2D>().ForEach(light => {
			if (light.Name.ToString().StartsWith("Candle")) CandleOff(light);
		});
		PlaceMetalbar();
		PlaceKnife();
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
	
	private void PlaceKnife() {
		var metalbar = GameObjectRepository
			.Create<WeaponMeleeGameObject>("Knife")
			.Configure(ConfigManager.Knife, damageBase: 12f, enemiesPerHit: 1);
		PlacePickable(metalbar, GetPositionFromMarker("ItemSpawn/Metalbar")+ new Vector2(100,0));
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

	public ProjectileTrail NewBullet() {
		var projectileTrail = ProjectilePool.Get();
		AddChild(projectileTrail);
		return projectileTrail;
	}

	public void LoadGame(MySaveGameConsumer saveGameConsumer) {
		saveGameConsumer.ConsumeAll<ZombieSaveObject>(LoadZombie);
		// saveGameConsumer.Verify();
		if (saveGameConsumer.Pending.Count > 0) {
			Console.WriteLine("Still pending objects to load");
			// throw new Exception("Still pending objects to load");
		}
	}

	public void AddNewZombie(Vector2 position) {
		var zombieNode = ZombiePool.Get();
		GameObjectRepository.Create<ZombieGameObject>("Zombie").LinkNode(zombieNode);
		GetNode("EnemySpawn")!.AddChild(zombieNode, () => zombieNode.GlobalPosition = position);
	}

	public void LoadZombie(ZombieSaveObject npcSaveObject) {
		var zombieNode = ZombiePool.Get();
		npcSaveObject.GameObject.LinkNode(zombieNode);
		GetNode("EnemySpawn")!.AddChild(zombieNode, () => zombieNode.GlobalPosition = npcSaveObject.GlobalPosition);
	}

	public PlayerNode AddNewPlayer(PlayerMapping playerMapping) {
		var name = $"Player{playerMapping.Player}";
		var playerGameObject = GameObjectRepository.Create<PlayerGameObject>(name, name);
		var playerNode = CreatePlayerNode(playerGameObject, playerMapping);
		this.AddChild(playerNode, () => {
			playerNode.GlobalPosition = GetPositionFromMarker("SpawnPlayer"); // + new Vector2(playerMapping.Player * 100, 0);
		});
		return playerNode;
	}

	public PlayerNode LoadPlayer(PlayerMapping playerMapping, PlayerSaveObject saveObject) {
		var playerNode = CreatePlayerNode(saveObject.GameObject, playerMapping);
		this.AddChild(playerNode, () => {
			playerNode.GlobalPosition = saveObject.GlobalPosition;
		});
		return playerNode;
	}

	private PlayerNode CreatePlayerNode(PlayerGameObject playerGameObject, PlayerMapping playerMapping) {
		var playerNode = PlayerPool.Get();
		playerGameObject.LinkNode(playerNode);
		playerNode.SetPlayerMapping(playerMapping);
		Players.Add(playerNode);
		return playerNode;
	}

	public bool IsPlayer(CharacterBody2D player) {
		return Players.Find(p => p.CharacterBody2D == player) != null;
	}


	public PlayerNode ClosestPlayer(Vector2 globalPosition) {
		return Players.OrderBy(p => p.GlobalPosition.DistanceSquaredTo(globalPosition)).First();
	}

	public void InstantiateNewZombie() {
		var position = GetNode("EnemySpawn").GetChildren().OfType<Marker2D>().First().GlobalPosition;
		AddNewZombie(position);
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
