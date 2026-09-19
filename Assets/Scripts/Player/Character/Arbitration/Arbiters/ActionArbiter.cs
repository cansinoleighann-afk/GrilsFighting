using UnityEngine;

namespace AwCon
{
    /// <summary>
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    /// </summary>
    public class ActionArbiter
    {
        private readonly BBBCharacterController _player;
        private readonly PlayerRuntimeData _data;

        public ActionArbiter(BBBCharacterController player)
        {
            _player = player;
            _data = player.RuntimeData;
        }

        /// <summary>
        // 已修复编码乱码的注释。
        /// </summary>
        public void Arbitrate()
        {
            if (!_data.ActionArbitration.HasRequest) return;

            var request = _data.ActionArbitration.HighestPriorityRequest;
            int currentResistance = GetCurrentOverrideResistance();

            bool isInOverride = _player.StateMachine.CurrentState is OverrideState;

            // 已修复编码乱码的注释。
            if (!isInOverride)
            {
                if (request.Priority <= currentResistance) return;

                _data.Override.IsActive = true;
                _data.Override.Request = request;
                _data.Override.ReturnState = _player.StateMachine.CurrentState;

                var state = _player.StateRegistry.GetState<OverrideState>();
                _player.StateMachine.ChangeState(state);
                return;
            }

            // 已修复编码乱码的注释。
            if (request.Priority < currentResistance) return;

            // 已修复编码乱码的注释。
            if (_data.Override.IsActive && _data.Override.Request.Clip == request.Clip) return;

            _data.Override.IsActive = true;
            _data.Override.Request = request;

            // 已修复编码乱码的注释。
            var overrideState = (OverrideState)_player.StateMachine.CurrentState;
            overrideState.ForceReapply();
        }

        /// <summary>
        // 已修复编码乱码的注释。
        /// </summary>
        private int GetCurrentOverrideResistance()
        {
            var current = _player.StateMachine.CurrentState;

            if (current is OverrideState s)
                return s.CurrentPriority;

            if (current is PlayerRollState) return 100;
            if (current is PlayerDodgeState) return 80;

            return 0;
        }
    }
}
