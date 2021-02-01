using Betauer.Characters.Player;
using Betauer.Characters.Player.States;
using Betauer.Tools.Input;
using Betauer.Tools.Statemachine;
using Godot;

namespace Betauer.Characters.Player {
    public abstract class PlayerState : State {

        protected readonly PlayerController Player;

        protected PlayerState(PlayerController player) {
            Player = player;
        }

        public void Debug(bool flag, string message) {
            if (flag) {
                Debug(message);
            }
        }

        public void Debug(string message) {
            GD.Print("#" + Player.GetFrame() + ": " + GetType().Name + " | " + message);
        }


        protected float XInput => Player.PlayerActions.LateralMotion.Strength;
        protected ActionState Jump => Player.PlayerActions.Jump;
        protected ActionState Attack => Player.PlayerActions.Attack;
        protected float Delta => Player.Delta;
        protected Vector2 Motion => Player.Motion;
        protected PlayerConfig PlayerConfig => Player.PlayerConfig;

        protected void GoToRunState() {
            // Change to run is immediate
            Player.SetNextState(typeof(GroundStateRun), true);
        }

        protected void GoToIdleState() {
            // Idle is deferred to the next frame
            Player.SetNextState(typeof(GroundStateIdle));
        }

        protected void GoToFallState() {
            // fall next frame
            Player.SetNextState(typeof(AirStateFall));
        }

        protected void GoToJumpState() {
            // Jump is immediate
            Player.SetNextState(typeof(AirStateJump), true);
        }

    }
}
