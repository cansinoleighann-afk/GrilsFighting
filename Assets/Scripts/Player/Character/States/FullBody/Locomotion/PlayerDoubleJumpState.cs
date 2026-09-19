using Animancer;
using UnityEngine;

namespace AwCon
{
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    public class PlayerDoubleJumpState : PlayerBaseState
    {
        private MotionClipData _clipData;
        private bool _canCheckLand;
        private float _jumpForce;

        public PlayerDoubleJumpState(BBBCharacterController player) : base(player) { }

        // 已修复编码乱码的注释。
        public override void Enter()
        {
            _canCheckLand = false;
            data.HasPerformedDoubleJumpInAir = true;

            SelectDoubleJumpAnimation();
            ChooseOptionsAndPlay(_clipData.Clip);
            PerformJumpPhysics();

            // 已修复编码乱码的注释。
            player.InputPipeline.ConsumeJumpPressed();

            AnimFacade.SetOnEndCallback(() =>
            {
                if (player.CharController.isGrounded)
                {
                    player.StateMachine.ChangeState(player.StateRegistry.GetState<PlayerLandState>());
                }
            });
        }

        // 已修复编码乱码的注释。
        private void SelectDoubleJumpAnimation()
        {
            bool isHandsEmpty = data.CurrentItem == null;

            // 已修复编码乱码的注释。
            MotionClipData baseClip = null;
            float baseForce = config.JumpAndLanding.JumpForce;

            switch (data.CurrentLocomotionState)
            {
                case LocomotionState.Idle:
                case LocomotionState.Walk:
                case LocomotionState.Jog:
                    baseClip = config.JumpAndLanding.DoubleJumpUp;
                    baseForce = config.JumpAndLanding.DoubleJumpForceUp;
                    break;

                case LocomotionState.Sprint:
                    if (isHandsEmpty)
                    {
                        baseClip = config.JumpAndLanding.DoubleJumpSprintRoll;
                        baseForce = config.JumpAndLanding.DoubleJumpEmptyHandSprintForceUp;
                    }
                    else
                    {
                        baseClip = config.JumpAndLanding.DoubleJumpUp;
                        baseForce = config.JumpAndLanding.DoubleJumpForceUp;
                    }
                    break;

                default:
                    Debug.Log("说明");
                    baseClip = config.JumpAndLanding.JumpAirAnim;
                    baseForce = config.JumpAndLanding.DoubleJumpForceUp;
                    break;
            }
            _clipData = baseClip;
            _jumpForce = baseForce;
        }

        // 已修复编码乱码的注释。
        private void PerformJumpPhysics()
        {
            data.VerticalVelocity = _jumpForce;
            data.IsGrounded = false;
        }

        // 已修复编码乱码的注释。
        protected override void UpdateStateLogic()
        {
            // 已修复编码乱码的注释。
            if (!_canCheckLand && AnimFacade.CurrentTime > 0.2f)
            {
                _canCheckLand = true;
            }

            // 已修复编码乱码的注释。
            if (_canCheckLand && data.VerticalVelocity <= 0 && player.CharController.isGrounded)
            {
                player.StateMachine.ChangeState(player.StateRegistry.GetState<PlayerLandState>());
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
            AnimFacade.ClearOnEndCallback();
            _clipData = null;
        }
    }
}
