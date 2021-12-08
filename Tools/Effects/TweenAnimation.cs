using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Object = Godot.Object;

namespace Tools.Effects {
    public delegate void OnFinishAnimationTweener(TweenSequence tweenSequence);

    internal interface ITweener {
        public void _start(float initialDelay, Tween tween);
        public float TotalTime { get; }
    }

    public delegate void TweenCallback();

    public class AnimationStep<T> {
        protected internal T From;
        protected internal float StepDelay;
        protected internal readonly T Offset;
        protected internal readonly float Duration;
        protected internal bool Relative = false;

        private TweenCallback _callback;

        private readonly Tween.TransitionType _trans;
        private readonly Tween.EaseType _ease;

        public AnimationStep(T offset, float duration, Tween.TransitionType trans, Tween.EaseType ease,
            TweenCallback callback) {
            Offset = offset;
            Duration = duration;
            _trans = trans;
            _ease = ease;
            _callback = callback;
        }

        public T StartAndGetFinalValue(float initialDelay, Tween tween, Node target, string property) {
            if (!Object.IsInstanceValid(target)) {
                TweenSequence.Logger.Warning("Can't create InterpolateProperty in a freed target instance");
                return default;
            }
            object absoluteTo = null;
            if (Relative) {
                if (From is float fromFloat && Offset is float toFloat) {
                    absoluteTo = fromFloat + toFloat;
                } else if (From is int fromInt && Offset is int toInt) {
                    absoluteTo = fromInt + toInt;
                } else if (From is Color fromColor && Offset is Color toColor) {
                    absoluteTo = fromColor + toColor;
                } else if (From is Vector2 fromVector2 && Offset is Vector2 toVector2) {
                    absoluteTo = fromVector2 + toVector2;
                } else if (From is Vector3 fromVector3 && Offset is Vector3 toVector3) {
                    absoluteTo = fromVector3 + toVector3;
                }
            } else {
                absoluteTo = Offset;
            }

            var start = StepDelay + initialDelay;

            var end = start + Duration;
            TweenSequence.Logger.Info(target.Name + "." + property + ": " + typeof(T).Name + " " +
                                      From + " to " + absoluteTo +
                                      " Start: " + start.ToString("F") +
                                      " End: " + end.ToString("F") +
                                      " (+" + Duration.ToString("F") + ")");
            tween.InterpolateProperty(target, property, From, absoluteTo, Duration, _trans, _ease, start);
            if (_callback != null) {
                TweenCallbackHolder holder = new TweenCallbackHolder(_callback);
                tween.InterpolateCallback(holder, start, nameof(TweenCallbackHolder.Call));
            }
            return (T)absoluteTo;
        }
    }

    internal class TweenCallbackHolder : Object {
        private TweenCallback _callback;

        public TweenCallbackHolder(TweenCallback callback) {
            _callback = callback;
        }

        internal void Call() {
            _callback.Invoke();
        }
    }


    internal class CallbackTweener : TweenCallbackHolder, ITweener {
        internal static readonly TweenCallback EmptyCallback = () => { };

        // private readonly TweenSequence _tweenSequence;
        private readonly float _delay;
        public float TotalTime => _delay;

        internal CallbackTweener(TweenSequence tweenSequence, float delay, TweenCallback callback) : base(callback) {
            // _tweenSequence = tweenSequence;
            _delay = delay;
        }

        public void _start(float initialDelay, Tween tween) {
            var start = _delay + initialDelay;
            TweenSequence.Logger.Info(" Callback: " + start.ToString("F"));
            tween.InterpolateCallback(this, initialDelay + _delay, nameof(Call));
        }
    }

    public class PropertyTweener<T> : ITweener {
        private readonly TweenSequence _tweenSequence;

        private readonly Node _target;
        private readonly string _member;
        private readonly bool _memberIsProperty;

        private readonly List<AnimationStep<T>> _steps = new List<AnimationStep<T>>(5);
        private readonly Tween.TransitionType _trans;
        private readonly Tween.EaseType _ease;

        private T _from;
        private bool _liveFrom = true;
        public float _time;
        public float TotalTime => _time;

        public PropertyTweener(TweenSequence tweenSequence, Node target, string member, bool memberIsProperty,
            Tween.TransitionType trans = Tween.TransitionType.Linear, Tween.EaseType ease = Tween.EaseType.InOut) {
            _tweenSequence = tweenSequence;
            _target = target;
            _member = member;
            _memberIsProperty = memberIsProperty;
            _trans = trans;
            _ease = ease;
            _from = default;
            _liveFrom = true;
        }

        public PropertyTweener<T> From(T from) {
            _from = from;
            _liveFrom = false;
            return this;
        }

        public PropertyTweener<T> From() {
            _from = default;
            _liveFrom = true;
            return this;
        }

        public PropertyTweener<T> To(T offset, float duration, TweenCallback callback = null) {
            return To(offset, duration, _trans, _ease, callback);
        }

        public PropertyTweener<T> To(T offset, float duration, Tween.TransitionType trans,
            TweenCallback callback = null) {
            return To(offset, duration, trans, _ease, callback);
        }

        public PropertyTweener<T> To(T offset, float duration, Tween.TransitionType trans, Tween.EaseType ease,
            TweenCallback callback = null) {
            var animationStepPropertyTweener = new AnimationStep<T>(offset, duration, trans, ease, callback);
            _steps.Add(animationStepPropertyTweener);
            _time += duration;
            return this;
        }

        public PropertyTweener<T> AddOffset(T offset, float duration, TweenCallback callback = null) {
            return AddOffset(offset, duration, _trans, _ease, callback);
        }

        public PropertyTweener<T> AddOffset(T offset, float duration, Tween.TransitionType trans,
            TweenCallback callback = null) {
            return AddOffset(offset, duration, trans, _ease, callback);
        }

        public PropertyTweener<T> AddOffset(T offset, float duration, Tween.TransitionType trans, Tween.EaseType ease,
            TweenCallback callback = null) {
            var step = new AnimationStep<T>(offset, duration, trans, ease, callback) {
                Relative = true
            };
            _time += duration;
            _steps.Add(step);
            return this;
        }

        public TweenSequence End() {
            return _tweenSequence;
        }

        public void _start(float initialDelay, Tween tween) {
            if (!Object.IsInstanceValid(_target)) {
                TweenSequence.Logger.Warning("Can't create InterpolateProperty in a freed target instance");
                return;
            }
            var from = _liveFrom ? (T)_target.GetIndexed(_member) : _from;
            var start = 0f;
            foreach (var step in _steps) {
                step.StepDelay = start;
                start += step.Duration;
                step.From = from;
                from = step.StartAndGetFinalValue(initialDelay, tween, _target, _member);
            }
        }
    }

    public class TweenSequence {
        internal static readonly Logger Logger = LoggerFactory.GetLogger(typeof(TweenSequence));
        internal readonly List<List<ITweener>> TweenList = new List<List<ITweener>>(10);
        private bool _parallel = false;

        public PropertyTweener<Color> AnimateColor(
            Node target, string property,
            Tween.TransitionType trans = Tween.TransitionType.Linear, Tween.EaseType ease = Tween.EaseType.InOut) {
            var tweener = new PropertyTweener<Color>(this, target, property, true, trans, ease);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyTweener<Vector2> AnimateVector2(Node target, string property,
            Tween.TransitionType trans = Tween.TransitionType.Linear, Tween.EaseType ease = Tween.EaseType.InOut) {
            var tweener = new PropertyTweener<Vector2>(this, target, property, true, trans, ease);
            AddTweener(tweener);
            return tweener;
        }

        public TweenSequence Parallel() {
            _parallel = true;
            return this;
        }

        public TweenSequence Pause(float delay) {
            AddTweener(new CallbackTweener(this, delay, CallbackTweener.EmptyCallback));
            return this;
        }

        public TweenSequence Callback(TweenCallback callback) {
            AddTweener(new CallbackTweener(this, 0, callback));
            return this;
        }

        internal void AddTweener(ITweener tweener) {
            if (_parallel) {
                TweenList.Last().Add(tweener);
                _parallel = false;
            } else {
                TweenList.Add(new List<ITweener>(3) { tweener });
            }
        }

        public int Loops = 1;
        public float Speed = 1.0f;
        public int GetLoops() => Loops;

        // Sets the speed scale of tweening.
        public TweenSequence SetSpeed(float speed) {
            Speed = speed;
            return this;
        }

        public TweenSequence SetLoops(int maxLoops) {
            Loops = maxLoops;
            return this;
        }
    }

    public class TweenPlayer : Reference {
        // Emited when one step of the sequence is finished.
        // [Signal]
        // delegate void step_finished(int idx);

        // Emited when a loop of the sequence is finished.
        // [Signal]
        // delegate void loop_finished();

        // Emitted when whole sequence is finished. Doesn't happen with infinite loops.
        // [Signal]
        // delegate void finished();

        public readonly string Name;
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(TweenPlayer));

        private readonly Tween _tween;
        private List<TweenSequence> _tweenSequences = new List<TweenSequence>(6);

        private int _currentSequence = 0;
        private int _sequenceLoop = 0;
        private int _currentPlayerLoop = 0;
        private bool _started = false;
        private bool _running = false;

        public int Loops = 0;
        private bool _killWhenFinished = false;

        public TweenPlayer(Tween tween, string name = null) {
            Name = name ?? tween.Name;
            _tween = tween;
            _tween.Connect("tween_all_completed", this, nameof(OnTweenAllCompletedSignaled));
        }

        public TweenPlayer(Node node) : this(new Tween()) {
            node.AddChild(_tween);
        }

        public TweenPlayer AddSequence(TweenSequence tweenSequence) {
            _tweenSequences.Add(tweenSequence);
            return this;
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
                RunCurrentSequence();
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
                RunCurrentSequence();
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

        private void RunCurrentSequence() {
            GD.Print("RunNextStep " + (1 + _currentSequence) + "/" + _tweenSequences.Count);
            float accumulated = 0;
            var currentSequence = _tweenSequences[_currentSequence];
            foreach (var group in currentSequence.TweenList) {
                float maxFinishTime = 0;
                foreach (var tweener in group) {
                    tweener._start(accumulated, _tween);
                    maxFinishTime = Math.Max(maxFinishTime, tweener.TotalTime);
                }
                accumulated += maxFinishTime;
            }
            _tween.PlaybackSpeed = currentSequence.Speed;
            _tween.Start();
        }

        private List<OnFinishAnimationTweener> _onFinishTween;

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
                _onFinishTween = new List<OnFinishAnimationTweener>(1) { onFinishTweenSequence };
            } else {
                _onFinishTween.Add(onFinishTweenSequence);
            }
        }


        private void OnTweenAllCompletedSignaled() {
            // EmitSignal(nameof(step_finished), _current_step);
            _sequenceLoop++;
            var currentSequence = _tweenSequences[_currentSequence];
            if (_sequenceLoop < currentSequence.Loops) {
                RunCurrentSequence();
                return;
            }

            _sequenceLoop = 0;
            _currentSequence++;
            if (_currentSequence < _tweenSequences.Count) {
                RunCurrentSequence();
                return;
            }

            _currentSequence = 0;
            _currentPlayerLoop++;
            if (IsInfiniteLoop() || _currentPlayerLoop < Loops) {
                // EmitSignal(nameof(loop_finished));
                RunCurrentSequence();
                return;
            }
            
            // Reset keeps the state, so Reset() will play again the sequence, meaning it will never finish
            Stop().Reset();
            // It's very important the event must be called the last
            // EmitSignal(nameof(finished));
            _onFinishTween?.ForEach(callback => callback.Invoke(_tweenSequences[_currentSequence]));
            if (_killWhenFinished) Kill();
        }
    }
}