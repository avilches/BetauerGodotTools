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
        public Node? DefaultTarget { get; }
        public float Speed { get; }
        public float Duration { get; }
        public Tween.TweenProcessMode ProcessMode { get; }
        public SceneTreeTween Execute(float initialDelay = 0, Node? target = null, float duration = -1);
        public Action<Node>? StartAction { get; }
    }

    public interface ILoopedSequence : ISequence {
        public int Loops { get; }
    }

    public abstract class Sequence {
        public abstract ICollection<ICollection<ITweener>> TweenList { get; protected set; }
        public Node? DefaultTarget { get; protected set; }
        public abstract float Duration { get; protected set; }
        public abstract Action<Node> StartAction { get; protected set; }

        public SceneTreeTween Execute(float initialDelay = 0, Node? target = null, float duration = -1) {
            float accumulatedDelay = 0;
            var realTarget = DefaultTarget ?? target ?? throw new Exception("Sequence has no target and Execute() method does not provide a target"); 
            StartAction?.Invoke(realTarget);
            var sceneTreeTween = realTarget.CreateTween();
            foreach (var parallelGroup in TweenList) {
                float longestTime = 0;
                foreach (var tweener in parallelGroup) {
                    var tweenTime = tweener.Start(sceneTreeTween, initialDelay + accumulatedDelay, realTarget, 
                        duration > 0 ? duration : Duration);
                    longestTime = Math.Max(longestTime, tweenTime);
                }
                accumulatedDelay += longestTime;
            }
            return sceneTreeTween;
        }
    }

    /**
     * A immutable Sequence to allow templates. It doesn't have Target
     * Pay attention the internal _tweenList could be mutated. To protect this, when you use a template with
     * ImportTemplate, all data (except the _tweenList) is copied and the flag _importedFromTemplate is set to true,
     * so any future call to the AddTweener() will make a new copy of the internal collection.
     */
    public class SequenceTemplate : Sequence, ISequence {
        public override Action<Node>? StartAction { get; protected set; }
        private readonly ICollection<ICollection<ITweener>> _tweenList;

        public override ICollection<ICollection<ITweener>> TweenList {
            get => _tweenList;
            protected set => throw new ReadOnlyException();
        }

        public float Speed { get; }

        private readonly float _duration;

        public override float Duration {
            get => _duration;
            protected set => throw new ReadOnlyException();
        }

        public Tween.TweenProcessMode ProcessMode { get; }

        public SequenceTemplate(ICollection<ICollection<ITweener>> tweenList,
            float duration, float speed, Tween.TweenProcessMode processMode, Action<Node>? onStart) {
            _tweenList = tweenList;
            _duration = duration;
            Speed = speed;
            ProcessMode = processMode;
            StartAction = onStart;
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
            return new SequenceTemplate(from.TweenList, from.Duration, from.Speed, from.ProcessMode, from.StartAction);
        }

        public SceneTreeTween Play(Node node, float initialDelay = 0, float duration = -1) {
            return Play(1, node, initialDelay, duration);
        }

        public SceneTreeTween PlayForever(Node node, float initialDelay = 0, float duration = -1) {
            return Play(0, node, initialDelay, duration);
        }

        public SceneTreeTween Play(int loops, Node node, float initialDelay = 0, float duration = -1) {
            return Execute(initialDelay, node, duration).SetLoops(loops);
        }

        public SceneTreeTween MultiPlay(IEnumerable<Node> children, float delayPerTarget, float initialDelay = 0, float duration = -1) {
            var step = 0;
            // TODO: return only one, instead of the last SceneTreeTween
            SceneTreeTween sceneTreeTween = null;
            foreach (var child in children) {
                sceneTreeTween = Execute(initialDelay + (delayPerTarget * step), child, duration);
                step++;
            }
            return sceneTreeTween;
        }
    }

    public class MutableSequence : Sequence, ISequence {
        public override ICollection<ICollection<ITweener>> TweenList { get; protected set; }
        public override float Duration { get; protected set; }
        public float Speed { get; protected set; } = 1.0f;
        protected bool ImportedFromTemplate = false;
        public Tween.TweenProcessMode ProcessMode { get; protected set; } = Tween.TweenProcessMode.Idle;
        public override Action<Node> StartAction { get; protected set; }
    }

    /**
     * Shared between Regular (sequence builders w/o player) and template builder
     */
    public abstract class AbstractSequenceBuilder<TBuilder> : MutableSequence where TBuilder : class {
        protected bool _parallel = false;

        internal AbstractSequenceBuilder(bool createEmptyTweenList) {
            if (createEmptyTweenList) {
                TweenList = new List<ICollection<ITweener>>();
            }
        }

        public TBuilder Parallel() {
            _parallel = true;
            return this as TBuilder;
        }

        public TBuilder Pause(float delay) {
            if (delay > 0f) {
                AddTweener(new PauseTweener(delay));
            }
            return this as TBuilder;
        }

        // TODO: create a callback receiving the defaultNode, useful for templates
        public TBuilder Callback(Action callback, float delay = 0) {
            AddTweener(new CallbackTweener(delay, callback));
            return this as TBuilder;
        }

        public TBuilder Callback(Node target, string method, float delay = 0, params object[] binds) {
            AddTweener(new MethodCallbackTweener(delay, method, binds));
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

        public override Action<Node> StartAction { get; protected set; }

        public TBuilder OnStart(Action<Node> onStart) {
            StartAction = onStart;
            return this as TBuilder;
        }

        public TBuilder SetProcessMode(Tween.TweenProcessMode processMode) {
            ProcessMode = processMode;
            return this as TBuilder;
        }

        protected void AddTweener(ITweener tweener) {
            CloneTweenListIfNeeded();
            if (_parallel) {
                TweenList.Last().Add(tweener);
                _parallel = false;
            } else {
                TweenList.Add(new List<ITweener> { tweener });
            }
        }

        protected void CloneTweenListIfNeeded() {
            if (ImportedFromTemplate) {
                var tweenListCloned = new LinkedList<ICollection<ITweener>>(TweenList);
                if (_parallel) {
                    var lastParallelCloned = new List<ITweener>(tweenListCloned.Last());
                    tweenListCloned.RemoveLast();
                    tweenListCloned.AddLast(lastParallelCloned);
                }
                TweenList = tweenListCloned;
                ImportedFromTemplate = false;
            }
        }
    }

    public class TemplateBuilder : AbstractSequenceBuilder<TemplateBuilder> {
        private TemplateBuilder() : base(true /* true to allow add tweens during the template creation */) {
        }

        /*
         * AnimateSteps
         */

        public PropertyKeyStepToBuilder<TProperty, TemplateBuilder> AnimateSteps<TProperty>(
            Action<TProperty> property, Easing? easing = null) {
            var tweener = new PropertyKeyStepToBuilder<TProperty, TemplateBuilder>(this, new CallbackProperty<TProperty>(property), easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepToBuilder<TProperty, TemplateBuilder> AnimateSteps<TProperty>(
            Action<Node, TProperty> property, Easing? easing = null) {
            var tweener = new PropertyKeyStepToBuilder<TProperty, TemplateBuilder>(this, new NodeCallbackProperty<TProperty>(property), easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepToBuilder<TProperty, TemplateBuilder> AnimateSteps<TProperty>(
            string property, Easing? easing = null) {
            var tweener = new PropertyKeyStepToBuilder<TProperty, TemplateBuilder>(this, new IndexedProperty<TProperty>(property), easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepToBuilder<TProperty, TemplateBuilder> AnimateSteps<TProperty>(
            IProperty<TProperty> property, Easing? easing = null) {
            var tweener = new PropertyKeyStepToBuilder<TProperty, TemplateBuilder>(this, property, easing);
            AddTweener(tweener);
            return tweener;
        }

        /*
         * AnimateStepsBy
         */

        public PropertyKeyStepOffsetBuilder<TProperty, TemplateBuilder> AnimateStepsBy<TProperty>(
            Action<TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty, TemplateBuilder>(this, new CallbackProperty<TProperty>(property), easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TemplateBuilder> AnimateStepsBy<TProperty>(
             Action<Node, TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty, TemplateBuilder>(this, new NodeCallbackProperty<TProperty>(property), easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TemplateBuilder> AnimateStepsBy<TProperty>(
            string property, Easing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty, TemplateBuilder>(this, new IndexedProperty<TProperty>(property), easing, false);
            AddTweener(tweener);
            return tweener;
        }

        // TODO: not tested!
        public PropertyKeyStepOffsetBuilder<TProperty, TemplateBuilder> AnimateStepsBy<TProperty>(
            IProperty<TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty, TemplateBuilder>(this, property, easing, false);
            AddTweener(tweener);
            return tweener;
        }

        /*
         * AnimateRelativeSteps
         */

        public PropertyKeyStepOffsetBuilder<TProperty, TemplateBuilder> AnimateRelativeSteps<TProperty>(
            Action<TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty, TemplateBuilder>(this, new CallbackProperty<TProperty>(property), easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TemplateBuilder> AnimateRelativeSteps<TProperty>(
            Action<Node, TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty, TemplateBuilder>(this, new NodeCallbackProperty<TProperty>(property), easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TemplateBuilder> AnimateRelativeSteps<TProperty>(
            string property, Easing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty, TemplateBuilder>(this, new IndexedProperty<TProperty>(property), easing, true);
            AddTweener(tweener);
            return tweener;
        }

        // TODO: not tested!
        public PropertyKeyStepOffsetBuilder<TProperty, TemplateBuilder> AnimateRelativeSteps<TProperty>(
            IProperty<TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty, TemplateBuilder>(this, property, easing, true);
            AddTweener(tweener);
            return tweener;
        }

        /*
         * AnimateKeys
         */

        public PropertyKeyPercentToBuilder<TProperty, TemplateBuilder> AnimateKeys<TProperty>(
            Action<TProperty> property, Easing? easing = null) {
            var tweener = new PropertyKeyPercentToBuilder<TProperty, TemplateBuilder>(this, new CallbackProperty<TProperty>(property), easing);
            AddTweener(tweener);
            return tweener;
        }


        public PropertyKeyPercentToBuilder<TProperty, TemplateBuilder> AnimateKeys<TProperty>(
            Action<Node, TProperty> property, Easing? easing = null) {
            var tweener = new PropertyKeyPercentToBuilder<TProperty, TemplateBuilder>(this, new NodeCallbackProperty<TProperty>(property), easing);
            AddTweener(tweener);
            return tweener;
        }


        public PropertyKeyPercentToBuilder<TProperty, TemplateBuilder> AnimateKeys<TProperty>(
            string property, Easing? easing = null) {
            var tweener = new PropertyKeyPercentToBuilder<TProperty, TemplateBuilder>(this, new IndexedProperty<TProperty>(property), easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentToBuilder<TProperty, TemplateBuilder> AnimateKeys<TProperty>(
            IProperty<TProperty> property, Easing? easing = null) {
            var tweener = new PropertyKeyPercentToBuilder<TProperty, TemplateBuilder>(this, property, easing);
            AddTweener(tweener);
            return tweener;
        }

        /*
         * AnimateKeysBy
         */

        public PropertyKeyPercentOffsetBuilder<TProperty, TemplateBuilder> AnimateKeysBy<TProperty>(
            Action<TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty, TemplateBuilder>(this, new CallbackProperty<TProperty>(property), easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TemplateBuilder> AnimateKeysBy<TProperty>(
            Action<Node, TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty, TemplateBuilder>(this, new NodeCallbackProperty<TProperty>(property), easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TemplateBuilder> AnimateKeysBy<TProperty>(
            string property, Easing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty, TemplateBuilder>(this, new IndexedProperty<TProperty>(property), easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TemplateBuilder> AnimateKeysBy<TProperty>(
            IProperty<TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty, TemplateBuilder>(this, property, easing, false);
            AddTweener(tweener);
            return tweener;
        }

        /*
         * AnimateRelativeKeys
         */

        public PropertyKeyPercentOffsetBuilder<TProperty, TemplateBuilder> AnimateRelativeKeys<TProperty>(
            Action<TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty, TemplateBuilder>(this, new CallbackProperty<TProperty>(property), easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TemplateBuilder> AnimateRelativeKeys<TProperty>(
            Action<Node, TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty, TemplateBuilder>(this, new NodeCallbackProperty<TProperty>(property), easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TemplateBuilder> AnimateRelativeKeys<TProperty>(
            string property, Easing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty, TemplateBuilder>(this, new IndexedProperty<TProperty>(property), easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TemplateBuilder> AnimateRelativeKeys<TProperty>(
            IProperty<TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty, TemplateBuilder>(this, property, easing, true);
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
    public abstract class RegularSequenceBuilder<TBuilder> : AbstractSequenceBuilder<TBuilder>, ILoopedSequence
        where TBuilder : class {
        protected RegularSequenceBuilder(bool createEmptyTweenList) : base(createEmptyTweenList) {
        }

        public int Loops { get; protected set; }

        public TBuilder SetInfiniteLoops() {
            Loops = 0;
            return this as TBuilder;
        }

        public TBuilder SetLoops(int maxLoops) {
            Loops = maxLoops;
            return this as TBuilder;
        }

        public TBuilder ImportTemplate(SequenceTemplate sequence, Node target = null, float duration = -1) {
            DefaultTarget = target;

            if (TweenList == null || TweenList.Count == 0) {
                TweenList = sequence.TweenList;
                ImportedFromTemplate = true;
            } else {
                CloneTweenListIfNeeded();
                var first = true;
                foreach (var parallelGroup in sequence.TweenList) {
                    if (first && _parallel) {
                        // TODO: this not tested
                        foreach (var tweener in parallelGroup) {
                            TweenList.Last().Add(tweener);
                        }
                        _parallel = false;
                    } else {
                        TweenList.Add(parallelGroup);
                    }
                    first = false;
                }
            }
            Speed = sequence.Speed;
            ProcessMode = sequence.ProcessMode;
            Duration = duration > 0 ? duration : sequence.Duration;

            return this as TBuilder;
        }

        public TBuilder SetDefaultTarget(Node defaultTarget) {
            DefaultTarget = defaultTarget;
            return this as TBuilder;
        }

        /*
         * AnimateSteps
         */

        public PropertyKeyStepToBuilder<TProperty, TBuilder> AnimateSteps<TProperty>(
            Action<TProperty> property, Easing? easing = null) {
            var tweener = new PropertyKeyStepToBuilder<TProperty, TBuilder>(this, new CallbackProperty<TProperty>(property), easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepToBuilder<TProperty, TBuilder> AnimateSteps<TProperty>(
            Action<Node, TProperty> property, Easing? easing = null) {
            var tweener = new PropertyKeyStepToBuilder<TProperty, TBuilder>(this, new NodeCallbackProperty<TProperty>(property), easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepToBuilder<TProperty, TBuilder> AnimateSteps<TProperty>(
            string property, Easing? easing = null) {
            var tweener = new PropertyKeyStepToBuilder<TProperty, TBuilder>(this, new IndexedProperty<TProperty>(property), easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepToBuilder<TProperty, TBuilder> AnimateSteps<TProperty>(
            IProperty<TProperty> property, Easing? easing = null) {
            var tweener = new PropertyKeyStepToBuilder<TProperty, TBuilder>(this, property, easing);
            AddTweener(tweener);
            return tweener;
        }

        /*
         * AnimateStepsBy
         */

        public PropertyKeyStepOffsetBuilder<TProperty, TBuilder> AnimateStepsBy<TProperty>(
            Action<TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty, TBuilder>(this, new CallbackProperty<TProperty>(property), easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TBuilder> AnimateStepsBy<TProperty>(
            Action<Node, TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty, TBuilder>(this, new NodeCallbackProperty<TProperty>(property), easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TBuilder> AnimateStepsBy<TProperty>(
            string property, Easing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty, TBuilder>(this, new IndexedProperty<TProperty>(property), easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TBuilder> AnimateStepsBy<TProperty>(
            IProperty<TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty, TBuilder>(this, property, easing, false);
            AddTweener(tweener);
            return tweener;
        }

        /*
         * AnimateRelativeSteps
         */

        public PropertyKeyStepOffsetBuilder<TProperty, TBuilder> AnimateRelativeSteps<TProperty>(
            Action<TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty, TBuilder>(this, new CallbackProperty<TProperty>(property), easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TBuilder> AnimateRelativeSteps<TProperty>(
            Action<Node, TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty, TBuilder>(this, new NodeCallbackProperty<TProperty>(property), easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TBuilder> AnimateRelativeSteps<TProperty>(
            string property, Easing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty, TBuilder>(this, new IndexedProperty<TProperty>(property), easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TBuilder> AnimateRelativeSteps<TProperty>(
            IProperty<TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty, TBuilder>(this, property, easing, true);
            AddTweener(tweener);
            return tweener;
        }

        /*
         * AnimateKeys
         */

        public PropertyKeyPercentToBuilder<TProperty, TBuilder> AnimateKeys<TProperty>(
            Action<TProperty> property, Easing? easing = null) {
            var tweener = new PropertyKeyPercentToBuilder<TProperty, TBuilder>(this, new CallbackProperty<TProperty>(property), easing);
            AddTweener(tweener);
            return tweener;
        }


        public PropertyKeyPercentToBuilder<TProperty, TBuilder> AnimateKeys<TProperty>(
            Action<Node, TProperty> property, Easing? easing = null) {
            var tweener = new PropertyKeyPercentToBuilder<TProperty, TBuilder>(this, new NodeCallbackProperty<TProperty>(property), easing);
            AddTweener(tweener);
            return tweener;
        }


        public PropertyKeyPercentToBuilder<TProperty, TBuilder> AnimateKeys<TProperty>(
            string property, Easing? easing = null) {
            var tweener = new PropertyKeyPercentToBuilder<TProperty, TBuilder>(this, new IndexedProperty<TProperty>(property), easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentToBuilder<TProperty, TBuilder> AnimateKeys<TProperty>(
            IProperty<TProperty> property, Easing? easing = null) {
            var tweener = new PropertyKeyPercentToBuilder<TProperty, TBuilder>(this, property, easing);
            AddTweener(tweener);
            return tweener;
        }

        /*
         * AnimateKeysBy
         */

        public PropertyKeyPercentOffsetBuilder<TProperty, TBuilder> AnimateKeysBy<TProperty>(
            Action<TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty, TBuilder>(this, new CallbackProperty<TProperty>(property), easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TBuilder> AnimateKeysBy<TProperty>(
            Action<Node, TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty, TBuilder>(this, new NodeCallbackProperty<TProperty>(property), easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TBuilder> AnimateKeysBy<TProperty>(
            string property, Easing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty, TBuilder>(this, new IndexedProperty<TProperty>(property), easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TBuilder> AnimateKeysBy<TProperty>(
            IProperty<TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty, TBuilder>(this, property, easing, false);
            AddTweener(tweener);
            return tweener;
        }

        /*
         * AnimateRelativeKeys
         */

        public PropertyKeyPercentOffsetBuilder<TProperty, TBuilder> AnimateRelativeKeys<TProperty>(
            Action<TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty, TBuilder>(this, new CallbackProperty<TProperty>(property), easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TBuilder> AnimateRelativeKeys<TProperty>(
            Action<Node, TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty, TBuilder>(this, new NodeCallbackProperty<TProperty>(property), easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TBuilder> AnimateRelativeKeys<TProperty>(
            string property, Easing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty, TBuilder>(this, new IndexedProperty<TProperty>(property), easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TBuilder> AnimateRelativeKeys<TProperty>(
            IProperty<TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty, TBuilder>(this, property, easing, true);
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

        public static SequenceBuilder Create(Node target) {
            var sequenceBuilder = new SequenceBuilder(true /* true to allow add tweens */).SetDefaultTarget(target);
            return sequenceBuilder;
        }

        public SceneTreeTween Play(Node node, float initialDelay = 0, float duration = -1) {
            return Play(Loops, node, initialDelay, duration);
        }

        public SceneTreeTween PlayForever(Node node = null, float initialDelay = 0, float duration = -1) {
            return Play(0, node, initialDelay, duration);
        }

        public SceneTreeTween Play(int loops, Node node, float initialDelay = 0, float duration = -1) {
            return Execute(initialDelay, node, duration).SetLoops(loops);
        }
    }
}