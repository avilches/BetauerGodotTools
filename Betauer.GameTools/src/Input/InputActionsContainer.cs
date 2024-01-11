using System.Collections.Generic;
using System.Linq;
using Betauer.Application.Settings;
using Betauer.Core;
using Betauer.Input.Handler;

namespace Betauer.Input;

public interface ISettingsContainerAware {
    public SettingsContainer SettingsContainer { get; }
}

public class InputActionsContainer {
    public List<AxisAction> AxisActions { get; } = new();
    public List<InputAction> InputActions { get; } = new();
    private readonly HashSet<GodotInputHandler> _onInputActions = new();

    public AxisAction? GetAxisAction(string name) {
        return AxisActions.Find(action => action.Name == name);
    }

    public InputAction? GetInputAction(string name) {
        return InputActions.Find(action => action.Name == name);
    }

    public void LoadFromInstance(object instance) {
        var propertyInfos = instance.GetType().GetProperties();

        propertyInfos
            .Where(p =>
                p.PropertyType.IsAssignableFrom(typeof(AxisAction)) ||
                p.PropertyType.IsAssignableFrom(typeof(InputAction)))
            .Select(p => p.GetValue(instance))
            .Where(i => i != null)
            .ForEach(i => {
                if (i is AxisAction axisAction) {
                    Add(axisAction);
                    TryAddSaveSettings(axisAction, instance);
                }
                else if (i is InputAction inputAction) {
                    Add(inputAction);
                    TryAddSaveSettings(inputAction, instance);
                }
                
            });
    }

    private void TryAddSaveSettings(AxisAction axisAction, object instance) {
        if (axisAction.SaveAs == null) return;
        if (!instance.GetType().ImplementsInterface(typeof(ISettingsContainerAware))) return;
        var settingsContainer = ((ISettingsContainerAware) instance).SettingsContainer;
        if (settingsContainer == null) return;
        axisAction.CreateSaveSetting(settingsContainer);
    }

    private void TryAddSaveSettings(InputAction axisAction, object instance) {
        if (axisAction.SaveAs == null) return;
        if (!instance.GetType().ImplementsInterface(typeof(ISettingsContainerAware))) return;
        var settingsContainer = ((ISettingsContainerAware) instance).SettingsContainer;
        if (settingsContainer == null) return;
        axisAction.CreateSaveSetting(settingsContainer);
    }

    public void Add(AxisAction axisAction) {
        if (axisAction == null || AxisActions.Contains(axisAction)) return; // Avoid duplicates
        AxisActions.Add(axisAction);
        TryLinkAxisActionToNegativePositiveInputs(axisAction);
        Add(axisAction.Negative);
        Add(axisAction.Positive);
    }

    public void Add(InputAction inputAction) {
        if (inputAction == null || InputActions.Contains(inputAction)) return; // Avoid duplicates
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

    public void Remove(AxisAction axisAction) {
        if (axisAction == null) return;
        AxisActions.Remove(axisAction);
        Remove(axisAction.Negative);
        Remove(axisAction.Positive);
    }

    public void Remove(InputAction inputAction) {
        if (inputAction == null) return;
        InputActions.Remove(inputAction);
        CheckInputHandler(inputAction);
    }

    public void DisableAll() {
        InputActions.ForEach(action => action.Enable(false));
    }

    public void EnableAll(bool enable = true) {
        InputActions.ForEach(action => action.Enable(enable));
    }

    public void Clear() {
        InputActions.ToList().ForEach(Remove);
        AxisActions.ToList().ForEach(Remove);
        // TODO: call to CheckInputHandler? 
    }

    internal void CheckInputHandler(InputAction inputAction) {
        if (inputAction.Handler is GodotInputHandler handler && handler.HasJustTimers) {
            if (inputAction.Enabled) {
                _onInputActions.Add(handler); // This is Set, so it already take care of duplicates
            } else {
                _onInputActions.Remove(handler);
            }
        }
    }

    // public override void _Input(InputEvent e) {
    //     if (_onInputActions.Count == 0) {
    //         SetProcessInput(false);
    //         return;
    //     }
    //     var span = CollectionsMarshal.AsSpan(_onInputActions);
    //     for (var i = 0; i < span.Length; i++) {
    //         var handler = span[i];
    //         handler.UpdateJustTimers();
    //     }
    // }
}