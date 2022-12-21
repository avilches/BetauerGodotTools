using Betauer.Animation;
using Betauer.Application;
using Betauer.Application.Monitor;
using Betauer.Application.Screen;
using Betauer.Camera;
using Betauer.DI;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.Logging;
using Betauer.Core.Pool;
using Betauer.OnReady;
using Betauer.StateMachine;
using Godot;
using Veronenger.Controller.Character;
using Container = Betauer.DI.Container;
using PropertyTweener = Betauer.Animation.PropertyTweener;

namespace Veronenger.Managers.Autoload {
    public partial class Bootstrap : Node /* needed to be instantiated as an Autoload from Godot */ {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(Bootstrap));

        [Inject] private NotificationsHandler NotificationsHandler { get; set; }
        [Inject] private DebugOverlayManager DebugOverlayManager { get; set; }

        public Bootstrap() {
            AppTools.ConfigureExceptionHandlers(GetTree);

            new AutoConfiguration(this)
                .Start(options => {
                    options
                        .ScanConfiguration(new DefaultConfiguration(this))
                        .Scan<MouseActions>();
                });

#if DEBUG
            DevelopmentConfig();
#else
            ExportConfig();
#endif
            
            GD.Print(string.Join("\n", Project.GetOSInfo()));
            GD.Print(string.Join("\n", Project.GetSettings(
                "logging/file_logging/enable_file_logging",
                "logging/file_logging/enable_file_logging.pc",
                "logging/file_logging/log_path",
                "logging/file_logging/log_path.standalone",
                "application/run/disable_stdout",
                "application/run/disable_stderr",
                "application/run/flush_stdout_on_print",
                "application/run/flush_stdout_on_print.debug",
                "application/config/use_custom_user_dir",
                "application/config/project_settings_override",
                "mono/runtime/unhandled_exception_policy",
                "mono/runtime/unhandled_exception_policy.standalone",
                "application/config/version"
            )));
        }

        public override void _Ready() {
            Name = nameof(Bootstrap); // This name is shown in the remote editor
            Logger.Info($"Bootstrap time: {Project.Uptime.TotalMilliseconds} ms");
            NotificationsHandler.OnWmCloseRequest += () => {
                GD.Print($"[WmQuitRequest] Uptime: {Project.Uptime.TotalMinutes:0} min {Project.Uptime.Seconds:00} sec");
            };
            DebugOverlayManager.DebugConsole.AddAllCommands();
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
            LoggerFactory.SetTraceLevel(typeof(SingletonFactoryProvider), TraceLevel.All);
            LoggerFactory.SetTraceLevel(typeof(TransientFactoryProvider), TraceLevel.All);
            LoggerFactory.SetTraceLevel(typeof(Container), TraceLevel.All);
            LoggerFactory.SetTraceLevel(typeof(Injector), TraceLevel.Error);

            // GameTools
            LoggerFactory.SetTraceLevel(typeof(BaseScreenResolutionService), TraceLevel.All);
            LoggerFactory.SetTraceLevel(typeof(ObjectPool), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(StateMachine), TraceLevel.Error);

            // Animation
            LoggerFactory.SetTraceLevel(typeof(AnimationStack), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(CallbackNodeTweener), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(CallbackTweener), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(PauseTweener), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(PropertyTweener), TraceLevel.Error);

            // Game
            LoggerFactory.SetTraceLevel("GameManager.StateMachine", TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(MainStateMachine), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(CameraStageLimiter), TraceLevel.All);

            // Player and enemies
            LoggerFactory.SetTraceLevel(typeof(PlayerNode), TraceLevel.Error);
            LoggerFactory.SetTraceLevel("Player.StateMachine", TraceLevel.Error);
            LoggerFactory.SetTraceLevel("Player.AnimationStack", TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(ZombieNode), TraceLevel.Error);
            LoggerFactory.SetTraceLevel("Zombie.StateMachine", TraceLevel.Error);
            LoggerFactory.SetTraceLevel("Zombie.AnimationStack", TraceLevel.Error);
            
            // LoggerFactory.OverrideTraceLevel(TraceLevel.All);
        }
    }
}