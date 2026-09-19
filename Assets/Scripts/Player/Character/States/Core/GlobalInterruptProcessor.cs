namespace AwCon
{
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    public class GlobalInterruptProcessor
    {
        private readonly BBBCharacterController _player;

        public GlobalInterruptProcessor(BBBCharacterController player)
        {
            _player = player;
        }

        // 已修复编码乱码的注释。
        // 已修复编码乱码的注释。
        public bool TryProcessInterrupts(PlayerBaseState currentState)
        {
            // 已修复编码乱码的注释。
            if (_player.Config == null || _player.Config.Brain == null || _player.Config.Brain.GlobalInterceptors == null)
                return false;

            // 已修复编码乱码的注释。
            var pipeline = _player.Config.Brain.GlobalInterceptors;
            for (int i = 0; i < pipeline.Count; i++)
            {
                var interceptor = pipeline[i];
                if (interceptor != null && interceptor.TryIntercept(_player, currentState, out var nextState))
                {
                    _player.StateMachine.ChangeState(nextState);
                    return true;
                }
            }

            return false;
        }
    }
}
