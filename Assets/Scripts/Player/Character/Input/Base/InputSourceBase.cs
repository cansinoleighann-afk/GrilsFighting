using UnityEngine;

namespace AwCon
{
    /// <summary>
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    /// </summary>
    public abstract class InputSourceBase : MonoBehaviour, IInputSource
    {
        [Header("输入缓冲设置")]
        [Tooltip("输入抖动缓冲时间")]
        public float InputFlickerBuffer = 0.05f;

        [Tooltip("动作缓冲时间")]
        public float ActionBufferTime = 0.2f;

        protected PlayerRuntimeData _runtimeData;

        protected virtual void Awake()
        {
            var player = GetComponentInParent<BBBCharacterController>();
            if (player != null) _runtimeData = player.RuntimeData;
        }

        /// <summary>
        // 已修复编码乱码的注释。
        /// </summary>
        // 已修复编码乱码的注释。
        public abstract void FetchRawInput(ref RawInputData rawData);

        public bool IsBlocked => _runtimeData != null && _runtimeData.Arbitration.BlockInput;
    }
}