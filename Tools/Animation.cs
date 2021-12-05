using System;
using Godot;
using Object = Godot.Object;

namespace Tools {
    public delegate void Callback();

    public class AnimationStatus : Object {
        protected AnimationStack _animationStack;
        protected Logger _logger;
        public readonly string Name;

        public bool Playing { get; private set; } = false;

        private protected Callback _onStart;
        private protected Callback _onEnd;

        internal AnimationStatus(AnimationStack animationStack, Logger logger, string name) {
            _animationStack = animationStack;
            _logger = logger;
            Name = name;
        }

        internal void ExecuteOnStart() {
            _logger.Debug("Start " + GetType().Name + " \"" + Name + "\"");
            Playing = true;
            _onStart?.Invoke();
        }

        internal void ExecuteOnEnd() {
            _logger.Debug("End " + GetType().Name + " \"" + Name + "\"");
            _onEnd?.Invoke();
            Playing = false;
        }
    }

    public class LoopAnimation : AnimationStatus {
        internal LoopAnimation(AnimationStack animationStack, Logger logger, string name) :
            base(animationStack, logger, name) {
        }

        public void Stop() {
            if (_animationStack.IsPlayingLoop(Name)) {
                _animationStack.StopLoopAnimation();
            }
        }

        public void PlayLoop() {
            _animationStack.PlayLoopAnimation(this);
        }

        public LoopAnimation OnStart(Callback callback) {
            _onStart = callback;
            return this;
        }

        public LoopAnimation OnEnd(Callback callback) {
            _onEnd = callback;
            return this;
        }
    }

    public class OnceAnimation : AnimationStatus {
        public bool CanBeInterrupted;
        public bool KillPreviousAnimation;

        protected internal OnceAnimation(AnimationStack animationStack, Logger logger, string name,
            bool canBeInterrupted, bool killPreviousAnimation) : base(animationStack, logger, name) {
            CanBeInterrupted = canBeInterrupted;
            KillPreviousAnimation = killPreviousAnimation;
        }

        public void PlayOnce(bool killPreviousAnimation = false) {
            _animationStack.PlayOnceAnimation(this, killPreviousAnimation);
        }

        public void Stop(bool killPreviousAnimation = false) {
            if (_animationStack.IsPlayingOnce(Name)) {
                _animationStack.StopOnceAnimation(killPreviousAnimation);
            }
        }

        public OnceAnimation OnStart(Callback callback) {
            _onStart = callback;
            return this;
        }

        public OnceAnimation OnEnd(Callback callback) {
            _onEnd = callback;
            return this;
        }
    }

    public class AnimationStack : Object /* needed to listen signals */ {
        private readonly System.Collections.Generic.Dictionary<string, LoopAnimation> _loopAnimations =
            new System.Collections.Generic.Dictionary<string, LoopAnimation>();

        private readonly System.Collections.Generic.Dictionary<string, OnceAnimation> _onceAnimations =
            new System.Collections.Generic.Dictionary<string, OnceAnimation>();

        private readonly AnimationPlayer _animationPlayer;
        private LoopAnimation _currentLoopAnimation;
        private OnceAnimation _currentOnceAnimation;
        private readonly Logger _logger;

        public AnimationStack(string name, AnimationPlayer animationPlayer) {
            _animationPlayer = animationPlayer;
            _animationPlayer.Connect(GodotConstants.GODOT_SIGNAL_animation_finished, this,
                nameof(OnceAnimationFinished));
            _logger = LoggerFactory.GetLogger(name, "AnimationStack");
        }

        public LoopAnimation AddLoopAnimation(string name) {
            Animation animation = _animationPlayer.GetAnimation(name);
            if (animation == null) {
                throw new Exception(
                    $"Animation {name} not found in AnimationPlayer {_animationPlayer.Name}");
            }
            animation.Loop = true;
            var loopAnimationStatus = new LoopAnimation(this, _logger, name);
            _loopAnimations.Add(name, loopAnimationStatus);
            return loopAnimationStatus;
        }

        public OnceAnimation AddOnceAnimation(string name, bool canBeInterrupted = false,
            bool killPreviousAnimation = false) {
            Animation animation = _animationPlayer.GetAnimation(name);
            if (animation == null) {
                throw new Exception(
                    $"Animation {name} not found in AnimationPlayer {_animationPlayer.Name}");
            }
            animation.Loop = false;
            var onceAnimationStatus =
                new OnceAnimation(this, _logger, name, canBeInterrupted, killPreviousAnimation);
            _onceAnimations.Add(name, onceAnimationStatus);
            return onceAnimationStatus;
        }

        public LoopAnimation GetLoopAnimation(string name) => _loopAnimations[name];
        public OnceAnimation GetOnceAnimation(string name) => _onceAnimations[name];
        public LoopAnimation GetPlayingLoop() => _currentLoopAnimation;
        public OnceAnimation GetPlayingOnce() => _currentOnceAnimation;
        public bool IsPlayingLoop(string name) => _currentOnceAnimation == null && _currentLoopAnimation?.Name == name;
        public bool IsPlayingLoop() => _currentOnceAnimation == null && _currentLoopAnimation != null;
        public bool IsPlayingOnce(string name) => _currentOnceAnimation?.Name == name;
        public bool IsPlayingOnce() => _currentOnceAnimation != null;

        public bool IsPlaying(string name) =>
            _currentLoopAnimation?.Name == name || _currentLoopAnimation?.Name == name;

        public void PlayLoopAnimation(string name) => PlayLoopAnimation(_loopAnimations[name]);

        public void PlayOnceAnimation(string name, bool killPreviousAnimation = false) =>
            PlayOnceAnimation(_onceAnimations[name], killPreviousAnimation);

        /*
         * currentLoop currentOnce newLoop
         * null        null        null       currentLoop = null (nothing to do)
         * p(a)        null        null       stop(a), currentLoop = null
         * null        p(x)        null       currentLoop = null (nothing to do)
         * a           p(x)        null       currentLoop = null
         */
        public void StopLoopAnimation() {
            // Stop and remove the currentLoopAnimation
            if (_currentOnceAnimation == null) {
                _animationPlayer.Stop();
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
        protected internal void PlayLoopAnimation(LoopAnimation loopAnimation) {
            if (_currentLoopAnimation == null || _currentLoopAnimation.Name != loopAnimation.Name) {
                _logger.Debug("PlayLoop \"" + loopAnimation.Name + "\"");
                if (_currentOnceAnimation == null) {
                    _currentLoopAnimation?.ExecuteOnEnd();
                    _currentLoopAnimation = loopAnimation;
                    _animationPlayer.Play(_currentLoopAnimation.Name);
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
        public void StopOnceAnimation(bool killPreviousAnimation = false) {
            if (_currentOnceAnimation == null) return;
            if (_currentOnceAnimation.CanBeInterrupted || killPreviousAnimation) {
                _currentOnceAnimation.ExecuteOnEnd();
                _currentOnceAnimation = null;
                if (_currentLoopAnimation == null) {
                    _animationPlayer.Stop();
                } else {
                    _animationPlayer.Play(_currentLoopAnimation.Name);
                    _currentLoopAnimation.ExecuteOnStart();
                }
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
        protected internal void PlayOnceAnimation(OnceAnimation newOnceAnimation,
            bool killPreviousAnimation) {
            if (_currentOnceAnimation == null || _currentOnceAnimation.CanBeInterrupted ||
                newOnceAnimation.KillPreviousAnimation || killPreviousAnimation) {
                if (_currentOnceAnimation != null) {
                    _logger.Debug("PlayOnce \"" + newOnceAnimation.Name + "\" (Interrupting: " +
                                  _currentOnceAnimation.Name + "\")");
                    _currentOnceAnimation.ExecuteOnEnd();
                    if (_currentOnceAnimation == newOnceAnimation) {
                        // The animation is interrupting it self!
                        _animationPlayer.Stop(); // This is needed to force play animation from begining
                    }
                } else {
                    _logger.Debug("PlayOnce \"" + newOnceAnimation.Name + "\"");
                }
                _currentLoopAnimation?.ExecuteOnEnd();
                _currentOnceAnimation = newOnceAnimation;
                _animationPlayer.Play(_currentOnceAnimation.Name);
                _currentOnceAnimation.ExecuteOnStart();
            } else if (!_currentOnceAnimation.CanBeInterrupted) {
                _logger.Warning("PlayOnce \"" + newOnceAnimation.Name +
                                "\" not played: current animation \"" +
                                _currentOnceAnimation.Name + "\" can't be interrupted)");
            }
        }

        public void OnceAnimationFinished(string animation) {
            _logger.Debug("OnceAnimationFinished: \"" + _currentOnceAnimation.Name + "\"");
            _currentOnceAnimation.ExecuteOnEnd();
            _currentOnceAnimation = null;
            if (_currentLoopAnimation != null) {
                _animationPlayer.Play(_currentLoopAnimation.Name);
                _currentLoopAnimation.ExecuteOnStart();
            }
        }
    }
}