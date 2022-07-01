using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Godot;
using Object = Godot.Object;

namespace Betauer.Animation {
    /// <summary>
    /// A Tween compatible with C# lambdas, aka, Actions.
    /// Instead of Tween.InterpolateCallback(object, 0f, "method") you can use:
    /// ScheduleCallback(0f, () => {})
    ///
    /// Instead of Tween.InterpolateMethod(object, "method", 0, 12, ...) you can use:
    /// InterpolateAction(0, 12, ..., (val) => { })
    /// 
    /// </summary>
    public class TweenActionCallback : Tween {
        private readonly Dictionary<int, Action> _actions = new Dictionary<int, Action>();
        private readonly HashSet<Object> _interpolateMethodActionSet = new HashSet<Object>();
        public const float ExtraDelayToFinish = 0.01f;

        public void ScheduleCallback(float delay, Action callback) {
            var actionId = callback.GetHashCode();
            _actions[actionId] = callback;
            base.InterpolateCallback(this, delay, nameof(ActionTweenCallback), actionId);
        }

        private void ActionTweenCallback(int actionId) {
            Action action = _actions[actionId];
            _actions.Remove(actionId);
            action.Invoke();
        }

        public void InterpolateAction<T>(T @from, T to, float duration, TransitionType transitionType,
            EaseType easeType, float delay, Action<T> action) {
            var actionWrapper = new InterpolateMethodAction<T>(action, _interpolateMethodActionSet);
            _interpolateMethodActionSet.Add(actionWrapper);
            base.InterpolateMethod(actionWrapper, nameof(InterpolateMethodAction<T>.CallFromGodot), @from, to, duration,
                transitionType, easeType, delay);
            ScheduleCallback(delay + duration + ExtraDelayToFinish, actionWrapper.Finish);
        }

        public HashSet<Object> GetPendingObjects() {
            return _interpolateMethodActionSet;
        }

        public List<Action> GetPendingActions() {
            return _actions.Values.ToList();
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            if (disposing) {
                foreach (var @object in _interpolateMethodActionSet) {
                    try {
                        @object.Free();
                    } catch (Exception e) {
                        Console.WriteLine(e);
                    }
                }
            }
        }

        /// <summary>
        /// Temporal Godot object to allow be called with Tween.InterpolateMethod() which accepts only
        /// godot objects and the method name as string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class InterpolateMethodAction<T> : Object {
            private readonly Action<T> _action;
            private readonly HashSet<Object> _interpolateMethodActionSet;

            public InterpolateMethodAction(Action<T> action, HashSet<Object> interpolateMethodActionSet) {
                _action = action;
                _interpolateMethodActionSet = interpolateMethodActionSet;
            }

            public void CallFromGodot(T value) {
                _action(value);
            }

            public void Finish() {
                _interpolateMethodActionSet.Remove(this);
                Free();
            }
        }
    }
}