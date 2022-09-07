using System;
using Betauer.Nodes.Property;
using Godot;

namespace Betauer.Animation.Tween {
    public class CallbackProperty<TProperty> : IProperty<TProperty> {
        private readonly Action<TProperty> _action;

        public CallbackProperty(Action<TProperty> action) {
            _action = action;
        }

        public TProperty GetValue(Node node) {
            return default;
        }

        public void SetValue(Node node, TProperty value) {
            _action.Invoke(value);
        }

        public bool IsCompatibleWith(Node node) {
            return true;
        }

        public override string ToString() {
            return $"CallbackProperty<{typeof(TProperty).Name}>";
        }
    }
}