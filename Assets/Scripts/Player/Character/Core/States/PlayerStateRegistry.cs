using System;
using System.Collections.Generic;
using UnityEngine;

namespace AwCon
{
    public class PlayerStateRegistry
    {
        private readonly Dictionary<Type, PlayerBaseState> _states = new Dictionary<Type, PlayerBaseState>();

        // 已修复编码乱码的注释。
        public PlayerBaseState InitialState { get; private set; }

        /// <summary>
        // 已修复编码乱码的注释。
        /// </summary>
        public void InitializeFromBrain(PlayerBrainSO brain, BBBCharacterController player)
        {
            if (brain == null || brain.AvailableStates == null || brain.AvailableStates.Count == 0)
            {
                Debug.LogError("说明");
                return;
            }

            for (int i = 0; i < brain.AvailableStates.Count; i++)
            {
                var stateTypeEnum = brain.AvailableStates[i];

                // 已修复编码乱码的注释。
                PlayerBaseState newState = stateTypeEnum switch
                {
                    PlayerStateType.Idle => new PlayerIdleState(player),
                    PlayerStateType.MoveStartState => new PlayerMoveStartState(player),
                    PlayerStateType.MoveLoopState => new PlayerMoveLoopState(player),
                    PlayerStateType.StopState => new PlayerStopState(player),
                    PlayerStateType.CrouchEnter => new PlayerCrouchEnterState(player),
                    PlayerStateType.CrouchIdle => new PlayerCrouchIdleState(player),
                    PlayerStateType.CrouchMove => new PlayerCrouchMoveState(player),
                    PlayerStateType.CrouchExit => new PlayerCrouchExitState(player),
                    PlayerStateType.Jump => new PlayerJumpState(player),
                    PlayerStateType.DoubleJump => new PlayerDoubleJumpState(player),
                    PlayerStateType.Fall => new PlayerFallState(player),
                    PlayerStateType.Land => new PlayerLandState(player),
                    PlayerStateType.Dodge => new PlayerDodgeState(player),
                    PlayerStateType.Roll => new PlayerRollState(player),
                    PlayerStateType.Vault => new PlayerVaultState(player),
                    PlayerStateType.AimIdle => new PlayerAimIdleState(player),
                    PlayerStateType.AimMove => new PlayerAimMoveState(player),
                    PlayerStateType.Override=> new OverrideState(player),
                    PlayerStateType.Death => new PlayerDeathState(player),
                    _ => null
                };

                if (newState != null)
                {
                    Type type = newState.GetType();
                    if (!_states.ContainsKey(type))
                    {
                        _states.Add(type, newState);
                    }

                    // 已修复编码乱码的注释。
                    // 已修复编码乱码的注释。
                    if (InitialState == null)
                    {
                        InitialState = newState;
                    }
                }
            }

            // Crouch is a complete locomotion branch rather than an optional action state.
            // Register it unconditionally so existing PlayerBrain assets remain backward-compatible.
            RegisterIfMissing(new PlayerCrouchEnterState(player));
            RegisterIfMissing(new PlayerCrouchIdleState(player));
            RegisterIfMissing(new PlayerCrouchMoveState(player));
            RegisterIfMissing(new PlayerCrouchExitState(player));
            // AimInterceptor can be enabled independently of the Brain state's list.
            // Register its states unconditionally so a lock-on input cannot resolve
            // to a null state when older Brain assets omit these enum entries.
            RegisterIfMissing(new PlayerAimIdleState(player));
            RegisterIfMissing(new PlayerAimMoveState(player));

            // 已修复编码乱码的注释。
        }

        private void RegisterIfMissing(PlayerBaseState state)
        {
            if (state == null) return;
            Type type = state.GetType();
            if (!_states.ContainsKey(type)) _states.Add(type, state);
        }

        /// <summary>
        // 已修复编码乱码的注释。
        /// </summary>
        public T GetState<T>() where T : PlayerBaseState
        {
            if (_states.TryGetValue(typeof(T), out var state))
                return state as T;

            Debug.LogError($"说明");
            return null;
        }
    }
}
