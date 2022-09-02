using System;
using Betauer.Signal;

namespace Betauer {
    public class GodotScheduler {
        private readonly Action _action;
        private readonly bool _pauseIfSceneTreeIsPaused = false;
        private bool _paused = false;
        private bool _requestStop = false;
        private bool _running = false;

        /// <summary>
        /// Execute a lambda periodically. It uses the "timeout" signal from SceneTree.CreateTimer()  
        /// </summary>
        /// <param name="action">lambda to execute</param>
        /// <param name="pauseIfSceneTreeIsPaused">
        /// true: the timer will pause along with the SceneTree
        /// false: ignore the pause, and the timer run always
        /// </param>
        public GodotScheduler(Action action, bool pauseIfSceneTreeIsPaused) {
            _action = action;
            _pauseIfSceneTreeIsPaused = pauseIfSceneTreeIsPaused;
        }

        public GodotScheduler Start(float seconds) {
            lock (this) {
                if (_running) return this;
                _running = true;
            }
            _Start(seconds);
            return this;
        }

        private async void _Start(float seconds) {
            _paused = false;
            while (true) {
                await SceneTreeHolder.SceneTree.CreateTimer(seconds, _pauseIfSceneTreeIsPaused).AwaitTimeout();
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