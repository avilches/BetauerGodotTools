using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core;
using Betauer.Tools.FastReflection;

namespace Betauer.Input.Joypad;

public abstract class JoypadController {
    public InputActionsContainer InputActionsContainer { get; private set; }
    private InputActionsContainer _source;
    private PlayerMapping _playerMapping;

    private List<ISetter>? _fastSetters;

    protected List<ISetter> FastSetters => _fastSetters ??=
        GetType().GetProperties()
            .Where(p => p.PropertyType.ImplementsInterface(typeof(IAction)))
            .Select(p => new FastSetter(p) as ISetter)
            .ToList();
    
    internal void Configure(PlayerMapping playerMapping, InputActionsContainer source) {
        _playerMapping = playerMapping;
        _source = source;
        JoypadIdChanged();
        playerMapping.OnJoypadIdChanged += JoypadIdChanged;
    }
    
    private void JoypadIdChanged() {
        InputActionsContainer?.Disable();
        InputActionsContainer?.Free();
        var suffix = _playerMapping.Player.ToString();
        InputActionsContainer = _source.Clone(suffix, UpdateInputAction);
        InputActionsContainer.Enable();
        
        FastSetters.ForEach(setter => {
            var name = $"{setter.Name}/{suffix}";
            var action = InputActionsContainer.FindAction(name);
            if (action == null) throw new Exception($"Action {name} not found");
            setter.SetValue(this, action);
        });
    }

    public void Redefine(InputAction newInputAction) {
        if (InputActionsContainer == null) return;
        var suffix = _playerMapping.Player.ToString();
        var name = $"{newInputAction.Name}/{suffix}";
        var found = InputActionsContainer!.InputActionList.Find(i => i.Name == name);
        if (found is InputAction inputAction) {
            inputAction.Update(updater => UpdateInputAction(newInputAction, updater));
        } else {
            throw new Exception($"Action not found: {newInputAction.Name}");
        }
    }

    private void UpdateInputAction(InputAction fromInputAction, InputAction.Updater newInputAction) {
        if (_playerMapping.Player == 0) {
            newInputAction.CopyAll(fromInputAction);
        } else {
            newInputAction.CopyJoypad(fromInputAction);
        }
        newInputAction.SetJoypadId(_playerMapping.JoypadId);
    }


    public void Disconnect() {
        _playerMapping.OnJoypadIdChanged -= JoypadIdChanged;
        InputActionsContainer?.Disable();
        InputActionsContainer?.QueueFree();
        InputActionsContainer = null;
    }
}