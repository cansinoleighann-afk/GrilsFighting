namespace AwCon
{
    public class StateMachine
    {
        public BaseState CurrentState { get; private set; }
        public void Initialize(BaseState startingState)
        {
            CurrentState = startingState;
            CurrentState.Enter();
        }
        public void ChangeState(BaseState newState)
        {
            if (newState == null)
            {
                UnityEngine.Debug.LogError("[StateMachine] 拒绝切换到空状态。请检查状态注册表或拦截器配置。");
                return;
            }

            if (CurrentState != null)
                CurrentState.Exit();
            CurrentState = newState;
            CurrentState.Enter();
        }
    }
}
