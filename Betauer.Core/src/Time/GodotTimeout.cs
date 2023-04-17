using System;
using System.Threading.Tasks;
using Betauer.Core.Signal;
using Godot;

namespace Betauer.Core.Time; 

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
    private double _timeout = 0;
    private double _timeLeft = 0;
    
    public event Action OnTimeout;
    public double TimeLeft => GetTimeLeft();
    public double Timeout {
        get => _timeout;
        set => SetTimeout(value);
    }
    public double Elapsed => _timeout - TimeLeft;
    public bool ProcessAlways = true;
    public bool ProcessInPhysics = false;
    public bool IgnoreTimeScale = false;
    public bool IsRunning => _running && !_paused;

    public GodotTimeout(SceneTree sceneTree, double timeout, Action onTimeout, bool processAlways = false,
        bool processInPhysics = false, bool ignoreTimeScale = false) {
        _sceneTree = sceneTree;
        _timeout = _timeLeft = timeout;
        OnTimeout += onTimeout;
        ProcessAlways = processAlways;
        ProcessInPhysics = processInPhysics;
        IgnoreTimeScale = ignoreTimeScale;
    }

    public GodotTimeout(double timeout, Action onTimeout, bool processAlways = false, bool processInPhysics = false, bool ignoreTimeScale = false) : 
        this(Engine.GetMainLoop() as SceneTree, timeout, onTimeout, processAlways, processInPhysics, ignoreTimeScale) {
    }


    public GodotTimeout RemoveOnTimeout(Action action) {
        OnTimeout -= action;
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
        _timeout = timeout;
        return this;
    }

    /// <summary>
    /// Starts, or resumes, the timer. Elapsed field will start to increase in every frame.
    /// </summary>
    /// <returns></returns>
    public GodotTimeout Start() {
        if (!_running) {
            // not running, create a new one
            _sceneTreeTimer = CreateTimer(_timeout);
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
            _timeLeft = _timeout;
        } else {
            // running and not paused 
            _sceneTreeTimer!.TimeLeft = _timeout;
        }
        return this;
    }

    public GodotTimeout Restart() {
        if (_running && !_paused) {
            _sceneTreeTimer!.TimeLeft = _timeout;
        } else {
            // running and paused, or not running
            _sceneTreeTimer = CreateTimer(_timeout);
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

    public Task Await() {
        TaskCompletionSource promise = new();
        void ToAdd() {
            OnTimeout -= ToAdd;
            promise.TrySetResult();
        }
        OnTimeout += ToAdd;
        return promise.Task;
    }

    public override string ToString() {
        return $"{(IsRunning ? "Running: " : "Stopped: ")}{_timeout}/{TimeLeft:0.0} (internal TimeLeft: {_sceneTreeTimer?.TimeLeft:0.0})";
    }

    private double GetTimeLeft() { 
        if (_running && !_paused) return _sceneTreeTimer!.TimeLeft;
        return _timeLeft;
    }

    private SceneTreeTimer CreateTimer(double timeout) {
        var sceneTreeTimer = _sceneTree.CreateTimer(timeout, ProcessAlways, ProcessInPhysics, IgnoreTimeScale);
        var sceneTreeTimerId = sceneTreeTimer.GetHashCode();
        sceneTreeTimer.AwaitTimeout().OnCompleted(() => {
            if (_sceneTreeTimer != null && _sceneTreeTimer.GetHashCode() == sceneTreeTimerId && _running && !_paused) {
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
        OnTimeout?.Invoke();
    }
}