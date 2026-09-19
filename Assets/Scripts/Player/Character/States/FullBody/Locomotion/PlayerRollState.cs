using UnityEngine;

namespace AwCon
{
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    public class PlayerRollState : PlayerBaseState
    {
        // 已修复编码乱码的注释。
        private WarpedMotionData _selectedData;

        // 已修复编码乱码的注释。
        private float _stateDuration;
        private bool _endTimeTriggered;

        public PlayerRollState(BBBCharacterController player) : base(player) { }

        // 已修复编码乱码的注释。
        protected override bool CheckInterrupts() => false;

        // 已修复编码乱码的注释。
        public override void Enter()
        {
            data.WantsToRoll = false;

            // 已修复编码乱码的注释。
            data.SfxQueue.Enqueue(PlayerSfxEvent.Roll);

            _stateDuration = 0f;
            _endTimeTriggered = false;

            // 已修复编码乱码的注释。
            _selectedData = GetRollData();

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
                HandleRollEnd();
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

            if (!_endTimeTriggered && _selectedData.EndTime > 0f && _stateDuration >= _selectedData.EndTime && data.CurrentLocomotionState != LocomotionState.Idle)
            {
                _endTimeTriggered = true;
                HandleRollEnd();
                return;
            }
        }

        // 已修复编码乱码的注释。
        public override void Exit()
        {
            data.WantsToRoll = false;

            player.MotionDriver.ClearWarpData();

            player.AnimFacade.ClearOnEndCallback();

            _selectedData = null;
        }

        // 已修复编码乱码的注释。
        private WarpedMotionData GetRollData()
        {
            float angle = data.DesiredLocalMoveAngle;

            const float SectorAngle = 45f;
            const float HalfSectorAngle = 22.5f;

            // 已修复编码乱码的注释。
            if (angle > -HalfSectorAngle && angle <= HalfSectorAngle)
                return config.Rolling.ForwardRoll;

            if (angle > HalfSectorAngle && angle <= HalfSectorAngle + SectorAngle)
                return config.Rolling.ForwardRightRoll;

            if (angle > HalfSectorAngle + SectorAngle && angle <= HalfSectorAngle + SectorAngle * 2)
                return config.Rolling.RightRoll;

            if (angle > HalfSectorAngle + SectorAngle * 2 && angle <= 180f - HalfSectorAngle)
                return config.Rolling.BackwardRightRoll;

            if (angle > 180f - HalfSectorAngle || angle <= -180f + HalfSectorAngle)
                return config.Rolling.BackwardRoll;

            if (angle > -180f + HalfSectorAngle && angle <= -HalfSectorAngle - SectorAngle * 2)
                return config.Rolling.BackwardLeftRoll;

            if (angle > -HalfSectorAngle - SectorAngle * 2 && angle <= -HalfSectorAngle - SectorAngle)
                return config.Rolling.LeftRoll;

            if (angle > -HalfSectorAngle - SectorAngle && angle <= -HalfSectorAngle)
                return config.Rolling.ForwardLeftRoll;

            // 已修复编码乱码的注释。
            return config.Rolling.LeftRoll;
        }

        // 已修复编码乱码的注释。
        private void HandleRollEnd()
        {
            _endTimeTriggered = true;

            // 已修复编码乱码的注释。
            if (data.CurrentLocomotionState == LocomotionState.Idle)
            {
                data.NextStatePlayOptions = config.Rolling.FadeInIdleOptions;
                player.StateMachine.ChangeState(player.StateRegistry.GetState<PlayerIdleState>());
            }
            else
            {
                // 已修复编码乱码的注释。
                data.NextStatePlayOptions = config.Rolling.FadeInMoveLoopOptions;
                data.ExpectedFootPhase = _selectedData.EndPhase;
                player.StateMachine.ChangeState(player.StateRegistry.GetState<PlayerMoveLoopState>());
            }
        }
    }
}
