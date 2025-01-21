using System;
using System.Threading.Tasks;
using Betauer.Application.Monitor;
using Betauer.Application.Persistent;
using Betauer.Application.Persistent.Json;
using Betauer.Application.SplitScreen;
using Betauer.Core.Pool;
using Betauer.Core.Signal;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Betauer.Input;
using Betauer.Nodes;
using Godot;
using Veronenger.Game.Platform;
using Veronenger.Game.RTS.HUD;
using Veronenger.Game.UI;

namespace Veronenger.Game.RTS.World;

public partial class RtsGameView : Control, IGameView {

	[Inject] private Betauer.DI.Container Container { get; set; }
	[Inject] private DebugOverlayManager DebugOverlayManager { get; set; }
	[Inject] private SceneTree SceneTree { get; set; }
	[Inject] private GameObjectRepository GameObjectRepository { get; set; }
	[Inject] private JsonGameLoader<RtsSaveGameMetadata> RtsGameObjectLoader { get; set; }
	
	[Inject] private MainBus MainBus { get; set; }
	[Inject] private ILazy<ProgressScreen> ProgressScreenLazy { get; set; }
	[Inject] private GameLoader GameLoader { get; set; }
	[Inject] private UiActions UiActions { get; set; }
	
	

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

	[Inject] private ITransient<RtsWorld> RtsWorldFactory { get; set; }
	[Inject] private ITransient<RtsHud> RtsHudFactory { get; set; }

	private RtsHud RtsHud { get; set; } = null!;
	private RtsWorld RtsWorld { get; set; } = null!;

	public async Task StartNewGame(string? saveName = null) {
		SaveGame<RtsSaveGameMetadata>? saveGame = null;
		if (saveName != null) {
			(var success, saveGame) = await LoadSaveGame(saveName);
			if (!success) return;
			CurrentMetadata = saveGame.Metadata;
		} else {
			CurrentMetadata = new RtsSaveGameMetadata();
		}

		await GameLoader.Load(RtsGameResources.GameLoaderTag);
		
		Configure();
		if (saveGame != null) {
			GameObjectRepository.LoadSaveObjects(saveGame.GameObjects);
			var consumer = new RtsSaveGameConsumer(saveGame);

			RtsWorld.LoadGame(consumer);
		} else {
			// CreatePlayer1(UiActions.JoypadId);
			// AllowAddingP2();				
			RtsWorld.StartNewGame();
		}
	}

	public async Task Save(string saveName) {
		MainBus.Publish(MainEvent.OnStartSavingGame);
		var l = await RtsGameObjectLoader.ListMetadatas();
		try {
			var saveObjects = GameObjectRepository.GetSaveObjects();
			Action<float>? saveProgress = saveObjects.Count < 1000 ? null : (progress) => ProgressScreenLazy.Get().ShowSaving(progress);
			await RtsGameObjectLoader.Save(saveName, CurrentMetadata, saveObjects, saveProgress);
		} catch (Exception e) {
			// Show saving error
			Console.WriteLine(e);
		}
		MainBus.Publish(MainEvent.OnEndSavingGame);
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

	private bool _initialized = false;
	private void Configure() {
		if (_initialized) throw new Exception("Already initialized, can't call it twice");
		_initialized = true;
		GameObjectRepository.Initialize(); // Singleton, so it must be initialized every time this class is created
		ConfigureDebugOverlays();

		UiActions.SetJoypad(UiActions.JoypadId);	// Player who starts the game is the player who control the UI forever
		SceneTree.Root.AddChild(this);
		
		RtsWorld = RtsWorldFactory.Create();
		RtsWorld.SetMainCamera(_splitViewport.Camera1);
		RtsHud = RtsHudFactory.Create();
		// PlatformQuery.Configure(RtsWorld);
		AddChild(RtsHud);
		
		_splitViewport.SetCommonWorld(RtsWorld);
		_splitViewport.Configure(this, () => GetViewportRect().Size);
		_splitViewport.OnChange += (split) => RtsHud.SplitScreenContainer.Split = split;
		_splitViewport.Split = false;
		_splitViewport.Refresh();
		
		GetTree().Root.SizeChanged += () => _splitViewport.Refresh();
		// this.OnPhysicsProcess((e) => ManageSplitScreen());
		this.OnInput((e) => {
			// if (_allowAddingP2 && e is InputEventJoypadButton button && !PlatformMultiPlayerContainer.IsJoypadInUse(button.Device)) {
				// CreatePlayer2(button.Device);
				// GetViewport().SetInputAsHandled();
				// if (PlatformMultiPlayerContainer.Players == MaxPlayer) NoAddingP2();
			// } else if (e.IsKeyReleased(Key.U)) {
				// if (ActivePlayers < 2) {
					// CreatePlayer2(1);
					// GetViewport().SetInputAsHandled();
				// }
			// } else
			if (e.IsKeyReleased(Key.F5)) {
				Save("savegame");
			} else if (e.IsKeyReleased(Key.F6)) {
				MainBus.Publish(MainEvent.TriggerLoadInGame);
			}
		});
		// PlatformMultiPlayerContainer.Start();
	}
	
	public async Task End(bool unload) {
		// PlatformMultiPlayerContainer.Stop();
		if (unload) {
			// If you comment this line, the objects in the pool will be used in the next game
			Container.ResolveAll<INodePool>().ForEach(p => p.FreeAll());
			GameLoader.UnloadResources(RtsGameResources.GameLoaderTag);
		}
		Free();
		DebugOverlayManager.Overlay("Pool").Free();
		GC.GetTotalMemory(true);
		PrintOrphanNodes();
		// PlatformBus.VerifyNoConsumers();
	}

	private void ConfigureDebugOverlays() {
		// DebugOverlayManager.Overlay("Pool")
			// .Children()
			// .TextField("Busy", () => PoolNodeContainer.BusyCount() + "", 1)
			// .TextField("Available", () => PoolNodeContainer.AvailableCount() + "", 1)
			// .TextField("Invalid", () => PoolNodeContainer.InvalidCount() + "", 1);

	}
}
