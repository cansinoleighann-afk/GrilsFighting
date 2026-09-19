using UnityEngine;

namespace AwCon
{
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    [CreateAssetMenu(fileName = "ExitUnavailableInterceptor", menuName = "AwCon/Player/Interceptors/UpperBody/ExitUnavailable")]
    public class ExitUnavailableInterceptorSO : UpperBodyInterceptorSO
    {
        public override bool TryIntercept(BBBCharacterController player, UpperBodyBaseState currentState, out UpperBodyBaseState nextState)
        {
            nextState = null;

            // 已修复编码乱码的注释。
            if (currentState == null || currentState is not UpperBodyUnavailableState)
            {
                return false;
            }

            // 已修复编码乱码的注释。
            var playerBaseState = player.StateMachine.CurrentState;

            // 已修复编码乱码的注释。
            if (playerBaseState is PlayerVaultState || playerBaseState is PlayerFallState || playerBaseState is PlayerRollState)
            {
                return false;
            }

            // 已修复编码乱码的注释。
            if (player.RuntimeData != null && player.RuntimeData.CurrentItem != null)
            {
                nextState = player.UpperBodyCtrl.StateRegistry.GetState<UpperBodyHoldItemState>();
            }
            else
            {
                nextState = player.UpperBodyCtrl.StateRegistry.GetState<UpperBodyEmptyState>();
            }

            return true;
        }
    }
}
