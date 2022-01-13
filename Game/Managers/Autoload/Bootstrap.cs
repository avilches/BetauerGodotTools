using System;
using System.Diagnostics;
using Godot;
using Betauer;
using Betauer.Animation;
using Betauer.Bus;
using Betauer.Screen;
using Veronenger.Game.Controller.Stage;
using TraceLevel = Betauer.TraceLevel;
using Directory = System.IO.Directory;
using Path = System.IO.Path;

namespace Veronenger.Game.Managers.Autoload {

    public class Bootstrap : DiBootstrap /* needed to be instantiated as an Autoload from Godot */ {
        public static readonly DateTime StartTime = DateTime.Now;
        public static TimeSpan Uptime => DateTime.Now.Subtract(StartTime);

        /*
         * Boostrap is the only Autoload node
         * It inherits from DiBootstrap, so all the singletons are scanned and loaded.
         * As soon as the GameManager is injected into the Bootstrap, the GameManager will initialize the screen
         */
        [Inject] public GameManager GameManager;

        private const UnhandledExceptionPolicy UnhandledExceptionPolicyConfig = UnhandledExceptionPolicy.TerminateApplication;
        private const bool LogToFileEnabled = false; // TODO: enabled by a parameter

        public override void _Ready() {
            Name = nameof(Bootstrap); // This name is shown in the remote editor
            CheckErrorPolicy(UnhandledExceptionPolicyConfig);
            // MicroBenchmarks();
            this.DisableAllNotifications();
        }

        public override DiRepository CreateDiRepository() {
            ConfigureLoggerFactory();
            return new DiRepository();
        }


        private void ConfigureLoggerFactory() {
            LoggerFactory.Start(this);

            if (LogToFileEnabled) {
                var folder = Directory.GetCurrentDirectory();
                var logPath = Path.Combine(folder, $"Veronenger.{DateTime.Now:yyyy-dd-M--HH-mm-ss}.log");
                LoggerFactory.AddFileWriter(logPath);
            }

            DisposeSnitchObject.ShowShutdownWarning = true;

            LoggerFactory.SetConsoleOutput(ConsoleOutput.Standard);
            LoggerFactory.IncludeTimestamp(true);
            LoggerFactory.SetDefaultTraceLevel(TraceLevel.All);

            // Tools
            LoggerFactory.SetTraceLevel(typeof(DiRepository), TraceLevel.All);
            LoggerFactory.SetTraceLevel(typeof(GodotTopic<>), TraceLevel.Off);
            LoggerFactory.SetTraceLevel(typeof(GodotListener<>), TraceLevel.Off);
            LoggerFactory.SetTraceLevel(typeof(AnimationStack), TraceLevel.Off);

            LoggerFactory.SetTraceLevel(typeof(Launcher), TraceLevel.Off);
            LoggerFactory.SetTraceLevel(typeof(MultipleSequencePlayer), TraceLevel.Off);
            LoggerFactory.SetTraceLevel(typeof(SingleSequencePlayer), TraceLevel.Off);
            LoggerFactory.SetTraceLevel(typeof(RepeatablePlayer<>), TraceLevel.Off);
            LoggerFactory.SetTraceLevel(typeof(PropertyTweener<>), TraceLevel.Off);
            LoggerFactory.SetTraceLevel(typeof(AnimationKeyStep<>), TraceLevel.Off);

            // Managers
            LoggerFactory.SetTraceLevel(typeof(ScreenIntegerResolutionService), TraceLevel.Off);
            LoggerFactory.SetTraceLevel(typeof(StageManager), TraceLevel.Off);

            // Player and enemies
            LoggerFactory.SetTraceLevel(typeof(StageCameraController), TraceLevel.Off);
            LoggerFactory.SetTraceLevel("Player:*", TraceLevel.Debug);
            LoggerFactory.SetTraceLevel("Player:*", "StateMachine", TraceLevel.Off);
            LoggerFactory.SetTraceLevel("Player:*", "AnimationStack", TraceLevel.Off);
            LoggerFactory.SetTraceLevel("Player:*", "Motion", TraceLevel.Off);
            LoggerFactory.SetTraceLevel("Player:*", "Collision", TraceLevel.Off);

            LoggerFactory.SetTraceLevel("Player:*", "JumpHelper", TraceLevel.Off);
            LoggerFactory.SetTraceLevel("Player:*", "CoyoteJump", TraceLevel.Off);
            LoggerFactory.SetTraceLevel("Player:*", "JumpVelocity", TraceLevel.Off);
            LoggerFactory.SetTraceLevel("Player:*", "Input", TraceLevel.Off);

            LoggerFactory.SetTraceLevel("Enemy.Zombie:*", "Motion", TraceLevel.Off);
            LoggerFactory.SetTraceLevel("Enemy.Zombie:*", "Collision", TraceLevel.Off);
            LoggerFactory.SetTraceLevel("Enemy.Zombie:*", "StateMachine", TraceLevel.Off);
            LoggerFactory.SetTraceLevel("Enemy.Zombie:*", "Animation", TraceLevel.Off);
            LoggerFactory.SetTraceLevel("Enemy.Zombie:*", TraceLevel.Off);
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