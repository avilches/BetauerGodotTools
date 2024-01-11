using System;
using System.Threading.Tasks;
using Betauer.Application.Monitor;
using Betauer.Application.Persistent;
using Betauer.Application.Persistent.Json;
using Betauer.Application.SplitScreen;
using Betauer.Core.Pool;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Betauer.Input;
using Betauer.Input.Joypad;
using Godot;
using Veronenger.Game.Platform.Character.InputActions;
using Veronenger.Game.Platform.Character.Player;
using Veronenger.Game.Platform.HUD;
using Veronenger.Game.UI;

namespace Veronenger.Game.Platform.World;

public partial class PlatformGameView : Control, IInjectable, IGameView {

	[Inject] private Betauer.DI.Container Container { get; set; }
	[Inject] private DebugOverlayManager DebugOverlayManager { get; set; }
	[Inject] private SceneTree SceneTree { get; set; }
	[Inject] private GameObjectRepository GameObjectRepository { get; set; }
	[Inject] private JsonGameLoader<PlatformSaveGameMetadata> PlatformGameObjectLoader { get; set; }

	[Inject] private MainBus MainBus { get; set; }
	[Inject] private ILazy<ProgressScreen> ProgressScreenLazy { get; set; }
	[Inject] private GameLoader GameLoader { get; set; }
	[Inject] private UiActions UiActions { get; set; }
	[Inject] private PlatformMultiPlayerContainer MultiPlayerContainer { get; set; }
	[Inject] private PlatformQuery PlatformQuery { get; set; }
	[Inject] private PlatformBus PlatformBus { get; set; }

	private SplitViewport _splitViewport;

	public const int MaxPlayer = 2;

	[Inject] private ITransient<PlatformHud> PlatformHudFactory { get; set; }
	[Inject] private ITransient<PlatformWorld> PlatformWorldFactory { get; set; }
	
	private PlatformHud PlatformHud { get; set; }
	private PlatformWorld PlatformWorld { get; set; }
	private int ActivePlayers => PlatformWorld != null ? PlatformWorld.Players.Count : 0;
	private bool _allowAddingP2 = true;
	private void AllowAddingP2() => _allowAddingP2 = true;
	private void NoAddingP2() => _allowAddingP2 = false;

	public Node GetWorld() => PlatformWorld;

	public void PostInject() {
		ConfigureDebugOverlays();
		_splitViewport = new SplitViewport {
			Camera1 = {
				Zoom = new Vector2(2, 2)
			},
			Camera2 = {
				Zoom = new Vector2(2, 2)
			},
			SubViewport1 = {
				CanvasItemDefaultTextureFilter = Viewport.DefaultCanvasItemTextureFilter.Nearest,
				RenderTargetUpdateMode = SubViewport.UpdateMode.Always,
				HandleInputLocally = false
			},
			SubViewport2 = {
				CanvasItemDefaultTextureFilter = Viewport.DefaultCanvasItemTextureFilter.Nearest,
				RenderTargetUpdateMode = SubViewport.UpdateMode.Always,
				HandleInputLocally = false
			}
		};
		_splitViewport.Configure(this, () => GetViewportRect().Size);
	}

	public override void _UnhandledInput(InputEvent e) {
		if (_allowAddingP2 && e is InputEventJoypadButton button && !MultiPlayerContainer.IsJoypadInUse(button.Device)) {
			CreatePlayer2(button.Device);
			GetViewport().SetInputAsHandled();
			if (MultiPlayerContainer.Players == MaxPlayer) NoAddingP2();				
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
		UiActions.SetJoypad(UiActions.CurrentJoypad);	// Player who starts the game is the player who control the UI forever
		
		await GameLoader.LoadPlatformGameResources();
		PlatformWorld = PlatformWorldFactory.Create();
		PlatformHud = PlatformHudFactory.Create();
		
		CurrentMetadata = new PlatformSaveGameMetadata();
		GameObjectRepository.Initialize();
		InitializeWorld();
		CreatePlayer1(UiActions.CurrentJoypad);
		AllowAddingP2();				
		PlatformQuery.Configure(PlatformWorld);
		PlatformWorld.StartNewGame();
		// _cameraController.WithMouseButton(MouseButton.Middle).Attach(_camera2D);
	}

	public async Task LoadFromMenu(string saveName) {
		SceneTree.Root.AddChild(this);
		PlatformQuery.Configure(PlatformWorld);
		UiActions.SetJoypad(UiActions.CurrentJoypad);	// Player who starts the game is the player who control the UI forever
		var (success, saveGame) = await LoadSaveGame(saveName);
		if (!success) return;
		await GameLoader.LoadPlatformGameResources();
		ContinueLoad(saveGame);
	}

	public async Task LoadInGame(string saveName) {
		UiActions.SetJoypad(UiActions.CurrentJoypad); // Player who starts the game is the player who control the UI forever
		var (success, saveGame) = await LoadSaveGame(saveName);
		if (!success) return;
		PlatformWorld.Free();
		ContinueLoad(saveGame);
	}

	private void ContinueLoad(SaveGame<PlatformSaveGameMetadata> saveGame) {
		CurrentMetadata = saveGame.Metadata;
		GameObjectRepository.Initialize();
		GameObjectRepository.LoadSaveObjects(saveGame.GameObjects);
		InitializeWorld();
		var consumer = new PlatformSaveGameConsumer(saveGame);
		LoadPlayer1(UiActions.CurrentJoypad, consumer);
		if (consumer.Player1 == null) AllowAddingP2();
		else NoAddingP2();
		PlatformWorld.LoadGame(consumer);
		HideLoading();
	}

	public async Task Save(string saveName) {
		MainBus.Publish(MainEvent.StartSavingGame);
		var l = await PlatformGameObjectLoader.ListMetadatas();
		try {
			var saveObjects = GameObjectRepository.GetSaveObjects();
			Action<float>? saveProgress = saveObjects.Count < 1000 ? null : (progress) => ProgressScreenLazy.Get().ShowSaving(progress);
			await PlatformGameObjectLoader.Save(saveName, CurrentMetadata, saveObjects, saveProgress);
		} catch (Exception e) {
			// Show saving error
			Console.WriteLine(e);
		}
		MainBus.Publish(MainEvent.EndSavingGame);
	}

	public void ShowLoading() {}
	public void HideLoading() {}
	
	public PlatformSaveGameMetadata CurrentMetadata { get; private set; }

	private async Task<(bool, SaveGame<PlatformSaveGameMetadata>)> LoadSaveGame(string save) {
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
		AddChild(PlatformHud);
		
		_splitViewport.SetCommonWorld(PlatformWorld);
		_splitViewport.OnChange += (split) => PlatformHud.SplitScreenContainer.Split = split;
		_splitViewport.Refresh();
		
		GetTree().Root.SizeChanged += () => _splitViewport.Refresh();
	}

	private PlayerNode CreatePlayer1(int joypadId) {
		PlatformPlayerActions playerActions = MultiPlayerContainer.AddPlayerActions(joypadId);
		PlayerNode playerNode = PlatformWorld.AddNewPlayer(playerActions);
		playerNode.SetCamera(_splitViewport.Camera1);
		return playerNode;
	}

	private PlayerNode LoadPlayer1(int joypadId, PlatformSaveGameConsumer consumer) {
		PlatformPlayerActions playerActions = MultiPlayerContainer.AddPlayerActions(joypadId);
		PlayerNode playerNode = PlatformWorld.LoadPlayer(consumer.Player0, consumer.Inventory0, playerActions);
		playerNode.SetCamera(_splitViewport.Camera1);
		return playerNode;
	}

	private PlayerNode CreatePlayer2(int joypadId) {
		if (MultiPlayerContainer.Players >= MaxPlayer) throw new Exception("No more players allowed");
		PlatformPlayerActions playerActions = MultiPlayerContainer.AddPlayerActions(joypadId);
		PlayerNode playerNode = PlatformWorld.AddNewPlayer(playerActions);
		playerNode.SetCamera(_splitViewport.Camera2);
		return playerNode;
	}
	
	public override void _Process(double delta) {
		ManageSplitScreen();
	}

	public void ManageSplitScreen() {
		if (ActivePlayers == 1) {
			// Ensure only one viewport is shown
			_splitViewport.Split = false;

		} else if (ActivePlayers == 2) {

			var p1Stage = PlatformWorld.Players[0].StageCameraController?.CurrentStage;
			var p2Stage = PlatformWorld.Players[1].StageCameraController?.CurrentStage;
			if (p1Stage == null || p2Stage == null) return;
			var sameStage = p1Stage == p2Stage;
			if (!sameStage) {
				_splitViewport.Split = true;
			} else {
				var p1Pos = PlatformWorld.Players[0].Marker2D.GlobalPosition;
				var p2Pos = PlatformWorld.Players[1].Marker2D.GlobalPosition;
				var distanceTo = p1Pos.DistanceTo(p2Pos);

				if (_splitViewport.Split) {
					if (distanceTo < (Size.X * 0.5 * 0.2f)) {
						_splitViewport.Split = false;
					}
				} else {
					if (distanceTo > (Size.X * 0.5 * 0.3f)) {
						_splitViewport.Split = true;
					}
				}
			}
		}
	}

	public async Task End(bool unload) {
		MultiPlayerContainer.RemoveAllPlayers();
		if (unload) {
			// If you comment this line, the objects in the pool will be used in the next game
			Container.ResolveAll<INodePool>().ForEach(p => p.FreeAll());
			GameLoader.UnloadPlatformGameResources();
		}
		Free();
		DebugOverlayManager.Overlay("Pool").Free();
		GC.GetTotalMemory(true);
		PrintOrphanNodes();
		PlatformBus.VerifyNoConsumers();
	}
	
	private void ConfigureDebugOverlays() {
		
		// DebugOverlayManager.Overlay("Pool")
			// .Children()
			// .TextField("Busy", () => PoolNodeContainer.BusyCount() + "")
			// .TextField("Available", () => PoolNodeContainer.AvailableCount() + "")
			// .TextField("Invalid", () => PoolNodeContainer.InvalidCount() + "");

	}
}
