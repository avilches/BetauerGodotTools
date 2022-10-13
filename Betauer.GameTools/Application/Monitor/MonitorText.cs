using System;
using Godot;

namespace Betauer.Application.Monitor {
    public class MonitorText : BaseMonitor {

        public static readonly Color DefaultLabelModulateColor = new Color(0.584314f, 0.584314f, 0.584314f, 1);
        
        private string _text = string.Empty;
        private readonly HBoxContainer _container = new();
        private Func<string>? _showValue;

        public readonly Label Label = new();
        public readonly Label Content = new();

        public MonitorText RemoveIfInvalid(Godot.Object target) {
            NodeToFollow = target;
            return this;
        }

        public MonitorText SetLabel(string? label) {
            if (label == null) {
                Label.Visible = false;
            } else {
                Label.Text = label;
                Label.Visible = true;
            }
            return this;
        }

        public MonitorText Show(Func<string> action) {
            _showValue = action;
            return this;
        }

        public MonitorText Show(Func<bool> action) {
            var previous = false;
            Content.Modulate = Colors.Tomato;
            _showValue = () => {
                var b = action();
                if (b != previous) {
                    // Change this in every frame just kills the fps
                    Content.Modulate = b ? Colors.GreenYellow : Colors.Tomato;
                }
                previous = b;
                return b.ToString();
            };
            return this;
        }

        public MonitorText Show(string? text) {
            _text = text ?? string.Empty;
            return this;
        }

        public override void _Ready() {
            Label.Visible = false;
            Label.Modulate = DefaultLabelModulateColor;

            _container.AddChild(Label);
            _container.AddChild(Content);
            AddChild(_container);
        }

        public string GetText() {
            try {
                return _showValue != null ? _showValue.Invoke() : _text;
            } catch (Exception e) {
                return e.Message;
            }
        }

        public override void Process(float delta) {
            Content.Text = GetText();
        }
    }
}