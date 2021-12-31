using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace Betauer.Animation {
    public class SingleSequencePlayer : TweenPlayer<SingleSequencePlayer> {
        public class SequencePlayerWithSingleSequence : RegularSequenceBuilder<SequencePlayerWithSingleSequence> {
            private readonly SingleSequencePlayer _singleSequencePlayer;

            internal SequencePlayerWithSingleSequence(SingleSequencePlayer singleSequencePlayer,
                bool createEmptyTweenList) : base(createEmptyTweenList) {
                _singleSequencePlayer = singleSequencePlayer;
            }

            public SingleSequencePlayer EndSequence() {
                return _singleSequencePlayer;
            }
        }

        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(SingleSequencePlayer));
        private int _loop = 0;
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
            if (Sequence != null) {
                throw new InvalidOperationException(
                    "Only one sequence is allowed. Please use Clear(), then you can create a new sequence.");
            }
            Sequence = sequence;
            _loopsDefined = false;
            return this;
        }

        public SequencePlayerWithSingleSequence ImportTemplate(SequenceTemplate template, Node target = null,
            float duration = -1) {
            var sequence = new SequencePlayerWithSingleSequence(this, false /* no data, it will use the template tweens */);
            sequence.ImportTemplate(template, target, duration);
            WithSequence(sequence);
            return sequence;
        }

        public SequencePlayerWithSingleSequence CreateSequence(Node target = null) {
            var sequence = new SequencePlayerWithSingleSequence(this,
                    true /* true to allow to add tweens during the sequence creation */)
                .SetTarget(target);
            WithSequence(sequence);
            return sequence;
        }

        public SingleSequencePlayer Clear() {
            Running = false;
            Reset();
            Sequence = null;
            return this;
        }

        protected override void OnReset() {
            _loop = 0;
        }

        protected override void OnStart() {
            RunSequence();
        }

        private CallbackTweener _sequenceFinishedCallback;

        private void RunSequence() {
            _sequenceStopwatch = Stopwatch.StartNew();
            var loopState = IsInfiniteLoop ? "infinite loop" : $"{_loop + 1}/{Loops} loops";
            Logger.Debug(
                $"RunSequence: Single sequence: {loopState}");
            Tween.PlaybackSpeed = Sequence.Speed;
            Tween.PlaybackProcessMode = Sequence.ProcessMode;
            var accumulatedDelay = Sequence.Start(Tween);
            _sequenceFinishedCallback =
                new CallbackTweener(accumulatedDelay, OnSequenceFinished, nameof(OnSequenceFinished));
            _sequenceFinishedCallback.Start(Tween);
            Logger.Debug($"RunSequence: Estimated time: {accumulatedDelay:F}");
            Tween.Start();
        }

        private void OnSequenceFinished() {
            _sequenceStopwatch?.Stop();
            Logger.Debug("RunSequence: OnSequenceFinished: " +
                         ((_sequenceStopwatch?.ElapsedMilliseconds ?? 0) / 1000f).ToString("F") + "s");
            _sequenceStopwatch = null;
            _loop++;
            if (IsInfiniteLoop || _loop < Loops) {
                RunSequence();
            } else {
                Finished();
            }
        }
    }
}