using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Player.States {
    public abstract class GroundState : PlayerState {
        protected GroundState(string name, PlayerController player) : base(name, player) {
        }

        protected bool CheckAttack() {
            if (!Attack.JustPressed) return false;
            // Attack was pressed
            Player.AnimationAttack.PlayOnce();
            return true;
        }

        protected void EnableSlopeStairs() {
            if (Player.IsOnSlopeStairsDown()) {
                if (IsUp) {
                    Player.EnableSlopeStairs();
                } else {
                    Player.DisableSlopeStairs();
                }
            } else if (Player.IsOnSlopeStairsUp()) {
                if (IsDown) {
                    Player.EnableSlopeStairs();
                } else {
                    Player.DisableSlopeStairs();
                }
            }
        }
    }
}