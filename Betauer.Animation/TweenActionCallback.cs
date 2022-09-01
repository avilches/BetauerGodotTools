using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Array = Godot.Collections.Array;
using Object = Godot.Object;

namespace Betauer.Animation {

    public class DefaultTweenCallbackManager {
        public static TweenCallbackManager Instance = new TweenCallbackManager();
    }
    
    public static class SceneTreeTweenExtensions {
        public static MethodTweener TweenInterpolateAction<T>(this SceneTreeTween sceneTreeTween, T @from, T to, float duration, Action<T> action) {
            return DefaultTweenCallbackManager.Instance.TweenInterpolateAction(sceneTreeTween, @from, to, duration, action);
        }

        public static Godot.CallbackTweener TweenCallbackAction(this SceneTreeTween sceneTreeTween, Action action) {
            return DefaultTweenCallbackManager.Instance.TweenCallbackAction(sceneTreeTween, action);
        }
    }
    
    /// <summary>
    /// A Tween compatible with C# lambdas, aka, Actions.
    /// Instead of Tween.InterpolateCallback(object, 0f, "method") you can use:
    /// TweenCallbackAction(0f, () => {})
    ///
    /// Instead of Tween.InterpolateMethod(object, "method", 0, 12, ...) you can use:
    /// InterpolateAction(0, 12, ..., (val) => { })
    /// 
    /// </summary>
    public class TweenCallbackManager : Object {
        private readonly Dictionary<int, Action> _actions = new Dictionary<int, Action>();
        private readonly HashSet<Object> _interpolateMethodActionSet = new HashSet<Object>();

        public Godot.CallbackTweener TweenCallbackAction(SceneTreeTween sceneTreeTween, Action callback) {
            var actionId = callback.GetHashCode();
            _actions[actionId] = callback;
            // TODO: Use IObjectWatcher
            var callbackTweener = sceneTreeTween
                .TweenCallback(this, nameof(GodotTweenCallbackMethod), new Array { actionId });
            return callbackTweener;
        }

        private void GodotTweenCallbackMethod(int actionId) {
            Action action = _actions[actionId];
            action.Invoke();
        }

        public MethodTweener TweenInterpolateAction<T>(SceneTreeTween sceneTreeTween, T @from, T to, float duration, Action<T> action) {
            var actionWrapper = new InterpolateMethodAction<T>(action, _interpolateMethodActionSet);
            _interpolateMethodActionSet.Add(actionWrapper);
            return sceneTreeTween
                .TweenMethod(actionWrapper, nameof(InterpolateMethodAction<T>.CallFromGodot), @from, to, duration);
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
        }
    }
}