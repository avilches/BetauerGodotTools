using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace Betauer.Animation {
    public class MultipleSequencePlayer : TweenPlayer<MultipleSequencePlayer> {
        public class TweenSequenceBuilderWithMultipleSequencePlayer : AbstractTweenSequenceBuilder<TweenSequenceBuilderWithMultipleSequencePlayer> {
            private readonly MultipleSequencePlayer _multipleSequencePlayer;

            internal TweenSequenceBuilderWithMultipleSequencePlayer(MultipleSequencePlayer multipleSequencePlayer,
                ICollection<ICollection<ITweener>> tweenList) : base(tweenList) {
                _multipleSequencePlayer = multipleSequencePlayer;
            }

            public MultipleSequencePlayer EndSequence() {
                return _multipleSequencePlayer;
            }
        }

        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(MultipleSequencePlayer));
        private int _currentSequence = 0;
        private int _sequenceLoop = 0;
        private int _currentPlayerLoop = 0;
        private Stopwatch _sequenceStopwatch;

        public readonly IList<ITweenSequence> TweenSequences = new List<ITweenSequence>(4);
        public int Loops { get; private set; }
        public bool IsInfiniteLoop => Loops == -1;

        public MultipleSequencePlayer() {
        }

        public MultipleSequencePlayer(Tween tween, bool freeOnFinish = false) : base(tween, freeOnFinish) {
        }

        public MultipleSequencePlayer SetInfiniteLoops() {
            Loops = -1;
            return this;
        }

        public MultipleSequencePlayer SetLoops(int maxLoops) {
            Loops = maxLoops;
            return this;
        }

        public MultipleSequencePlayer AddSequence(ITweenSequence tweenSequence) {
            TweenSequences.Add(tweenSequence);
            return this;
        }

        public TweenSequenceBuilderWithMultipleSequencePlayer ImportTemplate(TweenSequenceTemplate tweenSequence,
            Node defaultTarget = null, float duration = -1) {
            var tweenSequenceWithPlayerBuilder = new TweenSequenceBuilderWithMultipleSequencePlayer(this, null);
            tweenSequenceWithPlayerBuilder.ImportTemplate(tweenSequence, defaultTarget, duration);
            TweenSequences.Add(tweenSequenceWithPlayerBuilder);
            return tweenSequenceWithPlayerBuilder;
        }

        public TweenSequenceBuilderWithMultipleSequencePlayer CreateSequence() {
            var tweenSequence = new TweenSequenceBuilderWithMultipleSequencePlayer(this, new SimpleLinkedList<ICollection<ITweener>>());
            TweenSequences.Add(tweenSequence);
            return tweenSequence;
        }

        public MultipleSequencePlayer Clear() {
            Running = false;
            Reset();
            TweenSequences.Clear();
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

        private CallbackTweener _sequenceFinishedCallback;
        private void RunSequence() {
            _sequenceStopwatch = Stopwatch.StartNew();
            var sequence = TweenSequences[_currentSequence];
            Logger.Debug(
                $"RunSequence: Main loop: {(IsInfiniteLoop ? "infinite loop" : (_currentPlayerLoop + 1) + "/" + Loops)}. Sequence {_currentSequence + 1}/{TweenSequences.Count}. Sequence loop: {_sequenceLoop + 1}/{sequence.Loops}");
            Tween.PlaybackSpeed = sequence.Speed;
            Tween.PlaybackProcessMode = sequence.ProcessMode;
            var accumulatedDelay = sequence.Start(Tween);
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
            if (More()) {
                RunSequence();
            } else {
                Finished();
            }
        }

        private bool More() {
            // EmitSignal(nameof(step_finished), _current_step);
            _sequenceLoop++;
            var currentSequence = TweenSequences[_currentSequence];
            if (_sequenceLoop < currentSequence.Loops) {
                return true;
            }
            // End of a single sequence including all of the loops of the sequence
            // TODO: add a callback here?

            _sequenceLoop = 0;
            _currentSequence++;
            if (_currentSequence < TweenSequences.Count) {
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
    }
}