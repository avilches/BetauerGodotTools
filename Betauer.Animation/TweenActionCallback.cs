using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Object = Godot.Object;

namespace Betauer.Animation {
    public class TweenActionCallback : Tween {
        private readonly Dictionary<string, Action> _actions = new Dictionary<string, Action>();
        private readonly Dictionary<Object, Object> _objects = new Dictionary<Object, Object>();
        public const float ExtraDelayToFinish = 0.01f;

        private static readonly long StartTick = DateTime.UtcNow.Ticks;

        public void ScheduleCallback(float delay, Action callback) {
            var actionId = (DateTime.Now.Ticks - StartTick).ToString();
            _actions[actionId] = callback;
            InterpolateCallback(this, delay, nameof(ActionTweenCallback), actionId);
        }

        private void ActionTweenCallback(string actionId) {
            Action action = _actions[actionId];
            _actions.Remove(actionId);
            action.Invoke();
        }

        public void InterpolateAction<T>(T @from, T to, float duration, TransitionType transitionType,
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