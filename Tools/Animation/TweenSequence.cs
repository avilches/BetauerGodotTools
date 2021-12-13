using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Tools.Animation {
    public interface ITweenSequence {
        public ICollection<ICollection<ITweener>> TweenList { get; }
        public Node DefaultTarget { get; }
        public Property DefaultMember { get; }
        public int Loops { get; }
        public float Speed { get; }
        public float Duration { get; }
        public Tween.TweenProcessMode ProcessMode { get; }
    }

    /**
     * A readonly ITweenSequence
     * Pay attention the internal _tweenList could be mutable.
     *
     * To protect this, when you use a template with ImportTemplate, all data is copied and the flag
     * _importedFromTemplate is set to true, so, any future call to the AddTweener() will make a new copy of the
     * internal collection
     */
    public class TweenSequenceTemplate : ITweenSequence {
        private readonly ICollection<ICollection<ITweener>> _tweenList;
        private readonly Node _defaultTarget;
        private readonly Property _defaultMember;
        private readonly float _duration;
        private readonly int _loops;
        private readonly float _speed;
        private readonly Tween.TweenProcessMode _processMode;

        public ICollection<ICollection<ITweener>> TweenList => _tweenList;
        public Node DefaultTarget => _defaultTarget;
        public Property DefaultMember => _defaultMember;
        public float Duration => _duration;
        public int Loops => _loops;
        public float Speed => _speed;
        // public bool Template => true;
        public Tween.TweenProcessMode ProcessMode => _processMode;

        public TweenSequenceTemplate(ICollection<ICollection<ITweener>> tweenList, Node defaultTarget,
            Property defaultMember, float duration, int loops, float speed, Tween.TweenProcessMode processMode) {
            _tweenList = tweenList;
            _defaultTarget = defaultTarget;
            _defaultMember = defaultMember;
            _duration = duration;
            _loops = loops;
            _speed = speed;
            _processMode = processMode;
        }

        public static TweenSequenceTemplate Create(ITweenSequence from) {
            return new TweenSequenceTemplate(from.TweenList, from.DefaultTarget, from.DefaultMember,
                from.Duration, from.Loops, from.Speed, from.ProcessMode);
        }
    }

    public class TweenSequence : ITweenSequence {
        public ICollection<ICollection<ITweener>> TweenList { get; protected set; }
        public Node DefaultTarget { get; protected set; }
        public Property DefaultMember { get; protected set; }
        public float Duration { get; protected set; } = -1.0f;
        public int Loops { get; protected set; } = 1;
        public float Speed { get; protected set; } = 1.0f;
        private protected bool _importedFromTemplate = false;
        public Tween.TweenProcessMode ProcessMode { get; protected set; } = Tween.TweenProcessMode.Physics;

        public void ImportTemplate(TweenSequenceTemplate tweenSequence, Node defaultTarget, float duration = -1.0f) {
            TweenList = tweenSequence.TweenList;
            DefaultTarget = defaultTarget ?? tweenSequence.DefaultTarget;
            DefaultMember = tweenSequence.DefaultMember;
            Loops = tweenSequence.Loops;
            Speed = tweenSequence.Speed;
            Duration = duration > 0 ? duration : tweenSequence.Duration;
            ProcessMode = tweenSequence.ProcessMode;
            _importedFromTemplate = true;
        }
    }

    public class TweenSequenceBuilder : TweenSequence {
        private bool _parallel = false;

        public static TweenSequenceBuilder CreateTemplate() {
            var tweenSequenceBuilder = new TweenSequenceBuilder(new SimpleLinkedList<ICollection<ITweener>>());
            return tweenSequenceBuilder;
        }

        internal TweenSequenceBuilder(ICollection<ICollection<ITweener>> tweenList) {
            TweenList = tweenList;
        }

        public PropertyKeyStepTweenerBuilder<T> AnimateSteps<T>(Node target = null, Property<T> property = null,
            Easing easing = null) {
            var tweener = new PropertyKeyStepTweenerBuilder<T>(this, target, property, easing);
            AddTweener((ITweener)tweener);
            return tweener;
        }

        public PropertyKeyPercentTweenerBuilder<T> AnimateKeys<T>(Node target = null, Property<T> property = null,
            Easing easing = null) {
            var tweener = new PropertyKeyPercentTweenerBuilder<T>(this, target, property, easing);
            AddTweener((ITweener)tweener);
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
            if (_importedFromTemplate) {
                var tweenListCloned = new SimpleLinkedList<ICollection<ITweener>>(TweenList);
                if (_parallel) {
                    var lastParallelCloned = new SimpleLinkedList<ITweener>(tweenListCloned.Last());
                    tweenListCloned.RemoveEnd();
                    tweenListCloned.Add(lastParallelCloned);
                }
                TweenList = tweenListCloned;
                _importedFromTemplate = false;
            }
            if (_parallel) {
                TweenList.Last().Add(tweener);
                _parallel = false;
            } else {
                TweenList.Add(new SimpleLinkedList<ITweener> { tweener });
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

        public TweenSequenceTemplate Build() {
            return TweenSequenceTemplate.Create(this);
        }
    }
}