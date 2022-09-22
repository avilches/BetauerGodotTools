using System;
using System.Linq;
using Godot;
using Betauer;
using Betauer.Animation;
using Betauer.Animation.Tween;
using Betauer.Application;
using Betauer.Application.Screen;
using Betauer.DI;
using Betauer.DI.ServiceProvider;
using Betauer.Memory;
using Betauer.Pool;
using Betauer.StateMachine;
using Veronenger.Game.Controller.Character;
using Veronenger.Game.Controller.Stage;
using Container = Betauer.DI.Container;
using PropertyTweener = Betauer.Animation.Tween.PropertyTweener;
using TraceLevel = Betauer.TraceLevel;

namespace Veronenger.Game.Managers.Autoload {
    public class Bootstrap : AutoConfiguration /* needed to be instantiated as an Autoload from Godot */ {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(Bootstrap));
        public static readonly DateTime StartTime = DateTime.Now;
        public static TimeSpan Uptime => DateTime.Now.Subtract(StartTime);
        
        public Bootstrap() {
            EnableAddSingletonNodesToTree(true);
            SetObjectWatcherTimer(1f);
            
            #if DEBUG
                DevelopmentConfig();
            #else
                ExportConfig();
            #endif
            ShowConfig();
            Logger.Info($"Bootstrap time: {Uptime.TotalMilliseconds} ms");
        }

        private static void ShowConfig() {
            Logger.Info($"cmd line args: {string.Join(" ", OS.GetCmdlineArgs())}");
            Logger.Info($"app version  : {AppInfo.Version}");
            Logger.Info($"features     : {string.Join(", ", FeatureFlags.GetActiveList())}");
            Logger.Info($"name host    : {OS.GetName()}");
            Logger.Info($"data dir     : {OS.GetDataDir()}");
            Logger.Info($"user data dir: {OS.GetUserDataDir()}");
            Logger.Info($"config dir   : {OS.GetConfigDir()}");
            Logger.Info($"cache dir    : {OS.GetCacheDir()}");
            new[] {
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
                    "mono/unhandled_exception_policy",
                    "mono/unhandled_exception_policy.standalone",
                    "application/config/version"
                }.ToList()
                .Where(ProjectSettings.HasSetting)
                .ForEach(property => Logger.Info($"- {property}: {ProjectSettings.GetSetting(property)}"));
        }

        public override void _Ready() {
            Name = nameof(Bootstrap); // This name is shown in the remote editor
            MainLoopNotificationsHandler.OnWmQuitRequest += () => {
                var timespan = Uptime;
                var elapsed = $"{(int)timespan.TotalMinutes} min {timespan.Seconds:00} sec";
                Logger.Info("User requested exit the application. Uptime: " + elapsed);
            };
        }

        private static void ExportConfig() {
            LoggerFactory.SetConsoleOutput(ConsoleOutput.GodotPrint); // GD.Print means it appears in the user data logs
            LoggerFactory.SetDefaultTraceLevel(TraceLevel.Warning);
            LoggerFactory.SetTraceLevel(typeof(Bootstrap), TraceLevel.All);
            LoggerFactory.SetTraceLevel(typeof(ConfigFileWrapper), TraceLevel.All);
            LoggerFactory.SetTraceLevel(typeof(GameManager), TraceLevel.All);
        }

        private static void DevelopmentConfig() {
            DisposeTools.ShowWarningOnShutdownDispose = true;
            DisposeTools.ShowMessageOnNewInstance = false;
            DisposeTools.ShowMessageOnDispose = true;
            LoggerFactory.SetConsoleOutput(ConsoleOutput.ConsoleWriteLine); // No GD.Print means no logs

            // All enabled, then disabled one by one, so developers can enable just one 
            LoggerFactory.SetDefaultTraceLevel(TraceLevel.Error);

            // Bootstrap logs, all always :)
            LoggerFactory.SetTraceLevel(typeof(Bootstrap), TraceLevel.All);
            LoggerFactory.SetTraceLevel(typeof(Consumer), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(ObjectPool), TraceLevel.All);

            // DI
            LoggerFactory.SetTraceLevel(typeof(SingletonFactoryProvider), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(TransientFactoryProvider), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(Container), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(Injector), TraceLevel.Error);

            // GameTools
            LoggerFactory.SetTraceLevel(typeof(AnimationStack), TraceLevel.Error);

            // Animation
            LoggerFactory.SetTraceLevel(typeof(CallbackNodeTweener), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(CallbackTweener), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(MethodCallbackTweener), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(PauseTweener), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(PropertyTweener), TraceLevel.Error);

            // Screen
            LoggerFactory.SetTraceLevel(typeof(ViewportResolutionStrategy), TraceLevel.All);
            LoggerFactory.SetTraceLevel(typeof(IntegerScaledScreenResolutionStrategy), TraceLevel.All);
            LoggerFactory.SetTraceLevel(typeof(WindowSizeResolutionStrategy), TraceLevel.All);

            // Managers
            LoggerFactory.SetTraceLevel(typeof(GameManager), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(StageManager), TraceLevel.Error);

            LoggerFactory.SetTraceLevel(typeof(StateMachine), "GameManager", TraceLevel.Error);

            // Player and enemies
            LoggerFactory.SetTraceLevel(typeof(StageCameraController), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(PlayerController), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(StateMachine), "Player:*", TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(AnimationStack), "Player:*", TraceLevel.Error);
            LoggerFactory.SetTraceLevel("Motion", "Player:*", TraceLevel.Error);
            LoggerFactory.SetTraceLevel("Collision", "Player:*", TraceLevel.Error);
            LoggerFactory.SetTraceLevel("JumpVelocity", "Player:*", TraceLevel.Error);

            LoggerFactory.SetTraceLevel(typeof(StateMachine), "Enemy.Zombie:*", TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(AnimationStack), "Enemy.Zombie:*", TraceLevel.Error);
            LoggerFactory.SetTraceLevel("Motion", "Enemy.Zombie:*", TraceLevel.Error);
            LoggerFactory.SetTraceLevel("Collision", "Enemy.Zombie:*", TraceLevel.Error);
        }
    }
}