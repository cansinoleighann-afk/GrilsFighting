namespace AwCon
{
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    public class UpperBodyUnavailableState : UpperBodyBaseState
    {
        public UpperBodyUnavailableState(BBBCharacterController player) : base(player) { }

        // 已修复编码乱码的注释。
        public override void Enter()
        {
            player.AnimFacade.SetLayerWeight(1, 0f, 0.2f);

            // 已修复编码乱码的注释。
            // 已修复编码乱码的注释。
            // 已修复编码乱码的注释。
            // 已修复编码乱码的注释。
            if (player != null && player.EquipmentDriver != null)
            {
                player.EquipmentDriver.UnequipCurrentItem();
            }
        }

        // 已修复编码乱码的注释。
        public override void Exit()
        {
        }

        // 已修复编码乱码的注释。
        protected override void UpdateStateLogic()
        {
        }
    }
}
