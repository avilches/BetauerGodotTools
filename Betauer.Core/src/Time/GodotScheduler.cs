using System;
using System.Threading.Tasks;
using Godot;

namespace Betauer.Core.Time; 

/// <summary>
/// GodotScheduler executes an action periodically with an initial delay. Features:
/// 
/// - Start(), Stop(), Restart() and Reset() methods.
/// - It creates internally as many SceneTreeTimer instances needed with SceneTree.CreateTimer() to
/// handle the scheduler, so it's affected by the Engine.Timescale and the current SceneTree.Pause state.
/// - When created, it doesn't start automatically, so you need to call to Start().
/// - The EverySeconds property can be changed at runtime, but it will only take effect after the next execution.
///
/// Usage:
/// <code>
/// var gs = new GodotScheduler(1f, 0.1f, () => {}).Start();
/// </code> 
/// </summary>
public class GodotScheduler {
    private readonly GodotTimeout _godotTimeout;
    private TaskCompletionSource _promise;
    private Action _action;
    private bool _isRunning = false;

    public double EverySeconds { get; set; }
    public bool IsRunning() => _isRunning;

    public GodotScheduler(SceneTree sceneTree, double initialDelay, double everySeconds, Action action, bool processAlways = false, bool processInPhysics = false, bool ignoreTimeScale = false) {
        _action = action;
        EverySeconds = everySeconds;
        _godotTimeout = new GodotTimeout(sceneTree, initialDelay, Execute, processAlways, processInPhysics, ignoreTimeScale);
    }

    public GodotScheduler(double initialDelay, double seconds, Action action, bool processAlways = false, bool processInPhysics = false, bool ignoreTimeScale = false) :
        this((SceneTree)Engine.GetMainLoop(), initialDelay, seconds, action, processAlways, processInPhysics, ignoreTimeScale) {
    }

    public GodotScheduler OnExecute(Action action) {
        _action = action;
        return this;
    }

    public GodotScheduler Start() {
        _godotTimeout.Start();
        if (!_isRunning) {
            Loop();
        }
        return this;
    }

    public GodotScheduler Stop() {
        _godotTimeout.Stop();
        _isRunning = false;
        _promise?.TrySetResult();
        return this;
    }

    public GodotScheduler Reset() {
        _godotTimeout.Reset();
        return this;
    }

    private async void Loop() {
        _isRunning = true;
        while (_isRunning) {
            _promise = new TaskCompletionSource();
            await _promise.Task;
            _godotTimeout.SetTimeout(EverySeconds);
            if (_isRunning) {
                _godotTimeout.Restart();
            }
        }
    }

    private void Execute() {
        _action?.Invoke();
        _promise.TrySetResult();
    }
}