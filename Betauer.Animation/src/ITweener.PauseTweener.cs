using Betauer.Tools.Logging;
using Godot;

namespace Betauer.Animation {
    public class PauseTweener : ITweener {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(PauseTweener));
        private readonly float _delay;

        internal PauseTweener(float delay) {
            _delay = delay;
        }

        public bool IsCompatibleWith(Node node) {
            return true;
        }

        public float Start(Tween sceneTreeTween, float initialDelay, Node? ignoredDefaultTarget) {
            var delayEndTime = _delay + initialDelay;
            Logger.Debug("Adding a delay of " + _delay + "s. Scheduled from " + initialDelay.ToString("F") + "s to " +
                        delayEndTime.ToString("F") + "s");
            return _delay;
        }
    }
}