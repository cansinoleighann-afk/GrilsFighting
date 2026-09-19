using UnityEngine;

namespace AwCon
{
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    [CreateAssetMenu(fileName = "LandInterceptor", menuName = "AwCon/Player/Interceptors/Land")]
    public class LandInterceptorSO : StateInterceptorSO
    {
        public override bool TryIntercept(BBBCharacterController player, PlayerBaseState currentState, out PlayerBaseState nextState)
        {
            nextState = null;
            var data = player.RuntimeData;

            // 已修复编码乱码的注释。
            if (data.JustLanded && data.FallHeightLevel > 0 && currentState is not PlayerLandState)
            {
                nextState = player.StateRegistry.GetState<PlayerLandState>();
                return true;
            }

            return false;
        }
    }
}
