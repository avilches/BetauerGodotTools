using System;
using System.Threading.Tasks;
using Betauer.Application.Lifecycle.Pool;
using Betauer.Application.Monitor;
using Betauer.Application.Persistent;
using Betauer.Application.Persistent.Json;
using Betauer.Application.SplitScreen;
using Betauer.Core;
using Betauer.Core.Nodes;
using Betauer.Core.Signal;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Betauer.Input;
using Betauer.Input.Joypad;
using Godot;
using Veronenger.Game.UI;

namespace Veronenger.Game.RTS.World;

public partial class RtsGameView : Control, IInjectable, IGameView {

	[Inject] private DebugOverlayManager DebugOverlayManager { get; set; }
	[Inject] private SceneTree SceneTree { get; set; }
	[Inject] private GameObjectRepository GameObjectRepository { get; set; }
	[Inject] private JsonGameLoader<RtsSaveGameMetadata> RtsGameObjectLoader { get; set; }
	[Inject] private ITransient<RtsWorld> RtsWorldFactory { get; set; }
	// [Inject] private ITransient<HudCanvas> HudCanvasFactory { get; set; }

	[Inject] private IMain Main { get; set; }
	[Inject] private ILazy<ProgressScreen> ProgressScreenLazy { get; set; }
	[Inject] private GameLoader GameLoader { get; set; }
	[Inject("RtsPoolNodeContainer")] private PoolContainer<Node> PoolNodeContainer { get; set; }
	[Inject] private InputActionsContainer PlayerActionsContainer { get; set; }
	[Inject] private UiActionsContainer UiActionsContainer { get; set; }
	[Inject] private JoypadPlayersMapping JoypadPlayersMapping { get; set; }

	private SplitViewport _splitViewport;

	// public HudCanvas HudCanvas { get; private set; } = null!;
	public RtsWorld RtsWorld { get; private set; } = null!;

	public Node GetWorld() => RtsWorld;

	public void PostInject() {
		PlayerActionsContainer.Disable(); // The real actions are cloned per player in player.Connect()
		ConfigureDebugOverlays();
		_splitViewport = new SplitViewport(this, () => GetViewportRect().Size);
	}

	public override async void _UnhandledInput(InputEvent e) {
		if (e.IsKeyReleased(Key.F5)) {
			Save("savegame");
		} else if (e.IsKeyReleased(Key.F6)) {
			LoadInGame("savegame");
		}
	}

	public async Task StartNewGame() {
		SceneTree.Root.AddChild(this);
		UiActionsContainer.SetJoypad(UiActionsContainer.CurrentJoyPad);	// Player who starts the game is the player who control the UI forever
		
		await GameLoader.LoadRtsGameResources();
		
		CurrentMetadata = new RtsSaveGameMetadata();
		GameObjectRepository.Initialize();
		InitializeWorld();
		RtsWorld.StartNewGame();
	}

	public async Task LoadFromMenu(string saveName) {
		SceneTree.Root.AddChild(this);
		UiActionsContainer.SetJoypad(UiActionsContainer.CurrentJoyPad);	// Player who starts the game is the player who control the UI forever
		var (success, saveGame) = await LoadSaveGame(saveName);
		if (!success) return;
		await GameLoader.LoadRtsGameResources();
		ContinueLoad(saveGame);
	}

	public async Task LoadInGame(string saveName) {
		UiActionsContainer.SetJoypad(UiActionsContainer.CurrentJoyPad); // Player who starts the game is the player who control the UI forever
		var (success, saveGame) = await LoadSaveGame(saveName);
		if (!success) return;
		await FreeSceneKeepingPoolData();
		ContinueLoad(saveGame);
	}

	private void ContinueLoad(SaveGame<RtsSaveGameMetadata> saveGame) {
		CurrentMetadata = saveGame.Metadata;
		GameObjectRepository.Initialize();
		GameObjectRepository.LoadSaveObjects(saveGame.GameObjects);
		InitializeWorld();
		var consumer = new RtsSaveGameConsumer(saveGame);
		RtsWorld.LoadGame(consumer);
		HideLoading();
	}

	public async Task Save(string saveName) {
		Main.Send(MainEvent.StartSavingGame);
		var l = await RtsGameObjectLoader.ListMetadatas();
		try {
			var saveObjects = GameObjectRepository.GetSaveObjects();
			Action<float>? saveProgress = saveObjects.Count < 1000 ? null : (progress) => ProgressScreenLazy.Get().ShowSaving(progress);
			await RtsGameObjectLoader.Save(saveName, CurrentMetadata, saveObjects, saveProgress);
		} catch (Exception e) {
			// Show saving error
			Console.WriteLine(e);
		}
		Main.Send(MainEvent.Back);
	}

	public void ShowLoading() {}
	public void HideLoading() {}
	
	public RtsSaveGameMetadata CurrentMetadata { get; private set; }

	public async Task<(bool, SaveGame<RtsSaveGameMetadata>)> LoadSaveGame(string save) {
		ShowLoading();
		try {
			var saveGame = await RtsGameObjectLoader.Load(save);
			return (true, saveGame);
		} catch (Exception e) {
			HideLoading();
			Console.WriteLine(e);
			return (false, null);
		}
	}

	public void InitializeWorld() {
		JoypadPlayersMapping.RemoveAllPlayers();

		RtsWorld = RtsWorldFactory.Create();
		RtsWorld.SetMainCamera(_splitViewport.Camera1);

		_splitViewport.SetCommonWorld(RtsWorld);
		_splitViewport.Refresh();
		_splitViewport.OnChange += (split) => {
			if (split) {
				// HudCanvas.SplitScreen();
				// The HUD for player two should be always visible if the player 2 is alive 
			}
		};
		
		// HudCanvas = HudCanvasFactory.Create();
		// AddChild(HudCanvas);
	}
	
	public async Task End(bool unload) {
		if (unload) {
			UnloadResources();
		} else {
			await FreeSceneKeepingPoolData();
		}
		Free();
		DebugOverlayManager.Overlay("Pool").Free();
		GC.GetTotalMemory(true);
		// PrintOrphanNodes();
	}

	public void UnloadResources() {
		// To ensure the pool nodes are freed along with the scene:
		
		// 1. All busy elements are still attached to the tree and will be destroyed with the scene, so we don't need to
		// do anything with them.
		// GetAvailable() will loop over the non busy and valid element (which are outside of the scene tree) in the pool, so, loop them and free them:
		PoolNodeContainer.GetAvailable().ForEach(node => node.Free());
		// 2. Remove the data from the pool to avoid having references to busy elements which are going to die with the scene
		PoolNodeContainer.Clear();
		GameLoader.UnloadRtsGameResources();
	}

	public async Task FreeSceneKeepingPoolData() {
		// This line keeps the godot nodes in the pool removing them from scene, because the scene is going to be freed
		// The busy nodes are the nodes who still belongs to the tree, so loop them and remove them from the tree is a way
		// to keep them in the pool ready for the next game
		PoolNodeContainer.GetAllBusy().ForEach(node => node.RemoveFromParent());
		// Wait one frame before free the scene. Not waiting one frame will cause canvas modulate will not shown (when LoadInGame)
		// and some collisions will not work because the unparented nodes are still in the tree and the physics engine will try to use them
		await this.AwaitPhysicsFrame();
		RtsWorld.Free();
	}

	private void ConfigureDebugOverlays() {
		DebugOverlayManager.Overlay("Pool")
			.Text("Busy", () => PoolNodeContainer.BusyCount() + "").EndMonitor()
			.Text("Available", () => PoolNodeContainer.AvailableCount() + "").EndMonitor()
			.Text("Invalid", () => PoolNodeContainer.InvalidCount() + "").EndMonitor();

	}
}
