using System;
using System.Collections.Generic;
using Betauer.Memory;
using Godot;
using Array = Godot.Collections.Array;
using Object = Godot.Object;

namespace Betauer.Animation.Tween {

    public class DefaultTweenCallbackManager {
        public static TweenCallbackManager Instance = new TweenCallbackManager();
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
    public class TweenCallbackManager : DisposableGodotObject {
        private const string GodotTweenCallbackMethodName = nameof(GodotTweenCallbackMethod);

        public readonly Dictionary<int, List<Action>> ActionsPerSceneTree =
            new Dictionary<int, List<Action>>();

        public Godot.CallbackTweener TweenCallbackAction(SceneTreeTween sceneTreeTween, Action callback) {
            var sceneTreeTweenId = sceneTreeTween.GetHashCode();
            var actionId = callback.GetHashCode();
            if (!ActionsPerSceneTree.TryGetValue(sceneTreeTweenId, out var actionList)) {
                actionList = ActionsPerSceneTree[sceneTreeTweenId] = new List<Action>();
                new WatchSceneTreeTweenCallbacks(this)
                    .Watch(sceneTreeTween)
                    .AddToDefaultObjectWatcher();
            }
            actionList.Add(callback);
            var binds = new Array { sceneTreeTweenId, actionId };
            return sceneTreeTween.TweenCallback(this, GodotTweenCallbackMethodName, binds);
        }

        private void GodotTweenCallbackMethod(int sceneTreeTweenId, int actionId) {
            Action action = ActionsPerSceneTree[sceneTreeTweenId].Find(action => action.GetHashCode() == actionId);
            action.Invoke();
        }

        // When a SceneTreeTween becomes invalid (IsValid() returns true), this method will be called from the 
        // DefaultObjectWatcherRunner
        public void RemoveActions(SceneTreeTween sceneTreeTweenId) {
            ActionsPerSceneTree.Remove(sceneTreeTweenId.GetHashCode());
        }

        public MethodTweener TweenInterpolateAction<T>(SceneTreeTween sceneTreeTween, T @from, T to, float duration, Action<T> action) {
            var interpolateMethodAction = new InterpolateMethodAction<T>(action);
            new WatchTweenAndFree()
                .Watch(sceneTreeTween)
                .Free(interpolateMethodAction)
                .AddToDefaultObjectWatcher();
            return sceneTreeTween
                .TweenMethod(interpolateMethodAction, nameof(InterpolateMethodAction<T>.CallFromGodot), @from, to, duration);
        }

        /// <summary>
        /// Temporal Godot object to allow be called with SceneTreeTween.TweenMethod() which accepts only
        /// godot objects and the method name as string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class InterpolateMethodAction<T> : DisposableGodotObject {
            private readonly Action<T> _action;

            public InterpolateMethodAction(Action<T> action) {
                _action = action;
            }

            public void CallFromGodot(T value) {
                _action(value);
            }
        }
        
        private class WatchSceneTreeTweenCallbacks : IObjectConsumer {
            private readonly TweenCallbackManager _tweenCallbackManager;
            private SceneTreeTween _sceneTreeTweenWatching;

            public WatchSceneTreeTweenCallbacks(TweenCallbackManager tweenCallbackManager) {
                _tweenCallbackManager = tweenCallbackManager;
            }

            public WatchSceneTreeTweenCallbacks Watch(SceneTreeTween sceneTreeTweenWatching) {
                _sceneTreeTweenWatching = sceneTreeTweenWatching;
                return this;
            }

            public bool Consume(bool force) {
                if (force || !_sceneTreeTweenWatching.IsValid()) {
                    _tweenCallbackManager.RemoveActions(_sceneTreeTweenWatching);
                    return true;
                } 
                return false;
            }
        
            public WatchSceneTreeTweenCallbacks AddToDefaultObjectWatcher() {
                DefaultObjectWatcherRunner.Instance.Add(this);
                return this;
            }

            public override string ToString() {
                return "Watching: " + (IsInstanceValid(_sceneTreeTweenWatching) ? _sceneTreeTweenWatching.ToString(): "(disposed)");
            }
        }
        
    }
}