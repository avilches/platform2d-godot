using Tools.Statemachine;
using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Player.States {
    public class AirStateJump : AirState {
        public AirStateJump(PlayerController player) : base(player) {
        }

        public override void Start(StateConfig config) {
            Player.SetMotionY(-PlayerConfig.JUMP_FORCE);
            Debug(PlayerConfig.DEBUG_JUMP_VELOCITY,
                "Jump: decelerating to " + -PlayerConfig.JUMP_FORCE);
            Player.AnimationJump.Play();
        }

        public override NextState Execute(NextState nextState) {
            CheckAttack();

            if (Jump.JustReleased && Motion.y < -PlayerConfig.JUMP_FORCE_MIN) {
                Debug(PlayerConfig.DEBUG_JUMP_VELOCITY,
                    "Short jump: decelerating from " + Motion.y + " to " + -PlayerConfig.JUMP_FORCE_MIN);
                Player.SetMotionY(-PlayerConfig.JUMP_FORCE_MIN);
            }

            Player.AddLateralMotion(XInput, PlayerConfig.ACCELERATION, PlayerConfig.AIR_RESISTANCE,
                PlayerConfig.STOP_IF_SPEED_IS_LESS_THAN, 0);
            Player.Flip(XInput);
            Player.ApplyGravity();
            Player.LimitMotion();
            Player.Slide();

            if (Motion.y >= 0) {
                return nextState.Immediate(typeof(AirStateFallShort));
            }

            return CheckLanding(nextState);

        }
    }
}