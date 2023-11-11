using System;
using Godot;

namespace Betauer.UI;

public static class NodeBuilderExtensions {
    public static NodeBuilder Children<TParent>(this TParent parent) where TParent : Node {
        return NodeBuilder.Children(parent);
    }

    public static NodeBuilder Button(this NodeBuilder builder, string label, Action? pressed = null, Action<Button>? config = null) {
        return Button<Button>(builder, label, pressed, config);
    }

    public static NodeBuilder Button<TButton>(this NodeBuilder builder, string label, Action? pressed = null, Action<TButton>? config = null) where TButton : Button {
        var b = Activator.CreateInstance<TButton>();
        if (pressed != null) b.Pressed += pressed;
        b.Text = label;
        return builder.Add(b, config);
    }

    public static NodeBuilder Label(this NodeBuilder builder, string label) {
        return builder.Add(new Label {
            Text = label
        });
    }

    public static NodeBuilder ToggleButton(this NodeBuilder builder, string label, Func<bool> isPressed, Action? pressed = null,
        ButtonGroup? group = null, Action<Button>? config = null) {
        return ToggleButton<Button>(builder, label, isPressed, pressed, group, config);
    }

    public static NodeBuilder ToggleButton<TButton>(this NodeBuilder builder, string label, Func<bool> isPressed, Action? pressed = null,
        ButtonGroup? group = null, Action<TButton>? config = null) where TButton : Button {
        var b = Activator.CreateInstance<TButton>();
        b.ToggleMode = true;
        b.Text = label;
        b.ButtonGroup = group;
        if (pressed != null) b.Pressed += pressed;
        b.Ready += () => b.SetPressedNoSignal(isPressed());
        config?.Invoke(b);
        return builder.Add(b);
    }
}