using System.Collections.Generic;
using System.Linq;
using Betauer.Core;

namespace Betauer.Input;

public partial class InputActionsContainer {
    public List<AxisAction> AxisActions { get; } = new();
    public List<InputAction> InputActions { get; } = new();

    public AxisAction? GetAxisAction(string name) {
        return AxisActions.Find(action => action.Name == name);
    }

    public InputAction? GetInputAction(string name) {
        return InputActions.Find(action => action.Name == name);
    }

    public void AddActionsFromProperties(object instance) {
        var propertyInfos = instance.GetType().GetProperties();

        foreach (var action in propertyInfos
                     .Where(p => typeof(InputAction).IsAssignableFrom(p.PropertyType))
                     .Select(p => p.GetValue(instance))
                     .Where(i => i != null)
                     .Cast<InputAction>()) {
            Add(action);
        }

        foreach (var action in propertyInfos
                     .Where(p => typeof(AxisAction).IsAssignableFrom(p.PropertyType))
                     .Select(p => p.GetValue(instance))
                     .Where(i => i != null)
                     .Cast<AxisAction>()) {
            Add(action);
        }
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
        InputActions.Remove(inputAction);
    }

    public void Remove(AxisAction axisAction) {
        AxisActions.Remove(axisAction);
        if (axisAction.Negative != null) Remove(axisAction.Negative);
        if (axisAction.Positive != null) Remove(axisAction.Positive);
    }

    public void EnableAll(bool enable = true) {
        InputActions.ForEach(action => action.Enable(enable));
    }

    public void DisableAll() {
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
}