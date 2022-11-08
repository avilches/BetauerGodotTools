using System;
using Betauer.Tools.Logging;
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

        public float Start(Tween sceneTreeTween, float initialDelay, Node target) {
            if (!Object.IsInstanceValid(sceneTreeTween)) {
                Logger.Debug("Can't start a " + nameof(CallbackTweener) + " from a freed tween instance");
                return 0;
            }
            var start = _delay + initialDelay;
            Logger.Debug("Adding anonymous callback with " + _delay + "s delay. Scheduled: " + start.ToString("F"));
            var callbackTweener = sceneTreeTween
                .Parallel()
                .TweenCallbackAction(() =>_callback(target))
                .SetDelay(start);
            return _delay;
        }
    }
}