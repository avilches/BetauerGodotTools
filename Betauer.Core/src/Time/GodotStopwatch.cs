using System;
using Betauer.Core.Signal;
using Godot;

namespace Betauer.Core.Time; 

/// <summary>
/// Stopwatch is a tool to measure elapsed time. Features:
/// 
/// - Start(), Stop(), Restart() and Reset() methods.
/// - It creates internally as many SceneTreeTimer instances needed with SceneTree.CreateTimer() to
/// handle the measurement, so it's affected by the Engine.Timescale and the current SceneTree.Pause state, things that C# System.Diagnostics.Stopwatch can not do.
/// - When created, it doesn't start automatically, so you need to call to Start().
/// - There is no timeout signal, so consider using SceneTreeTimer and add a Timeout event if you need to execute an action after some time. To pause/resume and
/// reuse timeout, consider using GodotTimer. 
///
/// At any time, the Stopwatch can set an Alarm, which is a time in seconds. The IsAlarm property will be true when the Alarm is reached.
/// <code>
/// var godotStopwatch = new GodotStopwatch().Start();
/// godotStopwatch.Alarm = 5;     // Set the Alarm to 5 seconds
/// if (godoStopwatch.IsAlarm())
/// {
///     // Will be true if Elapsed > 5
/// }
/// </code> 
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
    public bool ProcessAlways { get; init; } = true;
    public bool ProcessInPhysics { get; init; } = false;
    public bool IgnoreTimeScale { get; init; } = false;
    
    public double Elapsed => GetElapsed();
    public TimeSpan ElapsedTimeSpan => new TimeSpan((long) (Elapsed * TimeSpan.TicksPerSecond));
    public double Alarm { get; private set; } = double.MaxValue;
    public bool IsAlarm() => Elapsed >= Alarm;
    public bool IsRunning() => _running && !_paused;



    /// <summary>
    /// Creates a new Stopwatch not running, so it needs to be started with Start() or Restart() 
    /// </summary>
    /// <param name="sceneTree"></param>
    /// <param name="processAlways">If <c>processAlways</c> is set to <c>false</c>, pausing the <see cref="T:Godot.SceneTree" /> will also pause the timer.</param>
    /// <param name="processInPhysics">If <c>processInPhysics</c> is set to <c>true</c>, will update the <see cref="T:Godot.SceneTreeTimer" /> during the physics frame instead of the process frame (fixed framerate processing).</param>
    /// <param name="ignoreTimeScale">If <c>ignoreTimeScale</c> is set to <c>true</c>, will ignore <see cref="P:Godot.Engine.TimeScale" /> and update the <see cref="T:Godot.SceneTreeTimer" /> with the actual frame delta.</param>
    public GodotStopwatch(SceneTree sceneTree, bool processAlways = false, bool processInPhysics = false, bool ignoreTimeScale = false) {
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

    public GodotStopwatch(bool processAlways = false, bool processInPhysics = false, bool ignoreTimeScale = false) : 
        this((SceneTree)Engine.GetMainLoop(), processAlways, processInPhysics, ignoreTimeScale) {
    }

    /// <summary>
    /// Starts, or resumes, the timer. Elapsed field will start to increase.
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
    /// Set the Elapsed time to 0, keeping the current state. So, if it was running, it will continue running.
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
    /// Set the Elapsed time to 0, no matter the current state. Convenience method for replacing {sw.Reset(); sw.Start();} with a single sw.Restart() 
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

    public GodotStopwatch RemoveAlarm() {
        Alarm = double.MaxValue;
        return this;
    }

    public GodotStopwatch SetAlarm(double alarm) {
        Alarm = alarm;
        return this;
    }

    /// <summary>
    /// Stops the timer. It can be resumed with Start(). The Elapsed field stops increasing
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

    public override string ToString() {
        return
            $"{(IsRunning() ? "Running: " : "Stopped: ")}{Elapsed:0.000} (internal TimeLeft: {_sceneTreeTimer.TimeLeft})";
    }

    private double GetElapsed() { 
        if (_running && !_paused) return InternalStartTime - _sceneTreeTimer.TimeLeft + _accumulated;
        return _elapsed;
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
}