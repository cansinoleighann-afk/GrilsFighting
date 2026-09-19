using UnityEngine;

namespace AwCon
{
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    public class PlayerAimMoveState : PlayerBaseState
    {
        public PlayerAimMoveState(BBBCharacterController player) : base(player) { }

        private LocomotionState _lastTreeState = LocomotionState.Idle;

        // 已修复编码乱码的注释。
        public override void Enter()
        {
            player.FootstepController?.ResetCycle();
            _lastTreeState = LocomotionState.Idle;
            SwitchTreeIfNeeded(force: true);

            // 已修复编码乱码的注释。
            AnimFacade.SetMixerParameter(new Vector2(data.CurrentAnimBlendX, data.CurrentAnimBlendY));
        }

        // 已修复编码乱码的注释。
        protected override void UpdateStateLogic()
        {
            if (!data.IsAiming)
            {
                player.StateMachine.ChangeState(
                    data.CurrentLocomotionState == LocomotionState.Idle
                        ? (BaseState)player.StateRegistry.GetState<PlayerIdleState>()
                        : player.StateRegistry.GetState<PlayerMoveLoopState>());
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

            if (data.CurrentLocomotionState == LocomotionState.Idle)
            {
                player.StateMachine.ChangeState(player.StateRegistry.GetState<PlayerAimIdleState>());
                return;
            }

            // 已修复编码乱码的注释。
            SwitchTreeIfNeeded(force: false);

            // 已修复编码乱码的注释。
            AnimFacade.SetMixerParameter(new Vector2(data.CurrentAnimBlendX, data.CurrentAnimBlendY));

            // Aiming locomotion uses a separate mixer state, so it must feed the
            // same animation-phase footstep driver as normal locomotion.
            float cycle = AnimFacade.CurrentNormalizedTime;
            cycle -= Mathf.Floor(cycle);
            data.CurrentRunCycleTime = cycle;
            player.FootstepController?.UpdateLoop(cycle);
        }

        // 已修复编码乱码的注释。
        public override void PhysicsUpdate()
        {
            player.MotionDriver.UpdateMotion(null, 0f);
        }

        public override void Exit()
        {
            // 已修复编码乱码的注释。
            // 已修复编码乱码的注释。
        }

        // 已修复编码乱码的注释。
        private void SwitchTreeIfNeeded(bool force)
        {
            // 已修复编码乱码的注释。
            // 已修复编码乱码的注释。
            LocomotionState desired = data.CurrentLocomotionState;
            if (desired == LocomotionState.Idle)
                desired = LocomotionState.Jog;

            if (!force && desired == _lastTreeState)
                return;

            _lastTreeState = desired;

            var options = AnimPlayOptions.Default;
            options.FadeDuration = 0.12f;

            // 已修复编码乱码的注释。
            var aiming = config.Aiming;
            if (aiming == null)
            {
                Debug.LogError("说明");
                return;
            }

            object tree = desired switch
            {
                LocomotionState.Walk => aiming.AimWalkMixer,
                LocomotionState.Jog => aiming.AimJogMixer,
                LocomotionState.Sprint => aiming.AimSprintMixer,
                _ => aiming.AimJogMixer
            };

            AnimFacade.PlayTransition(tree, options);
        }

    }
}
