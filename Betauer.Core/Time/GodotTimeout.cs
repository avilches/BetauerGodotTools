using System;
using Betauer.Signal;
using Godot;

namespace Betauer.Time {
    /// <summary>
    /// A Timeout with Start/Stop and Reset.
    /// It uses internal SceneTreeTimer, so it's affected by the Engine.Timescale and the SceneTree.Pause (things that
    /// C# System.Diagnostics.Stopwatch can not do)
    ///
    /// Executes the action after the timeout 
    /// </summary>
    public class GodotTimeout {
        private SceneTreeTimer? _sceneTreeTimer;
        private bool _paused = true;
        private bool _running = false;
        private float _timeLeft = 0;
        public float Timeout { get; private set; } = 0;
        private Action _action;

        public readonly Node.PauseModeEnum PauseMode;
        public bool IsRunning => _running && !_paused;

        public GodotTimeout(float timeout, Action action, Node.PauseModeEnum pauseMode = Node.PauseModeEnum.Inherit) {
            PauseMode = pauseMode;
            Timeout = timeout;
            _timeLeft = timeout;
            _action = action;
        }

        public GodotTimeout OnTimeout(Action action) {
            _action = action;
            return this;
        }

        public GodotTimeout SetTimeout(float timeout) {
            Timeout = timeout;
            Reset();
            return this;
        }

        public float TimeLeft {
            get {
                if (_running && !_paused) return _sceneTreeTimer.TimeLeft;
                return _timeLeft;
            }
        }

        /// <summary>
        /// Starts, or resumes, the timer. Elapsed field will start to increase in every frame.
        /// </summary>
        /// <returns></returns>
        public GodotTimeout Start() {
            if (!_running) {
                // not running, create a new one
                _sceneTreeTimer = CreateTimer(Timeout);
            } else if (_paused) {
                // running and paused: resume
                _sceneTreeTimer = CreateTimer(_timeLeft);
            } // else running and not paused, nothing 
            _running = true;
            _paused = false;
            return this;
        }

        /// <summary>
        /// Set the Elapsed time to 0, keeping the state.
        /// </summary>
        /// <returns></returns>
        public GodotTimeout Reset() {
            if (!_running || _paused) {
                _timeLeft = Timeout;
            } else { 
                // running and not paused 
                _sceneTreeTimer.TimeLeft = Timeout;
            }
            return this;
        }

        public GodotTimeout Restart() {
            if (_running && !_paused) {
                _sceneTreeTimer.TimeLeft = Timeout;
            } else {
                // running and paused, or not running
                _sceneTreeTimer = CreateTimer(Timeout);
            }
            _paused = false;
            _running = true;
            return this;
        }

        /// <summary>
        /// Stops the timer. It can be resumed with Start(). The Elapsed field will stop to increase.
        /// </summary>
        /// <returns></returns>
        public GodotTimeout Stop() {
            if (_running && !_paused) {
                _timeLeft = _sceneTreeTimer.TimeLeft;
                _sceneTreeTimer = null;
                _paused = true;
            }
            return this;
        }

        private SceneTreeTimer CreateTimer(float timeout) {
            var pauseModeProcess = PauseMode == Node.PauseModeEnum.Process; // false = pausing the scene pause the timer 
            var sceneTreeTimer =
                SceneTreeHolder.SceneTree.CreateTimer(timeout, pauseModeProcess);
            var sceneTreeTimerId = sceneTreeTimer.GetHashCode();
            sceneTreeTimer.AwaitTimeout().OnCompleted(() => {
                if (_sceneTreeTimer != null && _sceneTreeTimer.GetHashCode() == sceneTreeTimerId) {
                    _action();
                    _timeLeft = 0;
                    _sceneTreeTimer = null;
                    _running = false;
                    _paused = true;
                }
            });
            return sceneTreeTimer;
        }

        public override string ToString() {
            return $"{(IsRunning ? "Running: " : "Stopped: ")}{Timeout}/{TimeLeft:0.0} (internal TimeLeft: {_sceneTreeTimer?.TimeLeft:0.0})";
        }
    }
}