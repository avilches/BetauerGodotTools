using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Object = Godot.Object;

namespace Betauer.Animation {
    public class ActionTween : Tween {
        private readonly Dictionary<int, Action> _actions = new Dictionary<int, Action>();
        private readonly Dictionary<Object, Object> _objects = new Dictionary<Object, Object>();
        public const float ExtraDelayToFinish = 0.01f;

        public void ScheduleCallback(float delay, Action callback) {
            var actionHashCode = callback.GetHashCode();
            _actions[actionHashCode] = callback;
            InterpolateCallback(this, delay, nameof(ActionTweenCallback), actionHashCode);
            Start();
        }

        public void ActionTweenCallback(int actionHashCode) {
            Action action = _actions[actionHashCode];
            _actions.Remove(actionHashCode);
            action.Invoke();
        }

        internal void InterpolateAction<T>(T @from, T to, float duration, TransitionType transitionType,
            EaseType easeType, float delay, Action<T> action) {
            var actionWrapper = new ActionWrapper<T>(action, _objects);
            _objects[actionWrapper] = actionWrapper;
            InterpolateMethod(actionWrapper, nameof(ActionWrapper<T>.CallFromGodot), @from, to, duration,
                transitionType, easeType, delay);
            ScheduleCallback(delay + duration + ExtraDelayToFinish, actionWrapper.Finish);
        }

        public List<Object> GetPendingObjects() {
            return _objects.Values.ToList();
        }

        public List<Action> GetPendingActions() {
            return _actions.Values.ToList();
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            if (disposing) {
                foreach (var @object in _objects.Values) {
                    try {
                        @object.Free();
                    } catch (Exception e) {
                        Console.WriteLine(e);
                    }
                }
            }
        }

        public class ActionWrapper<T> : Object {
            private readonly Action<T> _action;
            private readonly Dictionary<Object, Object> _objects;

            public ActionWrapper(Action<T> action, Dictionary<Object, Object> objects) {
                _action = action;
                _objects = objects;
            }

            public void CallFromGodot(T value) {
                _action(value);
            }

            public void Finish() {
                _objects.Remove(this);
                Free();
            }
        }
    }
}