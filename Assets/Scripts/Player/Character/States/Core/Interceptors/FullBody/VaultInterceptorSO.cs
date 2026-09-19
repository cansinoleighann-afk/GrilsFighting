using UnityEngine;

namespace AwCon
{
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    [CreateAssetMenu(fileName = "VaultInterceptor", menuName = "AwCon/Player/Interceptors/Vault")]
    public class VaultInterceptorSO : StateInterceptorSO
    {
        public override bool TryIntercept(BBBCharacterController player, PlayerBaseState currentState, out PlayerBaseState nextState)
        {
            nextState = null;
            var data = player.RuntimeData;

            // 已修复编码乱码的注释。
            if (data.WantsToVault && currentState is not PlayerVaultState)
            {
                nextState = player.StateRegistry.GetState<PlayerVaultState>();
                return true;
            }

            return false;
        }
    }
}
