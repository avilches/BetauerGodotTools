using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Betauer.Application.Lifecycle;
using Betauer.DI;
using Betauer.Core.Nodes;
using Betauer.Core.Pool;
using Betauer.Core.Signal;
using Betauer.DI.Factory;
using Veronenger.Character.Npc;
using Godot;
using Veronenger.Character.Player;
using Veronenger.Config;
using Veronenger.Persistent;
using Veronenger.Transient;
using Veronenger.UI;
using Veronenger.Worlds;

namespace Veronenger.Managers;

[Singleton]
public class Game {
	[Inject] private SceneTree SceneTree { get; set; }
	[Inject] private ItemRepository ItemRepository { get; set; }
	[Inject] private IFactory<HUD> HudScene { get; set; }
	[Inject] private ItemConfigManager ItemConfigManager { get; set; }
	[Inject] private StageManager StageManager { get; set; }
	[Inject] private IFactory<World3> World3 { get; set; }
	[Inject] private IFactory<PlayerNode> PlayerFactory { get; set; }

	[Inject] private GameLoaderContainer GameLoaderContainer { get; set; }
	[Inject] private PoolManager<INodeLifecycle> PoolManager { get; set; }
	[Inject] private IPool<ProjectileTrail> ProjectilePool { get; set; }
	[Inject] private IPool<ZombieNode> ZombiePool { get; set; }

	private World3 _currentGameScene;

	public async Task Start() {
		StageManager.ClearState();
		await StartWorld3();
	}

	public void StartNew() {
		// _currentGameScene = MainResourceLoader.CreateWorld2Empty();
		var tileMap = _currentGameScene.GetNode<TileMap>("RealTileMap");
		new WorldGenerator().Generate(tileMap);
		AddPlayerToScene();
		SceneTree.Root.AddChildDeferred(_currentGameScene);
	}

	public async Task StartWorld3() {
		ItemRepository.Clear();
		var loadTime = await GameLoaderContainer.LoadGameResources();
		Console.WriteLine($"Game load:{loadTime.TotalMilliseconds}ms");
		
		GD.PushWarning("World3 creation start");
		_currentGameScene = World3.Get();
		GD.PushWarning("World3 creation end");
		
		AddPlayerToScene();
		SceneTree.Root.AddChild(_currentGameScene);
		await SceneTree.AwaitProcessFrame();

		var bullets = new Node();
		bullets.Name = "Bullets";
		_currentGameScene.AddChild(bullets);
		HudScene.Get().StartGame();
	}

	public void ZombieSpawn(Node scene, Vector2 position) {
		var zombieNode = ZombiePool.Get();
		Console.WriteLine("ZombieSpawn "+zombieNode.GetInstanceId());
		var zombieItem = ItemRepository.Create<NpcItem>("Zombie").Configure(ItemConfigManager.ZombieConfig);
		zombieItem.LinkNode(zombieNode);
		zombieNode.AddToScene(scene, () => zombieNode.GlobalPosition = position);
	}

	public ProjectileTrail NewBullet() {
		var projectileTrail = ProjectilePool.Get();
		Console.WriteLine("NewBullet "+projectileTrail.GetInstanceId());
		projectileTrail.AddToScene(_currentGameScene, null);
		return projectileTrail;
	}

	private void AddPlayerToScene() {
		var playerNode = PlayerFactory.Get();
		_currentGameScene.AddChild(playerNode);
		playerNode.Ready += () => playerNode.GlobalPosition = _currentGameScene.GetPositionFromMarker("SpawnPlayer");
		ItemRepository.SetPlayer(playerNode);
	}

	public async Task End() {
		HudScene.Get().EndGame();
		// FreeSceneAndKeepPoolData();
		FreeSceneAndUnloadResources();
		_currentGameScene.QueueFree();
		_currentGameScene = null;
		await SceneTree.AwaitProcessFrame();
		GC.GetTotalMemory(true);

		Node.PrintOrphanNodes();
	}

	private void FreeSceneAndUnloadResources() {
		// To ensure the pool nodes are freed along with the scene:
		
		// 1. All busy elements are still attached to the tree and will be destroyed with the scene, so we don't need to
		// do anything with them.
		// But the non busy and valid element are outside of the scene tree in the pool, so, loop them and free them:
		PoolManager.ForEachElementInAllPools(e => e.Free());
		// 2. Remove the data from the pool to avoid having references to busy elements which are going to die with the scene
		PoolManager.Clear();
		GameLoaderContainer.UnloadGameResources();
	}

	private void FreeSceneAndKeepPoolData() {
		// This line keeps the godot nodes in the pool removing them from scene, because the scene is going to be freed
		PoolManager.ForEachElementInAllPools(e => e.RemoveFromScene());
	}

	public void InstantiateNewZombie() {
		var position = _currentGameScene.GetNode("EnemySpawn").GetChildren().OfType<Marker2D>().First().GlobalPosition;
		ZombieSpawn(_currentGameScene, position);
	}
}
