using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core;
using Betauer.Tools.FastReflection;

namespace Betauer.Input.Joypad;

public class JoypadController {
    public InputActionsContainer InputActionsContainer { get; private set; }
    private InputActionsContainer _source;
    private PlayerMapping _playerMapping;
    private Action<InputAction, InputAction.Updater> _updater;

    private List<ISetter>? _fastSetters;

    protected List<ISetter> FastSetters => _fastSetters ??=
        GetType().GetProperties()
            .Where(p => p.PropertyType.ImplementsInterface(typeof(IAction)))
            .Select(p => new FastSetter(p) as ISetter)
            .ToList();
    
    public void Configure(PlayerMapping playerMapping, InputActionsContainer source, Action<InputAction, InputAction.Updater> updater) {
        _playerMapping = playerMapping;
        _source = source;
        _updater = updater; 
        Reconnect();
        playerMapping.OnJoypadChanged += Reconnect;
    }
    
    private void Reconnect() {
        Disconnect();
        var suffix = _playerMapping.Player.ToString();
        InputActionsContainer = _source.Clone(_playerMapping.JoypadId, suffix, _updater);
        InputActionsContainer.Enable();
        
        FastSetters.ForEach(setter => {
            var name = $"{setter.Name}/{suffix}";
            var action = InputActionsContainer.FindAction(name);
            if (action == null) throw new Exception($"Action {name} not found");
            setter.SetValue(this, action);
        });
    }

    public void Disconnect() {
        InputActionsContainer?.Disable();
        InputActionsContainer?.QueueFree();
        InputActionsContainer = null;
    }
}