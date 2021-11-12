using Godot;
using Veronenger.Game.Controller.Character;
using Animation = Tools.Animation;

namespace Veronenger.Game.Character.Player.Animations {
    public class AnimationIdle : Animation {
        public AnimationIdle(string name) : base(name, true) {
        }
    }

    public class AnimationJump : Animation {
        public AnimationJump(string name) : base(name, true) {
        }
    }

    public class AnimationFall : Animation {
        public AnimationFall(string name) : base(name, true) {
        }
    }

    public class AnimationRun : Animation {
        public AnimationRun(string name) : base(name, true) {
        }
    }

    public class AnimationAttack : Animation {
        private readonly PlayerController Player;

        public AnimationAttack(string name, PlayerController player) : base(name, false) {
            Player = player;
        }

        public override void CanStart() {
            Player.IsAttacking = true;
        }

        public override void OnEnd() {
            Player.IsAttacking = false;
        }
    }

    public class AnimationJumpAttack : Animation {
        private readonly PlayerController Player;

        public AnimationJumpAttack(string name, PlayerController player) : base(name, false) {
            Player = player;
        }

        public override void CanStart() {
            Player.IsAttacking = true;
        }

        public override void OnEnd() {
            Player.IsAttacking = false;
        }
    }
}