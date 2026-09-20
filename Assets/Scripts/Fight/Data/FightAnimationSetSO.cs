using Animancer;
using UnityEngine;

namespace AwCon.Fight
{
    /// <summary>Animancer transition library for one fighter model.</summary>
    [CreateAssetMenu(fileName = "FightAnimationSet", menuName = "AwCon/Fight/Animation Set")]
    public sealed class FightAnimationSetSO : ScriptableObject
    {
        [Header("Locomotion")]
        public ClipTransition idle;
        public ClipTransition walk;

        [Header("Reactions")]
        public ClipTransition hit;
        public ClipTransition knockdown;
        public ClipTransition standUp;
        public ClipTransition death;

        [Header("Actions")]
        public ClipTransition jump;
        public ClipTransition dodge;
        public ClipTransition guard;
        public ClipTransition grab;
        public ClipTransition grabbed;
    }
}
