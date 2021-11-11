using Godot;
using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Player.Animations {
    public class AnimationIdle : AnimationState {
        public AnimationIdle(string name) : base(name) {
        }
    }

    public class AnimationJump : AnimationState {
        public AnimationJump(string name) : base(name) {
        }
    }

    public class AnimationAttack : AnimationState {
        private PlayerController _playerController;

        public AnimationAttack(string name, PlayerController playerController) : base(name) {
            _playerController = playerController;
        }

        public override bool OnStart() {
            return false;
        }

        public override void OnEnd() {
            _playerController.IsAttacking = false;
        }
    }

    public class AnimationJumpAttack : AnimationState {
        private PlayerController _playerController;

        public AnimationJumpAttack(string name, PlayerController playerController) : base(name) {
            _playerController = playerController;
        }

        public override bool OnStart() {
            return false;
        }

        public override void OnEnd() {
            _playerController.IsAttacking = false;
        }
    }

    public class AnimationFall : AnimationState {
        public AnimationFall(string name) : base(name) {
        }
    }

    public class AnimationRun : AnimationState {
        public AnimationRun(string name) : base(name) {
        }
    }
}