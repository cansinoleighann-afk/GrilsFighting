using UnityEngine;

namespace AwCon
{
    public abstract class StateInterceptorSO : ScriptableObject
    {
        public abstract bool TryIntercept(BBBCharacterController player, PlayerBaseState currentState, out PlayerBaseState nextState);
    }
}