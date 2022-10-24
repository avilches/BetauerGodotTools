using System;
using Betauer.UI;
using Godot;

namespace Betauer.Application.Monitor {
    public class MonitorText : BaseMonitor<MonitorText> {

        
        private string _text = string.Empty;
        private readonly HBoxContainer _container = new();
        private Func<string>? _showValue;

        public readonly Label Label = new() {
            Visible = false
        };
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

        public MonitorText SetMinWidth(int minWidth) {
            Content.RectMinSize = new Vector2(minWidth, Content.RectMinSize.y);
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
            Label.AddColorOverride("font_color", DefaultLabelColor);
            this.Child(_container)
                    .Child(Label).End()
                    .Child(Content).End()
                .End();
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