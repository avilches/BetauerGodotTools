using System;
using Godot;
using Object = Godot.Object;

namespace Betauer.Animation {
    public class SequencePlayer : ITweenPlayer<SequencePlayer> {
        public class SequencePlayerWithSequenceScheduler : RegularSequenceBuilder<SequencePlayerWithSequenceScheduler> {
            private readonly SequencePlayer _sequencePlayer;

            internal SequencePlayerWithSequenceScheduler(SequencePlayer sequencePlayer,
                bool createEmptyTweenList) : base(createEmptyTweenList) {
                _sequencePlayer = sequencePlayer;
            }

            public SequencePlayer EndSequence() {
                return _sequencePlayer;
            }

            public SequencePlayer Play(float initialDelay = 0, float duration = -1) {
                return _sequencePlayer.Execute(this, DefaultTarget, initialDelay, duration);
            }
        }
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(SequencePlayer));

        public Tween Tween { get; private set; }

        public SequencePlayer() {
        }

        public SequencePlayer(Tween tween) {
            WithTween(tween);
        }

        public SequencePlayer CreateNewTween(Node node) {
            var tween = new Tween();
            node.AddChild(tween);
            return WithTween(tween);
        }

        public SequencePlayer WithTween(Tween tween) {
            Tween = tween;
            return this;
        }

        public SequencePlayer RemoveTween() {
            Tween = null;
            return this;
        }
        
        public bool IsRunning() => Tween.IsActive();

        /*
         * Flow:
         * 1 Start <-> Stop
         * 2 Reset keeps the current status. So, Start -> Reset will continue playing (but from beginning)
         */

        public SequencePlayer Start() {
            if (!Object.IsInstanceValid(Tween)) {
                Logger.Warning("Can't Start in a freed Tween instance");
                return this;
            }
            Tween.Start();
            Tween.ResumeAll();
            return this;
        }

        public SequencePlayer Stop() {
            if (!Object.IsInstanceValid(Tween)) {
                Logger.Warning("Can't Stop AnimationTweenPlayer in a freed Tween instance");
                return this;
            }
            Logger.Info("Tween.StopAll()");
            Tween.StopAll();
            return this;
        }

        public SequencePlayer Reset() {
            if (!Object.IsInstanceValid(Tween)) {
                Logger.Warning("Can't Reset AnimationTweenPlayer in a freed Tween instance");
                return this;
            }
            Logger.Info("Tween.StopAll()");
            Tween.StopAll();
            Logger.Info("Tween.RemoveAll()");
            Tween.RemoveAll();
            Logger.Info("Tween.Start()");
            Tween.Start();
            return this;
        }

        public SequencePlayer Kill() {
            if (!Object.IsInstanceValid(Tween)) return this;
            Logger.Info("Tween.StopAll()");
            Tween.StopAll();
            Logger.Info("Tween.QueueFree()");
            Tween.QueueFree();
            return this;
        }
        

        public SequencePlayer PlaySequence(ISequence sequence, Node defaultTarget = null, float initialDelay = 0,
            float duration = -1) {
            Execute(sequence, defaultTarget, initialDelay, duration);
            return this;
        }

        public SequencePlayer PlayTemplate(SequenceTemplate template, Node defaultTarget, float initialDelay = 0,
            float duration = -1) {
            Execute(template, defaultTarget, initialDelay, duration);
            return this;
        }

        public SequencePlayerWithSequenceScheduler CreateSequence(Node defaultTarget = null) {
            var sequence = new SequencePlayerWithSequenceScheduler(this, true).SetDefaultTarget(defaultTarget);
            return sequence;
        }

        private SequencePlayer Execute(ISequence sequence, Node defaultTarget = null, float initialDelay = 0,
            float duration = -1) {
            if (sequence.Loops == 1) {
                sequence.Execute(Tween, initialDelay, defaultTarget, duration);
            } else {
                ExecuteWithLoops(sequence, defaultTarget, initialDelay, duration);
            }
            return this;
        }

        private void ExecuteWithLoops(ISequence sequence, Node defaultTarget = null, float initialDelay = 0,
            float duration = -1) {
            var loops = 0;
            CallbackTweener sequenceFinishedCallback;
            Action declaredExecutor = null;

            void Executor(float delay) {
                if (sequence.IsInfiniteLoop || loops < sequence.Loops) {
                    loops++;
                    var elapsed = sequence.Execute(Tween, delay, defaultTarget, duration);
                    sequenceFinishedCallback = new CallbackTweener(delay + elapsed, declaredExecutor);
                    sequenceFinishedCallback.Start(Tween);
                } else {
                    sequenceFinishedCallback = null;
                }
            }

            declaredExecutor = () => Executor(0f);
            Executor(initialDelay);
        }
    }
}