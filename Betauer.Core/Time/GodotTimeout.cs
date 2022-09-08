using System;
using Betauer.Signal;
using Godot;

namespace Betauer.Time {
    public static class GodotTimeoutExtensions {
        public static GodotTimeout OnTimeout(this SceneTree sceneTree, float timeout, Action action,
            Node.PauseModeEnum pauseMode = Node.PauseModeEnum.Inherit) {
            return new GodotTimeout(sceneTree, timeout, action, pauseMode);
        }
    }
    
    
    /// <summary>
    /// Executes an action after the timeout in seconds.
    ///  
    /// Wrapper object for a SceneTreeTimeout created with SceneTree.CreateTimer() with these differences:
    /// 
    /// - It has Start/Stop and Reset.
    /// - When created, it doesn't start automatically (so you need to call to Start())
    /// - It can be used more than once, changing the timeout and the action to execute.
    /// 
    /// </summary>
    public class GodotTimeout {
        private readonly SceneTree _sceneTree;
        private SceneTreeTimer? _sceneTreeTimer;
        private bool _paused = true;
        private bool _running = false;
        private float _timeLeft = 0;
        private Action _action;

        public float Timeout { get; private set; } = 0;
        public readonly Node.PauseModeEnum PauseMode;
        public bool IsRunning => _running && !_paused;

        public GodotTimeout(SceneTree sceneTree, float timeout, Action action, Node.PauseModeEnum pauseMode = Node.PauseModeEnum.Inherit) {
            _sceneTree = sceneTree;
            PauseMode = pauseMode;
            Timeout = _timeLeft = timeout;
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
            var sceneTreeTimer =_sceneTree.CreateTimer(timeout, pauseModeProcess);
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