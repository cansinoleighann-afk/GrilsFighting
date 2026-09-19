using UnityEngine;

namespace AwCon
{
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    [CreateAssetMenu(fileName = "FallInterceptor", menuName = "AwCon/Player/Interceptors/Fall")]
    public class FallInterceptorSO : StateInterceptorSO
    {
        public override bool TryIntercept(BBBCharacterController player, PlayerBaseState currentState, out PlayerBaseState nextState)
        {
            nextState = null;
            var data = player.RuntimeData;
            var config = player.Config;

            // 已修复编码乱码的注释。
            if (
                data.WantsToFall &&
                data.VerticalVelocity < config.Core.FallVerticalVelocityThreshold &&
                currentState is not PlayerFallState &&
                currentState is not PlayerVaultState)
            {
                data.NextStatePlayOptions = config.LocomotionAnims.FadeInFallOptions;
                nextState = player.StateRegistry.GetState<PlayerFallState>();
                return true;
            }

            return false;
        }
    }
}
