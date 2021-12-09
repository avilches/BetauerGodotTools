using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Object = Godot.Object;

namespace Tools.Animation {
    public delegate void OnFinishAnimationTweener(TweenSequence tweenSequence);

    internal interface ITweener {
        public float Start(float initialDelay, Tween tween);
    }

    public delegate void TweenCallback();

    public class AnimationStep<T> {
        public delegate void PropertyMethodCallback(T value);

        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(TweenSequence));

        // The "from" value is only known when the Tween start, so, if the step an offset, it step needs the "from"
        // to calculate the "to"
        private readonly bool _toIsOffset = false;
        private readonly T _offset; // _toIsOffset = true
        private readonly T _absoluteTo; // _toIsOffset = false
        internal T GetTo(T from) => _toIsOffset ? (T)GodotTools.SumVariant(from, _offset) : _absoluteTo;

        private readonly TweenCallback _callback;
        private readonly Tween.TransitionType _trans;
        private readonly Tween.EaseType _ease;
        private readonly BezierCurve _bezierCurve;

        // The step has the duration as a absolute time or a % of the total time
        private readonly float _durationOrPercentage;
        internal float GetRealDuration(float allStepsDuration = 1) => _durationOrPercentage * allStepsDuration;

        public AnimationStep(T absoluteToOrOffset, float durationOrPercentage, Tween.TransitionType trans,
            Tween.EaseType ease,
            BezierCurve bezierCurveCurve,
            TweenCallback callback, bool toIsOffset) {
            _durationOrPercentage = durationOrPercentage;
            _trans = trans;
            _ease = ease;
            _bezierCurve = bezierCurveCurve;
            _callback = callback;
            _toIsOffset = toIsOffset;
            if (toIsOffset) {
                _offset = absoluteToOrOffset;
            } else {
                _absoluteTo = absoluteToOrOffset;
            }
        }

        public void RunStep(T from, T absoluteTo, float start, float duration, Tween tween, Node target,
            string property) {
            if (!Object.IsInstanceValid(target)) {
                Logger.Warning("Can't create InterpolateProperty in a freed target instance");
                return;
            }
            var end = start + duration;
            Logger.Info(target.Name + "." + property + ": " + typeof(T).Name + " " +
                        from + " to " + absoluteTo +
                        " Start: " + start.ToString("F") +
                        " End: " + end.ToString("F") +
                        " (+" + duration.ToString("F") + ") " + _trans + "/" + _ease);

            if (_bezierCurve == null) {
                tween.InterpolateProperty(target, property, from, absoluteTo, duration, _trans, _ease, start);
            } else {
                // TODO: pending
                TweenPropertyMethodHolder<T> tweenPropertyMethodHolder = new TweenPropertyMethodHolder<T>(
                    delegate(T value) {
                        // Logger.Debug(""+value);
                    });
                tween.PlaybackProcessMode = Tween.TweenProcessMode.Physics;
                tween.InterpolateMethod(tweenPropertyMethodHolder, nameof(TweenPropertyMethodHolder<T>.Call), from,
                    absoluteTo, duration, Tween.TransitionType.Linear, Tween.EaseType.InOut, start);
            }
            if (_callback != null) {
                TweenCallbackHolder holder = new TweenCallbackHolder(_callback);
                tween.InterpolateCallback(holder, start, nameof(TweenCallbackHolder.Call));
            }
        }
    }

    internal class TweenCallbackHolder : Object {
        protected readonly TweenCallback Callback;

        public TweenCallbackHolder(TweenCallback callback) {
            Callback = callback;
        }

        internal void Call() {
            Callback?.Invoke();
        }
    }

    internal class TweenPropertyMethodHolder<T> : Object {
        private readonly AnimationStep<T>.PropertyMethodCallback _callback;

        public TweenPropertyMethodHolder(AnimationStep<T>.PropertyMethodCallback callback) {
            _callback = callback;
        }

        internal void Call(T value) {
            _callback.Invoke(value);
        }
    }


    internal class CallbackTweener : TweenCallbackHolder, ITweener {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(TweenSequence));

        // private readonly TweenSequence _tweenSequence;
        private readonly float _delay;
        internal CallbackTweener(TweenSequence tweenSequence, float delay, TweenCallback callback) : base(callback) {
            // _tweenSequence = tweenSequence;
            _delay = delay;
        }

        public float Start(float initialDelay, Tween tween) {
            var start = _delay + initialDelay;
            if (Callback != null) {
                Logger.Info("Scheduling callback with "+_delay+"s delay. Start: " + start.ToString("F"));
            } else {
                Logger.Info("Scheduling "+_delay+"s delay. Start: "+initialDelay.ToString("F")+" End: " + start.ToString("F"));

            }
            tween.InterpolateCallback(this, start, nameof(Call));
            return _delay;
        }
    }

    public class PropertyTweener<T> : ITweener {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(TweenSequence));
        private readonly TweenSequence _tweenSequence;

        private readonly Node _target;

        private readonly string _member;

        // TODO: create method/delegate interpolator
        private readonly bool _memberIsProperty;

        private readonly List<AnimationStep<T>> _steps = new List<AnimationStep<T>>(5);
        private readonly BezierCurve _bezierCurve;
        private readonly Tween.TransitionType _trans;
        private readonly Tween.EaseType _ease;

        private T _from;
        private bool _liveFrom = true;
        private float _allStepsDuration = 1;

        private PropertyTweener(TweenSequence tweenSequence, Node target, string member, bool memberIsProperty) {
            _tweenSequence = tweenSequence;
            _target = target;
            _member = member;
            _memberIsProperty = memberIsProperty;
            _from = default;
            _liveFrom = true;
        }

        public PropertyTweener(TweenSequence tweenSequence, Node target, string member, bool memberIsProperty,
            Tween.TransitionType trans, Tween.EaseType ease) : this(tweenSequence, target, member, memberIsProperty) {
            _trans = trans;
            _ease = ease;
        }

        public PropertyTweener(TweenSequence tweenSequence, Node target, string member, bool memberIsProperty,
            BezierCurve bezierCurve) : this(tweenSequence, target, member, memberIsProperty) {
            _bezierCurve = bezierCurve;
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

        /**
         * If you use Duration, then the To() and AddOffset() method will use percentages
         */
        public PropertyTweener<T> Duration(float duration) {
            _allStepsDuration = duration;
            return this;
        }

        public PropertyTweener<T> To(T offset, float durationOrPercent, TweenCallback callback = null) {
            return To(offset, durationOrPercent, _trans, _ease, callback);
        }

        public PropertyTweener<T> To(T offset, float durationOrPercent, Tween.TransitionType trans,
            TweenCallback callback = null) {
            return To(offset, durationOrPercent, trans, _ease, callback);
        }

        public PropertyTweener<T> To(T offset, float durationOrPercent, Tween.TransitionType trans, Tween.EaseType ease,
            TweenCallback callback = null) {
            var animationStepPropertyTweener =
                new AnimationStep<T>(offset, durationOrPercent, trans, ease, null, callback, false);
            _steps.Add(animationStepPropertyTweener);
            return this;
        }

        // TODO: can be the BezierCurve be a shortcut to Tween.TransitionType/EaseType ?
        // So we will have only one method for To and only one for Offset
        public PropertyTweener<T> To(T offset, float durationOrPercent, BezierCurve bezierCurve,
            TweenCallback callback = null) {
            var animationStepPropertyTweener = new AnimationStep<T>(offset, durationOrPercent,
                Tween.TransitionType.Linear,
                Tween.EaseType.InOut, bezierCurve, callback, false);
            _steps.Add(animationStepPropertyTweener);
            return this;
        }

        public PropertyTweener<T> Offset(T offset, float durationOrPercent, TweenCallback callback = null) {
            return Offset(offset, durationOrPercent, _trans, _ease, callback);
        }

        public PropertyTweener<T> Offset(T offset, float durationOrPercent, Tween.TransitionType trans,
            TweenCallback callback = null) {
            return Offset(offset, durationOrPercent, trans, _ease, callback);
        }

        public PropertyTweener<T> Offset(T offset, float durationOrPercent, BezierCurve bezierCurve,
            TweenCallback callback = null) {
            var step = new AnimationStep<T>(offset, durationOrPercent, Tween.TransitionType.Linear,
                Tween.EaseType.InOut, bezierCurve, callback, true);
            _steps.Add(step);
            return this;
        }

        public PropertyTweener<T> Offset(T offset, float durationOrPercent, Tween.TransitionType trans,
            Tween.EaseType ease,
            TweenCallback callback = null) {
            var step = new AnimationStep<T>(offset, durationOrPercent, trans, ease, null, callback, true);
            _steps.Add(step);
            return this;
        }

        public TweenSequence EndAnimate() {
            return _tweenSequence;
        }

        public float Start(float initialDelay, Tween tween) {
            if (!Object.IsInstanceValid(_target)) {
                Logger.Warning("Can't create InterpolateProperty in a freed target instance");
                return 0;
            }
            var from = GetFirstFromValue();
            var start = 0f;
            foreach (var step in _steps) {
                var to = step.GetTo(from);
                var duration = step.GetRealDuration(_allStepsDuration);
                step.RunStep(from, to, initialDelay + start, duration, tween, _target, _member);
                from = to;
                start += duration;
            }
            return start;
        }

        private T GetFirstFromValue() => _liveFrom ? (T)_target.GetIndexed(_member) : _from;
    }

    public class TweenSequence {
        internal static readonly Logger Logger = LoggerFactory.GetLogger(typeof(TweenSequence));
        internal readonly List<List<ITweener>> TweenList = new List<List<ITweener>>(10);

        // Used during build the sequence
        private bool _parallel = false;
        private TweenPlayer _tweenPlayer;

        public Tween.TweenProcessMode ProcessMode;
        public int Loops = 1;
        public float Speed = 1.0f;

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

        public PropertyTweener<float> AnimateFloat(Node target, string property,
            Tween.TransitionType trans = Tween.TransitionType.Linear, Tween.EaseType ease = Tween.EaseType.InOut) {
            var tweener = new PropertyTweener<float>(this, target, property, true, trans, ease);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyTweener<float> AnimateFloat(Node target, string property, BezierCurve bezierCurve) {
            var tweener = new PropertyTweener<float>(this, target, property, true, bezierCurve);
            AddTweener(tweener);
            return tweener;
        }

        public TweenSequence Parallel() {
            _parallel = true;
            return this;
        }

        public TweenSequence Pause(float delay) {
            AddTweener(new CallbackTweener(this, delay, null));
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

        // Sets the speed scale of tweening.
        public TweenSequence SetSpeed(float speed) {
            Speed = speed;
            return this;
        }

        public TweenSequence SetProcessMode(Tween.TweenProcessMode processMode) {
            ProcessMode = processMode;
            return this;
        }

        public TweenSequence SetLoops(int maxLoops) {
            Loops = maxLoops;
            return this;
        }

        internal void OnAddedToPlayer(TweenPlayer tweenPlayer) {
            _tweenPlayer = tweenPlayer;
        }

        public TweenPlayer EndSequence() {
            return _tweenPlayer;
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

        private readonly string Name;
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(TweenPlayer));

        private Tween _tween;
        private readonly List<TweenSequence> _tweenSequences = new List<TweenSequence>(6);

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

        public TweenPlayer AddSequence(TweenSequence tweenSequence) {
            _tweenSequences.Add(tweenSequence);
            // tweenSequence.OnAddedToPlayer(this);
            return this;
        }

        public TweenSequence CreateSequence() {
            var tweenSequence = new TweenSequence();
            // Only the TweenSequence created by this method will have the reference to the player (needed)
            tweenSequence.OnAddedToPlayer(this);
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
            GD.Print("RunSequence " + (1 + _currentSequence) + "/" + _tweenSequences.Count);
            float accumulated = 0;
            var currentSequence = _tweenSequences[_currentSequence];
            for (var parallelGroupCount = 0; parallelGroupCount < currentSequence.TweenList.Count; parallelGroupCount++) {
                var parallelGroup = currentSequence.TweenList[parallelGroupCount];
                var maxFinishTime = ScheduleSequenceTweens(parallelGroupCount, currentSequence, parallelGroup, accumulated);
                accumulated += maxFinishTime;
            }
            _tween.PlaybackSpeed = currentSequence.Speed;
            _tween.PlaybackProcessMode = currentSequence.ProcessMode;
            _tween.Start();
        }

        private float ScheduleSequenceTweens(int parallelGroupCount, TweenSequence currentSequence, List<ITweener> parallelGroup,
            float accumulated) {
            float maxFinishTime = 0;
            GD.Print("  Start Parallel group " + (parallelGroupCount + 1) + "/" + currentSequence.TweenList.Count +
                     ": " + parallelGroup.Count + " in parallel:");
            foreach (var tweener in parallelGroup) {
                var totalTime = tweener.Start(accumulated, _tween);
                GD.Print("      Launched tween. Time: " + totalTime.ToString("F") + "s");
                maxFinishTime = Math.Max(maxFinishTime, totalTime);
            }
            GD.Print("  End parallel group. Total time: " + maxFinishTime.ToString("F") + "s");
            return maxFinishTime;
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
                RunSequence();
                return;
            }

            _sequenceLoop = 0;
            _currentSequence++;
            if (_currentSequence < _tweenSequences.Count) {
                RunSequence();
                return;
            }

            _currentSequence = 0;
            _currentPlayerLoop++;
            if (IsInfiniteLoop() || _currentPlayerLoop < Loops) {
                // EmitSignal(nameof(loop_finished));
                RunSequence();
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