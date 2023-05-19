using System;
using System.Threading.Tasks;
using Betauer.Application.Lifecycle.Pool;
using Betauer.Application.Persistent;
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
using Veronenger.Config;
using Veronenger.UI;
using Veronenger.Worlds;

namespace Veronenger.Managers;

public partial class Game : Control, IInjectable {
	private World2D _noWorld = new();

	[Inject] private GameObjectRepository GameObjectRepository { get; set; }
	[Inject] private IGameObjectLoader<MySaveGame> GameObjectLoader { get; set; }
	[Inject] private HUD HudScene { get; set; }
	[Inject] private IFactory<WorldScene> World3 { get; set; }

	[Inject] private GameLoaderContainer GameLoaderContainer { get; set; }
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
	
	public WorldScene WorldScene { get; private set; }

	public void PostInject() {
		PlayerActionsContainer.Disable(); // The real actions are cloned per player in player.Connect()		
		NoAddingP2();				
	}

	public void StartNewProceduralWorld() {
		// _currentGameScene = MainResourceLoader.CreateWorld2Empty();
		var tileMap = WorldScene.GetNode<TileMap>("RealTileMap");
		new WorldGenerator().Generate(tileMap);
		// AddPlayerToScene();
		_subViewport1.AddChildDeferred(WorldScene);
	}

	private bool _allowAddingP2 = true;
	private void AllowAddingP2() {
		_allowAddingP2 = true;
	}

	private void NoAddingP2() {
		_allowAddingP2 = false;
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
			CreatePlayer2(1);
			GetViewport().SetInputAsHandled();
		} else if (e.IsKeyReleased(Key.I)) {
			DisablePlayer2Viewport(false);
			GetViewport().SetInputAsHandled();
		} else if (e.IsKeyReleased(Key.O)) {
			EnablePlayer2Viewport(false);
			GetViewport().SetInputAsHandled();
		} else if (e.IsKeyReleased(Key.F5)) {
			var l = await GameObjectLoader.ListSaveGames();
			Save("savegame");
		} else if (e.IsKeyReleased(Key.F6)) {
			LoadInGame("savegame");
		}
	}

	public async Task NewGame() {
		UiActionsContainer.SetJoypad(UiActionsContainer.CurrentJoyPad);	// Player who starts the game is the player who control the UI forever
		
		await GameLoaderContainer.LoadGameResources();
		
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
		(var success, CurrentSaveGame) = await LoadSaveGame(saveName);
		if (!success) return;
		
		await GameLoaderContainer.LoadGameResources();

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

	public async void LoadInGame(string saveName) {
		UiActionsContainer.SetJoypad(UiActionsContainer.CurrentJoyPad);	// Player who starts the game is the player who control the UI forever
		(var success, CurrentSaveGame) = await LoadSaveGame(saveName);
		if (!success) return;

		await FreeSceneAndKeepPoolData();

		GameObjectRepository.Initialize();
		GameObjectRepository.LoadSaveObjects(CurrentSaveGame.GameObjects);
		InitializeWorld();
		var consumer = new MySaveGameConsumer(CurrentSaveGame);
		LoadPlayer1(UiActionsContainer.CurrentJoyPad, consumer);
		NoAddingP2();
		if (consumer.Player1 == null) AllowAddingP2();
		else NoAddingP2();
		WorldScene.LoadGame(consumer);
		HideLoading();
	}

	public async Task Save(string saveName) {
		ShowSaving();
		try {
			var saveObjects = GameObjectRepository.GetSaveObjects();
			await GameObjectLoader.Save(CurrentSaveGame, saveObjects, saveName);
		} catch (Exception e) {
			// Show saving error
			Console.WriteLine(e);
		}
		HideSaving();
	}

	public void ShowLoading() {}
	public void HideLoading() {}
	public void ShowSaving() {}
	public void HideSaving() {}

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
		WorldScene = World3.Get();
		_subViewport1.AddChild(WorldScene);
		HudScene.Enable();
	}

	public PlayerNode CreatePlayer1(int joypad) {
		var playerMapping = JoypadPlayersMapping.AddPlayer().SetJoypadId(joypad);
		var player = WorldScene.AddNewPlayer(playerMapping);
		player.SetCamera(_camera1);
		DisablePlayer2Viewport(true);
		return player;
	}

	public PlayerNode LoadPlayer1(int joypad, MySaveGameConsumer consumer) {
		var playerMapping = JoypadPlayersMapping.AddPlayer().SetJoypadId(joypad);
		var player = WorldScene.LoadPlayer(playerMapping, consumer.Player0, consumer.Inventory0);
		player.SetCamera(_camera2);
		DisablePlayer2Viewport(true);
		return player;
	}

	public PlayerNode CreatePlayer2(int joypad) {
		if (JoypadPlayersMapping.Players >= MaxPlayer) throw new Exception("No more players allowed");
		var playerMapping = JoypadPlayersMapping.AddPlayer().SetJoypadId(joypad);
		var player = WorldScene.AddNewPlayer(playerMapping);
		player.SetCamera(_camera2);
		return player;
	}
	
	private float _effectDuration = 0.2f;
	private bool _busyPlayerTransition = false;
	private int _visiblePlayers = 0;

	// TODO: this has been done because Vector2.DistanceTo didn't work (returned Infinity)
	public float DistanceTo(Vector2 from, Vector2 to) {
		double fromX = (from.X - to.X) * (from.X - to.X) + (from.Y - to.Y) * (from.Y - to.Y);
		return Mathf.Sqrt((float)fromX);
	}

	public override void _PhysicsProcess(double delta) {
		if (!_busyPlayerTransition && WorldScene is { Players.Count: 2 }) {
			var p1Stage = WorldScene.Players[0].StageCameraController?.CurrentStage;
			var p2Stage = WorldScene.Players[1].StageCameraController?.CurrentStage;
			if (p1Stage == null || p2Stage == null) return;
			var sameStage = p1Stage == p2Stage;
			if (!sameStage) {
				if (_visiblePlayers == 1) EnablePlayer2Viewport(false);
			} else {
				var p1Pos = WorldScene.Players[0].Marker2D.GlobalPosition;
				var p2Pos = WorldScene.Players[1].Marker2D.GlobalPosition;
				var distanceTo = DistanceTo(p1Pos, p2Pos);

				if (_visiblePlayers == 2) {
					if (distanceTo < (Size.X * 0.5 * 0.2f)) {
						DisablePlayer2Viewport(false);
					}
				} else if (_visiblePlayers == 1) {
					if (distanceTo > (Size.X * 0.5 * 0.3f)) {
						EnablePlayer2Viewport(false);
					}
				}
			}
		}
	}

	private void EnablePlayer2Viewport(bool immediate) {
		_visiblePlayers = 2;
		var half = new Vector2I((int)Size.X / 2, (int)Size.Y);
		if (immediate || true) {
			_subViewport1.Size = half;
			_subViewport2.Size = half;
			_subViewport2.GetParent<SubViewportContainer>().Visible = true;
			_subViewport2.World2D = _subViewport1.World2D;
			HudScene.EnablePlayer2();
			_busyPlayerTransition = false;
		} else {
			_busyPlayerTransition = true;
			_subViewport2.World2D = _subViewport1.World2D;
			_subViewport2.Size = new Vector2I(0, (int)Size.Y);
			_subViewport2.GetParent<SubViewportContainer>().Visible = true;
			CreateTween().SetProcessMode(Tween.TweenProcessMode.Physics).TweenProperty(_subViewport1, "size", half, _effectDuration).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.InOut);
			CreateTween().SetProcessMode(Tween.TweenProcessMode.Physics).TweenProperty(_subViewport2, "size", half, _effectDuration).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.InOut);
			CreateTween().TweenCallback(Callable.From(() => {
				_busyPlayerTransition = false;
				HudScene.EnablePlayer2();
			})).SetDelay(_effectDuration);
		}
	}

	public void DisablePlayer2Viewport(bool immediate) {
		_visiblePlayers = 1;
		var full = new Vector2I((int)Size.X, (int)Size.Y);
		var zero = new Vector2I(0, (int)Size.Y);
		if (immediate || true) {
			_subViewport2.World2D = _noWorld;
			_subViewport2.GetParent<SubViewportContainer>().Visible = false;
			_subViewport1.Size = full;
			HudScene.DisablePlayer2();
			_busyPlayerTransition = false;
		} else {
			_busyPlayerTransition = true;
			CreateTween().SetProcessMode(Tween.TweenProcessMode.Physics).TweenProperty(_subViewport1, "size", full, _effectDuration).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.InOut);
			CreateTween().SetProcessMode(Tween.TweenProcessMode.Physics).TweenProperty(_subViewport2, "size", zero, _effectDuration).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.InOut);
			CreateTween().TweenCallback(Callable.From(() => {
				_subViewport2.World2D = _noWorld;
				_subViewport2.GetParent<SubViewportContainer>().Visible = false;
				_subViewport1.Size = full;
				HudScene.DisablePlayer2();
				_busyPlayerTransition = false;
			})).SetDelay(_effectDuration);
		}
	}

	public async Task End() {
		NoAddingP2();				
		HudScene.Disable();
		// FreeSceneAndKeepPoolData();
		await FreeSceneAndUnloadResources();
	}

	private async Task FreeSceneAndUnloadResources() {
		// To ensure the pool nodes are freed along with the scene:
		
		// 1. All busy elements are still attached to the tree and will be destroyed with the scene, so we don't need to
		// do anything with them.
		// But the non busy and valid element are outside of the scene tree in the pool, so, loop them and free them:
		PoolNodeContainer.Drain().ForEach(node => node.Free());
		// 2. Remove the data from the pool to avoid having references to busy elements which are going to die with the scene
		PoolNodeContainer.Clear();
		GameLoaderContainer.UnloadGameResources();
		WorldScene.QueueFree();
		WorldScene = null;
		GC.GetTotalMemory(true);
		PrintOrphanNodes();
		await GetTree().AwaitProcessFrame();
	}

	private async Task FreeSceneAndKeepPoolData() {
		// This line keeps the godot nodes in the pool removing them from scene, because the scene is going to be freed
		// The busy nodes are the node who still belongs to the tree, so loop them and remove them from the tree is a way
		// to keep them in the pool ready for the next game
		PoolNodeContainer.GetAllBusy().ForEach(node => node.Free());
		WorldScene.Free();
		await GetTree().AwaitProcessFrame();
		WorldScene = null;
		GC.GetTotalMemory(true);
		PrintOrphanNodes();
		await GetTree().AwaitProcessFrame();
	}
}
