using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Application.Persistent;
using Betauer.Camera;
using Betauer.Core;
using Betauer.Core.Data;
using Betauer.Core.Nodes;
using Betauer.Core.Pool;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Betauer.Input.Joypad;
using Betauer.NodePath;
using Godot;
using Veronenger.Game.Platform.Character.InputActions;
using Veronenger.Game.Platform.Character.Npc;
using Veronenger.Game.Platform.Character.Player;
using Veronenger.Game.Platform.Items;

namespace Veronenger.Game.Platform.World;

public partial class PlatformWorld : Node, IInjectable {
	[Inject] private ItemsManager ItemsManager { get; set; }
	[Inject] private GameObjectRepository GameObjectRepository { get; set; }
	[Inject] private PlatformConfig PlatformConfig { get; set; }
	
	[Inject] private ITransient<StageController> StageControllerFactory { get; set; }
	[Inject] private ITransient<PlayerNode> Player { get; set; }
	[Inject] private PlatformBus PlatformBus { get; set; }
	[Inject] private NodePool<PickableItemNode> PickableItemPool { get; set; }
	[Inject] private NodePool<ProjectileTrail> ProjectilePool { get; set; }
	[Inject] private NodePool<ZombieNode> ZombiePool { get; set; }

	[NodePath("EnemySpawn")] private Node _enemySpawn;
	[NodePath("Lights")] private Node _lights;
	[NodePath("ItemSpawn")] private Node _pickableSpawn;
	[NodePath("Stages")] private Node _stages;
	private readonly Node _bulletSpawn = new() { Name = "Bullets" };
	private readonly Node _playerSpawn = new() { Name = "PlayerSpawn" };

	public List<PlayerNode> Players { get; } = new();

	public void PostInject() {
		PlatformBus.Subscribe<PlatformCommand>(this, (e) => {
			if (e.Type == PlatformCommandType.SpawnZombie) InstantiateNewZombie();
		});
		
		PlatformBus.Subscribe<PlayerDropEvent>(this, (e) => PlayerDrop(e.Item, e.GlobalPosition, e.DropVelocity));
	}


	public override void _Ready() {
		PlatformConfig.ConfigurePlatformsCollisions();

		GetChildren().OfType<TileMap>().ForEach(PlatformConfig.ConfigureTileMapCollision);
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
			.Configure(ItemsManager.Metalbar, damageBase: 9f, enemiesPerHit: 2);
		PlacePickable(metalbar, GetPositionFromMarker("ItemSpawn/Metalbar"));
	}
	
	private void PlaceKnife() {
		var metalbar = GameObjectRepository
			.Create<WeaponMeleeGameObject>("Knife")
			.Configure(ItemsManager.Knife, damageBase: 12f, enemiesPerHit: 1);
		PlacePickable(metalbar, GetPositionFromMarker("ItemSpawn/Metalbar")+ new Vector2(100,0));
	}
	
	private void PlaceSlowGun() {
		var range = GameObjectRepository
			.Create<WeaponRangeGameObject>("Slow Gun")
			.Configure(ItemsManager.SlowGun, AmmoType.Bullet, damageBase: 6f, delayBetweenShots: 0.2f, magazineSize: 22);
		PlacePickable(range, GetPositionFromMarker("ItemSpawn/Gun"));
	}
	
	private void PlaceGun() {
		var range = GameObjectRepository
			.Create<WeaponRangeGameObject>("Gun")
			.Configure(ItemsManager.Gun, AmmoType.Bullet, damageBase: 9f, delayBetweenShots: 0.3f, magazineSize: 12);
		PlacePickable(range, GetPositionFromMarker("ItemSpawn/Gun"));
	}

	private void PlaceShotgun() {
		var range = GameObjectRepository
			.Create<WeaponRangeGameObject>("Shotgun")
			.Configure(ItemsManager.Shotgun, AmmoType.Cartridge, damageBase: 22f, delayBetweenShots: 0.5f, enemiesPerHit: 2, magazineSize: 8);
		PlacePickable(range, GetPositionFromMarker("ItemSpawn/Gun"));
	}

	private void PlaceMachineGun() {
		var range = GameObjectRepository
			.Create<WeaponRangeGameObject>("MachineGun")
			.Configure(ItemsManager.MachineGun, AmmoType.Bullet, damageBase: 4f, delayBetweenShots: 0.05f, enemiesPerHit: 1, magazineSize: 30, auto: true);
		PlacePickable(range, GetPositionFromMarker("ItemSpawn/Gun"));
	}

	public void PlacePickable(PickableGameObject gameObject, Vector2 position, Vector2? velocity = null) {
		PickableItemNode pickableItemNode = PickableItemPool.GetOrCreate();
		gameObject.LinkNode(pickableItemNode);
		_pickableSpawn.AddChild(pickableItemNode, () => pickableItemNode.Spawn(position, velocity));
	}

	public void PlayerDrop(PickableGameObject gameObject, Vector2 position, Vector2? velocity = null) {
		PickableItemNode pickableItemNode = PickableItemPool.GetOrCreate();
		gameObject.LinkNode(pickableItemNode);
		_pickableSpawn.AddChild(pickableItemNode, () => pickableItemNode.PlayerDrop(position, velocity));
	}

	private void LoadPickable(WeaponRangeSaveObject saveObject) {
		if (!saveObject.PickedUp) {
			// State is not saved, so all weapons are loaded as they were spawned by the system, not dropped by the player
			PickableItemNode pickableItemNode = PickableItemPool.GetOrCreate();
			saveObject.GameObject.LinkNode(pickableItemNode);
			_pickableSpawn.AddChild(pickableItemNode, () => pickableItemNode.Spawn(saveObject.GlobalPosition, saveObject.Velocity));
		}
	}

	private void LoadPickable(WeaponMeleeSaveObject saveObject) {
		if (!saveObject.PickedUp) {
			// State is not saved, so all weapons are loaded as they were spawned by the system, not dropped by the player
			PickableItemNode pickableItemNode = PickableItemPool.GetOrCreate();
			saveObject.GameObject.LinkNode(pickableItemNode);
			_pickableSpawn.AddChild(pickableItemNode, () => pickableItemNode.Spawn(saveObject.GlobalPosition, saveObject.Velocity));
		}
	}

	public ProjectileTrail NewBullet() {
		var projectileTrail = ProjectilePool.GetOrCreate();
		_bulletSpawn.AddChild(projectileTrail);
		return projectileTrail;
	}

	public void LoadGame(PlatformSaveGameConsumer saveGameConsumer) {
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
		var zombieNode = ZombiePool.GetOrCreate();
		GameObjectRepository.Create<ZombieGameObject>("Zombie").LinkNode(zombieNode);
		_enemySpawn.AddChild(zombieNode, () => {
			zombieNode.GlobalPosition = position;
			zombieNode.Velocity = Vector2.Zero; // avoid weird movement when spawning from a pool instance who had a big velocity
		});
	}

	public void LoadZombie(ZombieSaveObject npcSaveObject) {
		var zombieNode = ZombiePool.GetOrCreate();
		npcSaveObject.GameObject.LinkNode(zombieNode);
		_enemySpawn.AddChild(zombieNode, () => {
			zombieNode.GlobalPosition = npcSaveObject.GlobalPosition;
			zombieNode.Velocity = npcSaveObject.Velocity; // X Velocity doesn't matter because the MeleeAI state is not save, so it starts from Idle
			zombieNode.LateralState.IsFacingRight = npcSaveObject.IsFacingRight;
		});
	}

	public PlayerNode AddNewPlayer(PlatformPlayerActions actions) {
		var name = $"Player{actions.PlayerId}";
		var playerGameObject = GameObjectRepository.Create<PlayerGameObject>(name, name);
		var playerNode = CreatePlayerNode(playerGameObject);
		playerNode.SetPlayerActions(actions);

		var inventoryName = $"PlayerInventory{actions.PlayerId}";
		var inventoryGameObject = GameObjectRepository.Create<InventoryGameObject>(inventoryName, inventoryName);
		playerNode.Inventory.InventoryGameObject = inventoryGameObject;
		
		_playerSpawn.AddChild(playerNode, () => {
			playerNode.GlobalPosition = GetPositionFromMarker("SpawnPlayer");
			playerNode.PlatformBody.Motion = Vector2.Zero; // avoid weird movement when spawning from a pool instance who had a big velocity
		});
		return playerNode;
	}

	public PlayerNode LoadPlayer(PlayerSaveObject saveObject, InventorySaveObject inventorySaveObject, PlatformPlayerActions actions) {
		var playerNode = CreatePlayerNode(saveObject.GameObject);
		playerNode.SetPlayerActions(actions);
		playerNode.Inventory.InventoryGameObject = inventorySaveObject.GameObject;
		
		_playerSpawn.AddChild(playerNode, () => {
			playerNode.PlatformBody.Motion = saveObject.Velocity;
			playerNode.GlobalPosition = saveObject.GlobalPosition;
			playerNode.LateralState.IsFacingRight = saveObject.IsFacingRight;
			playerNode.Inventory.EquipCurrent();
		});
		return playerNode;
	}

	private PlayerNode CreatePlayerNode(PlayerGameObject playerGameObject) {
		var playerNode = Player.Create();
		playerGameObject.LinkNode(playerNode);
		Players.Add(playerNode);
		return playerNode;
	}

	public bool IsAnyPlayer(CharacterBody2D player) {
		return Players.Find(p => p.CharacterBody2D == player) != null;
	}

	public PlayerNode FindClosestPlayer(Vector2 globalPosition) {
		return FastSearch.FindMinimumValue(Players, p => p.GlobalPosition.DistanceSquaredTo(globalPosition));
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
			?.OnBodyEntered(CollisionLayerConstants.LayerPlayerBody, (player) => {
				if (player is CharacterBody2D character && IsAnyPlayer(character)) CandleOn(light);
			});
	}

	private void CandleOn(PointLight2D light) {
		light.Enabled = true;
		light.Color = new Color("ffd1c8");
		light.TextureScale = 0.8f;
		// light.ShadowEnabled = false;
	}

	public void Release(ZombieNode p) {
		ZombiePool.Release(p);
	}

	public void Release(ProjectileTrail p) {
		ProjectilePool.Release(p);
	}

}
