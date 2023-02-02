using System;
using Betauer.Core.Nodes;
using Betauer.UI;
using Godot;

namespace Betauer.Application.Monitor;

public partial class MonitorEditValue : BaseMonitor<MonitorEditValue> {
    private Action<string>? _updateValue;
    public readonly HBoxContainer HBoxContainer = new();

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

    public MonitorEditValue OnUpdate(Action<string> updateValue) {
        _updateValue = updateValue;
        return this;
    }

    public MonitorEditValue SetMinWidth(int minWidth) {
        Edit.CustomMinimumSize = new Vector2(minWidth, Edit.CustomMinimumSize.y);
        return this;
    }

    public override void _Ready() {
        Edit.TextSubmitted += text => {
            try {
                _updateValue?.Invoke(text);
                Edit.RemoveFontColor();
                Edit.ReleaseFocus();
            } catch (Exception e) {
                GD.PrintErr(e.Message);
                Edit.SetFontColor(DefaultErrorColor);
            }
        };
        this.NodeBuilder()
            .Child(HBoxContainer)
                .Child(Label)
                    .Config(label => {
                        label.SetFontColor(DefaultLabelColor);
                    })
                .End()
                .Child(Edit)
                .End()
            .End();
    }

    public override void UpdateMonitor(double delta) {
    }

    public MonitorEditValue SetValue(string value) {
        Edit.Text = value;
        return this;
    }
}