using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Betauer.Input;
using Betauer.Tools.Logging;
using Godot;
using static System.String;

namespace Betauer.Nodes;

public partial class NodeHandler : Node2D {
    public static bool ShouldProcess(bool pause, ProcessModeEnum processMode) {
        if (processMode == ProcessModeEnum.Inherit) return !pause;
        return processMode == ProcessModeEnum.Always ||
               (pause && processMode == ProcessModeEnum.WhenPaused) ||
               (!pause && processMode == ProcessModeEnum.Pausable);
    }

    public readonly List<IProcessHandler> ProcessList = new();
    public readonly List<IProcessHandler> PhysicsProcessList = new();
    public readonly List<IInputEventHandler> InputList = new();
    public readonly List<IInputEventHandler> ShortcutInputList = new();
    public readonly List<IInputEventHandler> UnhandledInputList = new();
    public readonly List<IInputEventHandler> UnhandledKeyInputList = new();
    public readonly List<IDrawHandler> DrawList = new();
    
    private SceneTree _sceneTree;
    
    public void AddTo(Viewport viewport) {
        GetParent()?.RemoveChild(this);
        viewport.AddChild(this);
    }

    public override void _EnterTree() {
        _sceneTree = GetTree();
        ProcessMode = ProcessModeEnum.Always;
        GetParent().ChildEnteredTree += EnsureLastChild;
    }

    public override void _ExitTree() {
        GetParent().ChildEnteredTree -= EnsureLastChild;
    }

    private void EnsureLastChild(Node _) {
        GetParent()?.MoveChild(this, -1);
    }

    public void OnProcess(IProcessHandler inputEvent) {
        ProcessList.Add(inputEvent);
        SetProcess(true);
    }

    public void OnPhysicsProcess(IProcessHandler inputEvent) {
        PhysicsProcessList.Add(inputEvent);
        SetPhysicsProcess(true);
    }

    public void OnInput(IInputEventHandler inputEvent) {
        InputList.Add(inputEvent);
        SetProcessInput(true);
    }

    public void OnUnhandledInput(IInputEventHandler inputEvent) {
        UnhandledInputList.Add(inputEvent);
        SetProcessUnhandledInput(true);
    }

    public void OnShortcutInput(IInputEventHandler inputEvent) {
        ShortcutInputList.Add(inputEvent);
        SetProcessShortcutInput(true);
    }

    public void OnUnhandledKeyInput(IInputEventHandler inputEvent) {
        UnhandledKeyInputList.Add(inputEvent);
        SetProcessUnhandledKeyInput(true);
    }

    public void OnDraw(IDrawHandler inputEvent) {
        DrawList.Add(inputEvent);
        SetProcess(true);
    }
    
    public Task AwaitInput(Func<InputEvent, bool> func, bool setInputAsHandled = true) {
        TaskCompletionSource promise = new();
        InputEventEventHandler eventHandler = null; 
        eventHandler = new InputEventEventHandler("AwaitInput", e => {
            if (func(e)) {
                if (setInputAsHandled) GetViewport().SetInputAsHandled();
                eventHandler.Destroy();
                promise.TrySetResult();
            }
        }, ProcessModeEnum.Always);
        OnInput(eventHandler);
        return promise.Task;
        
    }
    
    public Task AwaitUnhandledInput(Func<InputEvent, bool> func, bool setInputAsHandled = true) {
        TaskCompletionSource promise = new();
        InputEventEventHandler eventHandler = null; 
        eventHandler = new InputEventEventHandler("AwaitUnhandledInput", e => {
            if (func(e)) {
                if (setInputAsHandled) GetViewport().SetInputAsHandled();
                eventHandler.Destroy();
                promise.TrySetResult();
            }
        }, ProcessModeEnum.Always);
        OnUnhandledInput(eventHandler);
        return promise.Task;
    }

    public override void _Process(double delta) {
        ProcessNodeEvents(ProcessList, delta, () => {
            if (DrawList.Count == 0) SetProcess(false);
        });
        if (DrawList.Count > 0) QueueRedraw();
    }

    public override void _PhysicsProcess(double delta) {
        ProcessNodeEvents(PhysicsProcessList, delta, () => SetPhysicsProcess(false));
    }

    private void ProcessNodeEvents(List<IProcessHandler> processHandlerList, double delta, Action disabler) {
        if (processHandlerList.Count == 0) {
            disabler();
            return;
        }
        var isTreePaused = _sceneTree.Paused;
        processHandlerList.RemoveAll(processHandler => {
            if (processHandler.IsDestroyed) return true;
            if (processHandler.IsEnabled(isTreePaused)) {
                processHandler.Handle(delta);
            }
            return false;
        });
    }

    public override void _Input(InputEvent inputEvent) {
        ProcessInputEventList(InputList, inputEvent, () => SetProcessInput(false));
    }

    public override void _UnhandledInput(InputEvent inputEvent) {
        ProcessInputEventList(UnhandledInputList, inputEvent, () => SetProcessUnhandledInput(false));
    }

    public override void _ShortcutInput(InputEvent inputEvent) {
        ProcessInputEventList(ShortcutInputList, inputEvent, () => SetProcessShortcutInput(false));
    }

    public override void _UnhandledKeyInput(InputEvent inputEvent) {
        ProcessInputEventList(UnhandledKeyInputList, inputEvent, () => SetProcessUnhandledKeyInput(false));
    }

    public override void _Draw() {
        if (DrawList.Count == 0) return;
        var isTreePaused = _sceneTree.Paused;
        DrawList.RemoveAll(processHandler => {
            if (processHandler.IsDestroyed) return true;
            if (processHandler.IsEnabled(isTreePaused)) {
                processHandler.Handle(this);
            }
            return false;
        });
    }

    private void ProcessInputEventList(List<IInputEventHandler> inputEventHandlerList, InputEvent inputEvent, Action disabler) {
        if (inputEventHandlerList.Count == 0) {
            disabler();
            return;
        }
        var isInputHandled = GetViewport().IsInputHandled();
        var isTreePaused = _sceneTree.Paused;
        inputEventHandlerList.RemoveAll(inputEventHandler => {
            if (inputEventHandler.IsDestroyed) return true;
            if (!isInputHandled && inputEventHandler.IsEnabled(isTreePaused)) {
                inputEventHandler.Handle(inputEvent);
                isInputHandled = GetViewport().IsInputHandled();
            }
            return false;
        });
    }

    public string GetStateAsString() {
        return 
$@"{ProcessList.Count} Process: {Join(", ", ProcessList.Select(e => e.Name))}
{PhysicsProcessList.Count} PhysicsProcess: {Join(", ", PhysicsProcessList.Select(e => e.Name))}
{InputList.Count} Input: {Join(", ", InputList.Select(e => e.Name))}
{UnhandledInputList.Count} UnhandledInput: {Join(", ", UnhandledInputList.Select(e => e.Name))}
{ShortcutInputList.Count} ShortcutInput: {Join(", ", ShortcutInputList.Select(e => e.Name))}
{UnhandledKeyInputList.Count} UnhandledKeyInput: {Join(", ", UnhandledKeyInputList.Select(e => e.Name))}";
    }
}