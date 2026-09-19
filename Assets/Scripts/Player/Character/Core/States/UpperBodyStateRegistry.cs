using System;
using System.Collections.Generic;
using UnityEngine;

namespace AwCon
{
    /// <summary>
    // 已修复编码乱码的注释。
    /// </summary>
    public class UpperBodyStateRegistry
    {
        private readonly Dictionary<Type, UpperBodyBaseState> _states = new Dictionary<Type, UpperBodyBaseState>();
        public UpperBodyBaseState InitialState { get; private set; }

        public void InitializeFromBrain(PlayerBrainSO brain, BBBCharacterController player)
        {
            if (brain == null || brain.UpperBodyStates == null || brain.UpperBodyStates.Count == 0)
            {
                Debug.LogWarning("说明");
                return;
            }

            for (int i = 0; i < brain.UpperBodyStates.Count; i++)
            {
                var stateTypeEnum = brain.UpperBodyStates[i];

                UpperBodyBaseState newState = stateTypeEnum switch
                {
                    UpperBodyStateType.EmptyHands => new UpperBodyEmptyState(player),
                    UpperBodyStateType.HoldItem => new UpperBodyHoldItemState(player),
                    UpperBodyStateType.Unavailable => new UpperBodyUnavailableState(player),
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
                    if (InitialState == null)
                    {
                        InitialState = newState;
                    }
                }
            }
        }

        public T GetState<T>() where T : UpperBodyBaseState
        {
            if (_states.TryGetValue(typeof(T), out var state))
                return state as T;

            return null;
        }
    }
}
