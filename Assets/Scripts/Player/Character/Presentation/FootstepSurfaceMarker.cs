using UnityEngine;

namespace AwCon
{
    /// <summary>
    /// Marks a terrain or collider hierarchy with the surface used for footsteps.
    /// Put it on the collider's root or any parent; the resolver searches upward.
    /// </summary>
    public sealed class FootstepSurfaceMarker : MonoBehaviour
    {
        [Tooltip("Surface material used by the footstep audio resolver.")]
        public FootstepSurfaceType Surface = FootstepSurfaceType.Concrete;
    }
}
