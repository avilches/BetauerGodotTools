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
    public class TweenCallbackManager : Object {

        public readonly Dictionary<int, List<Action<object>>> ActionsByTween = new Dictionary<int, List<Action<object>>>();

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
            var sceneTreeTweenId = sceneTreeTween.GetHashCode();
            var actionId = action.GetHashCode();
            lock (ActionsByTween) {
                if (!ActionsByTween.TryGetValue(sceneTreeTweenId, out var actionList)) {
                    actionList = ActionsByTween[sceneTreeTweenId] = new List<Action<object>>();
                    new WatchSceneTreeTweenCallbacks(this)
                        .Watch(sceneTreeTween)
                        .AddToDefaultObjectWatcher();
                }
                actionList.Add(action);
            }
            return new Array { sceneTreeTweenId, actionId };
        }


        private void _GodotTweenMethod(object value, int sceneTreeTweenId, int actionId) {
            Action<object> action = ActionsByTween[sceneTreeTweenId].Find(action => action.GetHashCode() == actionId);
            action.Invoke(value);
        }

        private void _GodotTweenCallback(int sceneTreeTweenId, int actionId) {
            Action<object> action = ActionsByTween[sceneTreeTweenId].Find(action => action.GetHashCode() == actionId);
            action.Invoke(null);
        }

        public int GetCallbackCount(SceneTreeTween sceneTreeTween) {
            return ActionsByTween.TryGetValue(sceneTreeTween.GetHashCode(), out var actionList) ? actionList.Count : 0;
        }

        // When a SceneTreeTween becomes invalid (IsValid() returns true), this method will be called from the 
        // DefaultObjectWatcherRunner
        public void RemoveActions(SceneTreeTween sceneTreeTweenId) {
            ActionsByTween.Remove(sceneTreeTweenId.GetHashCode());
        }

        private class WatchSceneTreeTweenCallbacks : IObjectConsumer {
            private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(Consumer));
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
                    #if DEBUG
                    Logger.Debug($"Consumed: {_sceneTreeTweenWatching.ToStringSafe()} {_tweenCallbackManager.GetCallbackCount(_sceneTreeTweenWatching)} actions");
                    #endif
                    _tweenCallbackManager.RemoveActions(_sceneTreeTweenWatching);
                    return true;
                } 
                return false;
            }
        
            public WatchSceneTreeTweenCallbacks AddToDefaultObjectWatcher() {
                DefaultObjectWatcherTask.Instance.Add(this);
                return this;
            }

            public override string ToString() {
                return $"Watching: {_sceneTreeTweenWatching.ToStringSafe()}";
            }
        }
        
    }
}