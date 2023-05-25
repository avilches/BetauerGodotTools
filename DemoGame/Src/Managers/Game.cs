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
using Veronenger.Character.Player;
using Veronenger.UI;
using Veronenger.Worlds;

namespace Veronenger.Managers;

public partial class Game : Control, IInjectable {
	private static readonly World2D NoWorld = new(); // A cached World2D to re-use

	[Inject] private GameObjectRepository GameObjectRepository { get; set; }
	[Inject] private JsonGameLoader<MySaveGame> GameObjectLoader { get; set; }
	[Inject] private HUD HudScene { get; set; }
	[Inject] private ITransient<WorldScene> World3 { get; set; }

	[Inject] private MainStateMachine MainStateMachine { get; set; }
	[Inject] private ILazy<ProgressScreen> ProgressScreenFactory { get; set; }
	[Inject] private GameLoader GameLoader { get; set; }
	[Inject] private PoolContainer<Node> PoolNodeContainer { get; set; }
	[Inject] private InputActionsContainer PlayerActionsContainer { get; set; }
	[Inject] private UiActionsContainer UiActionsContainer { get; set; }
	[Inject] private JoypadPlayersMapping JoypadPlayersMapping { get; set; }

	[NodePath("%SubViewport1")] private SubViewport _subViewport1;
	[NodePath("%SubViewport2")] private SubViewport _subViewport2;
	[NodePath("%Camera2D1")] private Camera2D _camera1;
	[NodePath("%Camera2D2")] private Camera2D _camera2;
	// private readonly DragCameraController _cameraController = new();

	public const int MaxPlayer = 2;
	private float _splitCameraEffectDuration = 0.2f; // in seconds

	public WorldScene WorldScene { get; private set; } = null!;
	public bool BusyPlayerTransition { get; private set; } = false;
	public int VisiblePlayers { get; private set; } = 0;
	public bool ManualCamera { get; private set; } = false;
	private int ActivePlayers => WorldScene != null ? WorldScene.Players.Count : 0;
	private bool _allowAddingP2 = true;
	private void AllowAddingP2() => _allowAddingP2 = true;
	private void NoAddingP2() => _allowAddingP2 = false;

	public void PostInject() {
		PlayerActionsContainer.Disable(); // The real actions are cloned per player in player.Connect()		
	}

	public void StartNewProceduralWorld() {
		// _currentGameScene = MainResourceLoader.CreateWorld2Empty();
		var tileMap = WorldScene.GetNode<TileMap>("RealTileMap");
		new WorldGenerator().Generate(tileMap);
		// AddPlayerToScene();
		_subViewport1.AddChildDeferred(WorldScene);
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
			CreatePlayer2(button.Device);
			GetViewport().SetInputAsHandled();
			if (JoypadPlayersMapping.Players == MaxPlayer) NoAddingP2();				
		} else if (e.IsKeyReleased(Key.U)) {
			if (ActivePlayers < 2) {
				CreatePlayer2(1);
				GetViewport().SetInputAsHandled();
			}
		} else if (e.IsKeyReleased(Key.I)) {
			EnableOnlyOneViewport(false);
			GetViewport().SetInputAsHandled();
		} else if (e.IsKeyReleased(Key.O)) {
			EnableDoubleViewport(false);
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
		
		CurrentSaveGame = new MySaveGame();
		GameObjectRepository.Initialize();
		InitializeWorld();
		CreatePlayer1(UiActionsContainer.CurrentJoyPad);
		AllowAddingP2();				
		WorldScene.StartNewGame();
		// _cameraController.WithMouseButton(MouseButton.Middle).Attach(_camera2D);
	}

	public async void LoadFromMenu(string saveName) {
		UiActionsContainer.SetJoypad(UiActionsContainer.CurrentJoyPad);	// Player who starts the game is the player who control the UI forever
		var (success, saveGame) = await LoadSaveGame(saveName);
		if (!success) return;
		await GameLoader.LoadGameResources();
		ContinueLoad(saveGame);
	}

	public async void LoadInGame(string saveName) {
		UiActionsContainer.SetJoypad(UiActionsContainer.CurrentJoyPad); // Player who starts the game is the player who control the UI forever
		var (success, saveGame) = await LoadSaveGame(saveName);
		if (!success) return;
		await FreeSceneKeepingPoolData();
		ContinueLoad(saveGame);
	}

	private void ContinueLoad(MySaveGame saveGame) {
		CurrentSaveGame = saveGame;
		GameObjectRepository.Initialize();
		GameObjectRepository.LoadSaveObjects(CurrentSaveGame.GameObjects);
		InitializeWorld();
		var consumer = new MySaveGameConsumer(CurrentSaveGame);
		LoadPlayer1(UiActionsContainer.CurrentJoyPad, consumer);
		if (consumer.Player1 == null) AllowAddingP2();
		else NoAddingP2();
		WorldScene.LoadGame(consumer);
		HideLoading();
	}

	public async Task Save(string saveName) {
		MainStateMachine.Send(MainEvent.StartSavingGame);
		var l = await GameObjectLoader.ListSaveGames();
		try {
			var saveObjects = GameObjectRepository.GetSaveObjects();
			Action<float>? saveProgress = saveObjects.Count < 1000 ? null : (progress) => ProgressScreenFactory.Get().ShowSaving(progress);
			await GameObjectLoader.Save(CurrentSaveGame, saveObjects, saveName, saveProgress);
		} catch (Exception e) {
			// Show saving error
			Console.WriteLine(e);
		}
		MainStateMachine.Send(MainEvent.Back);
	}

	public void ShowLoading() {}
	public void HideLoading() {}
	
	public MySaveGame CurrentSaveGame { get; private set; }

	public async Task<(bool, MySaveGame)> LoadSaveGame(string save) {
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
		WorldScene = World3.Create();
		_subViewport1.AddChild(WorldScene);
		HudScene.Enable();
	}

	public PlayerNode CreatePlayer1(int joypad) {
		var playerMapping = JoypadPlayersMapping.AddPlayer().SetJoypadId(joypad);
		var player = WorldScene.AddNewPlayer(playerMapping);
		player.SetCamera(_camera1);
		return player;
	}

	public PlayerNode LoadPlayer1(int joypad, MySaveGameConsumer consumer) {
		var playerMapping = JoypadPlayersMapping.AddPlayer().SetJoypadId(joypad);
		var player = WorldScene.LoadPlayer(playerMapping, consumer.Player0, consumer.Inventory0);
		player.SetCamera(_camera1);
		return player;
	}

	public PlayerNode CreatePlayer2(int joypad) {
		if (JoypadPlayersMapping.Players >= MaxPlayer) throw new Exception("No more players allowed");
		var playerMapping = JoypadPlayersMapping.AddPlayer().SetJoypadId(joypad);
		var player = WorldScene.AddNewPlayer(playerMapping);
		player.SetCamera(_camera2);
		return player;
	}
	
	// TODO: this has been done because Vector2.DistanceTo didn't work (returned Infinity)
	public float DistanceTo(Vector2 from, Vector2 to) {
		double fromX = (from.X - to.X) * (from.X - to.X) + (from.Y - to.Y) * (from.Y - to.Y);
		return Mathf.Sqrt((float)fromX);
	}

	public override void _Process(double delta) {
		if (!ManualCamera) {
			ManageSplitScreen();
		}
	}

	public void ManageSplitScreen() {
		if (BusyPlayerTransition) return;
		
		if (ActivePlayers == 1) {
			// Ensure only one viewport is shown
			if (VisiblePlayers != 1) EnableOnlyOneViewport(true);
			
		} else if (ActivePlayers == 2) {

			var p1Stage = WorldScene.Players[0].StageCameraController?.CurrentStage;
			var p2Stage = WorldScene.Players[1].StageCameraController?.CurrentStage;
			if (p1Stage == null || p2Stage == null) return;
			var sameStage = p1Stage == p2Stage;
			if (!sameStage) {
				if (VisiblePlayers == 1) EnableDoubleViewport(false);
			} else {
				var p1Pos = WorldScene.Players[0].Marker2D.GlobalPosition;
				var p2Pos = WorldScene.Players[1].Marker2D.GlobalPosition;
				var distanceTo = DistanceTo(p1Pos, p2Pos);

				if (VisiblePlayers == 2) {
					if (distanceTo < (Size.X * 0.5 * 0.2f)) {
						EnableOnlyOneViewport(false);
					}
				} else if (VisiblePlayers == 1) {
					if (distanceTo > (Size.X * 0.5 * 0.3f)) {
						EnableDoubleViewport(false);
					}
				}
			}
		}
	}

	private void EnableDoubleViewport(bool immediate) {
		VisiblePlayers = 2;
		var half = new Vector2I((int)Size.X / 2, (int)Size.Y);
		if (immediate || true) {
			_subViewport1.Size = half;
			_subViewport2.Size = half;
			_subViewport2.GetParent<SubViewportContainer>().Visible = true;
			_subViewport2.World2D = _subViewport1.World2D;
			HudScene.EnablePlayer2();
			BusyPlayerTransition = false;
		} else {
			BusyPlayerTransition = true;
			_subViewport2.World2D = _subViewport1.World2D;
			_subViewport2.Size = new Vector2I(0, (int)Size.Y);
			_subViewport2.GetParent<SubViewportContainer>().Visible = true;
			CreateTween().SetProcessMode(Tween.TweenProcessMode.Physics).TweenProperty(_subViewport1, "size", half, _splitCameraEffectDuration).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.InOut);
			CreateTween().SetProcessMode(Tween.TweenProcessMode.Physics).TweenProperty(_subViewport2, "size", half, _splitCameraEffectDuration).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.InOut);
			CreateTween().TweenCallback(Callable.From(() => {
				BusyPlayerTransition = false;
				HudScene.EnablePlayer2();
			})).SetDelay(_splitCameraEffectDuration);
		}
	}

	public void EnableOnlyOneViewport(bool immediate) {
		VisiblePlayers = 1;
		var full = new Vector2I((int)Size.X, (int)Size.Y);
		var zero = new Vector2I(0, (int)Size.Y);
		if (immediate || true) {
			_subViewport2.World2D = NoWorld;
			_subViewport2.GetParent<SubViewportContainer>().Visible = false;
			_subViewport1.Size = full;
			HudScene.DisablePlayer2();
			BusyPlayerTransition = false;
		} else {
			BusyPlayerTransition = true;
			CreateTween().SetProcessMode(Tween.TweenProcessMode.Physics).TweenProperty(_subViewport1, "size", full, _splitCameraEffectDuration).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.InOut);
			CreateTween().SetProcessMode(Tween.TweenProcessMode.Physics).TweenProperty(_subViewport2, "size", zero, _splitCameraEffectDuration).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.InOut);
			CreateTween().TweenCallback(Callable.From(() => {
				_subViewport2.World2D = NoWorld;
				_subViewport2.GetParent<SubViewportContainer>().Visible = false;
				_subViewport1.Size = full;
				HudScene.DisablePlayer2();
				BusyPlayerTransition = false;
			})).SetDelay(_splitCameraEffectDuration);
		}
	}

	public async Task End(bool unload) {
		HudScene.Disable();
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
		WorldScene.Free();
	}
}
