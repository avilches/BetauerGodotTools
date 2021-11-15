using System;
using Godot;
using Godot.Collections;
using Object = Godot.Object;

namespace Tools {
    public abstract class Animation {
        public abstract string Name { get; }
        public Godot.Animation GodotAnimation { get; set; }
    }

    public abstract class LoopAnimation : Animation {
    }

    public abstract class OnceAnimation : Animation {
        public abstract bool CanBeInterrupted { get; }

        public virtual void OnStart() {
        }

        public virtual void OnEnd() {
        }
    }

    public class OnceAnimationStatus : Object {
        public OnceAnimation Animation { get; set; }
        public bool Finished { get; set; } = false;
        public bool Interrupted { get; set; } = false;
        public bool CanBeInterrupted => Animation.CanBeInterrupted;

        public OnceAnimationStatus(OnceAnimation animation) {
            Animation = animation;
        }
    }


    public class AnimationStack : Object /* needed to listen signals */ {
        private readonly System.Collections.Generic.Dictionary<Type, LoopAnimation> _loopAnimations =
            new System.Collections.Generic.Dictionary<Type, LoopAnimation>();

        private readonly System.Collections.Generic.Dictionary<Type, OnceAnimationStatus> _onceAnimationsStatuses =
            new System.Collections.Generic.Dictionary<Type, OnceAnimationStatus>();

        // private readonly System.Collections.Generic.Dictionary<string, Animation> _animationsByName =
        // new System.Collections.Generic.Dictionary<string, Animation>();

        private readonly AnimationPlayer _animationPlayer;
        private LoopAnimation _currentLoopAnimation;
        private OnceAnimationStatus _currentOnceAnimationStatus;

        public AnimationStack(AnimationPlayer animationPlayer) {
            _animationPlayer = animationPlayer;
            _animationPlayer.Connect(GodotConstants.GODOT_SIGNAL_animation_finished, this,
                nameof(OnAnimationOnceFinished));
        }

        public AnimationStack AddLoopAnimation(LoopAnimation newAnimation) {
            Godot.Animation godotAnimation = _animationPlayer.GetAnimation(newAnimation.Name);
            if (godotAnimation == null) {
                throw new Exception(
                    $"Animation {newAnimation.Name} not found in AnimationPlayer {_animationPlayer.Name}");
            }
            godotAnimation.Loop = true;
            newAnimation.GodotAnimation = godotAnimation;

            _loopAnimations.Add(newAnimation.GetType(), newAnimation);
            // _animationsByName[newAnimation.Name] = newAnimation;
            return this;
        }

        public AnimationStack AddOnceAnimation(OnceAnimation newAnimation) {
            AddOnceAnimationAndGetStatus(newAnimation);
            return this;
        }

        public OnceAnimationStatus GetOnceAnimationStatus(Type animationType) {
            OnceAnimationStatus onceAnimationStatus = _onceAnimationsStatuses[animationType];
            if (onceAnimationStatus == null) {
                throw new Exception($"Animation {animationType.Name} not found in Animator");
            }
            return onceAnimationStatus;
        }

        public OnceAnimationStatus AddOnceAnimationAndGetStatus(OnceAnimation newAnimation) {
            Godot.Animation godotAnimation = _animationPlayer.GetAnimation(newAnimation.Name);
            if (godotAnimation == null) {
                throw new Exception(
                    $"Animation {newAnimation.Name} not found in AnimationPlayer {_animationPlayer.Name}");
            }
            godotAnimation.Loop = false;
            newAnimation.GodotAnimation = godotAnimation;

            var onceAnimationStatus = new OnceAnimationStatus(newAnimation);
            _onceAnimationsStatuses.Add(newAnimation.GetType(), onceAnimationStatus);
            // _animationsByName[newAnimation.Name] = newAnimation;
            return onceAnimationStatus;
        }


        public void PlayLoop(Type newAnimationType) {
            LoopAnimation loopAnimation = _loopAnimations[newAnimationType];
            if (loopAnimation == null) {
                throw new Exception($"Animation {newAnimationType.Name} not found in Animator");
            }
            if (_currentLoopAnimation != loopAnimation) {
                // GD.Print("PlayLoop From: "+_loopAnimation?.Name+" | To: "+newAnimation?.Name);
                _currentLoopAnimation = loopAnimation;
                if (_currentOnceAnimationStatus == null) {
                    _animationPlayer.Play(loopAnimation.Name);
                }
            }
        }

        public OnceAnimationStatus PlayOnce(Type newAnimationType) {
            OnceAnimationStatus newOnceAnimationStatus = GetOnceAnimationStatus(newAnimationType);

            if (_currentOnceAnimationStatus == null || _currentOnceAnimationStatus.CanBeInterrupted) {
                if (_currentOnceAnimationStatus != null) {
                    // GD.Print("PlayOnce Interrupting: "+_onceOnceAnimationOnceStatus.Animation.Name+ " "+_onceOnceAnimationOnceStatus.GetHashCode());
                    _currentOnceAnimationStatus.Finished = true;
                    _currentOnceAnimationStatus.Interrupted = true;
                    _currentOnceAnimationStatus.Animation.OnEnd();
                    if (_currentOnceAnimationStatus == newOnceAnimationStatus) {
                        _animationPlayer.Stop();
                    }
                }
                _currentOnceAnimationStatus = newOnceAnimationStatus;
                // GD.Print("PlayOnce new: "+newAnimation.Name+ " "+_onceOnceAnimationOnceStatus.GetHashCode());
                newOnceAnimationStatus.Animation.OnStart();
                _animationPlayer.Play(newOnceAnimationStatus.Animation.Name);
            }
            return _currentOnceAnimationStatus;
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

        public void OnAnimationOnceFinished(string animation) {
            // GD.Print("OnAnimationOnceFinished finishing: "+_onceOnceAnimationOnceStatus.Animation.Name+ " "+_onceOnceAnimationOnceStatus.GetHashCode());
            _currentOnceAnimationStatus.Finished = true;
            _currentOnceAnimationStatus.Animation.OnEnd();
            _currentOnceAnimationStatus = null;
            if (_currentLoopAnimation != null) {
                _animationPlayer.Play(_currentLoopAnimation.Name);
            }
        }
    }
}