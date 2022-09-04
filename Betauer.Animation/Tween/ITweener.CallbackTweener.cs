using System;
using Godot;
using Object = Godot.Object;

namespace Betauer.Animation.Tween {
    internal class CallbackTweener : ITweener {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(PropertyTweener<>));
        private readonly Action _callback;
        private readonly float _delay;

        internal CallbackTweener(float delay, Action callback) {
            _delay = delay;
            _callback = callback;
        }

        public float Start(SceneTreeTween sceneTreeTween, float initialDelay, Node ignoredTarget,
            float ignoredDuration) {
            if (!Object.IsInstanceValid(sceneTreeTween)) {
#if DEBUG
                Logger.Warning("Can't start a " + nameof(CallbackTweener) + " from a freed tween instance");
#endif
                return 0;
            }
            var start = _delay + initialDelay;
#if DEBUG
            Logger.Info("Adding anonymous callback with " + _delay + "s delay. Scheduled: " + start.ToString("F"));
#endif
            var callbackTweener = sceneTreeTween
                .Parallel()
                .TweenCallbackAction(_callback)
                .SetDelay(start);
            return _delay;
        }
    }
}