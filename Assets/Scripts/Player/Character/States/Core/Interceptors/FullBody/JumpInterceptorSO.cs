using UnityEngine;

namespace AwCon
{
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    [CreateAssetMenu(fileName = "JumpInterceptor", menuName = "AwCon/Player/Interceptors/Jump")]
    public class JumpInterceptorSO : StateInterceptorSO
    {
        public override bool TryIntercept(BBBCharacterController player, PlayerBaseState currentState, out PlayerBaseState nextState)
        {
            nextState = null;
            var data = player.RuntimeData;
            var config = player.Config;

            if (data == null || config == null) return false;

            if (!data.WantsToJump) return false;

            if (currentState is PlayerJumpState ||
                currentState is PlayerDoubleJumpState ||
                currentState is PlayerVaultState ||
                currentState is PlayerFallState ||
                currentState is PlayerRollState ||
                currentState is PlayerDodgeState)// 已修复编码乱码的注释。
            {
                return false;
            }

            data.NextStatePlayOptions = config.LocomotionAnims != null
                ? config.LocomotionAnims.FadeInJumpOptions
                : data.NextStatePlayOptions;

            // 已修复编码乱码的注释。
            data.WantsToJump = false;

            nextState = player.StateRegistry.GetState<PlayerJumpState>();
            return nextState != null;
        }
    }
}
