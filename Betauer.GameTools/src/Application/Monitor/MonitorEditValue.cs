using System;
using Betauer.Core.Nodes;
using Betauer.UI;
using Godot;

namespace Betauer.Application.Monitor;

public partial class MonitorEditValue : BaseMonitor<MonitorEditValue> {
    private const bool MultiLineDefault = false;
    private Action<string>? _updateValue;
    public readonly HBoxContainer HBoxContainer = new();
    public Func<string>? ValueLoader;
    private bool _focused = false;
    private string _previousValue = null!;
    private bool _multiLine = MultiLineDefault;

    public readonly Label Label = new() {
        Name = "Label",
        Visible = false
    };

    public readonly LineEdit LineEdit = new() {
        Name = "Edit",
        Visible = !MultiLineDefault,
    };

    public readonly TextEdit TextEdit = new() {
        Name = "TextEdit",
        Visible = MultiLineDefault,
    };
    
    public MonitorEditValue SetLabel(string? label) {
        Label.Text = label;
        Label.Visible = !string.IsNullOrWhiteSpace(label);
        return this;
    }
    
    public MonitorEditValue SetMultiLine(bool multiLine) {
        _multiLine = multiLine;
        if (multiLine) {
            LineEdit.Hide();
            TextEdit.Show();
        } else {
            LineEdit.Show();
            TextEdit.Hide();
        }
        return this;
    }

    public MonitorEditValue OnUpdate(Action<string> updateValue) {
        _updateValue = updateValue;
        return this;
    }

    public MonitorEditValue SetMinHeight(int minHeight) {
        LineEdit.CustomMinimumSize = new Vector2(LineEdit.CustomMinimumSize.Y, minHeight);
        TextEdit.CustomMinimumSize = new Vector2(TextEdit.CustomMinimumSize.Y, minHeight);
        return this;
    }

    public MonitorEditValue SetMinWidth(int minWidth) {
        LineEdit.CustomMinimumSize = new Vector2(minWidth, LineEdit.CustomMinimumSize.Y);
        TextEdit.CustomMinimumSize = new Vector2(minWidth, TextEdit.CustomMinimumSize.Y);
        return this;
    }

    public override void _Ready() {
        if (ValueLoader != null) {
            SetValue(ValueLoader());
        }
        LineEdit.TextChanged += text => {
            if (_previousValue == text) return;
            _previousValue = text;
            try {
                _updateValue?.Invoke(text);
                LineEdit.RemoveFontColor();
            } catch (Exception e) {
                GD.PrintErr(e.Message);
                LineEdit.SetFontColor(DefaultErrorColor);
            }
        };
        TextEdit.TextChanged += () => {
            var text = TextEdit.Text;
            if (_previousValue == text) return;
            _previousValue = text;
            try {
                _updateValue?.Invoke(text);
                TextEdit.RemoveFontColor();
            } catch (Exception e) {
                GD.PrintErr(e.Message);
                TextEdit.SetFontColor(DefaultErrorColor);
            }
        };
        LineEdit.FocusEntered += () => _focused = true;
        LineEdit.FocusExited += () => _focused = false;
        TextEdit.FocusEntered += () => _focused = true;
        TextEdit.FocusExited += () => _focused = false;
        this.Children()
            .Add(HBoxContainer, box => {
                box.Children()
                    .Add(Label)
                    .Add(LineEdit)
                    .Add(TextEdit);
            });
    }
    
    public override void UpdateMonitor(double delta) {
        if (!_focused && ValueLoader != null) {
            var value = ValueLoader();
            if (_multiLine) {
                TextEdit.Text = value;
            } else {
                LineEdit.Text = value;
            }
            _previousValue = value;
        }
    }

    public MonitorEditValue SetValueLoader(Func<string> valueLoader) {
        ValueLoader = valueLoader;
        return this;
    }

    public MonitorEditValue SetValue(string value) {
        TextEdit.Text = value;
        LineEdit.Text = value;
        _previousValue = value;
        return this;
    }
}