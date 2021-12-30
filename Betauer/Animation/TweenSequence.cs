using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Godot;

namespace Betauer.Animation {
    public interface ITweenSequence {
        public ICollection<ICollection<ITweener>> TweenList { get; }
        public Node DefaultTarget { get; }
        public int Loops { get; }
        public float Speed { get; }
        public float Duration { get; }
        public Tween.TweenProcessMode ProcessMode { get; }
        public float Start(Tween tween, float initialDelay = 0, Node? target = null, float duration = -1);
    }

    public abstract class TweenSequence {
        public abstract ICollection<ICollection<ITweener>> TweenList { get; protected set; }
        public abstract Node DefaultTarget { get; protected set; }
        public abstract float Duration { get; protected set; }

        public float Start(Tween tween, float initialDelay, Node? target, float duration) {
            float accumulatedDelay = 0;
            foreach (var parallelGroup in TweenList) {
                float longestTime = 0;
                foreach (var tweener in parallelGroup) {
                    var tweenTime = tweener.Start(tween, initialDelay + accumulatedDelay, target ?? DefaultTarget,
                        duration > 0 ? duration : Duration);
                    longestTime = Math.Max(longestTime, tweenTime);
                }
                accumulatedDelay += longestTime;
            }
            return accumulatedDelay;
        }
    }

    /**
     * A immutable TweenSequence
     * Pay attention the internal _tweenList could be mutated. To protect this, when you use a template with
     * ImportTemplate, all data (except the _tweenList) is copied and the flag _importedFromTemplate is set to true,
     * so any future call to the AddTweener() will make a new copy of the internal collection.
     */
    public class TweenSequenceTemplate : TweenSequence, ITweenSequence {
        private readonly ICollection<ICollection<ITweener>> _tweenList;
        public override ICollection<ICollection<ITweener>> TweenList {
            get => _tweenList;
            protected set => throw new ReadOnlyException();
        }

        private readonly Node _defaultTarget;
        public override Node DefaultTarget {
            get => _defaultTarget;
            protected set => throw new ReadOnlyException();
        }
        public int Loops { get; }
        public float Speed { get; }

        private readonly float _duration;
        public override float Duration {
            get => _duration;
            protected set => throw new ReadOnlyException();
        }
        public Tween.TweenProcessMode ProcessMode { get; }

        public TweenSequenceTemplate(ICollection<ICollection<ITweener>> tweenList, Node defaultTarget,
            float duration, int loops, float speed, Tween.TweenProcessMode processMode) {
            _tweenList = tweenList;
            _defaultTarget = defaultTarget;
            _duration = duration;
            Loops = loops;
            Speed = speed;
            ProcessMode = processMode;
        }

        public static TweenSequenceTemplate Create(ITweenSequence from) {
            return new TweenSequenceTemplate(from.TweenList, from.DefaultTarget,
                from.Duration, from.Loops, from.Speed, from.ProcessMode);
        }
    }

    public class MutableTweenSequence : TweenSequence, ITweenSequence {
        public override ICollection<ICollection<ITweener>> TweenList { get; protected set; }
        public override Node DefaultTarget { get; protected set; }
        public override float Duration { get; protected set; } = -1.0f;
        public int Loops { get; protected set; } = 1;
        public float Speed { get; protected set; } = 1.0f;
        private protected bool _importedFromTemplate = false;
        public Tween.TweenProcessMode ProcessMode { get; protected set; } = Tween.TweenProcessMode.Idle;

        public void ImportTemplate(TweenSequenceTemplate tweenSequence, Node defaultTarget, float duration = -1) {
            TweenList = tweenSequence.TweenList;
            DefaultTarget = defaultTarget ?? tweenSequence.DefaultTarget;
            Loops = tweenSequence.Loops;
            Speed = tweenSequence.Speed;
            Duration = duration > 0 ? duration : tweenSequence.Duration;
            ProcessMode = tweenSequence.ProcessMode;
            _importedFromTemplate = true;
        }
    }

    public abstract class AbstractTweenSequenceBuilder<TBuilder> : MutableTweenSequence where TBuilder : class {
        private bool _parallel = false;

        internal AbstractTweenSequenceBuilder(ICollection<ICollection<ITweener>> tweenList) {
            TweenList = tweenList;
        }

        public TBuilder Parallel() {
            _parallel = true;
            return this as TBuilder;
        }

        public TBuilder Pause(float delay) {
            AddTweener(new PauseTweener(delay));
            return this as TBuilder;
        }

        public TBuilder Callback(Action callback, float delay = 0) {
            AddTweener(new CallbackTweener(delay, callback));
            return this as TBuilder;
        }

        public TBuilder SetSpeed(float speed) {
            Speed = speed;
            return this as TBuilder;
        }

        public TBuilder SetDuration(float duration) {
            // TODO: id duration is defined, every single duration should be fit on it
            Duration = duration;
            return this as TBuilder;
        }

        public TBuilder SetProcessMode(Tween.TweenProcessMode processMode) {
            ProcessMode = processMode;
            return this as TBuilder;
        }

        public TBuilder SetLoops(int maxLoops) {
            Loops = maxLoops;
            return this as TBuilder;
        }

        public TBuilder SetInfiniteLoops() {
            return SetLoops(-1);
        }

        public bool IsInfiniteLoops() {
            return Loops == -1;
        }

        public PropertyKeyStepToBuilder<TProperty, TBuilder> AnimateSteps<TProperty>(Node target = null,
            IProperty<TProperty> property = null, Easing easing = null) {
            var tweener = new PropertyKeyStepToBuilder<TProperty, TBuilder>(this, target, property, easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TBuilder> AnimateStepsBy<TProperty>(Node target = null,
            IProperty<TProperty> property = null, Easing easing = null) {
            var tweener = new PropertyKeyStepOffsetBuilder<TProperty, TBuilder>(this, target, property, easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TBuilder> AnimateRelativeSteps<TProperty>(Node target = null,
            IProperty<TProperty> property = null, Easing easing = null) {
            var tweener = new PropertyKeyStepOffsetBuilder<TProperty, TBuilder>(this, target, property, easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentToBuilder<TProperty, TBuilder> AnimateKeys<TProperty>(Node target = null,
            IProperty<TProperty> property = null, Easing easing = null) {
            var tweener = new PropertyKeyPercentToBuilder<TProperty, TBuilder>(this, target, property, easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TBuilder> AnimateKeysBy<TProperty>(Node target = null,
            IProperty<TProperty> property = null, Easing easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty, TBuilder>(this, target, property, easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TBuilder> AnimateRelativeKeys<TProperty>(Node target = null,
            IProperty<TProperty> property = null, Easing easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty, TBuilder>(this, target, property, easing, true);
            AddTweener(tweener);
            return tweener;
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
    }

    public class TemplateBuilder : AbstractTweenSequenceBuilder<TemplateBuilder> {
        private TemplateBuilder(ICollection<ICollection<ITweener>> tweenList) : base(tweenList) {
        }
        public static TemplateBuilder Create() {
            var tweenSequenceBuilder = new TemplateBuilder(new SimpleLinkedList<ICollection<ITweener>>());
            return tweenSequenceBuilder;
        }

        public TweenSequenceTemplate BuildTemplate() {
            return TweenSequenceTemplate.Create(this);
        }
    }

    public class TweenSequenceBuilder : AbstractTweenSequenceBuilder<TweenSequenceBuilder> {
        private TweenSequenceBuilder(ICollection<ICollection<ITweener>> tweenList) : base(tweenList) {
        }

        public static TweenSequenceBuilder Create() {
            var tweenSequenceBuilder = new TweenSequenceBuilder(new SimpleLinkedList<ICollection<ITweener>>());
            return tweenSequenceBuilder;
        }

        public SingleSequencePlayer CreatePlayer(Node node) {
            return SingleSequencePlayer.With(node, this);
        }

        public SingleSequencePlayer Play(Node node) {
            return CreatePlayer(node).Start();
        }
    }
}