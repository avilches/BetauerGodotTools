using System;
using System.Threading.Tasks;
using Betauer.Core.Signal;
using Godot;

namespace Betauer.Core.Time {
    /// <summary>
    /// Execute a lambda periodically. It uses internally the "timeout" signal from a SceneTree timer,
    /// so it's affected by the Engine.Timescale and the SceneTree.Pause state.
    ///
    /// Usage:
    /// <code>
    /// var gs = new GodotScheduler(() => {});
    /// gs.Start(10f);
    /// 
    /// </code> 
    /// </summary>
    public class GodotScheduler {
        private Action _action;
        private readonly GodotTimeout _godotTimeout;

        public GodotScheduler(SceneTree sceneTree, double seconds, Action action, bool processAlways = true, bool processInPhysics = false, bool ignoreTimeScale = false) {
            _action = action;
            _godotTimeout = sceneTree.OnTimeout(seconds, () => _action(), processAlways, processInPhysics, ignoreTimeScale);
            _godotTimeout.Start().Stop();
            _Start();
        }

        private async void _Start() {
            while (true) {
                await _godotTimeout.Await();
                _godotTimeout.Restart();
            }
        }

        public GodotScheduler Execute(Action action) {
            _action = action;
            return this;
        }

        public GodotScheduler Start(double seconds) {
            _godotTimeout.SetTimeout(seconds);
            _godotTimeout.Start();
            return this;
        }

        public GodotScheduler Start() {
            _godotTimeout.Start();
            return this;
        }

        public GodotScheduler Stop() {
            _godotTimeout.Stop();
            return this;
        }

        public GodotScheduler Reset() {
            _godotTimeout.Reset();
            return this;
        }

        public bool IsRunning() => _godotTimeout.IsRunning;
    }
}