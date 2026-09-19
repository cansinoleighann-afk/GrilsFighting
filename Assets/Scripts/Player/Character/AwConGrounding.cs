using RootMotion.FinalIK;
using UnityEngine;

namespace AwCon
{
    [DefaultExecutionOrder(-250)]
    [RequireComponent(typeof(BBBCharacterController), typeof(GrounderFBBIK))]
    public sealed class AwConGrounding : MonoBehaviour
    {
        private BBBCharacterController _player;
        private GrounderFBBIK _grounder;

        private void Awake()
        {
            _player = GetComponent<BBBCharacterController>();
            _grounder = GetComponent<GrounderFBBIK>();
        }

        private void LateUpdate()
        {
            var data = _player.RuntimeData;
            if (data == null) return;
            bool grounded = data.IsGrounded && data.VerticalVelocity <= 0f &&
                            !data.IsWarping && !data.IsVaulting;
            _grounder.weight = Mathf.MoveTowards(_grounder.weight, grounded ? 1f : 0f,
                Time.deltaTime * (grounded ? 6f : 20f));
        }
    }
}
