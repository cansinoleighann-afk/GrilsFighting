using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AwCon
{
    [CreateAssetMenu(fileName = "New CannonSO", menuName = "AwCon/Items/Weapons/Cannon")]
    public class CannonSO : RangedWeaponSO
    {
        [Header("装备设置")]
        [Tooltip("装备动画结束时间")] public float EquipEndTime = 0.5f;
        [Tooltip("启用IK时间")] public float EnableIKTime = 0.4f;
        [Tooltip("禁用IK时间")] public float DisableIKTime = 0.4f;

        [Header("投射物设置")]
        [Tooltip("投射物预制体")]
        public GameObject ProjectilePrefab;

        [Tooltip("投射物速度")]
        public float ProjectileSpeed = 20f;

        [Header("射击设置")]
        [Tooltip("射击间隔")]
        public float ShootInterval = 0.1f;

        [Tooltip("射击音效")]
        public AudioClip ShootSound;

        [Header("命中设置")]
        [Tooltip("投射物命中音效")]
        public AudioClip ProjectileHitSound;

        [Header("特效设置")]
        [Tooltip("枪口特效预制体")]
        public GameObject MuzzleVFXPrefab;

        [Header("后坐力设置")]
        [Tooltip("后坐力俯仰角")]
        public float RecoilPitchAngle = 2f;

        [Tooltip("后坐力偏航角")]
        public float RecoilYawAngle = 1f;

        [Header("后坐力随机范围")]
        [Tooltip("后坐力俯仰角随机范围")]
        public float RecoilPitchRandomRange = 0.5f;

        [Tooltip("后坐力偏航角随机范围")]
        public float RecoilYawRandomRange = 0.5f;
    }
}