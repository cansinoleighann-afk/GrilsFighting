namespace AwCon
{
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    public class UpperBodyEmptyState : UpperBodyBaseState
    {
        public UpperBodyEmptyState(BBBCharacterController player) : base(player) { }

        // 已修复编码乱码的注释。
        public override void Enter()
        {
            // 已修复编码乱码的注释。
            // 已修复编码乱码的注释。
            player.AnimFacade.SetLayerWeight(1, 0f, 0.25f);
        }

        // 已修复编码乱码的注释。
        public override void Exit()
        {
        }

        // 已修复编码乱码的注释。
        protected override void UpdateStateLogic()
        {
            // 已修复编码乱码的注释。
            if (data.CurrentItem != null)
            {
                player.UpperBodyCtrl.StateMachine.ChangeState(
                    player.UpperBodyCtrl.StateRegistry.GetState<UpperBodyHoldItemState>()
                );
            }
        }
    }
}
