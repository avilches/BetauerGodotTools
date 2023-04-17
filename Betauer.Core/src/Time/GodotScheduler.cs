using System;
using System.Threading.Tasks;
using Godot;

namespace Betauer.Core.Time; 

/// <summary>
/// Execute a lambda periodically. It uses internally the "timeout" signal from a SceneTree timer,
/// so it's affected by the Engine.Timescale and the SceneTree.Pause state.
///
/// Usage:
/// <code>
/// var gs = new GodotScheduler(1f, 0.1f, () => {}).Start();
/// 
/// </code> 
/// </summary>
public class GodotScheduler {
    private Action _action;
    private readonly GodotTimeout _godotTimeout;
    private TaskCompletionSource _promise;

    public double EverySeconds { get; set; }
    public bool IsRunning { get; private set; } = false;

    public GodotScheduler(SceneTree sceneTree, double initialDelay, double everySeconds, Action action, bool processAlways = false, bool processInPhysics = false, bool ignoreTimeScale = false) {
        _action = action;
        EverySeconds = everySeconds;
        _godotTimeout = new GodotTimeout(sceneTree, initialDelay, Execute, processAlways, processInPhysics, ignoreTimeScale);
    }

    public GodotScheduler(double initialDelay, double seconds, Action action, bool processAlways = false, bool processInPhysics = false, bool ignoreTimeScale = false) :
        this(Engine.GetMainLoop() as SceneTree, initialDelay, seconds, action, processAlways, processInPhysics, ignoreTimeScale) {
    }

    public GodotScheduler OnExecute(Action action) {
        _action = action;
        return this;
    }

    public GodotScheduler Start() {
        _godotTimeout.Start();
        if (!IsRunning) {
            Loop();
        }
        return this;
    }

    public GodotScheduler Stop() {
        _godotTimeout.Stop();
        IsRunning = false;
        _promise?.TrySetResult();
        return this;
    }

    public GodotScheduler Reset() {
        _godotTimeout.Reset();
        return this;
    }

    private async void Loop() {
        IsRunning = true;
        while (IsRunning) {
            _promise = new TaskCompletionSource();
            await _promise.Task;
            _godotTimeout.SetTimeout(EverySeconds);
            if (IsRunning) {
                _godotTimeout.Restart();
            }
        }
    }

    private void Execute() {
        _action?.Invoke();
        _promise.TrySetResult();
    }
}