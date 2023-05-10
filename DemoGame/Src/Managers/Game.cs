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
using Betauer.NodePath;
using Godot;
using Veronenger.Config;
using Veronenger.UI;
using Veronenger.Worlds;

namespace Veronenger.Managers;

public partial class Game : Control, IInjectable {
	private World2D _noWorld = new();

	[Inject] private GameObjectRepository GameObjectRepository { get; set; }
	[Inject] private HUD HudScene { get; set; }
	[Inject] private ConfigManager ConfigManager { get; set; }
	[Inject] private StageManager StageManager { get; set; }
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

	public async Task Start() {
		JoypadPlayersMapping.RemoveAllPlayers();
		await StartWorld3();
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
			DisablePlayer2();
			GetViewport().SetInputAsHandled();
		} else if (e.IsKeyReleased(Key.O)) {
			EnablePlayer2();
			GetViewport().SetInputAsHandled();
		}
	}

	public async Task StartWorld3() {
		GameObjectRepository.Clear();
		await GameLoaderContainer.LoadGameResources();
		
		GD.PushWarning("World3 creation start");
		WorldScene = World3.Get();
		GD.PushWarning("World3 creation end");

		CreatePlayer1();
		// This ensures the player who starts the game is the player who control the UI forever
		UiActionsContainer.SetJoypad(UiActionsContainer.CurrentJoyPad);
		_subViewport1.AddChild(WorldScene);
		await GetTree().AwaitProcessFrame();

		var bullets = new Node();
		bullets.Name = "Bullets";
		WorldScene.AddChild(bullets);
		HudScene.StartGame();
		
		// Delay until needed
		AllowAddingP2();				
		//CreatePlayer2(1);
	}

	public void CreatePlayer1() {
		WorldScene.RemoveAllPlayers();
		var playerMapping = JoypadPlayersMapping.AddPlayer().SetJoypadId(UiActionsContainer.CurrentJoyPad);
		var player = WorldScene.AddPlayerToScene(playerMapping, _subViewport1);
		DisablePlayer2();
	}

	public void CreatePlayer2(int joypad) {
		if (JoypadPlayersMapping.Players >= MaxPlayer) throw new Exception("No more players allowed");
		EnablePlayer2();
		var playerMapping = JoypadPlayersMapping.AddPlayer().SetJoypadId(joypad);
		var player = WorldScene.AddPlayerToScene(playerMapping, _subViewport2);
	}

	private void EnablePlayer2() {
		var halfScreen = new Vector2I((int)Size.X / 2, (int)Size.Y);
		_subViewport1.Size = halfScreen;
		_subViewport2.Size = halfScreen;
		
		_subViewport2.GetParent<SubViewportContainer>().Visible = true;
		_subViewport2.World2D = _subViewport1.World2D;
		HudScene.EnablePlayer2();
	}

	public void DisablePlayer2() {
		_subViewport2.World2D = _noWorld;
		_subViewport2.GetParent<SubViewportContainer>().Visible = false;
		_subViewport1.Size = new Vector2I((int)Size.X, (int)Size.Y);
		HudScene.DisablePlayer2();
	}

	public async Task End() {
		NoAddingP2();				
		HudScene.EndGame();
		// FreeSceneAndKeepPoolData();
		FreeSceneAndUnloadResources();
		WorldScene.QueueFree();
		WorldScene = null;
		await GetTree().AwaitProcessFrame();
		GC.GetTotalMemory(true);

		PrintOrphanNodes();
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
