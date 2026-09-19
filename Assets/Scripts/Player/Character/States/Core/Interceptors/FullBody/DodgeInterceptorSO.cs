using UnityEngine;

namespace AwCon
{
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    [CreateAssetMenu(fileName = "DodgeInterceptor", menuName = "AwCon/Player/Interceptors/Dodge")]
    public class DodgeInterceptorSO : StateInterceptorSO
    {
        public override bool TryIntercept(BBBCharacterController player, PlayerBaseState currentState, out PlayerBaseState nextState)
        {
            nextState = null;
            var data = player.RuntimeData;

            // 已修复编码乱码的注释。
            if (data.WantsToDodge)
            {
                // 已修复编码乱码的注释。
                data.NextStatePlayOptions = data.LastLocomotionState == LocomotionState.Sprint ?
                    player.Config.LocomotionAnims.FadeInMoveDodgeOptions :
                    player.Config.LocomotionAnims.FadeInQuickDodgeOptions;

                nextState = player.StateRegistry.GetState<PlayerDodgeState>();
                return true;
            }

            return false;
        }
    }
}
