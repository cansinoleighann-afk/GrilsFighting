using System;

namespace AwCon
{
    // 已修复编码乱码的注释。
    public class ItemInstance
    {
        // 已修复编码乱码的注释。
        public string InstanceID { get; private set; }

        // 已修复编码乱码的注释。
        // 已修复编码乱码的注释。
        public ItemDefinitionSO BaseData { get; private set; }

        // 已修复编码乱码的注释。
        // 已修复编码乱码的注释。
        public int CurrentAmount { get; set; }
        public int AmmoInMagazine { get; set; } = -1;
        public int ReserveAmmo { get; set; } = -1;

        // 已修复编码乱码的注释。
        // 已修复编码乱码的注释。
        public ItemInstance(ItemDefinitionSO baseData, int amount = 1)
        {
            // 已修复编码乱码的注释。
            InstanceID = Guid.NewGuid().ToString();
            // 已修复编码乱码的注释。
            BaseData = baseData;
            // 已修复编码乱码的注释。
            CurrentAmount = amount;
        }

        // 已修复编码乱码的注释。
        public T GetSODataAs<T>() where T : ItemDefinitionSO
        {
            // 已修复编码乱码的注释。
            return BaseData as T;
        }
    }
}
