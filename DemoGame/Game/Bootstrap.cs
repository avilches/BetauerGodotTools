using Betauer.Animation;
using Betauer.Application;
using Betauer.Application.Lifecycle;
using Betauer.Application.Monitor;
using Betauer.Application.Screen;
using Betauer.Camera;
using Betauer.Core.Restorer;
using Betauer.DI.ServiceProvider;
using Betauer.FSM;
using Betauer.NodePath;
using Betauer.Nodes;
using Betauer.Tools.Logging;
using Godot;
using PropertyTweener = Betauer.Animation.PropertyTweener;
using Container = Betauer.DI.Container;

namespace Veronenger.Game;

public partial class Bootstrap : Node /* needed to be instantiated as an Autoload from Godot */ {
	private static readonly Logger Logger = LoggerFactory.GetLogger<Bootstrap>();

	public Bootstrap() {
		Logging.Configure();
	}

	public override void _EnterTree() {
		var sceneTree = GetTree();
		NodePathScanner.ConfigureAutoInject(sceneTree);
		var container = new Container();
		container.Build(builder => {
			builder.InjectOnEnterTree(sceneTree);
			builder.Scan(GetType().Assembly);
			builder.Register(Provider.Static(sceneTree, "SceneTree"));
		});
		Logger.Info($"Bootstrap time: {Project.Uptime.TotalMilliseconds} ms");
		NodeManager.MainInstance.Node.OnWMCloseRequest += () => {
			GD.Print($"[OnWMCloseRequest] Uptime: {Project.Uptime.TotalMinutes:0} min {Project.Uptime.Seconds:00} sec");
		};
		container.Resolve<DebugOverlayManager>("DebugOverlayManager").DebugConsole.AddAllCommands();
		QueueFree();
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
		LoggerFactory.SetTraceLevel<Bootstrap>(TraceLevel.All);
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
		LoggerFactory.SetTraceLevel<Bootstrap>(TraceLevel.All);
		LoggerFactory.SetTraceLevel<ResourceLoaderContainer>(TraceLevel.All);
		LoggerFactory.SetTraceLevel<ConfigFileWrapper>(TraceLevel.All);

		// DI
		LoggerFactory.SetTraceLevel<Provider>(TraceLevel.Error);
		LoggerFactory.SetTraceLevel<Container>(TraceLevel.Error);
		LoggerFactory.SetTraceLevel<Container.Scanner>(TraceLevel.Error);

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
