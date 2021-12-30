using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace Betauer.Animation {
    public class SingleSequencePlayer : TweenPlayer<SingleSequencePlayer> {
        public class SequencePlayerWithSingleSequence : AbstractSequenceBuilder<SequencePlayerWithSingleSequence> {
            private readonly SingleSequencePlayer _singleSequencePlayer;

            internal SequencePlayerWithSingleSequence(SingleSequencePlayer singleSequencePlayer,
                ICollection<ICollection<ITweener>> tweenList) : base(tweenList) {
                _singleSequencePlayer = singleSequencePlayer;
            }

            public SingleSequencePlayer EndSequence() {
                return _singleSequencePlayer;
            }
        }

        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(SingleSequencePlayer));
        private int _sequenceLoop = 0;
        private bool _loopsDefined = false;
        private Stopwatch _sequenceStopwatch;

        public ISequence Sequence { get; private set; }
        private int _loops;
        public int Loops => _loopsDefined ? _loops : Sequence?.Loops ?? 1;
        public bool IsInfiniteLoop => Loops == -1;

        public SingleSequencePlayer() {
        }

        public SingleSequencePlayer(Tween tween, bool freeOnFinish = false) : base(tween, freeOnFinish) {
        }

        public static SingleSequencePlayer With(Node node, SequenceTemplate template, float duration = -1) {
            return new SingleSequencePlayer()
                .CreateNewTween(node)
                .ImportTemplate(template, node, duration)
                .EndSequence();
        }

        public static SingleSequencePlayer With(Node node, ISequence sequence) {
            return new SingleSequencePlayer()
                .CreateNewTween(node)
                .WithSequence(sequence);
        }

        public SingleSequencePlayer SetInfiniteLoops() {
            _loopsDefined = true;
            _loops = -1;
            return this;
        }

        public SingleSequencePlayer SetLoops(int maxLoops) {
            _loopsDefined = true;
            _loops = maxLoops;
            return this;
        }

        public SingleSequencePlayer WithSequence(ISequence sequence) {
            Sequence = sequence;
            _loopsDefined = false;
            return this;
        }

        public SequencePlayerWithSingleSequence ImportTemplate(SequenceTemplate sequence,
            Node defaultTarget = null, float duration = -1) {
            var sequenceWithPlayerBuilder = new SequencePlayerWithSingleSequence(this, null);
            sequenceWithPlayerBuilder.ImportTemplate(sequence, defaultTarget, duration);
            Sequence = sequenceWithPlayerBuilder;
            _loopsDefined = false;
            return sequenceWithPlayerBuilder;
        }

        public SequencePlayerWithSingleSequence CreateSequence() {
            var sequencePlayerWithSingleSequence = new SequencePlayerWithSingleSequence(this, new SimpleLinkedList<ICollection<ITweener>>());
            Sequence = sequencePlayerWithSingleSequence;
            _loopsDefined = false;
            return sequencePlayerWithSingleSequence;
        }

        public SingleSequencePlayer Clear() {
            Running = false;
            Reset();
            Sequence = null;
            return this;
        }

        protected override void OnReset() {
            _sequenceLoop = 0;
        }

        protected override void OnStart() {
            RunSequence();
        }

        private CallbackTweener _sequenceFinishedCallback;
        private void RunSequence() {
            _sequenceStopwatch = Stopwatch.StartNew();
            Logger.Debug($"RunSequence: Sequence loop: {(IsInfiniteLoop ? "infinite loop" : (_sequenceLoop + 1) + "/" + Loops)}");
            Tween.PlaybackSpeed = Sequence.Speed;
            Tween.PlaybackProcessMode = Sequence.ProcessMode;
            var accumulatedDelay = Sequence.Start(Tween);
            _sequenceFinishedCallback = new CallbackTweener(accumulatedDelay, OnSequenceFinished, nameof(OnSequenceFinished));
            _sequenceFinishedCallback.Start(Tween);
            Logger.Debug($"RunSequence: Estimated time: {accumulatedDelay:F}");
            Tween.Start();
        }

        private void OnSequenceFinished() {
            _sequenceStopwatch?.Stop();
            Logger.Debug("RunSequence: OnSequenceFinished: " +
                         ((_sequenceStopwatch?.ElapsedMilliseconds ?? 0) / 1000f).ToString("F") + "s");
            _sequenceStopwatch = null;
            _sequenceLoop++;
            if (IsInfiniteLoop || _sequenceLoop < Loops) {
                RunSequence();
            } else {
                Finished();
            }
        }
    }
}