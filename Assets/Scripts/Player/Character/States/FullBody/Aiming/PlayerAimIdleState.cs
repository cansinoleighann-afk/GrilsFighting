namespace AwCon
{
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    public class PlayerAimIdleState : PlayerBaseState
    {
        public PlayerAimIdleState(BBBCharacterController player) : base(player) { }

        // 已修复编码乱码的注释。
        public override void Enter()
        {
            var options = AnimPlayOptions.Default;
            options.FadeDuration = 0.4f;
            options.NormalizedTime = 0f;
            AnimFacade.PlayTransition(config.LocomotionAnims.IdleAnim, options);
        }

        // 已修复编码乱码的注释。
        protected override void UpdateStateLogic()
        {
            if (!data.IsAiming)
            {
                player.StateMachine.ChangeState(player.StateRegistry.GetState<PlayerIdleState>());
                return;
            }

            if (data.WantsDoubleJump)
            {
                player.StateMachine.ChangeState(player.StateRegistry.GetState<PlayerDoubleJumpState>());
                return;
            }

            if (data.WantsToJump)
            {
                player.StateMachine.ChangeState(player.StateRegistry.GetState<PlayerJumpState>());
                return;
            }

            if (data.CurrentLocomotionState != LocomotionState.Idle)
            {
                player.StateMachine.ChangeState(player.StateRegistry.GetState<PlayerAimMoveState>());
            }
        }

        // 已修复编码乱码的注释。
        public override void PhysicsUpdate()
        {
            player.MotionDriver.UpdateMotion(null, 0f);
        }

        // 已修复编码乱码的注释。
        public override void Exit()
        {
        }
    }
}
