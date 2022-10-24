using System;
using Betauer.Signal;
using Godot;

namespace Betauer.UI {
    public class ButtonBar {
        public static ButtonBar Create(Theme? theme = null) => new(theme);
        private readonly HBoxContainer _container = new();
        private readonly Theme? _theme;
        private ButtonBar(Theme? theme) {
            _theme = theme;
        }

        public ButtonBar Button(string label, Action action) {
            var b = new Button();
            b.Text = label;
            b.OnPressed(action);
            _container.AddChild(b);
            return this;
        }

        public ButtonBar Add(Control control) {
            _container.AddChild(control);
            return this;
        }

        public HBoxContainer Build() {
            _container.Theme = _theme;
            return _container;
        }
    }
}