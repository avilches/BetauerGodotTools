using Betauer.Animation.Easing;
using Betauer.Application.Monitor;
using Betauer.Application.Persistent;
using Betauer.Camera;
using Betauer.Camera.Control;
using Betauer.Core;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.FSM.Sync;
using Betauer.Input;
using Betauer.NodePath;
using Betauer.UI;
using Godot;

namespace Veronenger.Game.RTS.World;

public partial class RtsWorld : Node, IInjectable {
	[Inject] private WorldGenerator WorldGenerator { get; set; }
	[Inject] private GameObjectRepository GameObjectRepository { get; set; }
	[Inject] public RtsConfig RtsConfig { get; private set; }
	[Inject] public CameraContainer CameraContainer { get; private set; }
	[Inject] protected DebugOverlayManager DebugOverlayManager { get; set; }
	private readonly DragCameraController _dragCameraController = new();

	[NodePath("TerrainTileMap")] private TileMap TerrainTileMap { get; set; }
	[NodePath("TextureHeight")] private Sprite2D TextureHeight { get; set; }
	[NodePath("TextureHumidity")] private Sprite2D TextureHumidity { get; set; }
	[NodePath("TexturePoisson")] private Sprite2D TexturePoisson { get; set; }

	private CameraController CameraController;
	private CameraGameObject CameraGameObject;
	private Camera2D Camera => CameraController.Camera2D;

	public enum RtsState {
		DoNothing,
		Idle
	}

	public enum RtsTransition {
		Idle,
	}

	private readonly FsmNodeSync<RtsState, RtsTransition> _fsm = new(RtsState.DoNothing, "ScreenState.FSM", true);

	public void PostInject() {
		_fsm.On(RtsTransition.Idle).Set(RtsState.Idle);

		_fsm.State(RtsState.DoNothing)
			.Enter(() => { _dragCameraController.Enable(false); })
			.Build();

		_fsm.State(RtsState.Idle)
			.Enter(() => { _dragCameraController.Enable(); })
			.OnInput(Zooming)
			.Build();

		_fsm.Execute();

		AddChild(_fsm);
	}
	
	private void Zooming(InputEvent @event) {
		if (@event.IsKeyJustPressed(Key.Q) || @event.IsClickPressed(MouseButton.WheelUp)) {
			if (CameraGameObject.ZoomLevel == RtsConfig.ZoomLevels.Count - 1) return;
			var targetZoom = RtsConfig.ZoomLevels[++CameraGameObject.ZoomLevel];
			CameraController.Zoom(new Vector2(targetZoom, targetZoom), RtsConfig.ZoomTime, Easings.Linear, CameraController.Camera2D.GetLocalMousePosition);

			GetViewport().SetInputAsHandled();
		} else if (@event.IsKeyJustPressed(Key.E) || @event.IsClickPressed(MouseButton.WheelDown)) {
			if (CameraGameObject.ZoomLevel == 0) return;
			var targetZoom = RtsConfig.ZoomLevels[--CameraGameObject.ZoomLevel];
			CameraController.Zoom(new Vector2(targetZoom, targetZoom), RtsConfig.ZoomTime, Easings.Linear, CameraController.Camera2D.GetLocalMousePosition);

			GetViewport().SetInputAsHandled();
		}
	}

	public void SetMainCamera(Camera2D camera2D) {
		CameraController = CameraContainer.Camera(camera2D);
		_dragCameraController.Attach(camera2D).WithMouseButton(MouseButton.Left).Enable(false);
	}

	public async void StartNewGame() {
		CameraGameObject = GameObjectRepository.Create<CameraGameObject>("ScreenState", "ScreenState");
		CameraGameObject.ZoomLevel = 1;
		CameraGameObject.Position = new Vector2(4500, 3200);
		
		Init();
		TerrainTileMap.Clear();
		WorldGenerator.Configure(TerrainTileMap);
		WorldGenerator.Generate();
		ConfigureDebugOverlay();
		// var poissonDemos = new PoissonDemos(TextureTerrainMap, TexturePoisson);
		// AddChild(poissonDemos);
		// poissonDemos.QueueFree();
	}

	public void LoadGame(RtsSaveGameConsumer consumer) {
		CameraGameObject = GameObjectRepository.Get<CameraGameObject>("ScreenState");
		// Load the values from the save game

		Init();
		ConfigureDebugOverlay();
	}

	private void Init() {
		CameraGameObject.Configure(Camera);
		var zoom = RtsConfig.ZoomLevels[CameraGameObject.ZoomLevel];
		CameraController.Camera2D.Zoom = new Vector2(zoom, zoom);
		CameraController.Camera2D.Position = CameraGameObject.Position;
		_fsm.Send(RtsTransition.Idle);
	}
	
	private void ConfigureDebugOverlay() {
		var viewGroup = new ButtonGroup();

		DebugOverlayManager.Overlay(TerrainTileMap)
			.OnDestroy(() => viewGroup.Dispose())
			.SetMinSize(400, 100)
			.Children()
			.Edit("Seed", WorldGenerator.Seed, seed => {
				WorldGenerator.Seed = seed;
				WorldGenerator.Generate();
			}, (edit) => edit.SetMinSize(20))
			.Add<HBoxContainer>(box => box.Children()
				.Label("View Mode")
				.ToggleButton("Terrain", () => WorldGenerator.CurrentViewMode == WorldGenerator.ViewMode.Terrain, () => {
					WorldGenerator.CurrentViewMode = WorldGenerator.ViewMode.Terrain;
					WorldGenerator.UpdateView();
				})
				.ToggleButton("HeightFallOff", () => WorldGenerator.CurrentViewMode == WorldGenerator.ViewMode.HeightFalloff, () => {
					WorldGenerator.CurrentViewMode = WorldGenerator.ViewMode.HeightFalloff;
					WorldGenerator.UpdateView();
				})
				.ToggleButton("Height", () => WorldGenerator.CurrentViewMode == WorldGenerator.ViewMode.Height, () => {
					WorldGenerator.CurrentViewMode = WorldGenerator.ViewMode.Height;
					WorldGenerator.UpdateView();
				})
				.ToggleButton("Humidity", () => WorldGenerator.CurrentViewMode == WorldGenerator.ViewMode.Humidity, () => {
					WorldGenerator.CurrentViewMode = WorldGenerator.ViewMode.Humidity;
					WorldGenerator.UpdateView();
				})
				.ToggleButton("FallOff", () => WorldGenerator.CurrentViewMode == WorldGenerator.ViewMode.FalloffMap, () => {
					WorldGenerator.CurrentViewMode = WorldGenerator.ViewMode.FalloffMap;
					WorldGenerator.UpdateView();
				})
			)
			.Label("Height")
			.Edit("Frequency", () => WorldGenerator.BiomeGenerator.HeightNoise.Frequency, (value) => {
				WorldGenerator.BiomeGenerator.HeightNoise.Frequency = value;
				WorldGenerator.Generate();
			})
			.Add<HBoxContainer>(box => box.Children()
				.ToggleButton("Enabled", () => WorldGenerator.BiomeGenerator.FalloffEnabled, () => {
					WorldGenerator.BiomeGenerator.FalloffEnabled = !WorldGenerator.BiomeGenerator.FalloffEnabled;
					WorldGenerator.Generate();
				})
				.Edit("FallOff Exp", () => WorldGenerator.BiomeGenerator.FallOffMapExp, (value) => {
					WorldGenerator.BiomeGenerator.FallOffMapExp = value;
					WorldGenerator.Generate();
				})
				.Edit("FallOff Offset", () => WorldGenerator.BiomeGenerator.FallOffMapOffset, (value) => {
						WorldGenerator.BiomeGenerator.FallOffMapOffset = value;
						WorldGenerator.Generate();
					}
				)
			)
			.Label("Humidity")
			.Edit("Frequency", () => WorldGenerator.BiomeGenerator.HumidityNoise.Frequency, (value) => {
				WorldGenerator.BiomeGenerator.HumidityNoise.Frequency = value;
				WorldGenerator.Generate();
			})
			// .Builder()
			// .Button("Generate", (b) => {
			// 	WorldGenerator.Generate();
			// }).End()
			// .Button("Generate", (b) => {
			// 	WorldGenerator.Generate();
			// }).End()
			// .Button("Generate", (b) => {
			// 	WorldGenerator.Generate();
			// }).End()
			//
			//
			;
		
		

	}

}
