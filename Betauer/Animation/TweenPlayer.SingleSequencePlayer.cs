using System;
using System.Diagnostics;
using Godot;

namespace Betauer.Animation {
    public class SingleSequencePlayer : RepeatablePlayer<SingleSequencePlayer> {
        public class SequenceBuilderWithSingleSequencePlayer : RegularSequenceBuilder<SequenceBuilderWithSingleSequencePlayer> {
            private readonly SingleSequencePlayer _singleSequencePlayer;

            internal SequenceBuilderWithSingleSequencePlayer(SingleSequencePlayer singleSequencePlayer,
                bool createEmptyTweenList) : base(createEmptyTweenList) {
                _singleSequencePlayer = singleSequencePlayer;
            }

            public SingleSequencePlayer EndSequence() {
                return _singleSequencePlayer;
            }
        }

        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(SingleSequencePlayer));
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        private int _currentLoop = 0;
        private int _loops;
        private bool _loopsOverriden = false;

        public ISequence Sequence { get; private set; }

        public int Loops => _loopsOverriden ? _loops :
            Sequence is ILoopedSequence loopedSequence ? loopedSequence.Loops : 1;

        public bool IsInfiniteLoop => Loops == -1;

        public SingleSequencePlayer() {
        }

        public SingleSequencePlayer(DisposableTween tween, bool disposeOnFinish = false) : base(tween, disposeOnFinish) {
        }

        public static SingleSequencePlayer Create(Node node, SequenceTemplate template, float duration = -1) {
            return new SingleSequencePlayer()
                .CreateNewTween(node)
                .ImportTemplate(template, node, duration)
                .EndSequence();
        }

        public static SingleSequencePlayer Create(Node node, ISequence sequence) {
            return new SingleSequencePlayer()
                .CreateNewTween(node)
                .WithSequence(sequence);
        }

        public SingleSequencePlayer SetInfiniteLoops() {
            _loopsOverriden = true;
            _loops = -1;
            return this;
        }

        public SingleSequencePlayer SetLoops(int loops) {
            _loopsOverriden = true;
            _loops = loops;
            return this;
        }

        public SingleSequencePlayer WithSequence(ISequence sequence) {
            if (Sequence != null) {
                throw new InvalidOperationException(
                    "Only one sequence is allowed. Please use Clear(), then you can create a new sequence.");
            }
            Sequence = sequence;
            _loopsOverriden = false;
            return this;
        }

        public SequenceBuilderWithSingleSequencePlayer ImportTemplate(SequenceTemplate template, Node defaultTarget = null,
            float duration = -1) {
            var sequence = new SequenceBuilderWithSingleSequencePlayer(this, false /* no data, it will use the template tweens */);
            sequence.ImportTemplate(template, defaultTarget, duration);
            WithSequence(sequence);
            return sequence;
        }

        public SequenceBuilderWithSingleSequencePlayer CreateSequence(Node defaultTarget = null) {
            var sequence = new SequenceBuilderWithSingleSequencePlayer(this,
                    true /* true to allow to add tweens during the sequence creation */)
                .SetDefaultTarget(defaultTarget);
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
            _currentLoop = 0;
        }

        protected override void OnStart() {
            RunSequence();
        }

        private void RunSequence() {
            _stopwatch.Restart();
            var loopState = IsInfiniteLoop ? "infinite loop" : $"{_currentLoop + 1}/{Loops} loops";
            Logger.Debug(
                $"RunSequence: Single sequence: {loopState}");
            Tween.PlaybackSpeed = Sequence.Speed;
            Tween.PlaybackProcessMode = Sequence.ProcessMode;
            var accumulatedDelay = Sequence.Execute(Tween);
            Tween.InterpolateCallback(this, accumulatedDelay, nameof(OnSequenceFinished));
            Logger.Debug($"RunSequence: Estimated time: {accumulatedDelay:F}");
        }

        private void OnSequenceFinished() {
            _stopwatch.Stop();
            Logger.Debug("RunSequence: OnSequenceFinished: " +
                         (_stopwatch.ElapsedMilliseconds / 1000f).ToString("F") + "s");
            _currentLoop++;
            if (IsInfiniteLoop || _currentLoop < Loops) {
                RunSequence();
            } else {
                Finished();
            }
        }
    }
}