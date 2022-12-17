using System;
using Betauer.Core.Time;
using Betauer.Nodes;
using Godot;

namespace Betauer.Input;

public class DelayedAction : IDisposable {
    private readonly DelayedActionInputEventHandler _eventHandler;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sceneTree"></param>
    /// <param name="action"></param>
    /// <param name="processAlways">If <c>processAlways</c> is set to <c>false</c>, pausing the <see cref="T:Godot.SceneTree" /> will also pause the timer.</param>
    /// <param name="processInPhysics">If <c>processInPhysics</c> is set to <c>true</c>, will update the action timer during the physics frame instead of the process frame (fixed framerate processing).</param>
    /// <param name="ignoreTimeScale">If <c>ignoreTimeScale</c> is set to <c>true</c>, will ignore <see cref="P:Godot.Engine.TimeScale" /> and update the <see cref="T:Godot.SceneTreeTimer" /> with the actual frame delta.</param>
    internal DelayedAction(SceneTree sceneTree, InputAction action, bool processAlways = false, bool processInPhysics = false, bool ignoreTimeScale = false) {
        var godotStopwatch = new GodotStopwatch(sceneTree, processAlways, processInPhysics, ignoreTimeScale);
        _eventHandler = new DelayedActionInputEventHandler(godotStopwatch, action, processAlways);
        DefaultNodeHandler.Instance.OnInput(_eventHandler);
    }

    private class DelayedActionInputEventHandler : NodeHandler.IInputEventHandler {
        internal readonly GodotStopwatch GodotStopwatch;
        private readonly InputAction _inputAction;
        private readonly bool _processAlways;

        public DelayedActionInputEventHandler(GodotStopwatch godotStopwatch, InputAction inputAction, bool processAlways) {
            Name = $"Delayed{inputAction.Name}";
            GodotStopwatch = godotStopwatch;
            _inputAction = inputAction;
            _processAlways = processAlways;
        }

        public string? Name { get; }
        public bool IsDestroyed { get; set;  }
        public bool IsEnabled(bool isTreePaused) => _processAlways || !isTreePaused;
        public void OnInput(InputEvent inputEvent) {
            if (_inputAction.IsEventJustPressed(inputEvent)) GodotStopwatch.Restart();
        }
    }

    public double LastPressed => _eventHandler.GodotStopwatch.Elapsed;
    public bool WasPressed(float limit) => _eventHandler.GodotStopwatch.IsRunning && (float)_eventHandler.GodotStopwatch.Elapsed <= limit;

    ~DelayedAction() => Dispose(false);
    public void Dispose() => Dispose(true);

    private void Dispose(bool disposing) {
        if (disposing) GC.SuppressFinalize(this);
        _eventHandler.IsDestroyed = true;
    }

}