using Betauer.Tools.Logging;
using Godot;

namespace Betauer.Animation {
    public class PauseTweener : ITweener {
        private static readonly Logger Logger = LoggerFactory.GetLogger<PauseTweener>();
        private readonly float _delay;

        internal PauseTweener(float delay) {
            _delay = delay;
        }

        public bool IsCompatibleWith(Node node) {
            return true;
        }

        public float Start(Tween sceneTreeTween, float initialDelay, Node? ignoredDefaultTarget) {
            var delayEndTime = _delay + initialDelay;
            Logger.Debug("Adding a delay of {0}s. Scheduled from {1:F}s to {2:F}s", _delay, initialDelay, delayEndTime);
            return _delay;
        }
    }
}