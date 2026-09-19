using Animancer;
using UnityEngine;

namespace AwCon
{
    /// <summary>
    // 已修复编码乱码的注释。
    /// </summary>
    public abstract class EquippableItemSO : ItemDefinitionSO
    {
        [Header("装备预制体设置")]
        [Tooltip("装备预制体")]
        public GameObject Prefab;

        public Vector3 HoldPositionOffset;
        public Quaternion HoldRotationOffset;

        [Header("装备动画设置")]
        [Tooltip("装备动画")]
        public ClipTransition EquipAnim;
        public AnimPlayOptions EquipAnimPlayOptions = AnimPlayOptions.UpperBodyDefault;

        [Tooltip("卸下动画")]
        public ClipTransition UnEquipAnim;
        public AnimPlayOptions UnEquipAnimPlayOptions = AnimPlayOptions.UpperBodyDefault;

        [Tooltip("装备待机动画")]
        public ClipTransition EquipIdleAnim;
        public AnimPlayOptions EquipIdleAnimOptions= AnimPlayOptions.UpperBodyDefault;

    }
}