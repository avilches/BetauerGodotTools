using System;
using Godot;
using Object = Godot.Object;

namespace Tools {
    public class AnimationStatus : Object {
        protected AnimationStack _animationStack;

        public AnimationStatus(AnimationStack animationStack) {
            _animationStack = animationStack;
        }
    }

    public class LoopAnimationStatus : AnimationStatus {
        public readonly Animation GodotAnimation;
        public readonly string Name;

        protected internal LoopAnimationStatus(AnimationStack animationStack, string name, Animation loopAnimation) : base(
            animationStack) {
            GodotAnimation = loopAnimation;
            Name = name;
        }

        public void PlayLoop() {
            _animationStack.PlayLoop(this);
        }
    }

    public class OnceAnimationStatus : AnimationStatus {
        public readonly Animation GodotAnimation;
        public readonly string Name;

        public bool Playing { get; set; } = false;
        public bool Interrupted { get; set; } = false;

        public bool CanBeInterrupted;
        public bool KillPreviousAnimation;

        protected internal OnceAnimationStatus(AnimationStack animationStack, string name, Animation animation,
            bool canBeInterrupted = true, bool killPreviousAnimation = false) : base(animationStack) {
            GodotAnimation = animation;
            Name = name;
            CanBeInterrupted = canBeInterrupted;
            KillPreviousAnimation = killPreviousAnimation;
        }

        public void PlayOnce(bool killPreviousAnimation = false) {
            _animationStack.PlayOnce(this, killPreviousAnimation);
        }
    }


    public class AnimationStack : Object /* needed to listen signals */ {
        private readonly System.Collections.Generic.Dictionary<string, LoopAnimationStatus> _loopAnimations =
            new System.Collections.Generic.Dictionary<string, LoopAnimationStatus>();

        private readonly System.Collections.Generic.Dictionary<string, OnceAnimationStatus> _onceAnimationsStatuses =
            new System.Collections.Generic.Dictionary<string, OnceAnimationStatus>();

        private readonly AnimationPlayer _animationPlayer;
        private LoopAnimationStatus _currentLoopAnimation;
        private OnceAnimationStatus _currentOnceAnimationStatus;
        private readonly Logger _logger;

        public AnimationStack(string name, AnimationPlayer animationPlayer) {
            _animationPlayer = animationPlayer;
            _animationPlayer.Connect(GodotConstants.GODOT_SIGNAL_animation_finished, this,
                nameof(OnceAnimationFinished));
            _logger = LoggerFactory.GetLogger(name, "Animation");
        }

        public AnimationStack AddLoopAnimation(string name) {
            AddLoopAnimationAndGetStatus(name);
            return this;
        }

        public LoopAnimationStatus AddLoopAnimationAndGetStatus(string name) {
            Animation animation = _animationPlayer.GetAnimation(name);
            if (animation == null) {
                throw new Exception(
                    $"Animation {name} not found in AnimationPlayer {_animationPlayer.Name}");
            }
            animation.Loop = true;
            var loopAnimationStatus = new LoopAnimationStatus(this, name, animation);
            _loopAnimations.Add(name, loopAnimationStatus);
            // _animationsByName[newAnimation.Name] = newAnimation;
            return loopAnimationStatus;
        }

        public AnimationStack AddOnceAnimation(string newAnimation) {
            AddOnceAnimationAndGetStatus(newAnimation);
            return this;
        }

        public OnceAnimationStatus AddOnceAnimationAndGetStatus(string name) {
            Animation animation = _animationPlayer.GetAnimation(name);
            if (animation == null) {
                throw new Exception(
                    $"Animation {name} not found in AnimationPlayer {_animationPlayer.Name}");
            }
            animation.Loop = false;

            var onceAnimationStatus = new OnceAnimationStatus(this, name, animation);
            _onceAnimationsStatuses.Add(name, onceAnimationStatus);
            // _animationsByName[newAnimation.Name] = newAnimation;
            return onceAnimationStatus;
        }

        public OnceAnimationStatus GetOnceAnimationStatus(string name) {
            OnceAnimationStatus onceAnimationStatus = _onceAnimationsStatuses[name];
            if (onceAnimationStatus == null) {
                throw new Exception($"Animation {name} not found in Animator");
            }
            return onceAnimationStatus;
        }


        public void PlayLoop(string name) {
            LoopAnimationStatus loopAnimation = _loopAnimations[name];
            if (loopAnimation == null) {
                throw new Exception($"Animation {name} not found in Animator");
            }
            PlayLoop(loopAnimation);
        }

        protected internal void PlayLoop(LoopAnimationStatus loopAnimation) {
            if (_currentLoopAnimation == null || _currentLoopAnimation.Name != loopAnimation.Name) {
                _logger.Debug("PlayLoop \"" + loopAnimation.Name + "\"");
                _currentLoopAnimation = loopAnimation;
                if (_currentOnceAnimationStatus == null) {
                    _animationPlayer.Play(loopAnimation.Name);
                }
            }
        }

        public OnceAnimationStatus PlayOnce(string name, bool killPreviousAnimation = false) {
            OnceAnimationStatus newOnceAnimationStatus = GetOnceAnimationStatus(name);
            return PlayOnce(newOnceAnimationStatus, killPreviousAnimation);
        }

        protected internal OnceAnimationStatus PlayOnce(OnceAnimationStatus newOnceAnimationStatus,
            bool killPreviousAnimation = false) {
            if (_currentOnceAnimationStatus == null || _currentOnceAnimationStatus.CanBeInterrupted ||
                newOnceAnimationStatus.KillPreviousAnimation || killPreviousAnimation) {
                if (_currentOnceAnimationStatus != null) {
                    _logger.Debug("PlayOnce \"" + newOnceAnimationStatus.Name + "\" (Interrupting: " +
                                  _currentOnceAnimationStatus.Name + "\")");
                    _currentOnceAnimationStatus.Playing = false;
                    _currentOnceAnimationStatus.Interrupted = true;
                    if (_currentOnceAnimationStatus == newOnceAnimationStatus) {
                        _animationPlayer.Stop();
                    }
                } else {
                    _logger.Debug("PlayOnce \"" + newOnceAnimationStatus.Name + "\"");
                }
                _currentOnceAnimationStatus = newOnceAnimationStatus;
                newOnceAnimationStatus.Playing = true;
                newOnceAnimationStatus.Interrupted = false;
                _animationPlayer.Play(newOnceAnimationStatus.Name);
            } else if (!_currentOnceAnimationStatus.CanBeInterrupted) {
                _logger.Error("PlayOnce \"" + newOnceAnimationStatus.Name +
                              "\" not played: current animation \"" +
                              _currentOnceAnimationStatus.Name + "\" can't be interrupted)");
            }
            return newOnceAnimationStatus;
        }

        public bool IsPlayingLoopAnimation(string animationType) {
            if (_currentLoopAnimation == null) return false;
            return _currentLoopAnimation.Name == animationType;
        }

        public bool IsPlayingOnceAnimation(string animationType) {
            if (_currentOnceAnimationStatus == null) return false;
            return _currentOnceAnimationStatus.Name == animationType;
        }

        public bool IsPlaying(string name) {
            return IsPlayingLoopAnimation(name) || IsPlayingOnceAnimation(name);
        }

        public bool IsLooping() {
            return _currentOnceAnimationStatus == null;
        }

        public void OnceAnimationFinished(string animation) {
            _logger.Debug("OnceAnimationFinished: \"" + _currentOnceAnimationStatus.Name + "\"");
            _currentOnceAnimationStatus.Playing = false;
            _currentOnceAnimationStatus = null;
            if (_currentLoopAnimation != null) {
                _animationPlayer.Play(_currentLoopAnimation.Name);
            }
        }
    }
}