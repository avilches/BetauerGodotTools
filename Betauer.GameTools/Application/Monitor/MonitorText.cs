using System;
using Godot;

namespace Betauer.Application.Monitor {
    public class MonitorText : BaseMonitor<MonitorText> {

        
        private string _text = string.Empty;
        private readonly HBoxContainer _container = new();
        private Func<string>? _showValue;

        public readonly Label Label = new();
        public readonly Label Content = new();

        public MonitorText SetLabel(string? label) {
            Label.Text = label;
            Label.Visible = !string.IsNullOrWhiteSpace(label);
            return this;
        }

        public MonitorText Show(Func<string> action) {
            _showValue = action;
            return this;
        }

        public MonitorText Show(Func<bool> action) {
            var previous = false;
            Content.AddColorOverride("font_color", Colors.Tomato);
            _showValue = () => {
                var b = action();
                if (b != previous) {
                    // Change this in every frame just kills the fps
                    Content.AddColorOverride("font_color", b ? Colors.GreenYellow : Colors.Tomato);
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
            Label.AddColorOverride("font_color", DefaultLabelColor);

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