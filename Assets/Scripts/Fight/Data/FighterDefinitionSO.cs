using UnityEngine;

namespace AwCon.Fight
{
    [CreateAssetMenu(menuName = "AwCon/Fight/Fighter")]
    public sealed class FighterDefinitionSO : ScriptableObject
    {
        public string displayName;
        public GameObject modelPrefab;
        [Min(1)] public float health = 100;
        [Min(0)] public float moveSpeed = 4;
        [Min(0)] public float aiAttackInterval = 1.2f;
        public FightAnimationSetSO animations;

        // Retained only as a source for the one-time project migration. Runtime uses animations.
        [HideInInspector] public AnimationClip idle, walk, hit, knockdown, standUp, death, jump, dodge, guard, grab, grabbed;
        public CombatMoveSO[] punches, kicks;
        public CombatMoveSO jumpAttack, groundAttack, throwAttack;
    }
}
