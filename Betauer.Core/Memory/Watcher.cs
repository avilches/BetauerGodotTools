using System;
using System.Linq;
using Godot;
using Object = Godot.Object;

namespace Betauer.Core.Memory {
    public class Watcher : IObjectConsumer {
        private readonly Func<bool> _condition;
        public event Action? OnExecute;
        private string _debug = "";

        public static Watcher IfInvalidInstance(Object o) {
            return new Watcher(() => !Object.IsInstanceValid(o) ||
                                     (o is Tween tween && !tween.IsValid()))
                .Debug($"If not valid {o.ToStringSafe()} > ");
        }

        public static Watcher IfAllInvalidInstance(params Object[] os) {
            return new Watcher(() => os.All(o => !Object.IsInstanceValid(o)));
        }

        public static Watcher IfAnyInvalidInstance(params Object[] os) {
            return new Watcher(() => os.Any(o => !Object.IsInstanceValid(o)));
        }

        public Watcher(Func<bool> action) {
            _condition = action;
        }

        public Watcher Debug(string debug) {
            _debug += debug;
            return this;
        }

        public Watcher StartWatching() {
            DefaultObjectWatcherTask.Instance.Add(this);
            return this;
        }

        public Watcher StopWatching() {
            DefaultObjectWatcherTask.Instance.Remove(this);
            return this;
        }

        public Watcher Free(Object target, bool deferred = true) {
            return Do(() => {
                if (!Object.IsInstanceValid(target)) return;
                if (deferred) {
                    if (target is Node node) node.QueueFree();
                    else target.FreeDeferred();
                } else {
                    target.Free();
                }
            }, $"Free{(deferred ? " deferred: " : ": ")}{target.ToStringSafe()}");
        }

        public Watcher Do(Action? execute, string debug) {
            OnExecute += execute;
            return Debug(debug).StartWatching();
        }

        public override string ToString() {
            return $"Watcher: {_debug}";
        }

        public bool Consume(bool force) {
            if (force || _condition()) {
                OnExecute?.Invoke();
                return true;
            }
            return false;
        }
    }
}