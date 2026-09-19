namespace AwCon
{
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    public class UpperBodyInterruptProcessor
    {
        private readonly BBBCharacterController _player;
        private readonly UpperBodyController _upperBody;

        public UpperBodyInterruptProcessor(BBBCharacterController player, UpperBodyController upperBody)
        {
            _player = player;
            _upperBody = upperBody;
        }

        // 已修复编码乱码的注释。
        // 已修复编码乱码的注释。
        public bool TryProcessInterrupts(UpperBodyBaseState currentState)
        {
            if (_player.Config == null || _player.Config.Brain == null || _player.Config.Brain.UpperBodyInterceptors == null)
                return false;

            var pipeline = _player.Config.Brain.UpperBodyInterceptors;
            for (int i = 0; i < pipeline.Count; i++)
            {
                var interceptor = pipeline[i];
                if (interceptor != null && interceptor.TryIntercept(_player, currentState, out var nextState))
                {
                    _upperBody.StateMachine.ChangeState(nextState);
                    return true;
                }
            }
            return false;
        }
    }
}
