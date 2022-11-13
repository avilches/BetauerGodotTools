using Betauer.Core.Signal;
using Godot;

namespace Betauer.Core.Time {
    /// <summary>
    /// A Stopwatch to measure elapsed time since start. It has Start/Stop and Reset.
    /// 
    /// It uses internal SceneTreeTimer with SceneTree.CreateTimer(), so it's affected by the Engine.Timescale and the
    /// SceneTree.Pause state (these things C# System.Diagnostics.Stopwatch can not do)
    /// </summary>
    public class GodotStopwatch {
        // 3600 is one hour. With bigger numbers, the Timer doesn't work. If the 
        private const float InternalStartTime = 3600f; 

        private readonly SceneTree _sceneTree;
        private SceneTreeTimer _sceneTreeTimer;
        private double _elapsed = 0;
        private double _accumulated = 0;
        private double _timeLeftOnPause = 0;
        private bool _paused = true;
        private bool _running = false;

        public bool ProcessAlways = true;
        public bool ProcessInPhysics = false;
        public bool IgnoreTimeScale = false;

        public bool IsRunning => _running && !_paused;

        /// <summary>
        /// Creates a new Stopwatch not running, so it needs to be started with Start() 
        /// </summary>
        /// <param name="sceneTree"></param>
        /// </param>
        public GodotStopwatch(SceneTree sceneTree, bool processAlways = true, bool processInPhysics = false, bool ignoreTimeScale = false) {
            _sceneTree = sceneTree;
            ProcessAlways = processAlways;
            ProcessInPhysics = processInPhysics;
            IgnoreTimeScale = ignoreTimeScale;
            _sceneTreeTimer = CreateTimer();
            // Not running when it starts
            _paused = true;
            _running = false;
            _elapsed = 0;
        }

        public double Elapsed {
            get {
                if (_running && !_paused) return InternalStartTime - _sceneTreeTimer.TimeLeft + _accumulated;
                return _elapsed;
            }
        }

        /// <summary>
        /// Starts, or resumes, the timer. Elapsed field will start to increase in every frame.
        /// </summary>
        /// <returns></returns>
        public GodotStopwatch Start() {
            if (_running && _paused) {
                var diff = _timeLeftOnPause - _sceneTreeTimer.TimeLeft;
                _accumulated -= diff;
            } else if (!_running) {
                _accumulated = 0;
                _sceneTreeTimer.TimeLeft = InternalStartTime;
                _running = true;
            }
            _paused = false;
            return this;
        }

        /// <summary>
        /// Set the Elapsed time to 0, keeping the state.
        /// </summary>
        /// <returns></returns>
        public GodotStopwatch Reset() {
            if (_paused) _running = false;
            _elapsed = 0;
            _accumulated = 0;
            _sceneTreeTimer.TimeLeft = InternalStartTime;
            return this;
        }

        /// <summary>
        /// Start time from 0, no matter the current state.
        /// </summary>
        /// <returns></returns>
        public GodotStopwatch Restart() {
            _elapsed = 0;
            _accumulated = 0;
            _sceneTreeTimer.TimeLeft = InternalStartTime;
            _paused = false;
            _running = true;
            return this;
        }

        public double Alarm { get; private set; } = double.MaxValue;

        public GodotStopwatch RemoveAlarm() {
            Alarm = double.MaxValue;
            return this;
        }

        public GodotStopwatch SetAlarm(double alarm) {
            Alarm = alarm;
            return this;
        }

        public bool IsAlarm() => Elapsed >= Alarm;

        /// <summary>
        /// Stops the timer. It can be resumed with Start(). The Elapsed field will stop to increase.
        /// </summary>
        /// <returns></returns>
        public GodotStopwatch Stop() {
            if (_running && !_paused) {
                _elapsed = InternalStartTime - _sceneTreeTimer.TimeLeft + _accumulated;
                _timeLeftOnPause = _sceneTreeTimer.TimeLeft;
                _paused = true;
            }
            return this;
        }

        private SceneTreeTimer CreateTimer() {
            var sceneTreeTimer = _sceneTree.CreateTimer(InternalStartTime, ProcessAlways, ProcessInPhysics, IgnoreTimeScale);
            // With this trick will create another timer if the current one finishes.
            sceneTreeTimer.AwaitTimeout().OnCompleted(() => {
                if (!_paused) _accumulated += InternalStartTime;
                _sceneTreeTimer = CreateTimer();
            });
            return sceneTreeTimer;
        }

        public override string ToString() {
            return
                $"{(IsRunning ? "Running: " : "Stopped: ")}{Elapsed:0.000} (internal TimeLeft: {_sceneTreeTimer.TimeLeft})";
        }
    }
}