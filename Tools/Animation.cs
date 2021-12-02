using System;
using Godot;
using Object = Godot.Object;

namespace Tools {

    public delegate void Callback();
    public class AnimationStatus : Object {
        protected AnimationStack _animationStack;

        public AnimationStatus(AnimationStack animationStack) {
            _animationStack = animationStack;
        }
    }

    public class LoopAnimation : AnimationStatus {
        public readonly string Name;

        protected internal LoopAnimation(AnimationStack animationStack, string name) : base(animationStack) {
            Name = name;
        }

        public void PlayLoop() {
            _animationStack.PlayLoopAnimation(this);
        }
    }

    public class OnceAnimation : AnimationStatus {
        public readonly string Name;

        public bool Playing { get; set; } = false;
        public bool Interrupted { get; set; } = false;

        private Callback _onStart;
        private Callback _onEnd;

        public bool CanBeInterrupted;
        public bool KillPreviousAnimation;

        protected internal OnceAnimation(AnimationStack animationStack, string name,
            bool canBeInterrupted, bool killPreviousAnimation) : base(animationStack) {
            Name = name;
            CanBeInterrupted = canBeInterrupted;
            KillPreviousAnimation = killPreviousAnimation;
        }

        public void PlayOnce(bool killPreviousAnimation = false) {
            _animationStack.PlayOnceAnimation(this, killPreviousAnimation);
        }

        public OnceAnimation OnStart(Callback callback) {
            _onStart = callback;
            return this;
        }

        public OnceAnimation OnEnd(Callback callback) {
            _onEnd = callback;
            return this;
        }

        internal void Start() {
            _onStart?.Invoke();
        }

        internal void End() {
            _onEnd?.Invoke();
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
            _logger = LoggerFactory.GetLogger(name, "Animation");
        }

        public LoopAnimation AddLoopAnimation(string name) {
            Animation animation = _animationPlayer.GetAnimation(name);
            if (animation == null) {
                throw new Exception(
                    $"Animation {name} not found in AnimationPlayer {_animationPlayer.Name}");
            }
            animation.Loop = true;
            var loopAnimationStatus = new LoopAnimation(this, name);
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
                new OnceAnimation(this, name, canBeInterrupted, killPreviousAnimation);
            _onceAnimations.Add(name, onceAnimationStatus);
            return onceAnimationStatus;
        }

        public OnceAnimation GetOnceAnimation(string name) {
            OnceAnimation onceAnimation = _onceAnimations[name];
            if (onceAnimation == null) {
                throw new Exception($"Animation {name} not found in Animator");
            }
            return onceAnimation;
        }

        public void PlayLoopAnimation(string name) {
            LoopAnimation loopAnimation = _loopAnimations[name];
            if (loopAnimation == null) {
                throw new Exception($"Animation {name} not found in Animator");
            }
            PlayLoopAnimation(loopAnimation);
        }

        protected internal void PlayLoopAnimation(LoopAnimation loopAnimation) {
            if (_currentLoopAnimation == null || _currentLoopAnimation.Name != loopAnimation.Name) {
                _logger.Debug("PlayLoop \"" + loopAnimation.Name + "\"");
                _currentLoopAnimation = loopAnimation;
                if (_currentOnceAnimation == null) {
                    _animationPlayer.Play(loopAnimation.Name);
                }
            }
        }

        public OnceAnimation PlayOnceAnimation(string name, bool killPreviousAnimation = false) {
            OnceAnimation newOnceAnimation = GetOnceAnimation(name);
            return PlayOnceAnimation(newOnceAnimation, killPreviousAnimation);
        }

        protected internal OnceAnimation PlayOnceAnimation(OnceAnimation newOnceAnimation,
            bool killPreviousAnimation) {
            if (_currentOnceAnimation == null || _currentOnceAnimation.CanBeInterrupted ||
                newOnceAnimation.KillPreviousAnimation || killPreviousAnimation) {
                if (_currentOnceAnimation != null) {
                    _logger.Debug("PlayOnce \"" + newOnceAnimation.Name + "\" (Interrupting: " +
                                  _currentOnceAnimation.Name + "\")");
                    _currentOnceAnimation.Playing = false;
                    _currentOnceAnimation.Interrupted = true;
                    _currentOnceAnimation.End();
                    if (_currentOnceAnimation == newOnceAnimation) {
                        _animationPlayer.Stop();
                    }
                } else {
                    _logger.Debug("PlayOnce \"" + newOnceAnimation.Name + "\"");
                }
                _currentOnceAnimation = newOnceAnimation;
                newOnceAnimation.Playing = true;
                newOnceAnimation.Interrupted = false;
                _currentOnceAnimation.Start();
                _animationPlayer.Play(newOnceAnimation.Name);
            } else if (!_currentOnceAnimation.CanBeInterrupted) {
                _logger.Error("PlayOnce \"" + newOnceAnimation.Name +
                              "\" not played: current animation \"" +
                              _currentOnceAnimation.Name + "\" can't be interrupted)");
            }
            return newOnceAnimation;
        }

        public bool IsPlayingLoopAnimation(string animationType) {
            if (_currentLoopAnimation == null) return false;
            return _currentLoopAnimation.Name == animationType;
        }

        public bool IsPlayingOnceAnimation(string animationType) {
            if (_currentOnceAnimation == null) return false;
            return _currentOnceAnimation.Name == animationType;
        }

        public bool IsPlaying(string name) {
            return IsPlayingLoopAnimation(name) || IsPlayingOnceAnimation(name);
        }

        public bool IsLooping() {
            return _currentOnceAnimation == null;
        }

        public void OnceAnimationFinished(string animation) {
            _logger.Debug("OnceAnimationFinished: \"" + _currentOnceAnimation.Name + "\"");
            _currentOnceAnimation.End();
            _currentOnceAnimation.Playing = false;
            _currentOnceAnimation = null;
            if (_currentLoopAnimation != null) {
                _animationPlayer.Play(_currentLoopAnimation.Name);
            }
        }
    }
}