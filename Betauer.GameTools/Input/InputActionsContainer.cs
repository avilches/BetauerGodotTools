using System.Collections.Generic;
using Betauer.Application.Monitor;
using Betauer.DI;
using Godot;

namespace Betauer.Input; 

public class InputActionsContainer {
    [Inject(Nullable = true)] protected DebugOverlayManager? DebugOverlayManager { get; set; }

    public readonly List<IAction> InputActionList = new();
    public readonly Dictionary<string, IAction> ActionMap = new();

    [PostInject]
    public void ConfigureCommands() {
        DebugOverlayManager?.DebugConsole.AddInputEventCommand(this);
        DebugOverlayManager?.DebugConsole.AddInputMapCommand(this);
    }

    public IAction? FindAction(string name) {
        return ActionMap.TryGetValue(name, out var action) ? action : null;
    }

    public T? FindAction<T>(string name) where T : class, IAction {
        return ActionMap.TryGetValue(name, out var action) ? action as T: null;
    }

    public IAction? FindAction(InputEvent inputEvent) {
        return InputActionList.Find(action => action.IsEvent(inputEvent));
    }

    public List<IAction> FindActions(InputEvent inputEvent) {
        var list = new List<IAction>();
        for (var i = 0; i < InputActionList.Count; i++) {
            if (InputActionList[i].IsEvent(inputEvent)) list.Add(InputActionList[i]);
        }
        return list;
    }

    public AxisAction? FindAction(JoyAxis axis) {
        return InputActionList.Find(action => action is AxisAction && action.Axis == axis) as AxisAction;
    }

    public List<IAction> FindActions(JoyAxis axis) {
        return InputActionList.FindAll(action => action.Axis == axis);
    }

    public InputAction? FindAction(JoyButton button) {
        return InputActionList.Find(action => action is InputAction a && a.HasButton(button)) as InputAction;
    }

    public List<InputAction> FindActions(JoyButton button) {
        var list = new List<InputAction>();
        for (var i = 0; i < InputActionList.Count; i++) {
            if (InputActionList[i] is InputAction a && a.HasButton(button)) list.Add(a);
        }
        return list;
    }

    public InputAction? FindAction(Key key) {
        return InputActionList.Find(action => action is InputAction a && a.HasKey(key)) as InputAction;
    }

    public List<InputAction> FindActions(Key key) {
        var list = new List<InputAction>();
        for (var i = 0; i < InputActionList.Count; i++) {
            if (InputActionList[i] is InputAction a && a.HasKey(key)) list.Add(a);
        }
        return list;
    }

    public void Add(InputAction inputAction) {
        InputActionList.Add(inputAction);
        ActionMap.Add(inputAction.Name, inputAction);
    }

    public void Add(AxisAction axisAction) {
        InputActionList.Add(axisAction);
        ActionMap.Add(axisAction.Name, axisAction);
    }

    public void Disable() {
        InputActionList.ForEach(action => action.Enable(false));
    }

    public void Enable(bool enabled = true) {
        InputActionList.ForEach(action => action.Enable(enabled));
    }
}