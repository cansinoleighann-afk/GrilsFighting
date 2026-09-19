using UnityEngine;

namespace AwCon
{
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    public class PlayerDodgeState : PlayerBaseState
    {
        // 已修复编码乱码的注释。
        private WarpedMotionData _selectedData;

        // 已修复编码乱码的注释。
        private float _stateDuration;
        private bool _endTimeTriggered;

        public PlayerDodgeState(BBBCharacterController player) : base(player) { }

        // 已修复编码乱码的注释。
        protected override bool CheckInterrupts() => false;

        // 已修复编码乱码的注释。
        public override void Enter()
        {
            data.IsDodgeing = true;
            data.WantsToDodge = false;

            // 已修复编码乱码的注释。
            data.SfxQueue.Enqueue(PlayerSfxEvent.Dodge);

            _stateDuration = 0f;
            _endTimeTriggered = false;

            // 已修复编码乱码的注释。
            _selectedData = GetDodgeData();

            // 已修复编码乱码的注释。
            if (_selectedData == null || _selectedData.Clip == null)
            {
                player.StateMachine.ChangeState(player.StateRegistry.GetState<PlayerIdleState>());
                return;
            }

            // 已修复编码乱码的注释。
            player.MotionDriver.InitializeWarpData(_selectedData);

            ChooseOptionsAndPlay(_selectedData.Clip);

            // 已修复编码乱码的注释。
            player.AnimFacade.SetOnEndCallback(() =>
            {
                if (_endTimeTriggered) return;
                HandleDodgeEnd();
            });

            // 已修复编码乱码的注释。
            data.ExpectedFootPhase = _selectedData.EndPhase;
        }

        // 已修复编码乱码的注释。
        protected override void UpdateStateLogic()
        {
        }

        // 已修复编码乱码的注释。
        public override void PhysicsUpdate()
        {
            if (_selectedData == null) return;

            float normalizedTime = player.AnimFacade.CurrentNormalizedTime;
            player.MotionDriver.UpdateWarpMotion(normalizedTime);

            // 已修复编码乱码的注释。
            _stateDuration = player.AnimFacade.CurrentTime;

            if (!_endTimeTriggered && _selectedData.EndTime > 0f && _stateDuration >= _selectedData.EndTime)
            {
                _endTimeTriggered = true;
                HandleDodgeEnd();
                return;
            }
        }

        // 已修复编码乱码的注释。
        public override void Exit()
        {
            data.IsDodgeing = false;
            data.WantsToDodge = false;

            player.MotionDriver.ClearWarpData();

            player.AnimFacade.ClearOnEndCallback();

            _selectedData = null;
        }

        // 已修复编码乱码的注释。
        private WarpedMotionData GetDodgeData()
        {
            float angle = data.DesiredLocalMoveAngle;

            const float SectorAngle = 45f;
            const float HalfSectorAngle = 22.5f;

            // 已修复编码乱码的注释。
            if (angle > -HalfSectorAngle && angle <= HalfSectorAngle)
                return config.Dodging.ForwardDodge;

            if (angle > HalfSectorAngle && angle <= HalfSectorAngle + SectorAngle)
                return config.Dodging.ForwardRightDodge;

            if (angle > HalfSectorAngle + SectorAngle && angle <= HalfSectorAngle + SectorAngle * 2)
                return config.Dodging.RightDodge;

            if (angle > HalfSectorAngle + SectorAngle * 2 && angle <= 180f - HalfSectorAngle)
                return config.Dodging.BackwardRightDodge;

            if (angle > 180f - HalfSectorAngle || angle <= -180f + HalfSectorAngle)
                return config.Dodging.BackwardDodge;

            if (angle > -180f + HalfSectorAngle && angle <= -HalfSectorAngle - SectorAngle * 2)
                return config.Dodging.BackwardLeftDodge;

            if (angle > -HalfSectorAngle - SectorAngle * 2 && angle <= -HalfSectorAngle - SectorAngle)
                return config.Dodging.LeftDodge;

            if (angle > -HalfSectorAngle - SectorAngle && angle <= -HalfSectorAngle)
                return config.Dodging.ForwardLeftDodge;

            // 已修复编码乱码的注释。
            return config.Dodging.LeftDodge;
        }

        // 已修复编码乱码的注释。
        private void HandleDodgeEnd()
        {
            _endTimeTriggered = true;

            // 已修复编码乱码的注释。
            if (data.CurrentLocomotionState == LocomotionState.Idle)
            {
                data.NextStatePlayOptions = config.Dodging.FadeInIdleOptions;
                player.StateMachine.ChangeState(player.StateRegistry.GetState<PlayerIdleState>());
            }
            else
            {
                // 已修复编码乱码的注释。
                data.NextStatePlayOptions = config.Dodging.FadeInMoveLoopOptions;
                data.ExpectedFootPhase = _selectedData.EndPhase;
                player.StateMachine.ChangeState(player.StateRegistry.GetState<PlayerMoveLoopState>());
            }
        }
    }
}
