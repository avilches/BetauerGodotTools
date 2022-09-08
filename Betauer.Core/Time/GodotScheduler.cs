using System;
using Betauer.Signal;
using Godot;

namespace Betauer.Time {
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
        private readonly Action _action;
        public readonly Node.PauseModeEnum PauseMode;
        private bool _paused = false;
        private bool _requestStop = false;
        private bool _running = false;

        public GodotScheduler(Action action, Node.PauseModeEnum pauseMode = Node.PauseModeEnum.Inherit) {
            _action = action;
            PauseMode = pauseMode;
        }

        public GodotScheduler Start(SceneTree sceneTree, float seconds) {
            lock (this) {
                if (_running) return this;
                _running = true;
            }
            _Start(sceneTree, seconds);
            return this;
        }

        private async void _Start(SceneTree sceneTree, float seconds) {
            _paused = false;
            while (true) {
                var pauseModeProcess = PauseMode == Node.PauseModeEnum.Process; // false = pausing the scene pause the timer 
                await sceneTree.CreateTimer(seconds, pauseModeProcess).AwaitTimeout();
                if (_requestStop) {
                    _requestStop = false;
                    break;
                }
                if (!_paused) _action();
            }
            lock (this) {
                _running = false;
            }
        }

        public bool IsRunning() => _running;
        public bool IsPaused() =>  _paused;

        public void Pause() => _paused = true;
        public void Resume() => _paused = false;
        
        public void Stop() => _requestStop = true;
    }
}