using Godot;

namespace Betauer.Animation.Tween {
    internal class PauseTweener : ITweener {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(PropertyTweener<>));
        private readonly float _delay;

        internal PauseTweener(float delay) {
            _delay = delay;
        }

        public bool IsCompatibleWith(Node node) {
            return true;
        }

        public float Start(SceneTreeTween sceneTreeTween, float initialDelay, Node? ignoredDefaultTarget) {
            var delayEndTime = _delay + initialDelay;
#if DEBUG
            Logger.Info("Adding a delay of " + _delay + "s. Scheduled from " + initialDelay.ToString("F") + "s to " +
                        delayEndTime.ToString("F") + "s");
#endif
            return _delay;
        }
    }
}