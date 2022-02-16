using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Betauer.Collections;
using Godot;

namespace Betauer.Animation {
    public interface ISequence {
        public ICollection<ICollection<ITweener>> TweenList { get; }
        public Node DefaultTarget { get; }
        public float Speed { get; }
        public float Duration { get; }
        public Tween.TweenProcessMode ProcessMode { get; }
        public float Execute(ActionTween tween, float initialDelay = 0, Node target = null, float duration = -1);
    }

    public interface ILoopedSequence : ISequence {
        public int Loops { get; }
    }

    public abstract class Sequence {
        public abstract ICollection<ICollection<ITweener>> TweenList { get; protected set; }
        public abstract Node DefaultTarget { get; protected set; }
        public abstract float Duration { get; protected set; }

        public float Execute(ActionTween tween, float initialDelay = 0, Node target = null, float duration = -1) {
            float accumulatedDelay = 0;
            foreach (var parallelGroup in TweenList) {
                float longestTime = 0;
                foreach (var tweener in parallelGroup) {
                    var tweenTime = tweener.Start(tween, initialDelay + accumulatedDelay,
                        DefaultTarget ?? target, duration > 0 ? duration : Duration);
                    longestTime = Math.Max(longestTime, tweenTime);
                }
                accumulatedDelay += longestTime;
            }
            return accumulatedDelay;
        }
    }


    public class LoopStatus {
        /// <summary>
        /// Returns the number of total Loops for the sequence. -1 means infinite loops.
        /// </summary>
        public int Loops;
        /// <summary>
        /// The current sequence to be executed the Loops  
        /// </summary>
        public readonly ISequence Sequence;
        /// <summary>
        /// Returns true if the sequence will be executed an infinite number of times.
        /// </summary>
        public bool IsInfiniteLoop => Loops == -1;
        /// <summary>
        /// This counter will be incremented in every loop until it reaches the Loops (or forever, if Loops is -1)
        /// </summary>
        public int LoopCounter { get; private set; }

        private readonly ActionTween _tween;
        private readonly Node _defaultTarget = null;
        private readonly float _duration = -1;
        private readonly TaskCompletionSource<LoopStatus> _promise = new TaskCompletionSource<LoopStatus>();
        private Action _onFinish;
        private bool _done = false;

        public LoopStatus(ActionTween tween, int loops, ISequence sequence, Node defaultTarget, float duration) {
            _tween = tween;
            Loops = loops;
            Sequence = sequence;
            _defaultTarget = defaultTarget;
            _duration = duration;
        }

        public LoopStatus OnFinish(Action onFinish) {
            _onFinish = onFinish;
            return this;
        }

        public LoopStatus Start(float initialDelay = 0) {
            if (_done) return this;
            _done = true;
            _tween.Start();
            ExecuteLoop(initialDelay);
            return this;
        }

        public LoopStatus End() {
            Loops = 0;
            return this;
        }

        private void ExecuteLoop(float delay) {
            var elapsed = Sequence.Execute(_tween, delay, _defaultTarget, _duration);
            _tween.ScheduleCallback(delay + elapsed, _FinishedLoop);
        }

        private void _FinishedLoop() {
            LoopCounter++;
            if (IsInfiniteLoop || LoopCounter < Loops) {
                ExecuteLoop(0f);
            } else {
                try {
                    _onFinish?.Invoke();
                } finally {
                    _promise.TrySetResult(this);
                }
            }
        }

        public Task<LoopStatus> Await() {
            return _promise.Task;
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
        public float Speed { get; }

        private readonly float _duration;

        public override float Duration {
            get => _duration;
            protected set => throw new ReadOnlyException();
        }

        public Tween.TweenProcessMode ProcessMode { get; }

        public SequenceTemplate(ICollection<ICollection<ITweener>> tweenList,
            float duration, float speed, Tween.TweenProcessMode processMode) {
            _tweenList = tweenList;
            _duration = duration;
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
            return new SequenceTemplate(from.TweenList, from.Duration, from.Speed, from.ProcessMode);
        }

        public SingleSequencePlayer CreatePlayer(Node node) {
            return SingleSequencePlayer.Create(node, this);
        }

        public LoopStatus Play(ActionTween tween, Node node, float initialDelay = 0, float duration = -1) {
            return Play(tween, 1, node, initialDelay, duration);
        }

        public LoopStatus PlayForever(ActionTween tween, Node node, float initialDelay = 0, float duration = -1) {
            return Play(tween, -1, node, initialDelay, duration);
        }

        public LoopStatus Play(ActionTween tween, int loops, Node node, float initialDelay = 0, float duration = -1) {
            LoopStatus loopStatus = new LoopStatus(tween, loops, this, node, duration);
            return loopStatus.Start(initialDelay);
        }
    }

    public class MutableSequence : Sequence, ISequence {
        public override ICollection<ICollection<ITweener>> TweenList { get; protected set; }
        public override Node DefaultTarget { get; protected set; }
        public override float Duration { get; protected set; } = -1.0f;
        public float Speed { get; protected set; } = 1.0f;
        protected bool ImportedFromTemplate = false;
        public Tween.TweenProcessMode ProcessMode { get; protected set; } = Tween.TweenProcessMode.Idle;
    }

    /**
     * Shared between Regular (sequence builders w/o player) and template builder
     */
    public abstract class AbstractSequenceBuilder<TBuilder> : MutableSequence where TBuilder : class {
        protected bool _parallel = false;

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

        public TBuilder Callback(Node target, string method, float delay = 0,
            object? p1 = null, object? p2 = null, object? p3 = null, object? p4 = null, object? p5 = null) {
            AddTweener(new MethodCallbackTweener(delay, target, method, p1, p2, p3, p4, p5));
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
            if (ImportedFromTemplate) {
                var tweenListCloned = new SimpleLinkedList<ICollection<ITweener>>(TweenList);
                if (_parallel) {
                    var lastParallelCloned = new SimpleLinkedList<ITweener>(tweenListCloned.Last());
                    tweenListCloned.RemoveEnd();
                    tweenListCloned.Add(lastParallelCloned);
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
            var tweener = new PropertyKeyStepToBuilder<TProperty, TemplateBuilder>(this, null, new CallbackProperty<TProperty>(property), easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepToBuilder<TProperty, TemplateBuilder> AnimateSteps<TProperty>(
            Action<Node, TProperty> property, Easing? easing = null) {
            var tweener = new PropertyKeyStepToBuilder<TProperty, TemplateBuilder>(this, null, new NodeCallbackProperty<TProperty>(property), easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepToBuilder<TProperty, TemplateBuilder> AnimateSteps<TProperty>(
            string property, Easing? easing = null) {
            var tweener = new PropertyKeyStepToBuilder<TProperty, TemplateBuilder>(this, null, new IndexedProperty<TProperty>(property), easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepToBuilder<TProperty, TemplateBuilder> AnimateSteps<TProperty>(
            IProperty<TProperty> property, Easing? easing = null) {
            var tweener = new PropertyKeyStepToBuilder<TProperty, TemplateBuilder>(this, null, property, easing);
            AddTweener(tweener);
            return tweener;
        }

        /*
         * AnimateStepsBy
         */

        public PropertyKeyStepOffsetBuilder<TProperty, TemplateBuilder> AnimateStepsBy<TProperty>(
            Action<TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty, TemplateBuilder>(this, null, new CallbackProperty<TProperty>(property), easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TemplateBuilder> AnimateStepsBy<TProperty>(
             Action<Node, TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty, TemplateBuilder>(this, null, new NodeCallbackProperty<TProperty>(property), easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TemplateBuilder> AnimateStepsBy<TProperty>(
            string property, Easing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty, TemplateBuilder>(this, null, new IndexedProperty<TProperty>(property), easing, false);
            AddTweener(tweener);
            return tweener;
        }

        // TODO: not tested!
        public PropertyKeyStepOffsetBuilder<TProperty, TemplateBuilder> AnimateStepsBy<TProperty>(
            IProperty<TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty, TemplateBuilder>(this, null, property, easing, false);
            AddTweener(tweener);
            return tweener;
        }

        /*
         * AnimateRelativeSteps
         */

        public PropertyKeyStepOffsetBuilder<TProperty, TemplateBuilder> AnimateRelativeSteps<TProperty>(
            Action<TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty, TemplateBuilder>(this, null, new CallbackProperty<TProperty>(property), easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TemplateBuilder> AnimateRelativeSteps<TProperty>(
            Action<Node, TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty, TemplateBuilder>(this, null, new NodeCallbackProperty<TProperty>(property), easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TemplateBuilder> AnimateRelativeSteps<TProperty>(
            string property, Easing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty, TemplateBuilder>(this, null, new IndexedProperty<TProperty>(property), easing, true);
            AddTweener(tweener);
            return tweener;
        }

        // TODO: not tested!
        public PropertyKeyStepOffsetBuilder<TProperty, TemplateBuilder> AnimateRelativeSteps<TProperty>(
            IProperty<TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty, TemplateBuilder>(this, null, property, easing, true);
            AddTweener(tweener);
            return tweener;
        }

        /*
         * AnimateKeys
         */

        public PropertyKeyPercentToBuilder<TProperty, TemplateBuilder> AnimateKeys<TProperty>(
            Action<TProperty> property, Easing? easing = null) {
            var tweener = new PropertyKeyPercentToBuilder<TProperty, TemplateBuilder>(this, null, new CallbackProperty<TProperty>(property), easing);
            AddTweener(tweener);
            return tweener;
        }


        public PropertyKeyPercentToBuilder<TProperty, TemplateBuilder> AnimateKeys<TProperty>(
            Action<Node, TProperty> property, Easing? easing = null) {
            var tweener = new PropertyKeyPercentToBuilder<TProperty, TemplateBuilder>(this, null, new NodeCallbackProperty<TProperty>(property), easing);
            AddTweener(tweener);
            return tweener;
        }


        public PropertyKeyPercentToBuilder<TProperty, TemplateBuilder> AnimateKeys<TProperty>(
            string property, Easing? easing = null) {
            var tweener = new PropertyKeyPercentToBuilder<TProperty, TemplateBuilder>(this, null, new IndexedProperty<TProperty>(property), easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentToBuilder<TProperty, TemplateBuilder> AnimateKeys<TProperty>(
            IProperty<TProperty> property, Easing? easing = null) {
            var tweener = new PropertyKeyPercentToBuilder<TProperty, TemplateBuilder>(this, null, property, easing);
            AddTweener(tweener);
            return tweener;
        }

        /*
         * AnimateKeysBy
         */

        public PropertyKeyPercentOffsetBuilder<TProperty, TemplateBuilder> AnimateKeysBy<TProperty>(
            Action<TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty, TemplateBuilder>(this, null, new CallbackProperty<TProperty>(property), easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TemplateBuilder> AnimateKeysBy<TProperty>(
            Action<Node, TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty, TemplateBuilder>(this, null, new NodeCallbackProperty<TProperty>(property), easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TemplateBuilder> AnimateKeysBy<TProperty>(
            string property, Easing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty, TemplateBuilder>(this, null, new IndexedProperty<TProperty>(property), easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TemplateBuilder> AnimateKeysBy<TProperty>(
            IProperty<TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty, TemplateBuilder>(this, null, property, easing, false);
            AddTweener(tweener);
            return tweener;
        }

        /*
         * AnimateRelativeKeys
         */

        public PropertyKeyPercentOffsetBuilder<TProperty, TemplateBuilder> AnimateRelativeKeys<TProperty>(
            Action<TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty, TemplateBuilder>(this, null, new CallbackProperty<TProperty>(property), easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TemplateBuilder> AnimateRelativeKeys<TProperty>(
            Action<Node, TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty, TemplateBuilder>(this, null, new NodeCallbackProperty<TProperty>(property), easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TemplateBuilder> AnimateRelativeKeys<TProperty>(
            string property, Easing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty, TemplateBuilder>(this, null, new IndexedProperty<TProperty>(property), easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TemplateBuilder> AnimateRelativeKeys<TProperty>(
            IProperty<TProperty> property, Easing? easing = null) {
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
    public abstract class RegularSequenceBuilder<TBuilder> : AbstractSequenceBuilder<TBuilder>, ILoopedSequence
        where TBuilder : class {
        protected RegularSequenceBuilder(bool createEmptyTweenList) : base(createEmptyTweenList) {
        }

        public int Loops { get; protected set; }

        public TBuilder SetInfiniteLoops() {
            Loops = -1;
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
            var tweener = new PropertyKeyStepToBuilder<TProperty, TBuilder>(this, null, new CallbackProperty<TProperty>(property), easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepToBuilder<TProperty, TBuilder> AnimateSteps<TProperty>(
            Node? defaultTarget, Action<Node, TProperty> property, Easing? easing = null) {
            var tweener = new PropertyKeyStepToBuilder<TProperty, TBuilder>(this, defaultTarget, new NodeCallbackProperty<TProperty>(property), easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepToBuilder<TProperty, TBuilder> AnimateSteps<TProperty>(
            Node? defaultTarget, string property, Easing? easing = null) {
            var tweener = new PropertyKeyStepToBuilder<TProperty, TBuilder>(this, defaultTarget, new IndexedProperty<TProperty>(property), easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepToBuilder<TProperty, TBuilder> AnimateSteps<TProperty>(
            Node? defaultTarget, IProperty<TProperty> property, Easing? easing = null) {
            var tweener = new PropertyKeyStepToBuilder<TProperty, TBuilder>(this, defaultTarget, property, easing);
            AddTweener(tweener);
            return tweener;
        }

        /*
         * AnimateStepsBy
         */

        public PropertyKeyStepOffsetBuilder<TProperty, TBuilder> AnimateStepsBy<TProperty>(
            Action<TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty, TBuilder>(this, null, new CallbackProperty<TProperty>(property), easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TBuilder> AnimateStepsBy<TProperty>(
            Node? defaultTarget, Action<Node, TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty, TBuilder>(this, defaultTarget, new NodeCallbackProperty<TProperty>(property), easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TBuilder> AnimateStepsBy<TProperty>(
            Node? defaultTarget, string property, Easing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty, TBuilder>(this, defaultTarget, new IndexedProperty<TProperty>(property), easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TBuilder> AnimateStepsBy<TProperty>(
            Node? defaultTarget, IProperty<TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty, TBuilder>(this, defaultTarget, property, easing, false);
            AddTweener(tweener);
            return tweener;
        }

        /*
         * AnimateRelativeSteps
         */

        public PropertyKeyStepOffsetBuilder<TProperty, TBuilder> AnimateRelativeSteps<TProperty>(
            Action<TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty, TBuilder>(this, null, new CallbackProperty<TProperty>(property), easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TBuilder> AnimateRelativeSteps<TProperty>(
            Node? defaultTarget, Action<Node, TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty, TBuilder>(this, defaultTarget, new NodeCallbackProperty<TProperty>(property), easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TBuilder> AnimateRelativeSteps<TProperty>(
            Node? defaultTarget, string property, Easing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty, TBuilder>(this, defaultTarget, new IndexedProperty<TProperty>(property), easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TBuilder> AnimateRelativeSteps<TProperty>(
            Node? defaultTarget, IProperty<TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty, TBuilder>(this, defaultTarget, property, easing, true);
            AddTweener(tweener);
            return tweener;
        }

        /*
         * AnimateKeys
         */

        public PropertyKeyPercentToBuilder<TProperty, TBuilder> AnimateKeys<TProperty>(
            Action<TProperty> property, Easing? easing = null) {
            var tweener = new PropertyKeyPercentToBuilder<TProperty, TBuilder>(this, null, new CallbackProperty<TProperty>(property), easing);
            AddTweener(tweener);
            return tweener;
        }


        public PropertyKeyPercentToBuilder<TProperty, TBuilder> AnimateKeys<TProperty>(
            Node? defaultTarget, Action<Node, TProperty> property, Easing? easing = null) {
            var tweener = new PropertyKeyPercentToBuilder<TProperty, TBuilder>(this, defaultTarget, new NodeCallbackProperty<TProperty>(property), easing);
            AddTweener(tweener);
            return tweener;
        }


        public PropertyKeyPercentToBuilder<TProperty, TBuilder> AnimateKeys<TProperty>(
            Node? defaultTarget, string property, Easing? easing = null) {
            var tweener = new PropertyKeyPercentToBuilder<TProperty, TBuilder>(this, defaultTarget, new IndexedProperty<TProperty>(property), easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentToBuilder<TProperty, TBuilder> AnimateKeys<TProperty>(
            Node? defaultTarget, IProperty<TProperty> property, Easing? easing = null) {
            var tweener = new PropertyKeyPercentToBuilder<TProperty, TBuilder>(this, defaultTarget, property, easing);
            AddTweener(tweener);
            return tweener;
        }

        /*
         * AnimateKeysBy
         */

        public PropertyKeyPercentOffsetBuilder<TProperty, TBuilder> AnimateKeysBy<TProperty>(
            Action<TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty, TBuilder>(this, null, new CallbackProperty<TProperty>(property), easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TBuilder> AnimateKeysBy<TProperty>(
            Node? defaultTarget, Action<Node, TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty, TBuilder>(this, defaultTarget, new NodeCallbackProperty<TProperty>(property), easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TBuilder> AnimateKeysBy<TProperty>(
            Node? defaultTarget, string property, Easing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty, TBuilder>(this, defaultTarget, new IndexedProperty<TProperty>(property), easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TBuilder> AnimateKeysBy<TProperty>(
            Node? defaultTarget, IProperty<TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty, TBuilder>(this, defaultTarget, property, easing, false);
            AddTweener(tweener);
            return tweener;
        }

        /*
         * AnimateRelativeKeys
         */

        public PropertyKeyPercentOffsetBuilder<TProperty, TBuilder> AnimateRelativeKeys<TProperty>(
            Action<TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty, TBuilder>(this, null, new CallbackProperty<TProperty>(property), easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TBuilder> AnimateRelativeKeys<TProperty>(
            Node? defaultTarget, Action<Node, TProperty> property, Easing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty, TBuilder>(this, defaultTarget, new NodeCallbackProperty<TProperty>(property), easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TBuilder> AnimateRelativeKeys<TProperty>(
            Node? defaultTarget, string property, Easing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty, TBuilder>(this, defaultTarget, new IndexedProperty<TProperty>(property), easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TBuilder> AnimateRelativeKeys<TProperty>(
            Node? defaultTarget, IProperty<TProperty> property, Easing? easing = null) {
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

        public LoopStatus Play(ActionTween tween, Node node, float initialDelay = 0, float duration = -1) {
            return Play(tween, Loops, node, initialDelay, duration);
        }

        public LoopStatus PlayForever(ActionTween tween, Node node = null, float initialDelay = 0, float duration = -1) {
            return Play(tween, -1, node, initialDelay, duration);
        }

        public LoopStatus Play(ActionTween tween, int loops, Node node, float initialDelay = 0, float duration = -1) {
            LoopStatus loopStatus = new LoopStatus(tween, loops, this, node, duration);
            return loopStatus.Start(initialDelay);
        }
    }
}