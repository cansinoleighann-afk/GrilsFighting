using System;
using UnityEngine;

namespace AwCon
{
    [Serializable]
    public sealed class OverrideState : PlayerBaseState
    {
        private bool _applied;

        public int CurrentPriority => data.Override.IsActive ? data.Override.Request.Priority : 0;

        public OverrideState(BBBCharacterController player) : base(player) { }

        public override void Enter()
        {
            _applied = false;

            // 已修复编码乱码的注释。
            data.Arbitration.BlockInventory = true;

            Apply();
        }

        public override void Exit()
        {
            // 已修复编码乱码的注释。
            AnimFacade.ClearOverrideOnEndCallback();

            // 已修复编码乱码的注释。
            AnimFacade.StopFullBodyAction();

            data.Override.Clear();
            data.Arbitration.BlockInventory = false;
        }

        protected override bool CheckInterrupts() => false;

        protected override void UpdateStateLogic()
        {
            if (!_applied) Apply();
        }

        public override void PhysicsUpdate()
        {
            if (!data.Override.IsActive) return;

            if (data.Override.Request.ApplyGravity)
                player.MotionDriver.UpdateGravityOnly();
        }

        // 已修复编码乱码的注释。
        public void ForceReapply()
        {
            _applied = false;
            Apply();
        }

        private void Apply()
        {
            if (!data.Override.IsActive) return;

            _applied = true;

            var req = data.Override.Request;

            AnimFacade.PlayFullBodyAction(req.Clip, req.FadeDuration);

            // 已修复编码乱码的注释。
            AnimFacade.SetOverrideOnEndCallback(OnClipEnd);
        }

        private void OnClipEnd()
        {
            AnimFacade.ClearOverrideOnEndCallback();

            if (!data.Override.IsActive) return;

            if (data.Override.ReturnState != null)
            {
                player.StateMachine.ChangeState(data.Override.ReturnState);
                return;
            }

            if (data.CurrentLocomotionState != LocomotionState.Idle)
                player.StateMachine.ChangeState(player.StateRegistry.GetState<PlayerMoveLoopState>());
            else
                player.StateMachine.ChangeState(player.StateRegistry.GetState<PlayerIdleState>());
        }
    }
}
