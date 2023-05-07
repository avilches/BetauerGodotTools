using System;
using System.Threading.Tasks;
using Betauer.Application.Lifecycle;
using Betauer.Application.Persistent;
using Betauer.Core.Nodes;
using Betauer.Core.Pool.Lifecycle;
using Betauer.Core.Signal;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Betauer.Input;
using Betauer.Input.Joypad;
using Betauer.Nodes;
using Godot;
using Veronenger.Config;
using Veronenger.UI;
using Veronenger.Worlds;

namespace Veronenger.Managers;

[Singleton]
public class Game : IInjectable {
	[Inject] private SceneTree SceneTree { get; set; }
	[Inject] private GameObjectRepository GameObjectRepository { get; set; }
	[Inject] private IFactory<HUD> HudScene { get; set; }
	[Inject] private ConfigManager ConfigManager { get; set; }
	[Inject] private StageManager StageManager { get; set; }
	[Inject] private IFactory<WorldScene> World3 { get; set; }

	[Inject] private GameLoaderContainer GameLoaderContainer { get; set; }
	[Inject] private PoolContainer<IPoolLifecycle> PoolContainer { get; set; }
	[Inject] private InputActionsContainer PlayerActionsContainer { get; set; }
	[Inject] private UiActionsContainer UiActionsContainer { get; set; }
	[Inject] private JoypadPlayersMapping JoypadPlayersMapping { get; set; }

	public const int MaxPlayer = 2;
	
	public WorldScene WorldScene { get; private set; }
	private InputEventHandler newPlayerHandler { get; }

	public Game() {
		newPlayerHandler = new InputEventHandler(NewPlayerDetector, Node.ProcessModeEnum.Pausable, "NewPlayerDetector");
		newPlayerHandler.Disable();
	}

	public void PostInject() {
		DefaultNodeHandler.Instance.OnInput(newPlayerHandler);
		PlayerActionsContainer.Disable(); // The real actions are cloned per player in player.Connect()		
	}

	public async Task Start() {
		JoypadPlayersMapping.RemoveAllPlayers();
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

	private void NewPlayerDetector(InputEvent e) {
		if (e is InputEventJoypadButton button && !JoypadPlayersMapping.IsJoypadUsed(button.Device)) {
			AddPlayerToScene(button.Device);
			DefaultNodeHandler.Instance.GetViewport().GetViewport().SetInputAsHandled();
			if (JoypadPlayersMapping.Players == MaxPlayer) {
				newPlayerHandler.Disable();				
			}
		}
	}

	public async Task StartWorld3() {
		GameObjectRepository.Clear();
		await GameLoaderContainer.LoadGameResources();
		
		GD.PushWarning("World3 creation start");
		WorldScene = World3.Get();
		GD.PushWarning("World3 creation end");

		AddPlayerToScene(UiActionsContainer.CurrentJoyPad);
		// This ensures the player who starts the game is the player who control the UI forever
		UiActionsContainer.SetJoypad(UiActionsContainer.CurrentJoyPad);
		SceneTree.Root.AddChild(WorldScene);
		await SceneTree.AwaitProcessFrame();

		var bullets = new Node();
		bullets.Name = "Bullets";
		WorldScene.AddChild(bullets);
		HudScene.Get().StartGame();
		newPlayerHandler.Enable();
	}

	public void AddPlayerToScene(int joypad) {
		var playerMapping = JoypadPlayersMapping.AddPlayer();
		playerMapping.SetJoypadId(joypad);
		WorldScene.AddPlayerToScene(playerMapping);
	}

	public async Task End() {
		newPlayerHandler.Disable();
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
