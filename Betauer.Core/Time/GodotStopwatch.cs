using Betauer.Signal;
using Godot;

namespace Betauer.Time {
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
        private float _elapsed = 0;
        private float _accumulated = 0;
        private float _timeLeftOnPause = 0;
        private bool _paused = true;
        private bool _running = false;

        public readonly Node.PauseModeEnum PauseMode;
        public bool IsRunning => _running && !_paused;

        /// <summary>
        /// Creates a new Stopwatch not running, so it needs to be started with Start() 
        /// </summary>
        /// <param name="sceneTree"></param>
        /// <param name="pauseMode">
        /// PauseModeEnum.Inherit or PauseModeEnum.Stop: the timer will pause along with the SceneTree
        /// PauseModeEnum.Process: timer run always, ignoring the SceneTree.Pause state 
        /// </param>
        public GodotStopwatch(SceneTree sceneTree, Node.PauseModeEnum pauseMode = Node.PauseModeEnum.Inherit) {
            _sceneTree = sceneTree;
            PauseMode = pauseMode;
            _sceneTreeTimer = CreateTimer();
            // Not running when it starts
            _paused = true;
            _running = false;
            _elapsed = 0;
        }

        public float Elapsed {
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

        public float Alarm { get; private set; } = float.MaxValue;

        public GodotStopwatch RemoveAlarm() {
            Alarm = float.MaxValue;
            return this;
        }

        public GodotStopwatch SetAlarm(float alarm) {
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
            var pauseModeProcess = PauseMode == Node.PauseModeEnum.Process; // false = pausing the scene pause the timer 
            var sceneTreeTimer = _sceneTree.CreateTimer(InternalStartTime, pauseModeProcess);
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