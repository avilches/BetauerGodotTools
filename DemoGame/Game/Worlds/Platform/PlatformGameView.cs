using System;
using System.Threading.Tasks;
using Betauer.Application.Lifecycle.Pool;
using Betauer.Application.Monitor;
using Betauer.Application.Persistent;
using Betauer.Application.Persistent.Json;
using Betauer.Core;
using Betauer.Core.Nodes;
using Betauer.Core.Signal;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Betauer.Input;
using Betauer.Input.Joypad;
using Betauer.NodePath;
using Godot;
using Veronenger.Game.Character.Player;
using Veronenger.Game.HUD;
using Veronenger.Game.Platform;
using Veronenger.Game.UI;

namespace Veronenger.Game.Worlds.Platform;

public partial class PlatformGameView : Control, IInjectable, IGameView {

	[Inject] private DebugOverlayManager DebugOverlayManager { get; set; }
	[Inject] private SceneTree SceneTree { get; set; }
	[Inject] private GameObjectRepository GameObjectRepository { get; set; }
	[Inject] private JsonGameLoader<PlatformSaveGameMetadata> PlatformGameObjectLoader { get; set; }
	[Inject] private ITransient<PlatformWorld> PlatformWorldFactory { get; set; }
	[Inject] private ITransient<HudCanvas> HudCanvasFactory { get; set; }

	[Inject] private MainStateMachine MainStateMachine { get; set; }
	[Inject] private ILazy<ProgressScreen> ProgressScreenLazy { get; set; }
	[Inject] private GameLoader GameLoader { get; set; }
	[Inject("PlatformPoolNodeContainer")] private PoolContainer<Node> PoolNodeContainer { get; set; }
	[Inject] private InputActionsContainer PlayerActionsContainer { get; set; }
	[Inject] private UiActionsContainer UiActionsContainer { get; set; }
	[Inject] private JoypadPlayersMapping JoypadPlayersMapping { get; set; }

	[NodePath("SplitScreen")] private SplitScreen _splitScreen;

	public const int MaxPlayer = 2;

	public HudCanvas HudCanvas { get; private set; } = null!;
	public PlatformWorld PlatformWorld { get; private set; } = null!;
	private int ActivePlayers => PlatformWorld != null ? PlatformWorld.Players.Count : 0;
	private bool _allowAddingP2 = true;
	private void AllowAddingP2() => _allowAddingP2 = true;
	private void NoAddingP2() => _allowAddingP2 = false;

	public Node GetWorld() => PlatformWorld;

	public void PostInject() {
		PlayerActionsContainer.Disable(); // The real actions are cloned per player in player.Connect()
		ConfigureDebugOverlays();
	}

	public override async void _UnhandledInput(InputEvent e) {
		if (_allowAddingP2 && e is InputEventJoypadButton button && !JoypadPlayersMapping.IsJoypadUsed(button.Device)) {
			CreatePlayer2(button.Device);
			GetViewport().SetInputAsHandled();
			if (JoypadPlayersMapping.Players == MaxPlayer) NoAddingP2();				
		} else if (e.IsKeyReleased(Key.U)) {
			if (ActivePlayers < 2) {
				CreatePlayer2(1);
				GetViewport().SetInputAsHandled();
			}
		} else if (e.IsKeyReleased(Key.F5)) {
			Save("savegame");
		} else if (e.IsKeyReleased(Key.F6)) {
			LoadInGame("savegame");
		}
	}

	public async Task StartNewGame() {
		SceneTree.Root.AddChild(this);
		UiActionsContainer.SetJoypad(UiActionsContainer.CurrentJoyPad);	// Player who starts the game is the player who control the UI forever
		
		await GameLoader.LoadPlatformGameResources();
		
		CurrentMetadata = new PlatformSaveGameMetadata();
		GameObjectRepository.Initialize();
		InitializeWorld();
		CreatePlayer1(UiActionsContainer.CurrentJoyPad);
		AllowAddingP2();				
		PlatformWorld.StartNewGame();
		// _cameraController.WithMouseButton(MouseButton.Middle).Attach(_camera2D);
	}

	public async Task LoadFromMenu(string saveName) {
		SceneTree.Root.AddChild(this);
		UiActionsContainer.SetJoypad(UiActionsContainer.CurrentJoyPad);	// Player who starts the game is the player who control the UI forever
		var (success, saveGame) = await LoadSaveGame(saveName);
		if (!success) return;
		await GameLoader.LoadPlatformGameResources();
		ContinueLoad(saveGame);
	}

	public async Task LoadInGame(string saveName) {
		UiActionsContainer.SetJoypad(UiActionsContainer.CurrentJoyPad); // Player who starts the game is the player who control the UI forever
		var (success, saveGame) = await LoadSaveGame(saveName);
		if (!success) return;
		await FreeSceneKeepingPoolData();
		ContinueLoad(saveGame);
	}

	private void ContinueLoad(SaveGame<PlatformSaveGameMetadata> saveGame) {
		CurrentMetadata = saveGame.Metadata;
		GameObjectRepository.Initialize();
		GameObjectRepository.LoadSaveObjects(saveGame.GameObjects);
		InitializeWorld();
		var consumer = new PlatformSaveGameConsumer(saveGame);
		LoadPlayer1(UiActionsContainer.CurrentJoyPad, consumer);
		if (consumer.Player1 == null) AllowAddingP2();
		else NoAddingP2();
		PlatformWorld.LoadGame(consumer);
		HideLoading();
	}

	public async Task Save(string saveName) {
		MainStateMachine.Send(MainEvent.StartSavingGame);
		var l = await PlatformGameObjectLoader.ListMetadatas();
		try {
			var saveObjects = GameObjectRepository.GetSaveObjects();
			Action<float>? saveProgress = saveObjects.Count < 1000 ? null : (progress) => ProgressScreenLazy.Get().ShowSaving(progress);
			await PlatformGameObjectLoader.Save(saveName, CurrentMetadata, saveObjects, saveProgress);
		} catch (Exception e) {
			// Show saving error
			Console.WriteLine(e);
		}
		MainStateMachine.Send(MainEvent.Back);
	}

	public void ShowLoading() {}
	public void HideLoading() {}
	
	public PlatformSaveGameMetadata CurrentMetadata { get; private set; }

	public async Task<(bool, SaveGame<PlatformSaveGameMetadata>)> LoadSaveGame(string save) {
		ShowLoading();
		try {
			var saveGame = await PlatformGameObjectLoader.Load(save);
			return (true, saveGame);
		} catch (Exception e) {
			HideLoading();
			Console.WriteLine(e);
			return (false, null);
		}
	}

	public void InitializeWorld() {
		JoypadPlayersMapping.RemoveAllPlayers();

		PlatformWorld = PlatformWorldFactory.Create();
		_splitScreen.SetWorld(PlatformWorld);
		_splitScreen.SinglePlayer(true);

		HudCanvas = HudCanvasFactory.Create();
		AddChild(HudCanvas);
		HudCanvas.SinglePlayer();
		
		_splitScreen.OnDoubleChanged += (visible) => {
			if (visible) {
				HudCanvas.EnableSplitScreen();
				// The HUD for player two should be always visible if the player 2 is alive 
			}
		};
	}

	public PlayerNode CreatePlayer1(int joypad) {
		var playerMapping = JoypadPlayersMapping.AddPlayer().SetJoypadId(joypad);
		var player = PlatformWorld.AddNewPlayer(playerMapping);
		player.SetCamera(_splitScreen.Camera1);
		return player;
	}

	public PlayerNode LoadPlayer1(int joypad, PlatformSaveGameConsumer consumer) {
		var playerMapping = JoypadPlayersMapping.AddPlayer().SetJoypadId(joypad);
		var player = PlatformWorld.LoadPlayer(playerMapping, consumer.Player0, consumer.Inventory0);
		player.SetCamera(_splitScreen.Camera1);
		return player;
	}

	public PlayerNode CreatePlayer2(int joypad) {
		if (JoypadPlayersMapping.Players >= MaxPlayer) throw new Exception("No more players allowed");
		var playerMapping = JoypadPlayersMapping.AddPlayer().SetJoypadId(joypad);
		var player = PlatformWorld.AddNewPlayer(playerMapping);
		player.SetCamera(_splitScreen.Camera2);
		return player;
	}
	
	public override void _Process(double delta) {
		ManageSplitScreen();
	}

	public void ManageSplitScreen() {
		if (_splitScreen.BusyPlayerTransition) return;

		var visiblePlayers = _splitScreen.VisiblePlayers;
		if (ActivePlayers == 1) {
			// Ensure only one viewport is shown
			if (visiblePlayers != 1) _splitScreen.SinglePlayer(true);
			
		} else if (ActivePlayers == 2) {

			var p1Stage = PlatformWorld.Players[0].StageCameraController?.CurrentStage;
			var p2Stage = PlatformWorld.Players[1].StageCameraController?.CurrentStage;
			if (p1Stage == null || p2Stage == null) return;
			var sameStage = p1Stage == p2Stage;
			if (!sameStage) {
				if (visiblePlayers == 1) _splitScreen.EnableSplitScreen(false);
			} else {
				var p1Pos = PlatformWorld.Players[0].Marker2D.GlobalPosition;
				var p2Pos = PlatformWorld.Players[1].Marker2D.GlobalPosition;
				var distanceTo = p1Pos.DistanceTo(p2Pos);

				if (visiblePlayers == 2) {
					if (distanceTo < (Size.X * 0.5 * 0.2f)) {
						_splitScreen.SinglePlayer(false);
					}
				} else if (visiblePlayers == 1) {
					if (distanceTo > (Size.X * 0.5 * 0.3f)) {
						_splitScreen.EnableSplitScreen(false);
					}
				}
			}
		}
	}

	public async Task End(bool unload) {
		if (unload) {
			UnloadResources();
		} else {
			await FreeSceneKeepingPoolData();
		}
		Free();
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
		GameLoader.UnloadPlatformGameResources();
	}

	public async Task FreeSceneKeepingPoolData() {
		// This line keeps the godot nodes in the pool removing them from scene, because the scene is going to be freed
		// The busy nodes are the nodes who still belongs to the tree, so loop them and remove them from the tree is a way
		// to keep them in the pool ready for the next game
		PoolNodeContainer.GetAllBusy().ForEach(node => node.RemoveFromParent());
		// Wait one frame before free the scene. Not waiting one frame will cause canvas modulate will not shown (when LoadInGame)
		// and some collisions will not work because the unparented nodes are still in the tree and the physics engine will try to use them
		await this.AwaitPhysicsFrame();
		PlatformWorld.Free();
	}
	
	private void ConfigureDebugOverlays() {
		DebugOverlayManager.Overlay("Pool")
			.Text("Busy", () => PoolNodeContainer.BusyCount() + "").EndMonitor()
			.Text("Available", () => PoolNodeContainer.AvailableCount() + "").EndMonitor()
			.Text("Invalid", () => PoolNodeContainer.InvalidCount() + "").EndMonitor();

	}
}
