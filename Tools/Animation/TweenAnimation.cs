using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Object = Godot.Object;

namespace Tools.Animation {
    public delegate void OnFinishAnimationTweener(ITweenSequence tweenSequence);

    public interface ITweener {
        public float Start(float initialDelay, Tween tween);
    }

    public delegate void TweenCallback();

    internal class TweenCallbackHolder : Object {
        protected readonly TweenCallback Callback;

        internal TweenCallbackHolder(TweenCallback callback) {
            Callback = callback;
        }

        internal void Call() {
            Callback?.Invoke();
        }
    }

    internal class TweenPropertyMethodHolder<T> : Object {
        public delegate void PropertyMethodCallback(T value);

        private readonly PropertyMethodCallback _callback;

        public TweenPropertyMethodHolder(PropertyMethodCallback callback) {
            _callback = callback;
        }

        internal void Call(T value) {
            _callback.Invoke(value);
        }
    }

    internal class CallbackTweener : TweenCallbackHolder, ITweener {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(PropertyTweener<>));

        // private readonly TweenSequence _tweenSequence;
        private readonly float _delay;

        internal CallbackTweener(float delay, TweenCallback callback) : base(callback) {
            // _tweenSequence = tweenSequence;
            _delay = delay;
        }

        public float Start(float initialDelay, Tween tween) {
            var start = _delay + initialDelay;
            if (Callback != null) {
                Logger.Info("Scheduling callback with " + _delay + "s delay. Start: " + start.ToString("F"));
            } else {
                Logger.Info("Scheduling " + _delay + "s delay. Start: " + initialDelay.ToString("F") + " End: " +
                            start.ToString("F"));
            }
            tween.InterpolateCallback(this, start, nameof(Call));
            return _delay;
        }
    }

    public abstract class PropertyTweener<T> {
        protected static readonly Logger Logger = LoggerFactory.GetLogger(typeof(PropertyTweener<>));

        protected readonly Node _target;
        protected readonly string _member;

        // TODO: create method/delegate interpolator
        protected readonly bool _memberIsProperty;
        protected readonly Easing _defaultEasing;

        protected T _from;
        protected bool _liveFrom = true;

        internal PropertyTweener(Node target, string member, bool memberIsProperty, Easing defaultEasing) {
            _target = target;
            _member = member;
            _memberIsProperty = memberIsProperty;
            _defaultEasing = defaultEasing;
            _from = default;
            _liveFrom = true;
        }

        protected T GetFirstFromValue() => _liveFrom ? (T)_target.GetIndexed(_member) : _from;

        public void RunStep(Tween tween, Node target, string property,
            T from, T to, float start, float duration, Easing easing, TweenCallback callback) {
            easing ??= Easing.LinearInOut;
            if (!Object.IsInstanceValid(target)) {
                Logger.Warning("Can't create InterpolateProperty in a freed target instance");
                return;
            }
            var end = start + duration;
            Logger.Info("\"" + target.Name + "\" " + target.GetType().Name + "." + property + ": " +
                        from + " to " + to +
                        " Start: " + start.ToString("F") +
                        " End: " + end.ToString("F") +
                        " (+" + duration.ToString("F") + ") " + easing.Name);

            if (easing is GodotEasing godotEasing) {
                tween.InterpolateProperty(target, property, from, to, duration,
                    godotEasing.TransitionType, godotEasing.EaseType, start);
            } else if (easing is BezierCurve bezierCurve) {
                TweenPropertyMethodHolder<float> tweenPropertyMethodHolder = new TweenPropertyMethodHolder<float>(
                    delegate(float value) {
                        var y = bezierCurve.GetY(value);
                        var lerp = GodotTools.LerpVariant(from, to, y);
                        // Logger.Debug(target.Name + "." + property + ": " + typeof(T).Name + " t:"+value+" y:"+lerp);
                        target.SetIndexed(property, lerp);
                    });
                tween.InterpolateMethod(tweenPropertyMethodHolder, nameof(TweenPropertyMethodHolder<T>.Call),
                    0f, 1f, duration, Tween.TransitionType.Linear, Tween.EaseType.InOut, start);
            }
            if (callback != null) {
                TweenCallbackHolder holder = new TweenCallbackHolder(callback);
                tween.InterpolateCallback(holder, start, nameof(TweenCallbackHolder.Call));
            }
        }
    }

    public class PropertyKeyStepTweener<T> : PropertyTweener<T>, ITweener {
        protected readonly List<AnimationKeyStep<T>> _steps = new List<AnimationKeyStep<T>>(5);

        internal PropertyKeyStepTweener(Node target, string member, bool memberIsProperty, Easing defaultEasing) : base(target,
            member, memberIsProperty, defaultEasing) {
        }

        public float Start(float initialDelay, Tween tween) {
            if (!Object.IsInstanceValid(_target)) {
                Logger.Warning("Can't create InterpolateProperty in a freed target instance");
                return 0;
            }
            var from = GetFirstFromValue();
            var startTime = 0f;
            // var totalDuration = _steps.Sum(step => step.Duration);
            foreach (var step in _steps) {
                var to = step.GetTo(from);
                var duration = step.Duration;
                // var percentStart = startTime / totalDuration;
                // var percentEnd = (startTime + duration) / totalDuration;
                RunStep(tween, _target, _member, from, to, initialDelay + startTime, duration,
                    step.Easing ?? _defaultEasing, step.Callback);
                from = to;
                startTime += duration;
            }
            return startTime;
        }
    }

    public class PropertyKeyPercentTweener<T> : PropertyTweener<T>, ITweener {
        protected readonly List<AnimationKeyPercent<T>> _steps = new List<AnimationKeyPercent<T>>(5);
        protected float AllStepsDuration = 0;

        internal PropertyKeyPercentTweener(Node target, string member, bool memberIsProperty, Easing defaultEasing) : base(
            target, member, memberIsProperty, defaultEasing) {
        }

        public float Start(float initialDelay, Tween tween) {
            if (!Object.IsInstanceValid(_target)) {
                Logger.Warning("Can't create InterpolateProperty in a freed target instance");
                return 0;
            }
            if (AllStepsDuration == 0) {
                throw new Exception("Keyframe animation duration should be more than 0");
            }
            if (_steps?.Last()?.Percent != 1f) {
                throw new Exception("Last step should be 1 keyframe");
            }
            var from = GetFirstFromValue();
            var startTime = 0f;
            // var percentStart = 0f;
            foreach (var step in _steps) {
                var to = step.GetTo(from);
                var endTime = step.Percent * AllStepsDuration;
                var duration = endTime - startTime;
                // var percentEnd = step.Percent;
                RunStep(tween, _target, _member, from, to, initialDelay + startTime, duration,
                    step.Easing ?? _defaultEasing, step.Callback);
                from = to;
                // percentStart = percentEnd;
                startTime = endTime;
            }
            return AllStepsDuration;
        }
    }

    public class PropertyKeyStepTweenerBuilder<T> : PropertyKeyStepTweener<T> {
        private readonly TweenSequenceBuilder _tweenSequenceBuilder;

        internal PropertyKeyStepTweenerBuilder(TweenSequenceBuilder tweenSequenceBuilder, Node target, string member,
            bool memberIsProperty, Easing defaultEasing) : base(target, member, memberIsProperty, defaultEasing) {
            _tweenSequenceBuilder = tweenSequenceBuilder;
        }

        public PropertyKeyStepTweenerBuilder<T> From(T from) {
            _from = from;
            _liveFrom = false;
            return this;
        }

        public PropertyKeyStepTweenerBuilder<T> To(T to, float duration, Easing easing = null,
            TweenCallback callback = null) {
            var animationStepPropertyTweener =
                new AnimationKeyStepTo<T>(to, duration, easing ?? _defaultEasing, callback);
            _steps.Add(animationStepPropertyTweener);
            return this;
        }

        public PropertyKeyStepTweenerBuilder<T> Offset(T offset, float duration, Easing easing = null,
            TweenCallback callback = null) {
            var animationStepPropertyTweener =
                new AnimationKeyStepOffset<T>(offset, duration, easing ?? _defaultEasing, callback);
            _steps.Add(animationStepPropertyTweener);
            return this;
        }

        public TweenSequenceBuilder EndAnimate() {
            return _tweenSequenceBuilder;
        }
    }

    public class PropertyKeyPercentTweenerBuilder<T> : PropertyKeyPercentTweener<T> {
        private readonly TweenSequenceBuilder _tweenSequenceBuilder;

        internal PropertyKeyPercentTweenerBuilder(TweenSequenceBuilder tweenSequenceBuilder, Node target, string member,
            bool memberIsProperty, Easing defaultEasing) : base(target, member, memberIsProperty, defaultEasing) {
            _tweenSequenceBuilder = tweenSequenceBuilder;
        }

        public PropertyKeyPercentTweenerBuilder<T> Duration(float duration) {
            AllStepsDuration = duration;
            return this;
        }

        public PropertyKeyPercentTweenerBuilder<T> From(T from) {
            _from = from;
            _liveFrom = false;
            return this;
        }

        public PropertyKeyPercentTweenerBuilder<T> KeyframeTo(float percentage, T to, Easing easing = null,
            TweenCallback callback = null) {
            if (percentage == 0f) {
                From(to);
            } else {
                var animationStepPropertyTweener =
                    new AnimationKeyPercentTo<T>(percentage, to, easing ?? _defaultEasing, callback);
                _steps.Add(animationStepPropertyTweener);
            }
            return this;
        }

        public PropertyKeyPercentTweenerBuilder<T> KeyframeOffset(float percentage, T offset, Easing easing = null,
            TweenCallback callback = null) {
            if (percentage == 0) {
                throw new Exception(
                    $"Can't set a 0% keyframe with offset. Use KeyframeTo(0,{offset}) or From({offset}) instead");
            }
            var animationStepPropertyTweener =
                new AnimationKeyPercentOffset<T>(percentage, offset, easing, callback);
            _steps.Add(animationStepPropertyTweener);
            return this;
        }

        public TweenSequenceBuilder EndAnimate() {
            return _tweenSequenceBuilder;
        }
    }

    public interface ITweenSequence {
        public List<List<ITweener>> TweenList { get; }
        public Tween.TweenProcessMode ProcessMode { get; }
        public int Loops { get; }
        public float Speed { get; }
    }

    public class TweenSequence : ITweenSequence {
        public List<List<ITweener>> TweenList { get; }
        public Tween.TweenProcessMode ProcessMode { get; set; } = Tween.TweenProcessMode.Physics;
        public int Loops { get; set; } = 1;
        public float Speed { get; set; } = 1.0f;

        public TweenSequence(List<List<ITweener>> tweenList) {
            TweenList = tweenList;
        }

        public TweenSequence(List<List<ITweener>> tweenList, Tween.TweenProcessMode processMode, int loops,
            float speed) {
            TweenList = tweenList;
            ProcessMode = processMode;
            Loops = loops;
            Speed = speed;
        }
    }

    public class TweenSequenceBuilder : TweenSequence {
        private bool _parallel = false;

        public TweenSequenceBuilder() : base(new List<List<ITweener>>()) {
        }

        public PropertyKeyStepTweenerBuilder<Color> AnimateColor(Node target, string property, Easing easing = null) {
            var tweener = new PropertyKeyStepTweenerBuilder<Color>(this, target, property, true, easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentTweenerBuilder<Color>
            KeyframeColor(Node target, string property, Easing easing = null) {
            var tweener = new PropertyKeyPercentTweenerBuilder<Color>(this, target, property, true, easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepTweenerBuilder<Vector2>
            AnimateVector2(Node target, string property, Easing easing = null) {
            var tweener = new PropertyKeyStepTweenerBuilder<Vector2>(this, target, property, true, easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentTweenerBuilder<Vector2> KeyframeVector2(Node target, string property,
            Easing easing = null) {
            var tweener = new PropertyKeyPercentTweenerBuilder<Vector2>(this, target, property, true, easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepTweenerBuilder<float> AnimateFloat(Node target, string property, Easing easing = null) {
            var tweener = new PropertyKeyStepTweenerBuilder<float>(this, target, property, true, easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentTweenerBuilder<float>
            KeyframeFloat(Node target, string property, Easing easing = null) {
            var tweener = new PropertyKeyPercentTweenerBuilder<float>(this, target, property, true, easing);
            AddTweener(tweener);
            return tweener;
        }

        public TweenSequenceBuilder Parallel() {
            _parallel = true;
            return this;
        }

        public TweenSequenceBuilder Pause(float delay) {
            AddTweener(new CallbackTweener(delay, null));
            return this;
        }

        public TweenSequenceBuilder Callback(TweenCallback callback) {
            AddTweener(new CallbackTweener(0, callback));
            return this;
        }

        private void AddTweener(ITweener tweener) {
            if (_parallel) {
                TweenList.Last().Add(tweener);
                _parallel = false;
            } else {
                TweenList.Add(new List<ITweener>(3) { tweener });
            }
        }

        // Sets the speed scale of tweening.
        public TweenSequenceBuilder SetSpeed(float speed) {
            Speed = speed;
            return this;
        }

        public TweenSequenceBuilder SetProcessMode(Tween.TweenProcessMode processMode) {
            ProcessMode = processMode;
            return this;
        }

        public TweenSequenceBuilder SetLoops(int maxLoops) {
            Loops = maxLoops;
            return this;
        }

        public virtual TweenPlayer EndSequence() {
            return null;
        }

        // public List<List<ITweener>> Export() {
        // new System.Collections.Immutable.ImmutableList()
        // return new
        // }

        // public TweenSequence Build() {
        // return new TweenSequence(TweenList, ProcessMode, Loops, Speed);
        // }
    }

    public class TweenPlayer : Reference {
        public class TweenSequenceWithPlayerBuilder : TweenSequenceBuilder {
            private readonly TweenPlayer _tweenPlayer;

            private TweenSequenceWithPlayerBuilder() {
            }

            internal TweenSequenceWithPlayerBuilder(TweenPlayer tweenPlayer) {
                _tweenPlayer = tweenPlayer;
            }

            public override TweenPlayer EndSequence() {
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
        private readonly List<ITweenSequence> _tweenSequences = new List<ITweenSequence>(6);

        private int _currentSequence = 0;
        private int _sequenceLoop = 0;
        private int _currentPlayerLoop = 0;
        private bool _started = false;
        private bool _running = false;

        public int Loops = 0;
        private bool _killWhenFinished = false;

        public TweenPlayer(string name) {
            Name = name;
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

        public TweenSequenceWithPlayerBuilder CreateSequence() {
            var tweenSequence = new TweenSequenceWithPlayerBuilder(this);
            AddSequence(tweenSequence);
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
            var tweenListCount = sequence.TweenList.Count;
            float accumulatedDelay = 0;
            for (var parallelGroupCount = 0; parallelGroupCount < tweenListCount; parallelGroupCount++) {
                var parallelGroup = sequence.TweenList[parallelGroupCount];

                if (parallelGroup.Count == 1) {
                    Logger.Debug("Start single tween " + (parallelGroupCount + 1) + "/" + sequence.TweenList.Count);
                    var tweenTime = parallelGroup[0].Start(accumulatedDelay, _tween);
                    Logger.Debug("Launched tween. Time: " + tweenTime.ToString("F") + "s");
                    accumulatedDelay += tweenTime;
                } else {
                    Logger.Debug("Start parallel tweens " + (parallelGroupCount + 1) + "/" + sequence.TweenList.Count +
                                 ": " + parallelGroup.Count + " in parallel:");

                    float longestTime = 0;
                    foreach (var tweener in parallelGroup) {
                        var tweenTime = tweener.Start(accumulatedDelay, _tween);
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