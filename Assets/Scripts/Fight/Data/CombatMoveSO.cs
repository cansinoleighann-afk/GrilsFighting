using Animancer;
using UnityEngine;

namespace AwCon.Fight
{
    public enum FightAction { None, Punch, Kick, Jump, Dodge, Guard, Grab, Interact, Throw, Power }
    public enum FightState { Idle, Move, Attack, Jump, Dodge, Guard, Grab, Grabbed, Hit, Knockdown, Dead }

    [CreateAssetMenu(menuName = "AwCon/Fight/Combat Move")]
    public sealed class CombatMoveSO : ScriptableObject
    {
        [Header("Animancer")]
        public ClipTransition transition;

        // Retained only as a source for the one-time project migration. Runtime uses transition.
        [HideInInspector] public AnimationClip clip;
        [Min(0.05f)] public float speed = 1;
        [Min(0)] public float fade = 0.12f;
        [Min(0)] public float damage = 12;
        [Range(0, 1)] public float hitStart = 0.25f;
        [Range(0, 1)] public float hitEnd = 0.55f;
        [Range(0, 1)] public float comboStart = 0.55f;
        [Min(0.1f)] public float reach = 1.7f;
        [Min(0.1f)] public float radius = 0.75f;
        [Min(0)] public float knockback = 2;
        public bool knockdown;
        public bool unblockable;
        public AudioClip sound;
        public GameObject hitEffect;
        public float Duration => transition != null && transition.Clip != null
            ? transition.Clip.length / Mathf.Max(0.05f, speed)
            : 0.5f;
        private void OnValidate() { hitEnd = Mathf.Max(hitStart, hitEnd); }
    }
}
