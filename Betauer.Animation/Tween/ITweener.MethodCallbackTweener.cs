using Betauer.Tools.Logging;
using Godot;

namespace Betauer.Animation.Tween {
    public class MethodCallbackTweener : ITweener {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(ITweener));
        private readonly string _methodName;
        private readonly float _delay;
        private readonly object[]? _binds;
        private readonly Node? _target;

        public bool IsCompatibleWith(Node node) {
            return true;
        }

        public MethodCallbackTweener(float delay, Node? target, string methodName, params object[] binds) {
            _target = target;
            _methodName = methodName;
            _delay = delay;
            _binds = binds;
        }

        public float Start(SceneTreeTween sceneTreeTween, float initialDelay, Node target) {
            if (!Object.IsInstanceValid(sceneTreeTween)) {
                Logger.Warning("Can't start a " + nameof(MethodCallbackTweener) + " from a freed tween instance");
                return 0;
            }
            if (!Object.IsInstanceValid(target)) {
                Logger.Warning("Can't start a " + nameof(MethodCallbackTweener) + " using a freed target instance");
                return 0;
            }
            var start = _delay + initialDelay;
            Logger.Debug("Adding method callback " + _methodName + "with " + _delay + "s delay. Scheduled: " +
                         start.ToString("F"));
            var methodTweener = sceneTreeTween
                .Parallel()
                .TweenCallback(_target ?? target, _methodName,
                    _binds != null && _binds.Length > 0 ? new Godot.Collections.Array(_binds) : null)
                .SetDelay(start);
            return _delay;
        }
    }
}