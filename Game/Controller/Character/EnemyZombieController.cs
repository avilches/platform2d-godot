using Godot;
using Tools;
using Tools.Statemachine;
using Veronenger.Game.Character;
using Veronenger.Game.Character.Enemy;
using Veronenger.Game.Character.Enemy.States;
using Veronenger.Game.Managers.Autoload;

namespace Veronenger.Game.Controller.Character {
    public class EnemyZombieController : CharacterController {
        public readonly EnemyConfig EnemyConfig = new EnemyConfig();

        public EnemyZombieController() {
        }

        public LoopAnimationStatus AnimationIdle { get; private set; }
        public OnceAnimationStatus AnimationStep { get; private set; }

        protected override StateMachine CreateStateMachine() {
            return new StateMachine(EnemyConfig, GameManager.Instance)
                .AddState(new GroundStatePatrolStep( this))
                .AddState(new GroundStatePatrolWait( this))
                .AddState(new GroundStateIdle( this));
        }

        protected override CharacterConfig CreateCharacterConfig() {
            return EnemyConfig;
        }

        protected override AnimationStack CreateAnimationStack(AnimationPlayer animationPlayer) {
            var animationStack = new AnimationStack(animationPlayer);
            AnimationIdle = animationStack.AddLoopAnimationAndGetStatus(new LoopAnimationIdle());
            AnimationStep = animationStack.AddOnceAnimationAndGetStatus(new AnimationZombieStep());
            return animationStack;
        }

        public override void _EnterTree() {
            base._EnterTree();
            StateMachine.SetNextState(typeof(GroundStateIdle));
        }

        public override void _Ready() {
            base._Ready();
            GameManager.Instance.CharacterManager.ConfigureEnemyCollisions(this);
        }

        protected override void PhysicsProcess() {
            StateMachine.Execute();
            /*
            _label.Text = "Floor: " + IsOnFloor() + "\n" +
                          "Slope: " + IsOnSlope() + "\n" +
                          "Stair: " + IsOnSlopeStairs() + "\n" +
                          "Moving: " + IsOnMovingPlatform() + "\n" +
                          "Falling: " + IsOnFallingPlatform();
            */
        }

    }
}