using Godot;
using System;

public class StateJump : StateAir {
    public StateJump(PlayerController player) : base(player) {
    }

    private float time_jump_pressed = 0;
    private float jumps = 0;

    public override void Start() {
        time_jump_pressed = 0;
        jumps = 0;
        Player.SetMotionY(-PlayerConfig.JUMP_FORCE);
        Player.AnimateJump();
    }

    public override void Execute() {
        if (Motion.y > 0) {
            GoToFallState();
            return;
        }

        Player.AddLateralMovement(XInput, PlayerConfig.ACCELERATION, PlayerConfig.AIR_RESISTANCE,
            PlayerConfig.STOP_IF_SPEED_IS_LESS_THAN, 0);
        Player.Flip(XInput);
        Player.ApplyGravity();
        Player.LimitMotion();
        Player.Slide();

        CheckLanding();
    }
}