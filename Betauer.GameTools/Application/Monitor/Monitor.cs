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
        private string _text = string.Empty;

        public string? Prefix;
        public Godot.Object? Target;
        public Func<string>? Delegate;

        public bool IsEnabled => _isEnabled && (Target is not Node n || n.IsInsideTree());
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

        public void SetText(string text) {
            _text = text;
        }

        public void Enable(bool enable = true) {
            _isEnabled = enable;
        }

        public void Disable() {
            Enable(false);
        }

        public void Destroy() {
            _isDestroyed = true;
        }

        public string GetText() {
            try {
                var text = Delegate != null ? Delegate.Invoke() : _text;
                return Prefix != null ? $"{Prefix}: {text}" : text;
            } catch (Exception e) {
                return Prefix != null ? $"{Prefix}: {e.Message}" : e.Message;
            }
        }

    }
}