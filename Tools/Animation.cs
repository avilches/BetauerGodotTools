using System;
using System.Diagnostics;
using Godot;
using Tools.Effects;
using Object = Godot.Object;

namespace Tools {
    public delegate void Callback();

    public interface ILoopStatus {
        public string Name { get; }
        public bool Playing { get; }
        public void Stop();
        public void PlayLoop();

        // Builder chained methods
        public ILoopStatus OnStart(Callback callback);
        public ILoopStatus OnEnd(Callback callback);
    }

    public interface IOnceStatus {
        public string Name { get; }
        public bool Playing { get; }
        public void PlayOnce(bool killPrevious = false);
        public void Stop(bool killPrevious = false);

        // Builder chained methods
        public bool CanBeInterrupted { get; set; }
        public bool KillPrevious { get; set; }

        public IOnceStatus OnStart(Callback callback);
        public IOnceStatus OnEnd(Callback callback);
    }

    public class AnimationStack : Object /* needed to listen signals */ {
        private class Status {
            public string Name { get; }
            public bool Playing { get; private set; }

            protected Logger _logger;
            private protected Callback _onStart;
            private protected Callback _onEnd;

            public Status(Logger logger, string name) {
                _logger = logger;
                Name = name;
            }

            internal virtual void ExecuteOnStart() {
                _logger.Debug("Start " + GetType().Name + " \"" + Name + "\"");
                Playing = true;
                _onStart?.Invoke();
            }

            internal virtual void ExecuteOnEnd() {
                _logger.Debug("End " + GetType().Name + " \"" + Name + "\"");
                _onEnd?.Invoke();
                Playing = false;
            }
        }

        private abstract class OnceStatus : Status, IOnceStatus {
            private readonly AnimationStack _animationStack;
            public bool CanBeInterrupted { get; set; }
            public bool KillPrevious { get; set; }

            protected OnceStatus(AnimationStack animationStack, Logger logger, string name, bool canBeInterrupted,
                bool killPrevious) : base(logger, name) {
                _animationStack = animationStack;
                CanBeInterrupted = canBeInterrupted;
                KillPrevious = killPrevious;
            }

            public IOnceStatus OnStart(Callback callback) {
                _onStart = callback;
                return this;
            }

            public IOnceStatus OnEnd(Callback callback) {
                _onEnd = callback;
                return this;
            }

            public void PlayOnce(bool killPrevious = false) {
                _animationStack.PlayOnce(this, killPrevious);
            }

            public void Stop(bool killPrevious = false) {
                if (_animationStack.IsPlayingOnce(Name)) {
                    _animationStack.StopPlayingOnce(killPrevious);
                }
            }
        }

        private abstract class LoopStatus : Status, ILoopStatus {
            private readonly AnimationStack _animationStack;

            protected LoopStatus(AnimationStack animationStack, Logger logger, string name) : base(logger, name) {
                _animationStack = animationStack;
            }

            public ILoopStatus OnStart(Callback callback) {
                _onStart = callback;
                return this;
            }

            public ILoopStatus OnEnd(Callback callback) {
                _onEnd = callback;
                return this;
            }

            public void PlayLoop() {
                _animationStack.PlayLoop(this);
            }

            public void Stop() {
                if (_animationStack.IsPlayingLoop(Name)) {
                    _animationStack.StopPlayingLoop();
                }
            }
        }

        private class LoopTween : LoopStatus {
            private readonly TweenPlayer _tweenPlayer;
            private readonly TweenSequence _sequence;

            public LoopTween(AnimationStack animationStack, Logger logger, string name, TweenPlayer tweenPlayer,
                TweenSequence sequence) : base(
                animationStack, logger, name) {
                _tweenPlayer = tweenPlayer;
                _sequence = sequence;
            }

            internal override void ExecuteOnStart() {
                base.ExecuteOnStart();
                _tweenPlayer.Clear().AddSequence(_sequence).SetInfiniteLoops().Start();
            }

            internal override void ExecuteOnEnd() {
                base.ExecuteOnEnd();
                _tweenPlayer.Stop();
            }
        }

        private class OnceTween : OnceStatus {
            private readonly TweenPlayer _tweenPlayer;
            private readonly TweenSequence _sequence;

            public OnceTween(AnimationStack animationStack, Logger logger, string name, bool canBeInterrupted,
                bool killPrevious, TweenPlayer tweenPlayer, TweenSequence sequence) : base(animationStack, logger,
                name, canBeInterrupted, killPrevious) {
                _tweenPlayer = tweenPlayer;
                _sequence = sequence;
            }

            internal override void ExecuteOnStart() {
                base.ExecuteOnStart();
                _tweenPlayer.Clear().AddSequence(_sequence).SetLoops(1).Start();
            }

            internal override void ExecuteOnEnd() {
                base.ExecuteOnEnd();
                _tweenPlayer.Stop();
            }
        }

        private class LoopAnimation : LoopStatus {
            private readonly AnimationPlayer _animationPlayer;

            public LoopAnimation(AnimationStack animationStack, Logger logger, string name,
                AnimationPlayer animationPlayer) : base(animationStack,
                logger, name) {
                _animationPlayer = animationPlayer;
            }

            internal override void ExecuteOnStart() {
                base.ExecuteOnStart();
                _animationPlayer.Play(Name);
            }

            internal override void ExecuteOnEnd() {
                base.ExecuteOnEnd();
                _animationPlayer.Stop();
            }
        }

        private class OnceAnimation : OnceStatus {
            private readonly AnimationPlayer _animationPlayer;

            public OnceAnimation(AnimationStack animationStack, Logger logger, string name, bool canBeInterrupted,
                bool killPrevious, AnimationPlayer animationPlayer) : base(animationStack, logger, name,
                canBeInterrupted, killPrevious) {
                _animationPlayer = animationPlayer;
            }

            internal override void ExecuteOnStart() {
                base.ExecuteOnStart();
                _animationPlayer.Play(Name);
            }

            internal override void ExecuteOnEnd() {
                base.ExecuteOnEnd();
                _animationPlayer.Stop();
            }
        }

        private readonly System.Collections.Generic.Dictionary<string, LoopStatus> _loopAnimations =
            new System.Collections.Generic.Dictionary<string, LoopStatus>();

        private readonly System.Collections.Generic.Dictionary<string, OnceStatus> _onceAnimations =
            new System.Collections.Generic.Dictionary<string, OnceStatus>();

        private readonly AnimationPlayer _animationPlayer;
        private readonly TweenPlayer _tweenPlayer;
        private LoopStatus _currentLoopAnimation;
        private OnceStatus _currentOnceAnimation;
        private readonly Logger _logger;

        public AnimationStack(string name, TweenPlayer tweenPlayer, AnimationPlayer animationPlayer = null) :
            this(name, animationPlayer, tweenPlayer) {
        }

        public AnimationStack(string name, AnimationPlayer animationPlayer, TweenPlayer tweenPlayer = null) {
            _logger = LoggerFactory.GetLogger(name, "AnimationStack");

            _animationPlayer = animationPlayer;
            _animationPlayer?.Connect(GodotConstants.GODOT_SIGNAL_animation_finished, this,
                nameof(OnceAnimationFinished));

            _tweenPlayer = tweenPlayer;
            _tweenPlayer?.AddOnFinishTween(OnceTweenFinished);
        }

        public ILoopStatus AddLoopAnimation(string name) {
            Debug.Assert(_animationPlayer != null, "_animationPlayer != null");
            Animation animation = _animationPlayer.GetAnimation(name);
            if (animation == null) {
                throw new Exception(
                    $"Animation {name} not found in AnimationPlayer {_animationPlayer.Name}");
            }
            animation.Loop = true;
            var loopAnimationStatus = new LoopAnimation(this, _logger, name, _animationPlayer);
            _loopAnimations.Add(name, loopAnimationStatus);
            return loopAnimationStatus;
        }

        public IOnceStatus AddOnceAnimation(string name, bool canBeInterrupted = false,
            bool killPrevious = false) {
            Debug.Assert(_animationPlayer != null, "_animationPlayer != null");
            Animation animation = _animationPlayer.GetAnimation(name);
            if (animation == null) {
                throw new Exception(
                    $"Animation {name} not found in AnimationPlayer {_animationPlayer.Name}");
            }
            animation.Loop = false;
            var onceAnimationStatus =
                new OnceAnimation(this, _logger, name, canBeInterrupted, killPrevious, _animationPlayer);
            _onceAnimations.Add(name, onceAnimationStatus);
            return onceAnimationStatus;
        }

        public ILoopStatus AddLoopTween(string name, TweenSequence tweenSequence) {
            Debug.Assert(_tweenPlayer != null, "_tweenPlayer != null");
            var loopTweenStatus = new LoopTween(this, _logger, name, _tweenPlayer, tweenSequence);
            _loopAnimations.Add(name, loopTweenStatus);
            return loopTweenStatus;
        }

        public IOnceStatus AddOnceTween(string name, TweenSequence tweenSequence, bool canBeInterrupted = false,
            bool killPrevious = false) {
            Debug.Assert(_tweenPlayer != null, "_tweenPlayer != null");
            var onceTweenStatus =
                new OnceTween(this, _logger, name, canBeInterrupted, killPrevious, _tweenPlayer, tweenSequence);
            _onceAnimations.Add(name, onceTweenStatus);
            return onceTweenStatus;
        }

        public ILoopStatus GetLoop(string name) => _loopAnimations[name];
        public IOnceStatus GetOnce(string name) => _onceAnimations[name];
        public ILoopStatus GetPlayingLoop() => _currentLoopAnimation;
        public IOnceStatus GetPlayingOnce() => _currentOnceAnimation;
        public bool IsPlayingLoop(string name) => _currentOnceAnimation == null && _currentLoopAnimation?.Name == name;
        public bool IsPlayingLoop() => _currentOnceAnimation == null && _currentLoopAnimation != null;
        public bool IsPlayingOnce(string name) => _currentOnceAnimation?.Name == name;
        public bool IsPlayingOnce() => _currentOnceAnimation != null;

        public bool IsPlaying(string name) =>
            _currentLoopAnimation?.Name == name || _currentLoopAnimation?.Name == name;

        public void PlayLoop(string name) => PlayLoop(_loopAnimations[name]);

        public void PlayOnce(string name, bool killPrevious = false) =>
            PlayOnce(_onceAnimations[name], killPrevious);

        /*
         * currentLoop currentOnce newLoop
         * null        null        null       currentLoop = null (nothing to do)
         * p(a)        null        null       stop(a), currentLoop = null
         * null        p(x)        null       currentLoop = null (nothing to do)
         * a           p(x)        null       currentLoop = null
         */
        public void StopPlayingLoop() {
            // Stop and remove the currentLoopAnimation
            if (_currentOnceAnimation == null) {
                _currentLoopAnimation.ExecuteOnEnd();
            }
            _currentLoopAnimation = null;
        }

        /*
        * null        null        a          currentLoop = p(a)
        * p(a)        null        a          - nothing to do -
        * null        p(x)        a          currentLoop = a
        * a           p(x)        a          currentLoop = a (nothing to do)
        *
        * null        null        b          currentLoop = p(b)
        * p(a)        null        b          stop(a), currentLoop = p(b)
        * null        p(x)        b          currentLoop = b
        * a           p(x)        b          currentLoop = b
        */
        private void PlayLoop(LoopStatus loopAnimation) {
            if (_currentLoopAnimation == null || _currentLoopAnimation.Name != loopAnimation.Name) {
                _logger.Debug($"PlayLoop \"{loopAnimation.Name}\"");
                if (_currentOnceAnimation == null) {
                    _currentLoopAnimation?.ExecuteOnEnd();
                    _currentLoopAnimation = loopAnimation;
                    _currentLoopAnimation.ExecuteOnStart();
                } else {
                    _currentLoopAnimation = loopAnimation;
                }
            }
        }

        /*
        * currentLoop currentOnce newOnce
        * null        null        null       * currentOnce = null (nothing to do)
        * p(a)        null        null       * currentOnce = null (nothing to do)
        * null        p(x)        null       stop(x), currentOnce = null
        * a           p(x)        null       stop(x), play(a), currentOnce = null
        */
        public void StopPlayingOnce(bool killPrevious = false) {
            if (_currentOnceAnimation == null) return;
            if (_currentOnceAnimation.CanBeInterrupted || killPrevious) {
                _currentOnceAnimation.ExecuteOnEnd();
                _currentOnceAnimation = null;
                _currentLoopAnimation?.ExecuteOnStart();
            }
        }

        /*
        * null        null        a          currentOnce = p(a)
        * p(x)        null        a          stop(x), currentOnce = p(a)
        * null        p(a)        a          * currentOnce = p(a) (nothing to do)
        * x           p(a)        a          * currentOnce = p(a) (nothing to do)
        *
        * null        null        b          currentOnce = p(b)
        * p(x)        null        b          stop(x), currentOnce = p(b)
        * null        p(a)        b          stop(a), currentOnce = p(b)
        * x           p(a)        b          stop(a), currentOnce = p(b)
        */
        private void PlayOnce(OnceStatus newOnceAnimation,
            bool killPrevious) {
            if (_currentOnceAnimation == null || _currentOnceAnimation.CanBeInterrupted ||
                newOnceAnimation.KillPrevious || killPrevious) {
                if (_currentOnceAnimation != null) {
                    _logger.Debug(
                        $"PlayOnce \"{newOnceAnimation.Name}\" (Interrupting: {_currentOnceAnimation.Name}\")");
                    _currentOnceAnimation.ExecuteOnEnd();
                } else {
                    _logger.Debug($"PlayOnce \"{newOnceAnimation.Name}\"");
                }
                _currentLoopAnimation?.ExecuteOnEnd();
                _currentOnceAnimation = newOnceAnimation;
                _currentOnceAnimation.ExecuteOnStart();
            } else if (!_currentOnceAnimation.CanBeInterrupted) {
                _logger.Warning(
                    $"PlayOnce \"{newOnceAnimation.Name}\" not played: current animation \"{_currentOnceAnimation.Name}\" can't be interrupted)");
            }
        }

        private void OnceTweenFinished(TweenSequence tweenSequence) {
            if (_currentOnceAnimation == null) {
                // This could happen when a LoopTween is not infinite (lit a Tween that just rest the values)
                // so ignore the event
                return;
            }
            _logger.Debug("OnceTweenFinished: \"" + _currentOnceAnimation.Name + "\"");
            _currentOnceAnimation.ExecuteOnEnd();
            _currentOnceAnimation = null;
            _currentLoopAnimation?.ExecuteOnStart();
        }

        private void OnceAnimationFinished(string animation) {
            if (_currentOnceAnimation == null) {
                // This could happen when a LoopAnimation is not infinite (configured as loop in the animator)
                // This event is very unusual, but just in case, ignore it
                return;
            }
            _logger.Debug("OnceAnimationFinished: \"" + _currentOnceAnimation.Name + "\"");
            _currentOnceAnimation.ExecuteOnEnd();
            _currentOnceAnimation = null;
            _currentLoopAnimation?.ExecuteOnStart();
        }
    }
}