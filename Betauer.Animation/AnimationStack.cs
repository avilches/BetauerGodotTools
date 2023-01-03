using System;
using System.Collections.Generic;
using System.Diagnostics;
using Betauer.Tools.Logging;
using Betauer.Core.Signal;
using Godot;

namespace Betauer.Animation {
    public interface ILoopStatus {
        public string Name { get; }
        public bool Playing { get; }
        public void Stop();
        public void PlayLoop();

        // Builder chained methods
        public ILoopStatus OnStart(Action callback);
        public ILoopStatus OnEnd(Action callback);
    }

    public interface IOnceStatus {
        public string Name { get; }
        public bool Playing { get; }
        public void PlayOnce(bool killPrevious = false);
        public void Stop(bool killPrevious = false);

        // Builder chained methods
        public bool CanBeInterrupted { get; set; }
        public bool KillPrevious { get; set; }

        public IOnceStatus OnStart(Action callback);
        public IOnceStatus OnEnd(Action callback);
    }

    public class AnimationStack {
        private class Status {
            public string Name { get; }
            public bool Playing { get; private set; }

            private readonly Logger _logger;
            protected Action? _onStart;
            protected Action? _onEnd;

            protected Status(Logger logger, string name) {
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
            protected readonly AnimationStack AnimationStack;
            public bool CanBeInterrupted { get; set; }
            public bool KillPrevious { get; set; }

            protected OnceStatus(AnimationStack animationStack, Logger logger, string name, bool canBeInterrupted,
                bool killPrevious) : base(logger, name) {
                AnimationStack = animationStack;
                CanBeInterrupted = canBeInterrupted;
                KillPrevious = killPrevious;
            }

            public IOnceStatus OnStart(Action callback) {
                _onStart = callback;
                return this;
            }

            public IOnceStatus OnEnd(Action callback) {
                _onEnd = callback;
                return this;
            }

            public void PlayOnce(bool killPrevious = false) {
                AnimationStack.PlayOnce(this, killPrevious);
            }

            public void Stop(bool killPrevious = false) {
                if (AnimationStack.IsPlayingOnce(Name)) {
                    AnimationStack.StopPlayingOnce(killPrevious);
                }
            }
        }

        private abstract class LoopStatus : Status, ILoopStatus {
            private readonly AnimationStack _animationStack;

            protected LoopStatus(AnimationStack animationStack, Logger logger, string name) : base(logger, name) {
                _animationStack = animationStack;
            }

            public ILoopStatus OnStart(Action callback) {
                _onStart = callback;
                return this;
            }

            public ILoopStatus OnEnd(Action callback) {
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

        private interface IKillable {
            void Kill();
        }

        private class LoopTween : LoopStatus, IKillable {
            private readonly IAnimation _animation;
            private Tween _sceneTreeTween;

            internal LoopTween(AnimationStack animationStack, Logger logger, string name, IAnimation animation) : base(
                animationStack, logger, name) {
                _animation = animation;
            }

            internal override void ExecuteOnStart() {
                base.ExecuteOnStart();
                _sceneTreeTween = _animation.Play().SetLoops();
            }

            public void Kill() {
                _sceneTreeTween?.Kill();
                _sceneTreeTween = null;
            }

            internal override void ExecuteOnEnd() {
                base.ExecuteOnEnd();
                Kill();
            }
        }

        private class OnceTween : OnceStatus, IKillable {
            private readonly IAnimation _animation;
            private Tween _sceneTreeTween;

            internal OnceTween(AnimationStack animationStack, Logger logger, string name, bool canBeInterrupted,
                bool killPrevious, IAnimation animation) :
                base(animationStack, logger, name, canBeInterrupted, killPrevious) {
                _animation = animation;
            }

            internal override void ExecuteOnStart() {
                base.ExecuteOnStart();
                _sceneTreeTween = _animation.Play().SetLoops(1);
                _sceneTreeTween.OnFinished(AnimationStack.OnTweenPlayerFinishAll);
            }

            public void Kill() {
                _sceneTreeTween?.Kill();
                _sceneTreeTween = null;
            }


            internal override void ExecuteOnEnd() {
                base.ExecuteOnEnd();
                Kill();
            }
        }

        private class LoopAnimation : LoopStatus {
            private readonly AnimationPlayer _animationPlayer;

            internal LoopAnimation(AnimationStack animationStack, Logger logger, string name,
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

            internal OnceAnimation(AnimationStack animationStack, Logger logger, string name, bool canBeInterrupted,
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

        public AnimationPlayer? AnimationPlayer { get; private set; }

        private readonly Dictionary<string, LoopStatus> _loopAnimations = new();
        private readonly Dictionary<string, OnceStatus> _onceAnimations = new();

        private LoopStatus _currentLoopAnimation;
        private OnceStatus _currentOnceAnimation;
        private static readonly Logger StaticLogger = LoggerFactory.GetLogger<AnimationStack>();
        private readonly Logger _logger;

        public AnimationStack(string? name = null) {
            _logger = name == null ? StaticLogger : LoggerFactory.GetLogger(name);
        }

        public AnimationStack SetAnimationPlayer(AnimationPlayer animationPlayer) {
            AnimationPlayer = animationPlayer;
            AnimationPlayer.OnAnimationFinished(AnimationFinished);
            return this;
        }

        public ILoopStatus AddLoopAnimation(string name, bool pingPong = false) {
            Debug.Assert(AnimationPlayer != null, "_animationPlayer != null");
            Godot.Animation animation = AnimationPlayer.GetAnimation(name);
            if (animation == null) {
                throw new Exception($"Animation {name} not found in AnimationPlayer {AnimationPlayer.Name}");
            }
            // animation.LoopMode = pingPong ? Godot.Animation.LoopModeEnum.Pingpong : Godot.Animation.LoopModeEnum.Linear;
            animation.LoopMode = Godot.Animation.LoopModeEnum.None;
            var loopAnimationStatus = new LoopAnimation(this, _logger, name, AnimationPlayer);
            _loopAnimations.Add(name, loopAnimationStatus);
            return loopAnimationStatus;
        }

        public IOnceStatus AddOnceAnimation(string name, bool canBeInterrupted = false, bool killPrevious = false) {
            Debug.Assert(AnimationPlayer != null, "_animationPlayer != null");
            Godot.Animation animation = AnimationPlayer.GetAnimation(name);
            if (animation == null) {
                throw new Exception($"Animation {name} not found in AnimationPlayer {AnimationPlayer.Name}");
            }
            animation.LoopMode = Godot.Animation.LoopModeEnum.None;
            var onceAnimationStatus =
                new OnceAnimation(this, _logger, name, canBeInterrupted, killPrevious, AnimationPlayer);
            _onceAnimations.Add(name, onceAnimationStatus);
            return onceAnimationStatus;
        }

        public ILoopStatus AddLoopTween(string name, IAnimation animation) {
            var loopTweenStatus = new LoopTween(this, _logger, name, animation);
            _loopAnimations.Add(name, loopTweenStatus);
            return loopTweenStatus;
        }

        public IOnceStatus AddOnceTween(string name, IAnimation animation, bool canBeInterrupted = false,
            bool killPrevious = false) {
            var onceTweenStatus = new OnceTween(this, _logger, name, canBeInterrupted, killPrevious, animation);
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
        public bool IsPlaying(string name) => _currentLoopAnimation?.Name == name || _currentLoopAnimation?.Name == name;
        public void PlayLoop(string name) => PlayLoop(_loopAnimations[name]);
        public void PlayOnce(string name, bool killPrevious = false) => PlayOnce(_onceAnimations[name], killPrevious);

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
        private void PlayOnce(OnceStatus newOnceAnimation, bool killPrevious) {
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

        private void OnTweenPlayerFinishAll() {
            if (_currentOnceAnimation == null) {
                // This could happen when a LoopTween is not infinite (lit a Tween that just rest the values)
                // so ignore the event
                return;
            }
            _logger.Debug("OnTweenPlayerFinishAll: \"" + _currentOnceAnimation.Name + "\"");
            _currentOnceAnimation.ExecuteOnEnd();
            _currentOnceAnimation = null;
            _currentLoopAnimation?.ExecuteOnStart();
        }

        private void AnimationFinished(StringName animation) {
            if (_currentLoopAnimation?.Name == animation) {
                // TODO: this allows to create finite loops 
                AnimationPlayer!.Play(animation);
            } else if (_currentOnceAnimation?.Name == animation) {
                _logger.Debug("OnceAnimationFinished: \"" + _currentOnceAnimation.Name + "\"");
                _currentOnceAnimation.ExecuteOnEnd();
                _currentOnceAnimation = null;
                _currentLoopAnimation?.ExecuteOnStart();
            } else {
                // This could happen when a LoopAnimation is not infinite (configured as loop in the animator)
                // This event is very unusual, but just in case, ignore it
                GD.Print("What??");
            }
        }
    }
}