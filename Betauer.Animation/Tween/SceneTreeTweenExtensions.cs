using System;
using Godot;

namespace Betauer.Animation.Tween {
    public static class SceneTreeTweenExtensions {
        public static MethodTweener TweenInterpolateAction<T>(this Tween sceneTreeTween, T @from, T to, float duration, Action<T> action) {
            return DefaultTweenCallbackManager.Instance.TweenInterpolateAction(sceneTreeTween, @from, to, duration, action);
        }

        public static Godot.CallbackTweener TweenCallbackAction(this Tween sceneTreeTween, Action action) {
            return DefaultTweenCallbackManager.Instance.TweenCallbackAction(sceneTreeTween, action);
        }
    }
}