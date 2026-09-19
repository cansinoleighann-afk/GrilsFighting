using Animancer;

namespace AwCon
{
    /// <summary>Plays the standing-to-crouch transition before entering the crouch locomotion branch.</summary>
    public sealed class PlayerCrouchEnterState : PlayerBaseState
    {
        public PlayerCrouchEnterState(BBBCharacterController player) : base(player) { }

        public override void Enter()
        {
            var clip = config.LocomotionAnims.IdleToCrouch;
            if (clip == null)
            {
                Advance();
                return;
            }

            ChooseOptionsAndPlay(clip);
            AnimFacade.SetOnEndCallback(Advance);
        }

        protected override void UpdateStateLogic() { }
        public override void PhysicsUpdate() => player.MotionDriver.UpdateMotion();
        public override void Exit() => AnimFacade.ClearOnEndCallback();

        private void Advance()
        {
            player.StateMachine.ChangeState(data.IsCrouching
                ? player.StateRegistry.GetState<PlayerCrouchIdleState>()
                : player.StateRegistry.GetState<PlayerCrouchExitState>());
        }
    }

    /// <summary>Owns the crouched stationary pose and never routes through the normal stop state.</summary>
    public sealed class PlayerCrouchIdleState : PlayerBaseState
    {
        public PlayerCrouchIdleState(BBBCharacterController player) : base(player) { }

        public override void Enter()
        {
            ChooseOptionsAndPlay(config.LocomotionAnims.CrouchIdle ?? config.LocomotionAnims.IdleAnim);
        }

        protected override void UpdateStateLogic()
        {
            if (!data.IsCrouching)
            {
                player.StateMachine.ChangeState(player.StateRegistry.GetState<PlayerCrouchExitState>());
                return;
            }

            if (data.CurrentLocomotionState != LocomotionState.Idle)
            {
                player.StateMachine.ChangeState(player.StateRegistry.GetState<PlayerCrouchMoveState>());
            }
        }

        public override void PhysicsUpdate() => player.MotionDriver.UpdateMotion();
        public override void Exit() => AnimFacade.ClearOnEndCallback();
    }

    /// <summary>Owns crouch walk and crouch jog loops; releasing movement returns directly to CrouchIdle.</summary>
    public sealed class PlayerCrouchMoveState : PlayerBaseState
    {
        private bool _wasJogging;
        private DesiredDirection _direction;
        private const float FadeTime = 0.2f;

        public PlayerCrouchMoveState(BBBCharacterController player) : base(player) { }

        public override void Enter()
        {
            player.FootstepController?.ResetCycle();
            _wasJogging = data.IsCrouchJogging;
            _direction = data.QuantizedDirection;
            ChooseOptionsAndPlay(SelectClip());
        }

        protected override void UpdateStateLogic()
        {
            if (!data.IsCrouching)
            {
                player.StateMachine.ChangeState(player.StateRegistry.GetState<PlayerCrouchExitState>());
                return;
            }

            if (data.CurrentLocomotionState == LocomotionState.Idle)
            {
                player.StateMachine.ChangeState(player.StateRegistry.GetState<PlayerCrouchIdleState>());
                return;
            }

            if (_wasJogging != data.IsCrouchJogging || _direction != data.QuantizedDirection)
            {
                _wasJogging = data.IsCrouchJogging;
                _direction = data.QuantizedDirection;
                var options = AnimPlayOptions.Default;
                options.FadeDuration = FadeTime;
                AnimFacade.PlayTransition(SelectClip(), options);
            }

            float cycle = AnimFacade.CurrentNormalizedTime;
            cycle -= UnityEngine.Mathf.Floor(cycle);
            player.FootstepController?.UpdateLoop(cycle);
        }

        public override void PhysicsUpdate() => player.MotionDriver.UpdateLocomotionFromInput();
        public override void Exit() => AnimFacade.ClearOnEndCallback();

        private ClipTransition SelectClip()
        {
            var a = config.LocomotionAnims;
            return (data.IsCrouchJogging, data.QuantizedDirection) switch
            {
                (false, DesiredDirection.Backward) => a.CrouchWalkBack,
                (false, DesiredDirection.Left) => a.CrouchWalkLeft,
                (false, DesiredDirection.Right) => a.CrouchWalkRight,
                (false, DesiredDirection.ForwardLeft) => a.CrouchWalkFwdLeft,
                (false, DesiredDirection.ForwardRight) => a.CrouchWalkFwdRight,
                (false, DesiredDirection.BackwardLeft) => a.CrouchWalkBackLeft,
                (false, DesiredDirection.BackwardRight) => a.CrouchWalkBackRight,
                (false, _) => a.CrouchWalkFwd,
                (true, DesiredDirection.Backward) => a.CrouchJogBack,
                (true, DesiredDirection.Left) => a.CrouchJogLeft,
                (true, DesiredDirection.Right) => a.CrouchJogRight,
                (true, DesiredDirection.ForwardLeft) => a.CrouchJogFwdLeft,
                (true, DesiredDirection.ForwardRight) => a.CrouchJogFwdRight,
                (true, DesiredDirection.BackwardLeft) => a.CrouchJogBackLeft,
                (true, DesiredDirection.BackwardRight) => a.CrouchJogBackRight,
                (true, _) => a.CrouchJogFwd,
            };
        }
    }

    /// <summary>
    /// Plays the crouch-to-standing transition. Movement can interrupt it immediately so
    /// the character blends straight into the normal locomotion loop instead of waiting
    /// for the entire standing animation to finish.
    /// </summary>
    public sealed class PlayerCrouchExitState : PlayerBaseState
    {
        public PlayerCrouchExitState(BBBCharacterController player) : base(player) { }

        public override void Enter()
        {
            var clip = config.LocomotionAnims.CrouchToIdle;
            if (clip == null)
            {
                Advance();
                return;
            }

            ChooseOptionsAndPlay(clip);
            AnimFacade.SetOnEndCallback(Advance);
        }

        protected override void UpdateStateLogic()
        {
            // C was pressed again during the exit transition: reverse back into crouching.
            if (data.IsCrouching)
            {
                player.StateMachine.ChangeState(player.StateRegistry.GetState<PlayerCrouchEnterState>());
                return;
            }

            // Do not make movement wait for CrouchToIdle to finish. The normal loop picks
            // Jog/Sprint from CurrentLocomotionState and blends from the current transition.
            if (data.CurrentLocomotionState != LocomotionState.Idle)
            {
                data.NextStatePlayOptions = new AnimPlayOptions { FadeDuration = 0.15f };
                player.StateMachine.ChangeState(player.StateRegistry.GetState<PlayerMoveLoopState>());
            }
        }

        public override void PhysicsUpdate() => player.MotionDriver.UpdateLocomotionFromInput();
        public override void Exit() => AnimFacade.ClearOnEndCallback();

        private void Advance()
        {
            player.StateMachine.ChangeState(data.IsCrouching
                ? player.StateRegistry.GetState<PlayerCrouchEnterState>()
                : player.StateRegistry.GetState<PlayerIdleState>());
        }
    }
}
