using UnityEngine;

namespace AwCon
{
    /// <summary>
    // 已修复编码乱码的注释。
    /// </summary>
    public abstract class ItemDefinitionSO : ScriptableObject
    {
        [Header("物品基础设置")]
        [Tooltip("物品唯一ID")]
        public string ItemID;

        [Tooltip("物品显示名称")]
        public string DisplayName;

        [Tooltip("物品图标")]
        public Sprite Icon;

        [TextArea(2, 4)]
        [Tooltip("物品描述")]
        public string Description;

        [Tooltip("最大堆叠数量")]
        public int MaxStack = 1;

        // 已修复编码乱码的注释。
        protected virtual void OnValidate()
        {
            if (string.IsNullOrEmpty(ItemID))
            {
                ItemID = System.Guid.NewGuid().ToString();
            }
        }
    }
}