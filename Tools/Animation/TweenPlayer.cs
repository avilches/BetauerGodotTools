using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Tools.Animation {
    public delegate void OnFinishAnimationTweener(ITweenSequence tweenSequence);

    public class TweenPlayer : Reference {
        public class TweenSequenceWithPlayerBuilder : TweenSequenceBuilder {
            private readonly TweenPlayer _tweenPlayer;

            internal TweenSequenceWithPlayerBuilder(TweenPlayer tweenPlayer, ICollection<ICollection<ITweener>> tweenList) : base(tweenList) {
                _tweenPlayer = tweenPlayer;
            }

            public override TweenPlayer EndSequence() {
                base.EndSequence();
                return _tweenPlayer;
            }
        }

        // Emited when one step of the sequence is finished.
        // [Signal]
        // delegate void step_finished(int idx);

        // Emited when a loop of the sequence is finished.
        // [Signal]
        // delegate void loop_finished();

        // Emitted when whole sequence is finished. Doesn't happen with infinite loops.
        // [Signal]
        // delegate void finished();

        private readonly string Name;
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(TweenPlayer));

        private Tween _tween;
        public readonly IList<ITweenSequence> _tweenSequences = new List<ITweenSequence>(4);

        private int _currentSequence = 0;
        private int _sequenceLoop = 0;
        private int _currentPlayerLoop = 0;
        private bool _started = false;
        private bool _running = false;

        public int Loops = 0;
        private bool _killWhenFinished = false;

        public TweenPlayer(string name = null) {
            Name = name;
        }

        public static TweenPlayer Apply(Node node, TweenSequenceTemplate template, float duration = -1) {
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
            _tween = tween;
            _tween.Connect("tween_all_completed", this, nameof(OnTweenAllCompletedSignaled));
            return this;
        }

        public TweenPlayer RemoveTween() {
            if (_tween != null) {
                _running = false;
                Reset();
                _tween.Disconnect("tween_all_completed", this, nameof(OnTweenAllCompletedSignaled));
                _tween = null;
            }
            return this;
        }

        public TweenPlayer AddSequence(ITweenSequence tweenSequence) {
            _tweenSequences.Add(tweenSequence);
            return this;
        }

        public TweenSequenceWithPlayerBuilder ImportTemplate(TweenSequenceTemplate tweenSequence,
            Node defaultTarget = null, float duration = -1) {
            // int loops = 1, float speed = 1.0f, Tween.TweenProcessMode processMode = Tween.TweenProcessMode.Physics) {
            var tweenSequenceWithPlayerBuilder = new TweenSequenceWithPlayerBuilder(this, null);
            tweenSequenceWithPlayerBuilder.ImportTemplate(tweenSequence, defaultTarget, duration);
            _tweenSequences.Add(tweenSequenceWithPlayerBuilder);
            return tweenSequenceWithPlayerBuilder;
        }

        public TweenSequenceWithPlayerBuilder CreateSequence() {
            var tweenSequence = new TweenSequenceWithPlayerBuilder(this, new SimpleLinkedList<ICollection<ITweener>>());
            _tweenSequences.Add(tweenSequence);
            return tweenSequence;
        }

        // Sets the speed scale of tweening.
        public TweenPlayer SetSpeed(float speed) {
            _tween.PlaybackSpeed = speed;
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

        public int GetLoops() => Loops;
        public bool IsInfiniteLoop() => Loops == -1;

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
            if (!IsInstanceValid(_tween)) {
                Logger.Warning("Can't Start AnimationTweenPlayer in a freed Tween instance");
                return this;
            }
            if (!_started) {
                _started = true;
                _running = true;
                RunSequence();
            } else {
                if (!_running) {
                    _tween.ResumeAll();
                    _running = true;
                }
            }
            return this;
        }

        // Pauses the execution of the tweens.
        public TweenPlayer Stop() {
            if (!IsInstanceValid(_tween)) {
                Logger.Warning("Can't Stop AnimationTweenPlayer in a freed Tween instance");
                return this;
            }
            if (_running) {
                _tween.StopAll();
                _running = false;
            }
            return this;
        }

        public TweenPlayer Clear() {
            _running = false;
            Reset();
            _tweenSequences.Clear();
            return this;
        }

        // Stops the sequence && resets it to the beginning.
        public TweenPlayer Reset() {
            if (!IsInstanceValid(_tween)) {
                Logger.Warning("Can't Reset AnimationTweenPlayer in a freed Tween instance");
                return this;
            }
            _tween.StopAll();
            _tween.RemoveAll();
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
            if (!IsInstanceValid(_tween)) return;
            if (_running) {
                Stop();
            }
            _tween.QueueFree();
        }

        private void RunSequence() {
            Logger.Debug("RunSequence " + (1 + _currentSequence) + "/" + _tweenSequences.Count);
            var sequence = _tweenSequences[_currentSequence];
            float accumulatedDelay = 0;
            var parallelGroupCount = 0;
            foreach (var parallelGroup in sequence.TweenList) {
                parallelGroupCount++;
                if (parallelGroup.Count == 1) {
                    Logger.Debug("Start single tween " + (parallelGroupCount + 1) + "/" + sequence.TweenList.Count);
                    var tweenTime = parallelGroup.First().Start(_tween, accumulatedDelay, sequence.DefaultTarget,
                        sequence.DefaultMember, sequence.Duration);
                    Logger.Debug("Launched tween. Time: " + tweenTime.ToString("F") + "s");
                    accumulatedDelay += tweenTime;
                } else {
                    Logger.Debug("Start parallel tweens " + (parallelGroupCount + 1) + "/" + sequence.TweenList.Count +
                                 ": " + parallelGroup.Count + " in parallel:");

                    float longestTime = 0;
                    foreach (var tweener in parallelGroup) {
                        var tweenTime = tweener.Start(_tween, accumulatedDelay, sequence.DefaultTarget,
                            sequence.DefaultMember, sequence.Duration);
                        Logger.Debug("Launched tween. Time: " + tweenTime.ToString("F") + "s");
                        longestTime = Math.Max(longestTime, tweenTime);
                    }
                    Logger.Debug("End parallel group. Total time: " + longestTime.ToString("F") + "s");
                    accumulatedDelay += longestTime;
                }
            }
            _tween.PlaybackSpeed = sequence.Speed;
            _tween.PlaybackProcessMode = sequence.ProcessMode;
            _tween.Start();
        }

        private SimpleLinkedList<OnFinishAnimationTweener> _onFinishTween;

        public void AddOnFinishTween(OnFinishAnimationTweener onFinishTweenSequence) {
            // An array it's needed because the TweenAnimation uses this callback to return from a finished Once tween
            // to the previous loop tween stored in the stack. So, if a user creates a sequence with something in
            // the OnFinishTween, and it adds this sequence to the TweenAnimation, the callback will be lost. So, with
            // an array, the AnimationTweenPlayer can store the both OnFinishTween: the user one, and the AnimationTweenPlayer one.

            // Pay attention that with TweenAnimation, all this callback can be used
            // - AnimationTweenPlayer.OnFinishTween
            // - OnceTween (from TweenAnimation) OnEnd
            // The main difference is the OnEnd callback will be invoked in the TweenAnimation when a OnceTween is
            // finished or interrupted. But the AnimationTweenPlayer.OnFinishTween callback will be invoked only when finished.
            if (_onFinishTween == null) {
                _onFinishTween = new SimpleLinkedList<OnFinishAnimationTweener> { onFinishTweenSequence };
            } else {
                _onFinishTween.Add(onFinishTweenSequence);
            }
        }


        private void OnTweenAllCompletedSignaled() {
            // EmitSignal(nameof(step_finished), _current_step);
            _sequenceLoop++;
            var currentSequence = _tweenSequences[_currentSequence];
            if (_sequenceLoop < currentSequence.Loops) {
                Logger.Debug("OnTweenAllCompletedSignaled: Next loop in sequence: " + _sequenceLoop + "/" +
                             currentSequence.Loops);
                RunSequence();
                return;
            }
            Logger.Debug("OnTweenAllCompletedSignaled: End loop: " + _sequenceLoop + "/" + currentSequence.Loops);

            _sequenceLoop = 0;
            _currentSequence++;
            if (_currentSequence < _tweenSequences.Count) {
                Logger.Debug("OnTweenAllCompletedSignaled: Next sequence: " + _currentSequence + "/" +
                             _tweenSequences.Count);
                RunSequence();
                return;
            }
            Logger.Debug("OnTweenAllCompletedSignaled: End sequence: " + _currentSequence + "/" +
                         _tweenSequences.Count);

            _currentSequence = 0;
            _currentPlayerLoop++;
            if (IsInfiniteLoop() || _currentPlayerLoop < Loops) {
                Logger.Debug("OnTweenAllCompletedSignaled: Next player loop: " +
                             (IsInfiniteLoop() ? "infinite loop" : _currentPlayerLoop + "/" + Loops));
                // EmitSignal(nameof(loop_finished));
                RunSequence();
                return;
            }
            Logger.Debug("OnTweenAllCompletedSignaled: End player loop: " + _currentPlayerLoop + "/" + Loops);
            // Reset keeps the state, so Reset() will play again the sequence, meaning it will never finish
            Stop().Reset();
            // It's very important the event must be called the last
            // EmitSignal(nameof(finished));
            _onFinishTween?.ForEach(callback => callback.Invoke(_tweenSequences[_currentSequence]));
            if (_killWhenFinished) Kill();
        }
    }
}