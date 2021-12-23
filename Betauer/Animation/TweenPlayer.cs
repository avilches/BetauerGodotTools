using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Godot;

namespace Betauer.Animation {
    public delegate void OnTweenPlayerFinishAll();

    // public delegate void OnSequenceFinish(ITweenSequence tweenSequence);

    public class TweenPlayer : Reference {
        public class TweenSequenceWithPlayerBuilder : AbstractTweenSequenceBuilder<TweenSequenceWithPlayerBuilder> {
            private readonly TweenPlayer _tweenPlayer;

            internal TweenSequenceWithPlayerBuilder(TweenPlayer tweenPlayer,
                ICollection<ICollection<ITweener>> tweenList) : base(tweenList) {
                _tweenPlayer = tweenPlayer;
            }

            public TweenPlayer EndSequence() {
                return _tweenPlayer;
            }
        }

        public readonly string Name;
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(TweenPlayer));

        public readonly IList<ITweenSequence> TweenSequences = new List<ITweenSequence>(4);

        private int _currentSequence = 0;
        private int _sequenceLoop = 0;
        private int _currentPlayerLoop = 0;
        private bool _started = false;
        private bool _running = false;
        private bool _killWhenFinished = false;
        private SimpleLinkedList<OnTweenPlayerFinishAll> _onTweenPlayerFinishAll;

        public int Loops { get; private set; }
        public bool IsInfiniteLoop => Loops == -1;
        public Tween Tween { get; private set; }

        public TweenPlayer(string name = null) {
            Name = name;
        }

        public static TweenPlayer With(Node node, ITweenSequence tweenSequence) {
            return new TweenPlayer()
                .NewTween(node)
                .AddSequence(tweenSequence);
        }

        public static TweenPlayer With(Node node, TweenSequenceTemplate template, float duration = -1) {
            return new TweenPlayer()
                .NewTween(node)
                .ImportTemplate(template, node, duration)
                .EndSequence();
        }

        public TweenPlayer NewTween(Node node) {
            RemoveTween();
            var tween = new Tween();
            node.AddChild(tween);
            return SetTween(tween);
        }

        public TweenPlayer SetTween(Tween tween) {
            RemoveTween();
            Tween = tween;
            // _tween.Connect("tween_all_completed", this, nameof(OnTweenAllCompletedSignaled));
            // _tween.Connect("tween_started", this, nameof(TweenStarted));
            // _tween.Connect("tween_completed", this, nameof(TweenCompleted));
            return this;
        }

        public TweenPlayer RemoveTween() {
            if (Tween != null) {
                _running = false;
                Reset();
                Tween = null;
            }
            return this;
        }

        public TweenPlayer AddSequence(ITweenSequence tweenSequence) {
            TweenSequences.Add(tweenSequence);
            return this;
        }

        public TweenSequenceWithPlayerBuilder ImportTemplate(TweenSequenceTemplate tweenSequence,
            Node defaultTarget = null, float duration = -1) {
            var tweenSequenceWithPlayerBuilder = new TweenSequenceWithPlayerBuilder(this, null);
            tweenSequenceWithPlayerBuilder.ImportTemplate(tweenSequence, defaultTarget, duration);
            TweenSequences.Add(tweenSequenceWithPlayerBuilder);
            return tweenSequenceWithPlayerBuilder;
        }

        public TweenSequenceWithPlayerBuilder CreateSequence() {
            var tweenSequence = new TweenSequenceWithPlayerBuilder(this, new SimpleLinkedList<ICollection<ITweener>>());
            TweenSequences.Add(tweenSequence);
            return tweenSequence;
        }

        public TweenPlayer AddOnTweenPlayerFinishAll(OnTweenPlayerFinishAll onTweenPlayerFinishAllTweenSequence) {
            // An array it's needed because the TweenAnimation uses this callback to return from a finished Once tween
            // to the previous loop tween stored in the stack. So, if a user creates a sequence with something in
            // the OnFinishTween, and it adds this sequence to the TweenAnimation, the callback will be lost. So, with
            // an array, the AnimationTweenPlayer can store the both OnFinishTween: the user one, and the AnimationTweenPlayer one.

            // Pay attention that with TweenAnimation, all this callback can be used
            // - AnimationTweenPlayer.OnFinishTween
            // - OnceTween (from TweenAnimation) OnEnd
            // The main difference is the OnEnd callback will be invoked in the TweenAnimation when a OnceTween is
            // finished or interrupted. But the AnimationTweenPlayer.OnFinishTween callback will be invoked only when finished.
            if (_onTweenPlayerFinishAll == null) {
                _onTweenPlayerFinishAll = new SimpleLinkedList<OnTweenPlayerFinishAll>
                    { onTweenPlayerFinishAllTweenSequence };
            } else {
                _onTweenPlayerFinishAll.Add(onTweenPlayerFinishAllTweenSequence);
            }
            return this;
        }


        // Sets the speed scale of tweening.
        public TweenPlayer SetSpeed(float speed) {
            Tween.PlaybackSpeed = speed;
            return this;
        }

        public TweenPlayer SetInfiniteLoops() {
            Loops = -1;
            return this;
        }

        public TweenPlayer SetLoops(int maxLoops) {
            Loops = maxLoops;
            return this;
        }

        // Whether the Tween should be freed when sequence finishes.
        // Default is true. If set to false, sequence will restart on end.
        public TweenPlayer SetAutoKill(bool autoKill) {
            _killWhenFinished = autoKill;
            return this;
        }

        // Returns whether the sequence is currently running.
        public bool IsRunning() {
            return _running;
        }

        /*
         * Flow:
         * 1 Start <-> Stop
         * 2 Reset keeps the current status
         */

        public TweenPlayer Start() {
            if (!IsInstanceValid(Tween)) {
                Logger.Warning("Can't Start AnimationTweenPlayer in a freed Tween instance");
                return this;
            }
            if (!_started) {
                _started = true;
                _running = true;
                RunSequence();
            } else {
                if (!_running) {
                    Logger.Info("Tween.ResumeAll()");
                    Tween.ResumeAll();
                    _running = true;
                }
            }
            return this;
        }

        // Pauses the execution of the tweens.
        public TweenPlayer Stop() {
            if (!IsInstanceValid(Tween)) {
                Logger.Warning("Can't Stop AnimationTweenPlayer in a freed Tween instance");
                return this;
            }
            if (_running) {
                Logger.Info("Tween.StopAll()");
                Tween.StopAll();
                _running = false;
            }
            return this;
        }

        public TweenPlayer Clear() {
            _running = false;
            Reset();
            TweenSequences.Clear();
            return this;
        }

        // Stops the sequence && resets it to the beginning.
        public TweenPlayer Reset() {
            if (!IsInstanceValid(Tween)) {
                Logger.Warning("Can't Reset AnimationTweenPlayer in a freed Tween instance");
                return this;
            }
            Logger.Info("Tween.StopAll()");
            Tween.StopAll();
            Logger.Info("Tween.RemoveAll()");
            Tween.RemoveAll();
            _currentPlayerLoop = 0;
            _sequenceLoop = 0;
            _currentSequence = 0;
            if (_running) {
                RunSequence();
            } else {
                _started = false;
            }
            return this;
        }

        // Frees the underlying Tween. Sequence is unusable after this operation.
        public void Kill() {
            if (!IsInstanceValid(Tween)) return;
            if (_running) {
                Stop();
            }
            Logger.Info("Tween.QueueFree()");
            Tween.QueueFree();
        }

        private Stopwatch _sequenceStopwatch;

        private void RunSequence() {
            _sequenceStopwatch = Stopwatch.StartNew();
            var sequence = TweenSequences[_currentSequence];
            Logger.Debug(
                $"RunSequence: Main loop: {(IsInfiniteLoop ? "infinite loop" : (_currentPlayerLoop + 1) + "/" + Loops)}. Sequence {_currentSequence + 1}/{TweenSequences.Count}. Loop: {_sequenceLoop + 1}/{sequence.Loops}");
            float accumulatedDelay = 0;
            var tweens = 0;
            foreach (var parallelGroup in sequence.TweenList) {
                float longestTime = 0;
                foreach (var tweener in parallelGroup) {
                    var tweenTime = tweener.Start(Tween, accumulatedDelay, sequence.DefaultTarget,
                        sequence.DefaultProperty, sequence.Duration);
                    tweens++;
                    longestTime = Math.Max(longestTime, tweenTime);
                }
                accumulatedDelay += longestTime;
            }
            new CallbackTweener(0, OnSequenceFinished, nameof(OnSequenceFinished)).Start(Tween, accumulatedDelay);
            Tween.PlaybackSpeed = sequence.Speed;
            Tween.PlaybackProcessMode = sequence.ProcessMode;
            Logger.Debug("RunSequence: Start " + tweens + " tweens. Estimated time: " +
                         accumulatedDelay.ToString("F"));
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
                Logger.Debug($"OnTweenAllCompleted. End: stop & reset. Kill {_killWhenFinished}");
                // Reset keeps the state, so Reset() will play again the sequence, meaning it will never finish
                Stop().Reset();
                // End of ALL THE LOOPS of all the sequences of the player
                _onTweenPlayerFinishAll?.ForEach(callback => callback.Invoke());
                // EmitSignal(nameof(finished));
                if (_killWhenFinished) Kill();
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

        public Task<TweenPlayer> Await() {
            var promise = new TaskCompletionSource<TweenPlayer>();
            AddOnTweenPlayerFinishAll(() => promise.TrySetResult(this));
            return promise.Task;
        }
    }
}