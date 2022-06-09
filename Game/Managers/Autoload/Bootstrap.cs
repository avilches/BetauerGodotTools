using System;
using System.Diagnostics;
using Godot;
using Betauer;
using Betauer.Animation;
using Betauer.Application;
using Betauer.Bus;
using Betauer.DI;

using Betauer.Memory;
using Betauer.Screen;
using Veronenger.Game.Controller.Stage;
using Container = Betauer.DI.Container;
using TraceLevel = Betauer.TraceLevel;
using Directory = System.IO.Directory;
using Path = System.IO.Path;

namespace Veronenger.Game.Managers.Autoload {
    public class Bootstrap : GodotContainer /* needed to be instantiated as an Autoload from Godot */ {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(Bootstrap));
        public static readonly DateTime StartTime = DateTime.Now;
        public static TimeSpan Uptime => DateTime.Now.Subtract(StartTime);

        private const UnhandledExceptionPolicy UnhandledExceptionPolicyConfig =
            UnhandledExceptionPolicy.TerminateApplication;

        private const bool LogToFileEnabled = false; // TODO: enabled by a command line parameter

        public Bootstrap() {
            ConfigureLoggerFactory();
            CreateAndScan();
            Logger.Info($"Container time: {Uptime.TotalMilliseconds} ms");
        }

        public override void _Ready() {
            Name = nameof(Bootstrap); // This name is shown in the remote editor
            DevTools.CheckErrorPolicy(GetTree(), UnhandledExceptionPolicyConfig);
            // MicroBenchmarks();
            this.DisableAllNotifications();
        }

        private void ConfigureLoggerFactory() {
            LoggerFactory.Start(this);

            if (LogToFileEnabled) {
                var folder = Directory.GetCurrentDirectory();
                var logPath = Path.Combine(folder, $"Veronenger.{DateTime.Now:yyyy-dd-M--HH-mm-ss}.log");
                LoggerFactory.AddFileWriter(logPath);
            }

            DisposeTools.ShowShutdownWarning = true;
            DisposeTools.ShowMessageOnCreate = false;

            LoggerFactory.SetConsoleOutput(ConsoleOutput.Standard);
            LoggerFactory.IncludeTimestamp(true);
            LoggerFactory.SetDefaultTraceLevel(TraceLevel.Error);

            LoggerFactory.SetTraceLevel(typeof(GameManager), TraceLevel.All);

            // Tools
            LoggerFactory.SetTraceLevel(typeof(ContainerBuilder), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(Container), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(GodotTopic<>), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(GodotListener<>), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(AnimationStack), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(ObjectPool), TraceLevel.Error);

            LoggerFactory.SetTraceLevel(typeof(Launcher), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(MultipleSequencePlayer), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(SingleSequencePlayer), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(RepeatablePlayer<>), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(PropertyTweener<>), TraceLevel.Error);
            

            // Screen
            LoggerFactory.SetTraceLevel(typeof(FitToScreenResolutionService), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(PixelPerfectScreenResolutionService), TraceLevel.Error);

            // Managers
            LoggerFactory.SetTraceLevel(typeof(StageManager), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(InputManager), TraceLevel.Debug);

            // Player and enemies
            LoggerFactory.SetTraceLevel(typeof(StageCameraController), TraceLevel.Error);
            LoggerFactory.SetTraceLevel("Player:*", TraceLevel.Error);
            LoggerFactory.SetTraceLevel("Player:*", "StateMachine", TraceLevel.Error);
            LoggerFactory.SetTraceLevel("Player:*", "AnimationStack", TraceLevel.Error);
            LoggerFactory.SetTraceLevel("Player:*", "Motion", TraceLevel.Error);
            LoggerFactory.SetTraceLevel("Player:*", "Collision", TraceLevel.Error);

            LoggerFactory.SetTraceLevel("Player:*", "JumpHelper", TraceLevel.Error);
            LoggerFactory.SetTraceLevel("Player:*", "CoyoteJump", TraceLevel.Error);
            LoggerFactory.SetTraceLevel("Player:*", "JumpVelocity", TraceLevel.Error);
            LoggerFactory.SetTraceLevel("Player:*", "Input", TraceLevel.Error);

            LoggerFactory.SetTraceLevel("Enemy.Zombie:*", "Motion", TraceLevel.Error);
            LoggerFactory.SetTraceLevel("Enemy.Zombie:*", "Collision", TraceLevel.Error);
            LoggerFactory.SetTraceLevel("Enemy.Zombie:*", "StateMachine", TraceLevel.Error);
            LoggerFactory.SetTraceLevel("Enemy.Zombie:*", "Animation", TraceLevel.Error);
            LoggerFactory.SetTraceLevel("Enemy.Zombie:*", TraceLevel.Error);
        }

        /**
         * Detect quit app (by ALT+F4, Command+Q or user menu)
         */
        public override void _Notification(int what) {
            if (what == MainLoop.NotificationWmQuitRequest) {
                Exit();
            }
        }

        private void Exit() {
            var timespan = Uptime;
            var elapsed = $"{(int)timespan.TotalMinutes} min {timespan.Seconds:00} sec";
            Logger.Info("User requested exit the application. Uptime: " + elapsed);
            LoggerFactory.Dispose(); // Please, do this the last so previous disposing operation can log
        }

        private void MicroBenchmarks() {
            var x = Stopwatch.StartNew();
            LoggerFactory.GetLogger("x");
            int calls = 100000;
            for (int i = 0; i < calls; i++) {
                LoggerFactory.GetLogger("x").Info("aaa " + i);
            }
            x.Stop();
            GD.Print(
                $"{calls} calls in {x.Elapsed.ToString()}: {x.ElapsedMilliseconds / (float)calls} calls/ms. {(x.ElapsedMilliseconds * 1000) / (float)calls} calls/s.");
            // Quit();
        }
    }
}