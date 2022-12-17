using System;
using System.Threading.Tasks;
using Betauer.Core.Signal;
using Godot;

namespace Betauer.Core.Time {
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
        private double _timeLeft = 0;
        private event Action _OnTimeout;

        public double Timeout { get; private set; } = 0;
        public bool ProcessAlways = true;
        public bool ProcessInPhysics = false;
        public bool IgnoreTimeScale = false;

        public bool IsRunning => _running && !_paused;

        public GodotTimeout(SceneTree sceneTree, double timeout, Action onTimeout, bool processAlways = false,
            bool processInPhysics = false, bool ignoreTimeScale = false) {
            _sceneTree = sceneTree;
            Timeout = _timeLeft = timeout;
            _OnTimeout += onTimeout;
            ProcessAlways = processAlways;
            ProcessInPhysics = processInPhysics;
            IgnoreTimeScale = ignoreTimeScale;
        }

        public GodotTimeout(double timeout, Action onTimeout, bool processAlways = false, bool processInPhysics = false, bool ignoreTimeScale = false) : 
            this(Engine.GetMainLoop() as SceneTree, timeout, onTimeout, processAlways, processInPhysics, ignoreTimeScale) {
        }

        public GodotTimeout AddOnTimeout(Action action) {
            _OnTimeout += action;
            return this;
        }

        public GodotTimeout RemoveOnTimeout(Action action) {
            _OnTimeout -= action;
            return this;
        }
        
        public GodotTimeout SetTimeout(double timeout) {
            if (timeout < Elapsed) {
                End();
            } else {
                if (_running && !_paused) {
                    _sceneTreeTimer!.TimeLeft = timeout - Elapsed;
                } else {
                    _timeLeft = timeout - Elapsed;
                }
            }
            Timeout = timeout;
            return this;
        }

        public double TimeLeft {
            get {
                if (_running && !_paused) return _sceneTreeTimer!.TimeLeft;
                return _timeLeft;
            }
        }
        public double Elapsed => Timeout - TimeLeft;

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
                _sceneTreeTimer!.TimeLeft = Timeout;
            }
            return this;
        }

        public GodotTimeout Restart() {
            if (_running && !_paused) {
                _sceneTreeTimer!.TimeLeft = Timeout;
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
                _timeLeft = _sceneTreeTimer!.TimeLeft;
                _sceneTreeTimer = null;
                _paused = true;
            }
            return this;
        }

        private SceneTreeTimer CreateTimer(double timeout) {
            var sceneTreeTimer = _sceneTree.CreateTimer(timeout, ProcessAlways, ProcessInPhysics, IgnoreTimeScale);
            var sceneTreeTimerId = sceneTreeTimer.GetHashCode();
            sceneTreeTimer.AwaitTimeout().OnCompleted(() => {
                if (_sceneTreeTimer != null && _sceneTreeTimer.GetHashCode() == sceneTreeTimerId) {
                    End();
                }
            });
            return sceneTreeTimer;
        }

        private void End() {
            _timeLeft = 0;
            _sceneTreeTimer = null;
            _running = false;
            _paused = true;
            _OnTimeout?.Invoke();
        }

        public Task Await() {
            TaskCompletionSource promise = new();
            void ToAdd() {
                RemoveOnTimeout(ToAdd);
                promise.TrySetResult();
            }
            AddOnTimeout(ToAdd);
            return promise.Task;
        }

        public override string ToString() {
            return $"{(IsRunning ? "Running: " : "Stopped: ")}{Timeout}/{TimeLeft:0.0} (internal TimeLeft: {_sceneTreeTimer?.TimeLeft:0.0})";
        }
    }
}