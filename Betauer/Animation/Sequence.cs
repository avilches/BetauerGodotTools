using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace Betauer.Animation {
    public interface ISequence {
        public ICollection<ICollection<ITweener>> TweenList { get; }
        public Node DefaultTarget { get; }
        public int Loops { get; }
        public bool IsInfiniteLoop { get; }
        public float Speed { get; }
        public float Duration { get; }
        public Tween.TweenProcessMode ProcessMode { get; }
        public float Execute(Tween tween, float initialDelay = 0, Node target = null, float duration = -1);
    }

    public abstract class Sequence {
        public abstract ICollection<ICollection<ITweener>> TweenList { get; protected set; }
        public abstract Node DefaultTarget { get; protected set; }
        public abstract float Duration { get; protected set; }

        public float Execute(Tween tween, float initialDelay = 0, Node target = null, float duration = -1) {
            float accumulatedDelay = 0;
            foreach (var parallelGroup in TweenList) {
                float longestTime = 0;
                foreach (var tweener in parallelGroup) {
                    var tweenTime = tweener.Start(tween, initialDelay + accumulatedDelay, tweener.Target ?? DefaultTarget ?? target,
                        duration > 0 ? duration : Duration);
                    longestTime = Math.Max(longestTime, tweenTime);
                }
                accumulatedDelay += longestTime;
            }
            return accumulatedDelay;
        }
    }

    /**
     * A immutable Sequence to allow templates. It doesn't have Target
     * Pay attention the internal _tweenList could be mutated. To protect this, when you use a template with
     * ImportTemplate, all data (except the _tweenList) is copied and the flag _importedFromTemplate is set to true,
     * so any future call to the AddTweener() will make a new copy of the internal collection.
     */
    public class SequenceTemplate : Sequence, ISequence {
        private readonly ICollection<ICollection<ITweener>> _tweenList;

        public override ICollection<ICollection<ITweener>> TweenList {
            get => _tweenList;
            protected set => throw new ReadOnlyException();
        }

        public override Node DefaultTarget { get; protected set; } // NO Target for templates
        public int Loops { get; }
        public bool IsInfiniteLoop => Loops == -1;
        public float Speed { get; }

        private readonly float _duration;

        public override float Duration {
            get => _duration;
            protected set => throw new ReadOnlyException();
        }

        public Tween.TweenProcessMode ProcessMode { get; }

        public SequenceTemplate(ICollection<ICollection<ITweener>> tweenList,
            float duration, int loops, float speed, Tween.TweenProcessMode processMode) {
            _tweenList = tweenList;
            _duration = duration;
            Loops = loops;
            Speed = speed;
            ProcessMode = processMode;
        }

        public static SequenceTemplate Create(ISequence from) {
            if (from.TweenList == null || from.TweenList.Count == 0) {
                throw new InvalidDataException("Template TweenList can not be empty");
            }
            if (from.DefaultTarget != null) {
                // This is impossible to happen without mutating the template with reflection because the
                // Target has private set and the mutator SetTarget() is defined in the RegularBuilder only
                throw new InvalidDataException("Templates shouldn't have a target defined");
            }
            return new SequenceTemplate(from.TweenList, from.Duration, from.Loops, from.Speed, from.ProcessMode);
        }

        public SingleSequencePlayer CreatePlayer(Node node) {
            return SingleSequencePlayer.Create(node, this);
        }

        public Task<SingleSequencePlayer> Play(Node node, float initialDelay = 0, float duration = -1) {
            return new SingleSequencePlayer()
                .CreateNewTween(node)
                .CreateSequence()
                .Pause(initialDelay)
                .ImportTemplate(this, node, duration)
                .EndSequence()
                .Start()
                .Await();
        }


    }

    public class MutableSequence : Sequence, ISequence {
        public override ICollection<ICollection<ITweener>> TweenList { get; protected set; }
        public override Node DefaultTarget { get; protected set; }
        public override float Duration { get; protected set; } = -1.0f;
        public int Loops { get; protected set; } = 1;
        public bool IsInfiniteLoop => Loops == -1;
        public float Speed { get; protected set; } = 1.0f;
        protected bool _importedFromTemplate = false;
        public Tween.TweenProcessMode ProcessMode { get; protected set; } = Tween.TweenProcessMode.Idle;
    }

    /**
     * Shared between Regular (sequence builders w/o player) and template builder
     */
    public abstract class AbstractSequenceBuilder<TBuilder> : MutableSequence where TBuilder : class {
        private bool _parallel = false;

        internal AbstractSequenceBuilder(bool createEmptyTweenList) {
            if (createEmptyTweenList) {
                TweenList = new SimpleLinkedList<ICollection<ITweener>>();
            }
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

        protected void AddTweener(ITweener tweener) {
            CloneTweenListIfNeeded();
            if (_parallel) {
                TweenList.Last().Add(tweener);
                _parallel = false;
            } else {
                TweenList.Add(new SimpleLinkedList<ITweener> { tweener });
            }
        }

        protected void CloneTweenListIfNeeded() {
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
        }
    }

    public class TemplateBuilder : AbstractSequenceBuilder<TemplateBuilder> {
        private TemplateBuilder() : base(true /* true to allow add tweens during the template creation */) {
        }

        public PropertyKeyStepToBuilder<TProperty, TemplateBuilder> AnimateSteps<TProperty>(
            IProperty<TProperty> property, Easing easing = null) {
            var tweener = new PropertyKeyStepToBuilder<TProperty, TemplateBuilder>(this, null, property, easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TemplateBuilder> AnimateStepsBy<TProperty>(
            IProperty<TProperty> property, Easing easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty, TemplateBuilder>(this, null, property, easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TemplateBuilder> AnimateRelativeSteps<TProperty>(
            IProperty<TProperty> property, Easing easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty, TemplateBuilder>(this, null, property, easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentToBuilder<TProperty, TemplateBuilder> AnimateKeys<TProperty>(
            IProperty<TProperty> property, Easing easing = null) {
            var tweener = new PropertyKeyPercentToBuilder<TProperty, TemplateBuilder>(this, null, property, easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TemplateBuilder> AnimateKeysBy<TProperty>(
            IProperty<TProperty> property, Easing easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty, TemplateBuilder>(this, null, property, easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TemplateBuilder> AnimateRelativeKeys<TProperty>(
            IProperty<TProperty> property, Easing easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty, TemplateBuilder>(this, null, property, easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public static TemplateBuilder Create() {
            return new TemplateBuilder();
        }

        public SequenceTemplate BuildTemplate() {
            return SequenceTemplate.Create(this);
        }
    }

    /**
     * This builder is shared between SequenceBuilder and the TweenPlayer.CreateSequence() builders
     */
    public abstract class RegularSequenceBuilder<TBuilder> : AbstractSequenceBuilder<TBuilder> where TBuilder : class {
        protected RegularSequenceBuilder(bool createEmptyTweenList) : base(createEmptyTweenList) {
        }

        public TBuilder ImportTemplate(SequenceTemplate sequence, Node target, float duration = -1) {
            DefaultTarget = target;

            if (TweenList == null || TweenList.Count == 0) {
                TweenList = sequence.TweenList;
                _importedFromTemplate = true;
            } else {
                CloneTweenListIfNeeded();
                foreach (var parallelGroup in sequence.TweenList) {
                    TweenList.Add(parallelGroup);
                }
            }
            Loops = sequence.Loops;
            Speed = sequence.Speed;
            ProcessMode = sequence.ProcessMode;
            Duration = duration > 0 ? duration : sequence.Duration;

            return this as TBuilder;
        }

        public TBuilder SetDefaultTarget(Node defaultTarget) {
            DefaultTarget = defaultTarget;
            return this as TBuilder;
        }

        public PropertyKeyStepToBuilder<TProperty, TBuilder> AnimateSteps<TProperty>(
            Node defaultTarget = null, IProperty<TProperty> property = null, Easing easing = null) {
            var tweener = new PropertyKeyStepToBuilder<TProperty, TBuilder>(this, defaultTarget, property, easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TBuilder> AnimateStepsBy<TProperty>(
            Node defaultTarget = null, IProperty<TProperty> property = null, Easing easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty, TBuilder>(this, defaultTarget, property, easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TBuilder> AnimateRelativeSteps<TProperty>(
            Node defaultTarget = null, IProperty<TProperty> property = null, Easing easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty, TBuilder>(this, defaultTarget, property, easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentToBuilder<TProperty, TBuilder> AnimateKeys<TProperty>(
            Node defaultTarget = null, IProperty<TProperty> property = null, Easing easing = null) {
            var tweener = new PropertyKeyPercentToBuilder<TProperty, TBuilder>(this, defaultTarget, property, easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TBuilder> AnimateKeysBy<TProperty>(
            Node defaultTarget = null, IProperty<TProperty> property = null, Easing easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty, TBuilder>(this, defaultTarget, property, easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TBuilder> AnimateRelativeKeys<TProperty>(
            Node defaultTarget = null, IProperty<TProperty> property = null, Easing easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty, TBuilder>(this, defaultTarget, property, easing, true);
            AddTweener(tweener);
            return tweener;
        }
    }

    /**
     * This the public builder to create sequences from games
     */
    public class SequenceBuilder : RegularSequenceBuilder<SequenceBuilder> {
        private SequenceBuilder(bool createEmptyTweenList) : base(createEmptyTweenList) {
        }

        public static SequenceBuilder Create(Node target = null) {
            var sequenceBuilder = new SequenceBuilder(true /* true to allow add tweens */).SetDefaultTarget(target);
            return sequenceBuilder;
        }

        public Task<SingleSequencePlayer> Play(Node node) {
            if (DefaultTarget == null) {
                SetDefaultTarget(node);
            }
            return new SingleSequencePlayer()
                .CreateNewTween(DefaultTarget)
                .WithSequence(this)
                .Start()
                .Await();
        }
    }
}