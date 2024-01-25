using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Betauer.Core;
using Betauer.Input.Handler;
using Betauer.Nodes;
using Godot;

namespace Betauer.Input;

public partial class InputActionsContainer {
    public List<AxisAction> AxisActions { get; } = new();
    public List<InputAction> InputActions { get; } = new();

    private readonly List<GodotInputHandler> _onInputActions = new();

    public AxisAction? GetAxisAction(string name) {
        return AxisActions.Find(action => action.Name == name);
    }

    public InputAction? GetInputAction(string name) {
        return InputActions.Find(action => action.Name == name);
    }

    public void AddActionsFromProperties(object instance) {
        var propertyInfos = instance.GetType().GetProperties();

        propertyInfos
            .Where(p => typeof(InputAction).IsAssignableFrom(p.PropertyType))
            .Select(p => p.GetValue(instance))
            .Where(i => i != null)
            .Cast<InputAction>()
            .ForEach(Add);

        propertyInfos
            .Where(p => typeof(AxisAction).IsAssignableFrom(p.PropertyType))
            .Select(p => p.GetValue(instance))
            .Where(i => i != null)
            .Cast<AxisAction>()
            .ForEach(Add);
    }

    public void Add(AxisAction axisAction) {
        if (AxisActions.Contains(axisAction)) return; // Avoid duplicates
        AxisActions.Add(axisAction);
        TryLinkAxisActionToNegativePositiveInputs(axisAction);
        TryAddSaveSettings(axisAction);
        if (axisAction.Negative != null) Add(axisAction.Negative);
        if (axisAction.Positive != null) Add(axisAction.Positive);
    }

    public void Add(InputAction inputAction) {
        if (InputActions.Contains(inputAction)) return; // Avoid duplicates
        TryAddSaveSettings(inputAction);
        InputActions.Add(inputAction);
        if (inputAction.AxisName != null) {
            if (AxisActions.Find(action => action.Name == inputAction.AxisName) is AxisAction axisAction) {
                TryLinkAxisActionToNegativePositiveInputs(axisAction);
            }
        }
        CheckInputHandler(inputAction);
    }

    private void TryLinkAxisActionToNegativePositiveInputs(AxisAction axisAction) {
        if (axisAction.Negative == null && axisAction.Positive == null) {
            var pairs = InputActions.FindAll(action => action.AxisName == axisAction.Name);
            if (pairs.Count == 2) {
                axisAction.SetNegativeAndPositive(pairs[0], pairs[1]);
                Add(pairs[0]);
                Add(pairs[1]);
            }
        }
    }

    public void Remove(InputAction inputAction) {
        if (InputActions.Remove(inputAction)) {
            CheckInputHandler(inputAction);
        }
    }

    public void Remove(AxisAction axisAction) {
        AxisActions.Remove(axisAction);
        if (axisAction.Negative != null) Remove(axisAction.Negative);
        if (axisAction.Positive != null) Remove(axisAction.Positive);
    }

    public void EnableAll(bool enable = true) {
        InputActions.ForEach(action => {
            action.Enable(enable);
            CheckInputHandler(action);
        });
        NodeManager.MainInstance.OnInput -= OnInputHandler;
        if (_onInputActions.Count > 0) {
            NodeManager.MainInstance.OnInput += OnInputHandler;
        }
    }

    public void DisableAll() {
        NodeManager.MainInstance.OnInput -= OnInputHandler;
        InputActions.ForEach(action => action.Disable());
    }

    public void Clear() {
        InputActions.ForEach(action => {
            action.Disable();
            TryRemoveSaveSettings(action);
        });
        InputActions.Clear();
        AxisActions.ForEach(axisAction => {
            axisAction.Disable();
            TryRemoveSaveSettings(axisAction);
        });
        AxisActions.Clear();
    }

    private void CheckInputHandler(InputAction inputAction) {
        if (inputAction.Handler is GodotInputHandler handler && handler.HasJustTimers) {
            if (inputAction.Enabled) {
                if (!_onInputActions.Contains(handler)) _onInputActions.Add(handler);
            } else {
                _onInputActions.Remove(handler);
            }
        }
    }

    private void OnInputHandler(InputEvent obj) {
        if (_onInputActions.Count == 0) {
            NodeManager.MainInstance.OnInput -= OnInputHandler;
            return;
        }
        var span = CollectionsMarshal.AsSpan(_onInputActions);
        for (var i = 0; i < span.Length; i++) {
            var handler = span[i];
            handler.UpdateJustTimers();
        }
    }
}