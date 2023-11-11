using System;
using Betauer.Core.Nodes;
using Betauer.UI;
using Godot;

namespace Betauer.Application.Monitor;

public partial class MonitorEditValue : BaseMonitor<MonitorEditValue> {
    private Action<string>? _updateValue;
    public readonly HBoxContainer HBoxContainer = new();
    public Func<string>? ValueLoader;
    private bool _focused = false;
    

    public readonly Label Label = new() {
        Name = "Label",
        Visible = false
    };

    public readonly LineEdit Edit = new() {
        Name = "Edit"
    };

    public MonitorEditValue SetLabel(string? label) {
        Label.Text = label;
        Label.Visible = !string.IsNullOrWhiteSpace(label);
        return this;
    }

    public MonitorEditValue SetMinSize(int minSize) {
        Edit.CustomMinimumSize = new Vector2(minSize, Edit.CustomMinimumSize.Y);
        return this;
    }

    public MonitorEditValue OnUpdate(Action<string> updateValue) {
        _updateValue = updateValue;
        return this;
    }

    public MonitorEditValue SetMinWidth(int minWidth) {
        Edit.CustomMinimumSize = new Vector2(minWidth, Edit.CustomMinimumSize.Y);
        return this;
    }

    public override void _Ready() {
        Edit.TextChanged += text => {
            try {
                _updateValue?.Invoke(text);
                Edit.RemoveFontColor();
            } catch (Exception e) {
                GD.PrintErr(e.Message);
                Edit.SetFontColor(DefaultErrorColor);
            }
        };
        Edit.FocusEntered += () => {
            _focused = true;
        };
        Edit.FocusExited += () => _focused = false;
        this.Children()
            .Add(HBoxContainer, box => {
                box.Children()
                    .Add(Label)
                    .Add(Edit);
            });
    }
    
    public void LoadValue() {
        if (ValueLoader != null) {
            Edit.Text = ValueLoader();
        }
    }

    public override void UpdateMonitor(double delta) {
        if (!_focused) LoadValue();
    }

    public MonitorEditValue SetValueLoader(Func<string> valueLoader) {
        ValueLoader = valueLoader;
        return this;
    }

    public MonitorEditValue SetValue(string value) {
        Edit.Text = value;
        return this;
    }
}