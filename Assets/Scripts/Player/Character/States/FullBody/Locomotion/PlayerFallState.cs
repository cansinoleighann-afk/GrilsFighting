using UnityEngine;

namespace AwCon
{
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    public class PlayerFallState : PlayerBaseState
    {
        public PlayerFallState(BBBCharacterController player) : base(player) { }

        // 已修复编码乱码的注释。
        public override void Enter()
        {
            ChooseOptionsAndPlay(config.LocomotionAnims.FallAnim);
        }

        // 已修复编码乱码的注释。
        protected override void UpdateStateLogic()
        {
            // 已修复编码乱码的注释。
            if (data.IsGrounded)
            {
                player.StateMachine.ChangeState(player.StateRegistry.GetState<PlayerLandState>());
                return;
            }
        }

        // 已修复编码乱码的注释。
        public override void PhysicsUpdate()
        {
            player.MotionDriver.UpdateMotion();
        }

        // 已修复编码乱码的注释。
        public override void Exit()
        {
        }
    }
}
