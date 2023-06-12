using System;
using System.Threading.Tasks;
using Betauer.Application.Lifecycle.Pool;
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
using Veronenger.Game.HUD;
using Veronenger.Game.Platform;
using Veronenger.Game.UI;

namespace Veronenger.Game.Worlds;

public partial class GameTerrainView : Control, IInjectable, IGameView {

	[Inject] private GameObjectRepository GameObjectRepository { get; set; }
	[Inject] private JsonGameLoader<MySaveGameMetadata> GameObjectLoader { get; set; }
	[Inject] private HudCanvas HudCanvas { get; set; }
	[Inject] private ITransient<Terrain> WorldPlatformFactory { get; set; }

	[Inject] private MainStateMachine MainStateMachine { get; set; }
	[Inject] private ILazy<ProgressScreen> ProgressScreenLazy { get; set; }
	[Inject] private GameLoader GameLoader { get; set; }
	[Inject] private PoolContainer<Node> PoolNodeContainer { get; set; }
	[Inject] private InputActionsContainer PlayerActionsContainer { get; set; }
	[Inject] private UiActionsContainer UiActionsContainer { get; set; }
	[Inject] private JoypadPlayersMapping JoypadPlayersMapping { get; set; }

	[NodePath("SplitScreen")] private SplitScreen _splitScreen;
	// private readonly DragCameraController _cameraController = new();

	public const int MaxPlayer = 2;

	public Terrain Terrain { get; private set; } = null!;
	public bool ManualCamera { get; private set; } = false;
	private int ActivePlayers => 1; // WorldPlatform != null ? WorldPlatform.Players.Count : 0;
	private bool _allowAddingP2 = true;
	private void AllowAddingP2() => _allowAddingP2 = true;
	private void NoAddingP2() => _allowAddingP2 = false;

	public Node GetWorld() => Terrain;

	public void PostInject() {
		PlayerActionsContainer.Disable(); // The real actions are cloned per player in player.Connect()

		_splitScreen.OnDoubleChanged += (visible) => {
			if (visible) {
				HudCanvas.EnablePlayer2();
			} else {
				HudCanvas.DisablePlayer2();
			}
		};
	}

	public override void _Input(InputEvent e) {
		// if (e.IsLeftDoubleClick()) {
			// _camera2D.Position = Vector2.Zero;
		// } else if (e.IsKeyPressed(Key.Q)) {
			// _camera2D.Zoom -= new Vector2(0.05f, 0.05f);
		// } else if (e.IsKeyPressed(Key.W)) {
			// _camera2D.Zoom = new Vector2(1, 1);
		// } else if (e.IsKeyPressed(Key.E)) {
			// _camera2D.Zoom += new Vector2(0.05f, 0.05f);
		// }
	}


	public override async void _UnhandledInput(InputEvent e) {
		if (_allowAddingP2 && e is InputEventJoypadButton button && !JoypadPlayersMapping.IsJoypadUsed(button.Device)) {
			// CreatePlayer2(button.Device);
			GetViewport().SetInputAsHandled();
			if (JoypadPlayersMapping.Players == MaxPlayer) NoAddingP2();				
		} else if (e.IsKeyReleased(Key.U)) {
			if (ActivePlayers < 2) {
				// CreatePlayer2(1);
				GetViewport().SetInputAsHandled();
			}
		} else if (e.IsKeyReleased(Key.I)) {
			_splitScreen.EnableOnlyOneViewport(false);
			GetViewport().SetInputAsHandled();
		} else if (e.IsKeyReleased(Key.O)) {
			_splitScreen.EnableDoubleViewport(false);
			GetViewport().SetInputAsHandled();
		} else if (e.IsKeyReleased(Key.F5)) {
			Save("savegame");
		} else if (e.IsKeyReleased(Key.F6)) {
			LoadInGame("savegame");
		}
	}

	public async Task StartNewGame() {
		UiActionsContainer.SetJoypad(UiActionsContainer.CurrentJoyPad);	// Player who starts the game is the player who control the UI forever
		
		await GameLoader.LoadGameResources();
		
		CurrentMetadata = new MySaveGameMetadata();
		GameObjectRepository.Initialize();
		InitializeWorld();
		// CreatePlayer1(UiActionsContainer.CurrentJoyPad);
		AllowAddingP2();				
		Terrain.StartNewGame();
		// _cameraController.WithMouseButton(MouseButton.Middle).Attach(_camera2D);
	}

	public async Task LoadFromMenu(string saveName) {
		UiActionsContainer.SetJoypad(UiActionsContainer.CurrentJoyPad);	// Player who starts the game is the player who control the UI forever
		var (success, saveGame) = await LoadSaveGame(saveName);
		if (!success) return;
		await GameLoader.LoadGameResources();
		ContinueLoad(saveGame);
	}

	public async Task LoadInGame(string saveName) {
		UiActionsContainer.SetJoypad(UiActionsContainer.CurrentJoyPad); // Player who starts the game is the player who control the UI forever
		var (success, saveGame) = await LoadSaveGame(saveName);
		if (!success) return;
		await FreeSceneKeepingPoolData();
		ContinueLoad(saveGame);
	}

	private void ContinueLoad(SaveGame<MySaveGameMetadata> saveGame) {
		CurrentMetadata = saveGame.Metadata;
		GameObjectRepository.Initialize();
		GameObjectRepository.LoadSaveObjects(saveGame.GameObjects);
		InitializeWorld();
		var consumer = new MySaveGameConsumer(saveGame);
		// LoadPlayer1(UiActionsContainer.CurrentJoyPad, consumer);
		if (consumer.Player1 == null) AllowAddingP2();
		else NoAddingP2();
		Terrain.LoadGame(consumer);
		HideLoading();
	}

	public async Task Save(string saveName) {
		MainStateMachine.Send(MainEvent.StartSavingGame);
		var l = await GameObjectLoader.ListMetadatas();
		try {
			var saveObjects = GameObjectRepository.GetSaveObjects();
			Action<float>? saveProgress = saveObjects.Count < 1000 ? null : (progress) => ProgressScreenLazy.Get().ShowSaving(progress);
			await GameObjectLoader.Save(saveName, CurrentMetadata, saveObjects, saveProgress);
		} catch (Exception e) {
			// Show saving error
			Console.WriteLine(e);
		}
		MainStateMachine.Send(MainEvent.Back);
	}

	public void ShowLoading() {}
	public void HideLoading() {}
	
	public MySaveGameMetadata CurrentMetadata { get; private set; }

	public async Task<(bool, SaveGame<MySaveGameMetadata>)> LoadSaveGame(string save) {
		ShowLoading();
		try {
			var saveGame = await GameObjectLoader.Load(save);
			return (true, saveGame);
		} catch (Exception e) {
			HideLoading();
			Console.WriteLine(e);
			return (false, null);
		}
	}

	public void InitializeWorld() {
		JoypadPlayersMapping.RemoveAllPlayers();
		Terrain = WorldPlatformFactory.Create();
		_splitScreen.AddNode(Terrain);
		HudCanvas.Enable();
	}
/*
	public PlayerNode CreatePlayer1(int joypad) {
		var playerMapping = JoypadPlayersMapping.AddPlayer().SetJoypadId(joypad);
		var player = Terrain.AddNewPlayer(playerMapping);
		player.SetCamera(_splitScreen.Camera1);
		return player;
	}

	public PlayerNode LoadPlayer1(int joypad, MySaveGameConsumer consumer) {
		var playerMapping = JoypadPlayersMapping.AddPlayer().SetJoypadId(joypad);
		var player = Terrain.LoadPlayer(playerMapping, consumer.Player0, consumer.Inventory0);
		player.SetCamera(_splitScreen.Camera1);
		return player;
	}

	public PlayerNode CreatePlayer2(int joypad) {
		if (JoypadPlayersMapping.Players >= MaxPlayer) throw new Exception("No more players allowed");
		var playerMapping = JoypadPlayersMapping.AddPlayer().SetJoypadId(joypad);
		var player = Terrain.AddNewPlayer(playerMapping);
		player.SetCamera(_splitScreen.Camera2);
		return player;
	}
*/	
	public override void _Process(double delta) {
		if (!ManualCamera) {
			ManageSplitScreen();
		}
	}

	public void ManageSplitScreen() {
		if (_splitScreen.BusyPlayerTransition) return;
		//
		// var visiblePlayers = _splitScreen.VisiblePlayers;
		// if (ActivePlayers == 1) {
		// 	// Ensure only one viewport is shown
		// 	if (visiblePlayers != 1) _splitScreen.EnableOnlyOneViewport(true);
		// 	
		// } else if (ActivePlayers == 2) {
		//
		// 	var p1Stage = WorldPlatform.Players[0].StageCameraController?.CurrentStage;
		// 	var p2Stage = WorldPlatform.Players[1].StageCameraController?.CurrentStage;
		// 	if (p1Stage == null || p2Stage == null) return;
		// 	var sameStage = p1Stage == p2Stage;
		// 	if (!sameStage) {
		// 		if (visiblePlayers == 1) _splitScreen.EnableDoubleViewport(false);
		// 	} else {
		// 		var p1Pos = WorldPlatform.Players[0].Marker2D.GlobalPosition;
		// 		var p2Pos = WorldPlatform.Players[1].Marker2D.GlobalPosition;
		// 		var distanceTo = p1Pos.DistanceTo(p2Pos);
		//
		// 		if (visiblePlayers == 2) {
		// 			if (distanceTo < (Size.X * 0.5 * 0.2f)) {
		// 				_splitScreen.EnableOnlyOneViewport(false);
		// 			}
		// 		} else if (visiblePlayers == 1) {
		// 			if (distanceTo > (Size.X * 0.5 * 0.3f)) {
		// 				_splitScreen.EnableDoubleViewport(false);
		// 			}
		// 		}
		// 	}
		// }
	}

	public async Task End(bool unload) {
		HudCanvas.Disable();
		if (unload) {
			UnloadResources();
		} else {
			await FreeSceneKeepingPoolData();
		}
	}

	public void UnloadResources() {
		// To ensure the pool nodes are freed along with the scene:
		
		// 1. All busy elements are still attached to the tree and will be destroyed with the scene, so we don't need to
		// do anything with them.
		// Drain() will loop over the non busy and valid element (which are outside of the scene tree) in the pool, so, loop them and free them:
		PoolNodeContainer.GetAvailable().ForEach(node => node.Free());
		// 2. Remove the data from the pool to avoid having references to busy elements which are going to die with the scene
		PoolNodeContainer.Clear();
		GameLoader.UnloadGameResources();
	}

	public async Task FreeSceneKeepingPoolData() {
		// This line keeps the godot nodes in the pool removing them from scene, because the scene is going to be freed
		// The busy nodes are the nodes who still belongs to the tree, so loop them and remove them from the tree is a way
		// to keep them in the pool ready for the next game
		PoolNodeContainer.GetAllBusy().ForEach(node => node.RemoveFromParent());
		// Wait one frame before free the scene. Not waiting one frame will cause canvas modulate will not shown (when LoadInGame)
		// and some collisions will not work because the unparented nodes are still in the tree and the physics engine will try to use them
		await this.AwaitPhysicsFrame();
		Terrain.Free();
	}
}

