using System;
using System.Linq;
using Godot;
using Object = Godot.Object;

namespace Betauer.Memory {

    public class Watcher : IObjectConsumer {
        private readonly Func<bool> _condition;
        public event Action? OnExecute;

        public static Watcher IfInvalidInstance(Object o) {
            return new Watcher(() => !Object.IsInstanceValid(o)).StartWatching();
        }

        public static Watcher IfAllInvalidInstance(params Object[] os) {
            return new Watcher(() => os.All(o => !Object.IsInstanceValid(o))).StartWatching();
        }

        public static Watcher IfAnyInvalidInstance(params Object[] os) {
            return new Watcher(() => os.Any(o => !Object.IsInstanceValid(o))).StartWatching();
        }

        public static Watcher IfInvalidTween(SceneTreeTween sceneTreeTween) {
            return new Watcher(() => !sceneTreeTween.IsValid());
        }

        public Watcher(Func<bool> action) {
            _condition = action;
        }

        public Watcher StartWatching() {
            DefaultObjectWatcherTask.Instance.Add(this);
            return this;
        }

        public Watcher StopWatching() {
            DefaultObjectWatcherTask.Instance.Remove(this);
            return this;
        }

        public Watcher Do(Action? execute) {
            OnExecute += execute;
            return this;
        }
        
        public Watcher Free(Object target, bool deferred = true) {
            return Do(() => {
                if (!Object.IsInstanceValid(target)) return;
                if (deferred) {
                    if (target is Node node) node.QueueFree();
                    else target.CallDeferred("free");
                } else {
                    target.Free();
                }
            });
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