using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace Betauer.Animation {
    public class SingleSequencePlayer : TweenPlayer<SingleSequencePlayer> {
        public class TweenSequencePlayerWithSingleSequence : AbstractTweenSequenceBuilder<TweenSequencePlayerWithSingleSequence> {
            private readonly SingleSequencePlayer _singleSequencePlayer;

            internal TweenSequencePlayerWithSingleSequence(SingleSequencePlayer singleSequencePlayer,
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

        public ITweenSequence TweenSequence { get; private set; }
        private int _loops;
        public int Loops => _loopsDefined ? _loops : TweenSequence?.Loops ?? 1;
        public bool IsInfiniteLoop => Loops == -1;

        public SingleSequencePlayer() {
        }

        public SingleSequencePlayer(Tween tween, bool freeOnFinish = false) : base(tween, freeOnFinish) {
        }

        public static SingleSequencePlayer With(Node node, TweenSequenceTemplate template, float duration = -1) {
            return new SingleSequencePlayer()
                .CreateNewTween(node)
                .ImportTemplate(template, node, duration)
                .EndSequence();
        }

        public static SingleSequencePlayer With(Node node, ITweenSequence tweenSequence) {
            return new SingleSequencePlayer()
                .CreateNewTween(node)
                .WithSequence(tweenSequence);
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

        public SingleSequencePlayer WithSequence(ITweenSequence tweenSequence) {
            TweenSequence = tweenSequence;
            _loopsDefined = false;
            return this;
        }

        public TweenSequencePlayerWithSingleSequence ImportTemplate(TweenSequenceTemplate tweenSequence,
            Node defaultTarget = null, float duration = -1) {
            var tweenSequenceWithPlayerBuilder = new TweenSequencePlayerWithSingleSequence(this, null);
            tweenSequenceWithPlayerBuilder.ImportTemplate(tweenSequence, defaultTarget, duration);
            TweenSequence = tweenSequenceWithPlayerBuilder;
            _loopsDefined = false;
            return tweenSequenceWithPlayerBuilder;
        }

        public TweenSequencePlayerWithSingleSequence CreateSequence() {
            var tweenSequence = new TweenSequencePlayerWithSingleSequence(this, new SimpleLinkedList<ICollection<ITweener>>());
            TweenSequence = tweenSequence;
            _loopsDefined = false;
            return tweenSequence;
        }

        public SingleSequencePlayer Clear() {
            Running = false;
            Reset();
            TweenSequence = null;
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
            float accumulatedDelay = 0;
            var tweens = 0;
            foreach (var parallelGroup in TweenSequence.TweenList) {
                float longestTime = 0;
                foreach (var tweener in parallelGroup) {
                    var tweenTime = tweener.Start(Tween, accumulatedDelay, TweenSequence.DefaultTarget, TweenSequence.Duration);
                    tweens++;
                    longestTime = Math.Max(longestTime, tweenTime);
                }
                accumulatedDelay += longestTime;
            }
            _sequenceFinishedCallback = new CallbackTweener(0, OnSequenceFinished, nameof(OnSequenceFinished));
            _sequenceFinishedCallback.Start(Tween, accumulatedDelay);
            Tween.PlaybackSpeed = TweenSequence.Speed;
            Tween.PlaybackProcessMode = TweenSequence.ProcessMode;
            Logger.Debug("RunSequence: Start " + tweens + " tweens. Estimated time: " +
                         accumulatedDelay.ToString("F"));
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