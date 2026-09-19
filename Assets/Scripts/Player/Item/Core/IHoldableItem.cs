namespace AwCon
{
    /// <summary>
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    /// </summary>
    public interface IHoldableItem
    {
        /// <summary>
        // 已修复编码乱码的注释。
        /// </summary>
        void Initialize(ItemInstance instanceData);

        /// <summary>
        // 已修复编码乱码的注释。
        /// </summary>
        void OnEquipEnter(BBBCharacterController player);

        /// <summary>
        // 已修复编码乱码的注释。
        /// </summary>
        void OnUpdateLogic();

        /// <summary>
        // 已修复编码乱码的注释。
        /// </summary>
        void OnForceUnequip();
    }
}
