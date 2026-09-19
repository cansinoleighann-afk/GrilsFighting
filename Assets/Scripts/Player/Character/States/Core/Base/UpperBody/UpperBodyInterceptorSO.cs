using UnityEngine;

namespace AwCon
{
    public abstract class UpperBodyInterceptorSO : ScriptableObject
    {
        public abstract bool TryIntercept(BBBCharacterController player, UpperBodyBaseState currentState, out UpperBodyBaseState nextState);
    }
}