using System;
using System.Collections.Generic;
using Betauer.Memory;
using Godot;
using Array = Godot.Collections.Array;
using Object = Godot.Object;

namespace Betauer.Animation.Tween {

    public class DefaultTweenCallbackManager {
        public static TweenCallbackManager Instance = new();
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

        public readonly Dictionary<int, List<Action<object>>> ActionsByTween = new();

        public int GetCallbackCount(SceneTreeTween sceneTreeTween) {
            return ActionsByTween.TryGetValue(sceneTreeTween.GetHashCode(), out var actionList) ? actionList.Count : 0;
        }

        public CallbackTweener TweenCallbackAction(SceneTreeTween sceneTreeTween, Action action) {
            void Wrapper(object _) => action(); 
            var binds = AddAction(sceneTreeTween, Wrapper);
            return sceneTreeTween.TweenCallback(this, nameof(_GodotTweenCallback), binds);
        }

        public MethodTweener TweenInterpolateAction<T>(SceneTreeTween sceneTreeTween, T @from, T to, float duration, Action<T> action) {
            void Wrapper(object value) => action((T)value); 
            var binds = AddAction(sceneTreeTween, Wrapper);
            return sceneTreeTween.TweenMethod(this, nameof(_GodotTweenMethod), @from, to, duration, binds);
        }

        private Array AddAction(SceneTreeTween sceneTreeTween, Action<object> action) {
            var sceneTreeTweenHash = sceneTreeTween.GetHashCode();
            var actionHash = action.GetHashCode();
            lock (ActionsByTween) {
                if (!ActionsByTween.TryGetValue(sceneTreeTweenHash, out var actionList)) {
                    actionList = ActionsByTween[sceneTreeTweenHash] = new List<Action<object>>();
                    Watcher.IfInvalidInstance(sceneTreeTween)
                        .Do(() => ActionsByTween.Remove(sceneTreeTweenHash), "Remove all callbacks");
                }
                actionList.Add(action);
            }
            return new Array { sceneTreeTweenHash, actionHash };
        }

        private void _GodotTweenMethod(object value, int sceneTreeTweenHash, int actionHash) {
            if (ActionsByTween.TryGetValue(sceneTreeTweenHash, out var actions)) {
                var action = actions.Find(action => action.GetHashCode() == actionHash);
                action?.Invoke(value);
            }
        }

        private void _GodotTweenCallback(int sceneTreeTweenHash, int actionHash) {
            if (ActionsByTween.TryGetValue(sceneTreeTweenHash, out var actions)) {
                var action = actions.Find(action => action.GetHashCode() == actionHash);
                action?.Invoke(null);
            }
        }
    }
}