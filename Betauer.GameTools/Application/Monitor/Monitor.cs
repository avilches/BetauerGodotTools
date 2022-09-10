using System;
using Godot;

namespace Betauer.Application.Monitor {
    public interface IMonitor {
        public bool IsEnabled { get; }
        public bool IsDestroyed { get; }
        public string GetText();
    }

    public class Monitor : IMonitor {
        private bool _isEnabled = true;
        private bool _isDestroyed = false;

        public string? Prefix;
        public Godot.Object? Target;
        public Func<string>? Delegate;

        public bool IsEnabled => _isEnabled && (Target is Node n ? n.IsInsideTree() : true);
        public bool IsDestroyed => _isDestroyed || (Target != null && !Godot.Object.IsInstanceValid(Target));

        public Monitor WithPrefix(string prefix) {
            Prefix = prefix;
            return this;
        }

        public Monitor Bind(Godot.Object node) {
            Target = node;
            return this;
        }

        public Monitor Show(Func<string> action) {
            Delegate = action;
            return this;
        }

        public string GetText() {
            var label = Prefix != null ? Prefix + ": " : "";
            try {
                return $"{label}{Delegate?.Invoke()}";
            } catch (Exception e) {
                return $"{label}{e.Message}";
            }
        }

        public void Disable() => _isEnabled = false;

        public void Enable() => _isEnabled = true;

        public void Destroy() => _isDestroyed = true;
    }
}