using Tools;

namespace Veronenger.Game.Character.Enemy {
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

    public class AnimationDie : OnceAnimation {
        public override string Name => "Die";
        public override bool KillPreviousAnimation => true;
    }

    public class AnimationZombieStep : OnceAnimation {
        public override string Name => "Step";
    }

    public class AnimationAttack : OnceAnimation {
        public override string Name => "Attack";
    }
}