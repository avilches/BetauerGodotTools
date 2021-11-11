using System;
using System.Collections.Generic;
using Godot;
using Tools;
using Object = Godot.Object;

namespace Veronenger.Game.Character.Player.Animations {
    public abstract class AnimationState {
        public readonly string Name;

        protected AnimationState(string name) {
            Name = name;
        }

        public virtual bool OnStart() {
            return true;
        }
        public virtual void OnEnd() {
        }
    }

    public class AnimationStateMachine  : Object /* needed to listen signals */ {
        private readonly Dictionary<Type, AnimationState> _states = new Dictionary<Type, AnimationState>();
        private readonly Dictionary<string, AnimationState> _statesByName = new Dictionary<string, AnimationState>();
        private readonly AnimationPlayer _animationPlayer;
        private AnimationState _currentAnimation;
        private AnimationState _previousAnimation;
        // private boolean IsAttacking;

        public AnimationStateMachine(AnimationPlayer animationPlayer) {
            _animationPlayer = animationPlayer;
            _animationPlayer.Connect(GodotConstants.GODOT_SIGNAL_animation_finished, this, nameof(OnAnimationFinished));

        }

        public AnimationStateMachine AddAnimation(AnimationState state) {
            _states.Add(state.GetType(), state);
            _statesByName[state.Name] = state;
            return this;
        }

        public void Play(Type animationState) {
            var newAnimation = _states[animationState];
            // GD.Print("AnimationPlay "+animationState.Name+": "+newAnimation?.Name);
            AnimationPlay(newAnimation);
        }

        private void AnimationPlay(AnimationState newAnimation) {
            if (newAnimation == null ||_currentAnimation == newAnimation) return;
            if (!newAnimation.OnStart()) {
                _previousAnimation = newAnimation;
            } else {
                _previousAnimation = _currentAnimation;
                _animationPlayer.Play(newAnimation.Name);
                _currentAnimation = newAnimation;
            }
        }

        public void OnAnimationFinished(string animation) {
            _statesByName[animation].OnEnd();
            // var attackingAnimation = animation == ATTACK_ANIMATION || animation == JUMP_ATTACK_ANIMATION;
            // if (attackingAnimation) {
                // IsAttacking = false;
            // }

            // GD.Print($"IsAttacking {IsAttacking} (finished {animation} is attacking {attackingAnimation})");
            if (_previousAnimation != null) {
                AnimationPlay(_previousAnimation);
            }
        }
    }
}