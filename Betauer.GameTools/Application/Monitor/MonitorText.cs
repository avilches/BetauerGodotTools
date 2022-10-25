using System;
using Betauer.UI;
using Godot;

namespace Betauer.Application.Monitor {
    public class MonitorText : BaseMonitor<MonitorText> {
        private float _timeElapsed = 0;
        private float _time = 0;
        private Func<string>? _showValue;
        public readonly HBoxContainer HBoxContainer = new();

        public readonly Label Label = new() {
            Name = "Label",
            Visible = false
        };

        public readonly Label Content = new() {
            Name = "Content"
        };

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

        public MonitorText UpdateEvery(float time) {
            _time = Math.Max(time, 0);
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
            Content.Text = text ?? string.Empty;
            return this;
        }

        public override void _Ready() {
            Label.AddColorOverride("font_color", DefaultLabelColor);
            this.NodeBuilder()
                .Child(HBoxContainer)
                    .Child(Label).End()
                    .Child(Content).End()
                .End();
        }

        public override void Process(float delta) {
            if (_showValue != null) {
                _timeElapsed += delta;
                if (_timeElapsed >= _time) {
                    Content.Text = _showValue.Invoke();
                    _timeElapsed -= _time;
                }
            }
        }
    }
}