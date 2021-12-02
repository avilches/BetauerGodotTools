using System.Collections.Generic;
using System.Linq;
using Godot;
using Array = Godot.Collections.Array;
using GodotObject = Godot.Object;

namespace Tools.Effects {
    public interface Tweener {
        public void _start(Tween tween);
    }

    public class PropertyTweener<T> : Tweener {
        private readonly GodotObject _target;
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
        public PropertyTweener(GodotObject target, NodePath property, T to_value, float duration) {
            _target = target;
            _property = property;
            _to = to_value;
            _continue = true;
            // _from is calculated when the tween start by default
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
            if (!GodotObject.IsInstanceValid(_target)) {
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
        GodotObject _target;
        float _delay;
        string _method;
        object[] _args;

        public CallbackTweener(GodotObject target, string method, params object[] args) {
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
            if (!GodotObject.IsInstanceValid(_target)) {
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

    public class CallbackDelegateTweener : GodotObject, Tweener {
        public delegate void Callback();

        private Callback _callback;
        float _delay;

        // Set delay after which the method will be called.
        public Tweener SetDelay(float d) {
            _delay = d;
            return this;
        }

        public CallbackDelegateTweener(Callback callback) {
            _callback = callback;
        }

        public void _start(Tween tween) {
            if (!IsInstanceValid(this)) {
                TweenSequence.Logger.Warning("Can't create InterpolateCallback (delegate) in a freed target (this) instance");
                return;
            }
            tween.InterpolateCallback(this, _delay, "_");
        }

        private void _() {
            _callback();
        }
    }

    public class MethodTweener<T> : GodotObject, Tweener {
        GodotObject _target;
        string _method;
        private T _from;
        private T _to;
        float _duration;
        private Tween.TransitionType _trans;
        Tween.EaseType _ease;

        private float _delay;

        public MethodTweener(GodotObject target, string method, T from_value, T to_value,
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

    public class MethodDelegateTweener<T> : GodotObject, Tweener {
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
                TweenSequence.Logger.Warning("Can't create InterpolateMethod (delegate) in a freed target (this) instance");
                return;
            }
            tween.InterpolateMethod(this, "_", _from, _to, _duration, _trans, _ease, _delay);
        }

        private void _(T value) {
            _methodCallback(value);
        }
    }

    public class TweenSequence : Reference {
        // Emited when one step of the sequence is finished.
        // [Signal]
        // delegate void step_finished(int idx);

        // Emited when a loop of the sequence is finished.
        // [Signal]
        // delegate void loop_finished();

        // Emitted when whole sequence is finished. Doesn't happen with infinite loops.
        // [Signal]
        // delegate void finished();

        private static Logger _logger;
        internal static Logger Logger => _logger ??= LoggerFactory.GetLogger(typeof(TweenSequence));

        public Tween _tween;
        public List<List<Tweener>> _tweeners = new List<List<Tweener>>(10);

        public int _current_step = 0;
        public int _loops = 0;
        public bool _autostart = false;
        public bool _started = false;
        public bool _running = false;

        public bool _kill_when_finised = true;
        public bool _parallel = false;

        public TweenSequence(Tween tween) {
            _tween = tween;
            _tween.Connect("tween_all_completed", this, nameof(OnFinishTween));
        }

        /*
         * Flow:
         * 1 Start <-> Stop
         * 2 Reset keeps the current status
         */

        public TweenSequence(Node node, bool deferredStart = false) {
            _tween = new Tween();
            // _tween.SetMeta("sequence", this);
            // node.CallDeferred("add_child", _tween);
            node.AddChild(_tween);
            if (deferredStart) {
                var binds = new Array();
                node.GetTree().Connect("idle_frame", this, nameof(Start), binds, (uint)ConnectFlags.Oneshot);
            }
            _tween.Connect("tween_all_completed", this, nameof(OnFinishTween));
        }

        // Adds a PropertyTweener for tweening properties.
        public PropertyTweener<float> Add(GodotObject target, NodePath property, float to_value, float duration) {
            var tweener = new PropertyTweener<float>(target, property, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyTweener<int> Add(GodotObject target, NodePath property, int to_value, float duration) {
            var tweener = new PropertyTweener<int>(target, property, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyTweener<bool> Add(GodotObject target, NodePath property, bool to_value, float duration) {
            var tweener = new PropertyTweener<bool>(target, property, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyTweener<Vector2> Add(GodotObject target, NodePath property, Vector2 to_value,
            float duration) {
            var tweener = new PropertyTweener<Vector2>(target, property, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyTweener<Vector3> Add(GodotObject target, NodePath property, Vector3 to_value,
            float duration) {
            var tweener = new PropertyTweener<Vector3>(target, property, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyTweener<Rect2> Add(GodotObject target, NodePath property, Rect2 to_value, float duration) {
            var tweener = new PropertyTweener<Rect2>(target, property, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyTweener<Transform2D> Add(GodotObject target, NodePath property, Transform2D to_value,
            float duration) {
            var tweener = new PropertyTweener<Transform2D>(target, property, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyTweener<Transform> Add(GodotObject target, NodePath property, Transform to_value,
            float duration) {
            var tweener = new PropertyTweener<Transform>(target, property, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyTweener<Quat> Add(GodotObject target, NodePath property, Quat to_value, float duration) {
            var tweener = new PropertyTweener<Quat>(target, property, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyTweener<Basis> Add(GodotObject target, NodePath property, Basis to_value, float duration) {
            var tweener = new PropertyTweener<Basis>(target, property, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyTweener<Color> Add(GodotObject target, NodePath property, Color to_value, float duration) {
            var tweener = new PropertyTweener<Color>(target, property, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        // Adds a PropertyTweener operating on relative values.
        public PropertyTweener<float> AddOffset(GodotObject target, NodePath property, float offset,
            float duration) {
            var tweener = new PropertyTweener<float>(target, property, offset, duration);
            tweener._relative = true;
            AddTweener(tweener);
            return tweener;
        }

        public PropertyTweener<int> AddOffset(GodotObject target, NodePath property, int offset, float duration) {
            var tweener = new PropertyTweener<int>(target, property, offset, duration);
            tweener._relative = true;
            AddTweener(tweener);
            return tweener;
        }

        public PropertyTweener<Color> AddOffset(GodotObject target, NodePath property, Color offset,
            float duration) {
            var tweener = new PropertyTweener<Color>(target, property, offset, duration);
            tweener._relative = true;
            AddTweener(tweener);
            return tweener;
        }

        public PropertyTweener<Vector2> AddOffset(GodotObject target, NodePath property, Vector2 offset,
            float duration) {
            var tweener = new PropertyTweener<Vector2>(target, property, offset, duration);
            tweener._relative = true;
            AddTweener(tweener);
            return tweener;
        }

        public PropertyTweener<Vector3> AddOffset(GodotObject target, NodePath property, Vector3 offset,
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
        public CallbackTweener AddCallback(GodotObject target, string method, params object[] args) {
            var tweener = new CallbackTweener(target, method, args);
            AddTweener(tweener);
            return tweener;
        }

        // Adds a CallbackTweener for calling methods on target object.
        public CallbackDelegateTweener AddCallback(CallbackDelegateTweener.Callback callback) {
            var tweener = new CallbackDelegateTweener(callback);
            AddTweener(tweener);
            return tweener;
        }

        // Adds a MethodTweener for tweening arbitrary values using methods.
        public MethodTweener<float> AddMethod(GodotObject target, string method, float from_value, float to_value,
            float duration) {
            var tweener = new MethodTweener<float>(target, method, from_value, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        // Adds a MethodTweener for tweening arbitrary values using methods.
        public MethodTweener<int> AddMethod(GodotObject target, string method, int from_value, int to_value,
            float duration) {
            var tweener = new MethodTweener<int>(target, method, from_value, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        // Adds a MethodTweener for tweening arbitrary values using methods.
        public MethodTweener<bool> AddMethod(GodotObject target, string method, bool from_value, bool to_value,
            float duration) {
            var tweener = new MethodTweener<bool>(target, method, from_value, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        // Adds a MethodTweener for tweening arbitrary values using methods.
        public MethodTweener<Vector2> AddMethod(GodotObject target, string method, Vector2 from_value,
            Vector2 to_value,
            float duration) {
            var tweener = new MethodTweener<Vector2>(target, method, from_value, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        // Adds a MethodTweener for tweening arbitrary values using methods.
        public MethodTweener<Vector3> AddMethod(GodotObject target, string method, Vector3 from_value,
            Vector3 to_value,
            float duration) {
            var tweener = new MethodTweener<Vector3>(target, method, from_value, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        // Adds a MethodTweener for tweening arbitrary values using methods.
        public MethodTweener<Rect2> AddMethod(GodotObject target, string method, Rect2 from_value, Rect2 to_value,
            float duration) {
            var tweener = new MethodTweener<Rect2>(target, method, from_value, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        // Adds a MethodTweener for tweening arbitrary values using methods.
        public MethodTweener<Transform2D> AddMethod(GodotObject target, string method, Transform2D from_value,
            Transform2D to_value,
            float duration) {
            var tweener = new MethodTweener<Transform2D>(target, method, from_value, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        // Adds a MethodTweener for tweening arbitrary values using methods.
        public MethodTweener<Transform> AddMethod(GodotObject target, string method, Transform from_value,
            Transform to_value,
            float duration) {
            var tweener = new MethodTweener<Transform>(target, method, from_value, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        // Adds a MethodTweener for tweening arbitrary values using methods.
        public MethodTweener<Quat> AddMethod(GodotObject target, string method, Quat from_value, Quat to_value,
            float duration) {
            var tweener = new MethodTweener<Quat>(target, method, from_value, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        // Adds a MethodTweener for tweening arbitrary values using methods.
        public MethodTweener<Basis> AddMethod(GodotObject target, string method, Basis from_value, Basis to_value,
            float duration) {
            var tweener = new MethodTweener<Basis>(target, method, from_value, to_value, duration);
            AddTweener(tweener);
            return tweener;
        }

        // Adds a MethodTweener for tweening arbitrary values using methods.
        public MethodTweener<Color> AddMethod(GodotObject target, string method, Color from_value, Color to_value,
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

        // Sets the speed scale of tweening.
        public TweenSequence SetSpeed(float speed) {
            _tween.PlaybackSpeed = speed;
            return this;
        }

        // Sets how many the sequence should repeat.
        // When used without arguments, sequence will run infinitely.
        public TweenSequence SetLoops(int loops = -1) {
            _loops = loops;
            return this;
        }

        // Starts the sequence manually, unless it"s already started.
        public TweenSequence Start() {
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

        // Returns whether the sequence is currently running.
        public bool IsRunning() {
            return _running;
        }

        // Pauses the execution of the tweens.
        public TweenSequence Stop() {
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
        public TweenSequence Reset() {
            if (!IsInstanceValid(_tween)) {
                Logger.Warning("Can't Reset TweenSequence in a freed Tween instance");
                return this;
            }
            _tween.StopAll();
            _tween.RemoveAll();
            _current_step = 0;
            if (_running) {
                RunNextStep();
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

        // Whether the Tween should be freed when sequence finishes.
        // Default is true. If set to false, sequence will restart on end.
        public void SetAutokill(bool autokill) {
            _kill_when_finised = autokill;
        }

        public TweenSequence AddTweener(Tweener tweener) {
            if (!IsInstanceValid(_tween)) {
                Logger.Warning("Can't Add Tweener in a freed Tween instance");
                return this;
            }
            if (_parallel) {
                _tweeners.Last().Add(tweener);
                _parallel = false;
            } else {
                _tweeners.Add(new List<Tweener>(3) { tweener });
            }
            return this;
        }

        private void RunNextStep() {
            if (_tweeners.Count == 0) {
                Logger.Warning("Sequence has no steps!");
                return;
            }
            var group = _tweeners[_current_step];
            foreach (var tweener in group) {
                tweener._start(_tween);
            }
            _tween.Start();
        }

        private void OnFinishTween() {
            // EmitSignal(nameof(step_finished), _current_step);
            _current_step += 1;

            if (_current_step == _tweeners.Count) {
                _loops -= 1;
                if (_loops == -1) {
                    // EmitSignal(nameof(finished));
                    if (_kill_when_finised) {
                        Kill();
                    } else {
                        Reset();
                    }
                } else {
                    // EmitSignal(nameof(loop_finished));
                    _current_step = 0;
                    RunNextStep();
                }
            } else {
                RunNextStep();
            }
        }
    }
}