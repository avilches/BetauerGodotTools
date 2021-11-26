using System;
using System.Diagnostics;
using Godot;
using Tools;
using Tools.Bus;
using Veronenger.Game.Controller.Stage;
using Veronenger.Game.Tools.Resolution;
using TraceLevel = Tools.TraceLevel;
using Directory = System.IO.Directory;
using Path = System.IO.Path;

namespace Veronenger.Game.Managers.Autoload {
    public class Bootstrap : DiBootstrap /* needed to be instantiated as an Autoload from Godot */ {

        [Inject]
        public GameManager GameManager;

        public override void _Ready() {
            // MicroBenchmarks();
            GameManager.IsRunningTests = GetTree().CurrentScene.Filename == "res://Tests/Runner/RunTests.tscn";
        }

        private bool _logToFileEnabled = false;

        public override void ConfigureLoggerFactory() {
            if (_logToFileEnabled) {
                var folder = Directory.GetCurrentDirectory();
                var logPath = Path.Combine(folder, $"Veronenger.{DateTime.Now:yyyy-dd-M--HH-mm-ss}.log");
                LoggerFactory.AddFileWriter(logPath);
            }

            LoggerFactory.AddGodotPrinter();

            LoggerFactory.Start(this);
            LoggerFactory.SetDefaultTraceLevel(TraceLevel.Debug);
            // Tools
            LoggerFactory.SetTraceLevel(typeof(DiRepository), TraceLevel.Debug);
            LoggerFactory.SetTraceLevel(typeof(GodotTopic<>), TraceLevel.Off);
            LoggerFactory.SetTraceLevel(typeof(GodotListener<>), TraceLevel.Off);
            LoggerFactory.SetTraceLevel(typeof(AnimationStack), TraceLevel.Off);
            // Managers
            LoggerFactory.SetTraceLevel(typeof(ScreenManager), TraceLevel.Off);
            LoggerFactory.SetTraceLevel(typeof(StageManager), TraceLevel.Off);

            // Player and enemies
            LoggerFactory.SetTraceLevel(typeof(StageCameraController), TraceLevel.Off);
            LoggerFactory.SetTraceLevel("Player:*", TraceLevel.Off);
            LoggerFactory.SetTraceLevel("Player:*", "StateMachine", TraceLevel.Off);
            LoggerFactory.SetTraceLevel("Player:*", "Animation", TraceLevel.Off);
            LoggerFactory.SetTraceLevel("Player:*", "Motion", TraceLevel.Off);
            LoggerFactory.SetTraceLevel("Player:*", "Collision", TraceLevel.Off);
            
            LoggerFactory.SetTraceLevel("Player:*", "JumpHelper", TraceLevel.Off);
            LoggerFactory.SetTraceLevel("Player:*", "CoyoteJump", TraceLevel.Off);
            LoggerFactory.SetTraceLevel("Player:*", "JumpVelocity", TraceLevel.Off);
            LoggerFactory.SetTraceLevel("Player:*", "Input", TraceLevel.Off);

            LoggerFactory.SetTraceLevel("Enemy.Zombie:*", "Motion", TraceLevel.Off);
            LoggerFactory.SetTraceLevel("Enemy.Zombie:*", "Collision", TraceLevel.Off);
            LoggerFactory.SetTraceLevel("Enemy.Zombie:*", "StateMachine", TraceLevel.Debug);
            LoggerFactory.SetTraceLevel("Enemy.Zombie:*", "Animation", TraceLevel.Debug);
            LoggerFactory.SetTraceLevel("Enemy.Zombie:*", TraceLevel.Debug);
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