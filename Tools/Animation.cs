using System;
using System.Collections.Generic;
using Godot;
using Object = Godot.Object;

namespace Tools {
    public abstract class Animation {
        public readonly string Name;
        public Godot.Animation GodotAnimation { get; set; }

        protected Animation(string name) {
            Name = name;
        }
    }

    public abstract class LoopAnimation : Animation {
        protected LoopAnimation(string name) : base(name) {
        }
    }

    public abstract class OnceAnimation : Animation {
        protected OnceAnimation(string name) : base(name) {
        }

        public virtual void OnStart() {
        }

        public virtual void OnEnd() {
        }
    }


    public class AnimationStack : Object /* needed to listen signals */ {
        private readonly Dictionary<Type, Animation> _states = new Dictionary<Type, Animation>();
        private readonly Dictionary<string, Animation> _statesByName = new Dictionary<string, Animation>();
        private readonly AnimationPlayer _animationPlayer;
        private LoopAnimation _loopAnimation;
        private OnceAnimation _onceAnimation;

        public AnimationStack(AnimationPlayer animationPlayer) {
            _animationPlayer = animationPlayer;
            _animationPlayer.Connect(GodotConstants.GODOT_SIGNAL_animation_finished, this,
                nameof(OnAnimationOnceFinished));
        }

        public AnimationStack AddLoopAnimation(LoopAnimation newAnimation) {
            return _AddAnimation(newAnimation, true);
        }

        public AnimationStack AddOnceAnimation(OnceAnimation newAnimation) {
            return _AddAnimation(newAnimation, false);
        }

        private AnimationStack _AddAnimation(Animation newAnimation, bool loop) {
            Godot.Animation godotAnimation = _animationPlayer.GetAnimation(newAnimation.Name);
            if (godotAnimation == null) {
                throw new Exception(
                    $"Animation {newAnimation.Name} not found in AnimationPlayer {_animationPlayer.Name}");
            }
            godotAnimation.Loop = loop;
            newAnimation.GodotAnimation = godotAnimation;

            _states.Add(newAnimation.GetType(), newAnimation);
            _statesByName[newAnimation.Name] = newAnimation;
            return this;
        }

        public void PlayLoop(Type animationType) {
            Animation newAnimation = _states[animationType];
            if (newAnimation is LoopAnimation loopAnimation) {
                if (_loopAnimation != loopAnimation) {
                    // GD.Print("AnimationPlay from:"+_currentAnimation?.Name+" | To: "+newAnimation?.Name);
                    // AnimationPlay(newAnimation);
                    _loopAnimation = loopAnimation;
                    if (_onceAnimation == null) {
                        _animationPlayer.Play(newAnimation.Name);
                    }
                }
            } else {
                throw new Exception($"Animation {newAnimation.Name} must be a LoopAnimation");
            }
        }

        public void PlayOnce(Type animationType) {
            Animation newAnimation = _states[animationType];
            if (newAnimation is OnceAnimation onceAnimation) {
                if (_onceAnimation != onceAnimation) {
                    // GD.Print("AnimationPlay from:"+_currentAnimation?.Name+" | To: "+newAnimation?.Name);
                    // AnimationPlay(newAnimation);
                    _onceAnimation = onceAnimation;
                    _onceAnimation.OnStart();
                    _animationPlayer.Play(newAnimation.Name);
                }
            } else {
                throw new Exception($"Animation {newAnimation.Name} must be a OnceAnimation");
            }
        }

        public bool IsPlaying(Type animationType) {
            return (_loopAnimation != null && _loopAnimation.GetType() == animationType) ||
                   (_onceAnimation != null && _onceAnimation.GetType() == animationType);
        }

        public bool IsLooping() {
            return _onceAnimation == null;
        }

        public void OnAnimationOnceFinished(string animation) {
            (_statesByName[animation] as OnceAnimation).OnEnd();
            _onceAnimation = null;
            if (_loopAnimation != null) {
                _animationPlayer.Play(_loopAnimation.Name);
            }
        }
    }
}