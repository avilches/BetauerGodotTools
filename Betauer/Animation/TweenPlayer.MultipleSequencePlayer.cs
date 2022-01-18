using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace Betauer.Animation {
    public class MultipleSequencePlayer : RepeatablePlayer<MultipleSequencePlayer> {
        public class SequenceBuilderWithMultipleSequencePlayer : RegularSequenceBuilder<SequenceBuilderWithMultipleSequencePlayer> {
            private readonly MultipleSequencePlayer _multipleSequencePlayer;

            internal SequenceBuilderWithMultipleSequencePlayer(MultipleSequencePlayer multipleSequencePlayer,
                bool createEmptyTweenList) : base(createEmptyTweenList) {
                _multipleSequencePlayer = multipleSequencePlayer;
            }

            public MultipleSequencePlayer EndSequence() {
                return _multipleSequencePlayer;
            }
        }

        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(MultipleSequencePlayer));
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        private int _currentSequence = 0;
        private int _sequenceLoop = 0;
        private int _currentPlayerLoop = 0;

        public readonly IList<ISequence> Sequences = new List<ISequence>(4);
        public int Loops { get; private set; }
        public bool IsInfiniteLoop => Loops == -1;

        public MultipleSequencePlayer() {
        }

        public MultipleSequencePlayer(DisposableTween tween, bool disposeOnFinish = false) : base(tween, disposeOnFinish) {
        }

        public MultipleSequencePlayer SetInfiniteLoops() {
            Loops = -1;
            return this;
        }

        public MultipleSequencePlayer SetLoops(int maxLoops) {
            Loops = maxLoops;
            return this;
        }

        public MultipleSequencePlayer AddSequence(ISequence sequence) {
            if (sequence is SequenceTemplate) {
                throw new InvalidOperationException("Use ImportTemplate instead");
            }
            Sequences.Add(sequence);
            return this;
        }

        public SequenceBuilderWithMultipleSequencePlayer ImportTemplate(SequenceTemplate template,
            Node defaultTarget, float duration = -1) {
            var sequence = new SequenceBuilderWithMultipleSequencePlayer(this,
                false /* false because the template already have the tween list */);
            sequence.ImportTemplate(template, defaultTarget, duration);
            Sequences.Add(sequence);
            return sequence;
        }

        public SequenceBuilderWithMultipleSequencePlayer CreateSequence(Node defaultTarget = null) {
            var sequence = new SequenceBuilderWithMultipleSequencePlayer(this, true).SetDefaultTarget(defaultTarget);
            Sequences.Add(sequence);
            return sequence;
        }

        public MultipleSequencePlayer Clear() {
            Running = false;
            Reset();
            Sequences.Clear();
            return this;
        }


        protected override void OnReset() {
            _currentPlayerLoop = 0;
            _sequenceLoop = 0;
            _currentSequence = 0;
        }

        protected override void OnStart() {
            RunSequence();
        }

        private void RunSequence() {
            _stopwatch.Restart();
            var sequence = Sequences[_currentSequence];
            Logger.Debug(
                $"RunSequence: Main loop: {(IsInfiniteLoop ? "infinite loop" : (_currentPlayerLoop + 1) + "/" + Loops)}. Sequence {_currentSequence + 1}/{Sequences.Count}. Sequence loop: {_sequenceLoop + 1}/{GetLoopsFromSequence(sequence)}");
            Tween.PlaybackSpeed = sequence.Speed;
            Tween.PlaybackProcessMode = sequence.ProcessMode;
            var accumulatedDelay = sequence.Execute(Tween);
            Tween.InterpolateCallback(this, accumulatedDelay, nameof(OnSequenceFinished));
            Logger.Debug($"RunSequence: Estimated time: {accumulatedDelay:F}");
        }

        private void OnSequenceFinished() {
            _stopwatch.Stop();
            Logger.Debug("RunSequence: OnSequenceFinished: " +
                         (_stopwatch.ElapsedMilliseconds / 1000f).ToString("F") + "s");
            if (More()) {
                RunSequence();
            } else {
                Finished();
            }
        }

        private bool More() {
            // EmitSignal(nameof(step_finished), _current_step);
            _sequenceLoop++;
            var currentSequence = Sequences[_currentSequence];
            if (_sequenceLoop < GetLoopsFromSequence(currentSequence)) {
                return true;
            }
            // End of a single sequence including all of the loops of the sequence
            // TODO: add a callback here?

            _sequenceLoop = 0;
            _currentSequence++;
            if (_currentSequence < Sequences.Count) {
                return true;
            }
            // End of a ONE LOOP of all the sequences of the player
            // TODO: add a callback here?

            _currentSequence = 0;
            _currentPlayerLoop++;
            if (IsInfiniteLoop || _currentPlayerLoop < Loops) {
                // EmitSignal(nameof(loop_finished));
                return true;
            }
            return false;
        }

        private static int GetLoopsFromSequence(ISequence sequence) {
            return sequence is ILoopedSequence loopedSequence ? loopedSequence.Loops : 1;
        }
    }
}