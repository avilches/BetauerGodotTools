using System;
using System.Diagnostics;
using Betauer.Application.Lifecycle;
using Betauer.Application.Monitor;
using Betauer.Application.Persistent;
using Betauer.Camera;
using Betauer.Camera.Control;
using Betauer.Core.Easing;
using Betauer.Core.Nodes.Events;
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
	[Inject("DebugConsoleTheme")] private ResourceHolder<Theme> DebugConsoleTheme { get; set; }

	[NodePath("TerrainTileMap")] private TileMap TerrainTileMap { get; set; }
	// [NodePath("TextureHeight")] private Sprite2D TextureHeight { get; set; }
	// [NodePath("TextureHumidity")] private Sprite2D TextureHumidity { get; set; }
	// [NodePath("TexturePoisson")] private Sprite2D TexturePoisson { get; set; }
	// [NodePath("TextureFinalMap")] private Sprite2D TextureFinalMap { get; set; }
	[NodePath("TextureRect")] private TextureRect TextureRect { get; set; }
	
	[NodePath("FalloffGraph")] private TextureRect FalloffGraph { get; set; }
	[NodePath("HeightGraph")] private TextureRect HeightGraph { get; set; }
	
	[NodePath("ColorRect")] private ColorRect ColorRect { get; set; }
	[NodePath("%MouseInfo")] private Control MouseInfo { get; set; }
	[NodePath("%MouseText")] private Label MouseText { get; set; }

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
			.Execute(UpdateCursor)
			.Build();

		_fsm.Execute();

		AddChild(_fsm);
	}
	
	private Vector2 _lastMousePosition = Vector2.Zero;
	public void UpdateCursor() {
		var mousePosition = TextureRect.GetLocalMousePosition();
		if (mousePosition == _lastMousePosition) return;
		_lastMousePosition = mousePosition;
		var x = Mathf.RoundToInt(mousePosition.X);
		var y = Mathf.RoundToInt(mousePosition.Y);
		if (x < 0 || x >= WorldGenerator.BiomeGenerator.Width || y < 0 || y >= WorldGenerator.BiomeGenerator.Height) return;
		Console.WriteLine("Cell:"+x+","+y);
		var cell = WorldGenerator.BiomeGenerator.BiomeCells[x, y];
		ColorRect.Color = cell.Biome.Color;
		MouseInfo.Position = GetViewport().GetMousePosition() + new Vector2I(20, 20);
		MouseText.Text = $"{cell.HeightMt:0.00}c ({cell.Height:0.00})\n";
		// MouseText.Text = $"{cell.TempCelsius:0.00}c ({cell.Temp:0.00})\n{cell.HeightMt:0.00}c ({cell.Height:0.00})\n{cell.Humidity:0.00}%\n";
	}
	
	private async void Zooming(InputEvent e) {
		if (e.IsKeyJustPressed(Key.Q) || e.IsClickPressed(MouseButton.WheelUp)) {
			if (CameraController.IsBusy()) return;
			if (!CameraGameObject.ZoomIn()) return;
			GetViewport().SetInputAsHandled();
			await CameraController.Zoom(new Vector2(CameraGameObject.Zoom, CameraGameObject.Zoom), RtsConfig.ZoomTime, Interpolation.Linear, CameraController.Camera2D.GetLocalMousePosition);
			Console.WriteLine($"Zooming {CameraGameObject.Zoom}x Position:{CameraController.Camera2D.Position}");
		} else if (e.IsKeyJustPressed(Key.E) || e.IsClickPressed(MouseButton.WheelDown)) {
			if (CameraController.IsBusy()) return;
			if (!CameraGameObject.ZoomOut()) return;
			GetViewport().SetInputAsHandled();
			await CameraController.Zoom(new Vector2(CameraGameObject.Zoom, CameraGameObject.Zoom), RtsConfig.ZoomTime, Interpolation.Linear, CameraController.Camera2D.GetLocalMousePosition);
			Console.WriteLine($"Zooming {CameraGameObject.Zoom}x Position:{CameraController.Camera2D.Position}");
		}
	}

	public void SetMainCamera(Camera2D camera2D) {
		CameraController = CameraContainer.Camera(camera2D);
		_dragCameraController.Attach(camera2D).WithMouseButton(MouseButton.Left).Enable(false);
	}

	public async void StartNewGame() {
		CameraGameObject = GameObjectRepository.Create<CameraGameObject>("ScreenState", "ScreenState");
		CameraGameObject.Zoom = 4f;
		CameraGameObject.Position = new Vector2(795, 139);

		var s = Stopwatch.StartNew();
		Init();
		TerrainTileMap.Clear();
		WorldGenerator.Configure(TerrainTileMap, TextureRect);
		WorldGenerator.Generate();
		WorldGenerator.Draw(WorldGenerator.ViewMode.Terrain);
		WorldGenerator.BiomeGenerator.GraphFalloff(FalloffGraph);
		WorldGenerator.BiomeGenerator.GraphHeight(HeightGraph);
		ConfigureDebugOverlay();
		Console.WriteLine($"StartNewGame:{s.ElapsedMilliseconds}ms");
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
		CameraController.Camera2D.Zoom = new Vector2(CameraGameObject.Zoom, CameraGameObject.Zoom);
		// CameraController.Camera2D.Position = CameraGameObject.Position;
		_fsm.Send(RtsTransition.Idle);
	}
	
	private void ConfigureDebugOverlay() {
		var viewGroup = new ButtonGroup();

		DebugOverlayManager.Overlay(TerrainTileMap)
			.Title("PCG")
			.OnDestroy(() => viewGroup.Dispose())
			.SetMinSize(400, 600)
			.Children()
			.Edit("Seed", WorldGenerator.Seed.ToString(), seed => {
				WorldGenerator.Seed = seed.ToInt();
				WorldGenerator.Generate();
				WorldGenerator.ReDraw();
				WorldGenerator.BiomeGenerator.GraphFalloff(FalloffGraph);
				WorldGenerator.BiomeGenerator.GraphHeight(HeightGraph);
			}, (edit) => edit.SetMinWidth(20))
			.Edit("Biome", WorldGenerator.BiomeGenerator.LandBiomesConfig, (value) => {
				WorldGenerator.BiomeGenerator.ConfigureLandBiomeMap(value);
				WorldGenerator.Generate();
				WorldGenerator.ReDraw();
				WorldGenerator.BiomeGenerator.GraphFalloff(FalloffGraph);
				WorldGenerator.BiomeGenerator.GraphHeight(HeightGraph);
			}, config => {
				config.SetMultiLine(true).SetMinHeight(300).SetMinWidth(300);
				config.TextEdit.Theme = DebugConsoleTheme.Get();
			})
			.Label("---------- Layer -------------")
			.Add<HBoxContainer>(box => box.Children()
				.ToggleButton("Massland", () => WorldGenerator.CurrentViewMode == WorldGenerator.ViewMode.Massland, (v) => {
					WorldGenerator.Draw(WorldGenerator.ViewMode.Massland);
				}, viewGroup)
				.ToggleButton("Height", () => WorldGenerator.CurrentViewMode == WorldGenerator.ViewMode.Height, (v) => {
					WorldGenerator.Draw(WorldGenerator.ViewMode.Height);
				}, viewGroup)
				.ToggleButton("HeightFallOff", () => WorldGenerator.CurrentViewMode == WorldGenerator.ViewMode.HeightFalloff, (v) => {
					WorldGenerator.Draw(WorldGenerator.ViewMode.HeightFalloff);
				}, viewGroup)
				.ToggleButton("Humidity", () => WorldGenerator.CurrentViewMode == WorldGenerator.ViewMode.Humidity, (v) => {
					WorldGenerator.Draw(WorldGenerator.ViewMode.Humidity);
				}, viewGroup)
				.ToggleButton("Temp", () => WorldGenerator.CurrentViewMode == WorldGenerator.ViewMode.Temperature, (v) => {
					WorldGenerator.Draw(WorldGenerator.ViewMode.Temperature);
				}, viewGroup)
				.ToggleButton("Terrain", () => WorldGenerator.CurrentViewMode == WorldGenerator.ViewMode.Terrain, (v) => {
					WorldGenerator.Draw(WorldGenerator.ViewMode.Terrain);
				}, viewGroup)
			)
			.Label("---------- Terrain -------------")
			.Add<HBoxContainer>(box => box.Children()
				.Edit("Frequency", () => WorldGenerator.BiomeGenerator.HeightNoise.Frequency, (value) => {
					WorldGenerator.BiomeGenerator.HeightNoise.Frequency = value;
					WorldGenerator.Generate();
					WorldGenerator.ReDraw();
					WorldGenerator.BiomeGenerator.GraphFalloff(FalloffGraph);
					WorldGenerator.BiomeGenerator.GraphHeight(HeightGraph);
				})
				.ToggleButton("Falloff", () => WorldGenerator.BiomeGenerator.FalloffEnabled, (v) => {
					WorldGenerator.BiomeGenerator.FalloffEnabled = v;
					WorldGenerator.Generate();
					WorldGenerator.ReDraw();
					WorldGenerator.BiomeGenerator.GraphFalloff(FalloffGraph);
					WorldGenerator.BiomeGenerator.GraphHeight(HeightGraph);
				})
				.Edit("Bias", () => WorldGenerator.BiomeGenerator.MasslandBias, (value) => {
					WorldGenerator.BiomeGenerator.MasslandBias = value;
					WorldGenerator.Generate();
					WorldGenerator.ReDraw();
					WorldGenerator.BiomeGenerator.GraphFalloff(FalloffGraph);
					WorldGenerator.BiomeGenerator.GraphHeight(HeightGraph);
				})
				.Edit("Offset", () => WorldGenerator.BiomeGenerator.MasslandOffset, (value) => {
					WorldGenerator.BiomeGenerator.MasslandOffset = value;
					WorldGenerator.Generate();
					WorldGenerator.ReDraw();
					WorldGenerator.BiomeGenerator.GraphFalloff(FalloffGraph);
					WorldGenerator.BiomeGenerator.GraphHeight(HeightGraph);
				})
			)
			.Label("---------- Humidity -------------")
			.Add<HBoxContainer>(box => box.Children()
				.ToggleButton("Enabled", () => WorldGenerator.BiomeGenerator.HumidityEnabled, (v) => {
					WorldGenerator.BiomeGenerator.HumidityEnabled = v;
					WorldGenerator.Generate();
					WorldGenerator.ReDraw();
				})
				.Edit("Frequency", () => WorldGenerator.BiomeGenerator.HumidityNoise.Frequency, (value) => {
					WorldGenerator.BiomeGenerator.HumidityNoise.Frequency = value;
					WorldGenerator.Generate();
					WorldGenerator.ReDraw();
				})
			);
	}

}
