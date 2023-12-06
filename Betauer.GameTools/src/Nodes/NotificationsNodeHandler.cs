using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Nodes;
using Betauer.Core.Nodes.Events;
using Betauer.Tools.Logging;
using Godot;

namespace Betauer.Nodes;

[Notifications(PhysicsProcess = false, Process = false)]
public partial class NotificationsNodeHandler : Node {

    private static readonly Logger Logger = LoggerFactory.GetLogger<NotificationsNodeHandler>();

    public NotificationsNodeHandler() {
        ProcessMode = ProcessModeEnum.Always;
    }

    public List<(GodotObject, Action)> Watchers { get; } = new();
    public List<IProcessHandler> ProcessList { get; } = new();
    public List<IProcessHandler> PhysicsProcessList { get; } = new();
    public List<IInputEventHandler> InputList { get; } = new();
    public List<IInputEventHandler> ShortcutInputList { get; } = new();
    public List<IInputEventHandler> UnhandledInputList { get; } = new();
    public List<IInputEventHandler> UnhandledKeyInputList { get; } = new();

    private Viewport? _viewport;

    public override void _EnterTree() {
        _viewport = GetViewport();
        GetParent().ChildEnteredTree += EnsureLastChild;
    }

    public override void _ExitTree() {
        _viewport = null;
        GetParent().ChildEnteredTree -= EnsureLastChild;
    }

    private void EnsureLastChild(Node _) {
        GetParent()?.MoveChildDeferred(this, -1);
    }

    public void AddOnDestroy(GodotObject? o, Action removeAction) {
        if (o == null) return;
        Watchers.Add((o, removeAction));
        SetProcess(true);
    }
    
    public void RemoveOnDestroy(GodotObject? o, Action removeAction) {
        if (o == null) return;
        Watchers.Remove((o, removeAction));
    }
    
    public void AddOnProcess(IProcessHandler inputEvent) {
        ProcessList.Add(inputEvent);
        SetProcess(true);
    }

    public void AddOnPhysicsProcess(IProcessHandler inputEvent) {
        PhysicsProcessList.Add(inputEvent);
        SetPhysicsProcess(true);
    }

    public void AddOnInput(IInputEventHandler inputEvent) {
        InputList.Add(inputEvent);
        SetProcessInput(true);
    }

    public void AddOnUnhandledInput(IInputEventHandler inputEvent) {
        UnhandledInputList.Add(inputEvent);
        SetProcessUnhandledInput(true);
    }

    public void AddOnShortcutInput(IInputEventHandler inputEvent) {
        ShortcutInputList.Add(inputEvent);
        SetProcessShortcutInput(true);
    }

    public void AddOnUnhandledKeyInput(IInputEventHandler inputEvent) {
        UnhandledKeyInputList.Add(inputEvent);
        SetProcessUnhandledKeyInput(true);
    }

    public override partial void _Notification(int what);

    public override void _Process(double delta) {
        PurgeWatchers();
        var watchers = Watchers.Count;
        ProcessNodeEvents(ProcessList, delta, watchers == 0 ? () => SetProcess(false) : null);
    }

    public override void _PhysicsProcess(double delta) {
        ProcessNodeEvents(PhysicsProcessList, delta, () => SetPhysicsProcess(false));
    }

    public override void _Input(InputEvent inputEvent) {
        ProcessInputEventList(InputList, inputEvent, _viewport!, () => SetProcessInput(false));
    }

    public override void _UnhandledInput(InputEvent inputEvent) {
        ProcessInputEventList(UnhandledInputList, inputEvent, _viewport!, () => SetProcessUnhandledInput(false));
    }

    public override void _ShortcutInput(InputEvent inputEvent) {
        ProcessInputEventList(ShortcutInputList, inputEvent, _viewport!, () => SetProcessShortcutInput(false));
    }

    public override void _UnhandledKeyInput(InputEvent inputEvent) {
        ProcessInputEventList(UnhandledKeyInputList, inputEvent, _viewport!, () => SetProcessUnhandledKeyInput(false));
    }

    private void PurgeWatchers() {
        var destroyed = 0;
        Watchers.RemoveAll(tuple => {
            if (IsInstanceValid(tuple.Item1)) return false;
            tuple.Item2();
            destroyed++;
            return true;
        });
        if (destroyed > 0) {
            Logger.Debug("Removed {0} destroyed GodotObjects", destroyed);
        }
    }

    private static void ProcessNodeEvents(List<IProcessHandler> processHandlerList, double delta, Action? disabler) {
        if (processHandlerList.Count == 0) {
            disabler?.Invoke();
            return;
        }
        var destroyed = 0;
        processHandlerList.RemoveAll(processHandler => {
            if (processHandler.IsDestroyed) {
                destroyed++;
                return true;
            }
            try {
                if (processHandler.IsEnabled) {
                    processHandler.Handle(delta);
                }
            } catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
            return false;
        });
        if (destroyed > 0) {
            Logger.Debug("Removed {0} destroyed process handlers", destroyed);
        }
    }

    private static void ProcessInputEventList(List<IInputEventHandler> inputEventHandlerList, InputEvent inputEvent, Viewport viewport, Action disabler) {
        if (inputEventHandlerList.Count == 0) {
            disabler();
            return;
        }
        var isInputHandled = viewport.IsInputHandled();
        var destroyed = 0;
        inputEventHandlerList.RemoveAll(inputEventHandler => {
            if (inputEventHandler.IsDestroyed) {
                destroyed++;
                return true;
            }
            if (!isInputHandled && inputEventHandler.IsEnabled) {
                inputEventHandler.Handle(inputEvent);
                isInputHandled = viewport.IsInputHandled();
            }
            return false;
        });
        if (destroyed > 0) {
            Logger.Debug("Removed {0} destroyed input handlers", destroyed);
        }
    }

    public string GetStateAsString() {
        return
            $@"{Watchers.Count} Node watchers
{ProcessList.Count} Process: {String.Join(", ", ProcessList.Select(e => e.Name))}
{PhysicsProcessList.Count} PhysicsProcess: {String.Join(", ", PhysicsProcessList.Select(e => e.Name))}
{InputList.Count} Input: {String.Join(", ", InputList.Select(e => e.Name))}
{UnhandledInputList.Count} UnhandledInput: {String.Join(", ", UnhandledInputList.Select(e => e.Name))}
{ShortcutInputList.Count} ShortcutInput: {String.Join(", ", ShortcutInputList.Select(e => e.Name))}
{UnhandledKeyInputList.Count} UnhandledKeyInput: {String.Join(", ", UnhandledKeyInputList.Select(e => e.Name))}";
    }

    public void Reset() {
        Watchers.Clear();
        ProcessList.Clear();
        PhysicsProcessList.Clear();
        InputList.Clear();
        ShortcutInputList.Clear();
        UnhandledInputList.Clear();
        UnhandledKeyInputList.Clear();        
    }
}