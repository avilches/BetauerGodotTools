using Tools;

namespace Veronenger.Game.Character.Player {
    public class LoopAnimationIdle : LoopAnimation {
        public LoopAnimationIdle(string name) : base(name) {
        }
    }

    public class LoopAnimationJump : LoopAnimation {
        public LoopAnimationJump(string name) : base(name) {
        }
    }

    public class LoopAnimationFall : LoopAnimation {
        public LoopAnimationFall(string name) : base(name) {
        }
    }

    public class LoopAnimationRun : LoopAnimation {
        public LoopAnimationRun(string name) : base(name) {
        }
    }

    public class AnimationAttack : OnceAnimation {
        public AnimationAttack(string name) : base(name) {
        }
    }

    public class AnimationJumpAttack : OnceAnimation {
        public AnimationJumpAttack(string name) : base(name) {
        }
    }
}