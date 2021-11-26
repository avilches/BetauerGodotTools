using Tools;

namespace Veronenger.Game.Character.Player {
    public class LoopAnimationIdle : LoopAnimation {
        public override string Name => "Idle";
    }

    public class LoopAnimationJump : LoopAnimation {
        public override string Name => "Jump";
    }

    public class LoopAnimationFall : LoopAnimation {
        public override string Name => "Fall";
    }

    public class LoopAnimationRun : LoopAnimation {
        public override string Name => "Run";
    }

    public class AnimationAttack : OnceAnimation {
        public override string Name => "Attack";
    }

    public class AnimationJumpAttack : OnceAnimation {
        public override string Name => "JumpAttack";
    }
}