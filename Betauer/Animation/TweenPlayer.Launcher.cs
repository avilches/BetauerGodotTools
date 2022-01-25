using Godot;
using Object = Godot.Object;

namespace Betauer.Animation {
    public class Launcher : ITweenPlayer<Launcher> {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(Launcher));
        public DisposableTween DisposableTween { get; private set; }

        public Launcher() {
        }

        public Launcher(DisposableTween tween) {
            WithTween(tween);
        }

        public Launcher CreateNewTween(Node node) {
            var tween = new DisposableTween();
            node.AddChild(tween);
            return WithTween(tween);
        }

        public Launcher WithTween(DisposableTween tween) {
            DisposableTween = tween;
            return this;
        }

        public Launcher RemoveTween() {
            DisposableTween = null;
            return this;
        }

        public bool IsRunning() => DisposableTween.IsActive();

        /*
         * Flow:
         * 1 Start <-> Stop
         * 2 Reset keeps the current status. So, Start -> Reset will continue playing (but from beginning)
         */

        public Launcher Start() {
            if (!Object.IsInstanceValid(DisposableTween)) {
                Logger.Warning("Can't Start in a freed Tween instance");
                return this;
            }
            DisposableTween.Start();
            DisposableTween.ResumeAll();
            return this;
        }

        public Launcher Stop() {
            if (!Object.IsInstanceValid(DisposableTween)) {
                Logger.Warning("Can't Stop AnimationTweenPlayer in a freed Tween instance");
                return this;
            }
            Logger.Info("Tween.StopAll()");
            DisposableTween.StopAll();
            return this;
        }

        public Launcher Reset() {
            if (!Object.IsInstanceValid(DisposableTween)) {
                Logger.Warning("Can't Reset AnimationTweenPlayer in a freed Tween instance");
                return this;
            }
            Logger.Info("Tween.RemoveAll()");
            DisposableTween.RemoveAll();
            Logger.Info("Tween.Start()");
            DisposableTween.Start();
            return this;
        }

        public Launcher Kill() {
            if (!Object.IsInstanceValid(DisposableTween)) return this;
            Logger.Info("Tween.StopAll()");
            DisposableTween.StopAll();
            Logger.Info("Tween.Dispose()");
            DisposableTween.Dispose();
            return this;
        }

        public LoopStatus Play(ISequence sequence, Node? defaultTarget = null, float initialDelay = 0,
            float duration = -1) {
            var loops = sequence is ILoopedSequence loopedSequence ? loopedSequence.Loops : 1;
            return Play(loops, sequence, defaultTarget, initialDelay, duration);
        }

        public LoopStatus PlayForever(ISequence sequence, Node? defaultTarget = null, float initialDelay = 0,
            float duration = -1) {
            return Play(-1, sequence, defaultTarget, initialDelay, duration);
        }

        public LoopStatus Play(int loops, ISequence sequence, Node? defaultTarget = null, float initialDelay = 0,
            float duration = -1) {
            LoopStatus loopStatus = new LoopStatus(DisposableTween, loops, sequence, defaultTarget, duration);
            return loopStatus.Start(initialDelay);
        }
    }
}