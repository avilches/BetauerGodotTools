using System;
using Godot;
using Object = Godot.Object;

namespace Betauer.Animation.Tween {
    public class CallbackNodeTweener : ITweener {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(CallbackNodeTweener));
        private readonly Action<Node> _callback;
        private readonly float _delay;

        public bool IsCompatibleWith(Node node) {
            return true;
        }

        internal CallbackNodeTweener(float delay, Action<Node> callback) {
            _delay = delay;
            _callback = callback;
        }

        public float Start(SceneTreeTween sceneTreeTween, float initialDelay, Node target) {
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
                .TweenCallbackAction(() =>_callback(target))
                .SetDelay(start);
            return _delay;
        }
    }
}