using System;
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
        
    public double EverySeconds { get; set; }

    public GodotScheduler(SceneTree sceneTree, double initialDelay, double everySeconds, Action action, bool processAlways = false, bool processInPhysics = false, bool ignoreTimeScale = false) {
        _action = action;
        EverySeconds = everySeconds;
        _godotTimeout = new GodotTimeout(sceneTree, initialDelay, () => _action(), processAlways, processInPhysics, ignoreTimeScale);
        Loop();
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
        return this;
    }

    public GodotScheduler Stop() {
        _godotTimeout.Stop();
        return this;
    }

    public GodotScheduler Reset() {
        _godotTimeout.Reset();
        return this;
    }

    public bool IsRunning() => _godotTimeout.IsRunning;

    private async void Loop() {
        while (true) {
            await _godotTimeout.Await();
            _godotTimeout.SetTimeout(EverySeconds);
            _godotTimeout.Restart();
        }
    }
}