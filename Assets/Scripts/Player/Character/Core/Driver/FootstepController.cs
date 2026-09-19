using UnityEngine;

namespace AwCon
{
    /// <summary>
    /// Converts locomotion animation cycle crossings into terrain-aware footstep events.
    /// This class owns timing and surface lookup; AudioDriver owns playback.
    /// </summary>
    public sealed class FootstepController
    {
        private readonly BBBCharacterController _player;
        private bool _hasPreviousCycle;
        private float _previousCycle;
        private const float StepThreshold = 0.5f;
        private const float GroundProbeDistance = 0.45f;

        public FootstepController(BBBCharacterController player)
        {
            _player = player;
        }

        public void ResetCycle()
        {
            _hasPreviousCycle = false;
            _previousCycle = 0f;
        }

        public void UpdateLoop(float normalizedCycle)
        {
            if (_player == null || _player.RuntimeData == null) return;

            normalizedCycle -= Mathf.Floor(normalizedCycle);
            if (!_hasPreviousCycle)
            {
                _previousCycle = normalizedCycle;
                _hasPreviousCycle = true;
                return;
            }

            bool wrapped = normalizedCycle < _previousCycle - 0.25f;
            bool crossedStep = (_previousCycle < StepThreshold && normalizedCycle >= StepThreshold) ||
                               wrapped;

            _previousCycle = normalizedCycle;
            if (!crossedStep || !_player.RuntimeData.IsGrounded) return;

            var action = _player.RuntimeData.IsCrouchJogging ||
                         _player.RuntimeData.CurrentLocomotionState == LocomotionState.Jog ||
                         _player.RuntimeData.CurrentLocomotionState == LocomotionState.Sprint
                ? FootstepActionType.Run
                : FootstepActionType.Walk;

            _player.AudioDriver?.PlayFootstep(ResolveSurface(), action);
        }

        public FootstepSurfaceType ResolveSurface()
        {
            if (_player == null) return FootstepSurfaceType.Concrete;

            Vector3 origin = _player.transform.position + Vector3.up * 0.15f;
            if (_player.CharController != null)
                origin.y = _player.CharController.bounds.min.y + 0.15f;

            if (Physics.Raycast(origin, Vector3.down, out var hit, GroundProbeDistance,
                                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                var marker = hit.collider.GetComponentInParent<FootstepSurfaceMarker>();
                if (marker != null) return marker.Surface;
            }

            return FootstepSurfaceType.Concrete;
        }
    }
}
