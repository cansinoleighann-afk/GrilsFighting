using Animancer;
using UnityEngine;

namespace AwCon
{
    [CreateAssetMenu(fileName = "New Ranged Weapon", menuName = "AwCon/Items/Weapons/Ranged Weapon")]
    public class RangedWeaponSO : EquippableItemSO
    {
        [Header("武器表现（可在多种枪械间复用）")]
        public WeaponAnimationProfileSO AnimationProfile;
        [Header("瞄准设置")]
        [Tooltip("瞄准动画")]
        public ClipTransition AimAnim;
        public AnimPlayOptions AnimPlayOptions=AnimPlayOptions.UpperBodyDefault;

        [Tooltip("最大弹药量")]
        public int MaxAmmo = 30;

        [Tooltip("射击速率")]
        public float FireRate = 0.1f;

        // 已修复编码乱码的注释。
        // public ClipTransition AimIdleAnim; 
    }
}
