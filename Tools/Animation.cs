using System;
using Godot;
using Object = Godot.Object;

namespace Tools {
    public abstract class Animation {
        public abstract string Name { get; }
        public Godot.Animation GodotAnimation { get; set; }
    }

    public abstract class LoopAnimation : Animation {
    }

    public abstract class OnceAnimation : Animation {
        public virtual bool CanBeInterrupted { get; }
        public virtual bool KillPreviousAnimation { get; }

        public virtual void OnStart() {
        }

        public virtual void OnEnd() {
        }
    }

    public class AnimationStatus : Object {
        protected AnimationStack _animationStack;

        public AnimationStatus(AnimationStack animationStack) {
            _animationStack = animationStack;
        }
    }

    public class LoopAnimationStatus : AnimationStatus {
        public LoopAnimation Animation { get; }

        protected internal LoopAnimationStatus(AnimationStack animationStack, LoopAnimation loopAnimation) : base(
            animationStack) {
            Animation = loopAnimation;
        }

        public void PlayLoop() {
            _animationStack.PlayLoop(this);
        }
    }

    public class OnceAnimationStatus : AnimationStatus {
        public OnceAnimation Animation { get; set; }

        public bool Playing { get; set; } = false;
        public bool Interrupted { get; set; } = false;
        public bool CanBeInterrupted => Animation.CanBeInterrupted;
        public bool KillPreviousAnimation => Animation.KillPreviousAnimation;

        protected internal OnceAnimationStatus(AnimationStack animationStack, OnceAnimation animation) : base(
            animationStack) {
            Animation = animation;
        }

        public void PlayOnce(bool killPreviousAnimation = false) {
            _animationStack.PlayOnce(this, killPreviousAnimation);
        }
    }


    public class AnimationStack : Object /* needed to listen signals */ {
        private readonly System.Collections.Generic.Dictionary<Type, LoopAnimationStatus> _loopAnimations =
            new System.Collections.Generic.Dictionary<Type, LoopAnimationStatus>();

        private readonly System.Collections.Generic.Dictionary<Type, OnceAnimationStatus> _onceAnimationsStatuses =
            new System.Collections.Generic.Dictionary<Type, OnceAnimationStatus>();

        private readonly AnimationPlayer _animationPlayer;
        private LoopAnimation _currentLoopAnimation;
        private OnceAnimationStatus _currentOnceAnimationStatus;
        private readonly Logger _logger;

        public AnimationStack(string name, AnimationPlayer animationPlayer) {
            _animationPlayer = animationPlayer;
            _animationPlayer.Connect(GodotConstants.GODOT_SIGNAL_animation_finished, this,
                nameof(OnceAnimationFinished));
            _logger = LoggerFactory.GetLogger(name, "Animation");
        }

        public AnimationStack AddLoopAnimation(LoopAnimation newAnimation) {
            AddLoopAnimationAndGetStatus(newAnimation);
            return this;
        }

        public LoopAnimationStatus AddLoopAnimationAndGetStatus(LoopAnimation newAnimation) {
            Godot.Animation godotAnimation = _animationPlayer.GetAnimation(newAnimation.Name);
            if (godotAnimation == null) {
                throw new Exception(
                    $"Animation {newAnimation.Name} not found in AnimationPlayer {_animationPlayer.Name}");
            }
            godotAnimation.Loop = true;
            newAnimation.GodotAnimation = godotAnimation;

            var loopAnimationStatus = new LoopAnimationStatus(this, newAnimation);
            _loopAnimations.Add(newAnimation.GetType(), loopAnimationStatus);
            // _animationsByName[newAnimation.Name] = newAnimation;
            return loopAnimationStatus;
        }

        public AnimationStack AddOnceAnimation(OnceAnimation newAnimation) {
            AddOnceAnimationAndGetStatus(newAnimation);
            return this;
        }

        public OnceAnimationStatus AddOnceAnimationAndGetStatus(OnceAnimation newAnimation) {
            Godot.Animation godotAnimation = _animationPlayer.GetAnimation(newAnimation.Name);
            if (godotAnimation == null) {
                throw new Exception(
                    $"Animation {newAnimation.Name} not found in AnimationPlayer {_animationPlayer.Name}");
            }
            godotAnimation.Loop = false;
            newAnimation.GodotAnimation = godotAnimation;

            var onceAnimationStatus = new OnceAnimationStatus(this, newAnimation);
            _onceAnimationsStatuses.Add(newAnimation.GetType(), onceAnimationStatus);
            // _animationsByName[newAnimation.Name] = newAnimation;
            return onceAnimationStatus;
        }

        public OnceAnimationStatus GetOnceAnimationStatus(Type animationType) {
            OnceAnimationStatus onceAnimationStatus = _onceAnimationsStatuses[animationType];
            if (onceAnimationStatus == null) {
                throw new Exception($"Animation {animationType.Name} not found in Animator");
            }
            return onceAnimationStatus;
        }


        public void PlayLoop(Type newAnimationType) {
            LoopAnimationStatus loopAnimation = _loopAnimations[newAnimationType];
            if (loopAnimation == null) {
                throw new Exception($"Animation {newAnimationType.Name} not found in Animator");
            }
            PlayLoop(loopAnimation);
        }

        protected internal void PlayLoop(LoopAnimationStatus loopAnimation) {
            if (_currentLoopAnimation != loopAnimation.Animation) {
                _logger.Debug("PlayLoop \"" + loopAnimation.Animation.Name + "\"");
                _currentLoopAnimation = loopAnimation.Animation;
                if (_currentOnceAnimationStatus == null) {
                    _animationPlayer.Play(loopAnimation.Animation.Name);
                }
            }
        }

        public OnceAnimationStatus PlayOnce(Type newAnimationType, bool killPreviousAnimation = false) {
            OnceAnimationStatus newOnceAnimationStatus = GetOnceAnimationStatus(newAnimationType);
            return PlayOnce(newOnceAnimationStatus, killPreviousAnimation);
        }

        protected internal OnceAnimationStatus PlayOnce(OnceAnimationStatus newOnceAnimationStatus, bool killPreviousAnimation = false) {
            if (_currentOnceAnimationStatus == null || _currentOnceAnimationStatus.CanBeInterrupted || newOnceAnimationStatus.KillPreviousAnimation || killPreviousAnimation) {
                if (_currentOnceAnimationStatus != null) {
                    _logger.Debug("PlayOnce \"" + newOnceAnimationStatus.Animation.Name + "\" (Interrupting: " +
                                  _currentOnceAnimationStatus.Animation.Name + "\")");
                    _currentOnceAnimationStatus.Playing = false;
                    _currentOnceAnimationStatus.Interrupted = true;
                    _currentOnceAnimationStatus.Animation.OnEnd();
                    if (_currentOnceAnimationStatus == newOnceAnimationStatus) {
                        _animationPlayer.Stop();
                    }
                } else {
                    _logger.Debug("PlayOnce \"" + newOnceAnimationStatus.Animation.Name + "\"");
                }
                _currentOnceAnimationStatus = newOnceAnimationStatus;
                newOnceAnimationStatus.Playing = true;
                newOnceAnimationStatus.Interrupted = false;
                newOnceAnimationStatus.Animation.OnStart();
                _animationPlayer.Play(newOnceAnimationStatus.Animation.Name);
            } else if (!_currentOnceAnimationStatus.CanBeInterrupted) {
                _logger.Error("PlayOnce \"" + newOnceAnimationStatus.Animation.Name + "\" not played: current animation \"" +
                              _currentOnceAnimationStatus.Animation.Name + "\" can't be interrupted)");
            }
            return newOnceAnimationStatus;
        }

        public bool IsPlayingLoopAnimation(Type animationType) {
            if (_currentLoopAnimation == null) return false;
            return _currentLoopAnimation.GetType() == animationType;
        }

        public bool IsPlayingOnceAnimation(Type animationType) {
            if (_currentOnceAnimationStatus == null) return false;
            return _currentOnceAnimationStatus.Animation.GetType() == animationType;
        }

        public bool IsPlaying(Type animationType) {
            return IsPlayingLoopAnimation(animationType) || IsPlayingOnceAnimation(animationType);
        }

        public bool IsLooping() {
            return _currentOnceAnimationStatus == null;
        }

        public void OnceAnimationFinished(string animation) {
            _logger.Debug("OnceAnimationFinished: \"" + _currentOnceAnimationStatus.Animation.Name + "\"");
            _currentOnceAnimationStatus.Playing = false;
            _currentOnceAnimationStatus.Animation.OnEnd();
            _currentOnceAnimationStatus = null;
            if (_currentLoopAnimation != null) {
                _animationPlayer.Play(_currentLoopAnimation.Name);
            }
        }
    }
}