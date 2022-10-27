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
        
        public Bootstrap() {
            EnableAddSingletonNodesToTree(true);
            SetObjectWatcherTimer(1f);
            
            #if DEBUG
                DevelopmentConfig();
            #else
                ExportConfig();
            #endif
            ShowConfig();
        }

        private static void ShowConfig() {
            Logger.Info($"executable    : {OS.GetExecutablePath()}");
            Logger.Info($"cmd line args : {string.Join(" ", OS.GetCmdlineArgs())}");
            Logger.Info($"app version   : {AppInfo.Version}");
            Logger.Info($"features      : {string.Join(", ", FeatureFlags.GetActiveList())}");
            Logger.Info($"name host     : {OS.GetName()}");
            Logger.Info($"data dir      : {OS.GetDataDir()}");
            Logger.Info($"user data dir : {OS.GetUserDataDir()}");
            Logger.Info($"config dir    : {OS.GetConfigDir()}");
            Logger.Info($"cache dir     : {OS.GetCacheDir()}");
            Logger.Info($"permissions   : {string.Join(", ", OS.GetGrantedPermissions())}");
            Logger.Info($"video name    : {VisualServer.GetVideoAdapterName()}");
            Logger.Info($"video vendor  : {VisualServer.GetVideoAdapterVendor()}");
            Logger.Info($"processor name: {OS.GetProcessorName()}");
            Logger.Info($"processors    : {OS.GetProcessorCount().ToString()}");
            Logger.Info($"locale        : {OS.GetLocale()}/{OS.GetLocaleLanguage()}");
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
                    "mono/runtime/unhandled_exception_policy",
                    "mono/runtime/unhandled_exception_policy.standalone",
                    "application/config/version"
                }.ToList()
                .ForEach(property => {
                    if (ProjectSettings.HasSetting(property)) {
                        Logger.Info($"- {property} = {ProjectSettings.GetSetting(property)}");
                    } else {
                        Logger.Info($"! {property} !");
                    }
                });
        }

        public override void _Ready() {
            Logger.Info($"Bootstrap time: {FeatureFlags.Uptime.TotalMilliseconds} ms");
            Name = nameof(Bootstrap); // This name is shown in the remote editor
            MainLoopNotificationsHandler.OnWmQuitRequest += () => {
                var elapsed = $"{(int)FeatureFlags.Uptime.TotalMinutes} min {FeatureFlags.Uptime.Seconds:00} sec";
                Logger.Info("User requested exit the application. Uptime: " + elapsed);
            };
        }

        private static void ExportConfig() {
            LoggerFactory.SetConsoleOutput(ConsoleOutput.GodotPrint); // GD.Print means it appears in the user data logs
            LoggerFactory.SetDefaultTraceLevel(TraceLevel.Warning);

            // Bootstrap logs, always log everything :)
            LoggerFactory.SetTraceLevel(typeof(Bootstrap), TraceLevel.All);
            LoggerFactory.SetTraceLevel(typeof(ConfigFileWrapper), TraceLevel.All);
            LoggerFactory.SetTraceLevel(typeof(BaseScreenResolutionService), TraceLevel.All);
            LoggerFactory.SetTraceLevel("GameManager.StateMachine", TraceLevel.All);
        }

        private static void DevelopmentConfig() {
            DisposeTools.ShowWarningOnShutdownDispose = true;
            DisposeTools.ShowMessageOnNewInstance = false;
            DisposeTools.ShowMessageOnDispose = true;
            LoggerFactory.SetConsoleOutput(ConsoleOutput.ConsoleWriteLine); // No GD.Print means no logs

            // All enabled, then disabled one by one, so developers can enable just one 
            LoggerFactory.SetDefaultTraceLevel(TraceLevel.All);

            // Bootstrap logs, always log everything :)
            LoggerFactory.SetTraceLevel(typeof(Bootstrap), TraceLevel.All);
            LoggerFactory.SetTraceLevel(typeof(ConfigFileWrapper), TraceLevel.All);

            // DI
            LoggerFactory.SetTraceLevel(typeof(SingletonFactoryProvider), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(TransientFactoryProvider), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(Container), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(Injector), TraceLevel.Error);

            // GameTools
            LoggerFactory.SetTraceLevel(typeof(BaseScreenResolutionService), TraceLevel.All);
            LoggerFactory.SetTraceLevel(typeof(Consumer), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(ObjectPool), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(StateMachine), TraceLevel.Error);

            // Animation
            LoggerFactory.SetTraceLevel(typeof(AnimationStack), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(CallbackNodeTweener), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(CallbackTweener), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(MethodCallbackTweener), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(PauseTweener), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(PropertyTweener), TraceLevel.Error);

            // Game
            LoggerFactory.SetTraceLevel("GameManager.StateMachine", TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(MainStateMachine), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(StageManager), TraceLevel.Error);

            // Player and enemies
            LoggerFactory.SetTraceLevel(typeof(StageCameraController), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(PlayerController), TraceLevel.Error);
            LoggerFactory.SetTraceLevel("Player.StateMachine", TraceLevel.Error);
            LoggerFactory.SetTraceLevel("Player.AnimationStack", TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(ZombieController), TraceLevel.Error);
            LoggerFactory.SetTraceLevel("Zombie.StateMachine", TraceLevel.Error);
            LoggerFactory.SetTraceLevel("Zombie.AnimationStack", TraceLevel.Error);
            
            // LoggerFactory.OverrideTraceLevel(TraceLevel.All);
        }
    }
}