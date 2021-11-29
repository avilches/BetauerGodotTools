using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using Array = Godot.Collections.Array;
using Object = System.Object;

namespace Tools.Effects {
    public abstract class Tweener : Reference {
        internal abstract void _start(Tween tween);
    }

    public class PropertyTweener : Tweener {
        private readonly Godot.Object _target;
        private readonly NodePath _property;
        private readonly object _to;
        private readonly float _duration;
        private object _from;
        Tween.TransitionType _trans;
        Tween.EaseType _ease;

        private float _delay;
        bool _continue = true;
        public bool _advance = false;

        public PropertyTweener(Godot.Object target, NodePath property, object to_value, float duration) {
            System.Diagnostics.Debug.Assert(target != null, "Invalid target Object.");
            _target = target;
            _property = property;
            _from = _target.GetIndexed(property);
            _to = to_value;
            _duration = duration;
            _trans = Tween.TransitionType.Linear; // TRANS_LINEAR;
            _ease = Tween.EaseType.InOut; // EASE_IN_OUT;

            // Sets custom starting value for the tweener.
            // By default, it starts from value at the start of this tweener.
        }

        // Sets the starting value to the current value,
        // i.e. value at the time of creating sequence.
        public PropertyTweener from(object val) {
            _from = val;
            _continue = false;
            return this;
        }

        // Sets the starting value to the current value,
        // i.e. value at the time of creating sequence.
        public PropertyTweener from_current() {
            _continue = false;
            return this;

            // Sets transition type of this tweener, from Tween.TransitionType.
        }

        public PropertyTweener set_trans(Tween.TransitionType t) {
            _trans = t;
            return this;

            // Sets ease type of this tweener, from Tween.EaseType.
        }

        public PropertyTweener set_ease(Tween.EaseType e) {
            _ease = e;
            return this;

            // Sets the delay after which this tweener will start.
        }

        public PropertyTweener set_delay(float d) {
            _delay = d;
            return this;
        }

        internal override void _start(Tween tween) {
            if (!IsInstanceValid(_target)) {
                GD.PushWarning("Target object freed, aborting Tweener.");
                return;
            }
            if (_continue) {
                _from = _target.GetIndexed(_property);
            }
            if (_advance) {
                // tween.InterpolateProperty(_target, _property, _from, _from + _to, _duration, _trans, _ease,
                // _delay);
            } else {
                tween.InterpolateProperty(_target, _property, _from, _to, _duration, _trans, _ease, _delay);

                // Generic tweener for creating delays in sequence.
            }
        }
    }

    public class IntervalTweener : Tweener {
        private float _time;

        public IntervalTweener(float time) {
            _time = time;
        }

        internal override void _start(Tween tween) {
            tween.InterpolateCallback(this, _time, "_");
        }

        public void _() {
        }
    }

    public class CallbackTweener : Tweener {
        Godot.Object _target;
        float _delay;
        string _method;
        Array _args;

        public CallbackTweener(Godot.Object target, string method, Array args) {
            _target = target;
            _method = method;
            _args = args;

            // Set delay after which the method will be called.
        }

        public Tweener set_delay(float d) {
            _delay = d;
            return this;
        }

        internal override void _start(Tween tween) {
            if (!IsInstanceValid(_target)) {
                GD.PushWarning("Target object freed, aborting Tweener.");
                return;
            }
            tween.InterpolateCallback(_target, _delay, _method,
                SafeGetArg(0), SafeGetArg(1), SafeGetArg(2),
                SafeGetArg(3), SafeGetArg(4));
        }

        public Object SafeGetArg(int i) {
            if (i < _args.Count) {
                return _args[i];
            } else {
                return null;
                // Tweener for tweening arbitrary values using getter/setter method.
            }
        }
    }

    public class MethodTweener : Tweener {
        Godot.Object _target;
        string _method;
        private Object _from;
        private Object _to;
        float _duration;
        private Tween.TransitionType _trans;
        Tween.EaseType _ease;

        private float _delay;

        public MethodTweener(Godot.Object target, string method, Object from_value, Object to_value,
            float duration) {
            _target = target;
            _method = method;
            _from = from_value;
            _to = to_value;
            _duration = duration;
            _trans = Tween.TransitionType.Linear;
            _ease = Tween.EaseType.InOut;

            // Sets transition type of this tweener, from Tween.TransitionType.
        }

        public Tweener set_trans(Tween.TransitionType t) {
            _trans = t;
            return this;

            // Sets ease type of this tweener, from Tween.EaseType.
        }

        public Tweener set_ease(Tween.EaseType e) {
            _ease = e;
            return this;

            // Sets the delay after which this tweener will start.
        }

        public Tweener set_delay(float d) {
            _delay = d;
            return this;
        }

        internal override void _start(Tween tween) {
            if (!IsInstanceValid(_target)) {
                GD.PushWarning("Target object freed, aborting Tweener.");
                return;
            }
            tween.InterpolateMethod(_target, _method, _from, _to, _duration, _trans, _ease, _delay);

            // Emited when one step of the sequence is finished.
        }
    }

    public class TweenSequence : Reference {
        [Signal]
        delegate void step_finished(int idx);

        // Emited when a loop of the sequence is finished.
        [Signal]
        delegate void loop_finished();

        // Emitted when whole sequence is finished. Doesn't happen with inifnite loops.
        [Signal]
        delegate void finished();

        public SceneTree _tree;
        public Tween _tween;
        public List<List<Tweener>> _tweeners = new List<List<Tweener>>(10);

        public int _current_step = 0;
        public int _loops = 0;
        public bool _autostart = true;
        public bool _started = false;
        public bool _running = false;

        public bool _kill_when_finised = true;
        public bool _parallel = false;

        // You need to provide SceneTree to be used by the sequence.
        public TweenSequence(SceneTree tree) {
            _tree = tree;
            _tween = new Tween();
            _tween.SetMeta("sequence", this);
            _tree.Root.CallDeferred("add_child", _tween);

            var binds = new Array();
            _tree.Connect("idle_frame", this, "start", binds, (uint)ConnectFlags.Oneshot);
            _tween.Connect("tween_all_completed", this, "_step_complete");
        }

        // All Tweener-creating methods will return the Tweeners for further chained usage.
        // Appends a PropertyTweener for tweening properties.
        public PropertyTweener append(Godot.Object target, NodePath property, Object to_value, float duration) {
            var tweener = new PropertyTweener(target, property, to_value, duration);
            _add_tweener(tweener);
            return tweener;
        }

        // Appends a PropertyTweener operating on relative values.
        public PropertyTweener append_advance(Godot.Object target, NodePath property, Object by_value, float duration) {
            var tweener = new PropertyTweener(target, property, by_value, duration);
            tweener._advance = true;
            _add_tweener(tweener);
            return tweener;
        }

        // Appends an IntervalTweener for creating delay intervals.
        public IntervalTweener append_interval(float time) {
            var tweener = new IntervalTweener(time);
            _add_tweener(tweener);
            return tweener;
        }


        // Appends a CallbackTweener for calling methods on target object.
        public CallbackTweener append_callback(Godot.Object target, string method, Array args) {
            var tweener = new CallbackTweener(target, method, args);
            _add_tweener(tweener);
            return tweener;
        }

        // Appends a MethodTweener for tweening arbitrary values using methods.
        public MethodTweener append_method(Godot.Object target, string method, Object from_value, Object to_value,
            float duration) {
            var tweener = new MethodTweener(target, method, from_value, to_value, duration);
            _add_tweener(tweener);
            return tweener;
        }

        // When used, next Tweener will be added as a parallel to previous one.
        // Example: sequence.parallel().append(...)
        public TweenSequence parallel() {
            if (_tweeners.Count == 0) {
                _tweeners.Add(new List<Tweener>(3));
            }
            _parallel = true;
            return this;
        }

        // Alias to parallel(), except it won't work without first tweener.
        public TweenSequence join() {
            System.Diagnostics.Debug.Assert(_tweeners.Count == 0, "Cant join with empty sequence!");
            _parallel = true;
            return this;
        }


        // Sets the speed scale of tweening.
        public TweenSequence set_speed(float speed) {
            _tween.PlaybackSpeed = speed;
            return this;
        }

        // Sets how many the sequence should repeat.
        // When used without arguments, sequence will run infinitely.
        public TweenSequence set_loops(int loops = -1) {
            _loops = loops;
            return this;
        }

        // Whether the sequence should autostart || not.
        // Enabled by default.
        public TweenSequence set_autostart(bool autostart) {
            if (_autostart && !autostart) {
                _tree.Disconnect("idle_frame", this, "start");
            } else if (!_autostart && autostart) {
                var binds = new Array();
                _tree.Connect("idle_frame", this, "start", binds, (uint)ConnectFlags.Oneshot);
            }
            _autostart = autostart;
            return this;
        }

        // Starts the sequence manually, unless it"s already started.
        public void start() {
            System.Diagnostics.Debug.Assert(_tween != null, "Tween was removed!");
            System.Diagnostics.Debug.Assert(!_started, "Sequence already started!");
            _started = true;
            _running = true;
            _run_next_step();
        }

        // Returns whether the sequence is currently running.
        public bool is_running() {
            return _running;

        }

        // Pauses the execution of the tweens.
        public void pause() {
            System.Diagnostics.Debug.Assert(_tween != null, "Tween was removed!");
            System.Diagnostics.Debug.Assert(_running, "Sequence !running!");
            _tween.StopAll();
            _running = false;

        }

        // Resumes the execution of the tweens.
        public void resume() {
            System.Diagnostics.Debug.Assert(_tween != null, "Tween was removed!");
            System.Diagnostics.Debug.Assert(!_running, "Sequence already running!");
            _tween.ResumeAll();
            _running = true;

        }

        // Stops the sequence && resets it to the beginning.
        public void reset() {
            System.Diagnostics.Debug.Assert(_tween != null, "Tween was removed!");
            if (_running) {
                pause();
            }
            _started = false;
            _current_step = 0;
            _tween.ResetAll();
        }

        // Frees the underlying Tween. Sequence is unusable after this operation.
        public void kill() {
            System.Diagnostics.Debug.Assert(_tween != null, "Tween was already removed!");
            if (_running) {
                pause();
            }
            _tween.QueueFree();

        }

        // Whether the Tween should be freed when sequence finishes.
        // Default is true. If set to false, sequence will restart on end.
        public void set_autokill(bool autokill) {
            _kill_when_finised = autokill;
        }

        public void _add_tweener(Tweener tweener) {
            System.Diagnostics.Debug.Assert(_tween != null, "Tween was removed!");
            System.Diagnostics.Debug.Assert(!_started, "Can't append to a started sequence!");
            if (!_parallel) {
                _tweeners.Add(new List<Tweener>(3));
            }
            _tweeners.Last().Add(tweener);
            _parallel = false;
        }

        public void _run_next_step() {
            System.Diagnostics.Debug.Assert(_tweeners.Count != 0, "Sequence has no steps!");
            var group = _tweeners[_current_step];
            foreach (var tweener in group) {
                tweener._start(_tween);
            }
            _tween.Start();
        }

        public void _step_complete() {
            EmitSignal("step_finished", _current_step);
            _current_step += 1;

            if (_current_step == _tweeners.Count) {
                _loops -= 1;
                if (_loops == -1) {
                    EmitSignal("finished");
                    if (_kill_when_finised) {
                        kill();
                    } else {
                        reset();
                    }
                } else {
                    EmitSignal("loop_finished");
                    _current_step = 0;
                    _run_next_step();
                }
            } else {
                _run_next_step();
            }
        }
    }
}