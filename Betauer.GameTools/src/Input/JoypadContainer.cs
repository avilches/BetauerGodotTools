using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Tools.FastReflection;
using TypeExtensions = Betauer.Core.TypeExtensions;

namespace Betauer.Input;

public abstract class JoypadContainer {
    
    public InputActionsContainer JoypadActionsContainer { get; private set; }

    private List<ISetter>? _fastSetters;

    protected List<ISetter> FastSetters => _fastSetters ??=
        GetType().GetProperties()
            .Where(p => TypeExtensions.ImplementsInterface(p.PropertyType, typeof(IAction)))
            .Select(p => new FastSetter(p) as ISetter)
            .ToList();

    public virtual void Connect(int joypadId) {
        Disconnect();
        JoypadActionsContainer = Source.Clone(joypadId);
        FastSetters.ForEach(setter => {
            var name = $"{setter.Name}/{joypadId}";
            var action = JoypadActionsContainer.FindAction(name);
            if (action == null) throw new Exception($"Action {name} not found");
            setter.SetValue(this, action);
        });
        JoypadActionsContainer.Enable();
    }

    public virtual void Disconnect() {
        JoypadActionsContainer?.Disable();
        JoypadActionsContainer?.QueueFree();
        JoypadActionsContainer = null;
    }
    
    protected abstract InputActionsContainer Source { get; } 
}