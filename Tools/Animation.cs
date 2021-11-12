using System;
using System.Collections.Generic;
using Godot;
using Object = Godot.Object;

namespace Tools {
    public abstract class Animation {
        public readonly string Name;
        public readonly bool Loop;
        public Godot.Animation GodotAnimation { get; set; }

        protected Animation(string name, bool loop) {
            Name = name;
            Loop = loop;
        }

        public virtual void CanStart() {
        }

        public virtual void OnEnd() {
        }
    }

    public class AnimationStack : Object /* needed to listen signals */ {
        private readonly Dictionary<Type, Animation> _states = new Dictionary<Type, Animation>();
        private readonly Dictionary<string, Animation> _statesByName = new Dictionary<string, Animation>();
        private readonly AnimationPlayer _animationPlayer;
        private Animation _loopAnimation;
        private Animation _onceAnimation;

        public AnimationStack(AnimationPlayer animationPlayer) {
            _animationPlayer = animationPlayer;
            _animationPlayer.Connect(GodotConstants.GODOT_SIGNAL_animation_finished, this, nameof(OnAnimationFinished));
        }

        public AnimationStack AddLoopAnimation(Animation newAnimation) {
            if (!newAnimation.Loop) {
                throw new Exception($"Animation {newAnimation.Name} is not a loop");
            }
            return _AddAnimation(newAnimation, true);
        }
        public AnimationStack AddOnceAnimation(Animation newAnimation) {
            if (newAnimation.Loop) {
                throw new Exception($"Animation {newAnimation.Name} is not a loop");
            }
            return _AddAnimation(newAnimation, false);
        }

        private AnimationStack _AddAnimation(Animation newAnimation, bool loop) {
            Godot.Animation godotAnimation = _animationPlayer.GetAnimation(newAnimation.Name);
            if (godotAnimation == null) {
                throw new Exception($"Animation {newAnimation.Name} not found in AnimationPlayer {_animationPlayer.Name}");
            }
            godotAnimation.Loop = loop;
            newAnimation.GodotAnimation = godotAnimation;

            _states.Add(newAnimation.GetType(), newAnimation);
            _statesByName[newAnimation.Name] = newAnimation;
            return this;
        }

        public void PlayLoop(Type animationType) {
            Animation newAnimation = _states[animationType];
            if (!newAnimation.Loop) {
                throw new Exception($"Animation {newAnimation.Name} is not a loop");
            }
            if (_loopAnimation != newAnimation) {
                // GD.Print("AnimationPlay from:"+_currentAnimation?.Name+" | To: "+newAnimation?.Name);
                // AnimationPlay(newAnimation);
                _loopAnimation = newAnimation;
                if (_onceAnimation == null) {
                    _animationPlayer.Play(newAnimation.Name);
                }
            }
        }

        public void PlayOnce(Type animationType) {
            Animation newAnimation = _states[animationType];
            if (newAnimation.Loop) {
                throw new Exception($"Animation {newAnimation.Name} can't be a loop");
            }
            if (_onceAnimation != newAnimation) {
                // GD.Print("AnimationPlay from:"+_currentAnimation?.Name+" | To: "+newAnimation?.Name);
                // AnimationPlay(newAnimation);
                _onceAnimation = newAnimation;
                _animationPlayer.Play(newAnimation.Name);
            }
        }

        public bool IsPlaying(Type animationType) {
            var newAnimation = _states[animationType];
            return newAnimation != null && _loopAnimation == newAnimation;
        }

        public bool IsLooping() {
            return _onceAnimation == null;
        }


        private void AnimationPlay(Animation newAnimation) {
            if (newAnimation == null || _loopAnimation == newAnimation) return;
            if (_loopAnimation != null && _loopAnimation.Loop) {
                _onceAnimation = _loopAnimation;
            }
            newAnimation.CanStart();
            _animationPlayer.Play(newAnimation.Name);
            _loopAnimation = newAnimation;
        }

        public void OnAnimationFinished(string animation) {
            // _statesByName[animation].OnEnd();
            _onceAnimation = null;
            if (_loopAnimation != null) {
                _animationPlayer.Play(_loopAnimation.Name);

            }
        }

        public void OnAnimationFinishedOld(string animation) {
            _statesByName[animation].OnEnd();
            // GD.Print($"IsAttacking {IsAttacking} (finished {animation} is attacking {attackingAnimation})");
            if (_onceAnimation != null) {
                // GD.Print("OnAnimationFinished "+animation+". Playing previous: "+_previousAnimation.Name);
                AnimationPlay(_onceAnimation);
            }
        }
    }
}