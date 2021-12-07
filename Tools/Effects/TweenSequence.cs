using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Object = Godot.Object;

namespace Tools.Effects {
    public delegate void OnFinishTweenSequence(TweenSequence tweenSequence);

    public delegate void TweenCallback();


    public interface Tweener {
        public void _start(Tween tween);
    }

    public class PropertyTweener<T> : Tweener {
        private readonly Node _target;
        private readonly NodePath _property;
        private readonly T _to;
        private readonly float _duration;
        private T _from;
        Tween.TransitionType _trans;
        Tween.EaseType _ease;

        private float _delay;
        bool _continue = true;
        public bool _relative = false;

        // Sets custom starting value for the tweener.
        // By default, it starts from value at the start of this tweener.
        public PropertyTweener(Node target, NodePath property, T to, float duration) {
            _target = target;
            _property = property;
            _to = to;
            _continue = true; // continue true means _from is calculated when the tween start
            _duration = duration;
            _trans = Tween.TransitionType.Linear; // TRANS_LINEAR;
            _ease = Tween.EaseType.InOut; // EASE_IN_OUT;
        }

        // Sets the starting value to the current value,
        // i.e. value at the time of creating sequence.
        public PropertyTweener<T> From(T val) {
            _from = val;
            _continue = false;
            return this;
        }

        // Sets the starting value to the current value,
        // i.e. value at the time of creating sequence.
        public PropertyTweener<T> FromCurrent() {
            _from = (T)_target.GetIndexed(_property);
            _continue = false;
            return this;
        }

        // Sets transition type of this tweener, from Tween.TransitionType.
        public PropertyTweener<T> SetTrans(Tween.TransitionType t) {
            _trans = t;
            return this;
        }

        // Sets ease type of this tweener, from Tween.EaseType.
        public PropertyTweener<T> SetEase(Tween.EaseType e) {
            _ease = e;
            return this;
        }

        // Sets the delay after which this tweener will start.
        public PropertyTweener<T> SetDelay(float d) {
            _delay = d;
            return this;
        }

        void Tweener._start(Tween tween) {
            if (!Object.IsInstanceValid(_target)) {
                TweenSequence.Logger.Warning("Can't create InterpolateProperty in a freed target instance");
                return;
            }

            if (_continue) {
                _from = (T)_target.GetIndexed(_property);
            }
            object advancedTo = null;
            if (_relative) {
                if (_from is float fromFloat && _to is float toFloat) {
                    advancedTo = fromFloat + toFloat;
                } else if (_from is int fromInt && _to is int toInt) {
                    advancedTo = fromInt + toInt;
                } else if (_from is Color fromColor && _to is Color toColor) {
                    advancedTo = fromColor + toColor;
                } else if (_from is Vector2 fromVector2 && _to is Vector2 toVector2) {
                    advancedTo = fromVector2 + toVector2;
                } else if (_from is Vector3 fromVector3 && _to is Vector3 toVector3) {
                    advancedTo = fromVector3 + toVector3;
                }
            }
            if (advancedTo != null) {
                tween.InterpolateProperty(_target, _property, _from, advancedTo, _duration, _trans, _ease,
                    _delay);
            } else {
                // TweenSequence.Logger.Info(_target.Name+"."+_property+": "+typeof(T).Name+"("+_from+" -> "+_to+") in "+_delay.ToString("F")+"s");
                tween.InterpolateProperty(_target, _property, _from, _to, _duration, _trans, _ease, _delay);
            }
        }
    }

    // Generic tweener for creating delays in sequence.
    public class IntervalTweener : Reference, Tweener {
        private float _time;

        public IntervalTweener(float time) {
            _time = time;
        }

        public void _start(Tween tween) {
            tween.InterpolateCallback(this, _time, "_");
        }

        public void _() {
        }
    }

    public class CallbackTweener : Tweener {
        Node _target;
        float _delay;
        string _method;
        object[] _args;

        public CallbackTweener(Node target, string method, params object[] args) {
            _target = target;
            _method = method;
            _args = args;
        }

        // Set delay after which the method will be called.
        public Tweener SetDelay(float d) {
            _delay = d;
            return this;
        }

        public void _start(Tween tween) {
            if (!Object.IsInstanceValid(_target)) {
                TweenSequence.Logger.Warning("Can't create InterpolateCallback in a freed target instance");
                return;
            }
            tween.InterpolateCallback(_target, _delay, _method,
                SafeGetArg(0), SafeGetArg(1), SafeGetArg(2),
                SafeGetArg(3), SafeGetArg(4));
        }

        private object SafeGetArg(int i) {
            return i < _args.Length ? _args[i] : null;
        }
    }

    public class CallbackDelegateTweener : Node, Tweener {
        private TweenCallback _tweenCallback;
        float _delay;

        // Set delay after which the method will be called.
        public Tweener SetDelay(float d) {
            _delay = d;
            return this;
        }

        public CallbackDelegateTweener(TweenCallback tweenCallback) {
            _tweenCallback = tweenCallback;
        }

        public void _start(Tween tween) {
            if (!IsInstanceValid(this)) {
                TweenSequence.Logger.Warning(
                    "Can't create InterpolateCallback (delegate) in a freed target (this) instance");
                return;
            }
            tween.InterpolateCallback(this, _delay, "_");
        }

        private void _() {
            _tweenCallback?.Invoke();
        }
    }

    public class MethodTweener<T> : Node, Tweener {
        Node _target;
        string _method;
        private T _from;
        private T _to;
        float _duration;
        private Tween.TransitionType _trans;
        Tween.EaseType _ease;

        private float _delay;

        public MethodTweener(Node target, string method, T from_value, T to_value,
            float duration) {
            _target = target;
            _method = method;
            _from = from_value;
            _to = to_value;
            _duration = duration;
            _trans = Tween.TransitionType.Linear;
            _ease = Tween.EaseType.InOut;
        }

        // Sets transition type of this tweener, from Tween.TransitionType.
        public Tweener SetTrans(Tween.TransitionType t) {
            _trans = t;
            return this;
        }

        // Sets ease type of this tweener, from Tween.EaseType.
        public Tweener SetEase(Tween.EaseType e) {
            _ease = e;
            return this;
        }

        // Sets the delay after which this tweener will start.
        public Tweener SetDelay(float d) {
            _delay = d;
            return this;
        }

        public void _start(Tween tween) {
            if (!IsInstanceValid(_target)) {
                TweenSequence.Logger.Warning("Can't create InterpolateMethod in a freed target instance");
                return;
            }
            tween.InterpolateMethod(_target, _method, _from, _to, _duration, _trans, _ease, _delay);
        }
    }

    public class MethodDelegateTweener<T> : Node, Tweener {
        public delegate void MethodCallback(T value);

        private MethodCallback _methodCallback;
        private T _from;
        private T _to;
        float _duration;
        private Tween.TransitionType _trans;
        Tween.EaseType _ease;
        private float _delay;

        public MethodDelegateTweener(MethodCallback methodCallback, T from_value, T to_value, float duration) {
            _methodCallback = methodCallback;
            _from = from_value;
            _to = to_value;
            _duration = duration;
            _trans = Tween.TransitionType.Linear;
            _ease = Tween.EaseType.InOut;
        }

        // Sets transition type of this tweener, from Tween.TransitionType.
        public Tweener SetTrans(Tween.TransitionType t) {
            _trans = t;
            return this;
        }

        // Sets ease type of this tweener, from Tween.EaseType.
        public Tweener SetEase(Tween.EaseType e) {
            _ease = e;
            return this;
        }

        // Sets the delay after which this tweener will start.
        public Tweener SetDelay(float d) {
            _delay = d;
            return this;
        }

        public void _start(Tween tween) {
            if (!IsInstanceValid(this)) {
                TweenSequence.Logger.Warning(
                    "Can't create InterpolateMethod (delegate) in a freed target (this) instance");
                return;
            }
            tween.InterpolateMethod(this, "_", _from, _to, _duration, _trans, _ease, _delay);
        }

        private void _(T value) {
            _methodCallback(value);
        }
    }

    public class TweenSequence {
        internal static readonly Logger Logger = LoggerFactory.GetLogger(typeof(TweenSequence));

        internal readonly List<List<Tweener>> _tweeners = new List<List<Tweener>>(10);

        private bool _parallel = false;

        // Adds a PropertyTweener for tweening properties.
        public PropertyTweener<float> AddProperty(Node target, NodePath property, float to_value, float duration) {
            var tweener = new PropertyTweener<float>(target, property, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyTweener<int> AddProperty(Node target, NodePath property, int to_value, float duration) {
            var tweener = new PropertyTweener<int>(target, property, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyTweener<bool> AddProperty(Node target, NodePath property, bool to_value, float duration) {
            var tweener = new PropertyTweener<bool>(target, property, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyTweener<Vector2> AddProperty(Node target, NodePath property, Vector2 to_value,
            float duration) {
            var tweener = new PropertyTweener<Vector2>(target, property, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyTweener<Vector3> AddProperty(Node target, NodePath property, Vector3 to_value,
            float duration) {
            var tweener = new PropertyTweener<Vector3>(target, property, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyTweener<Rect2> AddProperty(Node target, NodePath property, Rect2 to_value, float duration) {
            var tweener = new PropertyTweener<Rect2>(target, property, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyTweener<Transform2D> AddProperty(Node target, NodePath property, Transform2D to_value,
            float duration) {
            var tweener = new PropertyTweener<Transform2D>(target, property, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyTweener<Transform> AddProperty(Node target, NodePath property, Transform to_value,
            float duration) {
            var tweener = new PropertyTweener<Transform>(target, property, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyTweener<Quat> AddProperty(Node target, NodePath property, Quat to_value, float duration) {
            var tweener = new PropertyTweener<Quat>(target, property, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyTweener<Basis> AddProperty(Node target, NodePath property, Basis to_value, float duration) {
            var tweener = new PropertyTweener<Basis>(target, property, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyTweener<Color> AddProperty(Node target, NodePath property, Color to_value, float duration) {
            var tweener = new PropertyTweener<Color>(target, property, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        // Adds a PropertyTweener operating on relative values.
        public PropertyTweener<float> AddOffset(Node target, NodePath property, float offset,
            float duration) {
            var tweener = new PropertyTweener<float>(target, property, offset, duration);
            tweener._relative = true;
            AddTweener(tweener);
            return tweener;
        }

        public PropertyTweener<int> AddOffset(Node target, NodePath property, int offset, float duration) {
            var tweener = new PropertyTweener<int>(target, property, offset, duration);
            tweener._relative = true;
            AddTweener(tweener);
            return tweener;
        }

        public PropertyTweener<Color> AddOffset(Node target, NodePath property, Color offset,
            float duration) {
            var tweener = new PropertyTweener<Color>(target, property, offset, duration);
            tweener._relative = true;
            AddTweener(tweener);
            return tweener;
        }

        public PropertyTweener<Vector2> AddOffset(Node target, NodePath property, Vector2 offset,
            float duration) {
            var tweener = new PropertyTweener<Vector2>(target, property, offset, duration);
            tweener._relative = true;
            AddTweener(tweener);
            return tweener;
        }

        public PropertyTweener<Vector3> AddOffset(Node target, NodePath property, Vector3 offset,
            float duration) {
            var tweener = new PropertyTweener<Vector3>(target, property, offset, duration);
            tweener._relative = true;
            AddTweener(tweener);
            return tweener;
        }

        // Adds an IntervalTweener for creating delay intervals.
        public IntervalTweener AddInterval(float time) {
            var tweener = new IntervalTweener(time);
            AddTweener(tweener);
            return tweener;
        }


        // Adds a CallbackTweener for calling methods on target object.
        public CallbackTweener AddCallback(Node target, string method, params object[] args) {
            var tweener = new CallbackTweener(target, method, args);
            AddTweener(tweener);
            return tweener;
        }

        // Adds a CallbackTweener for calling methods on target object.
        public CallbackDelegateTweener AddCallback(TweenCallback callback) {
            var tweener = new CallbackDelegateTweener(callback);
            AddTweener(tweener);
            return tweener;
        }

        // Adds a MethodTweener for tweening arbitrary values using methods.
        public MethodTweener<float> AddMethod(Node target, string method, float from_value, float to_value,
            float duration) {
            var tweener = new MethodTweener<float>(target, method, from_value, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        // Adds a MethodTweener for tweening arbitrary values using methods.
        public MethodTweener<int> AddMethod(Node target, string method, int from_value, int to_value,
            float duration) {
            var tweener = new MethodTweener<int>(target, method, from_value, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        // Adds a MethodTweener for tweening arbitrary values using methods.
        public MethodTweener<bool> AddMethod(Node target, string method, bool from_value, bool to_value,
            float duration) {
            var tweener = new MethodTweener<bool>(target, method, from_value, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        // Adds a MethodTweener for tweening arbitrary values using methods.
        public MethodTweener<Vector2> AddMethod(Node target, string method, Vector2 from_value,
            Vector2 to_value,
            float duration) {
            var tweener = new MethodTweener<Vector2>(target, method, from_value, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        // Adds a MethodTweener for tweening arbitrary values using methods.
        public MethodTweener<Vector3> AddMethod(Node target, string method, Vector3 from_value,
            Vector3 to_value,
            float duration) {
            var tweener = new MethodTweener<Vector3>(target, method, from_value, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        // Adds a MethodTweener for tweening arbitrary values using methods.
        public MethodTweener<Rect2> AddMethod(Node target, string method, Rect2 from_value, Rect2 to_value,
            float duration) {
            var tweener = new MethodTweener<Rect2>(target, method, from_value, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        // Adds a MethodTweener for tweening arbitrary values using methods.
        public MethodTweener<Transform2D> AddMethod(Node target, string method, Transform2D from_value,
            Transform2D to_value,
            float duration) {
            var tweener = new MethodTweener<Transform2D>(target, method, from_value, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        // Adds a MethodTweener for tweening arbitrary values using methods.
        public MethodTweener<Transform> AddMethod(Node target, string method, Transform from_value,
            Transform to_value,
            float duration) {
            var tweener = new MethodTweener<Transform>(target, method, from_value, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        // Adds a MethodTweener for tweening arbitrary values using methods.
        public MethodTweener<Quat> AddMethod(Node target, string method, Quat from_value, Quat to_value,
            float duration) {
            var tweener = new MethodTweener<Quat>(target, method, from_value, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        // Adds a MethodTweener for tweening arbitrary values using methods.
        public MethodTweener<Basis> AddMethod(Node target, string method, Basis from_value, Basis to_value,
            float duration) {
            var tweener = new MethodTweener<Basis>(target, method, from_value, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        // Adds a MethodTweener for tweening arbitrary values using methods.
        public MethodTweener<Color> AddMethod(Node target, string method, Color from_value, Color to_value,
            float duration) {
            var tweener = new MethodTweener<Color>(target, method, from_value, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        // Adds a MethodTweener for tweening arbitrary values using methods.
        public MethodDelegateTweener<float> AddMethod(MethodDelegateTweener<float>.MethodCallback methodCallback,
            float from_value, float to_value,
            float duration) {
            var tweener = new MethodDelegateTweener<float>(methodCallback, from_value, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        // Adds a MethodTweener for tweening arbitrary values using methods.
        public MethodDelegateTweener<int> AddMethod(MethodDelegateTweener<int>.MethodCallback methodCallback,
            int from_value, int to_value,
            float duration) {
            var tweener = new MethodDelegateTweener<int>(methodCallback, from_value, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        // Adds a MethodTweener for tweening arbitrary values using methods.
        public MethodDelegateTweener<bool> AddMethod(MethodDelegateTweener<bool>.MethodCallback methodCallback,
            bool from_value, bool to_value,
            float duration) {
            var tweener = new MethodDelegateTweener<bool>(methodCallback, from_value, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        // Adds a MethodTweener for tweening arbitrary values using methods.
        public MethodDelegateTweener<Vector2> AddMethod(MethodDelegateTweener<Vector2>.MethodCallback methodCallback,
            Vector2 from_value,
            Vector2 to_value,
            float duration) {
            var tweener = new MethodDelegateTweener<Vector2>(methodCallback, from_value, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        // Adds a MethodTweener for tweening arbitrary values using methods.
        public MethodDelegateTweener<Vector3> AddMethod(MethodDelegateTweener<Vector3>.MethodCallback methodCallback,
            Vector3 from_value,
            Vector3 to_value,
            float duration) {
            var tweener = new MethodDelegateTweener<Vector3>(methodCallback, from_value, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        // Adds a MethodTweener for tweening arbitrary values using methods.
        public MethodDelegateTweener<Rect2> AddMethod(MethodDelegateTweener<Rect2>.MethodCallback methodCallback,
            Rect2 from_value, Rect2 to_value,
            float duration) {
            var tweener = new MethodDelegateTweener<Rect2>(methodCallback, from_value, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        // Adds a MethodTweener for tweening arbitrary values using methods.
        public MethodDelegateTweener<Transform2D> AddMethod(
            MethodDelegateTweener<Transform2D>.MethodCallback methodCallback, Transform2D from_value,
            Transform2D to_value,
            float duration) {
            var tweener = new MethodDelegateTweener<Transform2D>(methodCallback, from_value, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        // Adds a MethodTweener for tweening arbitrary values using methods.
        public MethodDelegateTweener<Transform> AddMethod(
            MethodDelegateTweener<Transform>.MethodCallback methodCallback, Transform from_value,
            Transform to_value,
            float duration) {
            var tweener = new MethodDelegateTweener<Transform>(methodCallback, from_value, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        // Adds a MethodTweener for tweening arbitrary values using methods.
        public MethodDelegateTweener<Quat> AddMethod(MethodDelegateTweener<Quat>.MethodCallback methodCallback,
            Quat from_value, Quat to_value,
            float duration) {
            var tweener = new MethodDelegateTweener<Quat>(methodCallback, from_value, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        // Adds a MethodTweener for tweening arbitrary values using methods.
        public MethodDelegateTweener<Basis> AddMethod(MethodDelegateTweener<Basis>.MethodCallback methodCallback,
            Basis from_value, Basis to_value,
            float duration) {
            var tweener = new MethodDelegateTweener<Basis>(methodCallback, from_value, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        // Adds a MethodTweener for tweening arbitrary values using methods.
        public MethodDelegateTweener<Color> AddMethod(MethodDelegateTweener<Color>.MethodCallback methodCallback,
            Color from_value, Color to_value,
            float duration) {
            var tweener = new MethodDelegateTweener<Color>(methodCallback, from_value, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        // When used, next Tweener will be added as a parallel to previous one.
        // Example: sequence.parallel().append(...)
        public TweenSequence Parallel() {
            if (_tweeners.Count == 0) {
                _tweeners.Add(new List<Tweener>(3));
            }
            _parallel = true;
            return this;
        }

        public TweenSequence AddTweener(Tweener tweener) {
            if (_parallel) {
                _tweeners.Last().Add(tweener);
                _parallel = false;
            } else {
                _tweeners.Add(new List<Tweener>(3) { tweener });
            }
            return this;
        }

        public int Loops = -1;
        public float Speed = 1.0f;

        public int GetLoops() => Loops;
        public bool IsInfiniteLoop() => Loops == -1;


        // Sets the speed scale of tweening.
        public TweenSequence SetSpeed(float speed) {
            Speed = speed;
            return this;
        }

        public TweenSequence SetInfiniteLoops() {
            Loops = -1;
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
        private TweenSequence _tweenSequence;

        private int _currentStep = 0;
        private int _currentLoop = 0;
        private bool _started = false;
        private bool _running = false;

        public int Loops = 0;
        private bool _killWhenFinished = false;

        public TweenPlayer(Tween tween, string name = null) {
            Name = name ?? tween.Name;
            _tween = tween;
            _tween.Connect("tween_all_completed", this, nameof(OnTweenAllCompletedSignaled));
        }

        public TweenPlayer(Node node): this(new Tween()) {
            node.AddChild(_tween);
        }

        public TweenPlayer LoadSequence(TweenSequence tweenSequence) {
            Reset(tweenSequence);
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

        private void _loadSequence(TweenSequence tweenSequence) {
            _tweenSequence = tweenSequence;
            Loops = tweenSequence.Loops;
            _tween.PlaybackSpeed = tweenSequence.Speed;
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
                Logger.Warning("Can't Start TweenSequence in a freed Tween instance");
                return this;
            }
            if (!_started) {
                _started = true;
                _running = true;
                RunNextStep();
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
                Logger.Warning("Can't Stop TweenSequence in a freed Tween instance");
                return this;
            }
            if (_running) {
                _tween.StopAll();
                _running = false;
            }
            return this;
        }

        // Stops the sequence && resets it to the beginning.
        public TweenPlayer Reset(TweenSequence tweenSequence = null) {
            if (!IsInstanceValid(_tween)) {
                Logger.Warning("Can't Reset TweenSequence in a freed Tween instance");
                return this;
            }
            _tween.StopAll();
            _tween.RemoveAll();
            _currentStep = 0;
            _currentLoop = 0;
            if (tweenSequence != null) {
                _loadSequence(tweenSequence);
            }
            if (_running) {
                RunNextStep();
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

        private void RunNextStep() {
            // if (_tweeners.Count == 0) {
            // Logger.Warning("Sequence has no steps!");
            // return;
            // }
            if (_tweenSequence._tweeners.Count > _currentStep) {
                GD.Print("RunNextStep "+(1+_currentStep)+"/"+_tweenSequence._tweeners.Count);
                var group = _tweenSequence._tweeners[_currentStep];
                foreach (var tweener in group) {
                    tweener._start(_tween);
                }
                _tween.Start();
            } else {
                GD.Print("RunNextStep out of bounds!!");
            }
        }

        private List<OnFinishTweenSequence> _onFinishTween;

        public void AddOnFinishTween(OnFinishTweenSequence onFinishTweenSequence) {
            // An array it's needed because the TweenAnimation uses this callback to return from a finished Once tween
            // to the previous loop tween stored in the stack. So, if a user creates a sequence with something in
            // the OnFinishTween, and it adds this sequence to the TweenAnimation, the callback will be lost. So, with
            // an array, the TweenSequence can store the both OnFinishTween: the user one, and the TweenSequence one.

            // Pay attention that with TweenAnimation, all this callback can be used
            // - TweenSequence.OnFinishTween
            // - OnceTween (from TweenAnimation) OnEnd
            // The main difference is the OnEnd callback will be invoked in the TweenAnimation when a OnceTween is
            // finished or interrupted. But the TweenSequence.OnFinishTween callback will be invoked only when finished.
            if (_onFinishTween == null) {
                _onFinishTween = new List<OnFinishTweenSequence>(1) { onFinishTweenSequence };
            } else {
                _onFinishTween.Add(onFinishTweenSequence);
            }
        }


        private void OnTweenAllCompletedSignaled() {
            // EmitSignal(nameof(step_finished), _current_step);
            _currentStep++;
            if (_currentStep == _tweenSequence._tweeners.Count) {
                _currentLoop++;
                if (IsInfiniteLoop() || _currentLoop < Loops) {
                    // EmitSignal(nameof(loop_finished));
                    _currentStep = 0;
                    RunNextStep();
                } else {
                    // Reset keeps the state, so Reset() will play again the sequence, meaning it will never finish
                    Stop().Reset();
                    // It's very important the event must be called the last
                    // EmitSignal(nameof(finished));
                    _onFinishTween?.ForEach(callback => callback.Invoke(_tweenSequence));
                    if (_killWhenFinished) Kill();
                }
            } else {
                RunNextStep();
            }
        }
    }
}