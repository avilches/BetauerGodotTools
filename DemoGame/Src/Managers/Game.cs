using System;
using System.Collections.Generic;
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
	[Inject] private HUD HudScene { get; set; }
	[Inject] private IFactory<WorldScene> World3 { get; set; }

	[Inject] private GameLoaderContainer GameLoaderContainer { get; set; }
	[Inject] private PoolContainer<IPoolLifecycle> PoolContainer { get; set; }
	[Inject] private InputActionsContainer PlayerActionsContainer { get; set; }
	[Inject] private UiActionsContainer UiActionsContainer { get; set; }
	[Inject] private JoypadPlayersMapping JoypadPlayersMapping { get; set; }

	[NodePath("%SubViewport1")] private SubViewport _subViewport1;
	[NodePath("%SubViewport2")] private SubViewport _subViewport2;

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

	private void AllowAddingP2() {
		SetProcessUnhandledInput(true);
	}

	private void NoAddingP2() {
		SetProcessUnhandledInput(false);
	}

	public override void _UnhandledInput(InputEvent e) {
		if (e is InputEventJoypadButton button && !JoypadPlayersMapping.IsJoypadUsed(button.Device)) {
			CreatePlayer2(button.Device);
			GetViewport().SetInputAsHandled();
			if (JoypadPlayersMapping.Players == MaxPlayer) NoAddingP2();				
		} else if (e.IsKeyReleased(Key.U)) {
			CreatePlayer2(1);
			GetViewport().SetInputAsHandled();
		} else if (e.IsKeyReleased(Key.I)) {
			DisablePlayer2(false);
			GetViewport().SetInputAsHandled();
		} else if (e.IsKeyReleased(Key.O)) {
			EnablePlayer2(false);
			GetViewport().SetInputAsHandled();
		} else if (e.IsKeyReleased(Key.F5)) {
			GameObjectRepository.Save();
		} else if (e.IsKeyReleased(Key.F6)) {
			LoadInGame();
		}
	}

	public async Task NewGame() {
		await GameLoaderContainer.LoadGameResources();
		InitializeWorld();
		UiActionsContainer.SetJoypad(UiActionsContainer.CurrentJoyPad);	// Player who starts the game is the player who control the UI forever
		CreatePlayer1(UiActionsContainer.CurrentJoyPad);
		AllowAddingP2();				
		WorldScene.StartNewGame();
	}

	public async void LoadFromMenu() {
		var (success, objects) = await LoadData();
		if (!success) return;

		await GameLoaderContainer.LoadGameResources();
		InitializeWorld();
		LoadGame(objects!);
	}

	public async void LoadInGame() {
		var (success, objects) = await LoadData();
		if (!success) return;

		await FreeSceneAndKeepPoolData();
		InitializeWorld();
		LoadGame(objects!);
	}
	
	private void LoadGame(Dictionary<int, SaveObject> saveObjects) {
		UiActionsContainer.SetJoypad(UiActionsContainer.CurrentJoyPad); // Player who starts the game is the player who control the UI forever
		CreatePlayer1(UiActionsContainer.CurrentJoyPad);
		AllowAddingP2();				
		WorldScene.LoadGame(saveObjects);
		// TODO: hide loading screen
	}

	public async Task<(bool, Dictionary<int, SaveObject>?)> LoadData() {
		// TODO: Show loading screen
		try {
			var objects = await GameObjectRepository.Load();
			return (true, objects);
		} catch (Exception) {
			// TODO: Hide loading screen and show error
			return (false, null);
		}
	}

	public void InitializeWorld() {
		JoypadPlayersMapping.RemoveAllPlayers();
		GameObjectRepository.Clear();
		WorldScene = World3.Get();
		_subViewport1.AddChild(WorldScene);
		HudScene.Enable();
	}

	public PlayerNode CreatePlayer1(int joypad) {
		var playerMapping = JoypadPlayersMapping.AddPlayer().SetJoypadId(joypad);
		var player = WorldScene.AddPlayerToScene();
		player.SetViewport(_subViewport1);
		DisablePlayer2(true);
		return player;
	}

	public PlayerNode CreatePlayer2(int joypad) {
		if (JoypadPlayersMapping.Players >= MaxPlayer) throw new Exception("No more players allowed");
		var playerMapping = JoypadPlayersMapping.AddPlayer().SetJoypadId(joypad);
		var player = WorldScene.AddPlayerToScene();
		player.SetViewport(_subViewport2);
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
			var p1Stage = WorldScene.Players[0].StageController?.CurrentStage;
			var p2Stage = WorldScene.Players[1].StageController?.CurrentStage;
			if (p1Stage == null || p2Stage == null) return;
			var sameStage = p1Stage == p2Stage;
			if (!sameStage) {
				if (_visiblePlayers == 1) EnablePlayer2(false);
			} else {
				var p1Pos = WorldScene.Players[0].Marker2D.GlobalPosition;
				var p2Pos = WorldScene.Players[1].Marker2D.GlobalPosition;
				var distanceTo = DistanceTo(p1Pos, p2Pos);

				if (_visiblePlayers == 2) {
					if (distanceTo < (Size.X * 0.5 * 0.2f)) {
						DisablePlayer2(false);
					}
				} else if (_visiblePlayers == 1) {
					if (distanceTo > (Size.X * 0.5 * 0.3f)) {
						EnablePlayer2(false);
					}
				}
			}
		}
	}

	private void EnablePlayer2(bool immediate) {
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

	public void DisablePlayer2(bool immediate) {
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
		PoolContainer.ForEachElementInAllPools(e => ((Node)e).Free());
		// 2. Remove the data from the pool to avoid having references to busy elements which are going to die with the scene
		PoolContainer.Clear();
		GameLoaderContainer.UnloadGameResources();
		WorldScene.QueueFree();
		WorldScene = null;
		GC.GetTotalMemory(true);
		PrintOrphanNodes();
		await GetTree().AwaitProcessFrame();
	}

	private async Task FreeSceneAndKeepPoolData() {
		// This line keeps the godot nodes in the pool removing them from scene, because the scene is going to be freed
		PoolContainer.ForEachElementInAllPools(e => ((Node)e).RemoveFromParent());
		WorldScene.QueueFree();
		WorldScene = null;
		GC.GetTotalMemory(true);
		PrintOrphanNodes();
		await GetTree().AwaitProcessFrame();
	}
}
