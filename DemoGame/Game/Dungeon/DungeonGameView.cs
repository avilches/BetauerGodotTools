using System;
using System.Threading.Tasks;
using Betauer.Application.Lifecycle;
using Betauer.Application.Monitor;
using Betauer.Application.Persistent;
using Betauer.Application.Persistent.Json;
using Betauer.Camera.Control;
using Betauer.Core.Pool;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Godot;
using Veronenger.Game.Dungeon.World;
using Veronenger.Game.RTS;
using Veronenger.Game.UI;

namespace Veronenger.Game.Dungeon;

public partial class DungeonGameView : IGameView {

	[Inject] private Betauer.DI.Container Container { get; set; }
	// [Inject] private DebugOverlayManager DebugOverlayManager { get; set; }
	[Inject] private SceneTree SceneTree { get; set; }
	[Inject] private GameObjectRepository GameObjectRepository { get; set; }
	// [Inject] private JsonGameLoader<RtsSaveGameMetadata> RtsGameObjectLoader { get; set; }
	[Inject] public DungeonPlayerActions DungeonPlayerActions { get; set; }

	// [Inject] private MainBus MainBus { get; set; }
	// [Inject] private ILazy<ProgressScreen> ProgressScreenLazy { get; set; }
	[Inject] private GameLoader GameLoader { get; set; }
	// [Inject] private UiActions UiActions { get; set; }
	
	[Inject] private ITransient<DungeonMap> DungeonMapFactory { get; set; }
	[Inject] public CameraContainer CameraContainer { get; private set; }

	[Inject("Templates")] private ResourceHolder<TextResource> Templates { get; set; }

	public DungeonMap DungeonMap;
	public RogueWorld RogueWorld;

	private bool _initialized = false;

	public async Task StartNewGame(string? saveName = null) {
		await GameLoader.Load(DungeonGameResources.GameLoaderTag);
		Configure();
		DungeonMap.StartGame();
	}

	// This method could be called in new games or loaded games
	private void Configure() {
		if (_initialized) throw new Exception("Already initialized, can't call it twice");
		_initialized = true;
		RogueWorld.Configure(Templates.Get().Text);
		GameObjectRepository.Initialize(); // Singleton, so it must be initialized every time this class is created

		RogueWorld = new RogueWorld(10);
		DungeonMap = DungeonMapFactory.Create();
		DungeonMap.Configure(RogueWorld, CreateCamera());

		DungeonPlayerActions.Start();
		DungeonPlayerActions.EnableAll();

		SceneTree.Root.AddChild(DungeonMap); // last step (it calls to all the _Ready method/events)
	}

	private CameraController CreateCamera() {
		var camera = new Camera2D();
		camera.Enabled = true;
		return CameraContainer.Camera(camera);
	}

	public async Task End(bool unload) {
		if (unload) {
			// If you comment this line, the objects in the pool will be used in the next game
			Container.ResolveAll<INodePool>().ForEach(p => p.FreeAll());
			GameLoader.UnloadResources(DungeonGameResources.GameLoaderTag);
		}
		RogueWorld.WorldMap.TurnSystem.Stop();
		DungeonPlayerActions.DisableAll();
		DungeonPlayerActions.Stop();

		DungeonMap.Free();
		// DebugOverlayManager.Overlay("Pool").Free();
		GC.GetTotalMemory(true);
		DungeonMap.PrintOrphanNodes();
	}
}
