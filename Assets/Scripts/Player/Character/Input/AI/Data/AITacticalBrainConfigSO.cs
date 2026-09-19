using UnityEngine;

namespace AwCon
{
    [CreateAssetMenu(fileName = "AITacticalConfig_", menuName = "AwCon/AI/Tactical Brain Config")]
    public class AITacticalBrainConfigSO : ScriptableObject
    {
        [Header("战斗范围设置")]
        [SerializeField] public float EngagementRange = 15f;
        [SerializeField] public float AttackRange = 2f;          

        [Header("跳跃设置")]
        [SerializeField] public float JumpCooldown = 1.5f;
        [SerializeField] public float DoubleJumpDelay = 0.35f;

        [Header("闪避设置")]
        [Tooltip("闪避触发范围")]
        [SerializeField] public float DodgeTriggerRange = 8f;
        [Tooltip("闪避冷却时间")]
        [SerializeField] public float DodgeCooldown = 2.5f;
        [Tooltip("闪避概率")]
        [Range(0f, 1f)]
        [SerializeField] public float DodgeChance = 0.4f;
        [Tooltip("闪避最大尝试次数")]
        [SerializeField] public int DodgeMaxAttempts = 3;

        [Header("翻滚设置")]
        [Tooltip("翻滚触发范围")]
        [SerializeField] public float RollTriggerRange = 6f;
        [Tooltip("翻滚冷却时间")]
        [SerializeField] public float RollCooldown = 3f;
        [Tooltip("翻滚概率")]
        [Range(0f, 1f)]
        [SerializeField] public float RollChance = 0.25f;
        [Tooltip("翻滚最大尝试次数")]
        [SerializeField] public int RollMaxAttempts = 2;

        [Header("侧移设置")]
        [Tooltip("侧移最小冷却时间")]
        [SerializeField] public float StrafeCooldownMin = 1.5f;
        [Tooltip("侧移最大冷却时间")]
        [SerializeField] public float StrafeCooldownMax = 3.5f;
    }
}