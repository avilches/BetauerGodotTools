using System.Threading.Tasks;
using Betauer.Animation;
using Betauer.Application;
using Betauer.Application.Lifecycle;
using Betauer.Application.Monitor;
using Betauer.Application.Screen;
using Betauer.Camera;
using Betauer.Core.Nodes.Property;
using Betauer.Core.Restorer;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.DI.ServiceProvider;
using Betauer.FSM;
using Betauer.Input;
using Betauer.NodePath;
using Betauer.Nodes;
using Betauer.Tools.Logging;
using Godot;
using Container = Godot.Container;
using PropertyTweener = Betauer.Animation.PropertyTweener;

namespace Veronenger.Game; 

public partial class SplashScreen : CanvasLayer {
	private static readonly Logger Logger = LoggerFactory.GetLogger<SplashScreen>();

	[NodePath("%SplashScreen")] private Control _base;
	[NodePath("%TextureRect")] private TextureRect _sprite;
	
	[Inject] private GameLoader GameLoader { get; set; }
	[Inject] private UiActionsContainer UiActionsContainer { get; set; }
	[Inject] private DebugOverlayManager DebugOverlayManager { get; set; }
	[Inject] private IMain Main { get; set; }
	
	public SplashScreen() {
		Logging.Configure();
		NodeManager.MainInstance.OnWMCloseRequest += () => {
			GD.Print($"[OnWMCloseRequest] Uptime: {Project.Uptime.TotalMinutes:0} min {Project.Uptime.Seconds:00} sec");
		};

		TreeEntered += () => {
			var sceneTree = GetTree();
			NodePathScanner.ConfigureAutoInject(sceneTree); // This adds a hook to inject the [NodePath] attributes.
			StartContainer(sceneTree);
			DebugOverlayManager!.DebugConsole.AddAllCommands();
		};
		
		Ready += async () => {
			await LoadResources();
			AddSibling(Main as Node);
			QueueFree();
		};
	}

	private void StartContainer(SceneTree sceneTree) {
		var container = new Betauer.DI.Container();
		container.Build(builder => {
			builder.InjectOnEnterTree(sceneTree);
			builder.Scan(GetType().Assembly);
			builder.Register(Provider.Static(sceneTree, "SceneTree"));
		});
		container.InjectServices(this); // This ensures the [Inject] attributes are injected in the SplashScreen
	}

	private async Task LoadResources() {
		// TODO Godot 4
		// Vector2 _baseResolutionSize = _screenSettingsManager.WindowedResolution.Size;
		// if (_screenSettingsManager.Fullscreen) {
		// OS.WindowFullscreen = true;
		// } else {
		// OS.WindowSize = _screenSettingsManager.WindowedResolution.Size;
		// OS.CenterWindow();
		// }
		// GetTree().SetScreenStretch(Window.ContentScaleModeEnum.CanvasItems, Window.ContentScaleAspectEnum.Keep,_baseResolutionSize, 1);
		
		var tween = SequenceAnimation
			.Create(_sprite)
			.AnimateSteps(Properties.Modulate)
			.From(Colors.White)
			.To(Colors.Red, 0.2f)
			.EndAnimate()
			.SetInfiniteLoops()
			.Play();

		await GameLoader.LoadMainResources(); // (ResourceProgress rp) => { Logger.Info($"{rp.ResourcePercent:P} {rp.Resource}"); });
		Logger.Info($"Bootstrap time: {Project.Uptime.TotalMilliseconds} ms");
		tween.Kill();
		
		await NodeManager.MainInstance.AwaitInput(e => {
			if (e.IsJustPressed() && (e.IsAnyKey() || e.IsAnyButton() || e.IsAnyClick())) {
				if (e is InputEventJoypadButton button) {
					UiActionsContainer.SetJoypad(button.Device); // Configure the default joypad for the UI
				}
				return true;
			}
			return false;
		});
	}
}


file static class Logging {

	public static void Configure() {
		AppTools.AddLogOnException();
		AppTools.AddQuitGameOnException();

		// new Task(() => throw new Exception()).Start();
		// new Thread(() => throw new Exception()).Start();
		// throw new Exception("Xxxxx");

#if DEBUG
		DevelopmentConfig();
#else
			ExportConfig();
#endif
			
		GD.Print(string.Join("\n", Project.GetOSInfo()));
		GD.Print(string.Join("\n", Project.GetSettings(
			"debug/file_logging/enable_file_logging",
			"debug/file_logging/enable_file_logging.pc",
			"debug/file_logging/log_path",
			"debug/file_logging/log_path.standalone",
			"application/run/disable_stdout",
			"application/run/disable_stderr",
			"application/run/flush_stdout_on_print",
			"application/run/flush_stdout_on_print.debug",
			"application/config/use_custom_user_dir",
			"application/config/project_settings_override",
			"application/config/version"
		)));
	}

	private static void ExportConfig() {
		LoggerFactory.SetDefaultTraceLevel(TraceLevel.Warning);

		// Bootstrap logs, always log everything :)
		LoggerFactory.SetTraceLevel<SplashScreen>(TraceLevel.All);
		LoggerFactory.SetTraceLevel<ResourceLoaderContainer>(TraceLevel.All);
		LoggerFactory.SetTraceLevel<ConfigFileWrapper>(TraceLevel.All);
		LoggerFactory.SetTraceLevel<BaseScreenResolutionStrategy>(TraceLevel.All);
		PropertyNameRestorer.OverrideBehaviour = PropertyNameRestorer.Behaviour.DoNothing;
	}

	private static void DevelopmentConfig() {
		PropertyNameRestorer.OverrideBehaviour = PropertyNameRestorer.Behaviour.SaveAndLog;

		
		// All enabled, then disabled one by one, so developers can enable just one 
		LoggerFactory.SetDefaultTraceLevel(TraceLevel.All);

		// Bootstrap logs, always log everything :)
		LoggerFactory.SetTraceLevel<SplashScreen>(TraceLevel.All);
		LoggerFactory.SetTraceLevel<ResourceLoaderContainer>(TraceLevel.All);
		LoggerFactory.SetTraceLevel<ConfigFileWrapper>(TraceLevel.All);

		// DI
		LoggerFactory.SetTraceLevel<Provider>(TraceLevel.Error);
		LoggerFactory.SetTraceLevel<Container>(TraceLevel.Error);
		LoggerFactory.SetTraceLevel<Scanner>(TraceLevel.Error);
		LoggerFactory.SetTraceLevel<Injector>(TraceLevel.Error);
		LoggerFactory.SetTraceLevel<SingletonProvider>(TraceLevel.Error);
		LoggerFactory.SetTraceLevel<TransientProvider>(TraceLevel.Error);

		// GameTools
		LoggerFactory.SetTraceLevel<BaseScreenResolutionStrategy>(TraceLevel.All);
		LoggerFactory.SetTraceLevel<Fsm>(TraceLevel.Error);

		// Animation
		LoggerFactory.SetTraceLevel<CallbackNodeTweener>(TraceLevel.Error);
		LoggerFactory.SetTraceLevel<CallbackTweener>(TraceLevel.Error);
		LoggerFactory.SetTraceLevel<PauseTweener>(TraceLevel.Error);
		LoggerFactory.SetTraceLevel<PropertyTweener>(TraceLevel.Error);

		// Game
		LoggerFactory.SetTraceLevel<Main>(TraceLevel.Error);
		LoggerFactory.SetTraceLevel<StageController>(TraceLevel.Error);

		// Player and enemies
		// LoggerFactory.SetTraceLevel<PlayerNode>(TraceLevel.All);
		// LoggerFactory.SetTraceLevel<ZombieNode>(TraceLevel.All);
			
		// LoggerFactory.OverrideTraceLevel(TraceLevel.All);
	}
}
