using UnityEngine;

namespace AwCon
{
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    [CreateAssetMenu(fileName = "AimInterceptor", menuName = "AwCon/Player/Interceptors/Aim")]
    public class AimInterceptorSO : StateInterceptorSO
    {
        public override bool TryIntercept(BBBCharacterController player, PlayerBaseState currentState, out PlayerBaseState nextState)
        {
            nextState = null;

            var data = player.RuntimeData;

            // 已修复编码乱码的注释。
            if (data.IsAiming)
            {
                // 已修复编码乱码的注释。
                if (currentState is PlayerAimIdleState || currentState is PlayerAimMoveState)
                    return false;

                // 已修复编码乱码的注释。
                if (currentState is PlayerJumpState ||
                    currentState is PlayerDoubleJumpState ||
                    currentState is PlayerLandState ||
                    currentState is PlayerVaultState)
                    return false;

                // 已修复编码乱码的注释。
                nextState = data.CurrentLocomotionState == LocomotionState.Idle
                    ? player.StateRegistry.GetState<PlayerAimIdleState>()
                    : player.StateRegistry.GetState<PlayerAimMoveState>();

                return true;
            }

            return false;
        }
    }
}
