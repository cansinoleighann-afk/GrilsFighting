using System.Collections;
using System.Collections.Generic;
using Animancer;
using UnityEngine;

namespace AwCon
{
    [CreateAssetMenu(fileName = "New SwordSO", menuName = "AwCon/Items/Weapons/Sword")]
    public class SwordSO : MeleeWeaponSO
    {
        [Header("攻击设置")]
        [Tooltip("攻击动作请求")]
        public ActionRequest AttackRequest;

        [Header("音效设置")]
        [Tooltip("挥砍音效")]
        public AudioClip SwingSound;

        [Tooltip("命中音效")]
        public AudioClip HitSound;

        [Header("伤害设置")]
        [Tooltip("攻击伤害值")]
        public float AttackDamage = 10f;
    }
}