using Animancer;
using UnityEngine;

namespace AwCon
{
    /// <summary>
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    /// </summary>
    public abstract class MeleeWeaponSO : EquippableItemSO
    {
        [Header("装备设置")]
        [Tooltip("装备动画结束时间")]
        public float EquipEndTime = 0.5f;

        [Header("攻击设置")]
        [Tooltip("攻击冷却时间")]
        public float AttackCooldown = 0.5f;

        [Tooltip("是否启用IK")]
        public bool EnableIK = false;
    }
}