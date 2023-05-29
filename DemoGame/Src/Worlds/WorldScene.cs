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
using Betauer.NodePath;
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
	
	[Inject] private ITransient<StageController> StageControllerFactory { get; set; }
	[Inject] private NodePool<PlayerNode> PlayerPool { get; set; }
	[Inject] private NodePool<PickableItemNode> PickableItemPool { get; set; }
	[Inject] private NodePool<ProjectileTrail> ProjectilePool { get; set; }
	[Inject] private NodePool<ZombieNode> ZombiePool { get; set; }

	[NodePath("Focus1")] public Camera2D Focus1;
	[NodePath("EnemySpawn")] private Node _enemySpawn;
	[NodePath("Lights")] private Node _lights;
	[NodePath("ItemSpawn")] private Node _pickableSpawn;
	[NodePath("Stages")] private Node _stages;
	private readonly Node _bulletSpawn = new() { Name = "Bullets" };
	private readonly Node _playerSpawn = new() { Name = "PlayerSpawn" };

	public List<PlayerNode> Players { get; } = new();

	public override void _Ready() {
		PlatformManager.ConfigurePlatformsCollisions();

		GetChildren().OfType<TileMap>().ForEach(PlatformManager.ConfigureTileMapCollision);
		GetChildren().OfType<CanvasModulate>().ForEach(cm => cm.Visible = true);
		var stageController = StageControllerFactory.Create();
		_stages.GetChildren().OfType<Area2D>().ForEach(stageController.ConfigureStage);
		AddChild(_playerSpawn);
		AddChild(_bulletSpawn);
	}
	
	public void StartNewGame() {
		_enemySpawn.GetChildren().OfType<Marker2D>().ForEach(m => {
			if (m.Visible) AddNewZombie(m.GlobalPosition);
		});
		_lights.GetChildren().OfType<PointLight2D>().ForEach(light => {
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
		_pickableSpawn.AddChild(pickableItemNode, () => pickableItemNode.Spawn(position, velocity));
	}

	public void PlayerDrop(PickableGameObject gameObject, Vector2 position, Vector2? velocity = null) {
		PickableItemNode pickableItemNode = PickableItemPool.Get();
		gameObject.LinkNode(pickableItemNode);
		_pickableSpawn.AddChild(pickableItemNode, () => pickableItemNode.PlayerDrop(position, velocity));
	}

	private void LoadPickable(WeaponRangeSaveObject saveObject) {
		if (!saveObject.PickedUp) {
			// State is not saved, so all weapons are loaded as they were spawned by the system, not dropped by the player
			PickableItemNode pickableItemNode = PickableItemPool.Get();
			saveObject.GameObject.LinkNode(pickableItemNode);
			_pickableSpawn.AddChild(pickableItemNode, () => pickableItemNode.Spawn(saveObject.GlobalPosition, saveObject.Velocity));
		}
	}

	private void LoadPickable(WeaponMeleeSaveObject saveObject) {
		if (!saveObject.PickedUp) {
			// State is not saved, so all weapons are loaded as they were spawned by the system, not dropped by the player
			PickableItemNode pickableItemNode = PickableItemPool.Get();
			saveObject.GameObject.LinkNode(pickableItemNode);
			_pickableSpawn.AddChild(pickableItemNode, () => pickableItemNode.Spawn(saveObject.GlobalPosition, saveObject.Velocity));
		}
	}

	public ProjectileTrail NewBullet() {
		var projectileTrail = ProjectilePool.Get();
		_bulletSpawn.AddChild(projectileTrail);
		return projectileTrail;
	}

	public void LoadGame(MySaveGameConsumer saveGameConsumer) {
		saveGameConsumer.ConsumeAll<ZombieSaveObject>(LoadZombie);
		saveGameConsumer.ConsumeAll<WeaponMeleeSaveObject>(LoadPickable);
		saveGameConsumer.ConsumeAll<WeaponRangeSaveObject>(LoadPickable);
		// saveGameConsumer.Verify();
		if (saveGameConsumer.Pending.Count > 0) {
			Console.WriteLine("Still pending objects to load");
			// throw new Exception("Still pending objects to load");
		}
	}

	public void AddNewZombie(Vector2 position) {
		var zombieNode = ZombiePool.Get();
		GameObjectRepository.Create<ZombieGameObject>("Zombie").LinkNode(zombieNode);
		_enemySpawn.AddChild(zombieNode, () => {
			zombieNode.GlobalPosition = position;
			zombieNode.Velocity = Vector2.Zero; // avoid weird movement when spawning from a pool instance who had a big velocity
		});
	}

	public void LoadZombie(ZombieSaveObject npcSaveObject) {
		var zombieNode = ZombiePool.Get();
		npcSaveObject.GameObject.LinkNode(zombieNode);
		_enemySpawn.AddChild(zombieNode, () => {
			zombieNode.GlobalPosition = npcSaveObject.GlobalPosition;
			zombieNode.Velocity = npcSaveObject.Velocity; // X Velocity doesn't matter because the MeleeAI state is not save, so it starts from Idle
			zombieNode.LateralState.IsFacingRight = npcSaveObject.IsFacingRight;
		});
	}

	public PlayerNode AddNewPlayer(PlayerMapping playerMapping) {
		var name = $"Player{playerMapping.Player}";
		var playerGameObject = GameObjectRepository.Create<PlayerGameObject>(name, name);
		var playerNode = CreatePlayerNode(playerGameObject, playerMapping);

		var inventoryName = $"PlayerInventory{playerMapping.Player}";
		var inventoryGameObject = GameObjectRepository.Create<InventoryGameObject>(inventoryName, inventoryName);
		playerNode.Inventory.InventoryGameObject = inventoryGameObject;
		
		_playerSpawn.AddChild(playerNode, () => {
			playerNode.GlobalPosition = GetPositionFromMarker("SpawnPlayer");
			playerNode.PlatformBody.Motion = Vector2.Zero; // avoid weird movement when spawning from a pool instance who had a big velocity
		});
		return playerNode;
	}

	public PlayerNode LoadPlayer(PlayerMapping playerMapping, PlayerSaveObject saveObject, InventorySaveObject inventorySaveObject) {
		var playerNode = CreatePlayerNode(saveObject.GameObject, playerMapping);
		playerNode.Inventory.InventoryGameObject = inventorySaveObject.GameObject;
		
		_playerSpawn.AddChild(playerNode, () => {
			playerNode.PlatformBody.Motion = saveObject.Velocity;
			playerNode.GlobalPosition = saveObject.GlobalPosition;
			playerNode.LateralState.IsFacingRight = saveObject.IsFacingRight;
			playerNode.Inventory.EquipCurrent();
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

	public bool IsAnyPlayer(CharacterBody2D player) {
		return Players.Find(p => p.CharacterBody2D == player) != null;
	}

	public PlayerNode ClosestPlayer(Vector2 globalPosition) {
		return Players.OrderBy(p => p.GlobalPosition.DistanceSquaredTo(globalPosition)).First();
	}

	public void InstantiateNewZombie() {
		var position = _enemySpawn.GetChildren().OfType<Marker2D>().First().GlobalPosition;
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
				if (player is CharacterBody2D character && IsAnyPlayer(character)) CandleOn(light);
			});
	}

	private void CandleOn(PointLight2D light) {
		light.Enabled = true;
		light.Color = new Color("ffd1c8");
		light.TextureScale = 0.8f;
		// light.ShadowEnabled = false;
	}
}
