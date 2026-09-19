using System.Collections.Generic;
using UnityEngine;

namespace AwCon
{
    [CreateAssetMenu(fileName = "PlayerBrain_Default", menuName = "AwCon/Player/Modules/Player Brain")]
    public class PlayerBrainSO : ScriptableObject
    {
        [Header("可用状态列表")]
        [Tooltip("玩家可用状态类型列表")]
        public List<PlayerStateType> AvailableStates = new List<PlayerStateType>();

        [Header("全局拦截器")]
        [Tooltip("全局状态拦截器列表")]
        public List<StateInterceptorSO> GlobalInterceptors = new List<StateInterceptorSO>();

        [Header("上半身状态")]
        [Tooltip("上半身状态类型列表")]
        public List<UpperBodyStateType> UpperBodyStates = new List<UpperBodyStateType>();

        [Tooltip("上半身状态拦截器列表")]
        public List<UpperBodyInterceptorSO> UpperBodyInterceptors = new List<UpperBodyInterceptorSO>();

    }
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。

}