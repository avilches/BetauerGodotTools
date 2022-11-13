using System;
using Betauer.Core.Nodes;
using Betauer.UI;
using Godot;

namespace Betauer.Application.Monitor {
    public partial class MonitorText : BaseMonitor<MonitorText> {
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
            Content.CustomMinimumSize = new Vector2(minWidth, Content.CustomMinimumSize.y);
            return this;
        }

        public MonitorText Show(Func<bool> action) {
            var previous = false;
            Content.SetFontColor(Colors.Tomato);
            _showValue = () => {
                var b = action();
                if (b != previous) {
                    // Change this in every frame just kills the fps
                    Content.SetFontColor(b ? Colors.GreenYellow : Colors.Tomato);
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
            this.NodeBuilder()
                .Child(HBoxContainer)
                    .Child(Label)
                        .Config(label => {
                            label.SetFontColor(DefaultLabelColor);
                        })
                    .End()
                    .Child(Content)
                    .End()
                .End();
        }

        public override void UpdateMonitor(double delta) {
            if (_showValue != null) {
                Content.Text = _showValue.Invoke();
            }
        }
    }
}