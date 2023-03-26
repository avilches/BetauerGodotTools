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
	[Inject] private IFactory<WorldScene> World3 { get; set; }

	[Inject] private GameLoaderContainer GameLoaderContainer { get; set; }
	[Inject] private PoolContainer<INodePoolLifecycle> PoolContainer { get; set; }

	public WorldScene WorldScene { get; private set; }

	public async Task Start() {
		StageManager.ClearState();
		await StartWorld3();
	}

	public void StartNew() {
		// _currentGameScene = MainResourceLoader.CreateWorld2Empty();
		var tileMap = WorldScene.GetNode<TileMap>("RealTileMap");
		new WorldGenerator().Generate(tileMap);
		// AddPlayerToScene();
		SceneTree.Root.AddChildDeferred(WorldScene);
	}

	public async Task StartWorld3() {
		ItemRepository.Clear();
		var loadTime = await GameLoaderContainer.LoadGameResources();
		Console.WriteLine($"Game load:{loadTime.TotalMilliseconds}ms");
		
		GD.PushWarning("World3 creation start");
		WorldScene = World3.Get();
		GD.PushWarning("World3 creation end");
		
		WorldScene.AddPlayerToScene();
		SceneTree.Root.AddChild(WorldScene);
		await SceneTree.AwaitProcessFrame();

		var bullets = new Node();
		bullets.Name = "Bullets";
		WorldScene.AddChild(bullets);
		HudScene.Get().StartGame();
	}

	public async Task End() {
		HudScene.Get().EndGame();
		// FreeSceneAndKeepPoolData();
		FreeSceneAndUnloadResources();
		WorldScene.QueueFree();
		WorldScene = null;
		await SceneTree.AwaitProcessFrame();
		GC.GetTotalMemory(true);

		Node.PrintOrphanNodes();
	}

	private void FreeSceneAndUnloadResources() {
		// To ensure the pool nodes are freed along with the scene:
		
		// 1. All busy elements are still attached to the tree and will be destroyed with the scene, so we don't need to
		// do anything with them.
		// But the non busy and valid element are outside of the scene tree in the pool, so, loop them and free them:
		PoolContainer.ForEachElementInAllPools(e => ((Node)e).Free());
		// 2. Remove the data from the pool to avoid having references to busy elements which are going to die with the scene
		PoolContainer.Clear();
		GameLoaderContainer.UnloadGameResources();
	}

	private void FreeSceneAndKeepPoolData() {
		// This line keeps the godot nodes in the pool removing them from scene, because the scene is going to be freed
		PoolContainer.ForEachElementInAllPools(e => ((Node)e).RemoveFromParent());
	}
}
