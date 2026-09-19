using UnityEngine;

namespace AwCon
{
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    [CreateAssetMenu(fileName = "EnterUnavailableInterceptor", menuName = "AwCon/Player/Interceptors/UpperBody/EnterUnavailable")]
    public class EnterUnavailableInterceptorSO : UpperBodyInterceptorSO
    {
        public override bool TryIntercept(BBBCharacterController player, UpperBodyBaseState currentState, out UpperBodyBaseState nextState)
        {
            nextState = null;

            // 已修复编码乱码的注释。
            if (currentState != null && currentState is UpperBodyUnavailableState)
            {
                return false;
            }

            // 已修复编码乱码的注释。
            var playerbasestate = player.StateMachine.CurrentState;

            // 已修复编码乱码的注释。
            if (playerbasestate is PlayerVaultState || playerbasestate is PlayerFallState || playerbasestate is PlayerRollState)
            {
                // 已修复编码乱码的注释。
                nextState = player.UpperBodyCtrl.StateRegistry.GetState<UpperBodyUnavailableState>();
                return true;
            }

            return false;
        }
    }
}
