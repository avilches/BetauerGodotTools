using Betauer.Animation;
using Betauer.Application;
using Betauer.Application.Monitor;
using Betauer.Application.Notifications;
using Betauer.Application.Screen;
using Betauer.Camera;
using Betauer.DI;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.Logging;
using Betauer.Core.Pool;
using Betauer.StateMachine;
using Godot;
using Veronenger.Character.Player;
using Container = System.ComponentModel.Container;
using PropertyTweener = Betauer.Animation.PropertyTweener;
using ZombieNode = Veronenger.Character.Npc.ZombieNode;

namespace Veronenger.Managers.Autoload; 

public partial class Bootstrap : Node /* needed to be instantiated as an Autoload from Godot */ {
    private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(Bootstrap));

    [Inject] private NotificationsHandler NotificationsHandler { get; set; }
    [Inject] private DebugOverlayManager DebugOverlayManager { get; set; }
    [Inject] private WindowNotificationStatus WindowNotificationStatus { get; set; }

    public Bootstrap() {
        AppTools.ConfigureExceptionHandlers(GetTree);

        new GodotContainer(this)
            .Start(options => {
                options
                    .ScanConfiguration(new DefaultConfiguration(GetTree()))
                    .Scan(GetType().Assembly);
            });

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

    public override void _Ready() {
        Name = nameof(Bootstrap); // This name is shown in the remote editor
        Logger.Info($"Bootstrap time: {Project.Uptime.TotalMilliseconds} ms");
        NotificationsHandler.OnWMCloseRequest += () => {
            GD.Print($"[WmQuitRequest] Uptime: {Project.Uptime.TotalMinutes:0} min {Project.Uptime.Seconds:00} sec");
        };
        DebugOverlayManager.DebugConsole.AddAllCommands(WindowNotificationStatus);
    }

    private static void ExportConfig() {
        LoggerFactory.SetDefaultTraceLevel(TraceLevel.Warning);

        // Bootstrap logs, always log everything :)
        LoggerFactory.SetTraceLevel(typeof(Bootstrap), TraceLevel.All);
        LoggerFactory.SetTraceLevel(typeof(ConfigFileWrapper), TraceLevel.All);
        LoggerFactory.SetTraceLevel(typeof(BaseScreenResolutionService), TraceLevel.All);
    }

    private static void DevelopmentConfig() {
        // All enabled, then disabled one by one, so developers can enable just one 
        LoggerFactory.SetDefaultTraceLevel(TraceLevel.All);

        // Bootstrap logs, always log everything :)
        LoggerFactory.SetTraceLevel(typeof(Bootstrap), TraceLevel.All);
        LoggerFactory.SetTraceLevel(typeof(ConfigFileWrapper), TraceLevel.All);

        // DI
        LoggerFactory.SetTraceLevel(typeof(SingletonFactoryProvider), TraceLevel.Error);
        LoggerFactory.SetTraceLevel(typeof(TransientFactoryProvider), TraceLevel.Error);
        LoggerFactory.SetTraceLevel(typeof(Betauer.DI.Container), TraceLevel.Error);
        LoggerFactory.SetTraceLevel(typeof(Betauer.DI.Container.Injector), TraceLevel.Error);

        // GameTools
        LoggerFactory.SetTraceLevel(typeof(BaseScreenResolutionService), TraceLevel.All);
        LoggerFactory.SetTraceLevel(typeof(StateMachine), TraceLevel.Error);

        // Animation
        LoggerFactory.SetTraceLevel(typeof(CallbackNodeTweener), TraceLevel.Error);
        LoggerFactory.SetTraceLevel(typeof(CallbackTweener), TraceLevel.Error);
        LoggerFactory.SetTraceLevel(typeof(PauseTweener), TraceLevel.Error);
        LoggerFactory.SetTraceLevel(typeof(PropertyTweener), TraceLevel.Error);

        // Game
        LoggerFactory.SetTraceLevel("GameManager.StateMachine", TraceLevel.Error);
        LoggerFactory.SetTraceLevel(typeof(MainStateMachine), TraceLevel.Error);
        LoggerFactory.SetTraceLevel(typeof(CameraStageLimiter), TraceLevel.All);

        // Player and enemies
        LoggerFactory.SetTraceLevel(typeof(PlayerNode), TraceLevel.All);
        LoggerFactory.SetTraceLevel("Player.StateMachine", TraceLevel.Error);
        LoggerFactory.SetTraceLevel("Player.AnimationStack", TraceLevel.Error);
        LoggerFactory.SetTraceLevel(typeof(ZombieNode), TraceLevel.All);
        LoggerFactory.SetTraceLevel("Zombie.StateMachine", TraceLevel.Error);
        LoggerFactory.SetTraceLevel("Zombie.AnimationStack", TraceLevel.Error);
            
        // LoggerFactory.OverrideTraceLevel(TraceLevel.All);
    }
}