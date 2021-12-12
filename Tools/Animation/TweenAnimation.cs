using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Object = Godot.Object;

namespace Tools.Animation {
    public delegate void OnFinishAnimationTweener(ITweenSequence tweenSequence);

    public interface ITweener {
        public float Start(Tween tween, float initialDelay, Node defaultTarget, string defaultMember,
            float defaultDuration);
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

        private readonly float _delay;

        internal CallbackTweener(float delay, TweenCallback callback) : base(callback) {
            _delay = delay;
        }

        public float Start(Tween tween, float initialDelay, Node defaultTarget, string defaultMember,
            float defaultDuration) {
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
        protected readonly string _member; // TODO: create method/delegate interpolator
        protected readonly Easing _defaultEasing;
        protected readonly Callback _defaultCallback; // TODO: implement, why not?


        protected T _from;
        protected bool _liveFrom = true;

        internal PropertyTweener(Node target, string member, Easing defaultEasing) {
            _target = target;
            _member = member;
            _defaultEasing = defaultEasing;
            _from = default;
            _liveFrom = true;
        }

        protected T GetFirstFromValue() => _liveFrom ? (T)_target.GetIndexed(_member) : _from;

        protected bool Validate(int count, Node target, string member) {
            if (count == 0) {
                throw new Exception("Cant' start an animation with 0 steps or keys");
            }
            if (target == null) {
                throw new Exception("No target defined for the tween");
            }
            if (member == null) {
                throw new Exception("No member defined for the tween");
            }
            if (!Object.IsInstanceValid(target)) {
                Logger.Warning("Can't create InterpolateProperty in a freed target instance");
                return false;
            }
            return true;
        }

        public void RunStep(Tween tween, Node target, string property,
            T from, T to, float start, float duration, Easing easing, TweenCallback callback) {
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
        protected readonly SimpleLinkedList<AnimationKeyStep<T>> _steps = new SimpleLinkedList<AnimationKeyStep<T>>();

        internal PropertyKeyStepTweener(Node target, string member, bool memberIsProperty, Easing defaultEasing) :
            base(target, member, defaultEasing) {
        }

        public float Start(Tween tween, float initialDelay, Node defaultTarget, string defaultMember,
            float defaultDuration) {
            var target = _target ?? defaultTarget;
            var member = _member ?? defaultMember;
            if (!Validate(_steps.Count, target, member)) return 0;
            // TODO: defaultDuration could be % or absolute or nothing
            var from = GetFirstFromValue();
            var startTime = 0f;
            // var totalDuration = _steps.Sum(step => step.Duration);
            foreach (var step in _steps) {
                var to = step.GetTo(from);
                var duration = step.Duration;
                // var percentStart = startTime / totalDuration;
                // var percentEnd = (startTime + duration) / totalDuration;
                var easing = step.Easing ?? _defaultEasing ?? Easing.LinearInOut;
                RunStep(tween, target, member, from, to, initialDelay + startTime, duration, easing, step.Callback);
                from = to;
                startTime += duration;
            }
            return startTime;
        }
    }

    public class PropertyKeyPercentTweener<T> : PropertyTweener<T>, ITweener {
        protected readonly SimpleLinkedList<AnimationKeyPercent<T>> _steps =
            new SimpleLinkedList<AnimationKeyPercent<T>>();

        protected float AllStepsDuration = 0;

        internal PropertyKeyPercentTweener(Node target, string member, bool memberIsProperty, Easing defaultEasing) :
            base(
                target, member, defaultEasing) {
        }

        public float Start(Tween tween, float initialDelay, Node defaultTarget, string defaultMember,
            float defaultDuration) {
            var target = _target ?? defaultTarget;
            var member = _member ?? defaultMember;
            if (!Validate(_steps.Count, target, member)) return 0;
            var allStepsDuration = AllStepsDuration > 0f ? AllStepsDuration : defaultDuration;
            if (allStepsDuration <= 0)
                throw new Exception("Keyframe animation duration should be more than 0");
            var from = GetFirstFromValue();
            var startTime = 0f;
            // var percentStart = 0f;
            foreach (var step in _steps) {
                var to = step.GetTo(from);
                var endTime = step.Percent * allStepsDuration;
                var keyDuration = endTime - startTime;
                var easing = step.Easing ?? _defaultEasing ?? Easing.LinearInOut;
                // var percentEnd = step.Percent;
                RunStep(tween, target, member, from, to, initialDelay + startTime, keyDuration, easing, step.Callback);
                from = to;
                // percentStart = percentEnd;
                startTime = endTime;
            }
            return allStepsDuration;
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
        public IList<IList<ITweener>> TweenList { get; }
        public Node DefaultTarget { get; }
        public string DefaultMember { get; }
        public int Loops { get; }
        public float Speed { get; }
        public float Duration { get; }
        public bool Template { get; }
        public Tween.TweenProcessMode ProcessMode { get; }
    }

    public class TweenSequence : ITweenSequence {
        public IList<IList<ITweener>> TweenList { get; protected set; }
        public Node DefaultTarget { get; protected set; }
        public string DefaultMember { get; protected set; }
        public float Duration { get; protected set; } = -1.0f;
        public int Loops { get; protected set; } = 1;
        public float Speed { get; protected set; } = 1.0f;
        public bool Template { get; protected set; } = false;
        public Tween.TweenProcessMode ProcessMode { get; protected set; } = Tween.TweenProcessMode.Physics;

        public void ImportSequence(ITweenSequence tweenSequence, Node defaultTarget, string defaultMember,
            float duration = -1.0f) {
            // TODO: move the creation of the readonly or the immutable to a new method to export safe
            // TweenList = ImmutableArray.Create<IList<ITweener>>(tweenSequence.TweenList.ToArray());
            Template = tweenSequence.Template;
            TweenList = tweenSequence.TweenList;
            DefaultTarget = defaultTarget ?? tweenSequence.DefaultTarget;
            DefaultMember = defaultMember ?? tweenSequence.DefaultMember;
            Duration = duration > 0 ? duration : tweenSequence.Duration;
            Loops = tweenSequence.Loops;
            ProcessMode = tweenSequence.ProcessMode;
        }
    }

    public class TweenSequenceBuilder : TweenSequence {
        private bool _parallel = false;
        private bool _template = false;

        public static TweenSequenceBuilder Create() {
            return new TweenSequenceBuilder(true);
        }

        internal TweenSequenceBuilder(bool template) {
            _template = template;
            TweenList = new List<IList<ITweener>>();
        }

        public PropertyKeyStepTweenerBuilder<T> AnimateSteps<T>(Node target = null, string property = null,
            Easing easing = null) {
            var tweener = new PropertyKeyStepTweenerBuilder<T>(this, target, property, true, easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentTweenerBuilder<T> AnimateKeys<T>(Node target = null, string property = null,
            Easing easing = null) {
            var tweener = new PropertyKeyPercentTweenerBuilder<T>(this, target, property, true, easing);
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
            if (Template) {
                throw new Exception("Can't add twenners to a TweenSequence.Template = true animation");
            }
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

        public TweenSequenceBuilder SetDuration(float duration) {
            Duration = duration;
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

        public ITweenSequence Build() {
            if (_template) {
                Template = true;
            }
            return this;
        }
    }

    public class TweenPlayer : Reference {
        public class TweenSequenceWithPlayerBuilder : TweenSequenceBuilder {
            private readonly TweenPlayer _tweenPlayer;

            internal TweenSequenceWithPlayerBuilder(TweenPlayer tweenPlayer) : base(false) {
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
        public readonly List<ITweenSequence> _tweenSequences = new List<ITweenSequence>(6);

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

        public TweenSequenceWithPlayerBuilder ImportSequence(ITweenSequence tweenSequence,
            Node defaultTarget = null, string defaultMember = null, float duration = -1) {
            // int loops = 1, float speed = 1.0f, Tween.TweenProcessMode processMode = Tween.TweenProcessMode.Physics) {
            var tweenSequenceWithPlayerBuilder = new TweenSequenceWithPlayerBuilder(this);
            tweenSequenceWithPlayerBuilder.ImportSequence(tweenSequence, defaultTarget, defaultMember, duration);
            _tweenSequences.Add(tweenSequenceWithPlayerBuilder);
            return tweenSequenceWithPlayerBuilder;
        }

        public TweenSequenceWithPlayerBuilder CreateSequence() {
            var tweenSequence = new TweenSequenceWithPlayerBuilder(this);
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
            var tweenListCount = sequence.TweenList.Count;
            float accumulatedDelay = 0;
            for (var parallelGroupCount = 0; parallelGroupCount < tweenListCount; parallelGroupCount++) {
                var parallelGroup = sequence.TweenList[parallelGroupCount];

                if (parallelGroup.Count == 1) {
                    Logger.Debug("Start single tween " + (parallelGroupCount + 1) + "/" + sequence.TweenList.Count);
                    var tweenTime = parallelGroup[0].Start(_tween, accumulatedDelay, sequence.DefaultTarget,
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