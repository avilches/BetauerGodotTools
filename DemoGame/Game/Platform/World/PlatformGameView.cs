using System;
using System.Threading.Tasks;
using Betauer.Application.Monitor;
using Betauer.Application.Persistent;
using Betauer.Application.Persistent.Json;
using Betauer.Application.SplitScreen;
using Betauer.Core.Pool;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Betauer.Input;
using Betauer.Nodes;
using Godot;
using Veronenger.Game.Platform.Character.Player;
using Veronenger.Game.Platform.HUD;
using Veronenger.Game.UI;

namespace Veronenger.Game.Platform.World;

public partial class PlatformGameView : Control, IGameView {

	[Inject] private Betauer.DI.Container Container { get; set; }
	[Inject] private DebugOverlayManager DebugOverlayManager { get; set; }
	[Inject] private SceneTree SceneTree { get; set; }
	[Inject] private GameObjectRepository GameObjectRepository { get; set; }
	[Inject] private JsonGameLoader<PlatformSaveGameMetadata> PlatformGameObjectLoader { get; set; }

	[Inject] private MainBus MainBus { get; set; }
	[Inject] private ILazy<ProgressScreen> ProgressScreenLazy { get; set; }
	[Inject] private GameLoader GameLoader { get; set; }
	[Inject] private UiActions UiActions { get; set; }
	[Inject] private PlatformMultiPlayerContainer PlatformMultiPlayerContainer { get; set; }
	[Inject] private PlatformQuery PlatformQuery { get; set; }
	[Inject] private PlatformBus PlatformBus { get; set; }

	private readonly SplitViewport _splitViewport = new SplitViewport {
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

	public const int MaxPlayer = 2;

	[Inject] private ITransient<PlatformHud> PlatformHudFactory { get; set; }
	[Inject] private ITransient<PlatformWorld> PlatformWorldFactory { get; set; }
	
	private PlatformHud PlatformHud { get; set; }
	private PlatformWorld PlatformWorld { get; set; }
	private int ActivePlayers => PlatformWorld != null ? PlatformWorld.Players.Count : 0;
	private bool _allowAddingP2 = true;
	private void AllowAddingP2() => _allowAddingP2 = true;
	private void NoAddingP2() => _allowAddingP2 = false;

	public async Task StartNewGame(string? saveName = null) {
		SaveGame<PlatformSaveGameMetadata>? saveGame = null;
		if (saveName != null) {
			(var success, saveGame) = await LoadSaveGame(saveName);
			if (!success) return;
			CurrentMetadata = saveGame.Metadata;
		} else {
			CurrentMetadata = new PlatformSaveGameMetadata();
		}

		await GameLoader.Load(PlatformGameResources.GameLoaderTag);
		
		Configure();
		if (saveGame != null) {
			GameObjectRepository.LoadSaveObjects(saveGame.GameObjects);
			var consumer = new PlatformSaveGameConsumer(saveGame);

			LoadPlayer1(UiActions.JoypadId, consumer);
			if (consumer.Player1 == null) AllowAddingP2();
			else NoAddingP2();
			PlatformWorld.LoadGame(consumer);
		} else {
			CreatePlayer1(UiActions.JoypadId);
			AllowAddingP2();				
			PlatformWorld.StartNewGame();
		}
	}

	public async Task Save(string saveName) {
		MainBus.Publish(MainEvent.OnStartSavingGame);
		var l = await PlatformGameObjectLoader.ListMetadatas();
		try {
			var saveObjects = GameObjectRepository.GetSaveObjects();
			Action<float>? saveProgress = saveObjects.Count < 1000 ? null : (progress) => ProgressScreenLazy.Get().ShowSaving(progress);
			await PlatformGameObjectLoader.Save(saveName, CurrentMetadata, saveObjects, saveProgress);
		} catch (Exception e) {
			// Show saving error
			Console.WriteLine(e);
		}
		MainBus.Publish(MainEvent.OnEndSavingGame);
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

	private bool _initialized = false;
	private void Configure() {
		if (_initialized) throw new Exception("Already initialized, can't call it twice");
		_initialized = true;
		GameObjectRepository.Initialize(); // Singleton, so it must be initialized every time this class is created
		ConfigureDebugOverlays();

		UiActions.SetJoypad(UiActions.JoypadId);	// Player who starts the game is the player who control the UI forever
		SceneTree.Root.AddChild(this);
		
		PlatformWorld = PlatformWorldFactory.Create();
		PlatformHud = PlatformHudFactory.Create();
		PlatformQuery.Configure(PlatformWorld);
		AddChild(PlatformHud);
		
		_splitViewport.SetCommonWorld(PlatformWorld);
		_splitViewport.Configure(this, () => GetViewportRect().Size);
		_splitViewport.OnChange += (split) => PlatformHud.SplitScreenContainer.Split = split;
		_splitViewport.Split = false;
		_splitViewport.Refresh();
		
		GetTree().Root.SizeChanged += () => _splitViewport.Refresh();
		this.OnPhysicsProcess((e) => ManageSplitScreen());
		this.OnInput((e) => {
			if (_allowAddingP2 && e is InputEventJoypadButton button && !PlatformMultiPlayerContainer.IsJoypadInUse(button.Device)) {
				CreatePlayer2(button.Device);
				GetViewport().SetInputAsHandled();
				if (PlatformMultiPlayerContainer.Players == MaxPlayer) NoAddingP2();
			} else if (e.IsKeyReleased(Key.U)) {
				if (ActivePlayers < 2) {
					CreatePlayer2(1);
					GetViewport().SetInputAsHandled();
				}
			} else if (e.IsKeyReleased(Key.F5)) {
				Save("savegame");
			} else if (e.IsKeyReleased(Key.F6)) {
				MainBus.Publish(MainEvent.TriggerLoadInGame);
			}
		});
		PlatformMultiPlayerContainer.Start();
	}

	private PlayerNode CreatePlayer1(int joypadId) {
		PlatformPlayerActions playerActions = PlatformMultiPlayerContainer.AddPlayerActions(joypadId, true);
		PlayerNode playerNode = PlatformWorld.AddNewPlayer(playerActions);
		playerNode.SetCamera(_splitViewport.Camera1);
		return playerNode;
	}

	private PlayerNode LoadPlayer1(int joypadId, PlatformSaveGameConsumer consumer) {
		PlatformPlayerActions playerActions = PlatformMultiPlayerContainer.AddPlayerActions(joypadId, true);
		PlayerNode playerNode = PlatformWorld.LoadPlayer(consumer.Player0, consumer.Inventory0, playerActions);
		playerNode.SetCamera(_splitViewport.Camera1);
		return playerNode;
	}

	private PlayerNode CreatePlayer2(int joypadId) {
		if (PlatformMultiPlayerContainer.Players >= MaxPlayer) throw new Exception("No more players allowed");
		PlatformPlayerActions playerActions = PlatformMultiPlayerContainer.AddPlayerActions(joypadId, false);
		PlayerNode playerNode = PlatformWorld.AddNewPlayer(playerActions);
		playerNode.SetCamera(_splitViewport.Camera2);
		return playerNode;
	}
	
	private void ManageSplitScreen() {
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
		PlatformMultiPlayerContainer.Stop();
		if (unload) {
			// If you comment this line, the objects in the pool will be used in the next game
			Container.ResolveAll<INodePool>().ForEach(p => p.FreeAll());
			GameLoader.UnloadResources(PlatformGameResources.GameLoaderTag);
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
