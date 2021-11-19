using Veronenger.Game.Controller.Character;
using Veronenger.Game.Managers.Autoload;

namespace Veronenger.Game.Character.Player.States {
    public abstract class GroundState : PlayerState {
        public GroundState(Player2DPlatformController player2DPlatform) : base(player2DPlatform) {
        }

        protected bool CheckAttack() {
            if (!Attack.JustPressed) return false;
            // Attack was pressed
            Player2DPlatform.AnimationAttack.PlayOnce();
            return true;
        }

        protected void EnableSlopeStairs() {
            if (Player2DPlatform.IsOnSlopeStairsDown()) {
                if (IsUp) {
                    Player2DPlatform.EnableSlopeStairs();
                } else {
                    Player2DPlatform.DisableSlopeStairs();
                }
            } else if (Player2DPlatform.IsOnSlopeStairsUp()) {
                if (IsDown) {
                    Player2DPlatform.EnableSlopeStairs();
                } else {
                    Player2DPlatform.DisableSlopeStairs();
                }
            }
        }
    }
}